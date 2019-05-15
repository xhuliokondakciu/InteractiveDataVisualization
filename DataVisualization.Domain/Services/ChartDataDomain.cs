using Hangfire;
using DataVisualization.Common.Helper;
using DataVisualization.Domain.Contracts;
using DataVisualization.Domain.Factories;
using DataVisualization.Domain.Hubs;
using DataVisualization.Models.Chart;
using DataVisualization.Models.ChartConfiguration;
using DataVisualization.Models.Jobs;
using DataVisualization.Models.Workspace;
using DataVisualization.Repository.Contracts;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataVisualization.Domain.Services
{
    public class ChartDataDomain : BaseDomain<int, ChartDataSource>, IChartDataDomain
    {
        private readonly IChartObjectDomain _chartObjectDomain;
        private readonly ICategoryDomain _categoryDomain;
        private readonly ILogger _logger;
        private readonly ChartHubManager _chartHubManager;
        private readonly IJobDomain _jobDomain;
        private const int _dateColumnIndex = 2;

        private IChartDataRepository _chartDataRepository => repository as IChartDataRepository;

        private string _rootChartDataPath
        {
            get
            {
                var chartDataPath = System.Configuration.ConfigurationManager.AppSettings["ChartDataPath"];

                if (Path.IsPathRooted(chartDataPath))
                {
                    return chartDataPath;
                }

                return HttpContext.Current.Server.MapPath(chartDataPath);
            }
        }

        private string _userId
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
        }

        private string _userName
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserName();
            }
        }

        public ChartDataDomain(
            IChartDataRepository repository,
            IChartObjectDomain chartObjectDomain,
            ICategoryDomain categoryDomain,
            ILogger logger,
            ChartHubManager chartHubManager,
            IJobDomain jobDomain) : base(repository)
        {
            _chartObjectDomain = chartObjectDomain;
            _categoryDomain = categoryDomain;
            _logger = logger;
            _chartHubManager = chartHubManager;
            _jobDomain = jobDomain;
        }

        public void ProcessFile(ChartDataToProcess dataToProcess, int categoryId, string chartTitle, int configId)
        {
            try
            {
                if (dataToProcess.ChartFile == null)
                    throw new ArgumentNullException();

                var dataDirectory = FileSystemHelper.CreateChartDataDirectory(_userId);
                var jobStatus = new JobStatus
                {
                    UserFileName = dataToProcess.FileName,
                    SystemFileName = dataToProcess.FileName,
                    StartTime = DateTimeOffset.Now,
                    EndTime = null,
                    UserCreatorId = _userId,
                    ChartDataDirectory = dataDirectory,
                    Status = JobProcessedStatus.Unprocessed,
                    ChartsConfigId = configId
                };

                _jobDomain.Add(jobStatus);

                File.WriteAllBytes(jobStatus.UnprocessedDataFilePath, dataToProcess.ChartFile);

                var projectRoot = HttpContext.Current.Server.MapPath(@"~/");
                //Start background job to process chart data file
                var jobId = BackgroundJob.Enqueue<IChartDataDomain>(cdd =>
                cdd.ProcessData(
                    jobStatus.Id,
                    projectRoot,
                    categoryId,
                    chartTitle,
                    _userName,
                    _userId,
                    JobCancellationToken.Null));

                jobStatus.HangfireJobId = jobId;
                _jobDomain.Update(jobStatus);
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, "Chart data file is null or emtpy");
                throw;
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Error(ex, "Chart data directory doesn't exist");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Error(ex, "Couldn't access chart data directory. Access denied.");
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public void ProcessData(
            int jobStatusId,
            string projectRootPath,
            int categoryId,
            string chartTitle,
            string userName,
            string userId,
            IJobCancellationToken cancellationToken)
        {
            //Throws when hangfire background job is deleted
            //Here to stop the execution when deleting jobs
            cancellationToken.ThrowIfCancellationRequested();

            var jobStatus = _jobDomain.GetById(jobStatusId);
            Category createdCategory = null;
            try
            {
                var output = new StringBuilder();
                if (jobStatus.ChartsConfig.RequiresProcess)
                {
                    _chartHubManager.ShowStarProcessAlert(userName, jobStatus);
                    jobStatus.Status = JobProcessedStatus.Processing;
                    _jobDomain.Update(jobStatus);

                    jobStatus.StartTime = DateTimeOffset.Now;

                    var proc = CreateChartDataProcess(jobStatus.UnprocessedDataFilePath, jobStatus.ChartDataDirectory, jobStatus.ChartsConfig);
                    _logger.Info($"Starting process {proc.StartInfo.FileName} with parameters {proc.StartInfo.Arguments}");

                    proc.Start();

                    _chartHubManager.UpdateJobsBadge(userName, userId);

                    

                    StreamReader stdOut = proc.StandardOutput;
                    StreamReader stdError = proc.StandardError;
                    //Synchronous read is causing issues - either for quickyl finishing jobs with lot of output (EndOfStream hangs) or for slow output processes (Peek() hangs)
                    Task stdoutReadingTask = Task.Factory.StartNew(() =>
                    {
                        while (!stdOut.EndOfStream)
                        {
                            var line = stdOut.ReadLine();
                            output.AppendLine(line);
                            jobStatus.JobOutput = output.ToString();
                            _jobDomain.Update(jobStatus);
                            _chartHubManager.UpdateJobOutput(userName, jobStatus.Id, line);
                        }

                        while (!stdError.EndOfStream)
                        {
                            var line = stdError.ReadLine();
                            output.AppendLine(line);
                            jobStatus.JobOutput = output.ToString();
                            _jobDomain.Update(jobStatus);
                            _chartHubManager.UpdateJobOutput(userName, jobStatus.Id, line);
                        }
                    }
                    );

                    string stderr = proc.StandardError.ReadToEnd();

                    proc.WaitForExit();

                    if (proc.ExitCode == 1)
                    {
                        _chartHubManager.UpdateJobOutput(userName, jobStatus.Id, stderr);
                    }
                    proc.Dispose();

                    if (!stdoutReadingTask.Wait(TimeSpan.FromSeconds(30)))
                    {
                        throw new Exception("Not done reading process stdout to end within 30 seconds after process exit");
                    }
                }

                createdCategory = CreateChartsFolderStructure(jobStatus, categoryId, chartTitle, userId, projectRootPath);
                _categoryDomain.Add(createdCategory);

                //Update charts thumbnails
                BackgroundJob.Enqueue<IChartDataDomain>(cd => cd.UpdateThumbnail(createdCategory.ChartObjects.Select(c => c.Id), projectRootPath));

                if (jobStatus.ChartsConfig.RequiresProcess)
                {
                    jobStatus.JobOutput = output.ToString();
                    jobStatus.EndTime = DateTimeOffset.Now;
                    jobStatus.Status = JobProcessedStatus.Processed;
                    _chartHubManager.ShowSuccessProcessAlert(userName, jobStatus);

                    if (_chartHubManager.UserIsConnected(userName))
                    {
                        jobStatus.UserNotified = true;
                    }
                }
                else
                {
                    jobStatus.Status = JobProcessedStatus.NoProcessNeeded;
                    jobStatus.UserNotified = true;
                }
                
                _chartHubManager.ReloadCategory(userName, categoryId);
            }
            catch (Win32Exception ex)
            {
                var msg = string.Format("There was an error in opening the associated file [{0}] with args:[{1} {2} {3}]",
                        jobStatus.ChartsConfig.Processor.Path,
                        jobStatus.UnprocessedDataFilePath,
                        jobStatus.ChartDataDirectory,
                        jobStatus.ChartsConfig.Processor.ExtraParameters);

                jobStatus.JobOutput = msg;
                jobStatus.Status = JobProcessedStatus.ProcessedWithError;

                _logger.Error(ex, msg);

                _chartHubManager.UpdateJobOutput(userName, jobStatus.Id, msg);

                _chartHubManager.ShowErrorProcessAlert(userName, jobStatus);

                if (_chartHubManager.UserIsConnected(userName))
                {
                    jobStatus.UserNotified = true;
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                jobStatus.Status = JobProcessedStatus.ProcessedWithError;

                //remove created chart on error
                if (createdCategory.ChartObjects.Count > 0)
                {
                    foreach (var chart in createdCategory.ChartObjects)
                    {
                        if (_chartObjectDomain.GetById(chart.Id) != null)
                            _chartObjectDomain.Delete(chart.Id);
                    }
                }

                _chartHubManager.ShowErrorProcessAlert(userName, jobStatus);

                if (_chartHubManager.UserIsConnected(userName))
                {
                    jobStatus.UserNotified = true;
                }

                throw;
            }
            finally
            {
                if (jobStatus.ChartsConfig.RequiresProcess)
                {
                    _chartHubManager.SetProcessStatus(userName, jobStatus.Status == JobProcessedStatus.Processed);
                    jobStatus.JobFinishTime = DateTimeOffset.UtcNow;
                    _chartHubManager.UpdateJobsBadge(userName, userId);
                }
                
                _jobDomain.Update(jobStatus);
                
            }
        }

        private Process CreateChartDataProcess(string unprocessedDataFilePath, string chartDataDirectory, ChartsConfiguration config)
        {
            var arguments = $"{unprocessedDataFilePath} {chartDataDirectory} {config.Processor.ExtraParameters}";
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = config.Processor.Path,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(config.Processor.Path)
                }
            };

            return proc;
        }

        private Category CreateChartsFolderStructure(JobStatus jobStatus, int categoryId, string chartTitle, string userId, string projectRootPath)
        {
            var chartsConfig = jobStatus.ChartsConfig.GetConfigurationSchema();
            var chartObjects = new List<ChartObject>();
            foreach (var chart in chartsConfig.Charts)
            {
                var title = chart.ChartName;
                string timeSeriesFilePath;
                if (jobStatus.ChartsConfig.RequiresProcess)
                {
                    timeSeriesFilePath = Path.Combine(jobStatus.ChartDataDirectory, chartsConfig.TimeSeries.FileName);
                }
                else
                {
                    timeSeriesFilePath = Path.Combine(jobStatus.ChartDataDirectory, jobStatus.SystemFileName);
                }
                var timeSeriesColumn = chartsConfig.TimeSeries.ColumnName;
                List<SerieConfiguration> series;
                if (jobStatus.ChartsConfig.RequiresProcess)
                {
                    series = chart.SeriesList.Select(s =>
                        new SerieConfiguration(
                            s.SeriesName,
                            Path.Combine(jobStatus.ChartDataDirectory, s.FileName),
                            s.ColumnName)).ToList();
                }
                else
                {
                    series = chart.SeriesList.Select(s =>
                        new SerieConfiguration(
                            s.SeriesName,
                            Path.Combine(jobStatus.ChartDataDirectory, jobStatus.SystemFileName),
                            s.ColumnName)).ToList();
                }

                var chartObject = new ChartObject(title)
                {
                    OwnerId = userId,
                    ChartDataSource = new ChartDataSource(timeSeriesFilePath, timeSeriesColumn, series),
                    ChartType = chart.ChartType.Value
                };

                //Set default thumbnail
                chartObject.Thumbnail = new Thumbnail();
                chartObject.Thumbnail.SetThumbnailImage(Path.Combine(projectRootPath, "Content", "icons", "line-chart-96.png"));

                chartObjects.Add(chartObject);
            }

            var parentCategory = _categoryDomain.GetById(categoryId);
            if (parentCategory == null)
            {
                parentCategory = _categoryDomain.GetRootCategoryByOwnerId(userId);
            }
            var category = new Category(chartTitle)
            {
                ParentCategoryId = parentCategory.Id,
                ChartObjects = chartObjects,
                IsEveryones = parentCategory.IsEveryones,
                OwnerId = parentCategory.OwnerId
            };

            return category;

        }

        public void UpdateThumbnail(IEnumerable<int> charts, string projectRootPath)
        {
            foreach (var chartId in charts)
            {
                var chart = _chartObjectDomain.GetById(chartId);
                if (chart == null) continue;
                var tempPath = Path.Combine(projectRootPath, "TempChartThumbnails");
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                var thumbName = CreateTempChartImageName(chart.Id);
                var thumbImgPath = Path.Combine(tempPath, thumbName + ".png");
                var thumbConfigPath = Path.Combine(tempPath, thumbName + ".json");

                try
                {
                    if (chart.ChartDataSource == null)
                        continue;

                    var chartOption = HighchartsFactory.GetHighchartOptions(chart.ChartDataSource);
                    CreateThumbnailConfig(chartOption, thumbConfigPath);

                    using (var process = RunCreateThumbnailProcess(thumbConfigPath, thumbImgPath))
                    {
                        process.WaitForExit(300000);
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                        if (process.ExitCode == 0 && File.Exists(thumbImgPath))
                        {
                            chart.Thumbnail = new Thumbnail();
                            chart.Thumbnail.SetThumbnailImage(thumbImgPath);
                            _chartObjectDomain.Update(chart);
                        }
                        else
                        {
                            _logger.Error(process.StandardError.ReadToEnd());
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                finally
                {
                    if (File.Exists(thumbImgPath))
                    {
                        File.Delete(thumbImgPath);
                    }

                    if (File.Exists(thumbConfigPath))
                    {
                        File.Delete(thumbConfigPath);
                    }
                }
            }
        }

        private void CreateThumbnailConfig(Highsoft.Web.Mvc.Charts.Highcharts config, string fileName)
        {
            using (var stream = File.CreateText(fileName))
            {
                var chartConfig = new Highsoft.Web.Mvc.Charts.HighsoftNamespace().GetJsonOptions(config).ToString();
                stream.Write(chartConfig);
                stream.Flush();
            }
        }

        private Process RunCreateThumbnailProcess(string inputFileName, string outputFileName)
        {
            // Need to start with "/C". Carries out the command specified by the string and then terminates
            var command = $"/C highcharts-export-server -infile {inputFileName} -outfile {outputFileName} --type png -width 1080";

            Process highchartsServerPr = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = "cmd.exe",
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            highchartsServerPr.Start();

            return highchartsServerPr;
        }

        private string CreateTempChartImageName(int chartId)
        {
            return $"{Guid.NewGuid()}_{chartId}";
        }
    }
}
