﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Followme.AspNet.Core.FastCommon.Logging
{
    public class NullLogger : ILogger
    {
        public bool IsDebugEnabled => false;

        public void Debug(string msg, Exception exception = null,object[] parameters=null)
        {
        }

        public void Error(string msg, Exception exception = null,object[] parameters=null)
        {
        }

        public void Fatal(string msg, Exception exception = null,object[] parameters=null)
        {
        }

        public void Info(string msg, Exception exception = null,object[] parameters=null)
        {
        }

        public void Trace(string msg, Exception exception = null,object[] parameters=null)
        {
        }

        public void Warn(string msg, Exception exception = null,object[] parameters=null)
        {
        }
    }
}
