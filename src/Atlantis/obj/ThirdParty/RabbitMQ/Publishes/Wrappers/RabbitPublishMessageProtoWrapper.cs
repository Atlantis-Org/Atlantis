using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Serializing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes.Wrappers
{
    public class RabbitPublishMessageProtoWrapper<T> : RabbitPublishMessageWrapper<T>
    {
        private readonly IBinarySerializer _binarySerializer;

        public RabbitPublishMessageProtoWrapper(T message, RabbitPublishMessageMetadata metadata) : base(message, metadata)
        {
            _binarySerializer = ObjectContainer.Resolve<IBinarySerializer>();
        }

        public override byte[] Serialize()
        {
            return _binarySerializer.Serialize(Message);
        }
    }
}
