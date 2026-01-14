var ul = document.getElementsByClassName("woker-list").item(0);
var txtEmpCode = document.getElementById("txtEmpCode");

function getEventTarget(e) {
    e = e || window.event;
    return e.target || e.srcElement;
}

ul.onclick = function (event) {
    var target = getEventTarget(event).innerText;
    txtEmpCode.value = target.split("\n")[0];
    ul.remove();
};