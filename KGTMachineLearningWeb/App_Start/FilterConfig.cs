using KGTMachineLearningWeb.Attributes;
using KGTMachineLearningWeb.Domain.Contracts;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace KGTMachineLearningWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RunningJobsFilter((IJobDomain)(UnityConfig.Container as IUnityContainer).Resolve<IJobDomain>()));
        }
    }
}
