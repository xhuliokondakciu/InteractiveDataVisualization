using Highsoft.Web.Mvc.Charts;
using DataVisualization.Context;
using DataVisualization.Domain.Contracts;
using DataVisualization.Domain.Services;
using DataVisualization.Models.Identity;
using DataVisualization.Repository.Contracts;
using DataVisualization.Repository.Services;
using System.Data.Entity;
using Unity;
using Unity.Lifetime;

namespace DataVisualization.Config
{
    public static class DependencyInjection
    {
        public static void RegisterDependencies(IUnityContainer container)
        {

            RegisterRepositories(container);
            RegisterDomains(container);
            RegisterCommonDependencies(container);
            RegisterDbContext(container);
        }

        private static void RegisterDomains(IUnityContainer container)
        {
            container.RegisterType<IHighChartsDomain, HighChartsDomain>();
            container.RegisterType<INeuralNetworkDomain, NeuralNetworkDomain>();
            container.RegisterType<ICategoryDomain, CategoryDomain>();
            container.RegisterType<IChartObjectDomain, ChartObjectDomain>();
            container.RegisterType<IPermissionDomain, PermissionDomain>();
            container.RegisterType<IUserDomain, UserDomain>();
            container.RegisterType<IRoleDomain, RoleDomain>();
            container.RegisterType<IChartDataDomain, ChartDataDomain>();
            container.RegisterType<IJobDomain, JobDomain>();
            container.RegisterType<IProcessorConfigurationDomain, ProcessorConfigurationDomain>();
            container.RegisterType<IChartsConfigurationDomain, ChartsConfigurationDomain>();
        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            container.RegisterType<IPmPredictionsRepository,PmPredictionsRepository>();
            container.RegisterType<ICategoryRepository,CategoryRepository>();
            container.RegisterType<IChartObjectRepository,ChartObjectRepository>();
            container.RegisterType<IUserRepository,UserRepository>();
            container.RegisterType<IRoleRepository,RoleRepository>();
            container.RegisterType<IChartDataRepository,ChartDataRepository>();
            container.RegisterType<IJobStatusRepository,JobStatusRepository>();
            container.RegisterType<IProcessorConfigurationRepository,ProcessorConfigurationRepository>();
            container.RegisterType<IChartsConfigurationRepository,ChartsConfigurationRepository>();
        }

        private static void RegisterCommonDependencies(IUnityContainer container)
        {
            container.RegisterType<HighsoftNamespace>();
        }

        private static void RegisterDbContext(IUnityContainer container)
        {
            container.RegisterType<DbContext, VisContext>(new HierarchicalLifetimeManager());
            container.RegisterType<VisContext>();
        }
    }
}
