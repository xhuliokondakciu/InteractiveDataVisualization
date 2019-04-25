using KGTMachineLearningWeb.Models.ChartConfiguration;
using KGTMachineLearningWeb.Models.Jobs;
using KGTMachineLearningWeb.Models.Workspace;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Category> OwnedCategories { get; set; }

        public virtual ICollection<JobStatus> Jobs { get; set; }

        public virtual ICollection<ChartsConfiguration> ChartsConfigurations { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
