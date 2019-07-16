



function preloadFunc() {

  

    if (IsLoggedIn()) {
        var x = readCookie('user');
        if (x == 'webadmin') {
            window.location.replace("/webadmin.html");
        } else {
            window.location.replace("/Dashboard-v1.html");
        }

    }
    

}
window.onpaint = preloadFunc();





$(document).ready(function () {
    
    Register_form_events();
     var remember_me = readCookie("remember_me");
    if (remember_me != undefined) {
        if (remember_me =='true') {
            var pass = readCookie("password");
            $("#UserName_Input").val(readCookie('username'));
            $("#Password_Input").val(readCookie('password'));
            $("#remember_me_Checkbox").prop('checked', true);
        }
  
    }
});

//Login related
function Login() {
    

    
    if (ValidateUserNameAndPassword()) {
        $('#loaderRapper').show();
        var md5Pass = $.md5($("#Password_Input").val());
        var userName = $("#UserName_Input").val();
        var req = {
            username: userName,
            password: md5Pass
        };


        $.ajax({
            url: baseUrl + 'api/v2/user/login',
            type: 'POST',
            contentType: 'application/json;charset=utf-8',
            accepts: 'application/json',
            data: JSON.stringify(req),
            async: true,
            dataType: 'json',
            success: function (response) {
                
                if (!(response == null || response.toString() == "")) {

                     if ($("#remember_me_Checkbox").is(':checked')) {
                        
                        createCookie("remember_me", 'true');
                        createCookie("username", userName);
                        createCookie("password", $("#Password_Input").val())
                        createCookie("LastLoginTime", response.LastLoginTime);
                        createCookie("LastSyncTime", response.LastSyncTime);
                    }
                    else {
                        createCookie("remember_me", 'false');
                        createCookie("username", userName);
                        createCookie("LastLoginTime", response.LastLoginTime);
                        createCookie("LastSyncTime", response.LastSyncTime);
                        //$("#remember_me_Checkbox").prop('checked', false);
                    }

                    createCookie("token",'ndp '+ response.AuthKey.toString().toString());
                    if (userName == 'webadmin') {
                        createCookie("user", 'webadmin');
                        window.location.replace("/WebAdmin.html");
                    } else {
                        createCookie("user", 'other');
                       
                        window.location.replace("/Dashboard-v1.html");
                
                    }

                } else {
                    $('#loaderRapper').hide();
                    showMessage('Username or Password is incorrect');
                    
                }

            },
            error: function (response) {
                
                $('#loaderRapper').hide();
                switch (response.status) {
                    case 404:
                        showMessage("Invalid User!");
                        break;
                    case 400:
                        showMessage("Invalid username or password!");
                        break;
                    default:
                        showMessage("Request failed!");
                        break;
                }
            }
        });
    }
}


// End Login related

//Manage key press events

function Register_form_events() {
    $(document).bind('keypress', function (e) {
        if (e.which === 13) { // return
            $('#login_buttion').trigger('click');
        }
    });

    $("#remember_me_Checkbox").change(function () {
        if (!this.checked) {
            eraseCookie("username");
            eraseCookie("password");
        }
    });

    $('#Remember_me').click(function () {
        
        if ($("#remember_me_Checkbox").is(':checked')) {
            $("#remember_me_Checkbox").prop('checked', false);
        } else { $("#remember_me_Checkbox").prop('checked', true); }
    });
}
//End Manage key press events



//Manage Validation
function ValidateUserNameAndPassword() {
    var userName = $("#UserName_Input").val();
    var password = $("#Password_Input").val();

    if (!(userName == "" || password == "")) {
        return true;
    }
    else {
        $('#loaderRapper').hide();
        showMessage('Username or password is missing !! ');
        return false;
    }
}
//End Manage Validation

//messageBox
function showMessage(message) {
    
    $('#MessageBox').text(message);
    $('#MessageBox').slideDown('slow', function () {
        setTimeout("jQuery('#MessageBox').slideUp('slow');", 2000);
    });
}
//EndMessagebox