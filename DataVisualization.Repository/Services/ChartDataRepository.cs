using DataVisualization.Context;
using DataVisualization.Models.Chart;
using DataVisualization.Models.Jobs;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class ChartDataRepository : BaseRepository<int, ChartDataSource>, IChartDataRepository
    {
        public ChartDataRepository(VisContext context) : base(context)
        {
        }
    }
}
