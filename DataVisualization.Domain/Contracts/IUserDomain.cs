using DataVisualization.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Domain.Contracts
{
    public interface IUserDomain : IBaseDomain<string, ApplicationUser>
    {
        bool IsInRole(string userId, UserRoles userRole);
    }
}
