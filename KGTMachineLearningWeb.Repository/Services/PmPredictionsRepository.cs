using KGTMachineLearningWeb.Models;
using KGTMachineLearningWeb.Repository.Comparer;
using KGTMachineLearningWeb.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Repository.Services
{
    internal class PmPredictionsRepository : IPmPredictionsRepository
    {
        private string FilePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["PmPredictionsXmlPath"]);
            }
        }

        private const string PM_PREDICTIONS_KEY = "PmPredictionsData";

        public IEnumerable<string> GetTestData()
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["TestDB"];

            using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("[dbo].[GetTestData_SP]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Data")))
                                yield return reader.GetString(reader.GetOrdinal("Data"));
                        }
                    }
                }
            }
        }

        private PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTS ReadXmlFile()
        {

            if (!HttpContext.Current.Application.AllKeys.Contains("PmPredictionsData") || HttpContext.Current.Application["PmPredictionsData"] == null)
            {
                XmlSerializer ser = new XmlSerializer(typeof(PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTS));

                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var nnModel = ser.Deserialize(stream) as PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTS;
                    nnModel.Prediction.Sort(new PmPredictionsDateComparer());
                    HttpContext.Current.Application["PmPredictionsData"] = nnModel;
                }
            }

            return (PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTS)HttpContext.Current.Application["PmPredictionsData"];
        }

        public IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions()
        {
            var xmlData = ReadXmlFile();

            return xmlData?.Prediction;
        }

        public IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return GetPmPredictions()?.Where(prediction => prediction.DateTimeUtcDate.CompareTo(startDate) >= 0
            && prediction.DateTimeUtcDate.CompareTo(endDate) <= 0);
        }
    }
}