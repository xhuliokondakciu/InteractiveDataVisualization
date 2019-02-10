using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Attributes;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Domain.Factories;
using KGTMachineLearningWeb.Domain.Hubs;
using KGTMachineLearningWeb.Models;
using KGTMachineLearningWeb.Models.Jobs;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace KGTMachineLearningWeb.Controllers
{
    [Authorize]
    [CustomErrorHandle]
    public class ChartController : BaseController
    {
        private readonly IPmPredictionsDomain _pmPredictionsDomain;
        private readonly INeuralNetworkDomain _neuralNetworkDomain;
        private readonly HighsoftNamespace _highsoft;
        private readonly IChartObjectDomain _chartObjectDomain;
        public readonly IChartDataDomain _chartDataDomain;
        private readonly ChartHubManager _chartHubManager;
        private readonly ILogger _logger;

        public ChartController(
            IPmPredictionsDomain pmPredictionsDomain,
            HighsoftNamespace highsoft,
            INeuralNetworkDomain neuralNetworkDomain,
            IChartObjectDomain chartObjectDomain,
            IChartDataDomain chartDataDomain,
            ChartHubManager chartHubManager,
            ILogger logger)
        {
            _pmPredictionsDomain = pmPredictionsDomain;
            _highsoft = highsoft;
            _neuralNetworkDomain = neuralNetworkDomain;
            _chartObjectDomain = chartObjectDomain;
            _chartDataDomain = chartDataDomain;
            _chartHubManager = chartHubManager;
            _logger = logger;
        }

        // GET: Charts
        public ActionResult Index()
        {
            ViewBag.chartsOptions = GetChartsOptions();

            return View();
        }

        private IEnumerable<Highcharts> GetChartsOptions()
        {
            var chartsOptions = new List<Highcharts>
            {
                _pmPredictionsDomain.GetInputs(),
                _pmPredictionsDomain.GetBid(),
                _pmPredictionsDomain.GetAsk(),
                _pmPredictionsDomain.GetOutput(),
                _pmPredictionsDomain.GetBusinessResultPpmPerLabel(),
                _pmPredictionsDomain.GetPredictedBusinessYield(),
                _pmPredictionsDomain.GetDesiredPredictionsBinary(),
                _pmPredictionsDomain.GetDesiredPredictionsBinaryHeatmap(),
                _neuralNetworkDomain.GetNeuralNetworkOptions()
            };

            return chartsOptions;
        }

        [HttpGet]
        public ActionResult Thumbnail(int id)
        {
            var chartObject = _chartObjectDomain.GetById(id);
            if (chartObject == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find chart");

            if(chartObject.Thumbnail == null || chartObject.Thumbnail.Image == null)
            {
                return File("~/Content/icons/line-chart-96.png", "image/png");
            }

            return File(chartObject.Thumbnail.Image, "image/png");
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
                _chartDataDomain.ProcessFile(dataToProcess, fileModel.CategoryId, fileModel.ChartTitle,fileModel.ConfigId);

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
    }
}