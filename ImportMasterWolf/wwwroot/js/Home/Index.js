var btnHour = document.getElementsByClassName("hour").item(0);
var btnDocument = document.getElementsByClassName("document").item(0);
var toHourAction = "Home\\DisplayHour";
var toDocumentAction = "Home\\DisplayDocument";

btnHour.addEventListener("click", function () {
    let url = toHourAction;
    DisplayResult(url);
    BtnActiive("hour");
});
btnDocument.addEventListener("click", function () {
    let url = toDocumentAction;
    DisplayResult(url);
    BtnActiive("document");
});