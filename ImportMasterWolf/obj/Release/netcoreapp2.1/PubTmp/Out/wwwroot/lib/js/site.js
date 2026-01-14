// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function Edit(getID) {
    $.ajax({
        type: 'post',
        //url: '@Url.Action("Edit", "SearchVersion")',
        url: '/SearchVersion/Edit',
        data: { id: getID },
        success: function (data) {
            $.each(data, function (i, item) {
                $('#modal-form').remove();
                htmlSelected = (item.vsStatus == "Open") ? "selected='selected'" : '';
                htmlClose = (item.vsStatus == "Close") ? "selected='selected'" : '';
                var htmls = " <form id='modal-form'> "
                    + "<div class='form-group'>"
                    + "<input type='text' name='vsNo' id='updateNo' value='" + item.vsNo + "' readonly='readonly' style='display: none;' />"
                    + "<label for='recipient-name' class='col-form-label'>Status:</label>"
                    + "<div>"
                    + "<select class='form-select' asp-for='vsStatus' id='seStatus' name='vsStatus' onchange='ChangeStatus(this.id)'>"
                    + "<option value='Open'" + htmlSelected + "  style='color:forestgreen;'>Open</option>"
                    + "<option value='Close' " + htmlClose + " style='color:orangered;'>Close</option>"
                    + "</select>"
                    + "</div>"
                    + "</div>"
                    + "<div class='form-group'>"
                    + "<label for='recipient-name' class='col-form-label'>Detail:</label>"
                    + "<div>"
                    + "<input class='form-control' type='text' name='vsDetail' id='changeDetail' value='" + item.vsDetail + "'>"
                    + "</div>"
                    + "</div>"
                    + "<div class='form-group'>"
                    + "<label for='recipient-name' class='col-form-label'>STJ Send Date:</label>"
                    + "<div>"
                    + "<input class='form-control datepicker' type='text' name='vsSTJSendDate' id='changeSTJSendDate' value='" + item.vsSTJSendDate + "'>"
                    + "</div>"
                    + "</div>"
                    + "</form>"
                $('#modal-body').append(htmls);
            });
            $('#editModal').modal('show');
        }
    });
}

function Update(getID) {
    $.ajax({
        type: 'post',
        //url: '@Url.Action("Update", "SearchVersion")',
        url: '/SearchVersion/Update',
        data: $('#modal-form').serialize(),
        success: function (message) {
            if (message != '') {
                $('#tr_'+ $(getID).val()).attr('style', 'color: blue;');
                Swal.fire({
                    icon: message,
                    title: message,
                    text: message == "success" ? "Change data complete" : "Fail. Please check your data"
                });
            }
        }
    });
}

function warningDelete(getID, action, message){
    Swal.fire({
        title: "Are you sure?",
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, archive it!",
        cancelButtonText: "No, keep it!",
    }).then((result) => {
        if (result['isConfirmed']) {
            console.log('Confirm');
            Delete(getID, action);
        } else {
            console.log('Cancel');
            return false;
        }
    });
}

function Delete(getID, action) {
    console.log(getID[0]);
    console.log(action);
    $.ajax({
        type: 'post',
        url: action,
        data: {id:getID},
        success: function (message) {
            console.log('message:'+ message);
            if (message != '') {
                if (message == 'success') {
                    $("#tr_"+ getID[0]).remove();
                }
                Swal.fire({
                    icon: message,
                    title: message,
                    text: message == "success" ? "Deleted" : "Delete not complete"
                });
            }
        }
    });
}

function Save(btnSave, currentVersion) {
    Swal.fire({
        title: "Are you sure?",
        text: "Current version is " + currentVersion,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, archive it!",
        cancelButtonText: "No, i will refresh version before!",
    }).then((result) => {
        if (result['isConfirmed']) {
            console.log('Confirm');
            $("#" + btnSave).removeAttr("type");
            $("#" + btnSave).attr("type", "submit");
            $("#" + btnSave).click();
        } else {
            console.log('Cancel');
            return false;
        }
    });
}

function messageAlert(type, message) {
    if (type != '') {
        Swal.fire({
            icon: type,
            title: type,
            text: message
        });
    }
}
    
