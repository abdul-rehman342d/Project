<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NeeoServiceOperator.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Neeo Portal</title>
    <script src="Content/jquery-2.1.1.js"></script>
   
    <style type="text/css">
        .banner {
            width: 100%;
            height: 100px;
            background: #6aba19;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            position: relative;
        }

        .header {
            background-color: #6aba19;
            text-align: center;
            -moz-border-radius: 7px;
            -webkit-border-radius: 7px;
            border-radius: 7px;
            color: white;
        }

        .grid-header {
            background-color: #6aba19;
            text-align: center;
            -moz-border-radius: 7px;
            -webkit-border-radius: 7px;
            border-radius: 7px;
            color: white;
            font-size: 20px;
            border-right: 1px solid #6aba30;
        }

        .container {
            background-color: #6aba19;
            -moz-border-radius: 7px;
            -webkit-border-radius: 7px;
            border-radius: 7px;
            color: white;
        }

        tr {
            -moz-border-radius: 7px;
            -webkit-border-radius: 7px;
            border-radius: 7px;
        }

        .block {
            border: 1px solid whitesmoke;
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

        h3 {
            padding: 0px 0px 0px 0px;
            margin: 0px 0px 0px 0px;
        }

        input[type=text], input[type=password] {
            width: 200px;
            height: 18px;
            border-width: 1px;
            border-style: solid;
            border-color: lightgray;
            transition: 0.5s linear;
            -webkit-transition: 0.5s linear;
            -moz-transition: 0.5s linear;
        }

            input[type=text]:focus, input[type=password]:focus {
                border-color: #6aba19;
                box-shadow: #6aba19 0px 0px 10px;
            }

        /*input[type=submit] {
            border-color: #6aba19;
            height: 18px;
            width: 50px;
            border-width: 1px;
            border-style: solid;
            border-color: lightgray;

            
        }
        input[type=submit]:hover {
            border-color: #6aba19;
            border-width: 1px;
            border-style: solid;
            border-color: #6aba19 ;
            opacity: .7;
            box-shadow: #6aba19 10px 10px 30px 20px inset;
            
        }*/
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 100%;">
            <div class="banner">
                <div style="position: absolute; margin-top: 5px;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img style="vertical-align: middle;" height="90px" width="90px" src="Images/icon@2x.png" />
                </div>
                <div style="display: inline-block; margin-top: 8px; vertical-align: middle; text-align: center; width: 100%;">
                    <h1 style="color: white;">Neeo Portal</h1>
                </div>
            </div>
            <div style="width: 40%; position: fixed; padding: 5px; top: 120px; right: 0px; background-color: white; z-index: 99; border: lightgray solid 1px; margin: 10px; -moz-border-radius: 10px; -webkit-border-radius: 10px; border-radius: 10px; box-shadow: lightgray 5px 5px 5px;">
                <div style="padding: 5px;">
                    <asp:Button Style="position: absolute; z-index: 100; left: 13px; top: 3px;" ID="lnkReset" runat="server" OnClick="btnReset_Click" Text="Reset All Fields" />
                    <asp:LinkButton Style="position: absolute; z-index: 100; right: 13px; top: 3px; height: 19px;" ID="lnkBtnLogout" runat="server" OnClick="lnkBtnLogout_Click">Logout</asp:LinkButton>
                </div>

                <div style="margin: 15px 0px 15px 0px;">
                    <div class="header" style="width: 98%; margin: 5px 5px 15px 5px;">
                        <div style="padding: 5px 0px 5px 0px;">
                            <h3>Search User Status</h3>
                        </div>
                    </div>
                    <div style="margin: 5px;">
                        <div style="padding-left: 5%; padding-right: 5%;">
                            <div style="width: 100px; display: inline-block">
                                <label>Phone Number</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtSearchPhoneNumber" runat="server"></asp:TextBox>
                            </div>
                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>&nbsp;</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                                <%--<asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" />--%>
                            </div>
                        </div>
                    </div>
                </div>

                <div style="margin: 0px 0px 15px 0px;">
                    <div class="header" style="width: 98%; margin: 5px 5px 15px 5px;">
                        <div style="padding: 5px 0px 5px 0px;">
                            <h3>Change User Password</h3>
                        </div>
                    </div>
                    <div style="margin: 5px;">
                        <div style="padding-left: 5%; padding-right: 5%;">
                            <div style="width: 100px; display: inline-block">
                                <label>Username</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtPhoneNumber" runat="server" Enabled="false" ReadOnly="true"></asp:TextBox>
                            </div>

                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>Password</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
                            </div>
                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>&nbsp;</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" OnClick="btnChangePassword_Click" />
                            </div>
                            <br />
                            <asp:Label ID="lblStatus" runat="server" Text="" EnableViewState="false"></asp:Label>
                        </div>
                    </div>

                </div>

                <div>
                    <div class="header" style="width: 98%; margin: 5px 5px 15px 5px;">
                        <div style="padding: 5px 0px 5px 0px;">
                            <h3>Unblock Phone Number</h3>
                        </div>
                    </div>
                    <div style="margin: 5px;">
                        <div style="padding-left: 5%; padding-right: 5%;">
                            <div style="width: 100px; display: inline-block">
                                <label>Phone Number</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:TextBox ID="txtUnblockPhoneNumber" runat="server"></asp:TextBox>
                            </div>
                            <br />
                            <br />
                            <div style="width: 100px; display: inline-block">
                                <label>&nbsp;</label>
                            </div>
                            <div style="display: inline-block">
                                <asp:Button ID="btnUnblock" runat="server" Text="Un-Block" OnClick="btnUnblock_Click" />
                            </div>
                            <br />
                            <asp:Label ID="lblDeletionStatus" runat="server" EnableViewState="false" Text=""></asp:Label>
                        </div>
                    </div>

                </div>
            </div>
            <div style="top: 15px; position: relative; width: 50%;">
                <div class="header" style="width: 100%; margin: 5px 0px 15px 0px;">
                    <div style="padding: 5px 0px 5px 0px;">
                        <h3>All Users Status</h3>
                    </div>
                </div>
                <asp:GridView ID="GridView1" Width="100%" GridLines="None" runat="server" AutoGenerateColumns="False" DataKeyNames="username" AllowPaging="True" PageSize="50" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="username" HeaderText="User" ReadOnly="True" SortExpression="username" />
                        <asp:BoundField DataField="name" HeaderText="Name" SortExpression="name" />
                        <asp:ImageField DataImageUrlField="username" DataImageUrlFormatString="http://neeotest.neeopal.com:9090/plugins/presence/status?jid={0}@karzantest.net" HeaderText="Status">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:ImageField>
                        <asp:ImageField DataImageUrlField="username" DataImageUrlFormatString="http://neeotest.neeopal.com:9003/GetAvatar.ashx?uid={0}&amp;ts=0&amp;dim=100" HeaderText="Profile Image" NullDisplayText="No Image" DataAlternateTextFormatString="No Image" DataAlternateTextField="name">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:ImageField>
                        <asp:CommandField ShowSelectButton="True" HeaderText="Action">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:CommandField>
                    </Columns>
                    <HeaderStyle CssClass="grid-header" Height="40px" />
                    <RowStyle BackColor="whitesmoke" />

                    <AlternatingRowStyle BackColor="LightGray" />
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:XMPPDbConnectionString %>" SelectCommand="SELECT [username], [name] FROM [ofUser] WHERE (([username] &lt;&gt; @username) AND ([username] NOT LIKE '%' + @username2 + '%'))">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="admin" Name="username" Type="String" />
                        <asp:Parameter DefaultValue="+%" Name="username2" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:XMPPDbConnectionString %>" SelectCommand="SELECT [username], [name] FROM [ofUser] WHERE (([username] LIKE '%' + @username + '%') AND ([username] NOT LIKE '%' + @username2 + '%'))" OnSelected="SqlDataSource2_Selected">
                    <SelectParameters>
                        <asp:ControlParameter Name="username" ControlID="txtSearchPhoneNumber" PropertyName="Text" Type="String" />
                        <asp:Parameter DefaultValue="+%" Name="username2" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </div>
        <div id="voip_content" style="display:none">
            <iframe class="voip_users_data"  src="http://rtsip.neeopal.com/checksip.php"></iframe>
             <script>
                 $(document).ready(function () {
                     var i = $('#voip_content').find('html');
                 });
    </script>
        </div>
    </form>
</body>
</html>
