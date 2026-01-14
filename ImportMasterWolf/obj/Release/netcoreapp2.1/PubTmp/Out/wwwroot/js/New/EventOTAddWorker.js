var actionOTEmailForm = "New\\OTEmailForm";
var btnPreviosS4 = document.querySelector("button.previos4");
var btnNextS4 = document.querySelector("button.next4");
var txtEmpCode = document.getElementById("txtEmpCode");
var btnAddWorker = document.getElementsByClassName("add-worker").item(0);
var btnConfirmChangeEmp = document.querySelector("button.worker-confirm");
var lbWorkerSuccess = document.getElementsByClassName("work-success").item(0);
var btnChange = document.querySelector("button.change");


if (btnPreviosS4 != null) {
    btnPreviosS4.addEventListener("click", function () {
        Back("4");
    });
}

if (btnNextS4 != null) {
    btnNextS4.addEventListener("click", function () {
        //if (btnConfirmChangeEmp != null) {
        //    if (btnConfirmChangeEmp.style.display === "block") {
        //        alert("กรุณากดปุ่มยืนยันการเปลี่ยนแปลงพนักงาน");
        //        return false;
        //    }
        //}
        createNextstep("5");
        GoToNextStep("5", actionOTEmailForm);
    });
}

if (btnConfirmChangeEmp != null) {
    btnConfirmChangeEmp.addEventListener("click", function () {
        var param = new URLSearchParams();

        //push reqNo
        if (btnChange != null) 
            param.append("req",  btnChange.value);

        //loop push worker
        let poiterWorker = document.querySelectorAll(".worker-newot-details");
        poiterWorker.forEach(function (div) {
            param.append("NewWorkerList", JSON.stringify({
                "drEmpCode": div.getElementsByClassName("empcode").item(0).textContent,
                "drJobCode": div.getElementsByClassName("job").item(0).value
            }));
        });
        let url = "New\\ChangeWorker?";
        fetch(url + param, {
            method: "POST",
        }).then(
            function (response) {
                return response.text();
            }).then(function (cmd) {
                cmd = JSON.parse(cmd);
                if (cmd.icon == "success") {
                    btnConfirmChangeEmp.style.display = "none";
                    lbWorkerSuccess.style.display = "block";
                } else {
                    txtEmpCode.focus();
                }
            }).catch(function (err) {
                alert('Something went wrong.', err);
                return false;
            });
    });
}

if (txtEmpCode != null) {
    txtEmpCode.addEventListener("keyup", function (e) {
        if (e.keyCode === 13) {
            Swal.fire({
                text: 'Loading...',
                allowEscapeKey: false,
                //allowOutsideClick: false,
                showConfirmButton: false,
                timerProgressBar: true,
                timer: 500,
            }).then(function () {
                pushWorker(txtEmpCode.value);
            });
        } else {
            showEmpCode(txtEmpCode.value);
        }
    });
}

if (btnAddWorker != null) {
    btnAddWorker.addEventListener("click", function (e) {
        e.preventDefault();
        Swal.fire({
            text: 'Loading...',
            allowEscapeKey: false,
            allowOutsideClick: false,
            showConfirmButton: false,
            timerProgressBar: true,
            timer: 500,
        }).then(function () {
            pushWorker(txtEmpCode.value);
            });
        Swal.showLoad
        //pushWorker(txtEmpCode.value);
    });
}

function pushWorker(EmpCode) {
    displayLoading();
    let req = "";
    if (document.getElementById("mrNoReq"))
        req = document.getElementById("mrNoReq").value;
    let url = "New\\PushWorker";
    let param = "?p="+EmpCode + "&req="+req
    fetch(url + param).then(
        function (response) {
            return response.text();
        }).then(function (html) {
            let parser = new DOMParser();
            let doc = parser.parseFromString(html, "text/html");
            if (doc.getElementsByClassName("empcode").item(0).innerHTML != "") {
                txtEmpCode.value = "";

                let cateWorkerOnDocTarget = document.getElementById("collapse" + req);
                if (cateWorkerOnDocTarget != null)
                    updateWorkerAfterDelete(cateWorkerOnDocTarget, req);    //delete display on basepage opened
                document.getElementById("WorkerContent").append(doc.getElementsByTagName("div").item(0));   //delete display on modal opened

                LoadScript("js\\" + "New\\OTAddWorker\\EventDelete.js", "EventWorkerDelete");
            }
            hideLoading();
        
        }).catch(function (err) {
            hideLoading();
            alert('Something went wrong.', err);
            return false;
        });
}

function showEmpCode(val) {
    res = document.getElementsByClassName("autocomplete").item(0);
    res.innerHTML = '';
    if (val == '') {
        return;
    }
    let list = '';
    let url = "New\\suggest?q=";
    fetch(url + val).then(
        function (response) {
            return response.json();
        }).then(function (data) {
            for (i = 0; i < data.length; i++) {
                list += '<li class="border-rad px-2 cs-pt");>';
                list += '<div class="fw-800">' + data[i].empCode + '</div>' + data[i].fullNameAndDept;
                list += '</li >';
            }
            res.innerHTML = '<ul class="woker-list">' + list + '</ul>';
            LoadScript("js\\" + "New\\OTAddWorker\\EventAutocomplete.js", "EventWorkerAutocomplete");
            return true;
        }).catch(function (err) {
            alert('Something went wrong.', err);
            return false;
        });
}


//get data from form elements + loop from class
        //var subobject = {};
        //var jsonconcat = {};
        //formID.split("_").forEach(function (txtID) {
        //formStep.forEach((value, key) => subobject[key] = value);
        //object = {
        //    "otRequest": [subobject]}
        //});
        //let json = JSON.stringify(object);
        //jsonconcat = { ...jsonconcat, json};