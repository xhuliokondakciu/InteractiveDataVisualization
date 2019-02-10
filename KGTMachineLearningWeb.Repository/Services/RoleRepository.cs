using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Models.Identity;
using KGTMachineLearningWeb.Repository.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Services
{
    internal class RoleRepository : BaseRepository<string, IdentityRole>, IRoleRepository
    {
        public RoleRepository(KGTContext context) : base(context)
        {
            
        }

        public IEnumerable<IdentityRole> GetByUserId(string userId)
        {
            return _entities.Where(r => r.Users.Any(u => u.UserId == userId));
        }
    }
}
