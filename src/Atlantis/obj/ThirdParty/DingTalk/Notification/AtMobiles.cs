using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingTalk.Notification
{
    public class AtMobiles
    {
        public AtMobiles(List<string> mobiles, bool atAll = false)
        {
            atMobiles = mobiles;
            isAtAll = atAll;
        }
        public List<string> atMobiles { get; }

        public bool isAtAll { get; set; }
    }
}
