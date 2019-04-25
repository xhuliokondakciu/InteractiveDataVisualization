using DataVisualization.Common.Helper;
using DataVisualization.Domain.Contracts;
using DataVisualization.Models.Jobs;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Web;

namespace DataVisualization.Domain.Hubs
{
    public class ChartHubManager
    {
        private readonly IHubContext _context;
        private readonly IJobDomain _jobDomain;

        public ChartHubManager(IHubContext context,IJobDomain jobDomain)
        {
            _context = context;
            _jobDomain = jobDomain;
        }

        public bool UserIsConnected(string userName)
        {
            return ChartHub.Users.Contains(userName);
        }

        /// <summary>
        /// Notify the user
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="alertClass">Alert class to show to user</param>
        public void ShowAlert(string message, string alertClass)
        {
            _context.Clients.User(HttpContext.Current.User.Identity.Name).showAlert(message, alertClass);
        }

        public void ShowAlert(string userName, string message, string alertClass)
        {
            _context.Clients.User(userName).showAlert(message, alertClass);
        }

        public void ShowStarProcessAlert(string userName, JobStatus jobStatus)
        {
            var message = $"Started to process file \"{jobStatus.UserFileName}\".";
            _context.Clients.User(userName).showJobStatusUpdate(message, UserAlertClasses.INFO, jobStatus.Id);
        }

        public void ShowSuccessProcessAlert(string userName, JobStatus jobStatus)
        {
            var duration = jobStatus.GetProcessDuration();
            var message = $"File \"{jobStatus.UserFileName}\" processed successfully.";
            _context.Clients.User(userName).showJobStatusUpdate(message, UserAlertClasses.SUCCESS, jobStatus.Id);
        }

        public void ShowErrorProcessAlert(string userName, JobStatus jobStatus)
        {
            var message = $"Some error occurred while processing file \"{jobStatus.UserFileName}\".";

            _context.Clients.User(userName).showJobStatusUpdate(message, UserAlertClasses.ERROR, jobStatus.Id);
        }

        public void UpdateJobOutput(string userName, int jobId, string output)
        {
            _context.Clients.User(userName).updateOutput(jobId, output);
        }

        public void SetProcessStatus(string userName, bool success)
        {
            _context.Clients.User(userName).setProcessStatus(success);
        }

        public void ReloadCategory(string userName, int categoryId)
        {
            _context.Clients.User(userName).reloadNode(categoryId);
        }

        public void UpdateJobsBadge(string userName, string userId)
        {
            var runninJobs = _jobDomain.Get(j => j.UserCreatorId == userId && j.Status == JobProcessedStatus.Processing).Count();
            _context.Clients.User(userName).updateBadge(runninJobs);
        }
    }
}