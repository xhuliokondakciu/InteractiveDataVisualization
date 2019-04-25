using DataVisualization.Context;
using DataVisualization.Models.Identity;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class UserRepository : BaseRepository<string, ApplicationUser>, IUserRepository
    {
        public UserRepository(VisContext context) : base(context)
        {
        }
    }
}
