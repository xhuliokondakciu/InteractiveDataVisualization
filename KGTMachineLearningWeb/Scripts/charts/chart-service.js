class ChartService {

    constructor() {
        
    }

    _onError(errorMessage, error) {
        console.error(errorMessage, error);
    }

    sendGetChartOptionsRequest(chartId) {
        const url = `/Chart/GetHighchartOption/${chartId}`;
        return $.ajax({
            url: url,
            method: "GET",
            dataType: "json",
            error: (e) => {
                this._onError("Couldn't get chart options", e);
            }
        });
    }
}