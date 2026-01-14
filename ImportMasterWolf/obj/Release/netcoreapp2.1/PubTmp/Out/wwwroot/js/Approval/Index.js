//get button
var btnNewlate = document.getElementsByClassName("filter").item(0);
var Documents = document.querySelectorAll("div.docno");
var btnHRApprove = document.querySelectorAll("button.btnHRApprove");
var btnCheckAll = document.getElementById("CheckAllItem");
var btnApproveSeleted = document.getElementsByClassName("approveseleted").item(0);
var btnAcceptSeleted = document.getElementsByClassName("acceptseleted").item(0);
//path controller//action
var toFlowNewlate = "Approval\\DisplayNewlate";

var chckSec = document.querySelectorAll("input.chck-section");

if (chckSec != null)
    chckSec.forEach(function (checkboxSection, index) {
        let collapseSectionID = "collapse" + checkboxSection.value;
        let collapseSectionTarget = document.getElementById(collapseSectionID);
        if (collapseSectionTarget != null) {
            let chckDoc = collapseSectionTarget.querySelectorAll("input.chck-doc");
            checkboxSection.addEventListener("click", function () {
                if (chckDoc != null)
                    chckDoc.forEach(function (checkboxDoc) {
                        checkboxDoc.checked = checkboxSection.checked;
                    });
            });
        }
    });

if (btnApproveSeleted != null)
    btnApproveSeleted.addEventListener("click", function () {
        displayLoading();
        //let jsonReqDoc = {};
        //jsonReqDoc["no"] = reqDoc;
        fetch("Approval/ptvHeadEmailForm", {
            method: "POST",
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
            headers: { 'Content-Type': 'application/json' },
        }).then(function (response) {
            // When the page is loaded convert it to text
            return response.text()
            }).then(async function (html) {
                document.getElementById("modal-new-content").innerHTML = "";
                document.getElementById("roadApprov").innerText = "ส่งถึง";
                var parser = new DOMParser();
                var doc = parser.parseFromString(html, "text/html");
                var displayContent = document.getElementById("modal-email-content");
                var displayFooter = document.getElementById("FooterEmailContent");

                if (doc.getElementsByTagName("title").item(0) != null) {
                    if (doc.getElementsByTagName("title").item(0).textContent == "403") {
                        displayContent.innerHTML = doc.getElementsByTagName("body").item(0).innerHTML;
                        displayFooter.innerHTML = "";
                        hideLoading();
                    }
                } else {
                    var ToContent = doc.getElementById("OTContent_mail").outerHTML;
                    var ToFooter = doc.getElementById("footermail").outerHTML;

                    displayContent.innerHTML = ToContent;
                    displayFooter.innerHTML = ToFooter;
                    //SaveAndSendMail(document, div, "Approval/Approved");

                    //document is base page, 
                    //doc is modal appended (MailTo, List<MailCC>, Remark)
                    let btnSendSelected = document.getElementsByClassName("btnSendSelected").item(0);
                    let docList = {};
                    let arDocList = new Array();
                    let cc = new Array();

                    //start click send on modal email  
                    btnSendSelected.addEventListener("click",async function () {
                        //set empty value in array (Model: ApproveSelected in MultiApproveSelected)
                        arDocList = new Array();
                        //get input check position for get parentelement -> result <input> type="check" value="OTID(1 to n)"</input>
                        let chckDoc = document.querySelectorAll("input.chck-doc");
                        chckDoc.forEach(function (cbcheck, index) {
                            let model = new Array();
                            if (cbcheck.checked) {
                                //get parent of input type="check" -> result get html in <div class"docno"></div>
                                let div = cbcheck.parentElement.parentElement;
                                //get OT ID
                                let req = cbcheck.value;
                                var workerList = {};
                                let chckApprove = div.querySelectorAll(".chck-app");
                                if (chckApprove != null)
                                    chckApprove.forEach(function (checkbox) {
                                        if (checkbox.checked)
                                            model.push(checkbox.value);
                                    });

                                //** important  ->  "reqNo, workerList" need "same field name" in model at our seted
                                workerList["reqNo"] = req;
                                workerList["workerList"] = model;
                                arDocList.push(workerList);
                            }
                        });
                        displayLoadingAndShowProcess(arDocList.length);

                        //start set value from doc (Modal appended)
                        let txtMailID = document.getElementById("txtMail");
                        let emailcc = document.querySelectorAll("label.cc");
                        let txtRemarkID = document.getElementById("historyApproveds_htRemark");

                        //loop mail cc form text in label
                        if (emailcc != null)
                            emailcc.forEach(function (label) {
                                if (label != null) {
                                    cc.push(label.textContent);
                                }
                            });
                        //end loop mail cc
                        //end set value

                        docList["Document"] = arDocList;
                        //console.log(JSON.stringify(docList));
                        docList["empcodeMailTo"] = txtMailID == null ? "" : txtMailID.value;;
                        docList["cc"] = cc;
                        docList["remark"] = txtRemarkID == null ? "" : txtRemarkID.value;

                        let url = "Approval/ApproveSelected";
                        //console.log(parseInt(docList.Document.length / 100));
                        //recursive call
                        //await postUntilEmpty(docList);
                        //for await (var index of asyncDoneCouting()) {
                        //    await setTimeout(function () { document.getElementById("bCounting").innerText = index; }, 2000);
                        //}
                        
                        fetch(url, {
                            method: "POST",
                            referrerPolicy: "strict-origin-when-cross-origin",
                            credentials: "same-origin",
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify(docList)
                        }).then(
                            function (response) {
                                return response.text();
                            }).then(function (cmd) {
                                //trans text to json
                                let fakeIndex = 0;
                                cmd = JSON.parse(cmd);
                                var itemCounting = setInterval(function () {
                                    document.getElementById("bCounting").innerText = fakeIndex <= cmd.count ? ++fakeIndex : cmd.count;
                                    if (fakeIndex >= cmd.count + 1) {
                                        clearInterval(itemCounting);
                                        hideLoadingAndShowProcess();
                                        Swal.fire({
                                            title: cmd.title,
                                            text: cmd.message,
                                            icon: cmd.icon
                                        }).then(function () { GoSideMenu("Approval"); });;
                                    }
                                }, 100);
                            }).catch(function (err) {
                                hideLoading();
                                hideLoadingAndShowProcess();
                                displayEmailApprover(document);
                                alert('Something went wrong.', err);
                                return false;
                            });
                    });

                    LoadScript("js\\Approval\\SendEmail\\EventEmailForm.js", "EventEmailApproverForm");
                    displayEmailApprover(document);

                    hideLoading();
                    hideLoadingAndShowProcess();
                }
        });
    });

