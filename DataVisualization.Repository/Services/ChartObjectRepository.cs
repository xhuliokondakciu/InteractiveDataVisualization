using DataVisualization.Context;
using DataVisualization.Models.Workspace;
using DataVisualization.Repository.Contracts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataVisualization.Repository.Services
{
    public class ChartObjectRepository : BaseRepository<int, ChartObject>, IChartObjectRepository
    {
        public ChartObjectRepository(VisContext context) : base(context)
        {
        }

        public IEnumerable<ChartObject> GetByCategoryId(int categoryId)
        {
            var retVal = _entities.Where(co => co.CategoryId == categoryId);

            return retVal;
        }

        public IEnumerable<ChartObject> SearchByTitleAndDescription(string searchTerm)
        {
            var lowerCaseSearchTerm = searchTerm.ToLower();
            var retVal = _entities.Where(co => co.Title.ToLower().Contains(lowerCaseSearchTerm)
            || co.Description.ToLower().Contains(lowerCaseSearchTerm));

            return retVal;
        }
        
        public override ChartObject Update(ChartObject chartObject)
        {
            if (context.Entry(chartObject).State == EntityState.Detached)
                _entities.Attach(chartObject);
            context.Entry(chartObject).State = EntityState.Modified;

            if (context.Entry(chartObject.Thumbnail).State == EntityState.Detached)
                context.Set<Thumbnail>().Attach(chartObject.Thumbnail);
            context.Entry(chartObject.Thumbnail).State = EntityState.Modified;

            var retVal = Save();
            return chartObject;
        }
    }
}
