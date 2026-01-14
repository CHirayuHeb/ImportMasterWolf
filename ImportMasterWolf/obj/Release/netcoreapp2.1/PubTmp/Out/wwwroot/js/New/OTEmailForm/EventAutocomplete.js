var ul = document.getElementsByClassName("mail-by-position").item(0);
var ulCC = document.getElementsByClassName("mail-cc").item(0);
var txtMail = document.getElementById("txtMail");
var txtEmpApp = document.getElementById("mastRequestOT_mrEmpApp");
var txtMailCC = document.getElementById("txtMailCC");

function getEventTarget(e) {
    e = e || window.event;
    return e.target || e.srcElement;
}

if (ul != null) {
    ul.onclick = function (event) {
        var target = getEventTarget(event).innerText;
        txtMail.value = target.split("\n")[0];
        txtEmpApp.value = target.split("\n")[0];
        ul.remove();
        txtMail.focus();
    };
}

if (ulCC != null) {
    ulCC.onclick = function (event) {
        var target = getEventTarget(event).innerText;
        txtMailCC.value = target.split("\n")[0];
        ulCC.remove();
        txtMailCC.focus();
    };
}
