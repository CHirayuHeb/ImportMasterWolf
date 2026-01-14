LoadScript("js\\MyRequest\\EventModalWaiting.js", "EventModalWaiting");
//get button
var btnWaiting = document.getElementsByClassName("filter").item(0);
var btnDone = document.getElementsByClassName("filter").item(1);
var btnDisapp = document.getElementsByClassName("filter").item(2);
var btnDraft = document.getElementsByClassName("filter").item(3);
//path controller//action

var toFlowWaiting = "MyRequest\\DisplayWaiting";
var toFlowDone = "MyRequest\\DisplayDone";
var toFlowDisapp = "MyRequest\\DisplayDisapproved";
var toDraftPage = "MyRequest\\DisplayDraft";

if (btnWaiting != null)
    btnWaiting.addEventListener("click", async function () {
        let url = toFlowWaiting;
        await DisplayResult(url);
        BtnActiive("FlowWaiting");
        //$("#RequestControl").collapse("hide");
    });

if (btnDone != null)
    btnDone.addEventListener("click", function () {
        let url = toFlowDone;
        DisplayResult(url);
        BtnActiive("FlowDone");
        $("#RequestControl").collapse("hide");
    });

if (btnDisapp != null)
    btnDisapp.addEventListener("click", function () {
        let url = toFlowDisapp;
        DisplayResult(url);
        BtnActiive("FlowDisapproved");
        $("#RequestControl").collapse("hide");
    });

if (btnDraft != null)
    btnDraft.addEventListener("click", function () {
        let url = toDraftPage;
        DisplayResult(url);
        BtnActiive("DraftPage");
        $("#RequestControl").collapse("hide");
    });