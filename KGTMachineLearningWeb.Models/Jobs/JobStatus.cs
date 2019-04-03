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
    public class JobStatus : IEquatable<JobStatus>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as JobStatus);
        }

        public bool Equals(JobStatus other)
        {
            return other != null &&
                   Id == other.Id &&
                   UserFileName == other.UserFileName &&
                   SystemFileName == other.SystemFileName &&
                   ChartDataDirectory == other.ChartDataDirectory &&
                   UnprocessedDataFilePath == other.UnprocessedDataFilePath &&
                   Status == other.Status &&
                   EqualityComparer<ICollection<ChartDataSource>>.Default.Equals(ChartDataSources, other.ChartDataSources) &&
                   HangfireJobId == other.HangfireJobId &&
                   JobOutput == other.JobOutput &&
                   UserNotified == other.UserNotified &&
                   ChartsConfigId == other.ChartsConfigId &&
                   UserCreatorId == other.UserCreatorId &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(StartTime, other.StartTime) &&
                   EqualityComparer<DateTimeOffset?>.Default.Equals(EndTime, other.EndTime) &&
                   JobFinishTime.Equals(other.JobFinishTime);
        }

        public override int GetHashCode()
        {
            var hashCode = 1573362128;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserFileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemFileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChartDataDirectory);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UnprocessedDataFilePath);
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ICollection<ChartDataSource>>.Default.GetHashCode(ChartDataSources);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HangfireJobId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(JobOutput);
            hashCode = hashCode * -1521134295 + UserNotified.GetHashCode();
            hashCode = hashCode * -1521134295 + ChartsConfigId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserCreatorId);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset?>.Default.GetHashCode(StartTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset?>.Default.GetHashCode(EndTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(JobFinishTime);
            return hashCode;
        }

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

        public static bool operator ==(JobStatus left, JobStatus right)
        {
            return EqualityComparer<JobStatus>.Default.Equals(left, right);
        }

        public static bool operator !=(JobStatus left, JobStatus right)
        {
            return !(left == right);
        }
    }

    public enum JobProcessedStatus
    {
        Unprocessed = 0,
        Processing,
        Processed,
        ProcessedWithError,
        NoProcessNeeded
    }

    public class ChartDataToProcess : IEquatable<ChartDataToProcess>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartDataToProcess);
        }

        public bool Equals(ChartDataToProcess other)
        {
            return other != null &&
                   EqualityComparer<byte[]>.Default.Equals(ChartFile, other.ChartFile) &&
                   FileName == other.FileName;
        }

        public override int GetHashCode()
        {
            var hashCode = 496709813;
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(ChartFile);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            return hashCode;
        }

        public static bool operator ==(ChartDataToProcess left, ChartDataToProcess right)
        {
            return EqualityComparer<ChartDataToProcess>.Default.Equals(left, right);
        }

        public static bool operator !=(ChartDataToProcess left, ChartDataToProcess right)
        {
            return !(left == right);
        }
    }
}
