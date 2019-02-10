using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models.Identity;
using KGTMachineLearningWeb.Repository.Contracts;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Runtime.CompilerServices;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Domain.Services
{
    internal class UserDomain : BaseDomain<string, ApplicationUser>,IUserDomain
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
