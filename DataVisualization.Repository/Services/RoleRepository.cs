using DataVisualization.Context;
using DataVisualization.Models.Identity;
using DataVisualization.Repository.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class RoleRepository : BaseRepository<string, IdentityRole>, IRoleRepository
    {
        public RoleRepository(VisContext context) : base(context)
        {
            
        }

        public IEnumerable<IdentityRole> GetByUserId(string userId)
        {
            return _entities.Where(r => r.Users.Any(u => u.UserId == userId));
        }
    }
}
