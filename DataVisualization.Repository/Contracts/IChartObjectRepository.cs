using DataVisualization.Models.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Contracts
{
    public interface IChartObjectRepository : IBaseRepository<int, ChartObject>
    {
        IEnumerable<ChartObject> GetByCategoryId(int categoryId);

        IEnumerable<ChartObject> SearchByTitleAndDescription(string searchTerm);
    }
}
