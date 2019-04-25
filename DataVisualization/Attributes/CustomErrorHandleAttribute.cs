using NLog;
using System.Web.Mvc;
using Unity.Attributes;

namespace DataVisualization.Attributes
{
    public class CustomErrorHandleAttribute : HandleErrorAttribute
    {
        [Dependency]
        public ILogger Logger { get; set; }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            Logger.Error(filterContext.Exception);
        }
    }
}