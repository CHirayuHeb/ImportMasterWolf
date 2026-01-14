var btnApproverSetting = document.getElementById("btnApproverSetting");

if (btnApproverSetting != null)
    btnApproverSetting.addEventListener("click", function () {
        displayLoading();
        fetch("Administrator/ApproverSetting", {
            method: "POST",
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
            headers: { 'Content-Type': 'application/json' },
        }).then(function (response) {
            // When the page is loaded convert it to text
            return response.text()
        }).then(async function (html) {
            document.getElementById("modal-setting-content").innerHTML = "";
            document.getElementById("HeadTitle").innerText = "ตั้งค่า > การอนุมัติ";

            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            var displayContent = document.getElementById("modal-setting-content");
            var displayFooter = document.getElementById("FooterContent");

            if (doc.getElementsByTagName("title").item(0) != null) {
                if (doc.getElementsByTagName("title").item(0).textContent == "403") {
                    displayContent.innerHTML = doc.getElementsByTagName("body").item(0).innerHTML;
                    displayFooter.innerHTML = "";
                    hideLoading();
                }
            } else {
                var ToContent = doc.getElementById("OTAdmin_Setting").outerHTML;
                var ToFooter = doc.getElementById("footersetting").outerHTML;

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
                            reqs.push({ "no": cbcheck.value });
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
                //LoadScript("js\\Approval\\SendEmail\\EventEmailForm.js", "EventEmailApproverForm");
                hideLoading();
                hideLoadingAndShowProcess();
            }
        })
    });