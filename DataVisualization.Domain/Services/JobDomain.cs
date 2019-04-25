using DataVisualization.Domain.Contracts;
using DataVisualization.Models.Jobs;
using DataVisualization.Repository.Contracts;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace DataVisualization.Domain.Services
{
    public class JobDomain : BaseDomain<int, JobStatus>, IJobDomain
    {
        private IJobStatusRepository _jobStatusRepository => (IJobStatusRepository)repository;
        public JobDomain(IJobStatusRepository repository) : base(repository)
        {
        }

        public IEnumerable<JobStatus> GetNotNotifiedByUserId(string userId)
        {
            return repository.Get(cd => cd.UserCreatorId == userId && !cd.UserNotified);
        }

        public IEnumerable<JobStatus> GetByUserId(string userId)
        {
            return repository.Get(cd => cd.UserCreatorId == userId);
        }

        public byte[] CreateOrGetExportedFileZip(int jobId)
        {
            var job = GetById(jobId);
            string zipFileName = "ExportedData.zip";
            string rootZipPath = HttpContext.Current.Server.MapPath(@"~\ExportedData\Job_" + jobId);
            Directory.CreateDirectory(rootZipPath);
            string zipPath = Path.Combine(rootZipPath, zipFileName);
            if (!File.Exists(zipPath))
            {
                ZipFile.CreateFromDirectory(job.ChartDataDirectory, zipPath,CompressionLevel.Optimal,false);
            }
            byte[] zip = File.ReadAllBytes(zipPath);
            return zip;
        }
    }
}
