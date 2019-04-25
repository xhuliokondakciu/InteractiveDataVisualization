using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Attributes
{
    public class HangfireDashboardFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {

            var owinContext = new OwinContext(context.GetOwinEnvironment());

            return owinContext.Request.User.IsInRole(UserRoles.Admin.ToString()); ;
        }
    }
}