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
        private static Highcharts CreateLineChartDefaultOptions(string chartTitle, XAxisType xAxisType, YAxisType yAxisType)
        {
            return new Highcharts
            {
                Title = new Title { Text = chartTitle },
                Chart = new Chart
                {
                    Type = ChartType.Line,
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
                       TurboThreshold = double.MaxValue
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

        private static Highcharts CreateAreaChartDefaultOptions(string chartTitle, XAxisType xAxisType, YAxisType yAxisType)
        {
            return new Highcharts
            {
                Chart = new Chart
                {
                    Panning = true,
                    BackgroundColor = BACKGROUND_CHART_COLOR
                },
                Title = new Title
                {
                    Text = chartTitle
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
                        }
                    },
                    Series = new PlotOptionsSeries
                    {
                        TurboThreshold = 10000
                    }
                }
            };
        }

        private static Highcharts CreateHeatMapDefaultChartOptions(
            string chartTitle,
            XAxisType xAxisType,
            YAxisType yAxisType,
            double yAxisMin,
            double yAxisMax,
            double valueMin,
            double valueMax,
            double colSize)
        {
            return new Highcharts
            {
                Title = new Title
                {
                    Text = chartTitle
                },
                Chart = new Chart
                {
                    Type = ChartType.Heatmap,
                    ZoomType = ChartZoomType.Xy,
                    BackgroundColor = BACKGROUND_CHART_COLOR
                },
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        Type = xAxisType,
                        ShowLastLabel = false,
                        Offset = 0
                    }
                },
                YAxis = new List<YAxis>
                {
                    new YAxis
                    {
                        Min = yAxisMin,
                        Max = yAxisMax,
                        Type = yAxisType
                    }
                },
                ColorAxis = new ColorAxis
                {
                    Stops = new List<Stop>
                    {
                        new Stop
                        {
                            Position = 0,
                            Color = "#3060cf"
                        },
                        new Stop
                        {
                            Position = 0.5,
                            Color = "#fffbbc"
                        },
                        new Stop
                        {
                            Position = 0.9,
                            Color = "#c4463a"
                        },
                        new Stop
                        {
                            Position = 1,
                            Color = "#c4463a"
                        }
                    },
                    Min = valueMin,
                    Max = valueMax,
                    StartOnTick = false,
                    EndOnTick = false
                },
                Boost = new Boost
                {
                    UseGPUTranslations = true,
                    SeriesThreshold = 10000

                },
                Series = new List<Series>
                {
                    new HeatmapSeries
                    {
                        BoostThreshold = 100,
                        BorderWidth = 0,
                        NullColor = "#EFEFEF",
                        TurboThreshold = double.MaxValue,
                        Colsize = colSize
                    }
                }
            };
        }

        private static Highcharts CreateLineChartOptions(ChartData chartData, string chartTitle)
        {
            var lineSeries = chartData.Series.Select(s =>
            {
                return new LineSeries
                {
                    Name = s.Name,
                    Type = LineSeriesType.Line,
                    Data = s.Points.Select((p, index) =>
                    {
                        return new LineSeriesData
                        {
                            X = p.X?.ToUnixTimeMilliseconds(),
                            Y = p.Y
                        };
                    }).ToList(),
                    BoostThreshold = 10000,
                    TurboThreshold = double.MaxValue
                };
            });

            var chartOptions = CreateLineChartDefaultOptions(chartTitle, XAxisType.Datetime, YAxisType.Linear);
            chartOptions.Series = lineSeries.Cast<Series>().ToList();

            return chartOptions;
        }

        private static Highcharts CreateAreaChartOptions(ChartData chartData, string chartTitle)
        {
            var areaSeries = chartData.Series.Select(s =>
            {
                return new AreaSeries
                {
                    Name = s.Name,
                    Data = s.Points.Select((p, index) =>
                    {
                        return new AreaSeriesData
                        {
                            X = p.X?.ToUnixTimeMilliseconds(),
                            Y = p.Y
                        };
                    }).ToList()
                };
            });
            var chartOptions = CreateAreaChartDefaultOptions(chartTitle, XAxisType.Datetime, YAxisType.Linear);
            chartOptions.Series = areaSeries.Cast<Series>().ToList();

            return chartOptions;
        }

        public static Highcharts GetHighchartOptions(ChartDataSource dataSource)
        {
            //return CreateLineChartOptions(ChartDataFactory.CreateChartData(dataSource), dataSource?.ChartObject?.Title ?? "");

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

            var chartOptions = CreateLineChartDefaultOptions(dataSource?.ChartObject?.Title ?? "", XAxisType.Datetime, YAxisType.Linear);
            chartOptions.Data = new Data
            {
                Columns = columns,
                FirstRowAsNames = true
            };

            return chartOptions;
        }

        public static Highcharts GetHighchartOptionsForThumbnail(ChartDataSource dataSource)
        {
            return CreateLineChartOptions(ChartDataFactory.CreateChartData(dataSource), dataSource?.ChartObject?.Title ?? "");
        }
    }
}
