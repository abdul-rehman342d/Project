

//Global variables
var dataFromServer;
var allCountList = [];
var iosCountList = [];
var androidCountList = [];

//End Global variables


//Execution of this funcion starts before web page elements loading
function preloadFunc() {

    if (!IsLoggedIn()) {
        window.location = "/Login.html";
    }

}
window.onpaint = preloadFunc();



//Execution of this funcion starts when all elements of web page are loaded
$(document).ready(function () {

    SetUserCredientials(readCookie('username'), readCookie('LastLoginTime'), readCookie('LastSyncTime'));
    GetDataFromServer(readCookie('token'));
   
    

});



//Main function portion

function mainFunction(dataFromServer) {
    HideLodder();


    //Server Call For Data
  


        $('#all-box-value1').text(dataFromServer.TotalCount);
        $('#inner-header-all-count-value').text(dataFromServer.TotalCount);
        $('#ios-box-value1').text(dataFromServer.Ios.TotalCount);
        $('#inner-header-ios-count-value').text(dataFromServer.Ios.TotalCount);
        $('#an-box-value1').text(dataFromServer.Android.TotalCount);
        $('#inner-header-android-count-value').text(dataFromServer.Android.TotalCount);




        allCountList = sortByAllCounts(GetAllCountriesArray(dataFromServer.CountryStats));
        iosCountList = GetIosCountriesArray(dataFromServer.CountryStats);
        androidCountList = GetAndroidCountriesArray(dataFromServer.CountryStats)



      
        ManageLast24HoursActiveUserCounts(dataFromServer.Last24HrActiveUsers, dataFromServer.Ios.Last24HrActiveUsers, dataFromServer.Android.Last24HrActiveUsers);
        FillCountryCountsInPlateformTabs(allCountList.length, iosCountList.length, androidCountList.length);
        StartupPageUI();
        manageAllCountAndPapulate(allCountList, dataFromServer.TotalCount);
        BuildDoughnutChartAllCount(allCountList, dataFromServer.TotalCount, 6, true);
        RegisterCountryTabClickEvents();
        RegisterHistoryPortionEnvents();
        MouseHoverFunctions();
        RegisterOnClickLogOutButtonEvent();
        AnnimateUIControls();
       
        RightUpperManuEvents();
        UpperManuMethodsCall();



    
}


