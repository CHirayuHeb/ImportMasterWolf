const urlDefault = location.href.split("ImportMasterWolf")[0] + "\\ImportMasterWolf";
const loader = document.getElementById("loading");
const loadingProcesser = document.getElementById("loadingProcess");
const ForwardModalID = "OTContent_step";
const ModalContentBase = "modal-new-content";
const ModalFooterBase = "FooterContent";
const NewOTRoadStyle = "color: black;font-family: 'LeelawaD Bold';";
const FooterID = "footer";
const apiSTPoint = "http://10.200.128.20/Mvcpublish/ImportMasterWolf/";



//button tag
var home = document.querySelector("button.home");
var create = document.querySelector("button.new");
var myRequest = document.querySelector("button.my-request");
var approval = document.querySelector("button.approval");
var administrator = document.querySelector("button.administrator");
var signOut = document.querySelector("button.signOut");

//a tag
var ahome = document.querySelector("div.app a.home");
var acreate = document.querySelector("div.app a.create");
var amyRequest = document.querySelector("div.app a.my-request");
var aapproval = document.querySelector("div.app a.approval");
var aadministrator = document.querySelector("div.app a.administrator");
if (home != null)
    home.addEventListener("click", function () {
        GoSideMenu("Home");
    });
if (create != null)
    create.addEventListener("click", function () {
        GoSideMenu("New");
    });
if (myRequest != null)
    myRequest.addEventListener("click", function () {
        GoSideMenu("MyRequest");
    });
if (approval != null)
    approval.addEventListener("click", function () {
        GoSideMenu("Approval");
    });
if (administrator != null)
    administrator.addEventListener("click", function () {
        GoSideMenu("Administrator");
    });
if (signOut != null)
    signOut.addEventListener("click", function () {
        window.location.href = urlDefault + "\\Login\\SignOut\\";
    });

if (ahome != null)
    ahome.addEventListener("click", function () {
        GoSideMenu("Home");
        $("#AppLuncher").modal("hide");
    });
if (acreate != null)
    acreate.addEventListener("click", function () {
        GoSideMenu("New");
        $("#AppLuncher").modal("hide");
    });
if (amyRequest != null)
    amyRequest.addEventListener("click", function () {
        GoSideMenu("MyRequest");
        $("#AppLuncher").modal("hide");
    });
if (aapproval != null)
    aapproval.addEventListener("click", function () {
        GoSideMenu("Approval");
        $("#AppLuncher").modal("hide");
    });
if (aadministrator != null)
    aadministrator.addEventListener("click", function () {
        GoSideMenu("Administrator");
        $("#AppLuncher").modal("hide");
    });

function GoNewRequest(vtype) {
    let url = "New?UpdateType=" + vtype;
    GoSideMenu(url);
}


function GoSideMenu(controller) {
    displayLoading();
    //console.time();
    var url = controller;
    fetch(url, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (html) {
        // Initialize the DOM parser
        var parser = new DOMParser();

        // Parse the text
        var doc = parser.parseFromString(html, "text/html");

        var ToContent = doc.getElementById("DisplayContent").innerHTML;

        //get div Display
        var displayContent = document.getElementById("DisplayContent");

        //pointer side menu
        PositionY(controller);

        //text view controller to html
        displayContent.innerHTML = ToContent;

        //change url
        window.history.replaceState(controller, controller, url);
        hideLoading();
        //console.timeEnd();
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });

}


function PositionY(menu) {
    let idx = menu.indexOf("?");
    if (idx !== -1) {
        menu = menu.substring(0, idx);
    }
    let PY = 0;
    let opacity;
    switch (menu) {
        case "Home":
            //LoadScript(window.location.protocol + "\\" + "js\\" + "Home\\Index.js", "Home");
            //LoadScript("js/Home/Hour.js", "EventHomeHour");
            //LoadScript("js\\" + "Home\\Search\\HourControl.js", "HourControl");
            LoadScript("js/Shared/JsCalendar.js", "Shared");
            PY = "0px";
            opacity = "opacity-dot-7";
            break;
        case "New":
            //LoadScript("js/New/Index.js", "NewItem");
            //LoadScript("js/New/EventMore.js", "EventNewMore");
            LoadScript("js/Shared/divShow.js", "Shared");
            PY = "62px";
            opacity = "opacity-dot-3";
            break;
        case "MyRequest":
            LoadScript("js/MyRequest/Index.js", "MyRequest");
            LoadScript("js/New/EventMore.js", "EventMyRequestMore");
            PY = "124px";
            opacity = "opacity-dot-3";
            break;
        case "Approval":
            LoadScript("js\\Approval\\Index.js", "Approval");
            LoadScript("js\\New\\EventMore.js", "EventApprovalMore");
            PY = "186px";
            opacity = "opacity-dot-3";
            break;
        case "Administrator":
            PY = "248px";
            LoadScript("js\\Admin\\Index.js", "AdminSetting");
            opacity = "opacity-dot-3";
            break;
    }
    var Selector = document.getElementById("selector");
    var bg = document.getElementsByClassName("banner").item(0);
    var oldOpacity = Array.from(bg.classList).find(c => c.startsWith('opacity'));
    bg.classList.replace(oldOpacity, opacity);
    Selector.style.transform = "translate(0px, " + PY + ")";
}

function LoadScript(sourceFile, name) {

    var Time = Date.now();
    var oldScript = document.getElementById(name);
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.src = sourceFile + "?t=" + Time;
    script.type = "text/javascript";
    script.id = name;

    if (oldScript != null) {
        oldScript.parentNode.removeChild(oldScript);
    }
    head.appendChild(script);
    return false;
}

