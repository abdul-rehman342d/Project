
//var baseUrl = 'http://services.neeopal.com:9085/';
//var baseUrl = 'http://192.168.2.113:9085/';
//var baseUrl = 'http://localhost:5030/';
var baseUrl = 'http://nsvc-dashboard.neeopal.com/';



function IsLoggedIn() {
    
    var token = readCookie("token");
    if (!(token == null || token == '')) {
        var x = readCookie("user");
        if (readCookie("user") != 'webadmin') {
            if (checkSessionExistanc(token))
            {

                return true;
            }
            else
            { eraseCookie('token'); eraseCookie('user'); return false; }
        }

        else {
            eraseCookie('token'); eraseCookie('user');
            eraseCookie("LastLoginTime");
            eraseCookie("LastSyncTime");
            return false;
        }
    }
    else { return false; }
}

function checkSessionExistanc(key) {
    
    
    returnVal = 0;
    $.ajax({
        url: baseUrl + 'api/v2/user/login/',
        type: 'Get',
        contentType: 'application/json;charset=utf-8',
        headers: {
            "Authorization": key,
        },
        async: false,
        dataType: 'json',
        success: function (response) {
            
            if (!(response == null || response.toString() == "")) {
			var a=readCookie('username');
                if (a == response.Username) {
                    createCookie("LastLoginTime", response.LastLoginTime);
                    createCookie("LastSyncTime", response.LastSyncTime);
                    returnVal= 1;
                }
                else returnVal= 0;
                //returnVal = response;
               
            }
            else {
                returnVal = 0;
            }

        },
        error: function (response) {
            showMessage('Ajax call faild');
        },
    });
    return returnVal;
}


//Manage cookies
function createCookie(name, value) {
    if (name) {
        var date = new Date();
        //for two months
        date.setTime(date.getTime() + (30*60 * 60 * 24 * 1000));
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

