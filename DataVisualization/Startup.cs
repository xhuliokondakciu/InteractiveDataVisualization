 using Hangfire;
using DataVisualization.Attributes;
using DataVisualization.Domain.Contracts;
using DataVisualization.Domain.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Injection;

[assembly: OwinStartup(typeof(DataVisualization.Startup))]

namespace DataVisualization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //Register Hangfire
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("DefaultConnection");
            GlobalConfiguration.Configuration.UseUnityActivator(UnityConfig.Container);
            app.UseHangfireDashboard("/Admin/Jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardFilter() }
            });
            var serverOptions = new BackgroundJobServerOptions
            {
                ServerName = "ChartDataServer",
                WorkerCount = 4,
                ServerTimeout = new TimeSpan(4, 0, 0)

            };
            app.UseHangfireServer(serverOptions);


            GlobalHost.DependencyResolver = new UnitySignalRDependencyResolver(UnityConfig.Container);
            
            app.MapSignalR();

            UnityConfig.Container.RegisterType<ChartHub>();
            var jobDomainInstance = (UnityConfig.Container as IUnityContainer).Resolve(typeof(IJobDomain)) as IJobDomain;
            UnityConfig.Container.RegisterType<ChartHubManager>(new InjectionConstructor(GlobalHost.ConnectionManager.GetHubContext<ChartHub>(),jobDomainInstance));

        }
    }

    internal class UnitySignalRDependencyResolver : DefaultDependencyResolver
    {
        private IUnityContainer _container;
        public UnitySignalRDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {

            if (_container.IsRegistered(serviceType))
                return _container.Resolve(serviceType);
            else
                return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.IsRegistered(serviceType))
                return _container.ResolveAll(serviceType);
            else
                return base.GetServices(serviceType);
        }
    }
}
