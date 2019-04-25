using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataVisualization.Models
{
    public class JobModel
    {
        public int Id { get; set; }

        [Display(Name ="Job id")]
        public string HangfireJobId { get; set; }

        [Display(Name = "File to process")]
        public string FileToProcess { get; set; }

        [Display(Name = "Job output")]
        public string Output { get; set; }

        [Display(Name = "User creator name")]
        public string UserCreator { get; set; }

        [Display(Name = "Job start time")]
        public string JobStartTime { get; set; }

        [Display(Name = "Job end time")]
        public string JobEndTime { get; set; }

        [Display(Name = "Job duration")]
        public string JobDuration { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}