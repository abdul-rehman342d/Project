<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PowerfulPal.Neeo.Dashboard.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Powerfulpal Neeo Dashboard Login</title>
    <link href="CSS/LoginStyle.css" rel="stylesheet" />
    <script src="Script/jquery-1.11.2.min.js"></script>
    <script src="Script/jquery.md5.js"></script>
    <script src="Script/LoginScript.js"></script>
    
    
</head>
<body>
    <form id="loginform" runat="server">
      <div id="rapper" class="rapper">
         <div id="login_penal">
             <div id="logo_side">
                 <div id="logo"></div>
             </div>
             <div id="login_side">
             <table id="loginTable">
                 <tr><td><label id="UserName_lable">Username</label></td></tr>
                 <tr><td><input id="UserName_Input" type="text" maxlength="15" /></td></tr>
                  <tr><td style="height: 5px;"></td></tr>
                 <tr><td><label id="password_lable">Password</label></td></tr>
                 <tr><td><input id="Password_Input" type="password" maxlength="8" /></td></tr>
                 <tr><td style="height: 5px;"></td></tr>
                 <tr><td><button id="login_buttion" type="button" onclick="Login()" >Login</button></td></tr>
                 <tr><td><label id="reset_password">Reset Password</label></td></tr>

             </table>    
             </div>
         </div>
      </div>
    </form>
</body>
</html>
