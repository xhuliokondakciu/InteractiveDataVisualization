using KGTMachineLearningWeb.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Repository.Contracts
{
    public interface IJobStatusRepository:IBaseRepository<int,JobStatus>
    {
        /// <summary>
        /// Get data to process by its status
        /// </summary>
        /// <param name="status">Status to filter from</param>
        /// <returns>Enumerable of data to process with the specified status</returns>
        IEnumerable<JobStatus> GetByStatus(JobProcessedStatus status);
    }
}
