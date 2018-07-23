using Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes
{
    public class RabbitPublishMessageMetadata
    {
        public RabbitPublishMessageMetadata(RabbitPublishToAttribute attribute, RabbitServerSetting connectHostString)
        {
            Name = attribute.GroupName;
            ConnectSetting = connectHostString;
            Exchange = attribute.Exchange;
            SerializeType = attribute.SerializeType;
            RoutingKey = attribute.RoutingKey;
        }

        public string Name { get; set; }

        public RabbitServerSetting ConnectSetting { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public PublishMessageSerialzeType SerializeType { get; set; }
    }
}
