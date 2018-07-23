using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ
{
    public interface IRabbitMessagingHandler
    {
        string Queue { get; }

        string Exchange { get; }

        string VirtualHost { get; }

        string RoutingKey{get;}

        bool IsEnable { get; }

        string Name{get;}

        void Handle(RabbitConnection connection, object model, BasicDeliverEventArgs e);
    }

    public interface IRabbitMessagingHandler<TMessage>:IRabbitMessagingHandler where TMessage:IRabbitMessaging
    {
        
    }
}
