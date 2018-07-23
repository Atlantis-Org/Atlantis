using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ;
using Followme.AspNet.Core.FastCommon.Utilities;
using Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ;
using Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class RabbitConfigurationExetension
    {
        private static IList<Type> _metadatas = new List<Type>();

        public static Configuration RegisterRabbitMQ(this Configuration configuration,string rabbitMQSettingConfigName )
        {
            ObjectContainer.Register<IRabbitMessagePublisher, RabbitMessagePublisher>();
            configuration.Setting.SetRabbitMQSetting(configuration.GetSetting<RabbitMQSetting>(rabbitMQSettingConfigName) ?? throw new ArgumentNullException("Please setting rabbit!"));
            var types = RefelectionHelper.GetImplInterfaceTypes(typeof(IRabbitMessagingHandler), false, configuration.Setting.BussinessAssemblies);
            foreach(var type in types)
            {
                if(type.IsAbstract)continue;
                ObjectContainer.Register(type, LifeScope.Transient);
                _metadatas.Add(type);
            }
            return configuration;
        }

        public static Configuration StartRabbitMQ(this Configuration configuration)
        {
            var setting=configuration.Setting.GetRabbitMQSetting();
            foreach (var type in _metadatas)
            {
                var instance = (IRabbitMessagingHandler)ObjectContainer.Resolve(type);
                if (!instance.IsEnable) continue;
                if (string.IsNullOrWhiteSpace(instance.Name))throw new ArgumentNullException($"The virtualhost is null in the handler({instance.GetType().Name})!");
                var hostSetting=setting.GetServer(instance.Name)??throw new ArgumentNullException($"The handler name({instance.Name}) cannot found setting!");
                var rabbitContext = new RabbitConnection(instance.Queue, instance.Exchange,instance.RoutingKey, hostSetting, instance.Handle);
                Task.Run(()=>{rabbitContext.Start();});
            }
            _metadatas=null;
            return configuration;
        }
    }

    public static class RabbitMQConfigurationSettingExtension
    {
        private static RabbitMQSetting _setting;

        public static RabbitMQSetting GetRabbitMQSetting(this ConfigurationSetting setting)
        {
            return _setting;
        }

        public static ConfigurationSetting SetRabbitMQSetting(this ConfigurationSetting configurationSetting,RabbitMQSetting rabbitMQSetting)
        {
            _setting=rabbitMQSetting;
            return configurationSetting;
        }
    }
}
