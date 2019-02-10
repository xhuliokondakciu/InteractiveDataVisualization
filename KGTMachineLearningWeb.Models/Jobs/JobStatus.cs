using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.ChartConfiguration;
using KGTMachineLearningWeb.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace KGTMachineLearningWeb.Models.Jobs
{
    public class JobStatus
    {
        public int Id { get; set; }
        public string UserFileName { get; set; }

        public string SystemFileName { get; set; }

        public string ChartDataDirectory { get; set; }

        [NotMapped]
        public string UnprocessedDataFilePath
        {
            get
            {
                return Path.Combine(ChartDataDirectory, SystemFileName);
            }
        }

        public JobProcessedStatus Status { get; set; }

        public virtual ICollection<ChartDataSource> ChartDataSources { get; set; }

        public string HangfireJobId { get; set; }

        public string JobOutput { get; set; }

        public bool UserNotified { get; set; }

        public int ChartsConfigId { get; set; }

        public virtual ChartsConfiguration ChartsConfig { get; set; }

        public string UserCreatorId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser UserCreator { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public DateTimeOffset JobFinishTime { get; set; }

        public TimeSpan? GetProcessDuration()
        {
            if (Status == JobProcessedStatus.Processing && StartTime.HasValue)
                return DateTimeOffset.Now.Subtract(StartTime.Value);
            else if (StartTime.HasValue && EndTime.HasValue)
            {
                return EndTime.Value.Subtract(StartTime.Value);
            }
            else
            {
                return null;
            }
        }
    }

    public enum JobProcessedStatus
    {
        Unprocessed = 0,
        Processing,
        Processed,
        ProcessedWithError
    }

    public class ChartDataToProcess
    {
        /// <summary>
        /// Create a ChartDataToProcess object
        /// </summary>
        /// <param name="fileName">Name of the chart data file</param>
        /// <param name="fileLength">Length of the file in bytes</param>
        public ChartDataToProcess(string fileName, int fileLength)
        {
            FileName = fileName;
            ChartFile = new byte[fileLength];
        }
        public byte[] ChartFile { get; set; }

        public string FileName { get; set; }
    }
}
