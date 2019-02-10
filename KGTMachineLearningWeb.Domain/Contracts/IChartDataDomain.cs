using Hangfire;
using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.Jobs;
using KGTMachineLearningWeb.Models.Workspace;
using System;
using System.Collections.Generic;

namespace KGTMachineLearningWeb.Domain.Contracts
{
    public interface IChartDataDomain : IBaseDomain<int, ChartDataSource>
    {
        /// <summary>
        /// Start processing the xml file with data
        /// </summary>
        /// <param name="dataToProcess">Object containing the file content and file name</param>
        void ProcessFile(ChartDataToProcess dataToProcess,int categoryId,string chartTitle, int configId);

        [AutomaticRetry(Attempts = 0)]
        void ProcessData(int jobStatusId, string sourcePath, int categoryId, string chartTitle, string userName, string userId, IJobCancellationToken cancellationToken);

        /// <summary>
        /// Updates chart object thumbnail based on its configuration
        /// </summary>
        /// <param name="charts">Charts for which to create the thumbnails</param>
        /// <param name="projectRootPath">Root path of project folder</param>
        void UpdateThumbnail(IEnumerable<int> charts, string projectRootPath);

    }
}
