using System;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Followme.AspNet.Core.FastCommon.Components;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Autofac
{
    public class AutofacObjectContainer : IObjectContainer
    {
        private IContainer _container;
        private ContainerBuilder _builder;

        public AutofacObjectContainer()
        {
            _builder = new ContainerBuilder();
        }

        public void Register<TInterface, TService>(LifeScope lifeScope = LifeScope.Single)
        {
            _builder.RegisterType<TService>().As<TInterface>().SetLifeScope(lifeScope);
        }

        public void Register<TService>(LifeScope lifeScope = LifeScope.Single)
        {
            _builder.RegisterType<TService>().SetLifeScope(lifeScope);
        }

        public void Register(Type interfaceType, Type serviceType, LifeScope lifeScope = LifeScope.Single)
        {
            _builder.RegisterType(serviceType).As(interfaceType).SetLifeScope(lifeScope);
        }

        public void Register(Type serviceType, LifeScope lifeScope = LifeScope.Single)
        {
            _builder.RegisterType(serviceType).SetLifeScope(lifeScope);
        }

        public void Register<TService>(TService instance, Type aliasType, LifeScope lifeScope = LifeScope.Single) where TService : class
        {
            var registerBuilder = _builder.RegisterInstance(instance).SetLifeScope(lifeScope);
            if (aliasType != null) registerBuilder.As(aliasType);
        }

        public void RegisterFromAssemblysForInterface(params Assembly[] assemblys)
        {
            _builder.RegisterAssemblyTypes(assemblys).AsImplementedInterfaces();//.SetLifeScope(LifeScope.Transient);

        }

        public T Resolve<T>()
        {
            if (_container == null) _container = _builder.Build();
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            if (_container == null) _container = _builder.Build();
            return _container.Resolve(type);
        }
    }

    internal static class AutofacExetension
    {
        internal static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> SetLifeScope<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registerInfo, LifeScope lifeScope)
        {
            if (lifeScope == LifeScope.Single) registerInfo.SingleInstance();
            return registerInfo;
        }
    }

}