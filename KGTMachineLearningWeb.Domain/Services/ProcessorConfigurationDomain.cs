using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models.ChartConfiguration;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Domain.Services
{
    internal class ProcessorConfigurationDomain : BaseDomain<int, ProcessorConfiguration>, IProcessorConfigurationDomain
    {
        private IProcessorConfigurationRepository _processorConfigurationRepository;
        public ProcessorConfigurationDomain(IProcessorConfigurationRepository repository) : base(repository)
        {
            _processorConfigurationRepository = repository;
        }
    }
}
