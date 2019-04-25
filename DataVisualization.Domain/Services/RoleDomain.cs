using DataVisualization.Domain.Contracts;
using DataVisualization.Models.Identity;
using DataVisualization.Repository.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Domain.Services
{
    public class RoleDomain : BaseDomain<string, IdentityRole>, IRoleDomain
    {
        private IRoleRepository RoleRepository => repository as IRoleRepository;
        public RoleDomain(IRoleRepository repository) : base(repository)
        {
        }

        public IEnumerable<IdentityRole> GetByUserId(string userId)
        {
            return RoleRepository.GetByUserId(userId);
        }

        public IdentityRole GetByName(string roleName)
        {
            return RoleRepository.Get(r => r.Name.Equals(roleName,StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        }
    }
}
