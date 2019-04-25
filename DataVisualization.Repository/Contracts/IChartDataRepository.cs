using DataVisualization.Models.Chart;
using DataVisualization.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Contracts
{
    public interface IChartDataRepository :IBaseRepository<int,ChartDataSource>
    {
    }
}
