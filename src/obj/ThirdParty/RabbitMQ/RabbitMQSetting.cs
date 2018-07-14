using System;
using System.Collections.Generic;
using System.Linq;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ
{
    public class RabbitMQSetting
    {
        private List<RabbitServerSetting> _servers;
        
        public RabbitServerSetting[] Servers{get;set;}

        public RabbitQueueSetting[] Queues { get; set; }

        public RabbitServerSetting DefaultServer{get;set;}

        public RabbitServerSetting GetServer(string name)
        {
            if(string.IsNullOrWhiteSpace(name))return null;
            if(Servers==null||Servers.Length==0)return null;
            if(_servers==null)DecodeServers();
            var server = _servers.FirstOrDefault(p=>p.TypeName==name);
            return server;
        }

        private void DecodeServers()
        {
            _servers=new List<RabbitServerSetting>();
            foreach(var item in Servers)
            {
                if(!item.TypeName.Contains(";"))
                {
                    if(item.TypeName.Equals(RabbitServerSetting.Public_Server_Key))DefaultServer=item;
                    _servers.Add(item);
                    continue;
                }
                
                var decodeServers=DecodeServer(item);
                if(decodeServers==null||decodeServers.Count==0)continue;
                _servers.AddRange(decodeServers);
            }
        }

        private List<RabbitServerSetting> DecodeServer(RabbitServerSetting serverSetting)
        {
            if(serverSetting==null)return null;
            var servers=new List<RabbitServerSetting>();
            foreach(var item in serverSetting.TypeName.Split(';'))
            {
                var server=serverSetting.Clone();
                server.TypeName=item;
                servers.Add(server);
                if(server.TypeName.Equals(RabbitServerSetting.Public_Server_Key))DefaultServer=server;
            }
            return servers;
        }
    }

    public class RabbitServerSetting
    {
        public const string Public_Server_Key="Public";
        
        public string TypeName{get;set;}

        public string Host { get; set; }

        public int Port { get; set; }

        public string VirtualHost { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public ushort RequestedHeartbeat => 60;

        /// <summary>
        /// 等待重连周期（单位：ms）
        /// </summary>
        public int ReconnectTimeMillisecond = 6000;

        public RabbitServerSetting Clone()
        {
            return new RabbitServerSetting()
            {
                TypeName=TypeName,
                Host=Host,
                Port=Port,
                VirtualHost=VirtualHost,
                UserName=UserName,
                Password=Password,
            };
        }
    }

    public class RabbitQueueSetting
    {
        public const string SerializeType_Json="Json";
        public const string SerializeType_Proto="Proto";
        
        private QueueInfo _queueInfo;
        private string _serializeType;
        
        public string MsgName { get; set; }
        
        public string ServerType{get;set;}

        public string Queue { get; set; }

        public string SerializeType
        {
            get
            {
                return _serializeType;
            }
            set
            {
                if(string.IsNullOrWhiteSpace(value))_serializeType=SerializeType_Proto;
                else _serializeType=value;
            }
        }

        public QueueInfo QueueBaseInfo
        {
            get
            {
                if(_queueInfo==null)_queueInfo=DecodeQueue();
                return _queueInfo;
            }
        }

        private QueueInfo DecodeQueue()
        {
            if(string.IsNullOrWhiteSpace(Queue))throw new ArgumentNullException($@"The rabbit mq queue info is null, please set it at Queues[].Queue({MsgName}) section! , 
the section same is: 'QueueName;RoutingKey;Exchange'");
            var queueInfoStrArr=Queue.Split(';');
            if(queueInfoStrArr==null||queueInfoStrArr.Length!=3)throw new ArgumentNullException($@"The rabbit mq queue info has setting wrong, please check it at Queues[].Queue({MsgName}) section! 
the section same is: 'QueueName;RoutingKey;Exchange'");
            return new QueueInfo(queueInfoStrArr[0],queueInfoStrArr[1],queueInfoStrArr[3],this);
        }
            
        public class QueueInfo
        {
            public QueueInfo(string queueName,string routingKey,string exchange,RabbitQueueSetting queueSetting)
            {
                QueueName=queueName??throw new ArgumentNullException($"The queue name value cannot be null! the queue({queueSetting.MsgName})");
                RoutingKey=routingKey??throw new ArgumentNullException($"The queue routingkey value cannot be null! the queue({queueSetting.MsgName})");
                Exchange=exchange??throw new ArgumentNullException($"The queue exchange value cannot be null! the queue({queueSetting.MsgName})");
            }
            
            public string QueueName { get; set; }

            public string RoutingKey{get;set;}

            public string Exchange { get; set; }
        }
    }
}
