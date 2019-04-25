using DataVisualization.Models;
using System;
using System.Collections.Generic;

namespace DataVisualization.Repository.Contracts
{
    public interface IPmPredictionsRepository
    {
        IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions();
        IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions(DateTimeOffset startDate, DateTimeOffset endDate);
        IEnumerable<string> GetTestData();
    }
}