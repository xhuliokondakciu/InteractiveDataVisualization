var ChartWindowManager = /** @class */ (function () {
    function ChartWindowManager() {
    }
    ChartWindowManager.setUpChartWindow = function (chartData, openedCharts) {
        var _this = this;
        $("#" + this.getChartElementId(chartData.Id)).addClass("chart-selected");
        var chartWindow = window.open("", "_blank", "left=0,top=0,width=" + this.chartWindowWidth + ",height=" + this.chartWindowHeight);
        //Add windows styles
        this.addWindowStyles(chartWindow);
        $(chartWindow.document.head).append($("<title />", {
            text: chartData.Title
        }));
        chartWindow.onunload = function (ev) {
            $("#" + _this.getChartElementId(chartData.Id)).removeClass("chart-selected");
            var openedChart = openedCharts.find(function (oc) { return oc.id === chartData.Id; });
            if (openedChart)
                openedCharts.splice(openedCharts.indexOf(openedChart), 1);
            if (openedChart && openedChart.chart && openedChart.chart.destroy)
                openedChart.chart.destroy();
        };
        return chartWindow;
    };
    ChartWindowManager.setUpChartInfoWindow = function (infoText, toastrData) {
        var infoWindow = window.open("", "_blank", "left=0,top=0,width=400,height=400");
        infoWindow.onunload = function () {
            toastrData.window = null;
            toastrData.windowed = false;
        };
        this.addWindowStyles(infoWindow);
        var chartDataInfoContainer = $("<div />", {
            class: "chart-data-info-window"
        });
        chartDataInfoContainer.html(infoText);
        $(infoWindow.document.body).append(chartDataInfoContainer);
        //data.toastr.window.document.body.style.backgroundColor = "#f8f9fa";
        //$(data.toastr.window.document.body).css("background-color", "#f8f9fa");
        $(toastrData.element).remove();
        return infoWindow;
    };
    ChartWindowManager.getChartElementId = function (id) {
        return "chart-" + id;
    };
    ChartWindowManager.addWindowStyles = function (window) {
        return $.get("/Content/Charts/chart-window.css", function (styles) {
            $(window.document.head).append($("<style />", {
                rel: "stylesheet",
                text: styles
            }));
        });
    };
    ChartWindowManager.chartWindowWidth = screen.width / 2;
    ChartWindowManager.chartWindowHeight = 450;
    ChartWindowManager.chartWidth = ChartWindowManager.chartWindowWidth - 160;
    return ChartWindowManager;
}());
//# sourceMappingURL=ChartWindowManager.js.map