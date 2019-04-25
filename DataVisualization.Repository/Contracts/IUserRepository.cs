using DataVisualization.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Contracts
{
    public interface IUserRepository : IBaseRepository<string, ApplicationUser>
    {
    }
}
