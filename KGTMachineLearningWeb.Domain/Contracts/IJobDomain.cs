using KGTMachineLearningWeb.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IJobDomain:IBaseDomain<int,JobStatus>
    {
        /// <summary>
        /// Get jobs that user haven't been notified for their ending yet
        /// </summary>
        /// <param name="userId">User to filter with</param>
        /// <returns>Enumerable of jobs</returns>
        IEnumerable<JobStatus> GetNotNotifiedByUserId(string userId);

        /// <summary>
        /// Get list of jobs started by the given user
        /// </summary>
        /// <param name="userId">User to filter with</param>
        /// <returns>Enumerable of jobs</returns>
        IEnumerable<JobStatus> GetByUserId(string userId);

        byte[] CreateOrGetExportedFileZip(int jobId);
    }
}
