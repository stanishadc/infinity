$(document).ready(function () {
    alert('1')
    GetGroupTypes();
});
function GetGroupTypes() {
    $.ajax({
        url: '/grouptype/Get',
        type: 'get',
        datatype: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            if (response == null || response == undefined || response.length == 0) {
                var object = '';
                object += '<tr><td colspan="2"> No Records Found </td></tr>';
                $('#tblBody').html(object);
            }
            else {
                var object = '';
                var i = 1;
                $.each(response, function (index, item) {
                    var m = new Date(item.lastUpdated);
                    var dateString = m.getMonth() + "/" + (m.getDate() + 1) + "/" + m.getFullYear() + " " + m.getHours() + ":" + m.getMinutes();
                    object += '<tr>';
                    object += '<td>' + parseInt(index + i) + '</td>';
                    object += '<td>' + item.groupTypeName + '</td>';
                    object += '<td>' + dateString + '</td>';
                    object += '<td><a href="#" class="btn btn-primary btn-sm" onclick="return Edit(' + item.groupTypeId + ')">Edit</a><a href="#" class="btn btn-danger btn-sm" onclick="Delete(' + item.groupTypeId + ')">Delete</a></td>';
                });
                $('#tblBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}

function OpenModal() {
    $('#GroupTypeModal').modal('show');
    $('#modalTitle').text('Add Group Type');
}
function HideModal() {
    ClearData();
    $('#GroupTypeModal').modal('hide');
}
function ClearData() {
    $('#GroupTypeName').val('');
    $('#GroupTypeId').val('');
}
function Insert() {
    var formData = new Object();
    formData.groupTypeName = $('#GroupTypeName').val();
    $.ajax({
        url: '/grouptype/Insert',
        data: formData,
        type: 'POST',
        success: function (response) {
            //GetGroupTypes();
            HideModal();
        },
        error: function () {
            alert('Unable to save the data');
        }
    })
}
function Update() {
    var formData = new Object();
    formData.groupTypeName = $('#GroupTypeName').val();
    formData.groupTypeId = $('#groupTypeId').val();
    $.ajax({
        url: '/grouptype/Update',
        data: formData,
        type: 'PUT',
        success: function (response) {
            //GetGroupTypes();
            HideModal();
        },
        error: function () {
            alert('Unable to save the data');
        }
    })
}
function Edit(Id) {
    $.ajax({
        url: '/grouptype/Edit?id=' + Id,
        type: 'get',
        datatype: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('Data not available wit the id ' + Id);
            }
            else {
                $('#GroupTypeModal').modal('show');
                $('#modalTitle').text('Edit Group Type');
                $('#Save').css('display', 'none');
                $('#Update').css('display', 'block');
                $('#modalTitle').text('Edit Group Type');
                $('#GroupTypeId').val(response.groupTypeId);
                $('#GroupTypeName').val(response.groupTypeName);
            }
        },
        error: function () {
            alert('Unable to save the data');
        }
    })
}

<script type="text/javascript">
    function GetGroupTypes() {
        $.ajax({
            url: '/grouptype/Get',
            type: 'get',
            datatype: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (response) {

            },
            error: function () {
                alert('Unable to read the data');
            }
        });
    }
    function Edit(Id) {
        $.ajax({
            url: '/grouptype/Edit?id=' + Id,
            type: 'get',
            datatype: 'json',
            contentType: 'application/json;charset=utf-8',
            success: function (response) {
                if (response == null || response == undefined) {
                    alert('Unable to read the data');
                }
                else if (response.length == 0) {
                    alert('Data not available wit the id ' + Id);
                }
                else {
                    $('#GroupTypeId').val(response.groupTypeId);
                    $('#GroupTypeName').val(response.groupTypeName);
                }
            },
            error: function () {
                alert('Unable to save the data');
            }
        })
    }
    function Insert() {
        var formData = new Object();
    formData.groupTypeName = $('#GroupTypeName').val();
    $.ajax({
        url: '/grouptype/Insert',
    data: formData,
    type: 'POST',
    success: function (response) {
        GetGroupTypes();
    HideModal();
            },
    error: function () {
        alert('Unable to save the data');
            }
        })
    }
    function Update() {
        var formData = new Object();
    formData.groupTypeName = $('#GroupTypeName').val();
    formData.groupTypeId = $('#GroupTypeId').val();
    $.ajax({
        url: '/grouptype/Update',
    data: formData,
    type: 'PUT',
    success: function (response) {
        GetGroupTypes();
    HideModal();
            },
    error: function () {
        alert('Unable to save the data');
            }
        })
    }
    function DeleteRecord(Id) {
        var ans = confirm("Are you sure you want to delete?");
    if (ans) {
        $.ajax({
            url: "/GroupType/Delete?GroupTypeId=" + Id,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                GetGroupTypes();
            },
            error: function (errormessage) {
                //alert(errormessage.responseText);
            }
        });
        }
    }
    function OpenModal() {
        $('#addGroupType').modal('show');
    $('#modalTitle').text('New Group Type');
    }
    function UpdateModal(Id) {
        $('#Save').css('display', 'none');
    $('#Update').css('display', 'block');
    $('#GroupTypeModal').modal('show');
    $('#modalTitle').text('Edit Group Type');
    Edit(Id);
    }
    function HideModal() {
        ClearData();
    $('#GroupTypeModal').modal('hide');
    }
    function ClearData() {
        $('#GroupTypeName').val('');
    $('#GroupTypeId').val('');
    }
</script>