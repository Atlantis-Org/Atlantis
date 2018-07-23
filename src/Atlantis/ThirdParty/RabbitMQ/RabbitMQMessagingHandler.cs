using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using RabbitMQ.Client.Events;
using Followme.AspNet.Core.FastCommon.Commanding;
using Followme.AspNet.Core.FastCommon.Infrastructure;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ
{
    public abstract class RabbitMQMessagingHandler<TMessage> : MessageServicer, IRabbitMessagingHandler<TMessage> where TMessage : IRabbitMessaging
    {
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerialzer;
        private readonly IRepositoryContextManager _repositoryContextManager;
        private readonly ICurrentRepositoryContextProvider _currentRepositoryContextProvider;
        private readonly ICommandServicerDelegateFactory _commandDelegateFactory;
        private readonly ICommandHandler _commandHandler;

        public RabbitMQMessagingHandler()
        {
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(GetType());
            _jsonSerialzer = ObjectContainer.Resolve<IJsonSerializer>();
            _repositoryContextManager = ObjectContainer.Resolve<IRepositoryContextManager>();
            _currentRepositoryContextProvider = ObjectContainer.Resolve<ICurrentRepositoryContextProvider>();
            _commandDelegateFactory=ObjectContainer.Resolve<ICommandServicerDelegateFactory>();
            _commandHandler=ObjectContainer.Resolve<ICommandHandler>();
        }

        public abstract string Queue { get; }

        public abstract string Exchange { get; }

        public virtual string RoutingKey { get; set; }

        public virtual string VirtualHost { get; set; }

        public virtual bool IsEnable => true;

        public virtual string Name { get; set; }

        public async Task Handle(RabbitConnection connection, object model, BasicDeliverEventArgs e)
        {
            TMessage message;
            try
            {
                message = Deserialize(e.Body);
                if (message == null)
                {
                    _logger.Warn($"队列：{Queue}，无法序列化消息，已跳过该条消息！消息数据：{GetErrorByteBody(e.Body)}");
                    connection.AckMessage(e.DeliveryTag);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"队列：{Queue}，无法序列化消息，已跳过该条消息！",ex);
                connection.AckMessage(e.DeliveryTag);
                return;
            }

            try
            {
                _logger.Info($"队列：{Queue}，接收到MQ消息：{_jsonSerialzer.Serialize(message)}");
                _repositoryContextManager.Create();
                if (_logger.IsDebugEnabled) _logger.Debug($"创建新Context成功，ContextId: {_currentRepositoryContextProvider.Current.Id}");
                await Handle(message);
                _currentRepositoryContextProvider.Current.Commit();
                _currentRepositoryContextProvider.Current.Dispose();
                if (_logger.IsDebugEnabled) _logger.Debug($"Context执行结束，ContextId: {_currentRepositoryContextProvider.Current.Id}");
                CurrentRepositoryContextProvider.ReleaseCurrentContext(); 
                connection.AckMessage(e.DeliveryTag);
            }
            catch (Exception ex)
            {
                if (_currentRepositoryContextProvider.Current != null)
                {
                    _currentRepositoryContextProvider.Current.Dispose();
                    CurrentRepositoryContextProvider.ReleaseCurrentContext();
                }

                var isEat=EnsureException.EatException(ref ex);
                if (isEat)
                {
                    _logger.Warn($"队列：{Queue} 处理消息失败，消息体：{_jsonSerialzer.Serialize(message)}，原因：{ex.Message}，trace：{ex.GetCloseTrace()}");
                    connection.AckMessage(e.DeliveryTag);
                }
                else
                {
                    _logger.Fatal($"系统在执行{typeof(TMessage)}消息的时候出现异常！原因：{ex.GetExceptionMessage()}", ex);
                    Thread.Sleep(new Random().Next(1000, 30000));
                    connection.RejectMessage(e.DeliveryTag);
                }
            }
        }

        protected abstract Task Handle(TMessage message);

        protected virtual TMessage Deserialize(byte[] messageBody)
        {
            using (MemoryStream ms = new MemoryStream(messageBody))
            {
                ms.Position = 0;
                TMessage t = Serializer.Deserialize<TMessage>(ms);
                return t;
            }
        }

        protected virtual TMessage DeserializeByJson(byte[] messageBody)
        {
            return _jsonSerialzer.Deserialize<TMessage>(Encoding.UTF8.GetString(messageBody));
        }

        protected async Task<TResult> RunCommandAsync<TMsg, TResult>(TMsg message, bool isImplContext = true)
            where TMsg : BaseMessage
            where TResult : MessageResult, new()
        {
            if (isImplContext) return await _commandDelegateFactory.GetHandleDelegate<TMsg, TResult>(message).Invoke(message);
            else return await _commandHandler.Handle<TMsg, TResult>(message);
        }

        protected async Task RunCommandAsync<TMsg>(TMsg message, bool isImplContext = true) where TMsg : BaseMessage
        {
            if (isImplContext) await _commandDelegateFactory.GetHandleDelegate<TMsg>(message).Invoke(message);
            else await _commandHandler.Handle(message);
        }

        private string GetErrorByteBody(byte[] body)
        {
            if (body == null || body.Length == 0) return string.Empty;
            var str = new StringBuilder();
            foreach (var item in body)
            {
                str.Append($"{str},");
            }
            return str.ToString();
        }
    }
}
