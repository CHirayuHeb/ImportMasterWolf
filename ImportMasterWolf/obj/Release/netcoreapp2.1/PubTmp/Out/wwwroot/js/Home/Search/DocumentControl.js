var dateOTStart = document.getElementById("dateOTStart");
var dateOTEnd = document.getElementById("dateOTEnd");
var btnToXlsmSelected = document.getElementById("btnToXlsmSeleted");
if (dateOTStart != null)
    dateOTStart.addEventListener("change", function () {
        dateOTEnd.setAttribute("min", dateOTStart.value);
        if (Date.parse(dateOTEnd.value) < Date.parse(dateOTStart.value))
            dateOTEnd.value = dateOTStart.value;
        dateOTEnd.disabled = false;
        HomeSearch("Home\\SearchDocument");
    });

if (dateOTEnd != null)
    dateOTEnd.addEventListener("change", function () {
        HomeSearch("Home\\SearchDocument");
    });

if (btnToXlsmSelected != null)
    btnToXlsmSelected.addEventListener("click",async function () {
        let arr = new Array();;
        let checkboxs = document.querySelectorAll("input.target-no");
        checkboxs.forEach(function (ele) {
            if (ele.checked)
                arr.push({ "no": ele.value });
        });
        await ExportToXlsm(arr);
        //fetch("Functions/ToListXlsm", {
        //    method: "POST",
        //    referrerPolicy: "strict-origin-when-cross-origin",
        //    credentials: "same-origin",
        //    headers: { 'Content-Type': 'application/json' },
        //    body: JSON.stringify(arr),
        //}).then(function (response) {
        //    return response.text()
        //    }).then(function (xlsm) {
        //    location.href = "Functions/XlsxFromByte";
        //});
    });