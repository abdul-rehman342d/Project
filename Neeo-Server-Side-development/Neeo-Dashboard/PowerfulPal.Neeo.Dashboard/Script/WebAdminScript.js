

//Execution of this funcion starts before web page elements loading
function preloadFunc() {
    debugger;
    if (!IsLoggedInAdmin) {
        window.location = "/Login.html";
    }

}
window.onpaint = preloadFunc();

$(document).ready(function () {

    BuildDataGrig();
    RegisterClickEvents();

});


function RegisterClickEvents() {

    CloseEditPannalButtonClick();
    ChangePasswordButtonClick();
    closeSummaryPannalButtonClick();
    AddUser();
    UpdateUserStatusButtonClick();
    EditPannalOkButtonClick();
    closeAddPannalButtonClick();
    CloseDeletePannalClick();
    CancelButtonDeletePannalClick();
    DeleteUserPannalButtonClick();
    CancelButtonSummaryPannalClick();
    ClearHistoryButtonClick();
    LogOut();
}



//User Login Related
function LogOut() {

    $("#user-dropdown")
      .change(function () {
          var str = "";
          $("select option:selected").each(function () {
              str += $(this).text();
          });

          var x = CompareTwoStrings(str, 'Sign Out');
          if (x) {

              $('html, body').css({
                  'overflow': 'hidden',
                  'height': '100%'
              });

              var key = readCookie('token');

              $.ajax({
                  url: baseUrl + 'api/v2/user/' + readCookie('username') + '/logout/',
                  type: 'PUT',
                  contentType: 'application/json;charset=utf-8',
                  headers: {
                      "Authorization": key,
                  },
                  accepts: 'application/json',
                  async: false,

                  success: function (response) {

                      if (!(response == null || response.toString() == "")) {
                          if (CompareTwoStrings(response.toString(), 'true')) {
                              eraseCookie('token');
                              eraseCookie('user');
                              eraseCookie("LastLoginTime");
                              eraseCookie("LastSyncTime");
                              window.location.replace("/Login.html");
                          }
                      }

                  },
                  error: function (response) {
                      showMessage('Ajax call faild');
                      eraseCookie('token');
                      eraseCookie('user');
                      eraseCookie("LastLoginTime");
                      eraseCookie("LastSyncTime");
                      window.location.replace("/Login.html");
                  },
              });

          }

      })
      .change();


}

function IsLoggedInAdmin() {
    debugger;
    var token = readCookie("token");
    if (!(token == null || token == '')) {
        if (readCookie("user") == 'webadmin') {
            if (checkSessionExistanc(token))
            { return true; }
            else
            { eraseCookie('token'); eraseCookie('user'); return false; }
        }

        else { eraseCookie('token'); eraseCookie('user'); return false; }
    }
    else { return false; }
}


//End User Login Related


//User Related functions
function AddUser() {

    $("#Add-saveButton").jqxButton({ theme: theme });
    $('#Add-saveButton').click(function () {
        debugger;
        var username = $('#add-UserNameTextbox').val();
        var password = $('#add-PasswordTextbox').val();
        var reTypePassword = $('#add-retypePasswordTextbox').val();
        if (username != '' && password != '') {
            if (CompareTwoStrings(password, reTypePassword)) {
                var md5password = $.md5(password);
                if (AddNewUserSeverCall(username, md5password)) {

                    $('#add-UserNameTextbox').val('');
                    $('#add-PasswordTextbox').val('');
                    $('#add-retypePasswordTextbox').val('');
                    ClosePannal('500', '530', '1px', '1px', '.addPannal', '#addNewUserPannal');
                    BuildDataGrig();

                } else {
                    showMessage('Failed');
                }

            } else {
                showMessage('Password mismatch');
            }
        } else {
            showMessage('Username or Password is missing!!');
        }
    });
}

function AddNewUserSeverCall(userName, password) {
    var key = readCookie('token');
    var successfullyDone = false;
    var req = {
        username: userName,
        password: password

    };

    $.ajax({
        url: baseUrl + 'api/v2/user/register',
        type: 'POST',
        contentType: 'application/json;charset=utf-8',
        accepts: 'application/json',
        headers: {
            "Authorization": key,
        },
        async: false,
        data: JSON.stringify(req),
        //dataType: 'json',
        success: function (response) {

            successfullyDone = true;
        },
        error: function (response) {
            showMessage('Registration Faild');
            successfullyDone = false;
        },
    });
    return successfullyDone;

}

