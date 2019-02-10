using KGTMachineLearningWeb.Models.Workspace;
using System;
using System.Collections.Generic;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface ICategoryDomain : IBaseDomain<int, Category>
    {
        IEnumerable<Category> GetChildCategories(int parentId);

        bool HasChildren(int parentId);

        Category Move(int categoryId, int newParentId);

        IEnumerable<Category> SearchByTitleAndDescription(string searchTerm);

        LinkedList<int> GetParentHierarchy(int categoryId);

        Category GetRootCategoryByOwnerId(string ownerId);

        Category CreateRootCategoryForUser(string ownerId);

        /// <summary>
        /// Get root categories that everyone has access to
        /// </summary>
        /// <param name="currentUserId">Id of the current user so we don't get his categories</param>
        /// <returns>IEnumerable of root categories of other users and the everyones category</returns>
        IEnumerable<Category> GetSharedRootCategories(string currentUserId);

        /// <summary>
        /// Get root categories of every other user appart from the current loged in user
        /// </summary>
        /// <param name="currentUserId">The id of the currently loged in user</param>
        /// <returns>List of other users root category</returns>
        IEnumerable<Category> GetOtherUsersRootCategories(string currentUserId);

        /// <summary>
        /// Get everyones category root
        /// </summary>
        /// <returns>Everyones category</returns>
        Category GetEveryoneRootCategory();

        /// <summary>
        /// Get unique name of a category inside the parent category
        /// </summary>
        /// <param name="parentCategoryId">Id of the parent category</param>
        /// <param name="categoryTitle">Category title</param>
        /// <returns></returns>
        string GetUniqueCategoryName(Category category, int parentCategoryId);

        /// <summary>
        /// Copy category to new parent category
        /// </summary>
        /// <param name="categoryToCopyId">Id of category to copy</param>
        /// <param name="parentCategoryId">Id of category where to copy</param>
        /// <returns>The new copied category</returns>
        Category Copy(int categoryToCopyId, int parentCategoryId);
    }
}
