var btnCCBox = document.getElementsByClassName("cc-box");
var btnDelete = document.querySelectorAll("button.cc-delete");
if (btnDelete != null) {
    btnDelete.forEach(function (button, index) {
        button.addEventListener("click", function () {
            if (btnCCBox.item(index) != null)
                btnCCBox.item(index).outerHTML = "";
            LoadScript("js\\New\\OTEmailForm\\EventDelete.js", "EventEmailFormDelete");
        });
    });
}
