using Highsoft.Web.Mvc.Charts;
using DataVisualization.Attributes;
using DataVisualization.Domain.Contracts;
using DataVisualization.Domain.Factories;
using DataVisualization.Models.Workspace;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Controllers
{
    [Authorize]
    [CustomErrorHandle]
    public class WorkspaceController : BaseController
    {
        private readonly ICategoryDomain _categoryDomain;
        private readonly IPermissionDomain _permissionDomain;
        private readonly IUserDomain _userDomain;
        private readonly IChartObjectDomain _chartObjectDomain;
        private readonly IChartDataDomain _chartDataDomain;
        private readonly IChartsConfigurationDomain _chartsConfigDomain;
        private readonly ILogger logger; 

        public WorkspaceController(
            ICategoryDomain categoryDomain,
            IPermissionDomain permissionDomain, 
            IUserDomain userDomain,
            IChartObjectDomain chartObjectDomain,
            IChartDataDomain chartDataDomain,
            IChartsConfigurationDomain chartsConfigDomain,
            ILogger logger)
        {
            _categoryDomain = categoryDomain;
            _permissionDomain = permissionDomain;
            _userDomain = userDomain;
            _chartObjectDomain = chartObjectDomain;
            _chartDataDomain = chartDataDomain;
            _chartsConfigDomain = chartsConfigDomain;
            this.logger = logger;
        }

        // GET: Workspace
        public ActionResult Index()
        {
            LoadUserChartConfigurations();

            return View();
        }

        /// <summary>
        /// Get child categories of a category in the format of JSON response to be used by the tree to construct nodes
        /// </summary>
        /// <param name="parentId">Category id to get children of</param>
        /// <returns>Returns JSON of child categories in a format needed by fancytree</returns>
        [HttpGet]
        public ActionResult GetChildCategoriesNodes(int? parentId)
        {
            IEnumerable<TreeNodeModel> childCategories;
            if (!parentId.HasValue)
            {
                var rootNode = MapCategoryToTreeNodeModel(_categoryDomain.GetRootCategoryByOwnerId(User.Identity.GetUserId()));
                rootNode.IsRoot = true;
                childCategories = new List<TreeNodeModel>
                {
                    rootNode
                };
            }
            else
            {
                childCategories = _categoryDomain
                    .GetChildCategories(parentId.Value)
                    .OrderBy(c => c.Title)
                    .Select(MapCategoryToTreeNodeModel);
            }

            return Json(childCategories, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// On request without parentId
        /// </summary>
        /// <param name="parentId">Category id to get children of</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSharedCategoriesNodes(int? parentId)
        {
            //If id doesn't have value it means that is the initial request
            if (!parentId.HasValue)
            {
                IEnumerable<TreeNodeModel> childCategoriesNodes = _categoryDomain.GetSharedRootCategories(User.Identity.GetUserId())
                    .OrderBy(c => c.Title)
                    .Select(MapCategoryToTreeNodeModel).ToList();

                var rootTreeNodes = new List<TreeNodeModel>
                {
                    //Root folder placeholder
                    new TreeNodeModel
                    {
                        Key = DataVisualization.Common.Helper.CommonConstants.SHARED_ROOT_FOLDER_ID.ToString(),
                        Title = "Shared categories",
                        Folder = true,
                        Lazy = true,
                        Expanded = false,
                        IsRoot = true,
                        IsShared = true,
                        IsSharedRoot = true,
                        Children = childCategoriesNodes.ToList()
                    }
                };

                return Json(rootTreeNodes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("GetChildCategoriesNodes", new { parentId });
            }
        }

        [HttpGet]
        public ActionResult Category(int id, CategoryViewType viewType)
        {
            IEnumerable<CategoryModel> categories;
            if (id == Common.Helper.CommonConstants.SHARED_ROOT_FOLDER_ID)
            {
                categories = _categoryDomain.GetSharedRootCategories(User.Identity.GetUserId())
                    .OrderBy(c => c.Title)
                    .ToList()
                    .Select(MapCategoryToCategoryModel);
            }
            else
            {
                categories = _categoryDomain.GetChildCategories(id)
                    .OrderBy(c => c.Title)
                    .ToList()
                    .Select(MapCategoryToCategoryModel);
            }



            IEnumerable<ChartObjectModel> chartObjects = _chartObjectDomain.GetByCategoryId(id)
                .OrderBy(ca => ca.Title)
                .ToList()
                .Select(MapChartObjectToChartObjectModel);

            var categoryContentModel = new CategoryContentModel
            {
                Categories = categories,
                Charts = chartObjects
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView(GetCategoryContentViewName(viewType), categoryContentModel);
            }
            else
            {

                var selectedCategory = _categoryDomain.GetById(id);
                var selectedCategoryModel = MapCategoryToCategoryModel(selectedCategory);
                if (selectedCategoryModel != null)
                {
                    ViewData["hierarchy"] = Newtonsoft.Json.JsonConvert.SerializeObject(selectedCategoryModel.Hierarchy);
                    ViewData["viewType"] = viewType.ToString();
                }

                LoadUserChartConfigurations();

                return View("Index");
            }

        }

        [HttpGet]
        public ActionResult SearchCategories(string searchTerm, CategoryViewType viewType)
        {
            var categories = _categoryDomain.SearchByTitleAndDescription(searchTerm).ToList().Select(MapCategoryToCategoryModel);
            var chartObjects = _chartObjectDomain.SearchByTitleAndDescription(searchTerm).ToList().Select(MapChartObjectToChartObjectModel);

            var categoryContentModel = new CategoryContentModel
            {
                Categories = categories,
                Charts = chartObjects
            };
            return PartialView(GetCategoryContentViewName(viewType), categoryContentModel);
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "parentId", Permission = Permissions.Create)]
        public ActionResult CreateCategory(int parentId, string categoryTitle)
        {
            var parent = _categoryDomain.GetById(parentId);
            if (parent == null) return null;

            var category = new Category(categoryTitle)
            {
                ParentCategoryId = parentId,
                OwnerId = parent.IsEveryones ? null : parent.OwnerId,
                IsEveryones = parent.IsEveryones
            };
            _categoryDomain.Add(category);
            return Json(MapCategoryToTreeNodeModel(category));
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "categoryId", Permission = Permissions.Edit)]
        public ActionResult RenameCategory(int categoryId, string newTitle)
        {
            if (string.IsNullOrEmpty(newTitle)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "New category title is empty");
            var categoryToEdit = _categoryDomain.GetById(categoryId);
            categoryToEdit.Title = newTitle;
            if (categoryToEdit.HasParent)
                _categoryDomain.GetUniqueCategoryName(categoryToEdit, categoryToEdit.ParentCategoryId.Value);

            _categoryDomain.Update(categoryToEdit);

            return Json(categoryToEdit);
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "categoryId", Permission = Permissions.Edit)]
        public void EditCategoryDescription(int categoryId, string newDescription)
        {
            var categoryToEdit = _categoryDomain.GetById(categoryId);
            categoryToEdit.Description = newDescription.Trim();
            _categoryDomain.Update(categoryToEdit);
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "newParentId", Permission = Permissions.Create)]
        [CategoryAuthorize(CategoryIdRequestName = "categoryId", Permission = Permissions.Delete)]
        public ActionResult MoveCategory(int categoryId, int newParentId)
        {
            var category = _categoryDomain.Move(categoryId, newParentId);
            return Json(MapCategoryToTreeNodeModel(category));
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "categoryToCopyId", Permission = Permissions.Copy)]
        public void CopyCategory(int categoryToCopyId, int parentCategoryId)
        {
            _categoryDomain.Copy(categoryToCopyId, parentCategoryId);
        }

        [HttpPost]
        [CategoryAuthorize(CategoryIdRequestName = "categoryId", Permission = Permissions.Delete)]
        public void DeleteCategory(int categoryId)
        {
            _categoryDomain.Delete(categoryId);
        }

        //[HttpPost]
        //[CategoryAuthorize(CategoryIdRequestName = "categoryId", Permission = Permissions.Create)]
        //public ActionResult CreateChartObject(string title, string description, int categoryId)
        //{
        //    var category = _categoryDomain.GetById(categoryId);
        //    var chartObjectToCreate = new ChartObject(title,description)
        //    {
        //        CategoryId = categoryId,
        //        OwnerId = category.IsEveryones ? null : category.OwnerId
        //    };

        //    var createdChartObject = _chartObjectDomain.Add(chartObjectToCreate);

        //    if (createdChartObject == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "The chart was not created");
        //    }

        //    return Json(createdChartObject);
        //}

        [HttpPost]
        [ChartObjectAuthorize(ChartObjectIdRequestName = "chartObjectId", Permission = Permissions.Copy)]
        [CategoryAuthorize(CategoryIdRequestName = "newCategoryId", Permission = Permissions.Create)]
        public void ChangeChartObjectCategory(int chartObjectId, int newCategoryId)
        {
            var chartObject = _chartObjectDomain.GetById(chartObjectId);
            var category = _categoryDomain.GetById(newCategoryId);
            //Allow move of chart object only to
            if (chartObject != null && category != null)
                _chartObjectDomain.ChangeCategory(chartObjectId, newCategoryId);
        }

        [HttpPost]
        [ChartObjectAuthorize(ChartObjectIdRequestName = "chartObjectId", Permission = Permissions.Edit)]
        public ActionResult RenameChartObject(int chartObjectId, string newTitle)
        {
            var chartObject = _chartObjectDomain.GetById(chartObjectId);
            if (chartObject == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "New chart object title is empty");
            chartObject.Title = newTitle;
            _chartObjectDomain.GetUniqueChartObjectName(chartObject, chartObject.CategoryId);

            _chartObjectDomain.Update(chartObject);

            return Json(chartObject);
        }

        [HttpPost]
        [ChartObjectAuthorize(ChartObjectIdRequestName = "chartObjectId", Permission = Permissions.Edit)]
        public void EditChartObjectDescription(int chartObjectId, string newDescription)
        {
            var chartObjectToEdit = _chartObjectDomain.GetById(chartObjectId);
            chartObjectToEdit.Description = newDescription.Trim();
            _chartObjectDomain.Update(chartObjectToEdit);
        }

        [HttpPost]
        [ChartObjectAuthorize(ChartObjectIdRequestName = "chartObjectId", Permission = Permissions.Copy)]
        [CategoryAuthorize(CategoryIdRequestName = "targetCategoryId", Permission = Permissions.Create)]
        public ActionResult CopyChartObject(int chartObjectId, int targetCategoryId)
        {
            var copiedChartObject = _chartObjectDomain.Copy(chartObjectId, targetCategoryId,User.Identity.GetUserId());

            return Json(copiedChartObject);
        }

        [HttpPost]
        [ChartObjectAuthorize(ChartObjectIdRequestName = "chartObjectId", Permission = Permissions.Delete)]
        public void DeleteChartObject(int chartObjectId)
        {
            _chartObjectDomain.Delete(chartObjectId);
        }

        private string GetCategoryContentViewName(CategoryViewType viewType)
        {
            switch (viewType)
            {
                case CategoryViewType.List:
                    return "_CategoryContentsList";
                case CategoryViewType.Grid:
                    return "_CategoryContentsGrid";
                default:
                    return "_CategoryContentsGrid";
            }
        }

        /// <summary>
        /// Map a category entity to a category model object
        /// </summary>
        /// <param name="category">Category entity to map</param>
        /// <returns>Mapped category model</returns>
        private CategoryModel MapCategoryToCategoryModel(Category category)
        {
            if (category == null) return null;
            bool isSharedCategory = category.OwnerId != User.Identity.GetUserId() && !category.IsEveryones;
            return new CategoryModel
            {
                Id = category.Id,
                Title = isSharedCategory && category.IsRoot && !category.IsEveryones ? (category.Owner?.UserName ?? category.Title) : category.Title,
                Description = category.Description,
                Hierarchy = _categoryDomain.GetParentHierarchy(category.Id),
                IsShared = isSharedCategory,
                IsEveryones = category.IsEveryones,
                IsRoot = category.IsRoot,
                AllowedActions = _permissionDomain.GetCategoryPermissions(category.Id, User.Identity.GetUserId()).Select(c => c.ToString())
            };
        }

        private ChartObjectModel MapChartObjectToChartObjectModel(ChartObject chartObject)
        {
            if (chartObject == null) return null;
            bool isSharedChartObject = !_permissionDomain.IsCategoryOwner(chartObject.Category.Id, User.Identity.GetUserId());
            return new ChartObjectModel
            {
                Id = chartObject.Id,
                Title = chartObject.Title,
                ChartType = Highsoft.Web.Mvc.Charts.ChartType.Line,
                Description = chartObject.Description,
                CategoryId = chartObject.CategoryId,
                IsShared = isSharedChartObject,
                IsEveryones = chartObject.IsEveryones,
                Thumbnail = chartObject.Thumbnail,
                AllowedActions = _permissionDomain.GetChartObjectPermissions(chartObject.Id, User.Identity.GetUserId()).Select(c => c.ToString())
            };
        }

        private TreeNodeModel MapCategoryToTreeNodeModel(Category category)
        {
            if (category == null) return null;
            bool isSharedCategory = !_permissionDomain.IsCategoryOwner(category.Id, User.Identity.GetUserId()) && !category.IsEveryones;

            return new TreeNodeModel
            {
                Key = category.Id.ToString(),
                Folder = true,
                IsRoot = category.IsRoot,
                Lazy = _categoryDomain.HasChildren(category.Id),
                Title = isSharedCategory && category.IsRoot && !category.IsEveryones && category.Owner != null ? category.Owner.UserName : category.Title,
                Description = category.Description,
                IsShared = isSharedCategory,
                IsEveryones = category.IsEveryones,
                AllowedActions = _permissionDomain.GetCategoryPermissions(category.Id, User.Identity.GetUserId()).Select(c => c.ToString())
            };
        }

        private void LoadUserChartConfigurations()
        {
            ViewBag.ChartConfigs = _chartsConfigDomain.GetUserConfigurations(User.Identity.GetUserId())
                .Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _categoryDomain.Dispose();
                _chartDataDomain.Dispose();
                _chartObjectDomain.Dispose();
                _chartsConfigDomain.Dispose();
                _userDomain.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}