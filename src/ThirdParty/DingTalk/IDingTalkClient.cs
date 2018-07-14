using Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk.Notification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk
{
    public interface IDingTalkClient
    {
         DingTalkResult SentNotification(IDingTalkNotification notification);
    }
}
