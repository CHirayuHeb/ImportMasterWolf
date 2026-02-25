//var btnHour = document.getElementsByClassName("hour").item(0);
//var btnDocument = document.getElementsByClassName("document").item(0);
//var toHourAction = "Home\\DisplayHour";
//var toDocumentAction = "Home\\DisplayDocument";

//btnHour.addEventListener("click", function () {
//    let url = toHourAction;
//    DisplayResult(url);
//    BtnActiive("hour");
//});
//btnDocument.addEventListener("click", function () {
//    let url = toDocumentAction;
//    DisplayResult(url);
//    BtnActiive("document");
//});

$('input.Searchdatepicker').datepicker({
    //format: 'dd/mm/yyyy',
    format: 'yyyy/mm/dd',
    todayBtn: 'linked',
    todayHighlight: true,
    autoclose: true,
    orientation: "auto"

});


$('input.datepicker').datepicker({
    format: 'dd/mm/yyyy',
    // format: 'yyyy/mm/dd',
    todayBtn: 'linked',
    todayHighlight: true,
    autoclose: true,
    orientation: "auto"

});

$('input.Monthpicker').datepicker({
    //format: 'yyyy/mm',
    format: 'mm/yyyy',
    todayBtn: 'linked',
    todayHighlight: true,
    autoclose: true,
    orientation: "auto"

});

$('.timepicker').timepicker({
    timeFormat: 'HH:mm', // ใช้รูปแบบ 24 ชั่วโมง
    //interval: 30,        // ให้เลือกเวลาได้ทีละ 30 นาที
    minTime: '00:00',    // เวลาต่ำสุด
    maxTime: '23:30',    // เวลาสูงสุด
    dynamic: false,
    dropdown: true,
    scrollbar: true
    // minuteStep: 30,
});


$('input.MMYYpicker').datepicker({
    //format: 'yyyy/mm',
    format: 'mm/yy',
    todayBtn: 'linked',
    todayHighlight: true,
    autoclose: true,
    orientation: "auto"

});
