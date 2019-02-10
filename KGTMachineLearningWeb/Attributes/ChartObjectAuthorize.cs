using KGTMachineLearningWeb.Domain.Contracts;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity.Attributes;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Attributes
{
    public class ChartObjectAuthorize :AuthorizeAttribute
    {
        public string ChartObjectIdRequestName { get; set; }

        public Permissions Permission { get; set; }

        [Dependency]
        public IPermissionDomain PermissionDomain { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
            {
                return false;
            }

            int chartObjectId;
            if (int.TryParse(httpContext.Request[ChartObjectIdRequestName], out chartObjectId))
            {
                return PermissionDomain.HasChartObjectPermission(chartObjectId, httpContext.User.Identity.GetUserId(), Permission);
            }
            else
            {
                return false;
            }
        }
    }
}