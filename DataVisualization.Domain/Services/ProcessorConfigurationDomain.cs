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
    public class ProcessorConfigurationDomain : BaseDomain<int, ProcessorConfiguration>, IProcessorConfigurationDomain
    {
        private IProcessorConfigurationRepository _processorConfigurationRepository;
        public ProcessorConfigurationDomain(IProcessorConfigurationRepository repository) : base(repository)
        {
            _processorConfigurationRepository = repository;
        }
    }
}