function UpdateUser(id, username, check) {
    var isActive;
    debugger;
    if (CompareTwoStrings(check, 'true')) {
        isActive = 1;
    } else {
        isActive = 0;
    }


    var req = {
        userID: id,
        userName: username,
        isActive: isActive
    };

    $.ajax({
        url: baseUrl + '/dashboard/updateUser',
        type: 'POST',
        contentType: 'application/json;charset=utf-8',
        accepts: 'application/json',
        async: false,
        data: JSON.stringify(req),
        dataType: 'json',
        success: function (response) {
            debugger;
            if (!(response == null || response.toString() == "")) {

            }

        },
        error: function (response) {
            showMessage('Ajax call faild');
        },
    });

}

function GetAllUser() {
    var key = readCookie('token');
    var users;
    $.ajax({
        url: baseUrl + 'api/v2/user/all',
        data: {
            format: 'json'
        },
        async: false,
        headers: {
            "Authorization": key,
        },
        error: function () {
            showMessage('An error has occurred');
        },
        dataType: 'json',
        success: function (data) {

            if (!(data == null || data == "")) {

                users = data;

            }
        },
        type: 'GET'
    });
    return users;
}

function DeleteUserPannalButtonClick() {
    debugger;
    $('#DeleteButton').click(function () {
        var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
        var id = $("#jqxgrid").jqxGrid('getrowid', selectedrowindex);
        var rowdata = $("#jqxgrid").jqxGrid('getrowdata', id);
        var username = rowdata.UserName;
        if (DeleteUser(username)) {
            showMessage('Successfully Deleted');
            BuildDataGrig();
            ClosePannal('500', '530', '1px', '1px', '.deleteConfirmation', '#deteteConfermationWindowPannal');
        }
    });

}


function DeleteUser(userName) {
    var key = readCookie('token');
    var successfullyDone = false;
    debugger;
    $.ajax({
        url: baseUrl + 'api/v2/user/' + userName,

        headers: {
            "Authorization": key,
        },
        async: false,
        error: function (responce) {
            showMessage('Operation Faild');
            successfullyDone = false;
        },

        success: function (data) {
            successfullyDone = true;

        },
        type: 'DELETE'
    });
    return successfullyDone;
}

function ChangePasswordButtonClick() {
    $("#EditPannalSaveButton").click(function () {
        debugger;
        var offset = $("#jqxgrid").offset();
        var newPassword = $("#ChangepasswordTextbox").val();
        var retypePassword = $("#RetypepasswordTextbox").val();
        var str = $('#EditPannalUsername').text();
        var username = str.substring(10);

        if (CompareTwoStrings(newPassword, retypePassword)) {


            if (ChangePasswordServerCall(newPassword, username)) {
                showMessage('Successfully Changed');
                $("#ChangepasswordTextbox").val('');
                $("#RetypepasswordTextbox").val('');
            }

        } else {
            showMessage('Password Mismatch');
            $("#ChangepasswordTextbox").val('');
            $("#RetypepasswordTextbox").val('');
        }

    });
}

function ChangePasswordServerCall(newPassword, userName) {
    debugger;
    var key = readCookie('token');
    var IsChanged = false;
    var md5NewPass = $.md5(newPassword);

    var req = {
        username: userName,
        password: md5NewPass
    };




    $.ajax({
        url: baseUrl + 'api/v2/user/password',
        type: 'PUT',
        contentType: 'application/json;charset=utf-8',
        accepts: 'application/json',
        headers: {
            "Authorization": key,
        },
        async: false,
        data: JSON.stringify(req),
        success: function (response) {
            IsChanged = true;
        },
        error: function (response) {
            showMessage('Operation faild');
            IsChanged = false;
        },
    });
    return IsChanged;
}

function UpdateUserStatusButtonClick() {
    $('#StatusButton').click(function () {
        debugger;
        var statusText = $('#editpannalUserstatus').text();
        var reqiredStatus;
        if (CompareTwoStrings(statusText, 'Active')) {
            reqiredStatus = 0;
        } else reqiredStatus = 1;

        var str = $('#EditPannalUsername').text();
        var username = str.substring(10);
        if (username != 'webadmin') {
            if (UpdateUserStatusServerCall(username, reqiredStatus)) {
                ChangeEditPannalUserStausUi(reqiredStatus);
            }
        }
    });


}

