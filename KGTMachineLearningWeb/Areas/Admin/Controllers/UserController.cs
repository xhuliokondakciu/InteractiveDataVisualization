using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using KGTMachineLearningWeb.Areas.Models.User;
using KGTMachineLearningWeb.Attributes;
using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Controllers;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models;
using KGTMachineLearningWeb.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IUserDomain _userDomain;
        private readonly IRoleDomain _roleDomain;
        private readonly ICategoryDomain _categoryDomain;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserDomain userDomain, IRoleDomain roleDomain, ICategoryDomain categoryDomain, UserManager<ApplicationUser> userManager)
        {
            this._userDomain = userDomain;
            this._roleDomain = roleDomain;
            this._categoryDomain = categoryDomain;
            this._userManager = userManager;
        }

        // GET: User
        public ActionResult Index()
        {
            var users = _userDomain.GetAll();
            return View(users);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetUsers(DataTableModel model)
        {
            if (model.Order.FirstOrDefault() == null && model.Order.FirstOrDefault()?.Dir == null)
            {
                model.Order.FirstOrDefault().Dir = "asc";
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid request");

            string orderColumn = model.Columns[model.Order[0].Column].Data;
            string orderDir = model.Order[0].Dir;
            string orderBy = $"{orderColumn} {orderDir}";
            string searchValue = model.Search.Value;
            int totalCount = 0;
            IEnumerable<ApplicationUser> users;
            if (string.IsNullOrEmpty(searchValue))
            {
                users = _userDomain.Get(null, orderBy, model.Start, model.Length, out totalCount).ToList();
            }
            else
            {
                users = _userDomain.Get(u => u.UserName.ToLower().Contains(searchValue.ToLower()), orderBy, model.Start, model.Length, out totalCount).ToList();
            }

            var retVal = new DataTableResponse<UserModel>
            {
                Draw = model.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
                Data = users.Select(MapApplicationUserToUserModel)
            };
            return Json(retVal);
        }

        private UserModel MapApplicationUserToUserModel(ApplicationUser user)
        {
            var userRoles = _userManager.GetRoles(user.Id);

            return new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles
            };
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = _userDomain.GetById(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Admin/User/Create
        [HttpGet]
        public ActionResult Create()
        {
            var createModel = new CreateModel();
            var roles = _roleDomain.GetAll().OrderBy(r => r.Name);
            ViewBag.Role = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == roles.FirstOrDefault()?.Name
            });
            createModel.Role = roles.FirstOrDefault()?.Name;
            return PartialView("Modals/_CreateUserModal", createModel);
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Email,UserName,Password,ConfirmPassword,Role")] CreateModel userToCreate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        Email = userToCreate.Email,
                        UserName = userToCreate.UserName
                    };
                    var createUserResult = _userManager.Create(user, userToCreate.Password);

                    if (createUserResult.Succeeded)
                    {
                        _userManager.AddToRole(user.Id, userToCreate.Role);

                        _categoryDomain.CreateRootCategoryForUser(user.Id);
                        ViewBag.Success = true;
                        return new HttpStatusCodeResult(HttpStatusCode.Created, $"User {user.UserName} created successfully");
                    }
                    else
                    {
                        AddErrors(createUserResult);
                    }
                }
                catch (InvalidOperationException)
                {

                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User couldn't be created");
                }
                catch (Exception)
                {
                    throw;
                }

            }

            var roles = _roleDomain.GetAll().OrderBy(r => r.Name);
            ViewBag.Role = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == roles.FirstOrDefault()?.Name
            });

            return PartialView("Modals/_CreateUserModal", userToCreate);
        }

        // GET: Admin/User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = _userDomain.GetById(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Email,PhoneNumber,LockoutEnabled,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _userDomain.Update(applicationUser);
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Admin/User/Delete/5
        [AjaxOnly]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = _userDomain.GetById(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var isDeleted = _userDomain.Delete(id);
            if (!isDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GetRoles()
        {
            var roles = _roleDomain.GetAll();

            return Json(roles);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
