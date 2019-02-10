using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Domain.Services
{
    internal class NeuralNetworkDomain : INeuralNetworkDomain
    {
        private readonly IPmPredictionsRepository pmPredictionsRepository;

        private readonly IHighChartsDomain chartsDomain;

        public NeuralNetworkDomain(
            IPmPredictionsRepository pmPredictionsRepository,
            IHighChartsDomain chartsDomain)
        {
            this.pmPredictionsRepository = pmPredictionsRepository;
            this.chartsDomain = chartsDomain;
        }
        public Highcharts GetNeuralNetworkOptions()
        {
            //ToDo: change to get data from NN File

            var chartOptions = chartsDomain
                .GetNeuralNetworksDefaultOptions("Neural network data", 0, 256, -1, 1, 60000);

            chartOptions.Data = new Data
            {
                Csv = File.ReadAllText(HttpContext.Current.Server.MapPath("~/XmlData/NeuralNetworkData.csv"))
            };

            (chartOptions.Series.First() as HeatmapSeries).Tooltip = new HeatmapSeriesTooltip
            {
                HeaderFormat = "NN inputs<br/>",
                PointFormat = "<b>Date:</b> {point.x:%e %b, %Y %H:%M:%S.%L}<br/><b>Node:</b> {point.y}<br/><b>Value:</b> {point.value}"
            };

            return chartOptions;
        }
    }
}