function UpdateUserStatusServerCall(userName, status) {
    debugger;
    var key = readCookie('token');
    var IsChanged = false;


    var req = {
        username: userName,
        isActive: status
    };




    $.ajax({
        url: baseUrl + '/api/v2/user/active',
        type: 'PUT',
        headers: {
            "Authorization": key,
        },
        contentType: 'application/json;charset=utf-8',
        accepts: 'application/json',
        async: false,
        data: JSON.stringify(req),
        success: function (response) {
            IsChanged = true;
        },
        error: function (response) {
            showMessage('Operation faild');
            IsChanged = false;
        },
    });
    return IsChanged;
}

function EditPannalOkButtonClick() {
    $('#OkButton').click(function () {
        debugger;
        ClosePannal('500', '530', '1px', '1px', '.editPannal', '#UserEditPannal');
        BuildDataGrig();
    });
}

function GetLoginSummaryandPapulate(userName, status) {
    debugger;
    $("#LoginHistoryTable").empty();
    $('#SummeryPannal-userName').text('Username: ' + userName);
    $('#SummeryPannal-Status').text('Status: ' + status);
    var d = GetLoginSummaryServerCall(userName);
    // var CompleteSummary = JSON.parse(d);
    var j = 1;
    for (var k in d) {
        if (d.hasOwnProperty(k)) {
            $("#LoginHistoryTable").append('<tr><td>' + j + '</td><td>' + d[k] + '</td></tr>');
            j++;
        }
    }
}

function GetLoginSummaryServerCall(userName) {
    var key = readCookie('token');
    var successfullyDone = '';

    $.ajax({
        url: baseUrl + 'api/v2/user/' + userName + '/login/history',
        data: {
            format: 'json'
        },

        headers: {
            "Authorization": key,
        },
        async: false,
        error: function () {
            showMessage('An error has occurred');
            successfullyDone = '';
        },
        success: function (data) {

            if (!(data == null || data == "")) {
                successfullyDone = data;

            }
        },
        type: 'GET'
    });
    return successfullyDone;
}

function ClearHistoryButtonClick() {
    debugger;
    $('#CrearSummaryButton').click(function () {
        debugger;
        var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
        var id = $("#jqxgrid").jqxGrid('getrowid', selectedrowindex);
        var rowdata = $("#jqxgrid").jqxGrid('getrowdata', id);
        var username = rowdata.UserName;
        if (ClearLoginHistory(username)) {
            $('#LoginHistoryTable').empty();
            showMessage('History Cleared');

        }
    });
}

function ClearLoginHistory(userName) {
    var key = readCookie('token');
    var successfullyDone = false;
    debugger;
    $.ajax({
        url: baseUrl + 'api/v2/user/' + userName + '/login/history',
        async: false,
        headers: {
            "Authorization": key,
        },
        error: function (error) {
            showMessage('An error has occurred');
            successfullyDone = false;
        },
        success: function (data) {

           
                successfullyDone = true;

            
        },
        type: 'DELETE'
    });
    return successfullyDone;
}
//End User Related functions


