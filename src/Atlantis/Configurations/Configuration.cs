using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Followme.AspNet.Core.FastCommon.Commanding;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Domain.Repository;
using Followme.AspNet.Core.FastCommon.Domain.Repository.Implementions;
using Followme.AspNet.Core.FastCommon.Domain.UnitOfWorks;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.ThirdParty.Autofac;
using Followme.AspNet.Core.FastCommon.Utilities;
using System.Linq;
using Followme.AspNet.Core.FastCommon.CodeGeneration;
using Followme.AspNet.Core.FastCommon.Scheduling;
using Followme.AspNet.Core.FastCommon.Logging.Providers;
using Followme.AspNet.Core.FastCommon.ThirdParty.Cache;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public class Configuration
    {
        private readonly ConfigurationSetting _setting;

        public Configuration(ConfigurationSetting setting=null)
        { 
            _setting=setting??new ConfigurationSetting();
        }

        public ConfigurationSetting Setting=>_setting;

        public Configuration UseAutoFac()
        {
            ObjectContainer.SetContainer(new AutofacObjectContainer());
            return this;
        }

        public Configuration RegisterDefault()
        {
            ObjectContainer.RegisterInstance<Configuration>(this);
            ObjectContainer.Register<IRepositoryContext,NullRepositoryContext>(LifeScope.Transient);
            RepositoryContextManager.RegisterCreateNewContextFunc(() => { return new NullRepositoryContext(); });
            ObjectContainer.Register<ICurrentRepositoryContextProvider,CurrentRepositoryContextProvider>();
            ObjectContainer.Register<IRepositoryContextManager,RepositoryContextManager>();
            ObjectContainer.Register<ILoggerFactory,NullLoggerFactory>();
            ObjectContainer.Register<ILoggerProvider, NullLoggerProvider>();
            ObjectContainer.Register<IJsonSerializer,NullJsonSerializer>();
            ObjectContainer.Register<ICommandHandler,CommandHandler>();
            ObjectContainer.Register<IScheduleService,ScheduleService>();
            ObjectContainer.Register<ICache, InMemoryCache>();
            return this;
        }

        public Configuration RegisterBussinessAssemblies(params Assembly[] bussinessAssemblies)
        {
            _setting.BussinessAssemblies=bussinessAssemblies;
            this.RegisteCommandServicer().RegisteQueryServicer();
            ObjectContainer.RegisterFromAssemblysForInterface(_setting.BussinessAssemblies);
            
            return this;
        }

        public Configuration Build()
        {
            var codeAssembly=CodeBuilder.Instance.Build();
            ObjectContainer.RegisterFromAssemblysForInterface(codeAssembly.Assembly);

            return this;
        }
    }
}
