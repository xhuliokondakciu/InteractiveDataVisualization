using Highsoft.Web.Mvc.Charts;
using KGTMachineLearningWeb.Domain.Contracts;
using KGTMachineLearningWeb.Models;
using KGTMachineLearningWeb.Repository.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KGTMachineLearningWeb.Config")]
namespace KGTMachineLearningWeb.Domain.Services
{
    internal class PmPredictionsDomain : IPmPredictionsDomain
    {
        private readonly IPmPredictionsRepository pmPredictionsRepository;

        private readonly IHighChartsDomain chartsDomain;

        public PmPredictionsDomain(
            IPmPredictionsRepository pmPredictionsRepository,
            IHighChartsDomain chartsDomain)
        {
            this.pmPredictionsRepository = pmPredictionsRepository;
            this.chartsDomain = chartsDomain;
        }

        public IEnumerable<PM_PREDICTIONS_NONNUMERICLIST_WITH_BUSINESS_RESULTSPrediction> GetPmPredictions()
        {
            return pmPredictionsRepository.GetPmPredictions();
        }

        public Highcharts GetInputs()
        {
            var predictions = GetPmPredictions();

            var bidData = predictions.Select(prediction =>
            {
                return new LineSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.Bid
                };
            });

            var askData = predictions.Select(prediction =>
            {
                return new LineSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.Ask
                };
            });

            var series = new List<Series>{
                new LineSeries
                    {
                        Name = "Bid",
                        Data = bidData.ToList()
                    },
                new LineSeries
                    {
                        Name = "Ask",
                        Data = askData.ToList()
                    }
            };
            var chartOption = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Pm inputs");
            chartOption.Series = series;

