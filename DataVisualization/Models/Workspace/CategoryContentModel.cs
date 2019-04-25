using System.Collections.Generic;

namespace DataVisualization.Models.Workspace
{
    public class CategoryContentModel
    {
        public IEnumerable<CategoryModel> Categories { get; set; }
        public IEnumerable<ChartObjectModel> Charts { get; set; }
    }
}