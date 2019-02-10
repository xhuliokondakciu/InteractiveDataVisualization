using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Contracts
{
    public interface IChartDataRepository :IBaseRepository<int,ChartDataSource>
    {
    }
}
