using KGTMachineLearningWeb.Common.Helper;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models.Workspace;
using KGTMachineLearningWeb.Repository.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Domain.Services
{
    internal class CategoryDomain : BaseDomain<int, Category>, ICategoryDomain
    {
        private ICategoryRepository CategoryRepository => repository as ICategoryRepository;
        public CategoryDomain(ICategoryRepository categoryRepository) : base(categoryRepository) { }

        public override Category Add(Category entity)
        {
            if (entity.ParentCategoryId.HasValue)
            {
                GetUniqueCategoryName(entity, entity.ParentCategoryId.Value);
            }

            return base.Add(entity);
        }

        public IEnumerable<Category> SearchByTitleAndDescription(string searchTerm)
        {
            return CategoryRepository.SearchByTitleAndDescription(searchTerm) ?? new List<Category>();
        }

        public IEnumerable<Category> GetChildCategories(int parentId)
        {
            var parentCategory = CategoryRepository.GetById(parentId);
            return parentCategory?.ChildCategories ?? new List<Category>();
        }

        public bool HasChildren(int parentId)
        {
            var category = CategoryRepository.GetById(parentId);
            var caCount = category?.ChildCategories?.Count;
            return caCount != null && caCount > 0 ? true : false;
        }

        public Category Move(int categoryId, int newParentId)
        {
            var categoryToMove = GetById(categoryId);
            GetUniqueCategoryName(categoryToMove, newParentId);
            if (categoryToMove != null)
                categoryToMove.ParentCategoryId = newParentId;
            return CategoryRepository.Update(categoryToMove);
        }

        public LinkedList<int> GetParentHierarchy(int categoryId)
        {
            var hierarchy = new LinkedList<int>();
            var category = GetById(categoryId);

            if (category == null) return new LinkedList<int>();

            hierarchy.AddFirst(category.Id);

            var currentParent = category.ParentCategory;
            while (currentParent != null)
            {
                hierarchy.AddLast(currentParent.Id);
                currentParent = currentParent.ParentCategory;
            }

            return hierarchy;
        }

        public Category GetRootCategoryByOwnerId(string ownerId)
        {
            return CategoryRepository.GetRootByOwnerId(ownerId);
        }

        public Category CreateRootCategoryForUser(string ownerId)
        {
            var rootCategory = new Category(CommonConstants.USERS_ROOT_CATEGORY_NAME)
            {
                IsRoot = true,
                OwnerId = ownerId,
            };

            Add(rootCategory);

            return rootCategory;
        }

        public IEnumerable<Category> GetSharedRootCategories(string currentUserId)
        {
            var retVal = GetOtherUsersRootCategories(currentUserId)?.ToList() ?? new List<Category>();
            retVal.Insert(0, GetEveryoneRootCategory());
            return retVal;
        }

        public IEnumerable<Category> GetOtherUsersRootCategories(string currentUserId)
        {
            var otherUserCategories = CategoryRepository
                .Get(c => c.IsRoot && !c.IsEveryones && c.OwnerId != currentUserId);
            var retVal = otherUserCategories;
            return retVal ?? new List<Category>();
        }

        public Category GetEveryoneRootCategory()
        {
            var rootEveryonesFolder = CategoryRepository.Get(c => c.IsEveryones && c.IsRoot).FirstOrDefault();
            return rootEveryonesFolder;
        }

        public Category Copy(int categoryToCopyId, int parentCategoryId)
        {
            var categoryToCopy = GetById(categoryToCopyId);
            var parentCategory = GetById(parentCategoryId);
            bool copyChartObjects = categoryToCopy.OwnerId == parentCategory.OwnerId;

            var copiedCategory = categoryToCopy.Copy(parentCategory, copyChartObjects);

            Add(copiedCategory);

            return copiedCategory;
        }

        public string GetUniqueCategoryName(Category category, int parentCategoryId)
        {

            //Mach all numbers at the end of the name surronded by round brackets
            string pattern = Regex.Escape(category.Title) + @"\s{0,1}(\((?<number>\d+)\)){0,1}$";

            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            bool nameIsUnique = true;
            int currentNumber = 0;
            var parentCategory = GetById(parentCategoryId);
            foreach (var childCategory in parentCategory.ChildCategories)
            {
                if (childCategory.Id == category.Id)
                    continue;

                var match = Regex.Match(childCategory.Title, pattern, options);
                if (match.Success)
                {
                    nameIsUnique = false;
                    if (match.Groups["number"].Success)
                    {
                        int parsedNumber = 0;
                        int.TryParse(match.Groups["number"].Value, out parsedNumber);
                        if (parsedNumber > currentNumber)
                        {
                            currentNumber = parsedNumber;
                        }
                    }
                }
            }
            
            if (!nameIsUnique)
            {
                category.Title = string.Format("{0}({1})", category.Title, currentNumber + 1);
            }

            return category.Title;
        }
    }
}
