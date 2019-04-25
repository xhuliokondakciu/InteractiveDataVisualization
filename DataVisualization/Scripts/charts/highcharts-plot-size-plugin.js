(function (H) {
    H.wrap(H.Chart.prototype, 'setChartSize', function (proceed, skipAxes) {
        var chart = this,
            inverted = chart.inverted,
            renderer = chart.renderer,
            chartWidth = chart.chartWidth,
            chartHeight = chart.chartHeight,
            optionsChart = chart.options.chart,
            spacing = chart.spacing,
            clipOffset = chart.clipOffset,
            clipX,
            clipY,
            plotLeft,
            plotTop,
            plotWidth,
            plotHeight,
            plotBorderWidth,
            plotAreaWidth = chart.options.chart.plotAreaWidth,
            plotAreaHeight = chart.options.chart.plotAreaHeight;

        if (plotAreaWidth) {
            chart.plotWidth = plotWidth = plotAreaWidth;
            chart.plotLeft = plotLeft = Math.round((chartWidth - plotAreaWidth) / 2);
        } else {
            chart.plotLeft = plotLeft = Math.round(chart.plotLeft);
            chart.plotWidth = plotWidth = Math.max(0, Math.round(chartWidth - plotLeft - chart.marginRight));
        }
        if (plotAreaHeight) {
            chart.plotTop = plotTop = Math.round((chartHeight - plotAreaHeight) / 2);
            chart.plotHeight = plotHeight = plotAreaHeight;
        } else {
            chart.plotTop = plotTop = Math.round(chart.plotTop);
            chart.plotHeight = plotHeight = Math.max(0, Math.round(chartHeight - plotTop - chart.marginBottom));
        }

        chart.plotSizeX = inverted ? plotHeight : plotWidth;
        chart.plotSizeY = inverted ? plotWidth : plotHeight;

        chart.plotBorderWidth = optionsChart.plotBorderWidth || 0;

        // Set boxes used for alignment
        chart.spacingBox = renderer.spacingBox = {
            x: spacing[3],
            y: spacing[0],
            width: chartWidth - spacing[3] - spacing[1],
            height: chartHeight - spacing[0] - spacing[2]
        };
        chart.plotBox = renderer.plotBox = {
            x: plotLeft,
            y: plotTop,
            width: plotWidth,
            height: plotHeight
        };

        plotBorderWidth = 2 * Math.floor(chart.plotBorderWidth / 2);
        clipX = Math.ceil(Math.max(plotBorderWidth, clipOffset[3]) / 2);
        clipY = Math.ceil(Math.max(plotBorderWidth, clipOffset[0]) / 2);
        chart.clipBox = {
            x: clipX,
            y: clipY,
            width: Math.floor(chart.plotSizeX - Math.max(plotBorderWidth, clipOffset[1]) / 2 - clipX),
            height: Math.max(0, Math.floor(chart.plotSizeY - Math.max(plotBorderWidth, clipOffset[2]) / 2 - clipY))
        };

        if (!skipAxes) {
            Highcharts.each(chart.axes, function (axis) {
                axis.setAxisSize();
                axis.setAxisTranslation();
            });
        }
    });
}(Highcharts));