            return chartOption;
        }

        public Highcharts GetBid()
        {
            var predictions = GetPmPredictions();

            var chartData = predictions.Select(prediction =>
            {
                return new LineSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.Bid
                };
            });

            var series = new List<Series>
            {
                new LineSeries
                {
                    Name = "Bid",
                    Data = chartData.ToList()
                }
            };
            var chartOptions = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Bid values");
            chartOptions.Series = series;

            return chartOptions;
        }

        public Highcharts GetAsk()
        {
            var predictions = GetPmPredictions();

            var chartData = predictions.Select(prediction =>
            {
                return new LineSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.Ask
                };
            });

            var series = new List<Series>
            {
                new LineSeries
                {
                    Name = "Ask",
                    Data = chartData.ToList()
                }
            };
            var chartOptions = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Ask values");
            chartOptions.Series = series;

            return chartOptions;
        }

        public Highcharts GetOutput()
        {
            var predictions = GetPmPredictions();

            var series = new List<Series>();

            foreach (var ordinal in predictions.ElementAt(0).OutputNodeValues.Select(n => n.OrdinalId))
            {
                var outputData = predictions.Select(prediction =>
                {
                    return new LineSeriesData
                    {
                        X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                        Y = prediction.OutputNodeValues.First(n => n.OrdinalId == ordinal).OutputValue
                    };
                });

                series.Add(new LineSeries
                {
                    Name = predictions.ElementAt(0).OutputNodeValues.First(n => n.OrdinalId == ordinal).Meaning,
                    Data = outputData.ToList()
                });
            }

            var chartOption = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Pm outputs");
            chartOption.Series = series;
            return chartOption;
        }

        public Highcharts GetBusinessResultPpmPerLabel()
        {
            var predictions = GetPmPredictions();

            var series = new List<Series>();

            foreach (var label in predictions.ElementAt(0).BusinessResultPpmPerLabel)
            {
                var outputData = predictions.Select(prediction =>
                {
                    return new LineSeriesData
                    {
                        X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                        Y = prediction.BusinessResultPpmPerLabel.First(br => br.Name == label.Name).BusinessResultPpm
                    };
                });

                series.Add(new LineSeries
                {
                    Name = label.Name,
                    Data = outputData.ToList()
                });
            }

            var chartOption = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Business result Ppm per label");
            chartOption.Series = series;

            return chartOption;
        }

        public Highcharts GetPredictedBusinessYield()
        {
            var predictions = GetPmPredictions();

            var chartData = predictions.Select(prediction =>
            {
                return new LineSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.PredictedBusinessYieldPpm
                };
            });

            var series = new List<Series>
            {
                new LineSeries
                {
                    Name = "Business yield",
                    Data = chartData.ToList()
                }
            };
            var chartOptions = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Predicted business yield");
            chartOptions.Series = series;

            return chartOptions;
        }

        public Highcharts GetDesiredPredictionsBinary()
        {
            var predictions = GetPmPredictions();

            var buyDPredictionSeriesData = predictions.Select(prediction =>
            {
                return new ColumnSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.DesiredPredictionsBinaryStruct.Buy ? 1 : 0
                };
            });

            var sellDPredictionSeriesData = predictions.Select(prediction =>
            {
                return new ColumnSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.DesiredPredictionsBinaryStruct.Sell ? 1 : 0
                };
            });

            var noneDPredictionSeriesData = predictions.Select(prediction =>
            {
                return new ColumnSeriesData
                {
                    X = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds(),
                    Y = prediction.DesiredPredictionsBinaryStruct.None ? 1 : 0
                };
            });

            var series = new List<Series>
            {
                new ColumnSeries
                {
                    Name = "Desired prediction buy",
                    Data = buyDPredictionSeriesData.ToList()
                },
                new ColumnSeries
                {
                    Name = "Desired prediction sell",
                    Data = sellDPredictionSeriesData.ToList()
                },
                new ColumnSeries
                {
                    Name = "Desired prediction none",
                    Data = noneDPredictionSeriesData.ToList()
                }
            };
            var chartOptions = chartsDomain.GetPmPredictionsSeriesDefaultOptions("Desired predictions");
            chartOptions.Chart.Type = ChartType.Column;
            chartOptions.Series = series;
            chartOptions.PlotOptions = new PlotOptions
            {
                Column = new PlotOptionsColumn
                {
                    ShadowBool = false,
                    Grouping = false
                }
            };

            chartOptions.YAxis = new List<YAxis>
            {
                new YAxis
                {
                    Type = YAxisType.Category,
                    Categories = new List<string>
                    {
                        "False",
                        "True"
                    }
                }
            };

            return chartOptions;
        }

        public Highcharts GetDesiredPredictionsBinaryHeatmap()
        {
            var predictions = GetPmPredictions();

            var seriesData = new List<HeatmapSeriesData>();

            foreach(var prediction in predictions)
            {
                var date = prediction.DateTimeUtcDate.ToUnixTimeMilliseconds();
                seriesData.Add(new HeatmapSeriesData
                {
                    Name = "Buy",
                    X = date,
                    Y = 0,
                    Value = prediction.DesiredPredictionsBinaryStruct.Buy ? 1 : 0
                });

                seriesData.Add(new HeatmapSeriesData
                {
                    Name = "None",
                    X = date,
                    Y = 1,
                    Value = prediction.DesiredPredictionsBinaryStruct.None ? 1 : 0
                });

                seriesData.Add(new HeatmapSeriesData
                {
                    Name = "Sell",
                    X = date,
                    Y = 2,
                    Value = prediction.DesiredPredictionsBinaryStruct.Sell ? 1 : 0
                });
            }

            var chartOptions = chartsDomain.GetNeuralNetworksDefaultOptions("Desired predictions",0,2,0,1,60000);
            (chartOptions.Series.First() as HeatmapSeries).Data = seriesData;

            chartOptions.YAxis.First().Type = YAxisType.Category;
            chartOptions.YAxis.First().Categories = new List<string>
            {
                "Buy",
                "None",
                "Sell"
            };
            chartOptions.ColorAxis.TickAmount = 2;

            (chartOptions.Series.First() as HeatmapSeries).Tooltip = new HeatmapSeriesTooltip
            {
                HeaderFormat = "Desired predictions<br/>",
                PointFormat = "<b>Date:</b>{point.x:%e %b, %Y %H:%M:%S.%L}<br/><b>{point.name}:</b> {point.value}<br/>"
            };

            return chartOptions;
        }
    }
}