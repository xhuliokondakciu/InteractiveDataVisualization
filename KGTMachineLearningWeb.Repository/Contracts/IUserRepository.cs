using KGTMachineLearningWeb.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Contracts
{
    public interface IUserRepository : IBaseRepository<string, ApplicationUser>
    {
    }
}
