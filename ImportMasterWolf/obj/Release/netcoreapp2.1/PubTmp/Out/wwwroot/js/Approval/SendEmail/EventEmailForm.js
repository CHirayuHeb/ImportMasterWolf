var lbMail = document.querySelector("label.lbmail");
var lbPST = document.querySelector("label.lbpermiss");
var txtMail = document.getElementById("txtMail");
var txtCCMail = document.getElementById("txtMailCC");
var btnAddMailCC = document.getElementsByClassName("addcc").item(0);

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
            default:
                text = "ส่งถึง (To)";
        }

        lbMail.innerHTML = text;
    }
}


if (txtMail != null) {
    txtMail.addEventListener("keyup", function (e) {
        showMails(this.value, lbPST.textContent.trim().toUpperCase());
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
    let str = "<div class='d-flex flex-dir-row py-1 cc-box'><label class='cc'>" + val + "</label><button class='bd-0 bg-trans cc-delete' type='button'><span class='material-icon-symbols-outlined fs-16'>close</span></button></div>";
    let parser = new DOMParser();
    let html = parser.parseFromString(str, 'text/html');

    document.getElementById("CCMailContent").append(html.getElementsByTagName("div").item(0));
    LoadScript("js\\" + "New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");
    txtCCMail.value = "";
}

function showMails(val, pst) {
    res = document.getElementsByClassName("autocompleteEMail").item(0);
    res.innerHTML = '';
    if (val == '')
        return;
    let list = '';
    let url = "Approval\\suggestMails?q=";
    let req = document.getElementById("mastRequestOT_mrNoReq").value;
    let str = "&pst=" + pst + "&req=" + req;
    fetch(url + val + str, {
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(
        function (response) {
            return response.json();
        }).then(function (data) {
            for (i = 0; i < data.emails.length; i++) {
                list += '<li class="border-rad px2 cs-pt");>';
                list += '<div class="fw-800">' + data.emails[i].empCode + '</div>' + data.emails[i].empCode + " " + data.emails[i].fullNameAndDept;
                list += '</li >';
            }
            res.innerHTML = '<ul class="mail-by-position">' + list + '</ul>';
            document.getElementById("lbDisplayMailName").innerHTML = data.fullname;
            LoadScript("js\\" + "New\\OTEmailForm\\EventAutocomplete.js", "EventFormAutocomplete");
            return true;
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
    fetch(url + val).then(
        function (response) {
            return response.json();
        }).then(function (data) {
            for (i = 0; i < data.length; i++) {
                list += '<li class="border-rad px2 cs-pt");>';
                list += '<div class="fw-800">' + data[i].mail + '</div>' + data[i].empCode + " " + data[i].fullNameAndDept;
                list += '</li >';
            }
            res.innerHTML = '<ul class="mail-cc">' + list + '</ul>';
            
            LoadScript("js\\" + "New\\OTEmailForm\\EventAutocomplete.js", "EventFormAutocomplete");
            return true;
        }).catch(function (err) {
            alert('Something went wrong.', err);
            return false;
        });
}