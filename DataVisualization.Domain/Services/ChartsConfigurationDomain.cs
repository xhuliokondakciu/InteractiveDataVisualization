using DataVisualization.Domain.Contracts;
using DataVisualization.Models.ChartConfiguration;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Domain.Services
{
    public class ChartsConfigurationDomain : BaseDomain<int, ChartsConfiguration>, IChartsConfigurationDomain
    {
        private readonly IChartsConfigurationRepository _chartsConfigurationRepository;
        public ChartsConfigurationDomain(IChartsConfigurationRepository repository) : base(repository)
        {
            _chartsConfigurationRepository = repository;
        }

        public IEnumerable<ChartsConfiguration> GetUserConfigurations(string userId, int skip, int take,out int totalCount)
        {
            return _chartsConfigurationRepository
                .Get(
                c => c.IsSystem || c.UserId == userId,
                c => c.OrderBy(cn => cn.Title),
                skip,
                take,
                out totalCount);
        }

        public IEnumerable<ChartsConfiguration> GetUserConfigurations(string userId)
        {
            return _chartsConfigurationRepository
                .Get(
                c => c.IsSystem || c.UserId == userId,
                c => c.OrderBy(cn => cn.Title));
        }
    }
}
