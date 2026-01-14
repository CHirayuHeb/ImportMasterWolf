var btnPreviosS3 = document.querySelector("button.previos3");
var btnNextS3 = document.querySelector("button.next3");
var actionOTAddWorker = "New\\OTAddWorker";
var slcReason = document.getElementById("mastRequestOT_mrReason");
var lbTitleReason = document.getElementById("lbReasonTitle");
var txtTimepicker = document.querySelectorAll("input.timepicker"); 
$('.timepicker').timepicker({
    showMeridian: false,
});
//var txtDatepicker = document.querySelectorAll(".datepicker");

if (txtTimepicker != null) {
    txtTimepicker.forEach(function (input) {
        input.addEventListener("click", function () {
            input.readOnly = true;
            input.focus();
            setTimeout(function () { input.readOnly = false }, 50);
        });
    });
}
if (btnPreviosS3 != null) {
    btnPreviosS3.addEventListener("click", function () {
        Back("3");
    });
}

if (btnNextS3 != null) {
    btnNextS3.addEventListener("click", async function () {
        let draft = await draftOTDocument();
        if (draft == "resolved") {
            await createNextstep("4");
            await GoToNextStep("4", actionOTAddWorker);
        }
    });
}

if (slcReason != null) {
    TitleReason();
    slcReason.addEventListener("change", function () {
        TitleReason();
    });
}

function TitleReason() {
    let url = "New\\captionReason?q=";
    fetch(url + slcReason.value, {
        referrerPolicy: "strict-origin-when-cross-origin",
        credentials: "same-origin",
    }).then(
        function (response) {
            return response.json();
        }).then(function (caption) {
            lbTitleReason.innerHTML = caption;
            return true;
        }).catch(function (err) {
            alert('Something went wrong.', err);
            return false;
        });
}
//jQuery('#boostTimeST').timepicker();
//txtDatepicker.forEach(function (textbox) {
//    textbox.addEventListener("change", function () {
//        let valueDate = new Date(valuetextbox.valueDate).toLocaleDateString('en-US');
//        document.getElementById("OTDateRequest").value = valueDate;
//    });
//});
