using KGTMachineLearningWeb.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Models.Chart
{
    public class RbStreamWithDesiredPrediction
    {
        [Order]
        public double Price { get; set; }
        [Order]
        public double BusinessResultBuyPpm { get; set; }
        [Order]
        public double BusinessResultSellPpm { get; set; }
        [Order]
        public string BuySellNoneLabel { get; set; }

    }
}
