using DataVisualization.Context;
using DataVisualization.Models.Jobs;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public class JobStatusRepository : BaseRepository<int, JobStatus>, IJobStatusRepository
    {
        public JobStatusRepository(VisContext context) : base(context)
        {
            
        }

        public IEnumerable<JobStatus> GetByStatus(JobProcessedStatus status)
        {
            return _entities.Where(js => js.Status == status);
        }
    }
}
