using RabbitMQ.Client;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ
{
    public class RabbitMQUtils
    {
        public static IConnection CreateNewConnection(RabbitServerSetting setting)
        {
            var factory = new ConnectionFactory()
            {
                HostName = setting.Host,
                UserName = setting.UserName,
                Password = setting.Password,
                RequestedHeartbeat = setting.RequestedHeartbeat,
                AutomaticRecoveryEnabled = true
            };
            if (!string.IsNullOrWhiteSpace(setting.VirtualHost)) factory.VirtualHost = setting.VirtualHost;
            if (setting.Port > 0) factory.Port = setting.Port;
            return factory.CreateConnection();
        }
    }
}