if (btnAcceptSeleted != null)
    btnAcceptSeleted.addEventListener("click", function () {
        displayLoading();
        fetch("Approval/ptvExportHelper", {
            method: "POST",
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
            headers: { 'Content-Type': 'application/json' },
        }).then(function (response) {
            // When the page is loaded convert it to text
            return response.text()
        }).then(async function (html) {
            document.getElementById("modal-new-content").innerHTML = "";
            document.getElementById("roadApprov").innerText = "ช่วยเหลือ";

            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            var displayContent = document.getElementById("modal-email-content");
            var displayFooter = document.getElementById("FooterEmailContent");

            if (doc.getElementsByTagName("title").item(0) != null) {
                if (doc.getElementsByTagName("title").item(0).textContent == "403") {
                    displayContent.innerHTML = doc.getElementsByTagName("body").item(0).innerHTML;
                    displayFooter.innerHTML = "";
                    hideLoading();
                }
            } else {
                var ToContent = doc.getElementById("OTContent_helper").outerHTML;
                var ToFooter = doc.getElementById("footermail").outerHTML;

                displayContent.innerHTML = ToContent;
                displayFooter.innerHTML = ToFooter;
                //SaveAndSendMail(document, div, "Approval/Approved");

                //document is base page, 
                //doc is modal appended (MailTo, List<MailCC>, Remark)
                let btnSendSelected = document.getElementsByClassName("btnSendSelected").item(0);

                //start click send on modal email  
                btnSendSelected.addEventListener("click", async function () {
                    //get input check position for get parentelement -> result <input> type="check" value="OTID(1 to n)"</input>
                    let reqs = new Array();
                    let chckDoc = document.querySelectorAll("input.chck-doc");
                    let chckExport = document.getElementById("chckExport");
                    chckDoc.forEach(function (cbcheck, index) {
                        if (cbcheck.checked)
                            reqs.push({ "no" : cbcheck.value });
                    });
                    displayLoadingAndShowProcess(reqs.length);
                    let url = "Approval/HRApprovedSelected";
                    fetch(url, {
                        method: "POST",
                        referrerPolicy: "strict-origin-when-cross-origin",
                        credentials: "same-origin",
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(reqs)
                    }).then(function (response) {
                            return response.text();
                        }).then(function (cmd) {
                            //trans text to json
                            let fakeIndex = 0;
                            cmd = JSON.parse(cmd);
                            var itemCounting = setInterval(async function () {
                                document.getElementById("bCounting").innerText = fakeIndex <= cmd.count ? ++fakeIndex : cmd.count;
                                if (fakeIndex >= cmd.count + 1) {
                                    clearInterval(itemCounting);
                                    if (chckExport.checked) {
                                        displayExportingAndShowProcess();
                                        await ExportToXlsm(reqs).then(() =>
                                                Swal.fire({
                                                    title: cmd.title,
                                                    text: cmd.message,
                                                    icon: cmd.icon
                                            }).then(function () { GoSideMenu("Approval"); }));
                                    } else {
                                        Swal.fire({
                                            title: cmd.title,
                                            text: cmd.message,
                                            icon: cmd.icon
                                        }).then(function () { GoSideMenu("Approval"); });
                                    }
                                }
                            }, 100);
                        }).catch(function (err) {
                            hideLoading();
                            hideLoadingAndShowProcess();
                            alert('Something went wrong.', err);
                            return false;
                        });
                });
                LoadScript("js\\Approval\\SendEmail\\EventEmailForm.js", "EventEmailApproverForm");
                hideLoading();
                hideLoadingAndShowProcess();
            }
        })
    });

