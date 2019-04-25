using Hangfire;
using DataVisualization.Attributes;
using DataVisualization.Domain.Contracts;
using DataVisualization.Models;
using DataVisualization.Models.Jobs;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Controllers
{
    [Authorize]
    [CustomErrorHandle]
    public class JobsController : BaseController
    {
        private readonly IJobDomain _jobDomain;

        public JobsController(IJobDomain jobDomain)
        {
            _jobDomain = jobDomain;
        }
        // GET: Job
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Jobs(DataTableModel model)
        {
            int totalCount = 0;
            var userId = User.Identity.GetUserId();
            var jobs = _jobDomain
                .Get(j => j.UserCreatorId == userId, j => j.OrderByDescending(ij => ij.StartTime).ThenBy(ij => ij.UserFileName), model.Start, model.Length, out totalCount).Select(JobStatusToJobModelMapper);

            var retVal = new DataTableResponse<JobModel>
            {
                Draw = model.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = totalCount,
                Data = jobs
            };
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var job = _jobDomain.GetById(id);

            return View(JobStatusToJobModelMapper(job));
        }

        [HttpPost]
        public ActionResult Delete(int jobId)
        {
            var job = _jobDomain.GetById(jobId);
            if (job == null)
            {
                return new HttpNotFoundResult($"Job with id:{jobId} was not found");
            }
            if (job.HangfireJobId != null)
                BackgroundJob.Delete(job.HangfireJobId);
            _jobDomain.Delete(jobId);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult GetExportedData(int id)
        {
            var job = _jobDomain.GetById(id);
            if (job == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (job.UserCreatorId != User.Identity.GetUserId() && !User.IsInRole(UserRoles.Admin.ToString()))
                return new HttpUnauthorizedResult();

            var zip = _jobDomain.CreateOrGetExportedFileZip(id);

            return File(zip, "application/zip", "ExportedData.zip");
        }

        private JobModel JobStatusToJobModelMapper(JobStatus jobStatus)
        {
            return new JobModel
            {
                Id = jobStatus.Id,
                HangfireJobId = jobStatus.HangfireJobId,
                FileToProcess = jobStatus.UserFileName,
                Output = jobStatus.JobOutput,
                UserCreator = jobStatus.UserCreator.UserName,
                JobStartTime = jobStatus.StartTime.HasValue ? jobStatus.StartTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A",
                JobEndTime = jobStatus.EndTime.HasValue ? jobStatus.EndTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A",
                JobDuration = jobStatus.GetProcessDuration().HasValue ? jobStatus.GetProcessDuration().Value.ToString(@"hh\:mm\:ss") : "N/A",
                Status = jobStatus.Status.ToString()
            };
        }




    }
}