using System;
using Followme.AspNet.Core.FastCommon.Logging.Providers;

namespace Followme.AspNet.Core.FastCommon.Logging
{
    public abstract class LoggerFactory:ILoggerFactory
    {
        private readonly ILoggerProvider _loggerProvider;

        public LoggerFactory(ILoggerProvider loggerProvider)
        {
            _loggerProvider=loggerProvider;
            _loggerProvider.Config(GetSetting());
        }

        protected ILoggerProvider Provider=>_loggerProvider;
        
        public virtual ILogger Create<T>()
        {
            return _loggerProvider.CreateLogger(typeof(T).Name);
        }

        public virtual ILogger Create(string name)
        {
            return _loggerProvider.CreateLogger(name);
        }

        public virtual ILogger Create(Type type)
        {
            return _loggerProvider.CreateLogger(type.Name);
        }

        protected abstract ProviderSetting GetSetting();
    }
}
