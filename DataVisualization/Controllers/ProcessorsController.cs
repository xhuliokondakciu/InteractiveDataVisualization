using DataVisualization.Attributes;
using DataVisualization.Domain.Contracts;
using DataVisualization.Models;
using DataVisualization.Models.ChartConfiguration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DataVisualization.Controllers
{
    [Authorize(Roles = "Admin")]
    [CustomErrorHandle]
    public class ProcessorsController : BaseController
    {
        private readonly IProcessorConfigurationDomain _processorConfigurationDomain;

        public ProcessorsController(IProcessorConfigurationDomain processorConfigurationDomain)
        {
            _processorConfigurationDomain = processorConfigurationDomain;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Processors(DataTableModel model)
        {
            int totalCount = 0;
            var processors = _processorConfigurationDomain.Get(p => p.OrderBy(pr => pr.Name), model.Start, model.Length, out totalCount).ToList();

            var retVal = new DataTableResponse<ProcessorConfiguration>
            {
                Draw = model.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
                Data = processors
            };
            return Json(retVal);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProcessorConfiguration config)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _processorConfigurationDomain.Add(config);

            return RedirectToAction("Index");
        }

        
        public ActionResult Edit(int id)
        {
            var config = _processorConfigurationDomain.GetById(id);
            return View(config);
        }

        
        [HttpPost]
        public ActionResult Edit(ProcessorConfiguration config)
        {
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            _processorConfigurationDomain.Update(config);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = _processorConfigurationDomain.Delete(id);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
