using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Domain.Services
{
    public class PermissionDomain : IPermissionDomain
    {

        private readonly ICategoryDomain categoryDomain;
        private readonly IChartObjectDomain chartObjectDomain;
        private readonly IUserDomain userDomain;

        public PermissionDomain(
            IChartObjectDomain chartObjectDomain,
            ICategoryDomain categoryDomain,
            IUserDomain userDomain)
        {
            this.chartObjectDomain = chartObjectDomain;
            this.categoryDomain = categoryDomain;
            this.userDomain = userDomain;
        }

        public bool IsCategoryOwner(int categoryId, string userId)
        {
            var category = categoryDomain.GetById(categoryId);

            if (category?.OwnerId == userId)
                return true;

            return false;
        }

        public bool HasCategoryPermission(int categoryId, string userId, Permissions permission)
        {
            return GetCategoryPermissions(categoryId, userId).Any(p => p == permission);
        }

        public List<Permissions> GetCategoryPermissions(int categoryId, string userId)
        {
            var permissions = new List<Permissions>(){
                Permissions.View
            };
            var category = categoryDomain.GetById(categoryId);

            if (category == null)
                return permissions;

            bool isOwner = category.OwnerId == userId;
            var isAdmin = userDomain.IsInRole(userId, UserRoles.Admin);
            if (isAdmin)
            {
                permissions.Add(Permissions.Create);
                if (!category.IsRoot)
                {
                    permissions.AddRange(new List<Permissions>
                    {
                        Permissions.Copy,
                        Permissions.Delete,
                        Permissions.Edit
                    });
                }
            }
            else
            {
                if (!category.IsRoot) permissions.Add(Permissions.Copy);
                if ((isOwner || category.IsEveryones) && !category.IsRoot)
                {
                    permissions.Add(Permissions.Delete);
                    permissions.Add(Permissions.Edit);
                }
                if (isOwner || category.IsEveryones) permissions.Add(Permissions.Create);
            }

            return permissions;
        }

        public bool HasChartObjectPermission(int chartObjectId, string userId, Permissions permission)
        {
            return GetChartObjectPermissions(chartObjectId, userId).Any(p => p == permission);
        }


        public List<Permissions> GetChartObjectPermissions(int chartObjectId, string userId)
        {
            var permissions = new List<Permissions>(){
                Permissions.View
            };
            var chartObject = chartObjectDomain.GetById(chartObjectId);

            if (chartObject == null)
                return permissions;

            bool isOwner = chartObject.OwnerId == userId;
            var isAdmin = userDomain.IsInRole(userId, UserRoles.Admin);
            if (isAdmin)
            {
                permissions.AddRange(new List<Permissions>
                    {
                        Permissions.Copy,
                        Permissions.Delete,
                        Permissions.Edit
                    });
            }
            else
            {
                if (isOwner || (chartObject.Category?.IsEveryones ?? false))
                {
                    permissions.AddRange(new List<Permissions>
                    {
                        Permissions.Delete,
                        Permissions.Edit,
                        Permissions.Copy
                    });
                }
            }

            return permissions;
        }
    }
}
