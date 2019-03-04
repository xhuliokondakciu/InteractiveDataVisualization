var SyncCharts = (function ($, H) {

    if (typeof H === 'undefined' || typeof H.chart === 'undefined') {
        throw 'HighChart not detected. SyncCharts will not work';
    }

    H.Pointer.prototype.reset = function () {
        //remove toast together with tooltip
        this.chart.tooltip.hide();
        return undefined;
    };


    H.Point.prototype.highlight = function (event) {
        this.series.chart.xAxis[0].drawCrosshair(event, this); // Show the crosshair
    };
    /**
     * Create x synchronized charts
     * @param {any} selector Css selector of the charts container
     * @param {any} chartsOptions Array of HighChart options
     * @param {number} plotAreaWidth Plot area width of the synchronized charts in pixel
     * @returns {void}
     */
    var createCharts = function (selector, chartsOptions, plotAreaWidth = null) {

        var data = {
            charts: [],
            container: "",
            hoverEvent: {},
            plotAreaWidth: null,
            plotAreaHeight: null,
            toastr: {
                element: null,
                option: {
                    closeButton: true,
                    positionClass: 'toast-bottom-right',
                    toastClass: 'chart-data-info',
                    timeOut: 0,
                    extendedTimeOut: 0,
                    tapToDismiss: false,
                    hideDuration: 0
                },
                closed: false,
                windowed: false,
                window: null,
                onCloseClick: function () {
                    data.toastr.closed = true;
                }
            },
            listenForResize: true
        };

        var synchronizeChartsPointer = function (e) {

            var currentChart = $(this).highcharts(),
                chart,
                event = currentChart.pointer.normalize(e.originalEvent),
                currentPoint = currentChart.series.filter(s=>s.visible)[0].searchPoint(event, true),
                point,
                i;

            if (!currentPoint)
                return;

            data.hoverEvent = event;
            showHoverDataInfo(event, currentPoint);

            for (let chart of data.charts) {

                if (chart == currentChart)
                    continue;

                point = getSyncedChartPoint(currentPoint, chart);

                if (point)
                    point.highlight({
                        chartX: point.plotX + chart.plotLeft,
                        chartY: point.plotY + chart.plotTop
                    });

            }
        };

        var getSyncedChartPoint = function (currentPoint, chart) {
            const serie = chart.series.filter(s => s.visible)[0];
            const dataPoint = serie.points.find(p => p.x === currentPoint.x);
            let fixedEventForChart = {};
            let point = null;
            if (dataPoint) {
                fixedEventForChart = {
                    chartX: dataPoint.plotX + chart.plotLeft,
                    chartY: dataPoint.plotY + chart.plotTop
                };
                point = serie.searchPoint(fixedEventForChart, true); // Get the hovered point
            } else {
                fixedEventForChart = {
                    chartX: currentPoint.plotX + chart.plotLeft,
                    chartY: currentPoint.plotY + chart.plotTop
                };
                point = serie.searchPoint(fixedEventForChart, true); // Get the hovered point
            }

            return point;
        };

        /**
         * Get chart extremes bazed on the zoom on another chart
         * @param {number} chartDataMin The data min of the chart
         * @param {number} chartDataMax The data max of a chart
         * @param {number} zoomedChartDataMin The data min of the zoomed chart
         * @param {number} zoomedChartDataMax The data max of the zoomed chart
         * @param {number} zoomedChartMin The viewport min of the zoomed chart
         * @param {number} zoomedChartMax The viewport max of the zoomed chart
         * @returns {{min:number,max:number}} chart extremes
         */
        var getChartExtremes = function (
            chartDataMin,
            chartDataMax,
            zoomedChartDataMin,
            zoomedChartDataMax,
            zoomedChartMin,
            zoomedChartMax) {

            let min, max;
            try {
                const zoomedChartLength = zoomedChartDataMax - zoomedChartDataMin;
                const chartLength = chartDataMax - chartDataMin;
                let minRation = (zoomedChartMin - zoomedChartDataMin) / zoomedChartLength;
                let maxRation = (zoomedChartMax - zoomedChartDataMin) / zoomedChartLength;
                min = minRation * chartLength + chartDataMin;
                max = maxRation * chartLength + chartDataMin;
            } catch (error) {
                return {
                    min: chartDataMin,
                    max: chartDataMax
                };
            }

            return {
                min,
                max
            };
        };

        /**
         * Set new chart extremes bazed from the zoom on another chart
         * @param {number} zoomedChartDataMin Zoomed chart data min
         * @param {number} zoomedChartDataMax Zoomed chart data max
         * @param {number} zoomedChartMin Zoomed chart viewport min
         * @param {number} zoomedChartMax Zoomed chart viewport max
         * @param {object} zoomedChart Zoomed chart object
         * @returns {void}
         */
        var updateChartsExtremes = function (
            zoomedChartDataMin,
            zoomedChartDataMax,
            zoomedChartMin,
            zoomedChartMax,
            zoomedChart) {
            for (let chart of data.charts) {
                if (chart === zoomedChart) continue; //skip if zoomed chart

                let { min, max } = getChartExtremes(
                    chart.xAxis[0].dataMin,
                    chart.xAxis[0].dataMax,
                    zoomedChartDataMin,
                    zoomedChartDataMax,
                    zoomedChartMin,
                    zoomedChartMax);

                if (chart.xAxis[0].setExtremes) {

                    //Pass additional arguments to event so we dont have infinite loop
                    chart.xAxis[0].setExtremes(min, max, false, false, {
                        updateChart: false
                    });

                    chart.redraw(); //fix view after zoom
                }



            }
        };

        var registerChartEvents = function (chartOption) {
            chartOption.chart.events = Object.assign(chartOption.chart.events || {}, {
                destroy: function (e) {
                    let index = data.charts.indexOf(e.target);
                    if (!isNaN(index)) {
                        data.charts.splice(index, 1);
                        if (data.charts.length <= 1) {
                            if (data.toastr.element)
                                data.toastr.element.remove();
                            if (data.toastr.window) {
                                data.toastr.window.close();
                            }
                        }
                    }

                    //Remove window reiseze listener
                    const chartWindow = e.target.container.ownerDocument.defaultView || e.target.container.ownerDocument.parentWindow;
                    chartWindow.removeEventListener("optimizedResize", debouncedChartWindowResize, false);
                }
            });
        };


        /**
         * Register the set extrem event in the chart options
         * @param {object} chartOption HighChart option of the chart to init
         * @returns {void}
         */
        var registerSetExtremesEvent = function (chartOption) {
            chartOption.xAxis = chartOption.xAxis.map(axis => {
                axis.events = Object.assign(axis.events || {}, {
                    setExtremes: (event) => {
                        if (event.updateChart === false) return; //So that we dont have infinite loop

                        updateChartExtremesDebounced(
                            event.target.dataMin,
                            event.target.dataMax,
                            event.min || event.target.dataMin,
                            event.max || event.target.dataMax,
                            event.target.chart);
                    }
                });

                return axis;
            });
        };

        let updateChartExtremesDebounced = KGT.Helper.debounce(updateChartsExtremes, 150, false);

        var addZooming = function (chartOption) {
            //don't add scroll wheel zooming if heatmap
            if (chartOption.chart && chartOption.chart.type === 'heatmap') return;

            chartOption.mapNavigation = {
                enabled: true,
                enableButtons: false
            };
        };

        var normalizeChartPlotWidth = function (chartOption) {
            //chartOption.chart = Object.assign(chartOption.chart || {}, {
            //    plotAreaWidth: data.plotAreaWidth
            //});
        };

        /**
         * Add tooltip formatter to charts to show all charts data in one tooltip
         * @param {any} chartOption Options of the chart to add the formatter
         */
        var setTooltipUpdateFunction = function (chartOption) {
            //Don't show all the charts data in one tooltip if toaster library is found
            if (toastr) return;
            chartOption = Object.assign(chartOption, {
                tooltip: Object.assign(chartOption.tooltip || {}, {
                    shared: true,
                    formatter: function () {
                        const currentPoint = currentChart.series.filter(s => s.visible)[0].searchPoint(data.hoverEvent, true);
                        const s = '<b>' + H.dateFormat('%a %d %b %H:%M:%S', this.x) + '</b>';

                        for (let chart of data.charts) {

                            //Don't show for heatmap charts
                            if (chart.userOptions.chart.type === 'heatmap') continue;

                            const fixedEventForChart = {
                                chartX: currentPoint.plotX + chart.plotLeft,
                                chartY: currentPoint.plotY + chart.plotTop
                            };
                            let points = chart.series.map(serie => serie.searchPoint(fixedEventForChart, true));

                            s += `<br /><br /><br /><b>${chart.options.title.text}</b>`;
                            s += `<small>${H.dateFormat('%a %d %b %H:%M:%S.%L', points[0].x)}</small><br />`;
                            points.forEach(point => {

                                if (!point) return;
                                s += `<br/>${point.series.name}: ${point.y}`;
                            });
                        }

                        return s;
                    }
                })
            });
        };

        /**
         * Display synchronized chart values in a corner window at the bottom right
         * @param {any} event Hover event
         * @param {any} currentPoint Current selected point
         * @returns {void}
         */
        var showHoverDataInfo = function (event, currentPoint) {
            if (!toastr || data.toastr.closed || data.charts.length <= 1) return;

            let dataInfoHtml = createInfoHtml(event, currentPoint);

            if (data.toastr.windowed) {
                showWindowedInfo(dataInfoHtml);
            } else {
                showToastMessage(dataInfoHtml);
            }
        };

        var showWindowedInfo = function (infoHtml) {
            if (!data.toastr.window) {
                showToastMessage(infoHtml);
            } else {
                $(data.toastr.window.document.body).find(".chart-data-info-window").html(infoHtml);
            }
        };

        var showToastMessage = function (infoHtml, infoTitle = "") {
            if (!data.toastr.element || data.toastr.element.is(':hidden')) {
                data.toastr.element = toastr.info(infoHtml, infoTitle, data.toastr.option);
                data.toastr.element.find('button').bind('click', data.toastr.onCloseClick);
            } else {
                data.toastr.element.find('.toast-title').html(infoTitle);
                data.toastr.element.find('.toast-message').html(infoHtml);
            }

            if ($(data.toastr.element).find(".open-info-windowed").length > 0) return;

            let windowBtn = $("<button />", {
                type: "button",
                class: "btn btn-secondary open-info-windowed",
                text: "Open popup"
            });

            windowBtn.click(() => {
                let infoHtml = $(data.toastr.element).find(".toast-message").html();
                data.toastr.windowed = true;
                data.toastr.window = ChartWindowManager.setUpChartInfoWindow(infoHtml, data.toastr);
            });


            $(data.toastr.element).append(windowBtn);
        };

        /**
         * Create info html for sync chart data
         * @param {eny} event Pointer event
         * @param {any} currentPoint Current selected point
         * @returns {string} Data info fomratted html
         */
        var createInfoHtml = function (event, currentPoint) {
            if (!currentPoint)
                return "";

            let dataInfoTitle = `<div style="font-size:16px"><b>${H.dateFormat('%a %d %b %H:%M:%S', currentPoint.x)}</b></div><br/>`;

            let dataInfoText = dataInfoTitle;

            for (let chart of data.charts) {

                //Don't show for heatmap charts
                if (chart.userOptions.chart.type === 'heatmap') continue;

                dataInfoText += `<b>${chart.options.title.text}</b>`;
                const point = getSyncedChartPoint(currentPoint, chart);
                if (!point) return "";
                const fixedEventForChart = {
                    chartX: point.plotX + chart.plotLeft,
                    chartY: point.plotY + chart.plotTop
                };

                //Show data of only visible series
                let points = chart.series
                    .filter(serie => serie.visible)
                    .map(serie => serie.searchPoint(fixedEventForChart, true))
                    .filter(p => p !== null && p !== undefined);

                dataInfoText += `<br/><ul>`;
                points.forEach((point, index) => {
                    if (!point) return;
                    dataInfoText += `<li style="color:${point.color};">${point.series.name}: ${point.y}</li>`;
                });
                dataInfoText += `</ul>`;
            }

            return dataInfoText;
        };

        /**
         * Insert chart in the charts array
         * @param {any} chart Chart object to insert in the array
         * @returns {void}
         */
        var saveChart = function (chart) {
            data.charts.push(chart);
        };

        /**
         * Start the initialization of the x synchronized chart inside the container
         * @param {object} chartOption Option of the chart to init
         * @param {object} container container
         * @returns {object} The created chart
         */
        var initChart = function (chartOption, container) {

            var $container = $(container);
            prepareChartOptions(chartOption);
            var $chartElement = $('<div />');
            $container.append($chartElement);
            var chart = H.chart($chartElement[0], chartOption);
            registerWindowResize(chart);
            $chartElement.bind('mousemove touchmove touchstart', synchronizeChartsPointer);
            saveChart(chart);
            chart.redraw();
            return chart;
        };

        var registerWindowResize = function (chart) {
            const chartWindow = chart.container.ownerDocument.defaultView || chart.container.ownerDocument.parentWindow;
            KGT.Helper.throttle("resize", "optimizedResize", chartWindow);
            chartWindow.addEventListener("optimizedResize", debouncedChartWindowResize);
        };

        var debouncedChartWindowResize = KGT.Helper.debounce((e) => {
            if (!data.listenForResize) return;

            data.charts.forEach((chartToResize,index) => {
                data.listenForResize = false;
                const cWindow = chartToResize.container.ownerDocument.defaultView || chart.container.ownerDocument.parentWindow;
                if (cWindow.outerWidth !== e.target.outerWidth || cWindow.outerHeight <= e.target.outerHeight - 10 || cWindow.outerHeight >= e.target.outerHeight + 10) {
                    cWindow.resizeTo(e.target.outerWidth, e.target.outerHeight);
                }
                let chartHeight = 0.80 * e.target.outerHeight;
                let chartWidth = e.target.outerWidth;
                chartToResize.setSize(chartWidth, chartHeight);
            });
            data.listenForResize = true;
        }, 500, false);

        var prepareChartOptions = function (chartOption) {
            addZooming(chartOption);
            registerSetExtremesEvent(chartOption);
            setTooltipUpdateFunction(chartOption);
            registerChartEvents(chartOption);
            normalizeChartPlotWidth(chartOption);
        };

        /**
         * Init container with the synchronized charts
         * @param {any} selector Css selector of the container element
         * @param {any} chartsOptions Array of HighChart options
         * @param {number} plotAreaWidth Plot area width of the synchronized charts in pixel
         * @returns {object} Chart interface
         */
        var init = function (selector, chartsOptions, plotAreaWidth) {
            if ($(selector).length === 0) {
                console.log("Charts container couldn't be found");
                return null;
            }

            if (!chartsOptions && (typeof chartsOptions).toLowerCase() !== 'object') {
                console.log('Wrong HighChart options passed');
                return null;
            }

            data.plotAreaWidth = plotAreaWidth;

            chartsOptions.forEach(option => {
                initChart(option, selector);
            });

            return chartService;
        };

        var chartService = {
            addChart: initChart,
            getCharts: function () {
                return data.charts;
            }
        };

        return init(selector, chartsOptions, plotAreaWidth);
    };

    return {
        createCharts: createCharts
    };
}(jQuery, Highcharts));