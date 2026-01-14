fetch("Home/qryDataToCanvas", {
    method: "POST",
    referrerPolicy: "strict-origin-when-cross-origin",
    credentials: "same-origin",
}).then(function (response) {
    // When the page is loaded convert it to text
    return response.text()
    }).then(function (json) {
        json = JSON.parse(json);
        CreateChart(json);
    });

var dateOTStart = document.getElementById("dateOTStart");
var dateOTEnd = document.getElementById("dateOTEnd");

if (dateOTStart != null)
    dateOTStart.addEventListener("change", function () {
        dateOTEnd.setAttribute("min", dateOTStart.value);
        if (Date.parse(dateOTEnd.value) < Date.parse(dateOTStart.value))
            dateOTEnd.value = dateOTStart.value;
        dateOTEnd.disabled = false;
        HomeSearch("Home\\SearchGraph");
    });

if (dateOTEnd != null)
    dateOTEnd.addEventListener("change", function () {
        HomeSearch("Home\\SearchGraph");
    });


function CreateChart(jsondata) {
    var arr = JSON.parse(jsondata);
    var chart = new CanvasJS.Chart("chartContainer", {
        animationEnabled: true,
        title: {
            text: "ผลรวมการทำงานล่วงเวลาในปี 2024",
            fontColor: "darkgray",
            fontFamily: "LeelawaD"
        },
        axisX: {
            valueFormatString: "DD MMM,YY",
            titleFontFamily: "LeelawaD",
            titleFontColor: "darkgray",
            labelFontFamily: "LeelawaD",
        },
        axisY: {
            title: "ชั่วโมง",
            titleFontFamily: "LeelawaD",
            titleFontColor: "darkgray",
            labelFontFamily: "LeelawaD",
        },
        legend: {
            cursor: "pointer",
            fontSize: 16,
            itemclick: toggleDataSeries,
            fontFamily: "LeelawaD"
        },
        toolTip: {
            shared: true,
            fontFamily: "LeelawaD"
        },
        data: [{
            name: "",
            type: "line",
            yValueFormatString: "#0.## ชั่วโมง",
            showInLegend: true,
            indexLabelFontFamily: "LeelawaD",
            dataPoints: arr
        //},
        //{
        //    name: "Martha Vineyard",
        //    type: "spline",
        //    yValueFormatString: "#0.## °C",
        //    showInLegend: true,
        //    dataPoints: [
        //        { x: new Date(2017, 6, 24), y: 20 },
        //        { x: new Date(2017, 6, 25), y: 20 },
        //        { x: new Date(2017, 6, 26), y: 25 },
        //        { x: new Date(2017, 6, 27), y: 25 },
        //        { x: new Date(2017, 6, 28), y: 25 },
        //        { x: new Date(2017, 6, 29), y: 25 },
        //        { x: new Date(2017, 6, 30), y: 25 }
        //    ]
        //},
        //{
        //    name: "Nantucket",
        //    type: "spline",
        //    yValueFormatString: "#0.## °C",
        //    showInLegend: true,
        //    dataPoints: [
        //        { x: new Date(2017, 6, 24), y: 22 },
        //        { x: new Date(2017, 6, 25), y: 19 },
        //        { x: new Date(2017, 6, 26), y: 23 },
        //        { x: new Date(2017, 6, 27), y: 24 },
        //        { x: new Date(2017, 6, 28), y: 24 },
        //        { x: new Date(2017, 6, 29), y: 23 },
        //        { x: new Date(2017, 6, 30), y: 23 }
        //    ]
        }
            ]
    });
    chart.render();

    function toggleDataSeries(e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
            e.dataSeries.visible = false;
        }
        else {
            e.dataSeries.visible = true;
        }
        chart.render();
    }
}