function GetDataFromServer(key) {
  
    $.ajax({
        type: "GET",
        url: baseUrl + 'api/v2/dashboard/statistics',
        headers: {
            "Authorization": key,
        },
        async: true,
        dataType: "json",
        success: function (responce) {
            if (responce != undefined) {
                dataFromServer = responce;
                mainFunction(responce);
            }
            else { showMessage("Internal Server Error"); }
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


function UpdateGlobalVariables(all, and, ios) {
    allCountList = all;
    iosCountList = ios;
    androidCountList = and;

}



//End Main function portion

//Timer operations
function SetTimerForDBOperation(timeInterval) {

    setInterval(function () {

        manageCounts();

    }, timeInterval);
}

function SetTimerForClientSideOperation(timeInterval) {

}
//End Timer operations

//Manage Last24 Hours Active users
function ManageLast24HoursActiveUserCounts(all, ios, android) {

    $('#ios-box-value2').text(ios);
    $('#an-box-value2').text(android);
    $('#all-box-value2').text(all);

}
//End manage Last24 Hours Active users


//Manage Country Count in plateform tabs
function FillCountryCountsInPlateformTabs(allCountriesCount, iosCountriesCount, androidCountriesCount) {

    //papulation number of countries in the plateform tabs
    $("#all-box-value3").text(allCountriesCount);
    $("#ios-box-value3").text(iosCountriesCount);
    $("#an-box-value3").text(androidCountriesCount);
}
//End Manage Country Count in plateform tabs



//User Related
function RegisterOnClickLogOutButtonEvent() {

    $('#sign-out-button').click(function () {

        var key = readCookie('token');

        ShowLodder();
        $('html, body').css({
            'overflow': 'hidden',
            'height': '100%'
        });



        $.ajax({
            url: baseUrl + 'api/v2/user/' + readCookie('username') + '/logout/',
            type: 'PUT',
            contentType: 'application/json;charset=utf-8',
            headers: {
                "Authorization": key,
            },
            accepts: 'application/json',
            async: false,

            dataType: 'json',
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
                // showMessage('Ajax call faild');
                eraseCookie('token');
                eraseCookie('user');
                eraseCookie("LastLoginTime");
                eraseCookie("LastSyncTime");
                window.location.replace("/Login.html");
            },
        });

    });
}

function SetUserCredientials(username, LastLoginTime, LastSyncTime) {
    $("#last-login-time").text('Last Login: ' + LastLoginTime + ' (GMT)');
    $("#last-sync-time").text('Last Sync : ' + LastSyncTime + ' (GMT)');
    // $("#last-sync-time").text('Last Sync : ' + myDateFormatter(new Date(LastSyncTime).setMinutes(new Date(LastSyncTime).getMinutes() - 1)) + ' (GMT)');
    $('#last-sync-statement-time').text(myDateFormatter(new Date(LastSyncTime).setMinutes(new Date(LastSyncTime).getMinutes() - 1)) + ' ');

    $("#User-name-span").text(username);
}
//End User Related



function myDateFormatter(dateObject) {
    var d = new Date(dateObject);
    var day = d.getDate();
    var month = d.getMonth();
    var year = d.getFullYear();
    var hour = d.getHours();
    var minute = d.getMinutes();
    //if (day < 10) {
    //    day = "0" + day;
    //}
    //if (month < 10) {
    //    month = "0" + month;
    //}
    var date = day + " " + getMonthNameByNumber(month) + " " + year + " " + hour + ':' + minute;

    return date;
};

// Get Counts From Sever And Manage
function RegisterCountryTabClickEvents() {

    $('#r-all-1').click(function () {

        AllCountryTabSelected();
        manageAllCountAndPapulate(allCountList, dataFromServer.TotalCount);
        BuildDoughnutChartAllCount(allCountList, dataFromServer.TotalCount, 6, true);
        AnnimateUIControls();
        $('#right-upper-date').empty();
        $(".dashboardUpperManuSelectedTab").removeClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuRegistrationTab").addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
        $("#dashboardUpperManuLastPart").addClass($("#dashboardUpperManuQuarterlyTab").attr('class'));
        // ManageSizeBuildDoughnutChart();
    });

    $('#r-ios-1').click(function () {

        iosCountList = sortByIosCounts(iosCountList);
        IOSCountryTabSelected();
        manageIosCountAndPapulate(iosCountList, dataFromServer.Ios.TotalCount);
        BuildDoughnutChartIosCount(iosCountList, dataFromServer.Ios.TotalCount, 6, true);
        AnnimateUIControls();
        $('#right-upper-date').empty();
        $(".dashboardUpperManuSelectedTab").removeClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuRegistrationTab").addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
        $("#dashboardUpperManuLastPart").addClass($("#dashboardUpperManuQuarterlyTab").attr('class'));
        // ManageSizeBuildDoughnutChart();
    });

    $('#r-an-1').click(function () {

        androidCountList = sortByAndroidCounts(androidCountList);
        AndroidCountryTabSelected();
        manageAndroidCountAndPapulate(androidCountList, dataFromServer.Android.TotalCount);
        BuildDoughnutChartAndroidCount(androidCountList, dataFromServer.Android.TotalCount, 6, true);
        AnnimateUIControls();
        $('#right-upper-date').empty();
        $(".dashboardUpperManuSelectedTab").removeClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuRegistrationTab").addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
        $("#dashboardUpperManuLastPart").addClass($("#dashboardUpperManuQuarterlyTab").attr('class'));
        // ManageSizeBuildDoughnutChart();
    });
}


function manageAllCountAndPapulate(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = allCountList[i].TotalCount;

            if (i < 6) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}

function manageIosCountAndPapulate(iosCountList, totalDownloads) {

    if (!(iosCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < iosCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = iosCountList[i].CountryName;
            rowDataForTopCountryList[1] = iosCountList[i].Ios.TotalCount;

            if (i < 6) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}

function manageAndroidCountAndPapulate(androidCountList, totalDownloads) {

    if (!(androidCountList == undefined)) {

        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < androidCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = androidCountList[i].CountryName;
            rowDataForTopCountryList[1] = androidCountList[i].Android.TotalCount;

            if (i < 6) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}


//top country list
function manageTopCountryList(str, totalDownloads, i) {

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    if (str[0].length > 24) {
        $('#top-countries tbody').append('<tr><td><div style="width: 10px;height: 10px;background-color:' + colorCollection[i] + '; "></div></td>  <td><abbr title=\"' + str[0] + '\">' + (str[0].substring(0, 25)) + '..' + '</abbr></td><td>' + str[1] + '</td><td>' + ((parseInt(str[1]) / totalDownloads) * 100).toFixed(2) + '%' + '</td></tr>');
    }
    else {
        $('#top-countries tbody').append('<tr><td><div style="width: 10px;height: 10px;background-color:' + colorCollection[i] + '; "></div></td>  <td>' + str[0] + '</td><td>' + str[1] + '</td><td>' + ((parseInt(str[1]) / totalDownloads) * 100).toFixed(2) + '%' + '</td></tr>');
    }

}
//End top country list
//only for the first Time run
function FirstTimeManageIosCountAndPapulate(iosCountList) {
    if (!(iosCountList == undefined || iosCountList == "")) {

        var total = (iosCountList[0].split('::'))[1];
        $('#ios-box-value1').text(total);
        $('#inner-header-ios-count-value').text(total);
    }
}

function FirstTimeManageAndroidCountAndPapulate(androidCountList) {
    if (!(androidCountList == undefined || androidCountList == "")) {
        var total = (androidCountList[0].split('::'))[1];
        $('#an-box-value1').text(total);
        $('#inner-header-android-count-value').text(total);
    }
}
//End for first Time run
// End Get Counts From Sever And Manage


//Manage Charts

function BuildDoughnutChartAllCount(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];


    for (var i = 0; i <= numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length ; j++) {
                    sum += parseInt(list[j].TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}

function BuildDoughnutChartIosCount(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];


    for (var i = 0; i <= numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Ios.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length; j++) {
                    sum += parseInt(list[j].Ios.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}

function BuildDoughnutChartAndroidCount(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];


    for (var i = 0; i <= numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Android.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length; j++) {
                    sum += parseInt(list[j].Android.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}

function BuildDoughnutChart(doughnutData) {
    var ctx = document.getElementById("chart-area").getContext("2d");
    window.myDoughnut = new Chart(ctx).Doughnut(doughnutData, {
        percentageInnerCutout: 55
    });
}

function ManageSizeBuildDoughnutChart() {
    $('#chart-area').width('190px').height('190px');

}


function buildLineChart(inputDataArray, inputlableArray, lineColor, strokeHighlight) {

    //  var randomScalingFactor = function () { return Math.round(Math.random() * 100) };
    var lineChartData = {
        // labels: [getDatewithDaysDifference(4), getDatewithDaysDifference(3), getDatewithDaysDifference(2), getDatewithDaysDifference(1), getDatewithDaysDifference(0)],
        labels: inputlableArray,
        datasets: [

            {
                label: "My Second dataset",
                fillColor: "rgba(58,152,66,0.4)",
                //fillColor: "#36a9e1",
                strokeColor: lineColor,
                pointColor: lineColor,
                pointStrokeColor: lineColor,
                pointHighlightFill: lineColor,
                pointHighlightStroke: strokeHighlight,
                // data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                data: inputDataArray

            }
        ]
    }

    var ctx = document.getElementById("line-chart-canvas").getContext("2d");
    window.myLine = new Chart(ctx).Line(lineChartData, {
        responsive: false,
        ///Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,


        // String - Colour of the scale line
        scaleLineColor: "#808080",

        // Number - Pixel width of the scale line
        scaleLineWidth: 2,
        //String - Colour of the grid lines
        scaleGridLineColor: "#808080",

        //Number - Width of the grid lines
        scaleGridLineWidth: .5,

        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,

        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,

        //Boolean - Whether the line is curved between points
        bezierCurve: false,

        //Number - Tension of the bezier curve between points
        bezierCurveTension: 0.4,

        //Boolean - Whether to show a dot for each point
        pointDot: true,

        //Number - Radius of each point dot in pixels
        pointDotRadius: 4,

        //Number - Pixel width of point dot stroke
        pointDotStrokeWidth: 3,

        //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
        pointHitDetectionRadius: 20,

        //Boolean - Whether to show a stroke for datasets
        datasetStroke: true,

        //Number - Pixel width of dataset stroke
        datasetStrokeWidth: 3,
        responsive: false,

        //Boolean - Whether to fill the dataset with a colour
        datasetFill: false,

        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].strokeColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"


    });
}

function buildLineChartForAllPlateForms(inputAllDataArray, inputIosDataArray, inputAndroidDataArray, inputlableArray) {

    var allFillColor = "#4d5360";
    var allFillHighlight = "#303236";

    var iosFillColor = "#4386f4";
    var iosFillHighlight = "#17407d";

    var androidFillColor = "#34a853";
    var androidFillHighlight = "#318037";


    var lineChartData = {

        labels: inputlableArray,
        datasets: [


            {
                label: "My Second dataset",
                fillColor: "rgba(58,152,66,0.4)",
                //fillColor: "#36a9e1",
                strokeColor: allFillColor,
                pointColor: allFillColor,
                pointStrokeColor: allFillColor,
                pointHighlightFill: allFillColor,
                pointHighlightStroke: allFillHighlight,
                // data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                data: inputAllDataArray

            },
            {
                label: "My Second dataset",
                fillColor: "rgba(58,152,66,0.4)",
                //fillColor: "#36a9e1",
                strokeColor: iosFillColor,
                pointColor: iosFillColor,
                pointStrokeColor: iosFillColor,
                pointHighlightFill: iosFillColor,
                pointHighlightStroke: iosFillHighlight,
                // data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                data: inputIosDataArray

            },
            {
                label: "My Second dataset",
                fillColor: "rgba(58,152,66,0.4)",
                //fillColor: "#36a9e1",
                strokeColor: androidFillColor,
                pointColor: androidFillColor,
                pointStrokeColor: androidFillColor,
                pointHighlightFill: androidFillColor,
                pointHighlightStroke: androidFillHighlight,
                // data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                data: inputAndroidDataArray

            }
        ]
    }

    var ctx = document.getElementById("line-chart-canvas").getContext("2d");
    window.myLine = new Chart(ctx).Line(lineChartData, {
        responsive: false,
        ///Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,


        // String - Colour of the scale line
        scaleLineColor: "#808080",

        // Number - Pixel width of the scale line
        scaleLineWidth: 2,
        //String - Colour of the grid lines
        scaleGridLineColor: "#808080",

        //Number - Width of the grid lines
        scaleGridLineWidth: .5,

        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,

        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,

        //Boolean - Whether the line is curved between points
        bezierCurve: false,

        //Number - Tension of the bezier curve between points
        bezierCurveTension: 0.4,

        //Boolean - Whether to show a dot for each point
        pointDot: true,

        //Number - Radius of each point dot in pixels
        pointDotRadius: 4,

        //Number - Pixel width of point dot stroke
        pointDotStrokeWidth: 3,

        //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
        pointHitDetectionRadius: 20,

        //Boolean - Whether to show a stroke for datasets
        datasetStroke: true,

        //Number - Pixel width of dataset stroke
        datasetStrokeWidth: 3,
        responsive: false,

        //Boolean - Whether to fill the dataset with a colour
        datasetFill: false,

        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].strokeColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"


    });
}

//End Manage Charts

// History Portion Methods

function RegisterHistoryPortionEnvents() {
    HistoryButtonClick();
}

function HistoryButtonClick() {

    $('#inner-header-right-side').click(function () {

        var x = $('#inner-header-right-side').text();
        if ($('#inner-header-right-side').text() == 'View History') {
            $('#inner-lower-portion').hide();
            $('#History-portion').show();
            $('#inner_rapper').css('height', '801px');
            var recentHistory = dataFromServer.RecentHistory.WeeklyHistory;
            SetLineChartHeading(recentHistory[4].Date, recentHistory[0].Date);
            OnDropdownOptionSelected();
            $('#inner-header-right-side').text('View Dashboard');
        }
        else {
            $('#inner-lower-portion').show();
            $('#History-portion').hide();
            $('#inner-header-right-side').text('View History');
            $('#inner_rapper').css('height', '615px');
        }

    });

}


function OnDropdownOptionSelected() {
    if ($("#plateFormlist option:selected").attr("value") == 'all') {
        AllPlateformTabSelected();
        //alert($("#plateFormlist option:selected").attr("value"));
    }
    else if ($("#plateFormlist option:selected").attr("value") == 'ios') {
        IosPlateformTabSelected();
        //alert($("#plateFormlist option:selected").attr("value"));
    }
    else if ($("#plateFormlist option:selected").attr("value") == 'android') {
        AndroidPlateformTabSelected();
        //alert($("#plateFormlist option:selected").attr("value"));
    }
    else { }



}

function SetHistoryGridHeaders(yesterdays, twodaysback, ThreeDaysBack, foureDaysBack, fiveDaysBack) {
    $('#history-h-fivedays-back').text(fiveDaysBack);
    $('#history-h-fourdays-back').text(foureDaysBack);
    $('#history-h-threedays-back').text(ThreeDaysBack);
    $('#history-h-twodays-back').text(twodaysback);
    $('#history-h-Yesterdaydays').text(yesterdays);

}
function SetLineChartHeading(intialDate, finalDate) {

    $('#initial-date').text(new Date(intialDate).getDate() + ' ' + getMonthNameByNumber(new Date(intialDate).getMonth()) + ' ' + new Date(intialDate).getFullYear());
    $('#final-date').text(new Date(finalDate).getDate() + ' ' + getMonthNameByNumber(new Date(finalDate).getMonth()) + ' ' + new Date(intialDate).getFullYear());
}

function AllPlateformTabSelected() {
    $('#history-chart').empty();
    $('#history-chart').append('<div><canvas id="line-chart-canvas" height="320" width="1040"></canvas> </div>');
    buildLineChartForAllPlateForms(GetDataArray(dataFromServer.RecentHistory.WeeklyHistory), GetDataArray(dataFromServer.Ios.RecentHistory.WeeklyHistory), GetDataArray(dataFromServer.Android.RecentHistory.WeeklyHistory), getCaptionsForLinChart(dataFromServer.RecentHistory.WeeklyHistory));
    $(".plateFormColorBox").show();
    //  $("#all-box").css('','')

    manageAllCountriesGrid(sortCountryArry(GetAllCountriesArray(dataFromServer.CountryStats)));
    SetHistoryGridHeaders(dataFromServer.RecentHistory.WeeklyHistory[4].Caption, dataFromServer.RecentHistory.WeeklyHistory[3].Caption, dataFromServer.RecentHistory.WeeklyHistory[2].Caption, dataFromServer.RecentHistory.WeeklyHistory[1].Caption, dataFromServer.RecentHistory.WeeklyHistory[0].Caption);
    $('#History_grid_view').tablesorter();
    $('#history-grid').animate({ scrollTop: 0 }, "fast");
}

function IosPlateformTabSelected() {
    $('#history-chart').empty();
    $('#history-chart').append('<div><canvas id="line-chart-canvas" height="320" width="1040"></canvas> </div>');
    buildLineChart(GetDataArray(dataFromServer.Ios.RecentHistory.WeeklyHistory), getCaptionsForLinChart(dataFromServer.Ios.RecentHistory.WeeklyHistory), "#4386f4", "#17407d");
    $(".plateFormColorBox").hide();
    manageIosCountriesGrid(sortCountryArry(GetIosCountriesArray(dataFromServer.CountryStats)));
    SetHistoryGridHeaders(dataFromServer.RecentHistory.WeeklyHistory[4].Caption, dataFromServer.RecentHistory.WeeklyHistory[3].Caption, dataFromServer.RecentHistory.WeeklyHistory[2].Caption, dataFromServer.RecentHistory.WeeklyHistory[1].Caption, dataFromServer.RecentHistory.WeeklyHistory[0].Caption);
    $('#History_grid_view').tablesorter();
    $('#history-grid').animate({ scrollTop: 0 }, "fast");
}

function AndroidPlateformTabSelected() {
    $('#history-chart').empty();
    $('#history-chart').append('<div><canvas id="line-chart-canvas" height="320" width="1040"></canvas> </div>');
    buildLineChart(GetDataArray(dataFromServer.Android.RecentHistory.WeeklyHistory), getCaptionsForLinChart(dataFromServer.Android.RecentHistory.WeeklyHistory), "#34a853", "#318037");
    $(".plateFormColorBox").hide();
    manageAndroidCountriesGrid(sortCountryArry(GetAndroidCountriesArray(dataFromServer.CountryStats)));
    SetHistoryGridHeaders(dataFromServer.RecentHistory.WeeklyHistory[4].Caption, dataFromServer.RecentHistory.WeeklyHistory[3].Caption, dataFromServer.RecentHistory.WeeklyHistory[2].Caption, dataFromServer.RecentHistory.WeeklyHistory[1].Caption, dataFromServer.RecentHistory.WeeklyHistory[0].Caption);
    $('#History_grid_view').tablesorter();
    $('#history-grid').animate({ scrollTop: 0 }, "fast");
}


function GetDataArray(recentHistoryByCountry, previousCountBeforeHistory) {
    lineChartDataArray = [];
    lineChartDataArray[0] = parseInt(recentHistoryByCountry[4].TotalCount);
    lineChartDataArray[1] = parseInt(recentHistoryByCountry[3].TotalCount);
    lineChartDataArray[2] = parseInt(recentHistoryByCountry[2].TotalCount);
    lineChartDataArray[3] = parseInt(recentHistoryByCountry[1].TotalCount);
    lineChartDataArray[4] = parseInt(recentHistoryByCountry[0].TotalCount);
    return lineChartDataArray;
}
function getCaptionsForLinChart(recentHistoryByCountry) {
    lineChartLableArray = [];

    var j = 0;
    for (var i = 4; i >= 0; i--) {
        lineChartLableArray[j] = recentHistoryByCountry[i].Caption;
        j++;
    }
    return lineChartLableArray;
}


function manageAllCountriesGrid(countries) {
    $('#History_grid_view').empty();
    $('#History_grid_view').append('<thead style="position: absolute; top: -30px; z-index: 2; height: 30px; "><tr ><th style="border-top-left-radius: 10px; width: 91px;"><span>Sr</span></th><th style="text-align: left; width: 345px;"><span>Country</span></th><th><span id="history-h-fivedays-back">counts</span></th><th><span id="history-h-fourdays-back">counts</span></th><th><span id="history-h-threedays-back">counts</span></th><th><span id="history-h-twodays-back">counts</span></th><th style="border-top-right-radius: 10px;"><span id="history-h-Yesterdaydays">counts</span></th></tr></thead><tbody></tbody>')
    for (var i = 0 ; i < countries.length  ; i++) {

        $('#History_grid_view tbody').append('<tr>'
                + '<td class="lalign">' + (i + 1) + '</td>'
                + '<td>' + countries[i].CountryName + '</td>'
                + '<td>' + (countries[i].RecentHistory.WeeklyHistory[0].TotalCount + countries[i].RecentHistory.WeeklyHistory[1].TotalCount + countries[i].RecentHistory.WeeklyHistory[2].TotalCount + countries[i].RecentHistory.WeeklyHistory[3].TotalCount + countries[i].RecentHistory.WeeklyHistory[4].TotalCount + countries[i].PreviousCountBeforeHistory) + '</td>'
                + '<td>' + (countries[i].RecentHistory.WeeklyHistory[1].TotalCount + countries[i].RecentHistory.WeeklyHistory[2].TotalCount + countries[i].RecentHistory.WeeklyHistory[3].TotalCount + countries[i].RecentHistory.WeeklyHistory[4].TotalCount + countries[i].PreviousCountBeforeHistory) + '</td>'
                + '<td>' + (countries[i].RecentHistory.WeeklyHistory[2].TotalCount + countries[i].RecentHistory.WeeklyHistory[3].TotalCount + countries[i].RecentHistory.WeeklyHistory[4].TotalCount + countries[i].PreviousCountBeforeHistory) + '</td>'
                + '<td>' + (countries[i].RecentHistory.WeeklyHistory[3].TotalCount + countries[i].RecentHistory.WeeklyHistory[4].TotalCount + countries[i].PreviousCountBeforeHistory) + '</td>'
                + '<td>' + (countries[i].RecentHistory.WeeklyHistory[4].TotalCount + countries[i].PreviousCountBeforeHistory) + '</td>'
                + '</tr>'

                            )

    }

}

function manageAndroidCountriesGrid(countries) {

    $('#History_grid_view').empty();
    $('#History_grid_view').append('<thead style="position: absolute; top: -30px; z-index: 2; height: 30px; "><tr ><th style="border-top-left-radius: 10px; width: 91px;"><span>Sr</span></th><th style="text-align: left; width: 345px;"><span>Country</span></th><th><span id="history-h-fivedays-back">counts</span></th><th><span id="history-h-fourdays-back">counts</span></th><th><span id="history-h-threedays-back">counts</span></th><th><span id="history-h-twodays-back">counts</span></th><th style="border-top-right-radius: 10px;"><span id="history-h-Yesterdaydays">counts</span></th></tr></thead><tbody></tbody>')

    for (var i = 0 ; i < countries.length  ; i++) {

        $('#History_grid_view tbody').append('<tr>'
            + '<td>' + (i + 1) + '</td>'
            + '<td>' + countries[i].CountryName + '</td>'
             + '<td>' + (countries[i].Android.RecentHistory.WeeklyHistory[0].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[1].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Android.PreviousCountBeforeHistory) + '</td>'
              + '<td>' + (countries[i].Android.RecentHistory.WeeklyHistory[1].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Android.PreviousCountBeforeHistory) + '</td>'
              + '<td>' + (countries[i].Android.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Android.PreviousCountBeforeHistory) + '</td>'
               + '<td>' + (countries[i].Android.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Android.PreviousCountBeforeHistory) + '</td>'
            + '<td>' + (countries[i].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Android.PreviousCountBeforeHistory) + '</td>'
                    + '</tr>'

                        )

    }


}
function manageIosCountriesGrid(countries) {

    $('#History_grid_view').empty();
    $('#History_grid_view').append('<thead style="position: absolute; top: -30px; z-index: 2; height: 30px; "><tr ><th style="border-top-left-radius: 10px; width: 91px;"><span>Sr</span></th><th style="text-align: left; width: 345px;"><span>Country</span></th><th><span id="history-h-fivedays-back">counts</span></th><th><span id="history-h-fourdays-back">counts</span></th><th><span id="history-h-threedays-back">counts</span></th><th><span id="history-h-twodays-back">counts</span></th><th style="border-top-right-radius: 10px;"><span id="history-h-Yesterdaydays">counts</span></th></tr></thead><tbody></tbody>')

    for (var i = 0 ; i < countries.length  ; i++) {
        $('#History_grid_view tbody').append('<tr>'
            + '<td>' + (i + 1) + '</td>'
            + '<td>' + countries[i].CountryName + '</td>'
             + '<td>' + (countries[i].Ios.RecentHistory.WeeklyHistory[0].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[1].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Ios.PreviousCountBeforeHistory) + '</td>'
             + '<td>' + (countries[i].Ios.RecentHistory.WeeklyHistory[1].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Ios.PreviousCountBeforeHistory) + '</td>'
                + '<td>' + (countries[i].Ios.RecentHistory.WeeklyHistory[2].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Ios.PreviousCountBeforeHistory) + '</td>'
                  + '<td>' + (countries[i].Ios.RecentHistory.WeeklyHistory[3].TotalCount + countries[i].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Ios.PreviousCountBeforeHistory) + '</td>'
                  + '<td>' + (countries[i].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[i].Ios.PreviousCountBeforeHistory) + '</td>'
                         + '</tr>'
                        )

    }

}

function GetAllCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function GetAndroidCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Android.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function GetIosCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Ios.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}



function sortCountryArry(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].CountryName;

            var second = countries[j + 1].CountryName;

            if (first > second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}


function OLDsortAllCountryArry(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = getPercentageIncrease(countries[j].RecentHistory.WeeklyHistory[4].TotalCount + countries[j].PreviousCountBeforeHistory, countries[j].TotalCount);
            var second = getPercentageIncrease(countries[j + 1].RecentHistory.WeeklyHistory[4].TotalCount + countries[j + 1].PreviousCountBeforeHistory, countries[j + 1].TotalCount);

            if (parseFloat(first) < parseFloat(second)) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function OLDsortAndroidCountryArry(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = getPercentageIncrease(countries[j].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[j].Android.PreviousCountBeforeHistory, countries[j].Android.TotalCount);
            var second = getPercentageIncrease(countries[j + 1].Android.RecentHistory.WeeklyHistory[4].TotalCount + countries[j + 1].Android.PreviousCountBeforeHistory, countries[j + 1].Android.TotalCount);

            if (parseFloat(first) < parseFloat(second)) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function OLDsortIosCountryArry(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = getPercentageIncrease(countries[j].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[j].Ios.PreviousCountBeforeHistory, countries[j].Ios.TotalCount);
            var second = getPercentageIncrease(countries[j + 1].Ios.RecentHistory.WeeklyHistory[4].TotalCount + countries[j + 1].Ios.PreviousCountBeforeHistory, countries[j + 1].Ios.TotalCount);

            if (parseFloat(first) < parseFloat(second)) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}


//End History Portion Methods



//right Upper manu Methods
function UpperManuMethodsCall() {

    OnTabClicks();
    onClickFirstTab();
}



function OnTabClicks() {
    $("#dashboardUpperManuDailyTabsInnerDiv").click(function () {

        PapulateDataForDailyTab();
        $('#right-upper-date').empty();
        var dateFromServer = dataFromServer.RecentHistory.WeeklyHistory[0].Date;
        $("#right-upper-date").text(new Date(dateFromServer).getDate() + ' ' + getMonthNameByNumber(new Date(dateFromServer).getMonth()) + ' ' + new Date(dateFromServer).getFullYear());
        $('#gridPortion').animate({ scrollTop: 0 }, "fast");
    });

    $("#dashboardUpperManuWeeklyTabsInnerDiv").click(function () {
        debugger;
        PapulateDataForWeeklyTab();
        $('#right-upper-date').empty();
        var date1 = dataFromServer.RecentHistory.WeeklyHistory[6].Date;
        var date2 = dataFromServer.RecentHistory.WeeklyHistory[0].Date;
        var datePart1 = new Date(date1).getDate() + ' ' + getMonthNameByNumber(new Date(date1).getMonth()) + ' ' + new Date(date1).getFullYear();
        var datePart2 = new Date(date2).getDate() + ' ' + getMonthNameByNumber(new Date(date2).getMonth()) + ' ' + new Date(date2).getFullYear();

        $('#right-upper-date').text(datePart1 + ' - ' + datePart2);

        $('#gridPortion').animate({ scrollTop: 0 }, "fast");
    });


    $("#dashboardUpperManuMonthlyTabsInnerDiv").click(function () {
        PapulateDataForMonthlyTab();
        $('#right-upper-date').empty();
        var date1 = dataFromServer.RecentHistory.MonthlyCounts.LowerBoundDate;
        var date2 = dataFromServer.RecentHistory.MonthlyCounts.UpperBoundDate;
        var datePart1 = new Date(date1).getDate() + ' ' + getMonthNameByNumber(new Date(date1).getMonth()) + ' ' + new Date(date1).getFullYear();
        var datePart2 = new Date(date2).getDate() + ' ' + getMonthNameByNumber(new Date(date2).getMonth()) + ' ' + new Date(date2).getFullYear();
        $('#right-upper-date').text(datePart1 + ' - ' + datePart2);

        $('#gridPortion').animate({ scrollTop: 0 }, "fast");
    });

    $("#dashboardUpperManuQuarterlyTabsInnerDiv").click(function () {
        PapulateDataForQuarterlyTab();
        $('#right-upper-date').empty();
        var date1 = dataFromServer.RecentHistory.QuarterlyCounts.LowerBoundDate;
        var date2 = dataFromServer.RecentHistory.QuarterlyCounts.UpperBoundDate;
        var datePart1 = new Date(date1).getDate() + ' ' + getMonthNameByNumber(new Date(date1).getMonth()) + ' ' + new Date(date1).getFullYear();
        var datePart2 = new Date(date2).getDate() + ' ' + getMonthNameByNumber(new Date(date2).getMonth()) + ' ' + new Date(date2).getFullYear();
        $('#right-upper-date').text(datePart1 + ' - ' + datePart2);

        $('#gridPortion').animate({ scrollTop: 0 }, "fast");
    });



}
function onClickFirstTab() {
    $('#dashboardUpperManuRegisterTabsInnerDiv').click(function () {
        papulateDataForFirstTab();
        $('#gridPortion').animate({ scrollTop: 0 }, "fast");
    });
}

function papulateDataForFirstTab() {
    $('#right-upper-date').empty();
    var tabText = $('#dashboardUpperManuRegisterTabsInnerDiv').text();
    if (tabText == 'All Registered Users') {
        manageAllCountAndPapulate(allCountList, dataFromServer.TotalCount);
        BuildDoughnutChartAllCount(allCountList, dataFromServer.TotalCount, 6, true);
        AnnimateUIControls();
    }
    else if (tabText == 'iOS Registered Users') {
        iosCountList = sortByIosCounts(iosCountList);
        manageIosCountAndPapulate(iosCountList, dataFromServer.Ios.TotalCount);
        BuildDoughnutChartIosCount(iosCountList, dataFromServer.Ios.TotalCount, 6, true);
        AnnimateUIControls();
    }
    else {
        androidCountList = sortByAndroidCounts(androidCountList);
        manageAndroidCountAndPapulate(androidCountList, dataFromServer.Android.TotalCount);
        BuildDoughnutChartAndroidCount(androidCountList, dataFromServer.Android.TotalCount, 6, true);
        AnnimateUIControls();
    }
}

function PapulateDataForDailyTab() {
    var tabText = $('#dashboardUpperManuRegisterTabsInnerDiv').text();
    if (tabText == 'All Registered Users') {

        var sortedCountries = sortAllCountryArryDailyDownloads(GetDailyCountAllCountriesArray(dataFromServer.CountryStats));
        manageDailyAllCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.WeeklyHistory[0].TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartDailyAllCountForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.WeeklyHistory[0].TotalCount, 7, false);

    }
    else if (tabText == 'iOS Registered Users') {
        var sortedCountries = sortIosCountryArryDailyDownloads(GetDailyCountIosCountriesArray(dataFromServer.CountryStats));
        manageDailyIosCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.WeeklyHistory[0].TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartDailyIosCountForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.WeeklyHistory[0].TotalCount, 7, false);
    }
    else {
        var sortedCountries = sortAndroidCountryArryDailyDownloads(GetDailyCountAndroidCountriesArray(dataFromServer.CountryStats));
        manageDailyAndroidCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.WeeklyHistory[0].TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartDailyAndroidCountForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.WeeklyHistory[0].TotalCount, 7, false);
    }

}

//All Countries Daily Counts 
function GetDailyCountAllCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].RecentHistory.WeeklyHistory[0].TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAllCountryArryDailyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].RecentHistory.WeeklyHistory[0].TotalCount;

            var second = countries[j + 1].RecentHistory.WeeklyHistory[0].TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageDailyAllCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = allCountList[i].RecentHistory.WeeklyHistory[0].TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartDailyAllCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].RecentHistory.WeeklyHistory[0].TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].RecentHistory.WeeklyHistory[0].TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End All Countries Daily Counts 

//Ios Countries Daily Counts 
function GetDailyCountIosCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Ios.RecentHistory.WeeklyHistory[0].TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortIosCountryArryDailyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Ios.RecentHistory.WeeklyHistory[0].TotalCount;

            var second = countries[j + 1].Ios.RecentHistory.WeeklyHistory[0].TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageDailyIosCountAndPapulateForRightUpperTab(IosCountList, totalDownloads) {

    if (!(IosCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < IosCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = IosCountList[i].CountryName;
            rowDataForTopCountryList[1] = IosCountList[i].Ios.RecentHistory.WeeklyHistory[0].TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartDailyIosCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Ios.RecentHistory.WeeklyHistory[0].TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Ios.RecentHistory.WeeklyHistory[0].TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Ios Countries Daily Counts 


//Android Countries Daily Counts 
function GetDailyCountAndroidCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Android.RecentHistory.WeeklyHistory[0].TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAndroidCountryArryDailyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Android.RecentHistory.WeeklyHistory[0].TotalCount;

            var second = countries[j + 1].Android.RecentHistory.WeeklyHistory[0].TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageDailyAndroidCountAndPapulateForRightUpperTab(CountList, totalDownloads) {

    if (!(CountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < CountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = CountList[i].CountryName;
            rowDataForTopCountryList[1] = CountList[i].Android.RecentHistory.WeeklyHistory[0].TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartDailyAndroidCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Android.RecentHistory.WeeklyHistory[0].TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Android.RecentHistory.WeeklyHistory[0].TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Android Countries Daily Counts 


function PapulateDataForWeeklyTab() {
    debugger;
    var tabText = $('#dashboardUpperManuRegisterTabsInnerDiv').text();
    if (tabText == 'All Registered Users') {

        var sortedCountries = sortAllCountryArryWeeklyDownloads(GetWeeklyCountAllCountriesArray(dataFromServer.CountryStats));
        manageWeeklyAllCountAndPapulateForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.RecentHistory.WeeklyHistory));
        AnnimateTopCountryList();
        BuildDoughnutChartWeeklyAllCountForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.RecentHistory.WeeklyHistory), 7, false);

    }
    else if (tabText == 'iOS Registered Users') {
        var sortedCountries = sortIosCountryArryWeeklyDownloads(GetWeeklyCountIosCountriesArray(dataFromServer.CountryStats));
        manageWeeklyIosCountAndPapulateForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.Ios.RecentHistory.WeeklyHistory));
        AnnimateTopCountryList();
        BuildDoughnutChartWeeklyIosCountForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.Ios.RecentHistory.WeeklyHistory), 7, false);
    }
    else {
        var sortedCountries = sortAndroidCountryArryWeeklyDownloads(GetWeeklyCountAndroidCountriesArray(dataFromServer.CountryStats));
        manageWeeklyAndroidCountAndPapulateForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.Android.RecentHistory.WeeklyHistory));
        AnnimateTopCountryList();
        BuildDoughnutChartWeeklyAndroidCountForRightUpperTab(sortedCountries, GetTotalCountOfWeekDays(dataFromServer.Android.RecentHistory.WeeklyHistory), 7, false);
    }

}

