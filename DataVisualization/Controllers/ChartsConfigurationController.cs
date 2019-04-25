using DataVisualization.Attributes;
using DataVisualization.Common.Exceptions;
using DataVisualization.Domain.Contracts;
using DataVisualization.Models;
using DataVisualization.Models.ChartConfiguration;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Controllers
{
    [Authorize]
    [CustomErrorHandle]
    public class ChartsConfigurationController : BaseController
    {
        private readonly IChartsConfigurationDomain _chartsConfigDomain;
        private readonly IProcessorConfigurationDomain _processorConfigDomain;

        public ChartsConfigurationController(IChartsConfigurationDomain domain, IProcessorConfigurationDomain processorConfigDomain)
        {
            _chartsConfigDomain = domain;
            _processorConfigDomain = processorConfigDomain;
        }
        // GET: ChartsConfiguration
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ChartsConfig(DataTableModel model)
        {
            int totalCount = 0;
            var processors = _chartsConfigDomain.GetUserConfigurations(User.Identity.GetUserId(), model.Start, model.Length, out totalCount).ToList();

            var retVal = new DataTableResponse<ChartsConfiguration>
            {
                Draw = model.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
                Data = processors
            };
            return Json(retVal);
        }

        // GET: ChartsConfiguration/Details/5
        public ActionResult Details(int id)
        {
            var config = _chartsConfigDomain.GetById(id);
            if (config == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Chart config could not be found");
            }

            return View(config);
        }

        // GET: ChartsConfiguration/Create
        public ActionResult Create()
        {
            var processors = _processorConfigDomain.GetAll().OrderBy(p => p.Name);

            ViewBag.Processors = processors.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            });

            var config = new ChartsConfiguration
            {
                ConfigurationXml = ViewBag.ChartConfigTemplate = LoadChartConfigTemplate()
            };

            return View(config);
        }

        // POST: ChartsConfiguration/Create
        [HttpPost]
        public ActionResult Create(ChartsConfiguration config)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(config);
                }


                bool isSystem = false;
                if (User.IsInRole(UserRoles.Admin.ToString()))
                {
                    isSystem = config.IsSystem;
                }

                try
                {
                    ChartsConfiguration.SchemaIsValid(config.ConfigurationXml);
                }
                catch (ChartSchemaValidationException ex)
                {
                    ModelState.AddModelError("ConfigurationXml", ex.Message);

                    return View(config);
                }

                var configToCreate = new ChartsConfiguration(config.Title, config.ProcessorId, config.ConfigurationXml, User.Identity.GetUserId(), isSystem);
                
                _chartsConfigDomain.Add(configToCreate);

                ViewBag.ChartConfigTemplate = LoadChartConfigTemplate();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(config);
            }
            finally
            {
                var processors = _processorConfigDomain.GetAll().OrderBy(p => p.Name);

                ViewBag.Processors = processors.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                });
            }
        }
        
        // GET: ChartsConfiguration/Edit/5
        public ActionResult Edit(int id)
        {
            var config = _chartsConfigDomain.GetById(id);

            if(config.IsSystem && !User.IsInRole(UserRoles.Admin.ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            var processors = _processorConfigDomain.GetAll().OrderBy(p => p.Name);

            ViewBag.Processors = processors.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            });

            if (config == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Chart config could not be found");
            ViewBag.currentXmlValue = config.ConfigurationXml;

            return View(config);
        }

        // POST: ChartsConfiguration/Edit/5
        [HttpPost]
        public ActionResult Edit(ChartsConfiguration config)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var processors = _processorConfigDomain.GetAll().OrderBy(p => p.Name);

                    ViewBag.Processors = processors.Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    });
                    return View(config);
                }

                var configToEdit = _chartsConfigDomain.GetById(config.Id);
                if (configToEdit == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Chart config could not be found");

                if (configToEdit.IsSystem && !User.IsInRole(UserRoles.Admin.ToString()))
                {
                    return new HttpUnauthorizedResult();
                }

                ViewBag.currentXmlValue = configToEdit.ConfigurationXml;

                configToEdit.Title = config.Title;
                configToEdit.ProcessorId = config.ProcessorId;
                configToEdit.ConfigurationXml = config.ConfigurationXml;
                configToEdit.RequiresProcess = config.RequiresProcess;

                try
                {
                    ChartsConfiguration.SchemaIsValid(config.ConfigurationXml);
                }
                catch (ChartSchemaValidationException ex)
                {
                    ModelState.AddModelError("ConfigurationXml", ex.Message);
                    var processors = _processorConfigDomain.GetAll().OrderBy(p => p.Name);

                    ViewBag.Processors = processors.Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    });
                    return View(config);
                }

                if (User.IsInRole(UserRoles.Admin.ToString()))
                {
                    configToEdit.IsSystem = config.IsSystem;
                }

                _chartsConfigDomain.Update(configToEdit);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(config);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_chartsConfigDomain.Delete(id))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Chart config could not be found");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        private string LoadChartConfigTemplate()
        {
            try
            {
                var configPath = Server.MapPath(ConfigurationManager.AppSettings["ChartConfigTemplate"]);
                return System.IO.File.ReadAllText(configPath);
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
