using KGTMachineLearningWeb.Context;
using KGTMachineLearningWeb.Models.Jobs;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Services
{
    internal class JobStatusRepository : BaseRepository<int, JobStatus>, IJobStatusRepository
    {
        public JobStatusRepository(KGTContext context) : base(context)
        {
            
        }

        public IEnumerable<JobStatus> GetByStatus(JobProcessedStatus status)
        {
            return _entities.Where(js => js.Status == status);
        }
    }
}
