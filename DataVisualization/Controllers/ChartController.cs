using DataVisualization.Attributes;
using DataVisualization.Domain.Contracts;
using DataVisualization.Domain.Factories;
using DataVisualization.Domain.Hubs;
using DataVisualization.Models;
using DataVisualization.Models.Jobs;
using Highsoft.Web.Mvc.Charts;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace DataVisualization.Controllers
{
    [Authorize]
    [CustomErrorHandle]
    public class ChartController : BaseController
    {
        private readonly HighsoftNamespace _highsoft;
        private readonly IChartObjectDomain _chartObjectDomain;
        public readonly IChartDataDomain _chartDataDomain;
        private readonly ChartHubManager _chartHubManager;
        private readonly ILogger _logger;

        public ChartController(
            HighsoftNamespace highsoft,
            IChartObjectDomain chartObjectDomain,
            IChartDataDomain chartDataDomain,
            ChartHubManager chartHubManager,
            ILogger logger)
        {
            _highsoft = highsoft;
            _chartObjectDomain = chartObjectDomain;
            _chartDataDomain = chartDataDomain;
            _chartHubManager = chartHubManager;
            _logger = logger;
        }

        // GET: Charts
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Thumbnail(int id)
        {
            try
            {
                var chartObject = _chartObjectDomain.GetById(id);
                if (chartObject == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find chart");

                if (chartObject.Thumbnail == null || chartObject.Thumbnail.Image == null || chartObject.Thumbnail.Image.Length == 0)
                {
                    return File("~/Content/icons/line-chart-96.png", "image/png");
                }

                return File(chartObject.Thumbnail.Image, "image/png");
            }
            catch (Exception)
            {

                return File("~/Content/icons/line-chart-96.png", "image/png");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadChartData(ChartFileModel fileModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var dataToProcess = new ChartDataToProcess(fileModel.File.FileName, fileModel.File.ContentLength);
                await fileModel.File.InputStream.ReadAsync(dataToProcess.ChartFile, 0, fileModel.File.ContentLength);
                _chartDataDomain.ProcessFile(dataToProcess, fileModel.CategoryId, fileModel.ChartTitle, fileModel.ConfigId);

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error happend while processing chart file " + fileModel.File.FileName + ", uploaded by user with id " + User.Identity.GetUserId(), ex);
            }
        }

        [HttpGet]
        public ActionResult GetHighchartOption(int id)
        {
            var chart = _chartObjectDomain.GetById(id);

            var highchartOption = HighchartsFactory.GetHighchartOptions(chart.ChartDataSource);
            //var highchartOption = foo();
            var chartOptionsJson = new HighsoftNamespace().GetJsonOptions(highchartOption).ToString();

            return Content(chartOptionsJson, "application/json");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chartObjectDomain.Dispose();
                _chartDataDomain.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}