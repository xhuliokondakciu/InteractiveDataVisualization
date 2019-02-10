using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.Jobs;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Services
{
    internal class ChartDataRepository : BaseRepository<int, ChartDataSource>, IChartDataRepository
    {
        public ChartDataRepository(KGTContext context) : base(context)
        {
        }
    }
}
