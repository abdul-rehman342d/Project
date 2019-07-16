<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PowerfulPalDashboard.aspx.cs" Inherits="PowerfulPal.Neeo.Dashboard.PowerfulPalDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PowerfulPal Neeo Dashboard</title>
    <script src="Script/Chart.js"></script>
    <script src="Script/jquery-1.11.2.min.js"></script>
    <script src="Script/dashboardScript.js"></script>
    
    <link href="CSS/DashboardStyle.css" rel="stylesheet" />
    


</head>
<body>
    <form id="form1" runat="server" autocomplete="on">
     <div id="rapper">
         <div id="header">
             <table id="main-header-table" style="width: 100%; height: 100%">
                 <tr><td class="dashboard-column-size" ></td>
                     <td class="dashboard-column-size" id="dashboard-logo-column"><div id="dashboard-logo"></div></td>
                     <td class="dashboard-column-size" ></td>
                     <td id="dashboard-username-column">
                         <div id="user-sign"></div>  
                         <div id="user-dropdown-div" >
                             <select id="user-dropdown" >
                                 <option selected="selected" value="username" >User Name</option>
                                 <option value="Sign_Out" >Sign Out</option>
                             </select>
                         </div> 
                     </td>
                     <td class="dashboard-column-size"></td>
                 </tr>
             </table>  
         </div>
         <div id="inner_rapper">
              <div id="inner_header">
                <div id="inner-header-all-count"><div id="inner-header-all-count-value-div"><label id="inner-header-all-count-value">0</label></div></div>
                <div id="inner-header-ios-count"><div id="inner-header-ios-count-value-div"><label id="inner-header-ios-count-value">0</label></div></div>
                <div id="inner-header-android-count"><div id="inner-header-android-count-value-div"><label id="inner-header-android-count-value">0</label></div></div>
            </div>
              <div id="left_portion">
                   <div id="left_sub_all_part">
                       <div id="left-sub-all-part-header"><div id="all-heading" class="left-headings">All</div></div>
                       <div id="left_sub_all_part-mainRows" style="float: left;width: 63%;height: 70%;background-color: whitesmoke">
                          <div id="r-all-1" class="leftsideinnerRowsT">Country</div> 
                          <div id="r-all-2"class="leftsideinnerRowsm">Heading 2</div> 
                          <div id="r-all-3"class="leftsideinnerRowsB">Heading 3</div> 
                       </div>
                       <div id="left-all-valueBox" style="margin-left: 70%;margin-top: 5%;padding-top: 7px; width: 26%;height: 55%;background-color: rgb(212, 215, 208);">
                           <div id="all-box-value1" class="box-sub">0</div>
                           <div id="all-box-value2"class="box-sub"></div>
                           <div id="all-box-value3"class="box-sub"></div>
                       </div>

                   </div>
                    <div id="left_sub_ios_part">
                       <div id="left-sub-ios-part-header"><div id="ios-heading" class="left-headings">IOS</div></div>
                         <div id="left_sub_ios_part-mainRows" style="float: left;width: 63%;height: 70%;background-color: whitesmoke">
                          <div id="r-ios-1" class="leftsideinnerRowsT"> <label>Country</label></div> 
                          <div id="r-ios-2"class="leftsideinnerRowsm"><label>Heading 2</label></div> 
                          <div id="r-ios-3"class="leftsideinnerRowsB"><label>Heading 3</label></div> 
                       </div>
                       <div id="left-ios-valueBox" style="margin-left: 70%;margin-top: 5%;padding-top: 7px;width: 26%;height: 55%;background-color: rgb(212, 215, 208);">
                           <div id="ios-box-value1" class="box-sub">0</div>
                           <div id="ios-box-value2"class="box-sub"></div>
                           <div id="ios-box-value3"class="box-sub"></div> 
                       </div>
                   </div>
                   <div id="left_sub_android_part">
                       <div id="left-sub-android-part-header"><div id="android-heading" class="left-headings">Android</div></div>
                        <div id="left_sub_android_part-mainRows" style="float: left;width: 63%;height: 70%;background-color: whitesmoke">
                          <div id="r-an-1" class="leftsideinnerRowsT">Country</div> 
                          <div id="r-an-2"class="leftsideinnerRowsm">Heading 2</div> 
                          <div id="r-an-3"class="leftsideinnerRowsB">Heading 3</div> 
                       </div>
                       <div id="left-android-valueBox" style="margin-left: 70%;margin-top: 5%;padding-top: 7px;width: 26%;height: 55%;background-color: rgb(212, 215, 208);">
                            <div id="an-box-value1" class="box-sub">0</div>
                           <div id="an-box-value2"class="box-sub"></div>
                           <div id="an-box-value3"class="box-sub"></div>
                       </div>
                   </div>
              </div>
              <div id="right_portion">
                   <div id="graph-portion">
                       <div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div><div id="total-count-label"><label>Total Downloads</label></div></div>
                          <div style="width: 50%; height:60%; float: left;">
			                  <canvas id="canvas" height="200" width="200"></canvas>
		                 </div>
                         <div id="canvas-holder" class="GridViewCountryName">
			                <canvas id="chart-area" width="165" height="160" />
		                 </div>
                       <div id="Grid_header">
                        <%--   <table id="grid_header"><tr ><th>Number</th><th>Country</th><th>Downloads</th></tr></table>--%>
                           <div style="width:14%;float: left; ">S/N</div> 
                           <div style="float: left; margin-left: 3%; width: 20%;">Country</div>
                           <div style="float: left;margin-left: 19%;width: 10%;">Downloads</div>
                           <div style="margin-left: 78%;width: 10%;">%age</div>
                       </div>
                       <div id="gridPortion" >
                       <table id="grid_view" class="TFtable" >
                               
                           
                               </table>
                       </div>
                   </div>
              </div>
         </div>
     </div>
    </form>
</body>
</html>