function DisplayResult(url) {
    displayLoading();
    fetch(url, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (html) {
        // Initialize the DOM parser
        var parser = new DOMParser();

        // Parse the text
        var doc = parser.parseFromString(html, "text/html");

        var ToContent = doc.getElementsByClassName("just-group").item(0).outerHTML;

        //get div Display
        var displayContent = document.getElementsByClassName("search-box").item(0);

        //text view controller to html
        displayContent.innerHTML = ToContent;

        ScriptAppendAndReplace(doc.getElementsByTagName("div").item(0).id);
        //LoadScript("js\\" + "New\\EventMore.js", "EventNewMore");
        hideLoading();
        //change url
        //window.history.replaceState(controller, controller, url);
        return new Promise(function (resolve) { $("#RequestControl").collapse("hide"); resolve("resolved"); });
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });
}

function HomeSearch(url) {
    displayLoading();

    //effect from cbOTReqClick() need delay for new dateED changevalue
    let DateST = document.getElementById("dateOTStart");
    let DateED = document.getElementById("dateOTEnd");
    console.log(DateED);
    console.log(DateST);
    let jsonSearch = {};
    if (DateST != null)
        jsonSearch["start"] = DateST.value;
    if (DateED != null)
        jsonSearch["end"] = DateED.value;
    fetch(url, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(jsonSearch),
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (html) {
        // Initialize the DOM parser
        var parser = new DOMParser();
        console.log(html);
        // Parse the text
        var doc = parser.parseFromString(html, "text/html");
        var ToContent = doc.getElementsByClassName("just-group").item(0).outerHTML;

        //get div Display
        var displayContent = document.getElementsByClassName("search-box").item(1);

        //text view controller to html
        displayContent.innerHTML = ToContent;

        //ScriptAppendAndReplace(doc.getElementsByTagName("div").item(0).id);
        //LoadScript("js\\" + "New\\EventMore.js", "EventNewMore");
        hideLoading();
        //change url
        //window.history.replaceState(controller, controller, url);
    })
        .catch(function (err) {
            hideLoading();
            alert('Failed to fetch page: ', err);
        });
}

function ScriptAppendAndReplace(filename) {
    switch (filename) {
        case "Hour":
            LoadScript("js\\Home\\Search\\HourControl.js", "HourControl");
            break;
        case "Follow":
            LoadScript("js\\Home\\Search\\FollowControl.js", "FollowControl");
            break;
        case "Document":
            LoadScript("js\\Home\\Search\\DocumentControl.js", "DocumentControl");
            break;
        case "Graph":
            LoadScript("js\\Home\\Search\\GraphControl.js", "GraphControl");
            break;
        default:
            LoadScript("js\\New\\EventMore.js", "EventNewMore");
            break;
    }
    return;
}

function BtnActiive(ClassName) {
    var position;
    let oldActive;
    switch (ClassName) {
        case "hour": case "mytoday": case "FlowWaiting": case "FlowNewlate":
            position = 0;
            break;
        case "follow": case "myyesterday": case "FlowDone":
            position = 1;
            break;
        case "document": case "alltoday": case "FlowDisapproved":
            position = 2
            break;
        case "graph": case "allyesterday": case "DraftPage":
            position = 3
            break;
    }
    var buttonFilter = document.getElementsByClassName("item");
    for (var buttonAt = 0; buttonAt <= buttonFilter.length - 1; buttonAt++) {
        if (buttonAt == position) {
            oldActive = Array.from(buttonFilter.item(buttonAt).classList).find(c => c.startsWith('bg-'));
            buttonFilter.item(buttonAt).classList.replace(oldActive, "bg-active");
        } else {
            oldActive = Array.from(buttonFilter.item(buttonAt).classList).find(c => c.startsWith('bg-'));
            buttonFilter.item(buttonAt).classList.replace(oldActive, "bg-trans");
        }
    }
}

function resetStep(formID) {
    var form = document.getElementById(formID);
    var HisRoad = document.getElementsByClassName("istep");
    if (form.innerHTML.trim() != "") {
        form.innerHTML = "";
    }
    for (var item = 0; item <= HisRoad.length - 1; item++) {
        if (item == 0) { HisRoad.item(item).setAttribute("style", "display: block"); } else { HisRoad.item(item).setAttribute("style", "display: none"); }

    }
}

function Back(recentStep) {

    //History Link Road
    document.getElementsByClassName("istep").item(recentStep - 1).removeAttribute("style");
    document.getElementsByClassName("istep").item(recentStep - 2).setAttribute("style", NewOTRoadStyle);

    //content
    document.getElementById(ForwardModalID + recentStep).setAttribute("style", "display: none;");
    document.getElementById(ForwardModalID + (parseInt(recentStep) - 1)).removeAttribute("style");

    //footer
    document.getElementById(FooterID + recentStep).setAttribute("style", "display: none;");
    document.getElementById(FooterID + (parseInt(recentStep) - 1)).removeAttribute("style");
}

function createNextstep(nextStep) {
    var stepHasAlready = document.getElementById(ForwardModalID + nextStep);
    if (stepHasAlready == null) {
        let displayContent = document.getElementById(ModalContentBase);
        let displayFooter = document.getElementById(ModalFooterBase);
        let divContent = document.createElement("div");
        let divFooter = document.createElement("div");
        divContent.setAttribute("id", ForwardModalID + nextStep);
        displayContent.append(divContent);
        divFooter.setAttribute("id", FooterID + nextStep);
        displayFooter.append(divFooter);
    }
}

