using KGTMachineLearningWeb.Models.Workspace;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IChartObjectDomain : IBaseDomain<int, ChartObject>
    {
        /// <summary>
        /// Get chart objects by category id
        /// </summary>
        /// <param name="categoryId">Category id to get chart object for</param>
        /// <returns>Matched chart objects</returns>
        IEnumerable<ChartObject> GetByCategoryId(int categoryId);

        /// <summary>
        /// Change chart object category
        /// </summary>
        /// <param name="chartObjectId">Chart object to change category id</param>
        /// <param name="newCategoryId">New category id</param>
        /// <returns>Chart object with new category</returns>
        ChartObject ChangeCategory(int chartObjectId, int newCategoryId);

        /// <summary>
        /// Search chart objects by title and description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matched chart objects</returns>
        IEnumerable<ChartObject> SearchByTitleAndDescription(string searchTerm);

        /// <summary>
        /// Copy chart object
        /// </summary>
        /// <param name="chartObjectToCopyId">Chart object to copy id</param>
        /// <param name="destinationCategoryId">Destination category id</param>
        /// <returns>Copied chart object</returns>
        ChartObject Copy(int chartObjectToCopyId, int destinationCategoryId, string ownerId);

        /// <summary>
        /// Get unique name of chart object inside a category
        /// </summary>
        /// <param name="parentCategoryId">Id of the parent category of the chart object</param>
        /// <param name="chartObjectTitle">Title of the chart object</param>
        /// <returns>New unique name for the chart object</returns>
        string GetUniqueChartObjectName(ChartObject chartObject, int parentCategoryId);
    }
}
