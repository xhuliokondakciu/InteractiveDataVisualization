using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KGTMachineLearningWeb.Common.Helper
{
    public static class CommonConstants
    {
        public const int SHARED_ROOT_FOLDER_ID = -1;
        public const int EVERYONE_CATEGORY_ID = 1;
        public const string SUPER_USER_ID = "00000000-0000-0000-0000-000000000000";
        public const string TEST_USER_ID = "c5ba2679-cab8-448d-92e4-f56f4601dc33";
        public const string USERS_ROOT_CATEGORY_NAME = "My categories";
    }

    public static class UserAlertClasses
    {
        public const string PRIMARY = "alert-primary";
        public const string SUCCESS = "alert-success";
        public const string ERROR = "alert-danger";
        public const string WARNING = "alert-warning";
        public const string INFO = "alert-info";
    }

    public static class ProcessedFilesInfo
    {
        public const string PM_OUTPUTS = "pmOutputs";
        public const string RB_STREAM_WITH_DESIRED_PREDICTIONS = "rbStreamWithDesiredPredictions";
        public static readonly int[] PM_OUTPUT_DATA_INDEX = new[] { 0, 1, 2 };
        public static readonly int[] PM_OUTPUT_STRING_REPRESENTATION_INDEX = new[] { 3 };
        public static readonly int[] RB_STREAM_DATA_INDEX = new[] { 1, 50, 51 };
    }


    public static class FileSystemHelper
    {
        /// <summary>
        /// Creates chart data directory if it doesn't exist.
        /// </summary>
        /// <param name="userId">Current user id</param>
        /// <returns>Created chart data directory path</returns>
        public static string CreateChartDataDirectory(string userId)
        {
            string chartDataPath = string.Empty;
            if (Path.IsPathRooted(System.Configuration.ConfigurationManager.AppSettings["ChartDataPath"]))
            {
                chartDataPath = System.Configuration.ConfigurationManager.AppSettings["ChartDataPath"];
            }
            else
            {
                chartDataPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ChartDataPath"]);
            }
            var rootPath = chartDataPath;
            string dir = Path.Combine(rootPath, $"User_{userId}", Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