function GoToOTChoice(action, target) {
    var url = action;
    fetch(url, {
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
        var ToContent = doc.getElementById(ForwardModalID + "1").outerHTML;
        var footer = doc.getElementById("footer1").outerHTML;
        var displayContent = document.getElementById(target);
        var displayFooter = document.getElementById(ModalFooterBase);
        var displayHisRoad = document.getElementsByClassName("istep").item(0);

        displayHisRoad.setAttribute("style", NewOTRoadStyle);
        displayContent.innerHTML = ToContent;
        displayFooter.innerHTML = footer;


        //set div step2
        createNextstep(2);
        LoadScript("js\\" + "New\\EventOTType.js", "EventOTType");
    })
        .catch(function (err) {
            alert('Failed to fetch page: ', err);
        });
}

function GoToOTMyData(action, target, value) {
    var url = action;
    var displayHisRoad = document.getElementsByClassName("istep");
    displayHisRoad.item(0).removeAttribute("style");
    displayHisRoad.item(1).setAttribute("style", NewOTRoadStyle);
    document.getElementById("DaySelected").innerText = value;

    //send param to controller

    if (document.getElementById(ForwardModalID + "2").innerHTML.trim() == "") {
        fetch(url, {
            method: "POST",
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
        }).then(function (response) {
            // When the page is loaded convert it to text
            return response.text()
        }).then(function (html) {
            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            doc.getElementById("OTType").value = value;
            var ToContent = doc.getElementById(ForwardModalID + "2").outerHTML;
            var footer = doc.getElementById(FooterID + "2").outerHTML;


            document.getElementById(ForwardModalID + "1").setAttribute("style", "display:none;");
            document.getElementById(FooterID + "1").setAttribute("style", "display:none;");

            var displayContent = document.getElementById(target);
            var displayFooter = document.getElementById(FooterID + "2");


            displayContent.innerHTML = ToContent;
            displayFooter.innerHTML = footer;


            //LoadScript(urlHost + "js\\" + "New\\Index.js", "NewItem");
            LoadScript("js\\New\\EventOTMyData.js", "EventOTMyData");
        });
    } else {
        document.getElementById("OTType").value = value;
        if (document.getElementById(ForwardModalID + "2").style.display === "none") {
            //content
            document.getElementById(ForwardModalID + "2").style.display = "block";
            document.getElementById(ForwardModalID + "1").style.display = "none";
            //footer
            document.getElementById(FooterID + "2").style.display = "block";
            document.getElementById(FooterID + "1").style.display = "none";
        }
    }
}

function GoToNextStep(nextStep, ToAction) {
    var stepHasAlready = document.getElementById(ForwardModalID + nextStep);
    var displayHisRoad = document.getElementsByClassName("istep");
    displayHisRoad.item(nextStep - 2).removeAttribute("style");
    displayHisRoad.item(nextStep - 1).setAttribute("style", NewOTRoadStyle);
    if (stepHasAlready.innerHTML.trim() == "") {
        var url = ToAction;
        var targetContent = ForwardModalID + nextStep;
        var targetFooter = "footer" + nextStep;
        var data = new URLSearchParams();
        fetch(url, {
            method: "POST",
            body: data,
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
        }).then(function (response) {
            // When the page is loaded convert it to text
            return response.text()
        }).then(function (html) {
            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            var ToContent = doc.getElementById(ForwardModalID + nextStep).outerHTML;
            var ToFooter = doc.getElementById(FooterID + nextStep).outerHTML;

            //hide old display
            document.getElementById(ForwardModalID + (parseInt(nextStep) - 1)).setAttribute("style", "display:none;");
            document.getElementById(FooterID + (parseInt(nextStep) - 1)).setAttribute("style", "display:none;");

            var displayContent = document.getElementById(targetContent);
            var displayFooter = document.getElementById(targetFooter);
            displayContent.innerHTML = ToContent;
            displayFooter.innerHTML = ToFooter;

            BringScriptToPage(nextStep);

            return false;
        });
    } else {
        if (document.getElementById(ForwardModalID + nextStep).style.display === "none") {
            //content
            document.getElementById(ForwardModalID + nextStep).style.display = "block";
            document.getElementById(ForwardModalID + (parseInt(nextStep) - 1)).style.display = "none";
            //footer
            document.getElementById(FooterID + nextStep).style.display = "block";
            document.getElementById(FooterID + (parseInt(nextStep) - 1)).style.display = "none";
        }
    }
}

function CheckedMyChildren(checkboxEle) {
    var childrenEle = document.getElementById(checkboxEle.value);
    var checkboxsInChildren = childrenEle.querySelectorAll("input[type=checkbox]");
    checkboxsInChildren.forEach(function (ele) {
        ele.checked = checkboxEle.checked;
    });
}

function cbOTReqClick() {
    let cbOTReq = document.getElementById("cbOTReq");
    var dateOTStart = document.getElementById("dateOTStart");
    let dateOTEnd = document.getElementById("dateOTEnd");
    dateOTStart.disabled = !cbOTReq.checked;
    dateOTEnd.disabled = !cbOTReq.checked;
    //dateOTStart.addEventListener("change", function () {
    //    dateOTEnd.setAttribute("min", dateOTStart.value);
    //    if (Date.parse(dateOTEnd.value) < Date.parse(dateOTStart.value))
    //        dateOTEnd.value = dateOTStart.value;
    //    dateOTEnd.disabled = false;
    //    HomeSearch("Home\\SearchFollow");
    //});
}

