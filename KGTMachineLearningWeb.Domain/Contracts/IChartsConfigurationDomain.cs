using KGTMachineLearningWeb.Models.ChartConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IChartsConfigurationDomain:IBaseDomain<int,ChartsConfiguration>
    {
        IEnumerable<ChartsConfiguration> GetUserConfigurations(string userId, int skip, int take, out int totalCount);
        IEnumerable<ChartsConfiguration> GetUserConfigurations(string userId);
    }
}
