var dateOTStart = document.getElementById("dateOTStart");
var dateOTEnd = document.getElementById("dateOTEnd");
if (dateOTStart != null)
    dateOTStart.addEventListener("change", function () {
        dateOTEnd.setAttribute("min", dateOTStart.value);
        if (Date.parse(dateOTEnd.value) < Date.parse(dateOTStart.value))
            dateOTEnd.value = dateOTStart.value;
        dateOTEnd.disabled = false;
        HomeSearch("Home\\SearchFollow");
    });

if (dateOTEnd != null)
    dateOTEnd.addEventListener("change", function () {
        HomeSearch("Home\\SearchFollow");
    });