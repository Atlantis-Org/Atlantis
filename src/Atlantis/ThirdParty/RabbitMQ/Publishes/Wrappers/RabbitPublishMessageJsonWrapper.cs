using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Serializing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes.Wrappers
{
    public class RabbitPublishMessageJsonWrapper<T> : RabbitPublishMessageWrapper<T>
    {
        private readonly IJsonSerializer _jsonSerializer;

        public RabbitPublishMessageJsonWrapper(T message, RabbitPublishMessageMetadata metadata) : base(message, metadata)
        {
            _jsonSerializer = ObjectContainer.Resolve<IJsonSerializer>();
        }

        public override byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(Message));
        }
    }
}
