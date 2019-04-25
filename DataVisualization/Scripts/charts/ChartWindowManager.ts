declare var $;
class ChartWindowManager {

    static chartWindowWidth: number = screen.width / 2;
    static chartWindowHeight: number = 450;
    static chartWidth: number = ChartWindowManager.chartWindowWidth - 160;

    static setUpChartWindow(chartData, openedCharts) {

        $("#" + this.getChartElementId(chartData.Id)).addClass("chart-selected");

        let chartWindow = window.open("", "_blank", `left=0,top=0,width=${this.chartWindowWidth},height=${this.chartWindowHeight}`);

        //Add windows styles
        this.addWindowStyles(chartWindow);

        $(chartWindow.document.head).append($("<title />", {
            text: chartData.Title
        }));

        chartWindow.onunload = (ev) => {
            $("#" + this.getChartElementId(chartData.Id)).removeClass("chart-selected");
            const openedChart = openedCharts.find(oc => oc.id === chartData.Id);
            if (openedChart)
                openedCharts.splice(openedCharts.indexOf(openedChart), 1);
            if (openedChart && openedChart.chart && openedChart.chart.destroy)
                openedChart.chart.destroy();
        };

        return chartWindow;

    }

    static setUpChartInfoWindow(infoText, toastrData) {
        let infoWindow = window.open("", "_blank", `left=0,top=0,width=400,height=400`);

        infoWindow.onunload = () => {
            toastrData.window = null;
            toastrData.windowed = false;
        };

        this.addWindowStyles(infoWindow);

        let chartDataInfoContainer = $("<div />", {
            class: "chart-data-info-window"
        });
        chartDataInfoContainer.html(infoText);

        $(infoWindow.document.body).append(chartDataInfoContainer);
        //data.toastr.window.document.body.style.backgroundColor = "#f8f9fa";
        //$(data.toastr.window.document.body).css("background-color", "#f8f9fa");
        $(toastrData.element).remove();

        return infoWindow;
    }

    static getChartElementId(id: number): string {
        return "chart-" + id;
    }

    private static addWindowStyles(window: Window): any {
        return $.get("/Content/Charts/chart-window.css", (styles) => {
            $(window.document.head).append($("<style />", {
                rel: "stylesheet",
                text: styles
            }));
        });
    }
}