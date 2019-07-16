<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NeeoServiceOperator.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Neeo Portal</title>

    <style type="text/css">
        body {
            margin: 5px;
            font-family: 'Tangerine';
        }

        .page {
            width: 100%;
            height: 100%;
            min-height: 600px;
            min-width: 500px;
            position: relative;
        }

        .banner {
            width: 100%;
            height: 100px;
            background: #6aba19;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            position: relative;
        }

        .container {
            width: 100%;
            height: 100px;
        }

        .center {
            width: 400px;
            height: 225px;
            border: 1px solid whitesmoke;
            position: absolute;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            margin: auto;
            box-shadow: lightgray 7px 7px 5px;
        }

        .header {
            background-color: #6aba19;
            text-align: center;
            -moz-border-radius: 7px;
            -webkit-border-radius: 7px;
            border-radius: 7px;
        }

        input[type=text], input[type=password]{

            width: 200px;
            height: 18px;
            border-width: 1px;
            border-style: solid;
            border-color: lightgray;
            transition: 0.5s linear;
            -webkit-transition: 0.5s linear;
            -moz-transition: 0.5s linear;

        }

        input[type=text]:focus, input[type=password]:focus{
            border-color: #6aba19;
            box-shadow: #6aba19 0px 0px 10px;
        }

        /*input[type=submit]:hover {
            border-color: #6aba19;
            border-width: 1px;
            border-style: solid;
            border-color: #6aba19 ;
            
           /*box-shadow: #6aba19 5px 5px 30px 20px inset;*/
            
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">
            <div class="banner">
                <div style="position: absolute; margin-top: 5px;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img style="vertical-align: middle;" height="90px" width="90px" src="Images/icon@2x.png" />
                </div>
                <div style="display: inline-block; margin-top: 8px; vertical-align: middle; text-align: center; width: 100%;">
                    <h1 style="color: white;">Neeo Portal</h1>
                </div>
            </div>
            <div class="container">
                <div class="center">
                    <div class="header" style="width: 98%; margin: 5px;">
                        <div style="color: white; font-size: 24px; font-weight: bolder; padding: 5px 0px 5px 0px;">Login</div>
                    </div>
                    <div style="margin: 5px; ">
                        <div style="text-align: center; height: 20px; padding-top: 10px; margin-bottom: 15px;">
                            <asp:Label ID="lblErroMessage" runat="server" EnableViewState="false" Visible="False" ForeColor="Red" Text="Label"></asp:Label>
                        </div>

                        <div style="padding-left: 5%; padding-right: 5%;">
                            <div style="width: 100px; display: inline-block">
                                <label>Username</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtUsername" runat="server" Width="200px"></asp:TextBox>
                            </div>

                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>Password</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" Width="200px"></asp:TextBox>
                            </div>
                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>&nbsp;</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
