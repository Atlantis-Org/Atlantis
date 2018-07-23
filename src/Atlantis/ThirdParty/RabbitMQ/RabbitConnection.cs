using Followme.AspNet.Core.FastCommon.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.Components;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ
{
    public class RabbitConnection
    {
        private readonly RabbitServerSetting _setting;
        private readonly Func<RabbitConnection, object, BasicDeliverEventArgs,Task> _receiveAction;
        private IConnection _connection;
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private IModel _receiveChannel;
        private readonly string _receiveQueue;
        private readonly string _receiveExchange;
        private readonly string _routingKey;

        private int _connecting = 0;
        private int _reconnectTimes=1;

        public RabbitConnection(string receiveQueue, string receiveExchange,string routingKey, RabbitServerSetting setting, Func<RabbitConnection, object, BasicDeliverEventArgs,Task> receiveAction)
        {
            _receiveQueue = receiveQueue ?? throw new ArgumentNullException("The queue must be declare!");
            _receiveExchange = receiveExchange ?? throw new ArgumentNullException("The exchange must be declare!");
            _setting = setting ?? throw new ArgumentNullException("The rabbitmq host setting must be declare!");
            _receiveAction = receiveAction ?? throw new ArgumentNullException("The receive action must be declare!");
            _routingKey=routingKey??"#";

            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create<RabbitConnection>();
            _jsonSerializer = ObjectContainer.Resolve<IJsonSerializer>();
        }

        public IConnection Connection => _connection;

        public string ReceiveQueue => _receiveQueue;

        public string ReceiveExchange => _receiveExchange;

        public string RoutingKey=>_routingKey;

        public IModel ReceiveChannel => _receiveChannel;

        public void Start(bool isAutoConnect=true)
        {
            try
            {
                _connection = RabbitMQUtils.CreateNewConnection(_setting);
                Binding();
            }
            catch (Exception ex)
            {
                _logger.Error($"The rabbit mq service cannot connect! reason: {ex.GetExceptionMessage()}");
                if (isAutoConnect)
                {
                    _logger.Info($"Auto connect is enable, the service will reconnect to the rabbit mq server after {_setting.ReconnectTimeMillisecond} ms!");
                    TryReConnect();
                }
            }
        }

        public void RejectMessage(ulong deliveryTag)
        {
            if (!_connection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message reject failed! message deliverytag: {deliveryTag}");
            _receiveChannel.BasicReject(deliveryTag, true);
        }

        public void AckMessage(ulong deliveryTag)
        {
            if (!_connection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message ack failed! message deliverytag: {deliveryTag}");
            _receiveChannel.BasicAck(deliveryTag, false);
        }

        #region Reconnect
        private void TryReConnect()
        {
            if (!EnterReConnect()) return;
            Thread.Sleep(_setting.ReconnectTimeMillisecond);
            try
            {
                _logger.Info($"Try reconnect to server! server info: ip={_setting.Host}, port={_setting.Port}, virtual host={_setting.VirtualHost}, username={_setting.UserName}, password={_setting.Password}");
                ReConnect();
            }
            catch(Exception ex)
            {
                _logger.Error($"The connection connect failed! the reason is: {ex.Message}");
                ExitReConnect();
                var sleepingTime=_setting.ReconnectTimeMillisecond*_reconnectTimes*_reconnectTimes;
                _logger.Info($"The connection reconnect to server after {sleepingTime/1000} s!");
                Thread.Sleep(sleepingTime);
                if(_reconnectTimes<=30)_reconnectTimes++;
                TryReConnect();
                return;
            }
            ExitReConnect();

        }

       private void ReConnect()
        {
            _connection = RabbitMQUtils.CreateNewConnection(_setting);
            Binding();
            _reconnectTimes=1;
        }

        private bool EnterReConnect()
        {
            return Interlocked.CompareExchange(ref _connecting, 1, 0) == 0;
        }

        private void ExitReConnect()
        {
            Interlocked.Exchange(ref _connecting, 0);
        }
        #endregion

        private void Binding()
        {
            try
            {
                _receiveChannel = _connection.CreateModel();
                _receiveChannel.QueueDeclare(_receiveQueue, true, false, false, null);
                _receiveChannel.QueueBind(_receiveQueue, _receiveExchange, _routingKey, null);
                _receiveChannel.BasicQos(0, 1, false);
                var consume = new EventingBasicConsumer(_receiveChannel);
                consume.Received +=(model, e) => {  _receiveAction(this, model, e); };
                _receiveChannel.BasicConsume(_receiveQueue, false, consume);
                _logger.Info($"The channel binding success! queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! ");
            }
            catch(Exception ex)
            {
                _logger.Error($"Can not binding queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! reason: {ex.Message}");
                throw new InvalidOperationException($"Can not binding queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! reason: {ex.Message}");
            }
        }

    }
}