function ddlLineChange() {
    let ddlLine = document.getElementsByClassName("ddlLine").item(0);
    let ddlModel = document.getElementsByClassName("ddlModel").item(0);
    let url = "Functions/ModelsOfProdLine"
    let jsonProdLine = {};
    if (ddlLine != null)
        jsonProdLine["name"] = ddlLine.value;

    fetch(url, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(jsonProdLine),
    }).then(function (response) {
        // When the page is loaded convert it to text
        return response.text()
    }).then(function (json) {
        let str = "";
        json = JSON.parse(json);
        for (var index = 0; index <= json.length - 1; index++) {
            str += "<option value='" + json + "'> " + json + "</option>";
        }
        ddlModel.innerHTML = str;
    });
}

function draftOTDocument() {
    displayLoading();

    let formCrateNew = new FormData(document.getElementById("formCreateNew"));
    var param1 = new URLSearchParams(formCrateNew);
    param1.append("mrOTType", document.getElementById("OTType").value);
    let poiterWorker = document.querySelectorAll(".worker-newot-details");
    poiterWorker.forEach(function (div) {
        param1.append("NewWorkerList", JSON.stringify({
            "drEmpCode": div.getElementsByClassName("empcode").item(0).textContent,
            "drJobCode": div.getElementsByClassName("job").item(0).value
        }));
    });
    let poiterMailCC = document.querySelectorAll("label.cc");
    poiterMailCC.forEach(function (label) {
        param1.append("MailCCs", label.textContent,
        );
    });

    let url = "New\\DraftDocument";
    return fetch(url, {
        method: "POST",
        body: param1,
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(
        function (response) {
            return response.text();
        }).then(function (cmd) {
            hideLoading();

            //trans text to json
            cmd = JSON.parse(cmd);
            if (cmd.icon == "success") {
                if (document.getElementById("mrNoReq"))
                    document.getElementById("mrNoReq").value = cmd.req;
                return new Promise(function (resolve) { resolve("resolved"); });
            }
        }).catch(function (err) {
            hideLoading();
            alert('Something went wrong.', err);
            return false;
        });

}

function updateWorkerJob(Node) {
    let empcode = Node.parentNode.getElementsByClassName("empcode").item(0).innerHTML;
    let req = document.getElementById("mrNoReq").value;
    let jobselected = Node.value;
    let url = "New/UpdateWorkerJob?req=" + req + "&empcode=" + empcode + "&jobselected=" + jobselected;
    fetch(url, {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(function (response) {
        return response.text();
    });
}

function updateWorkerAfterDelete(targetPaste, req) {
    let urlUpdateBasePage = "New\\WorkerList?req=" + req;
    fetch(urlUpdateBasePage).then(function (response) {
        return response.text();
    }).then(function (partialtext) {
        let parser = new DOMParser();
        let categoryhtml = parser.parseFromString(partialtext, "text/html");
        targetPaste.getElementsByClassName("workers-category").item(0).innerHTML = categoryhtml.getElementsByTagName("body").item(0).innerHTML;
    }).catch(function (err) {
        alert('Something went wrong.', err);
        return false;
    });
}

function ToXlsm(ele) {
    let value = ele.value;
    let url = "Functions\\ToXlsm?req=" + value;
    window.open(url, "_blank");
}

function LoadEmpPic(ele) {
    let empcode = ele.getElementsByClassName("empcode").item(0).innerHTML;
    let url = "Functions/LoadEmpPic?empcode=" + empcode;

    fetch(url).then(function (response) {
        return response.text();
    }).then(function (imgDataURL) {
        let containerImg = ele.getElementsByClassName("img").item(0);
        containerImg.innerHTML = "<img class='wx-100 border-rad' src='" + imgDataURL + "'>";
    });
}

async function ExportToXlsm(noInArray) {
    fetch("Functions/ToListXlsm", {
        method: "POST",
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(noInArray),
    }).then(function (response) {
        return response.text()
    }).then(function (xlsm) {
        location.href = "Functions/XlsxFromByte";
    });
}

function notEnter(e) { if (e.keyCode == 13) return false; }

//showing Loading
function displayLoading() {
    loader.style.display = "flex";
    //setTimeout(() => {
    //    loader.style.display = "none";
    //}, 300000);
}

//hiding Loading
function hideLoading() {
    loader.style.display = "none";
}

//showing Loading
function displayLoadingAndShowProcess(maxCount) {
    Swal.fire({
        html: "กำลังอัพเดทข้อมูล... <p><b id='bCounting'>0</b>" + "of <b>" + maxCount + "</b></p>",
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        timerProgressBar: true,
    })
}

function displayExportingAndShowProcess() {
    Swal.fire({
        html: "กำลังสร้างไฟล์ Excel...",
        allowEscapeKey: false,
        allowOutsideClick: false,
        showConfirmButton: false,
        timerProgressBar: true,
    })
}

//hiding Loading
function hideLoadingAndShowProcess() {
    Swal.close();
    //loadingProcesser.style.display = "none";
}


function Menubar_SaveData1(action) {
    // let formDatamt = document.forms.namedItem("formNewimport");
    let formDatamt = new FormData(document.forms.namedItem("formNewimport"));
    fetch(action, {
        method: 'POST',
        body: formDatamt
    })
        .then(res => res.json())
        .then(data => {
            console.log(data);
        });



    //let empList = [];

    //document.querySelectorAll('#tbodyMSTEmployee tr').forEach(tr => {
    //    let id = tr.querySelector('.emp-id') ?.value;
    //    let code = tr.querySelector('.emp-code') ?.value;

    //    // ส่งเฉพาะแถวที่มีข้อมูล เพื่อลดขนาด Payload
    //    if (id || code) {
    //        empList.push({
    //            EmployeeId: id,
    //            EmployeeCode: code
    //        });
    //    }
    //});

    //let model = {
    //    _ListViewMSTEmployee: empList
    //};

    //// ตรวจสอบขนาดของ JSON ก่อนส่ง
    //console.log("Data size:", JSON.stringify(model).length);

    //fetch('/New/SaveData', {
    //    method: 'POST',
    //    headers: {
    //        'Content-Type': 'application/json',
    //        'Accept': 'application/json'
    //    },
    //    body: JSON.stringify(model)
    //})
    //    .then(r => {
    //        if (!r.ok) throw new Error('HTTP error ' + r.status);
    //        return r.json();
    //    })
    //    .then(d => console.log(d))
    //    .catch(err => console.error("Error:", err));

    //const form = document.getElementById('formNewimport');

    //form.addEventListener('submit', function (e) {
    //    e.preventDefault(); // ไม่ให้หน้า reload
    //    const formData = new FormData(form);

    //    fetch('/New/SaveData', {
    //        method: 'POST',
    //        body: formData
    //    })
    //        .then(res => res.text())
    //        .then(data => console.log(data))
    //        .catch(err => console.error(err));
    //});

}

function Menubar_SaveData(action, tsave) {

    // var rows = tbody.getElementsByTagName("tr");
    let viewModel1 = new FormData();
    //let vtype = document.getElementById("lType");
    //var value1 = document.getElementById("lType").textContent;
    var vtype = $("#lType").text();

    if (vtype == "" || vtype == null) {
        swal.fire({
            title: 'No Data Found !!',
            icon: 'info',
            text: "No changes were detected.(ไม่พบข้อมูล)",
        }).then((result) => {
            if (result.isConfirmed) {
                return false;
            }
        });
    }
    else {
        Swal.fire({
            title: "Are you sure?",
            text: "Are you sure " + tsave + " Data Wolf ?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes",
            cancelButtonText: "No"
        }).then((result) => {
            if (result['isConfirmed']) {
                if (vtype == "Div") {

                    _ListViewMSTDivision = [];
                    _ListViewMSTDepartment = [];
                    _ListViewMSTPosition = [];
                    var tbody = document.getElementById("tbodyMSTDivision");
                    const _RowMSTDivision = tbody.querySelectorAll('tr#trMSTDivision');
                    _RowMSTDivision.forEach((tr, index) => {
                        _ListViewMSTDivision.push({
                            DivisionId: tr.querySelector(".txtDivisionId").value,
                            NameTh: tr.querySelector(".txtNameTh").value,
                            NameEn: tr.querySelector(".txtNameEn").value,
                            CreatedDate: "",//tr.querySelector(".txtCreatedDate").value,
                            CreatedBy: tr.querySelector(".txtCreatedBy").value,
                            ModifiedDate: "",//tr.querySelector(".txtModifiedDate").value,
                            ModifiedBy: tr.querySelector(".txtModifiedBy").value,
                            IsActive: tr.querySelector(".txtIsActive").value,
                            AccountId: tr.querySelector(".txtAccountId").value,
                            DivisionCode: tr.querySelector(".txtDivisionCode").value,
                        });
                    });



                    var tbody1 = document.getElementById("tbodyMSTDepartment");
                    const _RowMSTDepartment = tbody1.querySelectorAll('tr#trMSTDepartment');
                    _RowMSTDepartment.forEach((tr, index) => {
                        _ListViewMSTDepartment.push({
                            DepartmentId: tr.querySelector(".txtDepartmentId").value,
                            ParentId: tr.querySelector(".txtParentId").value,
                            DivisionId: tr.querySelector(".txtDivisionId").value,
                            DepartmentCode: tr.querySelector(".txtDepartmentCode").value,
                            NameTh: tr.querySelector(".txtNameTh").value,
                            NameEn: tr.querySelector(".txtNameEn").value,
                            CreatedDate: "",//tr.querySelector(".txtDivisionId").value,
                            CreatedBy: tr.querySelector(".txtCreatedBy").value,
                            ModifiedDate: "",// tr.querySelector(".txtModifiedDate").value,
                            ModifiedBy: tr.querySelector(".txtModifiedBy").value,
                            IsActive: tr.querySelector(".txtIsActive").value,
                            AccountId: tr.querySelector(".txtAccountId").value,
                            LeaderId: tr.querySelector(".txtLeaderId").value,
                            CompanyCode: tr.querySelector(".txtCompanyCode").value,
                        });
                    });



                    var tbody2 = document.getElementById("tbodyMSTPosition");
                    const _RowMSTPosition = tbody2.querySelectorAll('tr#trMSTPosition');
                    _RowMSTPosition.forEach((tr, index) => {
                        _ListViewMSTPosition.push({

                            PositionId: tr.querySelector(".txtPositionId").value,
                            NameTh: tr.querySelector(".txtNameTh").value,
                            NameEn: tr.querySelector(".txtNameEn").value,
                            PositionLevelId: tr.querySelector(".txtPositionLevelId").value,
                            IsActive: tr.querySelector(".txtIsActive").value,
                            CreatedDate: "",//tr.querySelector(".txtDepartmentId").value,
                            CreatedBy: tr.querySelector(".txtCreatedBy").value,
                            ModifiedDate: "",// tr.querySelector(".txtDepartmentId").value,
                            ModifiedBy: tr.querySelector(".txtModifiedBy").value,
                            AccountId: tr.querySelector(".txtAccountId").value,
                            CompanyCode: tr.querySelector(".txtCompanyCode").value,

                        });
                    });
                    viewModel1.append("_ListViewMSTDivision", JSON.stringify(_ListViewMSTDivision));
                    viewModel1.append("_ListViewMSTDepartment", JSON.stringify(_ListViewMSTDepartment));
                    viewModel1.append("_ListViewMSTPosition", JSON.stringify(_ListViewMSTPosition));

                }
                //else {
                else if (vtype == "Accemployee") {
                    _ListViewMSTATACCEmployee = [];
                    _ListViewMSTATEmployee = [];
                    _ListViewMSTEmployee = [];
                    _ListViewWOLFAccount = [];

                    var tbody = document.getElementById("tbodyMSTATACCEmployee");
                    const _RowMSTATACCEmployee = tbody.querySelectorAll('tr#trMSTATACCEmployee');
                    _RowMSTATACCEmployee.forEach((tr, index) => {
                        _ListViewMSTATACCEmployee.push({
                            EMPID: tr.querySelector(".txtEMPID").value,
                            EMPCODE: tr.querySelector(".txtEMPCODE").value,
                            Name: tr.querySelector(".txtName").value,
                            NameTH: tr.querySelector(".txtNameTH").value,
                            JOB_NAME: tr.querySelector(".txtJOB_NAME").value,
                            PositionName: tr.querySelector(".txtPositionName").value,
                            DivisionName: tr.querySelector(".txtDivisionName").value,
                            DepartmentName: tr.querySelector(".txtDepartmentName").value,
                            SECName: tr.querySelector(".txtSECName").value,
                            GRPName: tr.querySelector(".txtGRPName").value,
                            UNTName: tr.querySelector(".txtUNTName").value,
                            DIRECT_INDIRECT_CODE: tr.querySelector(".txtDIRECT_INDIRECT_CODE").value,
                            INTERCOMNO: tr.querySelector(".txtINTERCOMNO").value,
                            NICKNAME: tr.querySelector(".txtNICKNAME").value,

                        });

                    });
                    var tbody1 = document.getElementById("tbodyMSTATEmployee");
                    const _RowMSTATEmployee = tbody1.querySelectorAll('tr#trMSTATEmployee');
                    _RowMSTATEmployee.forEach((tr, index) => {
                        _ListViewMSTATEmployee.push({
                            EMPID: tr.querySelector(".txtEMPID").value,
                            EMPCODE: tr.querySelector(".txtEMPCODE").value,
                            NICKNAME: tr.querySelector(".txtNICKNAME").value,
                            INTERCOMNO: tr.querySelector(".txtINTERCOMNO").value,
                            JOBCODE: tr.querySelector(".txtJOBCODE").value,
                            SECNAME: tr.querySelector(".txtSECNAME").value,
                            GRPNAME: tr.querySelector(".txtGRPNAME").value,
                            UNTNAME: tr.querySelector(".txtUNTNAME").value,
                        });

                    });

                    //tbodyMSTEmployee
                    //trMSTEmployee
                    var tbody2 = document.getElementById("tbodyMSTEmployee");
                    const _RowMSTEmployee = tbody2.querySelectorAll('tr#trMSTEmployee');
                    _RowMSTEmployee.forEach((tr, index) => {
                        _ListViewMSTEmployee.push({
                            EmployeeId: tr.querySelector(".txtEmployeeId").value,
                            EmployeeCode: tr.querySelector(".txtEmployeeCode").value,
                            Username: tr.querySelector(".txtUsername").value,
                            NameTh: tr.querySelector(".txtNameTh").value,
                            NameEn: tr.querySelector(".txtNameEn").value,
                            Email: tr.querySelector(".txtEmail").value,
                            IsActive: tr.querySelector(".txtIsActive").value, //bool
                            PositionId: tr.querySelector(".txtPositionId").value,
                            DepartmentId: tr.querySelector(".txtDepartmentId").value,
                            ReportToEmpCode: tr.querySelector(".txtReportToEmpCode").value,
                            SignPicPath: tr.querySelector(".txtSignPicPath").value,
                            Lang: tr.querySelector(".txtLang").value,
                            AccountId: tr.querySelector(".txtAccountId").value,
                            CreatedDate: "",// tr.querySelector(".txtCreatedDate").value,//date
                            CreatedBy: tr.querySelector(".txtCreatedBy").value,
                            ModifiedDate: "",//tr.querySelector(".txtModifiedDate").value, //date
                            ModifiedB: tr.querySelector(".txtModifiedBy").value,
                            ADTitle: tr.querySelector(".txtADTitle").value,
                            DivisionId: tr.querySelector(".txtDivisionId").value,
                            EmpLevel: tr.querySelector(".txtEmpLevel").value,
                            EMPL_RCD: tr.querySelector(".txtEMPL_RCD").value,
                            EmployeeLevel: tr.querySelector(".txtEmployeeLevel").value,
                            EffectiveDate: "",//tr.querySelector(".txtEffectiveDate").value, //date
                            Userid_Line: tr.querySelector(".txtUserid_Line").value,
                        });

                    });

                    var tbody3 = document.getElementById("tbodyWOLFAccount");
                    const _RowWOLFAccount = tbody3.querySelectorAll('tr#trWOLFAccount');
                    _RowWOLFAccount.forEach((tr, index) => {
                        _ListViewWOLFAccount.push({
                            ID: tr.querySelector(".txtID").value,
                            ContactCode: tr.querySelector(".txtContactCode").value,
                            Username: tr.querySelector(".txtUsername").value,
                            Password: tr.querySelector(".txtPassword").value,
                            IsVerify: tr.querySelector(".txtIsVerify").value,
                            GuidVerify: tr.querySelector(".txtGuidVerify").value,
                            Note: tr.querySelector(".txtNote").value,
                            Remark: tr.querySelector(".txtRemark").value,
                            Description: tr.querySelector(".txtDescription").value,
                            CreatedDate: "",//tr.querySelector(".txtCreatedDate").value,
                            CreatedBy: tr.querySelector(".txtCreatedBy").value,
                            ModifiedDate: "",//tr.querySelector(".txtModifiedDate").value,
                            ModifiedBy: tr.querySelector(".txtModifiedBy").value,
                            IsActive: tr.querySelector(".txtIsActive").value,
                        });

                    });


                    //console.log(_ListviewMSTATEmployees);
                    viewModel1.append("_ListViewMSTATACCEmployee", JSON.stringify(_ListViewMSTATACCEmployee));
                    viewModel1.append("_ListViewMSTATEmployee", JSON.stringify(_ListViewMSTATEmployee));
                    viewModel1.append("_ListViewMSTEmployee", JSON.stringify(_ListViewMSTEmployee));
                    viewModel1.append("_ListViewWOLFAccount", JSON.stringify(_ListViewWOLFAccount));
                }
                viewModel1.append("vType", vtype);
                viewModel1.append("tsave", tsave);
                $.ajax({
                    type: "POST",
                    url: action,
                    data: viewModel1,
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        swal.fire({
                            html: '<h5>Loading...</h5>',
                            showConfirmButton: false,
                            onRender: function () {
                                // there will only ever be one sweet alert open.
                                //$('.swal2-content').prepend(sweet_loader);
                            }
                        });
                    },
                    success: async function (config) {
                        //console.log("config.c1" + config.c1);
                        //alert(config.c1);
                        if (config.c1 == "S") {
                            swal.fire({
                                title: 'Saved!!',
                                icon: 'success',
                                text: config.c2,
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    GoSideMenu("Home");
                                }
                            });
                        }
                        else if (config.c1 == "E") {
                            {
                                swal.fire({
                                    title: 'Saved!',
                                    icon: 'error',
                                    text: config.c2,
                                }).then((result) => {
                                    if (result.isConfirmed) {
                                        // GoSideMenu("Home");
                                    }
                                });
                            }
                        }
                    }
                });

            } else {
                //console.log('Cancel');
                return false;
            }
        });
    }






}

function Menubar_SaveData2(action) {
    // --- เรียกใช้งานทีละ tbody ---
    //sendTbodyInBatches('tbodyMSTATACCEmployee', '/New/Upload')
    //    .then(function () {
    //        return sendTbodyInBatches('tbodyMSTEmployee', '/New/Upload');
    //    })
    //    .then(function () {
    //        return sendTbodyInBatches('tbodyMSTATEmployee', '/New/Upload');
    //    })
    //    .catch(function (err) {
    //        console.error('Error sending batches:', err);
    //    });



    sendTbodyInBatchesByRow('tbodyMSTATACCEmployee', '/New/Upload');
    sendTbodyInBatchesByRow('tbodyMSTEmployee', '/New/Upload');
    sendTbodyInBatchesByRow('tbodyMSTATEmployee', '/New/Upload');
}

function sendTbodyInBatches(tbodyId, actionUrl, maxFieldsPerRequest) {
    maxFieldsPerRequest = maxFieldsPerRequest || 20000;
    var tbody = document.getElementById(tbodyId);
    var rows = Array.prototype.slice.call(tbody.querySelectorAll('tr'));

    var batch = [];
    var batchFieldCount = 0;

    function sendBatch(batchRows) {
        return new Promise(function (resolve, reject) {
            var formData = new FormData();
            batchRows.forEach(function (row, rowIndex) {
                var inputs = Array.prototype.slice.call(row.querySelectorAll('input, select, textarea'));
                inputs.forEach(function (input) {
                    var name = input.name || 'row[' + rowIndex + '][' + input.type + ']';
                    if (input.type === 'file' && input.files.length > 0) {
                        for (var f = 0; f < input.files.length; f++) {
                            formData.append(name + '[' + f + ']', input.files[f]);
                        }
                    } else {
                        formData.append(name, input.value);
                    }
                });
            });

            fetch(actionUrl, { method: 'POST', body: formData })
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    console.log('Batch sent:', data);
                    resolve();
                })
                .catch(function (err) {
                    console.error(err);
                    reject(err);
                });
        });
    }

    // ส่ง batch ต่อ batch โดย Promise chain
    var chain = Promise.resolve();

    rows.forEach(function (row) {
        var inputs = Array.prototype.slice.call(row.querySelectorAll('input, select, textarea'));
        var fieldCount = inputs.length;

        if (batchFieldCount + fieldCount > maxFieldsPerRequest && batch.length > 0) {
            chain = chain.then(function () { return sendBatch(batch); });
            batch = [];
            batchFieldCount = 0;
        }

        batch.push(row);
        batchFieldCount += fieldCount;
    });

    // ส่ง batch สุดท้าย
    if (batch.length > 0) {
        chain = chain.then(function () { return sendBatch(batch); });
    }

    return chain;
}


