using KGTMachineLearningWeb.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IUserDomain : IBaseDomain<string, ApplicationUser>
    {
        bool IsInRole(string userId, UserRoles userRole);
    }
}
