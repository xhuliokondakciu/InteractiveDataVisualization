using Hangfire;
using DataVisualization.Common.Helper;
using DataVisualization.Domain.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataVisualization.Domain.Hubs
{
    public class ChartHub : Hub
    {
        public static HashSet<string> Users = new HashSet<string>();
        private readonly IJobDomain _jobDomain;
        private readonly ChartHubManager _hubManager;

        public ChartHub(IJobDomain jobDomain,ChartHubManager hubManager)
        {
            _jobDomain = jobDomain;
            _hubManager = hubManager;
        }

        public void CheckJobs()
        {
            var jobs = _jobDomain.GetNotNotifiedByUserId(Context.User.Identity.GetUserId());
            foreach (var job in jobs)
            {
                if (job.Status == Models.Jobs.JobProcessedStatus.Processed)
                {
                    _hubManager.ShowSuccessProcessAlert(Context.User.Identity.Name, job);
                }
                else if (job.Status == Models.Jobs.JobProcessedStatus.ProcessedWithError)
                {
                    _hubManager.ShowErrorProcessAlert(Context.User.Identity.Name, job);
                }
                job.UserNotified = true;
                _jobDomain.Update(job);
            }
        }

        //public void ShowAlert(string message, string alertClass)
        //{
        //    Clients.User(Context.User.Identity.Name).showAlert(message, alertClass);
        //}

        //public void ShowAlert(string userName, string message, string alertClass)
        //{
        //    Clients.User(userName).showAlert(message, alertClass);
        //}

        public override Task OnConnected()
        {
            Users.Add(Context.User.Identity.Name);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Users.Remove(Context.User.Identity.Name);
            return base.OnDisconnected(stopCalled);
        }
    }
}