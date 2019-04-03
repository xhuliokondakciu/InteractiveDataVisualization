using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace KGTMachineLearningWeb.Common.FileHelper
{
    public class CsvFileHelper
    {
        private readonly char _separator = ',';
        private readonly string _filePath;
        private static ObjectCache _cache = MemoryCache.Default;
        private string[][] _lines;
        private string[] _columnNames;

        public CsvFileHelper(string filePath)
        {
            _filePath = filePath;
            ReadFile();
        }

        public CsvFileHelper(string filePath, char separator)
        {
            _separator = separator;
            _filePath = filePath;
            ReadFile();
        }

        private void ReadFile()
        {
            if (_cache.Contains(_filePath))
            {
                _lines = _cache.GetCacheItem(_filePath).Value as string[][];
            }
            else
            {
                _lines = File.ReadLines(_filePath)
               .Where(l => !string.IsNullOrWhiteSpace(l))
               .Select(l =>
               l.Split(_separator).Select(v => v.Trim()).ToArray())
               .ToArray();

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new System.TimeSpan(0, 30, 00);
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { _filePath }));
                _cache.Add(_filePath, _lines, policy);
            }

            _columnNames = _lines[0];
        }

        /// <summary>
        /// Parse a single column from the csv file
        /// </summary>
        /// <param name="columnIndex">Index of column to parse</param>
        /// <param name="separator">Csv file separator</param>
        /// <returns></returns>
        public CsvColumn<T> ParseSingle<T>(int columnIndex)
        {
            return Parse<T>(new int[] { columnIndex }, new List<int>()).FirstOrDefault();
        }

        /// <summary>
        /// Parse a single column from the csv file
        /// </summary>
        /// <param name="columnName">Index of column to parse</param>
        /// <param name="separator">Csv file separator</param>
        /// <returns></returns>
        public CsvColumn<T> ParseSingle<T>(string columnName)
        {
            return Parse<T>(new string[] { columnName }).FirstOrDefault();
        }

        /// <summary>
        /// Parse a single column from the csv file
        /// </summary>
        /// <param name="columnIndex">Index of column to parse</param>
        /// <param name="notParsedIndexes">Indexes that couldn't be parsed</param>
        /// <returns></returns>
        public CsvColumn<T> ParseSingle<T>(string columnIndex, IList<int> notParsedIndexes)
        {
            return Parse<T>(new string[] { columnIndex }, notParsedIndexes).FirstOrDefault();
        }

        /// <summary>
        /// Parse multiple columns from csv file
        /// </summary>
        /// <param name="filePath">Path of csv file</param>
        /// <param name="separator">Csv file separator</param>
        /// <param name="columnIndexes">Column indexes to parse</param>
        /// <returns></returns>
        public IEnumerable<CsvColumn<T>> Parse<T>(string[] columnNames)
        {
            var columnIndexes = columnNames.Select(c =>
            {
                if (!_columnNames.Contains(c))
                    throw new System.Exception($"Column with name {c} was not found in source file");

                return _columnNames.ToList().IndexOf(c);
            }).ToArray();

            return Parse<T>(columnIndexes, new List<int>());
        }

        /// <summary>
        /// Parse multiple columns from csv file
        /// </summary>
        /// <param name="columnIndexes">Column indexes to parse</param>
        /// <param name="notParsedIndexes">Indexes that couldn't be parsed</param>
        /// <returns></returns>
        public IEnumerable<CsvColumn<T>> Parse<T>(string[] columnNames, IList<int> notParsedIndexes)
        {
            var columnIndexes = columnNames.Select(c =>
            {
                if (!_columnNames.Contains(c))
                    throw new System.Exception($"Column with name {c} was not found in source file");

                return _columnNames.ToList().IndexOf(c);
            }).ToArray();

            return Parse<T>(columnIndexes, notParsedIndexes);
        }

        public IEnumerable<CsvColumn<T>> Parse<T>()
        {
            var indexToParse = File.ReadLines(_filePath)
                .First()
                .Split(',')
                .Select((v, index) => index)
                .ToArray();

            return Parse<T>(indexToParse, new List<int>());
        }

        /// <summary>
        /// Parse multiple columns from csv file
        /// </summary>
        /// <param name="columnIndexes">Column indexes to parse</param>
        /// <param name="notParsedIndexes">Indexes that couldn't be parsed</param>
        /// <returns></returns>
        public IEnumerable<CsvColumn<T>> Parse<T>(int[] columnIndexes, IList<int> notParsedIndexes)
        {
            List<CsvColumn<T>> columns = new List<CsvColumn<T>>();
            for (int i = 0; i < _columnNames.Count() && i < columnIndexes.Count(); i++)
            {
                var column = new CsvColumn<T>(_columnNames[columnIndexes[i]], _lines.GetLength(0) - 1);
                for (int j = 1; j < _lines.GetLength(0); j++)
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter.CanConvertFrom(typeof(string)) && converter.IsValid(_lines[j][columnIndexes[i]]))
                    {
                        var value = (T)converter.ConvertFrom(_lines[j][columnIndexes[i]]);
                        column.Values[j - 1] = value;
                    }
                    else
                    {
                        column.Values[j - 1] = default(T);
                        notParsedIndexes.Add(j - 1);
                    }
                }

                columns.Add(column);
            }

            return columns;
        }

    }

    public class CsvColumn<T>
    {
        public CsvColumn(string columnName, int valuesCount)
        {
            ColumnName = columnName;
            Values = new T[valuesCount];
        }
        public string ColumnName { get; set; }
        public T[] Values { get; set; }
    }
}
