using DataVisualization.Common.Helper;
using DataVisualization.Models.Workspace;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using static DataVisualization.Common.Enum.Enumerators;

namespace DataVisualization.Models.Chart
{
    public class ChartDataSource : IEquatable<ChartDataSource>
    {
        protected ChartDataSource() { }

        public ChartDataSource(string timeSerieFilePath, string timeSerieColumn, IEnumerable<SeriesConfiguration> series)
        {
            TimeSerieFilePath = timeSerieFilePath;
            TimeSerieColumn = timeSerieColumn;
            Series = series.ToList();
        }

        [Key, ForeignKey("ChartObject")]
        public int Id { get; set; }

        public string TimeSerieFilePath { get; set; }

        public string TimeSerieColumn { get; set; }
        
        public virtual ICollection<SeriesConfiguration> Series { get; set; }

        public virtual ChartObject ChartObject { get; set; }

        public ChartDataSource Copy()
        {
            var newFilePath = FileSystemHelper.CreateChartDataDirectory(ChartObject.OwnerId);
            var timeSerieNewPath = CopyTimeSerie(newFilePath);
            var series = new List<SeriesConfiguration>();
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

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartDataSource);
        }

        public bool Equals(ChartDataSource other)
        {
            return other != null &&
                   Id == other.Id &&
                   TimeSerieFilePath == other.TimeSerieFilePath &&
                   TimeSerieColumn == other.TimeSerieColumn &&
                   EqualityComparer<ICollection<SeriesConfiguration>>.Default.Equals(Series, other.Series) &&
                   EqualityComparer<ChartObject>.Default.Equals(ChartObject, other.ChartObject);
        }

        public override int GetHashCode()
        {
            var hashCode = 2022755631;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TimeSerieFilePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TimeSerieColumn);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICollection<SeriesConfiguration>>.Default.GetHashCode(Series);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChartObject>.Default.GetHashCode(ChartObject);
            return hashCode;
        }

        public static bool operator ==(ChartDataSource source1, ChartDataSource source2)
        {
            return EqualityComparer<ChartDataSource>.Default.Equals(source1, source2);
        }

        public static bool operator !=(ChartDataSource source1, ChartDataSource source2)
        {
            return !(source1 == source2);
        }
    }

    public class SeriesConfiguration : IEquatable<SeriesConfiguration>
    {
        protected SeriesConfiguration() { }
        public SeriesConfiguration(string name, string filePath, string columnName)
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

        public SeriesConfiguration Copy(string path)
        {
            var fileNewPath = CopyFile(path);
            return new SeriesConfiguration(Name, fileNewPath, ColumnName);
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

        public override bool Equals(object obj)
        {
            return Equals(obj as SeriesConfiguration);
        }

        public bool Equals(SeriesConfiguration other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   FilePath == other.FilePath &&
                   ColumnName == other.ColumnName &&
                   EqualityComparer<ChartDataSource>.Default.Equals(ChartDataSource, other.ChartDataSource);
        }

        public override int GetHashCode()
        {
            var hashCode = -148795900;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ColumnName);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChartDataSource>.Default.GetHashCode(ChartDataSource);
            return hashCode;
        }

        public static bool operator ==(SeriesConfiguration configuration1, SeriesConfiguration configuration2)
        {
            return EqualityComparer<SeriesConfiguration>.Default.Equals(configuration1, configuration2);
        }

        public static bool operator !=(SeriesConfiguration configuration1, SeriesConfiguration configuration2)
        {
            return !(configuration1 == configuration2);
        }
    }
}
