using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Domain.Services;
using KGTMachineLearningWeb.Models.Identity;
using KGTMachineLearningWeb.Repository.Contracts;
using KGTMachineLearningWeb.Repository.Services;
using System.Data.Entity;
using Unity;
using Unity.Lifetime;

namespace KGTMachineLearningWeb.Config
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
            container.RegisterType<IPmPredictionsDomain, PmPredictionsDomain>();
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
            container.RegisterType<INeuralNetworkRepository,NeuralNetworkRepository>();
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
            container.RegisterType<DbContext, KGTContext>(new HierarchicalLifetimeManager());
            container.RegisterType<KGTContext>();
        }
    }
}