function sendTbodyInBatchesByRow(tbodyId, actionUrl, batchSize = 1000) {
    var tbody = document.getElementById(tbodyId);
    var rows = Array.prototype.slice.call(tbody.querySelectorAll('tr'));
    var formData = new FormData();
    formData.append("tbodyId", tbody);

    // แบ่ง rows เป็น batch
    for (var i = 0; i < rows.length; i += batchSize) {
        (function (batchRows) {

            batchRows.forEach(function (row, rowIndex) {
                var inputs = Array.prototype.slice.call(row.querySelectorAll('input, select, textarea'));
                inputs.forEach(function (input) {
                    var name = input.name || 'row[' + rowIndex + '][' + input.type + ']';
                    if (input.type === 'file' && input.files.length > 0) {
                        for (var f = 0; f < input.files.length; f++) {
                            formData.append(name + '[' + f + ']', input.files[f]);
                        }
                    } else {
                        formData.append(name, input.value);
                    }
                });
            });

            fetch(actionUrl, {
                method: 'POST',
                body: formData
            })
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    console.log('Batch sent:', data);
                })
                .catch(function (err) {
                    console.error('Error sending batch:', err);
                });
        })(rows.slice(i, i + batchSize)); // ส่ง batch นี้
    }
}


function _MenubarDetailMSTMaster(Mid, actionUrl) {

    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: {
            MasterId: Mid
        },
        beforeSend: function () {
            console.log("Showing loader..."); // ตรวจสอบว่าทำงานจริง

        },
        success: function (response) {
            //$("#myModalDetailData").html(response); // แสดง Loader
            $("#myModalBodyDetailData").html(response);
            $("#myModalDetailData").modal("show");
        }
    });




}