if (btnCheckAll != null)
    btnCheckAll.addEventListener("click", function () {
        let chckSec = document.querySelectorAll("input.chck-section");
        let chckDoc = document.querySelectorAll("input.chck-doc");
        if (chckSec != null)
            chckSec.forEach(function (cbcheck, index) {
                cbcheck.checked = btnCheckAll.checked;
            });
        if (chckDoc != null)
            chckDoc.forEach(function (cbcheck, index) {
                cbcheck.checked = btnCheckAll.checked;
            });

    });

if (btnNewlate != null) {
    btnNewlate.addEventListener("click", async function () {
        let url = toFlowNewlate;
        await DisplayResult(url);
        BtnActiive("FlowNewlate");
    });
}

if (btnHRApprove != null)
    btnHRApprove.forEach(function (button) {
        button.addEventListener("click", function () {
            displayLoading();
            let reqDoc = button.value;
            let jsonReqDoc = {};
            jsonReqDoc["no"] = reqDoc;
            fetch("Approval/HRApproved", {
                method: "POST",
                referrerPolicy: "strict-origin-when-cross-origin",
                credentials: "same-origin",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonReqDoc),
            }).then(function (response) {
                // When the page is loaded convert it to text
                return response.text()
                }).then(async function (swal) {
                    hideLoading();

                    //trans text to json
                    swal = JSON.parse(swal);
                    Swal.fire({
                        title: swal.title,
                        text: swal.message,
                        icon: swal.icon
                    }).then(function () {
                        if (swal.icon == "success") {
                            GoSideMenu("Approval");
                        }
                    });
                });
        });
    });


