using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Models.Identity;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Services
{
    internal class UserRepository : BaseRepository<string, ApplicationUser>, IUserRepository
    {
        public UserRepository(KGTContext context) : base(context)
        {
        }
    }
}
