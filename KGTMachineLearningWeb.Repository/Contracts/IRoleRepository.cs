using KGTMachineLearningWeb.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Contracts
{
    public interface IRoleRepository : IBaseRepository<string, IdentityRole>
    {
        IEnumerable<IdentityRole> GetByUserId(string userId);
    }
}
