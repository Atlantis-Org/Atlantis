using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes.Wrappers
{
    public interface IRabbitPublishMessageWrapper<T>
    {
        T Message { get; }

        RabbitPublishMessageMetadata Metadata { get; }

        byte[] Serialize();
    }

    public abstract class RabbitPublishMessageWrapper<T> : IRabbitPublishMessageWrapper<T>
    {
        public RabbitPublishMessageWrapper(T message, RabbitPublishMessageMetadata metadata)
        {
            Message = message;
            Metadata = metadata;
        }

        public T Message { get; protected set; }

        public RabbitPublishMessageMetadata Metadata { get; protected set; }

        public abstract byte[] Serialize();
    }
}