function _MenubarDeleteMSTMaster(Mid, actionUrl) {
    let vsearch = document.getElementById("searchInputTMasterData").value;
    Swal.fire({
        title: "Are you sure?",
        text: "Are you sure Delete : Master ID : " + Mid + " ?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes",
        cancelButtonText: "No"
    }).then((result) => {
        if (result['isConfirmed']) {
            $.ajax({
                type: 'post',
                url: actionUrl,
                data: { MasterId: Mid },
                success: function (res) {
                    swal.fire({
                        title: 'แจ้งเตือน',
                        icon: res.res,
                        text: res.res,
                    })
                        .then((result) => {
                            GoNewRequest('OpenBlock&_vSblock=' + vsearch + '');
                        });



                }
            });
        } else {

            return false;
        }

    });
}

function _MenubarSaveMSTMaster(btn, actionUrl) {
    let vsearch = document.getElementById("searchInputTMasterData").value;
    var $form = $(btn).closest('form');
    var formData = new FormData($form[0]);
    if ($form.length > 0) {
        console.log("เจอ Form แล้ว!", $form.serialize());
        // สั่ง AJAX POST ข้อมูลใน $form ต่อได้เลย
        var value1 = document.getElementById("MTDValue1").value;
        var value2 = document.getElementById("MTDValue2").value;
        var value3 = document.getElementById("MTDValue3").value;
        if (value1 == "" || value2 == "" || value3 == "") {
            //Swal.fire({
            //    icon: 'แจ้งเตือน',
            //    title: 'warning',
            //    text: "กรุณากรอกข้อมูลให้ครบถ้วน !!!!",
            //})
            //    .then((result) => {
            //        return false;
            //    });

            swal.fire({
                title: 'Please Input Data',
                icon: 'info',
                text: "กรุณากรอกข้อมูลให้ครบถ้วน !!!!",
            }).then((result) => {
                if (result.isConfirmed) {
                    return false;
                }
            });

        } else {
            $.ajax({
                type: "POST",
                url: actionUrl,
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    swal.fire({
                        html: '<h5>Loading...</h5>',
                        showConfirmButton: false,
                        onRender: function () {
                            // there will only ever be one sweet alert open.
                            //$('.swal2-content').prepend(sweet_loader);
                        }
                    });
                },
                success: async function (config) {
                    // alert(config.c1);
                    if (config.c1 == "S") {
                        // $("#loaderDiv").hide();
                        await $("#myModalDetailData").modal("hide");
                        swal.fire({
                            title: 'SUCCESS',
                            icon: 'success',
                            text: config.c2,
                        }).then((result) => {
                            //GoSideMenu("AddProcess");
                            //GoNewRequest('OpenBlock');
                            GoNewRequest('OpenBlock&_vSblock=' + vsearch + '');
                        });
                    }
                    else if (config.c1 == "E") {

                        Swal.fire({
                            icon: 'error',
                            title: 'ERROR',
                            text: config.c2,
                        })
                            .then((result) => {
                                $("#myModal5").modal("show");
                            });

                    }
                    else if (config.c1 == "N") {

                        Swal.fire({
                            icon: 'แจ้งเตือน',
                            title: 'warning',
                            text: config.c2,
                        })
                            .then((result) => {
                                $("#myModal5").modal("show");
                            });

                    }
                }
            });

        }

    } else {
        alert("หา Form ไม่เจอ! เช็กว่ามี <form> คลุมปุ่มหรือยัง");
    }




}
function Menubar_uploadFileMSTMaster(action) {
    //var getID = document.getElementById("i_NewOtherWK_DocumentNo").value; //txtMIssueID
    const form1 = document.forms.namedItem("formModelMSTData");
    let viewModel1 = new FormData(form1);


    $.ajax({
        type: 'post',
        url: action,
        data: viewModel1,
        processData: false,
        contentType: false,
        success: async function (config) {
            await $("#myModalMSTDataUpload").modal("hide");
            // alert(config.c1);
            if (config.c1 == "S") {
                swal.fire({
                    title: 'SUCCESS',
                    icon: 'success',
                    text: config.c2,
                }).then((result) => {
                    if (result.isConfirmed) {
                        //console.log("getID==> " + getID);
                        //GoNewMoldOtherWKRequest(getID, "");
                        GoNewRequest('OpenBlock');

                    }
                });
            }
            else if (config.c1 == "E") {

                Swal.fire({
                    icon: 'error',
                    title: 'ERROR',
                    text: config.c2,
                })
                    .then((result) => {
                        $('#filesImport').val('');
                        $("#myModalMSTDataUpload").modal("show");
                    });

            }
            else if (config.c1 == "P") {

                Swal.fire({
                    icon: 'warning',
                    title: 'warning',
                    text: config.c2,
                })
                    .then((result) => {
                        if (result.isConfirmed) {
                            $('#filesImport').val('');
                            $("#myModalMSTDataUpload").modal("show");
                        }

                    });

            }

        }
    });



}

