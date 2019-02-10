using KGTMachineLearningWeb.Models;
using System.Collections.Generic;

namespace KGTMachineLearningWeb.Repository.Comparer
{
    public class PmPredictionsDateComparer : IComparer<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction>
    {
        public int Compare(PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction x, PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction y)
        {
            return x.DateTimeUtcDate.CompareTo(y.DateTimeUtcDate);
        }
    }
}