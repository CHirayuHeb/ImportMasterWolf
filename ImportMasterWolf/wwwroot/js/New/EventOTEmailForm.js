var btnPreviosS5 = document.querySelector("button.previos5");
var btnSave = document.querySelector("button.finish");
var btnChange = document.querySelector("button.change");
var lbMail = document.querySelector("label.lbmail");
var lbPST = document.querySelector("label.lbpst");
var txtMail = document.getElementById("txtMail");
var txtCCMail = document.getElementById("txtMailCC");
var btnAddMailCC = document.getElementsByClassName("addcc").item(0);

if (btnPreviosS5 != null) {
    btnPreviosS5.addEventListener("click", function () {
        Back("5");
    });
}

if (btnChange != null) {
    btnChange.addEventListener("click", function (e) {
        displayLoading();

        let formCrateNew = new FormData(document.getElementById("formCreateNew"));
        var param1 = new URLSearchParams(formCrateNew);
        param1.append("mrOTType", document.getElementById("OTType").value);
        param1.append("req", btnChange.value);
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

        let url = "New\\ChangeUpdate";
        fetch(url, {
            method: "POST",
            referrerPolicy: "strict-origin-when-cross-origin",
            credentials: "same-origin",
            body: param1,
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
                        GoSideMenu("New");
                    }
                });
            }).catch(function (err) {
                hideLoading();
                alert('Something went wrong.', err);
                return false;
            });
    });
}

if (btnSave != null) {
    btnSave.addEventListener("click", function (e) {
        //show Loading
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

        let url = "New\\CreateNew";
        fetch(url, {
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
                Swal.fire({
                    title: cmd.title,
                    text: cmd.message,
                    icon: cmd.icon
                }).then(function () {
                    if (cmd.icon == "success") {
                        $('#modalNewOT').modal('hide');
                        GoSideMenu("New");
                    }
                });
            }).catch(function (err) {
                hideLoading();
                alert('Something went wrong.', err);
                return false;
            });
    });
}

if (lbMail != null) {
    if (lbMail.textContent != null) {
        let pst = lbMail.textContent.trim().toUpperCase();
        let text = new String;
        switch (pst) {
            case "UL":
                text = "ผู้พิจารณา (GL)"
                break;
            case "GL": case "ZZ":
                text = "ผู้พิจารณา (CS)";
                break;
            case "CS":
                text = "ผู้พิจารณา (DM)";
                break;
            case "DM":
                text = "ส่งถึง (Admin)";
                break;
            case "ADMIN":
                text = "ส่งถึง HCM Admin";
                break;
            default:
                text = "ส่งถึง (To)";
        }
        lbMail.innerHTML = text;
    }
}


if (txtMail != null) {
    txtMail.addEventListener("keyup", function (e) {
        showMails(txtMail.value, lbPST.textContent.trim().toUpperCase());
    });
}

if (txtCCMail != null) {
    txtCCMail.addEventListener("keyup", function (e) {
        if (e.keyCode === 13) {
            e.preventDefault();
            pushMailCC(txtCCMail.value);
        } else {
            showCCMails(txtCCMail.value);
        }
    });
}

if (btnAddMailCC != null) {
    btnAddMailCC.addEventListener("click", function () {
        pushMailCC(txtCCMail.value);
    });
}

function pushMailCC(val) {
    //<button class="bd-0 bg-trans worker-remove" type="button"><span class="material-icon-symbols-outlined fs-16">close</span></button>
    let str = "<div class='d-flex flex-dir-row py-1 cc-box'><label class='cc'>" + val + "</label><button class='bd-0 bg-trans cc-delete' type='button' onclick=this.parentElement.outerHTML=''><span class='material-icon-symbols-outlined fs-16'>close</span></button></div>";
    let parser = new DOMParser();
    let html = parser.parseFromString(str, 'text/html');
    
    document.getElementById("CCMailContent").append(html.getElementsByTagName("div").item(0));
    //LoadScript("js\\" + "New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");
    txtCCMail.value = "";
}

function showMails(val, pst) {
    res = document.getElementsByClassName("autocompleteEMail").item(0);
    res.innerHTML = '';
    if (val == '') {
        return;
    }
    let list = '';
    let url = "New\\suggestMails?q=";
    let str = "&pst=" + pst;
    fetch(url + val + str, {
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(
        function (response) {
            return response.json();
        }).then(async function (data) {
            for (i = 0; i < data.emails.length; i++) {
                list += '<li class="border-rad px2 cs-pt");>';
                list += '<div class="fw-800">' + data.emails[i].empCode + '</div>' + data.emails[i].empCode + " " + data.emails[i].fullNameAndDept;
                list += '</li >';
            }
            res.innerHTML = '<ul class="mail-by-position">' + list + '</ul>';
            document.getElementById("lbDisplayMailName").innerHTML = data.fullname;

            LoadScript("js\\" + "New\\OTEmailForm\\EventAutocomplete.js", "EventFormAutocomplete");
        }).catch(function (err) {
            alert('Something went wrong.', err);
            return false;
        });
}

function showCCMails(val) {
    res = document.getElementsByClassName("autocompleteEMailCC").item(0);
    res.innerHTML = '';
    if (val == '') {
        return;
    }
    let list = '';
    let url = "New\\suggestCCMails?q=";
    fetch(url + val, {
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(
        function (response) {
            return response.json();
        }).then(function (data) {
            for (i = 0; i < data.length; i++) {
                list += '<li class="border-rad px2 cs-pt");>';
                list += '<div class="fw-800">' + data[i].mail + '</div>' + data[i].empCode + " " + data[i].fullNameAndDept;
                list += '</li >';
            }
            res.innerHTML = '<ul class="mail-cc">' + list + '</ul>';
            LoadScript("js\\New\\OTEmailForm\\EventAutocomplete.js", "EventFormAutocomplete");
            return true;
        }).catch(function (err) {
            alert('Something went wrong.', err);
            return false;
        });
}