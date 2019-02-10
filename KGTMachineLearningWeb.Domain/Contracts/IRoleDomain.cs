using KGTMachineLearningWeb.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IRoleDomain:IBaseDomain<string,IdentityRole>
    {
        IEnumerable<IdentityRole> GetByUserId(string userId);

        IdentityRole GetByName(string roleName);
    }
}
