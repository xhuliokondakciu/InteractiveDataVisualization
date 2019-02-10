using KGTMachineLearningWeb.Common.FileHelper;
using KGTMachineLearningWeb.Models.Chart;
using KGTMachineLearningWeb.Models.Workspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static KGTMachineLearningWeb.Common.Enum.Enumerators;

namespace KGTMachineLearningWeb.Domain.Factories
{
    public static class ChartDataFactory
    {

        public static char Separator = ',';
        
        /// <summary>
        /// Get chart data form file
        /// </summary>
        /// <param name="chartDataSource">Chart data files configuration</param>
        /// <returns>Chart data object with values</returns>
        public static ChartData CreateChartData(ChartDataSource chartDataSource)
        {
            var xCsvColumn = new CsvFileHelper(chartDataSource.TimeSerieFilePath).ParseSingle<DateTimeOffset>(chartDataSource.TimeSerieColumn);

            var series = chartDataSource.Series.Select(serie =>
            {
                List<int> unparsedIndexes = new List<int>();
                var yValues = new CsvFileHelper(serie.FilePath).ParseSingle<double?>(serie.ColumnName, unparsedIndexes);
                
                ChartPoint[] chartPoints = new ChartPoint[yValues.Values.Length];
                
                for(int i = 0; i < yValues.Values.Length; i++)
                {
                    chartPoints[i] = new ChartPoint(xCsvColumn.Values[i], yValues.Values[i]);
                }

                return new ChartSerie(serie.Name, chartPoints);
            });

            return new ChartData(chartDataSource?.ChartObject?.Title ?? "", series);
        }
    }
}
