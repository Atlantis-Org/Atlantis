using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk.Notification
{
    public class DingTalkResult
    {
        public DingTalkResult(int errorCode, string errorMsg)
        {
            ErrCode = errorCode;
            ErrMsg = errorMsg;
        }

        public bool IsSucceed()=> ErrCode == 0;
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}