//All Countries Weekly Counts 
function GetWeeklyCountAllCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {

        if (GetTotalCountOfWeekDays(countries[countrykey].RecentHistory.WeeklyHistory) != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;

        }
    }
    return countriesArrary;
}
function sortAllCountryArryWeeklyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = GetTotalCountOfWeekDays(countries[j].RecentHistory.WeeklyHistory);

            var second = GetTotalCountOfWeekDays(countries[j + 1].RecentHistory.WeeklyHistory);

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageWeeklyAllCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = GetTotalCountOfWeekDays(allCountList[i].RecentHistory.WeeklyHistory);

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartWeeklyAllCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = GetTotalCountOfWeekDays(list[i].RecentHistory.WeeklyHistory);

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(GetTotalCountOfWeekDays(list[j + 1].RecentHistory.WeeklyHistory));
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End All Countries Weekly Counts 

//Ios Countries Weekly Counts 
function GetWeeklyCountIosCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (GetTotalCountOfWeekDays(countries[countrykey].Ios.RecentHistory.WeeklyHistory) != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortIosCountryArryWeeklyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = GetTotalCountOfWeekDays(countries[j].Ios.RecentHistory.WeeklyHistory);

            var second = GetTotalCountOfWeekDays(countries[j + 1].Ios.RecentHistory.WeeklyHistory);

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageWeeklyIosCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = GetTotalCountOfWeekDays(allCountList[i].Ios.RecentHistory.WeeklyHistory);

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartWeeklyIosCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = GetTotalCountOfWeekDays(list[i].Ios.RecentHistory.WeeklyHistory);

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(GetTotalCountOfWeekDays(list[j + 1].Ios.RecentHistory.WeeklyHistory));
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}

