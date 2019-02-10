using KGTMachineLearningWeb.Common.Helper;
using KGTMachineLearningWeb.Models.Workspace;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Models.Chart
{
    public class ChartDataSource
    {
        protected ChartDataSource() { }

        public ChartDataSource(string timeSerieFilePath, string timeSerieColumn, IEnumerable<SerieConfiguration> series)
        {
            TimeSerieFilePath = timeSerieFilePath;
            TimeSerieColumn = timeSerieColumn;
            Series = series.ToList();
        }

        [Key, ForeignKey("ChartObject")]
        public int Id { get; set; }

        public string TimeSerieFilePath { get; set; }

        public string TimeSerieColumn { get; set; }
        
        public virtual ICollection<SerieConfiguration> Series { get; set; }

        public virtual ChartObject ChartObject { get; set; }

        public ChartDataSource Copy()
        {
            var newFilePath = FileSystemHelper.CreateChartDataDirectory(ChartObject.OwnerId);
            var timeSerieNewPath = CopyTimeSerie(newFilePath);
            var series = new List<SerieConfiguration>();
            foreach(var serie in Series)
            {
                series.Add(serie.Copy(newFilePath));
            }

            return new ChartDataSource
            {
                TimeSerieFilePath = timeSerieNewPath,
                TimeSerieColumn = TimeSerieColumn,
                Series = series
            };
        }

        private string CopyTimeSerie(string filePath)
        {
            var timeSeriesNewPath = Path.Combine(filePath, Path.GetFileName(TimeSerieFilePath));
            try
            {

                if (!File.Exists(timeSeriesNewPath))
                    File.Copy(TimeSerieFilePath, timeSeriesNewPath);
            }
            catch (Exception)
            {
                if (File.Exists(timeSeriesNewPath))
                    File.Delete(timeSeriesNewPath);
            }

            return timeSeriesNewPath;
        }

    }

    public class SerieConfiguration
    {
        protected SerieConfiguration() { }
        public SerieConfiguration(string name, string filePath, string columnName)
        {
            Name = name;
            FilePath = filePath;
            ColumnName = columnName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string ColumnName { get; set; }
        public virtual ChartDataSource ChartDataSource { get; set; }

        public SerieConfiguration Copy(string path)
        {
            var fileNewPath = CopyFile(path);
            return new SerieConfiguration(Name, fileNewPath, ColumnName);
        }

        private string CopyFile(string path)
        {
            var fileNewPath = Path.Combine(path, Path.GetFileName(FilePath));

            try
            {
                if (!File.Exists(fileNewPath))
                {
                    File.Copy(FilePath, fileNewPath);
                }
            }
            catch (Exception e)
            {
                if (File.Exists(fileNewPath))
                    File.Delete(fileNewPath);
                throw new Exception("Couldn't copy files",e);
            }

            return fileNewPath;
        }
    }
}
