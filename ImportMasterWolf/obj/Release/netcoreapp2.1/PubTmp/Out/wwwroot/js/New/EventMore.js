var btnDraft = document.querySelectorAll("button.draft");
var btnEdit = document.querySelectorAll("button.edit");
var btnDelete = document.querySelectorAll("button.delete");
var btnViewDetail = document.querySelectorAll("button.view");
var targetOTMyData = ForwardModalID + "2";

if (btnDraft != null) {
    btnDraft.forEach(function (button) {
        button.addEventListener("click", function () {
            GoDraftForm(ModalContentBase, button.value)
        });
    });
}

if (btnEdit != null) {
    btnEdit.forEach(function (button) {
        button.addEventListener("click", function () {
            GoEditForm(ModalContentBase, button.value)
        });
    });
}

if (btnDelete != null) {
    btnDelete.forEach(function (button) {
        button.addEventListener("click", function () {
            Swal.fire({
                icon: "warning",
                title: button.value,
                text: "คุณต้องการลบรายการนี้ใช่หรือไม่",
                confirmButtonText: "ยืนยัน",
                cancelButtonText: "ยกเลิก",
                confirmButtonColor: "#bbbbbb",
                cancelButtonColor: "#ffaa88",
                focusCancel: true,
                showCloseButton: true,
                showCancelButton: true,
            }).then(result => {
                if (result.isConfirmed) {
                    GoDeleteReq(button.value)
                }
                else {return false;}
                }); 
        });
    });
}

if (btnViewDetail != null) {
    btnViewDetail.forEach(function (button) {
        button.addEventListener("click", function () {
            GoViewOTDetail(ModalContentBase, button.value);
        })
    });
}

function GoDeleteReq(value) {
    displayLoading();
    var url = "New/DeleteReq?req=";
    var param = new URLSearchParams(value);
    fetch(url + param, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        return response.text()
        }).then(function (result) {
            result = JSON.parse(result);
            Swal.fire({
                icon: result.icon,
                title: result.title,
                text: result.message,
            });
            hideLoading();
        });
}

function GoEditForm(target, value) {
    displayLoading();
    var url = "New/EditForm?req=";
    var param = new URLSearchParams(value);
    fetch(url + param, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
        }).then(function (html) {
            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            var ToContent = new String();
            var footer = new String();
            var displayContent = document.getElementById(target);
            var displayFooter = document.getElementById(ModalFooterBase);
            var displayHisRoad = document.getElementsByClassName("istep").item(0);

            if (doc.getElementsByTagName("title").item(0) != null) {
                if (doc.getElementsByTagName("title").item(0).textContent == "403") {
                    displayContent.innerHTML = doc.getElementsByTagName("body").item(0).innerHTML;
                    displayFooter.innerHTML = "";
                    hideLoading();
                }
            } else {
                resetStep(ModalContentBase);

                //loop write step on html
                for (let item = 0; item <= doc.querySelectorAll('[id^="' + ForwardModalID + '"]').length - 1; item++) {
                    ToContent += doc.getElementById(ForwardModalID + (item + 1)).outerHTML
                    footer += doc.getElementById("footer" + (item + 1)).outerHTML
                }

                //set selected day
                document.getElementById("DaySelected").innerText = doc.getElementById("OTType").value;

                displayHisRoad.setAttribute("style", NewOTRoadStyle);
                displayContent.innerHTML = ToContent;
                displayFooter.innerHTML = footer;

                //set loop style none
                var lenghtDivContent = document.querySelectorAll('[id^="' + ForwardModalID + '"]').length;
                for (let item = 0; item <= lenghtDivContent - 1; item++) {
                    if (item != lenghtDivContent - 1) {
                        document.getElementById(ForwardModalID + (item + 1)).style.display = "none";
                        document.getElementById("footer" + (item + 1)).style.display = "none";
                    }
                }

                //set historyRoad
                for (let road = 0; road <= document.getElementsByClassName("istep").length - 1; road++) {
                    document.getElementsByClassName("istep").item(road).removeAttribute("style");
                    if (road == document.getElementsByClassName("istep").length - 1) {
                        document.getElementsByClassName("istep").item(road).setAttribute("style", NewOTRoadStyle);
                    }
                }

                //load event
                LoadScript("js\\New\\EventOTType.js", "EventOTType");
                LoadScript("js\\New\\EventOTMyData.js", "EventOTMyData");
                LoadScript("js\\New\\EventOTForm.js", "EventOTForm");
                LoadScript("js\\New\\EventOTAddWorker.js", "EventOTAddWorker");
                LoadScript("js\\New\\EventOTEmailForm.js", "EventOTEmailForm");

                LoadScript("js\\New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");
                LoadScript("js\\New\\OTAddWorker\\EventDelete.js", "EventWorkerDelete");

                hideLoading();
            }
                    
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });
}