if (Documents != null) {
    Documents.forEach(function (div) {
        var reqDoc = div.querySelector("button.view").value;
        let rdoChecked = div.getElementsByClassName("checked").item(0);
        let rdoUncheck = div.getElementsByClassName("uncheck").item(0);
        let btnApprove = div.getElementsByClassName("btnApprove").item(0);
        let btnReject = div.getElementsByClassName("btnReject").item(0);

        if (rdoChecked != null)
            rdoChecked.addEventListener("click", function () {
                let chckApprove = div.querySelectorAll(".chck-app");
                chckApprove.forEach(function (checkbox) {
                    checkbox.checked = true;
                });
            });

        if (rdoUncheck != null)
            rdoUncheck.addEventListener("click", function () {
                let chckApprove = div.querySelectorAll(".chck-app");
                chckApprove.forEach(function (checkbox) {
                    checkbox.checked = false;
                });
            });

        if (btnApprove != null)
            btnApprove.addEventListener("click", function () {
                displayLoading();
                let jsonReqDoc = {};
                jsonReqDoc["no"] = reqDoc;
                fetch("Approval/ptvEmailForm", {
                    method: "POST",
                    referrerPolicy: "strict-origin-when-cross-origin",
                    credentials: "same-origin",
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(jsonReqDoc),
                }).then(function (response) {
                    // When the page is loaded convert it to text
                    return response.text()
                }).then(async function (html) {
                    document.getElementById("modal-new-content").innerHTML = "";
                    document.getElementById("roadApprov").innerText = "ส่งถึง";
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, "text/html");

                    var ToContent = doc.getElementById("OTContent_mail").outerHTML;
                    var ToFooter = doc.getElementById("footermail").outerHTML;

                    var displayContent = document.getElementById("modal-email-content");
                    var displayFooter = document.getElementById("FooterEmailContent");
                    displayContent.innerHTML = ToContent;
                    displayFooter.innerHTML = ToFooter;
                    SaveAndSendMail(document, div, "Approval/Approved");
                    LoadScript("js\\Approval\\SendEmail\\EventEmailForm.js", "EventEmailApproverForm");
                    displayEmailApprover(document);

                    hideLoading();

                });
            });

        if (btnReject != null)
            btnReject.addEventListener("click", function () {
                displayLoading();
                let jsonReqDoc = {};
                jsonReqDoc["no"] = reqDoc;
                fetch("Approval/ptvRejectForm", {
                    method: "POST",
                    referrerPolicy: "strict-origin-when-cross-origin",
                    credentials: "same-origin",
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(jsonReqDoc),
                }).then(function (response) {
                    // When the page is loaded convert it to text
                    return response.text()
                }).then(async function (html) {
                    document.getElementById("modal-new-content").innerHTML = "";
                    document.getElementById("roadApprov").innerText = "ไม่อนุมัติ";
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, "text/html");

                    var ToContent = doc.getElementById("OTContent_mail").outerHTML;
                    var ToFooter = doc.getElementById("footermail").outerHTML;
                    var displayContent = document.getElementById("modal-email-content");
                    var displayFooter = document.getElementById("FooterEmailContent");
                    displayContent.innerHTML = ToContent;
                    displayFooter.innerHTML = ToFooter;
                    SaveAndSendMail(document, div, "Approval/Reject");

                    hideLoading();
                });
            });

    });
}

function SaveAndSendMail(htmldocument, divdoc, api) {
    let btnSend = htmldocument.getElementsByClassName("btnSend").item(0);
    let reqDoc = divdoc.querySelector("button.view").value;
    let txtMailID = htmldocument.getElementById("txtMail");
    let txtRemarkID = htmldocument.getElementById("historyApproveds_htRemark");
    if (btnSend != null) {
        btnSend.addEventListener("click", function () {
            displayLoading();
            //get and set value to json form
            var workerList = {};
            let model = new Array();
            let cc = new Array();
            let chckApprove = divdoc.querySelectorAll(".chck-app");
            let emailcc = htmldocument.querySelectorAll("label.cc");
            if (chckApprove != null)
                chckApprove.forEach(function (checkbox) {
                    if (checkbox.checked)
                        model.push(checkbox.value);
                });
            if (emailcc != null)
                emailcc.forEach(function (label) {
                    if (label != null) {
                        cc.push(label.textContent);
                    }
                });
            workerList["workerList"] = model;
            workerList["reqNo"] = reqDoc;
            workerList["empcode"] = txtMailID == null ? "" : txtMailID.value;
            workerList["cc"] = cc;
            workerList["remark"] = txtRemarkID == null ? "" :txtRemarkID.value;

            fetch(api, {
                method: "POST",
                referrerPolicy: "strict-origin-when-cross-origin",
                credentials: "same-origin",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(workerList),
            }).then(
                function (response) {
                    return response.text();
                }).then(function (cmd) {
                    hideLoading();

                    //trans text to json
                    cmd = JSON.parse(cmd);
                    Swal.fire({
                        title: cmd.title,
                        text: cmd.message,
                        icon: cmd.icon
                    }).then(function () {
                        if (cmd.icon == "success") {
                            $('#modalNewOT').modal('hide');
                            GoSideMenu("Approval");
                        }
                    });
                }).catch(function (err) {
                    hideLoading();
                    alert('Something went wrong.', err);
                    return false;
                });
        });
    }
}

function displayEmailApprover(html) {
    let pst = html.getElementsByClassName("lbpermiss").item(0).textContent;
    let lbMail = html.querySelector("label.lbmailbypermiss");
    let text = "";
    switch (pst) {
        case "GL": case "ZZ":
            text = "ผู้พิจารณา (To GL)";
            break;
        case "CS":
            text = "ผู้พิจารณา (To CS)";
            break;
        case "DM":
            text = "ผู้อนุมัติ (To DM)";
            break;
        case "Admin":
            text = "ผู้พิจารณา (To Admin)";
            break;
        case "HCM":
            text = "ผู้ดูแล HCM (To HCM Admin)"
            break;
        default:
            text = "ส่งถึง (To)";
    }
    lbMail.innerHTML = text;
}



