using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IPermissionDomain
    {
        /// <summary>
        /// Check if specified user is the specified category owner
        /// </summary>
        /// <param name="categoryId">Category id to check if user is owner</param>
        /// <param name="userId">User id for whom to check</param>
        /// <returns></returns>
        bool IsCategoryOwner(int categoryId, string userId);

        /// <summary>
        /// Check if user has a specific permission for the specified category
        /// </summary>
        /// <param name="categoryId">Category id to check permission for</param>
        /// <param name="userId">User for whom to check permission</param>
        /// <param name="permission">Type of permission to check</param>
        /// <returns>True if user has specified permission for the specified category, else return false</returns>
        bool HasCategoryPermission(int categoryId, string userId, Permissions permission);

        /// <summary>
        /// Check if user has a specific permission for the specified chart object
        /// </summary>
        /// <param name="categoryId">Chart object id to check permission for</param>
        /// <param name="userId">User for whom to check permission</param>
        /// <param name="permission">Type of permission to check</param>
        /// <returns>True if user has specified permission for the specified chart object, else return false</returns>
        bool HasChartObjectPermission(int chartObjectId, string userId, Permissions permission);

        List<Permissions> GetCategoryPermissions(int categoryId, string userId);

        List<Permissions> GetChartObjectPermissions(int chartObjectId, string userId);
    }
}
