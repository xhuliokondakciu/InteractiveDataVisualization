using Highsoft.Web.Mvc.Charts;

namespace DataVisualization.Domain.Contracts
{
    public interface IHighChartsDomain
    {
        /// <summary>
        /// Get default Highchart options for pm predictions series
        /// </summary>
        /// <param name="chartTitle">Chart title</param>
        /// <returns>Highcart options</returns>
        Highcharts GetPmPredictionsSeriesDefaultOptions(string chartTitle = null);

        /// <summary>
        /// Get default Highchart options for neural network hidden layers inputs
        /// </summary>
        /// <returns>Highcart options</returns>
        Highcharts GetNeuralNetworksDefaultOptions();
        
        /// <summary>
        /// Get default Highchart options for neural network hidden layers inputs
        /// </summary>
        /// <param name="chartTitle">Chart title</param>
        /// <param name="yAxisMin">Min value of y axis</param>
        /// <param name="yAxisMax">Max value of y axis</param>
        /// <param name="valueMin">Min value of the data</param>
        /// <param name="valueMax">Max value of the data</param>
        /// <param name="colSize">Column size for the heat map</param>
        /// <returns>Highcart options</returns>
        Highcharts GetNeuralNetworksDefaultOptions(string chartTitle, double yAxisMin, double yAxisMax, double valueMin, double valueMax, double colSize);
    }
}