// End Ios Countries Weekly Counts 


//Android Countries Weekly Counts 
function GetWeeklyCountAndroidCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (GetTotalCountOfWeekDays(countries[countrykey].Android.RecentHistory.WeeklyHistory) != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAndroidCountryArryWeeklyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = GetTotalCountOfWeekDays(countries[j].Android.RecentHistory.WeeklyHistory);

            var second = GetTotalCountOfWeekDays(countries[j + 1].Android.RecentHistory.WeeklyHistory);

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageWeeklyAndroidCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = GetTotalCountOfWeekDays(allCountList[i].Android.RecentHistory.WeeklyHistory);

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartWeeklyAndroidCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = GetTotalCountOfWeekDays(list[i].Android.RecentHistory.WeeklyHistory);

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(GetTotalCountOfWeekDays(list[j + 1].Android.RecentHistory.WeeklyHistory));
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Android Countries Weekly Counts 
function GetTotalCountOfWeekDays(WeekDaysCollection) {
    var sum = 0;
    for (var i = 0; i < WeekDaysCollection.length; i++) {
        sum += WeekDaysCollection[i].TotalCount;
    }
    return sum;
}



function PapulateDataForMonthlyTab() {
    var tabText = $('#dashboardUpperManuRegisterTabsInnerDiv').text();
    if (tabText == 'All Registered Users') {

        var sortedCountries = sortAllCountryArryMonthlyDownloads(GetMonthlyCountAllCountriesArray(dataFromServer.CountryStats));
        manageMonthlyAllCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.MonthlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartMonthlyAllCountForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.MonthlyCounts.TotalCount, 7, false);

    }
    else if (tabText == 'iOS Registered Users') {
        var sortedCountries = sortIosCountryArryMonthlyDownloads(GetMonthlyCountIosCountriesArray(dataFromServer.CountryStats));
        manageMonthlyIosCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.MonthlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartMonthlyIosCountForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.MonthlyCounts.TotalCount, 7, false);
    }
    else {
        var sortedCountries = sortAndroidCountryArryMonthlyDownloads(GetMonthlyCountAndroidCountriesArray(dataFromServer.CountryStats));
        manageMonthlyAndroidCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.MonthlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartMonthlyAndroidCountForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.MonthlyCounts.TotalCount, 7, false);
    }

}