//Build and Papulate GiridView
function BuildDataGrig() {
    var data = GetAllUser();
    var source =
    {
        localdata: data,
        datatype: "array",
        datafields:
        [
            { name: 'UID', type: 'int' },
            { name: 'UserName', type: 'string' },
            { name: 'IsActive', type: 'boolean' }

        ],
        addrow: function (rowid, rowdata, position, commit) {
            // synchronize with the server - send insert command
            // call commit with parameter true if the synchronization with the server is successful 
            //and with parameter false if the synchronization failed.
            // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
            commit(true);
        },
        updaterow: function (rowid, rowdata, commit) {

            // synchronize with the server - send update command
            // call commit with parameter true if the synchronization with the server is successful 
            // and with parameter false if the synchronization failder.
            UpdateUser(rowdata.UID, rowdata.UserName, rowdata.IsActive);
            commit(true);
        },
        deleterow: function (rowdata, commit) {



            // synchronize with the server - send delete command
            // call commit with parameter true if the synchronization with the server is successful 
            //and with parameter false if the synchronization failed.

            commit(true);
        }


    };
    var photorenderer = function (row, column, value) {
        var name = $('#jqxgrid').jqxGrid('getrowdata', row).FirstName;
        var imgurl = '/Images/deleteButton.png';
        var img = '<div style="background: transparent;"><img style="" width="25" height="27" src="' + imgurl + '"></div>';
        return img;
    };
    // initialize the input fields.

    $("#uID").jqxInput({ theme: theme });
    $("#userName").jqxInput({ theme: theme });
    $("#isActive").jqxInput({ theme: theme });

    $("#uID").width(100);
    $("#uID").height(23);
    $("#userName").width(150);
    $("#userName").height(23);
    $("#isActive").width(100);
    $("#isActive").height(23);
    var dataAdapter = new $.jqx.dataAdapter(source);
    var editrow = -1;
    // initialize jqxGrid
    $("#jqxgrid").jqxGrid(
    {
        width: 650,
        source: dataAdapter,
        showtoolbar: true,
        rendertoolbar: function (toolbar) {
            var container = $("<div style='overflow: hidden; position: relative; margin: 5px;'></div>");
            toolbar.append(container);

            var addButton = $("<div style='float: left; margin-left: 5px;'><img style='position: relative; margin-top: 2px;' src='/Images/add.png'/><span style='margin-left: 4px; position: relative; top: -3px;'>Add</span></div>");
            var deleteButton = $("<div style='float: left; margin-left: 5px;'><img style='position: relative; margin-top: 2px;' src='/Images/close.png'/><span style='margin-left: 4px; position: relative; top: -3px;'>Delete</span></div>");
            var reloadButton = $("<div style='float: left; margin-left: 5px;'><img style='position: relative; margin-top: 2px;' src='/Images/refresh.png'/><span style='margin-left: 4px; position: relative; top: -3px;'>Reload</span></div>");
            var loginSummaryButton = $("<div style='float: left; margin-left: 5px;'><img style='position: relative; margin-top: 0px;' src='/Images/Summary.png'/><span style='margin-left: 4px; position: relative; top: -5px;'>Summary</span></div>");
            container.append(addButton);
            container.append(deleteButton);
            container.append(reloadButton);
            container.append(loginSummaryButton);
            toolbar.append(container);
            addButton.jqxButton({ width: 60, height: 20 });
            deleteButton.jqxButton({ width: 65, height: 20 });
            reloadButton.jqxButton({ width: 65, height: 20 });
            loginSummaryButton.jqxButton({ width: 90, height: 20 });
            // add new row.
            addButton.click(function (event) {
                //  var datarow = generatedata(1);
                //  $("#jqxgrid").jqxGrid('addrow', null, datarow[0]);
                Showpannal('100px', '430px', '240px', '200px', '.addPannal', "#addNewUserPannal");
            });
            // delete selected row.
            deleteButton.click(function (event) {
                debugger;

                var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');

                //var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;

                var id = $("#jqxgrid").jqxGrid('getrowid', selectedrowindex);
                var rowdata = $("#jqxgrid").jqxGrid('getrowdata', id);

                $('#UserToDelete').text('Are Sure to Delete Mr: ' + rowdata.UserName);
                Showpannal('100', '530', '250px', '200px', '.deleteConfirmation', '#deteteConfermationWindowPannal');



                //  $("#jqxgrid").jqxGrid('deleterow', rowdata);

            });
            // reload grid data.
            reloadButton.click(function (event) {
                BuildDataGrig();
            });
            // show Login summary.
            loginSummaryButton.click(function (event) {

                var offset = $("#jqxgrid").offset();
                var selectedrowindex = $("#jqxgrid").jqxGrid('getselectedrowindex');
                var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;
                var id = $("#jqxgrid").jqxGrid('getrowid', selectedrowindex);
                var rowdata = $("#jqxgrid").jqxGrid('getrowdata', id);
                var status;
                if (rowdata.IsActive) status = 'Active';
                else status = 'Blocked';



                GetLoginSummaryandPapulate(rowdata.UserName, status);
                Showpannal('76px', '430px', '470px', '450px', '.summeryPannal', "#AccountSummaryPannal");

            });
        },
        pageable: true,
        autoheight: true,
        columns: [
          { text: 'ID', datafield: 'UID', width: 150 },
          { text: 'User Name', datafield: 'UserName', width: 200 },
          { text: 'IsActive', datafield: 'IsActive', width: 200 },
          {
              text: '', width: 100, datafield: 'Edit', columntype: 'button', cellsrenderer: function () {
                  return "Edit";
              }, buttonclick: function (row) {
                  // open the popup window when the user clicks a button.
                  editrow = row;
                  var offset = $("#jqxgrid").offset();

                  // get the clicked row's data and initialize the input fields.
                  var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', editrow);
                  //$("#uID").val(dataRecord.uID);
                  $("#UserName").val(dataRecord.UserName);
                  $("#IsActive").val(dataRecord.IsActive);
                  PapulateEditPannalData(dataRecord.UserName, dataRecord.IsActive);

                  // show the popup window.
                  Showpannal('100px', '430px', '509px', '280px', '.editPannal', "#UserEditPannal");
                  //  $("#popupWindow").jqxWindow('open');
              }
          }



        ]
    });



}
//End Build and Papulate GiridView


