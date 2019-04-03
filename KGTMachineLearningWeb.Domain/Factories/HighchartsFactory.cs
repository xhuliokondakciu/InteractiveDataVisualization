using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Common.FileHelper;
using KGTMachineLearningWeb.Models.Chart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGTMachineLearningWeb.Domain.Factories
{
    public class HighchartsFactory
    {
        public const int DATE_COLUMN_INDEX = 2;
        public const string BACKGROUND_CHART_COLOR = "#f8f9fa";
        private static Highcharts CreateChartDefaultOptions(string chartTitle, XAxisType xAxisType, YAxisType yAxisType, ChartType chartType)
        {
            return new Highcharts
            {
                Title = new Title { Text = chartTitle },
                Chart = new Chart
                {
                    Type = chartType,
                    Panning = true,
                    BackgroundColor = BACKGROUND_CHART_COLOR
                },
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        Crosshair = new XAxisCrosshair
                        {
                            DashStyle = XAxisCrosshairDashStyle.Dash,
                            Width = 2
                        },
                        Type = xAxisType,
                        Offset = 0
                    }
                },
                YAxis = new List<YAxis>
                {
                    new YAxis
                    {
                        Type = yAxisType
                    }
                },
                PlotOptions = new PlotOptions
                {
                    Spline = new PlotOptionsSpline
                    {
                        Marker = new PlotOptionsSplineMarker
                        {
                            Enabled = false
                        },
                        FindNearestPointBy = PlotOptionsSplineFindNearestPointBy.X
                    },
                    Series = new PlotOptionsSeries
                    {
                       TurboThreshold = double.MaxValue,
                       ConnectNulls = false
                    }
                },
                Boost = new Boost
                {
                    UseGPUTranslations = true,
                    Enabled = true,
                    SeriesThreshold = 10000
                }
            };
        }

        private static ChartType GetHighchartChartType(ChartsConfigSchemaChartChartTypeValue chartType)
        {
            switch (chartType)
            {
                case ChartsConfigSchemaChartChartTypeValue.Column:
                    return ChartType.Column;
                case ChartsConfigSchemaChartChartTypeValue.Area:
                    return ChartType.Area;
                case ChartsConfigSchemaChartChartTypeValue.Line:
                    return ChartType.Line;
                default:
                    return ChartType.Line;
            }
        }

        private static Highcharts CreateLineChartOptions(IEnumerable<ChartSerie> series, string chartTitle,ChartsConfigSchemaChartChartTypeValue chartType)
        {
            var lineSeries = series.Select(s =>
            {
                switch (chartType)
                {
                    case ChartsConfigSchemaChartChartTypeValue.Line:
                        return (Series)new LineSeries
                        {
                            Name = s.Name,
                            Data = s.Points.Select((p, index) =>
                            {
                                return new LineSeriesData
                                {
                                    X = p.X?.ToUnixTimeMilliseconds(),
                                    Y = p.Y
                                };
                            }).ToList(),
                            BoostThreshold = 1000,
                            TurboThreshold = double.MaxValue
                        };
                    case ChartsConfigSchemaChartChartTypeValue.Area:
                        return (Series)new AreaSeries
                        {
                            Name = s.Name,
                            Data = s.Points.Select((p, index) =>
                            {
                                return new AreaSeriesData
                                {
                                    X = p.X?.ToUnixTimeMilliseconds(),
                                    Y = p.Y
                                };
                            }).ToList(),
                            BoostThreshold = 1000,
                            TurboThreshold = double.MaxValue
                        };
                    case ChartsConfigSchemaChartChartTypeValue.Column:
                        return (Series)new ColumnSeries
                        {
                            Name = s.Name,
                            Data = s.Points.Select((p, index) =>
                            {
                                return new ColumnSeriesData
                                {
                                    X = p.X?.ToUnixTimeMilliseconds(),
                                    Y = p.Y
                                };
                            }).ToList(),
                            BoostThreshold = 1000,
                            TurboThreshold = double.MaxValue
                        };
                    default:
                        goto case ChartsConfigSchemaChartChartTypeValue.Line;
                }
                
            });

            var chartOptions = CreateChartDefaultOptions(chartTitle, XAxisType.Datetime, YAxisType.Linear,GetHighchartChartType(chartType));
            chartOptions.Series = lineSeries.Cast<Series>().ToList();

            return chartOptions;
        }
        
        public static Highcharts GetHighchartOptions(ChartDataSource dataSource)
        {
            var timeColumn = new CsvFileHelper(dataSource.TimeSerieFilePath).ParseSingle<string>(dataSource.TimeSerieColumn);

            var xColumn = new List<object> { timeColumn.ColumnName };
            xColumn.AddRange(timeColumn.Values);

            var dataColumns = dataSource.Series.Select(s =>
            {
                var data = new CsvFileHelper(s.FilePath).ParseSingle<string>(s.ColumnName);
                var column = new List<object> { s.Name };
                column.AddRange(data.Values);
                return column;
            });

            var columns = new List<List<object>> { xColumn };
            columns.AddRange(dataColumns);

            var chartOptions = CreateChartDefaultOptions(dataSource?.ChartObject?.Title ?? "", XAxisType.Datetime, YAxisType.Linear,GetHighchartChartType(dataSource.ChartObject.ChartType));
            chartOptions.Data = new Data
            {
                Columns = columns,
                FirstRowAsNames = true
            };

            return chartOptions;
        }

        public static Highcharts GetHighchartOptionsForThumbnail(ChartDataSource dataSource)
        {
            var xCsvColumn = new CsvFileHelper(dataSource.TimeSerieFilePath).ParseSingle<DateTimeOffset>(dataSource.TimeSerieColumn);

            var series = dataSource.Series.Select(serie =>
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

            return CreateLineChartOptions(series, dataSource?.ChartObject?.Title ?? "",dataSource.ChartObject.ChartType);
        }
    }
}
