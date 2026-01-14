const urlDefault = location.href.split("ImportMasterWolf")[0] + "\\ImportMasterWolf";
const loader = document.getElementById("loading");
const loadingProcesser = document.getElementById("loadingProcess");
const ForwardModalID = "OTContent_step";
const ModalContentBase = "modal-new-content";
const ModalFooterBase = "FooterContent";
const NewOTRoadStyle = "color: black;font-family: 'LeelawaD Bold';";
const FooterID = "footer";
const apiSTPoint = "http://10.200.128.20/Mvcpublish/ImportMasterWolf/";


//async function apiPosting(dataModel){
//    let url = "Approval/ApproveSelected";
//    document.getElementById("bCounting").innerText = dataModel.Document.length;
//    return await console.log(dataModel.Document.length);
//    //fetch(url, {
//    //    method: "POST",
//    //    referrerPolicy: "strict-origin-when-cross-origin",
//    //    credentials: "same-origin",
//    //    headers: { 'Content-Type': 'application/json' },
//    //    body: JSON.stringify(dataModel)
//    //}).then(
//    //    function (response) {
//    //        return response.text();
//    //    }).then(function (cmd) {
//    //        //trans text to json
//    //        let fakeIndex = 0;
//    //        cmd = JSON.parse(cmd);
//    //        var itemCounting = setInterval(function () {
//    //            document.getElementById("bCounting").innerText = fakeIndex <= cmd.count ? ++fakeIndex : cmd.count;
//    //            if (fakeIndex >= cmd.count + 1) {
//    //                clearInterval(itemCounting);
//    //                Swal.fire({
//    //                    title: cmd.title,
//    //                    text: cmd.message,
//    //                    icon: cmd.icon
//    //                }).then(function () { GoSideMenu("Approval"); });;
//    //            }
//    //        }, 100);
//    //    }).catch(function (err) {
//    //        hideLoading();
//    //        alert('Something went wrong.', err);
//    //        return false;
//    //    });
//}

//async function postUntilEmpty(dataModel) {
//    var recentModel = { ...dataModel };
//    var splitModelPosting = {...dataModel};
//    splitModelPosting.Document = recentModel.Document.slice(0, 10);
//    recentModel.Document = recentModel.Document.slice(10);
//    console.log(splitModelPosting);
//    await apiPosting(splitModelPosting).then(() => document.getElementById("bCounting").innerText = recentModel.Document.length );
//    //console.log(recentModel);
//    //console.log(limitModel);
    

//    if (recentModel.Document.length)
//        postUntilEmpty(recentModel);

//}

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
    let PY = 0;
    let opacity;
    switch (menu) {
        case "Home":
            //LoadScript(window.location.protocol + "\\" + "js\\" + "Home\\Index.js", "Home");
            //LoadScript("js/Home/Hour.js", "EventHomeHour");
            //LoadScript("js\\" + "Home\\Search\\HourControl.js", "HourControl");
            PY = "0px";
            opacity = "opacity-dot-7";
            break;
        case "New":
            LoadScript("js/New/Index.js", "NewItem");
            LoadScript("js/New/EventMore.js", "EventNewMore");
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
            for (var index = 0; index <= json.length -1; index++) {
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
        containerImg.innerHTML = "<img class='wx-100 border-rad' src='"+ imgDataURL +"'>";
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
        html: "กำลังอัพเดทข้อมูล... <p><b id='bCounting'>0</b>" + "of <b>"+ maxCount +"</b></p>",
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
