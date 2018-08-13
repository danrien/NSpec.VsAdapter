using Autofac;
using NSpec.VsAdapter.Common;
using NSpec.VsAdapter.Core.CrossDomain;
using NSpec.VsAdapter.Core.Discovery;
using NSpec.VsAdapter.Core.Discovery.Target;
using NSpec.VsAdapter.Core.Execution;
using NSpec.VsAdapter.Core.Execution.Target;
using NSpec.VsAdapter.Logging;
using NSpec.VsAdapter.Settings;
using NSpec.VsAdapter.TestAdapter.Discovery;
using NSpec.VsAdapter.TestAdapter.Execution;
using System;

namespace NSpec.VsAdapter
{
    class DependencyContainer : IDisposable
    {
        private DependencyContainer()
        {
            var builder = new ContainerBuilder();

            RegisterContainerDiscoverer(builder);
            RegisterDiscoverer(builder);
            RegisterExecutor(builder);
            RegisterCommon(builder);

            container = builder.Build();
        }

        public ILifetimeScope BeginScope()
        {
            return container.BeginLifetimeScope();
        }

        public void Dispose()
        {
            container.Dispose();
        }

        readonly IContainer container;

        static void RegisterContainerDiscoverer(ContainerBuilder builder)
        {
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
        }

        static void RegisterDiscoverer(ContainerBuilder builder)
        {
            builder.RegisterType<MultiSourceTestDiscovererFactory>().As<IMultiSourceTestDiscovererFactory>().InstancePerLifetimeScope();
            #if NET452
            builder.RegisterType<ProxyableTestDiscovererFactory>().As<IProxyableFactory<IProxyableTestDiscoverer>>().InstancePerLifetimeScope();
            #endif
            builder.RegisterType<BinaryTestDiscoverer>().As<IBinaryTestDiscoverer>().InstancePerLifetimeScope();
            builder.RegisterType<TestCaseMapper>().As<ITestCaseMapper>().InstancePerLifetimeScope();
        }

        static void RegisterExecutor(ContainerBuilder builder)
        {
            builder.RegisterType<MultiSourceTestExecutorFactory>().As<IMultiSourceTestExecutorFactory>().InstancePerLifetimeScope();
            #if NET452
            builder.RegisterType<ProxyableTestExecutorFactory>().As<IProxyableFactory<IProxyableTestExecutor>>().InstancePerLifetimeScope();
            #endif
            builder.RegisterType<BinaryTestExecutor>().As<IBinaryTestExecutor>().InstancePerLifetimeScope();
            builder.RegisterType<ProgressRecorderFactory>().As<IProgressRecorderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TestResultMapper>().As<ITestResultMapper>().InstancePerLifetimeScope();
        }

        static void RegisterCommon(ContainerBuilder builder)
        {
            #if NET452
            builder.RegisterType<AppDomainFactory>().As<IAppDomainFactory>().InstancePerLifetimeScope();
            #endif
            builder.RegisterGeneric(typeof(CrossDomainRunner<,>)).As(typeof(ICrossDomainRunner<,>)).InstancePerLifetimeScope();
            builder.RegisterType<AdapterInfo>().As<IAdapterInfo>().InstancePerLifetimeScope();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().InstancePerLifetimeScope();
        }

        public static DependencyContainer Instance => instanceHolder.Value;

        static readonly Lazy<DependencyContainer> instanceHolder = new Lazy<DependencyContainer>(() => new DependencyContainer(), false);
    }
}
