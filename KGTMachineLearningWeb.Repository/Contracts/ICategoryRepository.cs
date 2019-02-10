using KGTMachineLearningWeb.Models.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Contracts
{
    public interface ICategoryRepository : IBaseRepository<int, Category>
    {
        IEnumerable<Category> SearchByTitleAndDescription(string searchTerm);

        Category GetRootByOwnerId(string ownerId);

    }
}
