using KGTMachineLearningWeb.Domain.Contracts;
using Microsoft.AspNet.Identity;
using NLog;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGTMachineLearningWeb.Attributes
{
    public class RunningJobsFilter : ActionFilterAttribute
    {
        private readonly IJobDomain _jobDomain;

        public RunningJobsFilter(IJobDomain jobDomain)
        {
            _jobDomain = jobDomain;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    return;
                }

                var userId = HttpContext.Current.User.Identity.GetUserId();
                var runninJobs = _jobDomain.Get(j => j.UserCreatorId == userId && j.Status == Models.Jobs.JobProcessedStatus.Processing).Count();
                filterContext.Controller.ViewBag.RunningJobs = runninJobs;
                base.OnActionExecuting(filterContext);
            }
            catch (System.Exception e)
            {
                filterContext.Controller.ViewBag.RunningJobs = 0;
            }
        }
    }
}