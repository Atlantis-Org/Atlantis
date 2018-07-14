using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk.Notification
{
    public class DingTalkMarkdownContent : IDingTalkNotification
    {
        public string msgtype { get => "markdown"; }

        public string Title { get; set; }

        public string Text { get; set; }
    }
}
