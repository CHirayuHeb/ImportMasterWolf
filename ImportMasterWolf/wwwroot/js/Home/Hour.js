var btnHour = document.querySelectorAll("button.hour").item(0);
var btnFollow = document.querySelectorAll("button.follow").item(0);
var btnDocument = document.querySelectorAll("button.document").item(0);
var btnGraph = document.querySelectorAll("button.graph").item(0);
var cbOTReq = document.getElementById("cbOTReq");
//var cbOTDept = document.getElementById("cbOTDept");
//var cbOTStatus = document.getElementById("cbOTStatus");

if (btnHour != null)
    btnHour.addEventListener("click", function (e) {
        DisplayResult("Home\\DisplayHour");
        BtnActiive("hour");
    });

if (btnFollow != null)
    btnFollow.addEventListener("click", function (e) {
        DisplayResult("Home\\DisplayFollow");
        BtnActiive("follow");
    });

if (btnDocument != null)
    btnDocument.addEventListener("click", function (e) {
        DisplayResult("Home\\DisplayDocument");
        BtnActiive("document");
    });

if (btnGraph != null)
    btnGraph.addEventListener("click", function (e) {
        DisplayResult("Home\\DisplayGraph");
        BtnActiive("graph");
    });

//if (cbOTDept != null)
//    cbOTDept.addEventListener("click", function () {
//        let txtOTDept = document.getElementById("txtOTDept");
//        txtOTDept.disabled = !this.checked;
//    });

//if (cbOTStatus != null)
//    cbOTStatus.addEventListener("click", function () {
//        let txtOTStatus = document.getElementById("txtOTStatus");
//        txtOTStatus.disabled = !this.checked;
//    });