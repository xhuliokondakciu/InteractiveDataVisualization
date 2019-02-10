using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Models;
using System.Collections.Generic;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IPmPredictionsDomain
    {
        /// <summary>
        /// Get IEnumerable of pm predictions data
        /// </summary>
        /// <returns>IEnumerable of PM predictions</returns>
        IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions();

        /// <summary>
        /// Get Highcharts options of ASK data
        /// </summary>
        /// <returns>Highcharts options for ASK data</returns>
        Highcharts GetAsk();

        /// <summary>
        /// Get Highcharts options of BID data
        /// </summary>
        /// <returns>Highcharts options for BID data</returns>
        Highcharts GetBid();

        /// <summary>
        /// Get Highcharts options of business result ppm per label data
        /// </summary>
        /// <returns>Highcharts options for business result ppm per label data</returns>
        Highcharts GetBusinessResultPpmPerLabel();

        /// <summary>
        /// Get Highcharts options of input data
        /// </summary>
        /// <returns>Highcharts options for input data</returns>
        Highcharts GetInputs();

        /// <summary>
        /// Get Highcharts options of output data
        /// </summary>
        /// <returns>Highcharts options for output data</returns>
        Highcharts GetOutput();

        /// <summary>
        /// Get Highcharts options of predicted business yield data
        /// </summary>
        /// <returns>Highcharts options for predicted business yield data</returns>
        Highcharts GetPredictedBusinessYield();

        /// <summary>
        /// Get Highcharts options of desired predictions binary data
        /// </summary>
        /// <returns>Highcharts options for desired predictions binary data</returns>
        Highcharts GetDesiredPredictionsBinary();

        Highcharts GetDesiredPredictionsBinaryHeatmap();
    }
}