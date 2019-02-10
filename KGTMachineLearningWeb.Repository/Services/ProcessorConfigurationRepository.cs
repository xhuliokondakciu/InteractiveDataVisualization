using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Models.ChartConfiguration;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Services
{
    class ProcessorConfigurationRepository : BaseRepository<int, ProcessorConfiguration>, IProcessorConfigurationRepository
    {
        public ProcessorConfigurationRepository(KGTContext context) : base(context)
        {
        }
    }
}