//All Countries Monthly Counts 
function GetMonthlyCountAllCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].RecentHistory.MonthlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAllCountryArryMonthlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].RecentHistory.MonthlyCounts.TotalCount;

            var second = countries[j + 1].RecentHistory.MonthlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageMonthlyAllCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = allCountList[i].RecentHistory.MonthlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartMonthlyAllCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].RecentHistory.MonthlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].RecentHistory.MonthlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End All Countries Monthly Counts 

//Ios Countries Monthly Counts 
function GetMonthlyCountIosCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Ios.RecentHistory.MonthlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortIosCountryArryMonthlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Ios.RecentHistory.MonthlyCounts.TotalCount;

            var second = countries[j + 1].Ios.RecentHistory.MonthlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageMonthlyIosCountAndPapulateForRightUpperTab(IosCountList, totalDownloads) {

    if (!(IosCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < IosCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = IosCountList[i].CountryName;
            rowDataForTopCountryList[1] = IosCountList[i].Ios.RecentHistory.MonthlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartMonthlyIosCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Ios.RecentHistory.MonthlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Ios.RecentHistory.MonthlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}

// End Ios Countries Monthly Counts 


//Android Countries Monthly Counts 
function GetMonthlyCountAndroidCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Android.RecentHistory.MonthlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAndroidCountryArryMonthlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Android.RecentHistory.MonthlyCounts.TotalCount;

            var second = countries[j + 1].Android.RecentHistory.MonthlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageMonthlyAndroidCountAndPapulateForRightUpperTab(CountList, totalDownloads) {

    if (!(CountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < CountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = CountList[i].CountryName;
            rowDataForTopCountryList[1] = CountList[i].Android.RecentHistory.MonthlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartMonthlyAndroidCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Android.RecentHistory.MonthlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Android.RecentHistory.MonthlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Android Countries Monthly Counts 




function PapulateDataForQuarterlyTab() {
    var tabText = $('#dashboardUpperManuRegisterTabsInnerDiv').text();
    if (tabText == 'All Registered Users') {

        var sortedCountries = sortAllCountryArryQuarterlyDownloads(GetQuarterlyCountAllCountriesArray(dataFromServer.CountryStats));
        manageQuarterlyAllCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.QuarterlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartQuarterlyAllCountForRightUpperTab(sortedCountries, dataFromServer.RecentHistory.QuarterlyCounts.TotalCount, 7, false);

    }
    else if (tabText == 'iOS Registered Users') {
        var sortedCountries = sortIosCountryArryQuarterlyDownloads(GetQuarterlyCountIosCountriesArray(dataFromServer.CountryStats));
        manageQuarterlyIosCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.QuarterlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartQuarterlyIosCountForRightUpperTab(sortedCountries, dataFromServer.Ios.RecentHistory.QuarterlyCounts.TotalCount, 7, false);
    }
    else {
        var sortedCountries = sortAndroidCountryArryQuarterlyDownloads(GetQuarterlyCountAndroidCountriesArray(dataFromServer.CountryStats));
        manageQuarterlyAndroidCountAndPapulateForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.QuarterlyCounts.TotalCount);
        AnnimateTopCountryList();
        BuildDoughnutChartQuarterlyAndroidCountForRightUpperTab(sortedCountries, dataFromServer.Android.RecentHistory.QuarterlyCounts.TotalCount, 7, false);
    }

}

//All Countries Quarterly Counts 
function GetQuarterlyCountAllCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].RecentHistory.QuarterlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAllCountryArryQuarterlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].RecentHistory.QuarterlyCounts.TotalCount;

            var second = countries[j + 1].RecentHistory.QuarterlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageQuarterlyAllCountAndPapulateForRightUpperTab(allCountList, totalDownloads) {

    if (!(allCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < allCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = allCountList[i].CountryName;
            rowDataForTopCountryList[1] = allCountList[i].RecentHistory.QuarterlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartQuarterlyAllCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].RecentHistory.QuarterlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].RecentHistory.QuarterlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End All Countries Quarterly Counts 

//Ios Countries Quarterly Counts 
function GetQuarterlyCountIosCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Ios.RecentHistory.QuarterlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortIosCountryArryQuarterlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Ios.RecentHistory.QuarterlyCounts.TotalCount;

            var second = countries[j + 1].Ios.RecentHistory.QuarterlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageQuarterlyIosCountAndPapulateForRightUpperTab(IosCountList, totalDownloads) {

    if (!(IosCountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < IosCountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = IosCountList[i].CountryName;
            rowDataForTopCountryList[1] = IosCountList[i].Ios.RecentHistory.QuarterlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartQuarterlyIosCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Ios.RecentHistory.QuarterlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Ios.RecentHistory.QuarterlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Ios Countries Quarterly Counts 


//Android Countries Quarterly Counts 
function GetQuarterlyCountAndroidCountriesArray(countries) {
    var countriesArrary = [];
    var i = 0;
    for (var countrykey in countries) {
        if (countries[countrykey].Android.RecentHistory.QuarterlyCounts.TotalCount != 0) {
            countriesArrary[i] = countries[countrykey];
            i++;
        }
    }
    return countriesArrary;
}
function sortAndroidCountryArryQuarterlyDownloads(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Android.RecentHistory.QuarterlyCounts.TotalCount;

            var second = countries[j + 1].Android.RecentHistory.QuarterlyCounts.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function manageQuarterlyAndroidCountAndPapulateForRightUpperTab(CountList, totalDownloads) {

    if (!(CountList == undefined)) {
        //var total = (allCountList[0].split('::'))[1];
        $('#grid_view').empty();
        $('#top-countries tbody').empty();

        for (var i = 0; i < CountList.length ; i++) {

            var rowDataForTopCountryList = [];
            rowDataForTopCountryList[0] = CountList[i].CountryName;
            rowDataForTopCountryList[1] = CountList[i].Android.RecentHistory.QuarterlyCounts.TotalCount;

            if (i < 7) {
                manageTopCountryList(rowDataForTopCountryList, totalDownloads, i);
            }


            if (rowDataForTopCountryList[0].length < 30) {
                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td>' + rowDataForTopCountryList[0] + '</td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + (parseInt(rowDataForTopCountryList[1]) / totalDownloads * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            else {


                $('#grid_view').append('<tr>' +
                    '<td>' + (i + 1) + '</td>' +
                    '<td><abbr title=\"' + rowDataForTopCountryList[0] + '\">' + (rowDataForTopCountryList[0].substring(0, 30)) + '..' + '</abbr></td>' +
                    '<td>' + rowDataForTopCountryList[1] + '</td>' +
                    '<td>' + ((parseInt(rowDataForTopCountryList[1]) / totalDownloads) * 100).toFixed(2) + '</td>' +
                    '</tr>');
            }
            // }
        }

    }
}
function BuildDoughnutChartQuarterlyAndroidCountForRightUpperTab(list, totalDownloads, numberOfCountries, isShowOther) {

    $('#DoughnutChartRapper').empty();
    $('#DoughnutChartRapper').append('<div id="total-count-in-graph"><div id="total-count-value"><label id="graph-total-count">0</label></div></div>');
    $('#DoughnutChartRapper').append('<canvas id="chart-area" width="190" height="190" />');
    $('#graph-total-count').text(totalDownloads);

    var colorCollection = ["#F7464A", "#46BFBD", "#568de0", "#949FB1", "#e16326", "#6aba19", "#4D5360"];
    var highlightCollection = ["#FF5A5E", "#5AD3D1", "#7ca7e7", "#A8B3C5", "#e88f23", "#aae86c", "#616774"];
    //var str = list[0].split('::');

    var valueCollection = [];

    if (numberOfCountries > list.length)
    { numberOfCountries = list.length; }
    for (var i = 0; i < numberOfCountries; i++) {
        var currentCountryData = [];
        currentCountryData[0] = list[i].CountryName;
        currentCountryData[1] = list[i].Android.RecentHistory.QuarterlyCounts.TotalCount;

        //To show others in Top Countries Table and Doughnut chart
        if (i == numberOfCountries) {
            if (isShowOther) {
                var sum = 0;
                for (var j = i; j < list.length - 1; j++) {
                    sum += parseInt(list[j + 1].Android.RecentHistory.QuarterlyCounts.TotalCount);
                }

                var valueInpercentage = 0;

                valueInpercentage = ((sum * 100 / totalDownloads).toFixed(2));


                //to show remaining in top country list
                var countryList = ['Others', sum];
                manageTopCountryList(countryList, totalDownloads, numberOfCountries);
                //to show remaining in top country list


                var a = {
                    value: parseFloat(valueInpercentage),
                    color: colorCollection[i],
                    highlight: highlightCollection[i],
                    label: "Others" + '(%)'
                };
                valueCollection.push(a);
            }
        }
        else {


            var valueInpercentage = 0;

            valueInpercentage = ((currentCountryData[1] * 100 / totalDownloads).toFixed(2));
            var a = {
                value: parseFloat(valueInpercentage),
                color: colorCollection[i],
                highlight: highlightCollection[i],
                label: currentCountryData[0] + '(%)'
            };
            valueCollection.push(a);
        }

    }




    BuildDoughnutChart(valueCollection);
}
// End Android Countries Quarterly Counts 




//End right Upper manu Methods





//Inernal functions

function getPercentageIncrease(minVal, maxVal) {
    var x;
    if (minVal == 0 && maxVal == 0)
    { x = '0.00'; }
    else if (minVal == 0) {
        minVal += 1;
        maxVal += 1;
        x = (((maxVal - minVal) / (minVal)) * 100).toFixed(2);
    }
    else {
        x = (((maxVal - minVal) / (minVal)) * 100).toFixed(2);
    }
    return x;


}

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

function sortByAllCounts(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].TotalCount;
            var second = countries[j + 1].TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function sortByIosCounts(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Ios.TotalCount;
            var second = countries[j + 1].Ios.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}
function sortByAndroidCounts(countries) {
    for (var i = countries.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = countries[j].Android.TotalCount;
            var second = countries[j + 1].Android.TotalCount;

            if (first < second) {
                var temp;
                temp = countries[j];
                countries[j] = countries[j + 1];
                countries[j + 1] = temp;
            }

        }
    }
    return countries;
}



function sortList(RawList) {


    for (var i = RawList.length; i > 0; i--) {
        for (var j = 0; j < i - 1; j++) {
            var first = RawList[j].split('::');
            if (RawList[j + 1] == undefined) {
                var x;
            }

            var second = RawList[j + 1].split('::');

            if (parseInt(first[1]) < parseInt(second[1])) {
                var temp;
                temp = RawList[j];
                RawList[j] = RawList[j + 1];
                RawList[j + 1] = temp;
            }

        }
    }

    if (RawList.length == 2) {
        var temp;
        temp = RawList[0];
        RawList[0] = RawList[1];
        RawList[1] = temp;
    }

    return RawList;

}
function getMonthNameByNumber(number) {
    var month = new Array();
    month[0] = "Jan";
    month[1] = "Feb";
    month[2] = "Mar";
    month[3] = "Apr";
    month[4] = "May";
    month[5] = "June";
    month[6] = "July";
    month[7] = "Aug";
    month[8] = "Sept";
    month[9] = "Oct";
    month[10] = "Nove";
    month[11] = "Dec";

    return month[number];
}

//end internal function

//Page UI at startUp
function StartupPageUI() {
    AllCountryTabSelected();
}
//End Page UI at startUp

//Manage Page UI State
function AllCountryTabSelected() {
    $("#r-all-1").css('background-color', 'gray');
    $("#r-all-1").css('color', 'white');

    $("#r-an-1").css('background-color', '#dfdfdf');
    $("#r-an-1").css('color', 'black');

    $("#r-ios-1").css('background-color', '#dfdfdf');
    $("#r-ios-1").css('color', 'black');

    $('#dashboardUpperManuRegisterTabsInnerDiv').text('All Registered Users');
    $('#gridPortion').animate({ scrollTop: 0 }, "fast");

}

function IOSCountryTabSelected() {

    $("#r-ios-1").css('background-color', 'gray');
    $("#r-ios-1").css('color', 'white');

    $("#r-an-1").css('background-color', '#dfdfdf');
    $("#r-an-1").css('color', 'black');

    $("#r-all-1").css('background-color', '#dfdfdf');
    $("#r-all-1").css('color', 'black');

    $('#dashboardUpperManuRegisterTabsInnerDiv').text('iOS Registered Users');
    $('#gridPortion').animate({ scrollTop: 0 }, "fast");
}

function AndroidCountryTabSelected() {
    $("#r-an-1").css('background-color', 'gray');
    $("#r-an-1").css('color', 'white');

    $("#r-ios-1").css('background-color', '#dfdfdf');
    $("#r-ios-1").css('color', 'black');

    $("#r-all-1").css('background-color', '#dfdfdf');
    $("#r-all-1").css('color', 'black');

    $('#dashboardUpperManuRegisterTabsInnerDiv').text('Android Registered Users');
    $('#gridPortion').animate({ scrollTop: 0 }, "fast");
}

var IsRegestrationTabSelected = true;
var IsQuaterlyTabSelected = false;
function RightUpperManuEvents() {
    //$(".dashboardUpperManuTabs").click(function () {
    //    if ($(this).attr('id') == "dashboardUpperManuLastPart") {
    //        $('#dashboardUpperManuLastPart').addClass('dashboardUpperManuSelectedTab');
    //    }
    //});

    //$(".dashboardUpperManuTabs").click(function () {
    //    if ($(this).attr('id') == "dashboardUpperManuLeftPart") {
    //        $('#dashboardUpperManuRegistrationTab').addClass('dashboardUpperManuSelectedTab');
    //    }
    //});


    // for registration Tab hover effact
    $(".dashboardUpperManuTabs").click(function () {
        if ($(this).attr('id') == "dashboardUpperManuRegistrationTab") {
            IsRegestrationTabSelected = true;
        } else IsRegestrationTabSelected = false;
    });



    $("#dashboardUpperManuRegistrationTab").hover(function () {
        $('#dashboardUpperManuRegistrationTab').addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
    }, function () {
        if (!IsRegestrationTabSelected) {
            $('#dashboardUpperManuRegistrationTab').removeClass("dashboardUpperManuSelectedTab");
            $("#dashboardUpperManuLeftPart").removeClass("dashboardUpperManuSelectedTab");
            $("#dashboardUpperManuRegistrationTab").addClass("dashboardUpperManuTabs");
            $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
        }
    });

    // End for registration Tab hover effact
    $(".dashboardUpperManuTabs").click(function () {
        if ($(this).attr('id') == "dashboardUpperManuQuarterlyTab") {
            IsQuaterlyTabSelected = true;
        } else IsQuaterlyTabSelected = false;
    });


    $("#dashboardUpperManuLastPart").hover(function () {
        $('#dashboardUpperManuLastPart').addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuQuarterlyTab").addClass($("#dashboardUpperManuLastPart").attr('class'));
    }, function () {
        if (!IsQuaterlyTabSelected) {
            $('#dashboardUpperManuLastPart').removeClass("dashboardUpperManuSelectedTab");
            $("#dashboardUpperManuQuarterlyTab").removeClass("dashboardUpperManuSelectedTab");
            $("#dashboardUpperManuLastPart").addClass("dashboardUpperManuTabs");
            $("#dashboardUpperManuQuarterlyTab").addClass($("#dashboardUpperManuLastPart").attr('class'));
        }
    });


    // for Quarerly Tab hover effact

    //  end for Quarerly Tab hover effact

    $(".dashboardUpperManuTabs").click(function () {
        $(".dashboardUpperManuSelectedTab").removeClass("dashboardUpperManuSelectedTab");
        $(this).addClass("dashboardUpperManuSelectedTab");
        $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));
        $("#dashboardUpperManuLastPart").addClass($("#dashboardUpperManuQuarterlyTab").attr('class'));
    });
    //for the first time tab selected
    $("#dashboardUpperManuRegistrationTab").addClass("dashboardUpperManuSelectedTab");
    $("#dashboardUpperManuLeftPart").addClass($("#dashboardUpperManuRegistrationTab").attr('class'));



}
function selectTabOnClick()
{ }
//End Manage Page UI state


//Mouse Hover Functions
function MouseHoverFunctions() {

    $("#r-all-1").hover(function () { // mouseent
        $(this).css('background-color', 'gray');
    },
      function () { // mouseleave
          if (!($('#r-ios-1').css('backgroundColor') == 'rgb(223, 223, 223)' && $('#r-an-1').css('backgroundColor') == 'rgb(223, 223, 223)'))
          { $(this).css('background-color', '#DFDFDF'); }
      });

    $("#r-ios-1").hover(function () { // mouseent
        $(this).css('background-color', 'gray');
    },
       function () { // mouseleave

           if (!($('#r-all-1').css('backgroundColor') == 'rgb(223, 223, 223)' && $('#r-an-1').css('backgroundColor') == 'rgb(223, 223, 223)'))
           { $(this).css('background-color', '#DFDFDF'); }
       });

    $("#r-an-1").hover(function () { // mouseent
        $(this).css('background-color', 'gray');
    },
       function () { // mouseleave
           if (!($('#r-ios-1').css('backgroundColor') == 'rgb(223, 223, 223)' && $('#r-all-1').css('backgroundColor') == 'rgb(223, 223, 223)'))
           { $(this).css('background-color', '#DFDFDF'); }
       });

}
//End Mouse Hover Functions


//Annimate Ui Controls
function AnnimateUIControls() {
    AnnimateTopCountryList();

}

function AnnimateTopCountryList() {

    $("#top-countries-list").hide().slideDown(800);

}
//End Annimate Ui Controls


//Manage Css According to Browser
function ManageCssAccordingtoBrowser() {

    if (/chrom(e|ium)/.test(navigator.userAgent.toLowerCase())) {
        $('link[href="CSS/DefaultDashboardStyle.min.css"]').attr('href', 'CSS/ChromDashboardStyleSheet.css');
    } else if (/firefox/.test(navigator.userAgent.toLowerCase())) {

    } else if (navigator.userAgent.match(/msie/i) || navigator.userAgent.match(/trident/i)) {
        $('link[href="CSS/DefaultDashboardStyle.min.css"]').attr('href', 'CSS/IEStyleSheet.css');
    }
}
//End Manage Css According to Browser

//Manage lodder

function HideLodder() {
    $('#loaderRapper').hide();
}
function ShowLodder() {

    $('#loaderRapper').show();
}
//end manage lodder

//messageBox
function showMessage(message, Cssclass) {
    $('#MessageBox').text(message);
    $('#MessageBox').slideDown('slow', function () {
        setTimeout("jQuery('#MessageBox').slideUp('slow');", 2000);
    });
}
//EndMessagebox