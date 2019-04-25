using DataVisualization.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Domain.Contracts
{
    public interface IRoleDomain:IBaseDomain<string,IdentityRole>
    {
        IEnumerable<IdentityRole> GetByUserId(string userId);

        IdentityRole GetByName(string roleName);
    }
}
