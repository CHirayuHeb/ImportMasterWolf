var btnWorkerBox = document.getElementsByClassName("worker-box");
var btnDelete = document.querySelectorAll("button.worker-remove");
var lbWorkerSuccess = document.getElementsByClassName("work-success").item(0);

if(btnDelete != null) {
btnDelete.forEach(function (button, index) {
    button.addEventListener("click", function () {
        Swal.fire({
            icon: "warning",
            title: "เตือน",
            text: "คุณต้องการลบรายการนี้ใช่หรือไม่",
            confirmButtonText: "ยืนยัน",
            cancelButtonText: "ยกเลิก",
            confirmButtonColor: "#bbbbbb",
            cancelButtonColor: "#ffaa88",
            focusCancel: true,
            showCloseButton: true,
            showCancelButton: true,
            showCancelButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                let empcode = btnWorkerBox.item(index).getElementsByClassName("empcode").item(0).innerHTML;
                let req = document.getElementById("mrNoReq").value;
                let url = "New/DeleteWorker?empcode=" + empcode + "&req=" + req;
                fetch(url, {
                    method: "POST",
                    referrerPolicy: "strict-origin-when-cross-origin",
                    credentials: "same-origin",
                }).then(function (response) {
                    return response.text();
                    }).then(function (result) {
                    result = JSON.parse(result);
                    if (result.icon = "success") {
                        btnWorkerBox.item(index).outerHTML = "";
                        //if (btnConfirmChangeEmp != null) {
                        //    btnConfirmChangeEmp.setAttribute("style", "display: block");
                        //    lbWorkerSuccess.setAttribute("style", "display: none");
                        //}

                        let cateWorkerOnDocTarget = document.getElementById("collapse" + req);
                        if (cateWorkerOnDocTarget != null) 
                            updateWorkerAfterDelete(cateWorkerOnDocTarget, req);

                        LoadScript("js\\" + "New\\OTAddWorker\\EventDelete.js", "EventWorkerDelete");
                    }
                });
            }
        });
    });
    });
}