using Followme.AspNet.Core.FastCommon.ThirdParty.DingTalk.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk.Notification
{
    public class DingTalkTextNotification : IDingTalkNotification
    {
        public DingTalkTextNotification(string msg, List<string> @Mobiles = null)
        {
            text = new TextContent(msg);
            atMobiles = @Mobiles == null ? null : new AtMobiles(@Mobiles);
        }

        public string msgtype { get => "text"; }

        public TextContent text { get; set; }

        public AtMobiles atMobiles { get; set; }
    }

    public class TextContent
    {
        public TextContent(string textContent)
        {
            content = textContent;
        }

        public string content { get;}
    }
}
