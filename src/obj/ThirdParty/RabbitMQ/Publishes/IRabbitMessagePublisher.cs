using Followme.AspNet.Core.FastCommon.ThirdParty.RibbitMQ;
using Followme.AspNet.Core.FastCommon.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes.Wrappers;
using Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ.Publishes;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ
{
    public interface IRabbitMessagePublisher
    {
        void Publish<T>(T message);
    }     
}
