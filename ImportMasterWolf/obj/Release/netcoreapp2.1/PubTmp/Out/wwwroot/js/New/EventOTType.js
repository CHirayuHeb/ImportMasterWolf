var actionOTMyData = "New\\OTMyData";
var targetOTMyData = ForwardModalID + "2";

var btnWeekDay = document.querySelector("button.weekday");
var btnWeekEnd = document.querySelector("button.weekend");

if (btnWeekDay != null) {
    btnWeekDay.addEventListener("click", function () {
        GoToOTMyData(actionOTMyData, targetOTMyData, "วันธรรมดา");
    });
}
if (btnWeekEnd != null) {
    btnWeekEnd.addEventListener("click", function () {
        GoToOTMyData(actionOTMyData, targetOTMyData, "วันหยุด");
    });
}