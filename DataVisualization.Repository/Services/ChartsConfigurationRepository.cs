using DataVisualization.Context;
using DataVisualization.Models.ChartConfiguration;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class ChartsConfigurationRepository : BaseRepository<int, ChartsConfiguration>, IChartsConfigurationRepository
    {
        public ChartsConfigurationRepository(VisContext context) : base(context)
        {
        }
    }
}
