using DataVisualization.Domain.Contracts;
using DataVisualization.Models.Identity;
using DataVisualization.Repository.Contracts;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Runtime.CompilerServices;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Domain.Services
{
    public class UserDomain : BaseDomain<string, ApplicationUser>,IUserDomain
    {
        private readonly IRoleDomain roleDomain;
        public UserDomain(IUserRepository repository,IRoleDomain roleDomain,UserManager<ApplicationUser> userManager) : base(repository)
        {
            this.roleDomain = roleDomain;
        }

        public bool IsInRole(string userId,UserRoles userRole)
        {
            var user = GetById(userId);
            var role = roleDomain.GetByName(userRole.ToString());
            if (user == null || role == null)
                return false;

            return user.Roles.Any(r => r.RoleId == role.Id);
        }
    }
}