function GoDraftForm(target, value) {
    displayLoading();
    var url = "New/DraftForm?req=";
    var param = new URLSearchParams(value);
    fetch(url + param, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (html) {
        resetStep(ModalContentBase);

        var parser = new DOMParser();
        var doc = parser.parseFromString(html, "text/html");
        var ToContent = new String();
        var footer = new String();

        //loop write step on html
        for (let item = 0; item <= doc.querySelectorAll('[id^="' + ForwardModalID + '"]').length - 1; item++) {
            ToContent += doc.getElementById(ForwardModalID + (item + 1)).outerHTML
            footer += doc.getElementById("footer" + (item + 1)).outerHTML
        }

        //set selected day
        document.getElementById("DaySelected").innerText = doc.getElementById("OTType").value;

        var displayContent = document.getElementById(target);
        var displayFooter = document.getElementById(ModalFooterBase);
        var displayHisRoad = document.getElementsByClassName("istep").item(0);

        displayHisRoad.setAttribute("style", NewOTRoadStyle);
        displayContent.innerHTML = ToContent;
        displayFooter.innerHTML = footer;

        //set loop style none
        var lenghtDivContent = document.querySelectorAll('[id^="' + ForwardModalID + '"]').length;
        for (let item = 0; item <= lenghtDivContent - 1; item++) {
            if (item != lenghtDivContent - 1) {
                document.getElementById(ForwardModalID + (item + 1)).style.display = "none";
                document.getElementById("footer" + (item + 1)).style.display = "none";
            }
        }

        //set historyRoad
        for (let road = 0; road <= document.getElementsByClassName("istep").length - 1; road++) {
            document.getElementsByClassName("istep").item(road).removeAttribute("style");
            if (road == document.getElementsByClassName("istep").length - 1) {
                document.getElementsByClassName("istep").item(road).setAttribute("style", NewOTRoadStyle);
            }
        }

        //load event
        LoadScript("js\\New\\EventOTType.js", "EventOTType");
        LoadScript("js\\New\\EventOTMyData.js", "EventOTMyData");
        LoadScript("js\\New\\EventOTForm.js", "EventOTForm");
        LoadScript("js\\New\\EventOTAddWorker.js", "EventOTAddWorker");
        LoadScript("js\\New\\EventOTEmailForm.js", "EventOTEmailForm");

        LoadScript("js\\New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");
        LoadScript("js\\New\\OTAddWorker\\EventDelete.js", "EventWorkerDelete");

        hideLoading();
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });
}


function GoViewOTDetail(target, value) {
    displayLoading();
   // var url = "/api/OtherFunctions/Details";
    var url = "Functions/ViewOTDetail?req=";
    var param = new URLSearchParams(value);
    fetch(url + param, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (html) {
        resetStep(ModalContentBase);
        var parser = new DOMParser();
        var doc = parser.parseFromString(html, "text/html");
        var ToContent = new String();
        var footer = new String();
        //doc.getElementsByClassName("lbmail").item(0).innerHTML = "ส่งถึง (To)";
        //loop write step on html
        for (let item = 0; item <= doc.querySelectorAll('[id^="' + ForwardModalID + '"]').length - 1; item++) {
            ToContent += doc.getElementById(ForwardModalID + (item + 1)).outerHTML
            footer += doc.getElementById("footer" + (item + 1)).outerHTML
        }
        
        //set selected day
        document.getElementById("DaySelected").innerText = doc.getElementById("OTType").value;
        

        var displayContent = document.getElementById(target);
        var displayFooter = document.getElementById(ModalFooterBase);
        var displayHisRoad = document.getElementsByClassName("istep").item(0);

        displayHisRoad.setAttribute("style", NewOTRoadStyle);
        displayContent.innerHTML = ToContent;
        displayFooter.innerHTML = footer;

        //set loop style none
        var lenghtDivContent = document.querySelectorAll('[id^="' + ForwardModalID + '"]').length;
        for (let item = 0; item <= lenghtDivContent - 1; item++) {
            if (item != lenghtDivContent - 1) {
                document.getElementById(ForwardModalID + (item + 1)).style.display = "none";
                document.getElementById("footer" + (item + 1)).style.display = "none";
            }
        }

        //set historyRoad
        for (let road = 0; road <= document.getElementsByClassName("istep").length - 1; road++) {
            document.getElementsByClassName("istep").item(road).removeAttribute("style");
            if (road == document.getElementsByClassName("istep").length - 1) {
                document.getElementsByClassName("istep").item(road).setAttribute("style", NewOTRoadStyle);
            }
        }

        //load event
        LoadScript("js\\New\\EventOTType.js", "EventOTMyData");
        LoadScript("js\\New\\EventOTMyData.js", "EventOTMyData");
        LoadScript("js\\New\\EventOTForm.js", "EventOTForm");
        LoadScript("js\\New\\EventOTAddWorker.js", "EventOTAddWorker");
        LoadScript("js\\New\\EventOTEmailForm.js", "EventOTEmailForm");

        LoadScript("js\\New\\OTAddWorker\\EventDelete.js", "EventWorkerDelete");
        LoadScript("js\\New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");

        hideLoading();
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });
}