//Mannage Edit pannal
function PapulateEditPannalData(userName, isActive) {
    $('#EditPannalUsername').text('Username: ' + userName);
    ChangeEditPannalUserStausUi(isActive);

}
//End Mannage Edit pannal


//Ui Control Functions

function CloseEditPannalButtonClick() {
    $('#CrossButtonPannel').click(function () {
        ClosePannal('500', '530', '1px', '1px', '.editPannal', '#UserEditPannal');
    });
}

function closeAddPannalButtonClick() {
    $('#CrossButtonAddPannel').click(function () {
        ClosePannal('500', '530', '1px', '1px', '.addPannal', '#addNewUserPannal');
    });
}

function closeSummaryPannalButtonClick() {
    $('#CrossButtonSummaryPannel').click(function () {
        ClosePannal('500', '530', '1px', '1px', '.summeryPannal', '#AccountSummaryPannal');
    });
}

function CloseDeletePannalClick() {
    $('#CrossButtonDelete').click(function () {
        ClosePannal('500', '530', '1px', '1px', '.deleteConfirmation', '#deteteConfermationWindowPannal');
    });
}

function CancelButtonDeletePannalClick() {
    $('#CancelButtonDeletePannal').click(function () {
        debugger;

        ClosePannal('500', '530', '1px', '1px', '.deleteConfirmation', '#deteteConfermationWindowPannal');
    });
}
function CancelButtonSummaryPannalClick() {
    $('#CancelButtonSummaryPannal').click(function () {
        debugger;

        ClosePannal('500', '530', '1px', '1px', '.summeryPannal', '#AccountSummaryPannal');
    });
}


function Showpannal(Top, Left, Width, Height, DivClass, DivToAnimate) {
    debugger;
    $(DivToAnimate).css("paddingLeft", "20px");
    $(DivClass).show();
    $(DivToAnimate).animate({
        left: Left,
        top: '170',
        opacity: '1',
        height: Height,
        width: Width

    }, "slow");
}
function ClosePannal(Top, Left, Width, Height, DivClass, DivToAnimate) {
    debugger;
    $(DivToAnimate).animate({
        left: Left,
        top: Top,
        opacity: '1',
        height: Height,
        width: Width
    }, function () {
        $(DivClass).hide();
    });
    //$('#UserEditPannal').hide();

}


function ChangeEditPannalUserStausUi(isActive) {
    if (isActive) {
        $('#editpannalUserstatus').text('Active');
        $('#editpannalUserstatus').css('color', '#6aba25');
        $('#StatusButton').text('Lockout');
    } else {
        $('#editpannalUserstatus').text('Blocked');
        $('#editpannalUserstatus').css('color', 'red');
        $('#StatusButton').text('UnBlock');
    }
}
//End Ui Control Functions

//Internal functions
function CompareTwoStrings(str1, str2) {
    var check = true;
    for (var i = 0; i < str1.length; i++) {
        if (str1[i] != str2[i]) {
            check = false;
            break;
        }

    }
    return check;
}
function IsNullOrEmpty(parm) {
    debugger;
    if (parm == undefined) {
        showMessage('undefined');
    }
    else if (CompareTwoStrings(parm, '')) {
        showMessage('empty');
    }

}
//End Internal functions


//Manage cookies
function createCookie(name, value) {
    if (name) {
        var date = new Date();
        date.setTime(date.getTime() + (20 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    } else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function refreshCookie() {
    createCookie('token', readCookie('token'));
}
//End manage cookies


//messageBox
function showMessage(message) {
    $('#MessageBox').text(message);
    $('#MessageBox').slideDown('slow', function () {
        setTimeout("jQuery('#MessageBox').slideUp('slow');", 2000);
    });
}
//EndMessagebox