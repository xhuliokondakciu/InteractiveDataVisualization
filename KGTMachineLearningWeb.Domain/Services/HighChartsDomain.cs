using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Domain.Contracts;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Domain.Services
{
    internal class HighChartsDomain : IHighChartsDomain
    {
        public Highcharts GetPmPredictionsSeriesDefaultOptions(string chartTitle)
        {
            return new Highcharts
            {
                Title = new Title { Text = chartTitle },
                Chart = new Chart
                {
                    Type = ChartType.Line,
                    Panning = true
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
                        Type = XAxisType.Datetime,
                        Offset = 0
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
                    }
                }
            };
        }

        public Highcharts GetNeuralNetworksDefaultOptions()
        {
            return new Highcharts
            {
                Chart = new Chart
                {
                    Type = ChartType.Heatmap,
                    ZoomType = ChartZoomType.Xy
                },
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        Type = XAxisType.Datetime,
                        Labels = new XAxisLabels
                        {
                            Y = 15
                        },
                        ShowLastLabel = false,
                        Offset = 0
                    }
                },
                YAxis = new List<YAxis>
                {
                    new YAxis
                    {
                        Title = new YAxisTitle
                        {
                            Text = null
                        }
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
                    StartOnTick = false,
                    EndOnTick = false
                },
                Boost = new Boost
                {
                    UseGPUTranslations = true

                },
                Series = new List<Series>
                {
                    new HeatmapSeries
                    {
                        BoostThreshold = 100,
                        BorderWidth = 0,
                        NullColor = "#EFEFEF",
                        TurboThreshold = double.MaxValue
                    }
                }
            };
        }

        public Highcharts GetNeuralNetworksDefaultOptions(string chartTitle, double yAxisMin, double yAxisMax, double valueMin, double valueMax, double colSize)
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
                    ZoomType = ChartZoomType.Xy
                },
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        Type = XAxisType.Datetime,
                        ShowLastLabel = false,
                        Offset = 0
                    }
                },
                YAxis = new List<YAxis>
                {
                    new YAxis
                    {
                        Min = yAxisMin,
                        Max = yAxisMax
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
                    UseGPUTranslations = true

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
    }
}
