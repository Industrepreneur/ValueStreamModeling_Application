<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="summary.aspx.cs" Inherits="E_Table" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <%-- GOOGLE CHARTS IMPLEMENTATION. --%>
    <%--TO-DO
    LINK WINDOW ONRESIZE TRIGGER TO MYCHART.DRAW--%>
    <%--TODO: LINK SHOW/HIDE TO SPACEBAR AND ENTER--%>
    <script type="text/javascript">

        google.charts.load('current', { 'packages': ['corechart', 'controls', 'table'] });
        google.charts.setOnLoadCallback(startGoogle);


        //THIS FUNCTION TRIGGERS WHEN GOOGLE CHARTS LIBRARIES ARE LOADED; RUNS SERVER-SIDE CODE TO QUERY USER MODEL
        //-- TO DO --
        //-- TRY TO PASS REFERENCE TO CURRENT CLASS TO SERVER-SIDE SUCH AS "THIS"
        //-- REQUIRED TO CREATE STATIC DATA TO PASS TO STATIC SERVER-SIDE [WEB METHOD]
        function startGoogle() {

            $.ajax(
                {

                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    url: '/api/mpx/v1/output/api_summary.aspx/SetGoogleData',

                    success: function (response) {
                        drawVisualization(response.d); // calling method  
                    },

                    error: function () {
                        alert("Error loading data! Please try again.");
                    }

                });



        }

        //THIS FUNCTION INITIALIZES THE CHART -- SPL 1/14/2018
        function drawVisualization(dataValues) {

            data = new google.visualization.DataTable();


            data.addColumn('string', 'PRODUCT');
            data.addColumn('string', 'GROUP');
            data.addColumn('number', 'Avg. Lotsize');
            data.addColumn('string', 'SCENARIO');
            data.addColumn('number', 'NUM');
            data.addColumn('string', 'DESCRIPTION');
            data.addColumn('string', 'EQUIPMENT');
            data.addColumn('string', 'LABOR');
            data.addColumn('number', 'LEAD-TIME');
            data.addColumn('number', 'WFL');

            data.addColumn('number', 'WFE');
            data.addColumn('number', 'WFLot');
            data.addColumn('number', 'SETUP');
            data.addColumn('number', 'RUN');

            data.addColumn('number', 'SETUPS');
            data.addColumn('number', 'WIP');
            data.addColumn('number', 'Visits1');
            data.addColumn('number', 'Visits2');


            for (var i = 0; i < dataValues.length; i++) {

                var opNum = Number(dataValues[i][5]);
                var lotSize = Number(dataValues[i][19]);
                var setups = Number(dataValues[i][18]);
                var wip = Number(dataValues[i][21]);
                var time = Number(dataValues[i][20]);
                var wfl = Number(dataValues[i][7]);
                var wfe = Number(dataValues[i][8]);
                var wflot = Number(dataValues[i][9]);
                var run = Number(dataValues[i][13]);
                var setup = Number(dataValues[i][14]);
                var visits1 = Number(dataValues[i][16]);
                var visits2 = Number(dataValues[i][17]);
                var e_setup = Number(dataValues[i][6]);
                var e_run = Number(dataValues[i][10]);
                var l_setup = Number(dataValues[i][12]);
                var l_run = Number(dataValues[i][11]);
                var equipment = dataValues[i][0];
                var scenario = dataValues[i][1];
                var labor = dataValues[i][15];
                var product = dataValues[i][2];
                var group = dataValues[i][3];
                var opName = dataValues[i][4];

                if (scenario == "BASECASE") {
                    scenario = null
                }

                data.addRow([product, group, lotSize, scenario, opNum, opName, equipment, labor, time, wfl, wfe, wflot, setup, run, setups, wip, visits1, visits2]);


            }

            if (data.getNumberOfRows() != 0) {


                //formatter = new google.visualization.NumberFormat({
                //    pattern: "###.##'%'"
                //});


                //formatter.format(data, 3);
                //formatter.format(data, 4);
                //formatter.format(data, 5);
                //formatter.format(data, 6);
                //formatter.format(data, 7);
                //formatter.format(data, 8);

                //formatter = new google.visualization.NumberFormat({
                //    pattern: "###.##"
                //});

                //formatter.format(data, 9);
                //formatter.format(data, 10);
                //formatter.format(data, 11);
                //formatter.format(data, 12);
                //formatter.format(data, 13);
                //formatter.format(data, 14);
                //formatter.format(data, 15);
                //formatter.format(data, 16);
                //formatter.format(data, 17);
                //formatter.format(data, 18);

                data.sort([0, 3, 4]);

                dashboard = new google.visualization.Dashboard(document.getElementById("dvChart"))

                var classes = {
                    headerCell: 'googHeaderCell',
                    tableCell: 'googRow',
                    oddTableRow: 'googOddRow',
                    tableRow: 'googTableRow',
                    hoverTableRow: 'googHoverRow',

                };


                myTable = new google.visualization.ChartWrapper({
                    'chartType': 'Table',
                    'containerId': 'dvMyTable',
                    'options': { width: '100%', cssClassNames: classes }


                });

                var scenarioFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter2',
                    'options': {
                        'filterColumnIndex': 3,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A SCENARIO' }
                    }
                });
                var productFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter1',
                    'options': {
                        'filterColumnIndex': 0,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A PRODUCT' }
                    }
                });
                var groupFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter0',
                    'options': {
                        'filterColumnIndex': 1,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A FAMILY' }
                    }
                });
                var equipmentFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter3',
                    'options': {
                        'filterColumnIndex': 6,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A EQUIPMENT' }
                    }
                });

                var laborFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter4',
                    'options': {
                        'filterColumnIndex': 7,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A LABOR' }
                    }
                });



                dashboard.bind(scenarioFilter, myTable);
                dashboard.bind(groupFilter, myTable);
                dashboard.bind(productFilter, myTable);
                dashboard.bind(equipmentFilter, myTable);
                dashboard.bind(laborFilter, myTable);
                dashboard.bind(productFilter, equipmentFilter);
                dashboard.bind(productFilter, laborFilter);
                dashboard.bind(groupFilter, productFilter);
                dashboard.bind(equipmentFilter, laborFilter);

                //google.visualization.events.addListener(myTable, 'ready', function () {
                //    var x = document.getElementsByClassName("google-visualization-table-table")[0];
                //    x.style.position = "absolute";
                //});

                dashboard.draw(data);


            } else {

                alert("Data cannot be drawn");

            }

        }



        $(function () {
            $(window).on("resize", debounce(googleScale, 250, false));
        });

        function debounce(func, wait, immediate) {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        };

        function googleScale() {
            //add on resize event listener to call this function. Grab SVG width. Compare to new viewWidth. Calculate scaler.
            myTable.draw();
            //startGoogle();
        }


        function googleCopy() {
            var sel = window.getSelection();
            sel.removeAllRanges();
            var myTableforCopy = $(".google-visualization-table-table")[0];
            var range = document.createRange();
            range.selectNode(myTableforCopy);
            sel.addRange(range);

            try {
                // Now that we've selected the anchor text, execute the copy command  
                var successful = document.execCommand('copy');
                var msg = successful ? 'successful' : 'unsuccessful';
                alert('Copy command was ' + msg);
            } catch (err) {
                alert('Oops, unable to copy');
            }

            sel.removeAllRanges();
        }



    </script>

    <%--GENERIC PAGE FUNCTIONS--%>
    <script>

        //CONTROLS FILTER CONTAINER -- SPL 1/14/2018
        function googleFilter() {

            //var tempHeight = document.querySelector(".inner-header").style.getPropertyValue("--header-height").trim();
            var tempPos = document.getElementById("filter").style.getPropertyValue("--header-pos").trim();

            if (tempPos === "-100%") {

                document.getElementById("filter").style.setProperty("--header-pos", "" + 0);

            } else {

                document.getElementById("filter").style.setProperty("--header-pos", -100 + "%");

            }

        }


    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script>


        var myCopyString = "<li><div class='mnuDropIcon' id='dvCopy' title='Show Data Table' onmousedown='googleCopy()'><label id='lblCopy' title='Show Data' class='icon-menu'><i class='fas fa-copy fa-fw row-icon'></i></label><span id='spnCopy'>COPY DATA</span></div></li>";
        var myHelpString = "<li><div class='mnuDropIcon' title='Click for page instructions' id='dvHelp'><label class='icon-menu'><i class='fas fa-question-circle fa-fw row-icon'></i><span id='spnHelp'>HELP</span></label></div>";
        var myHelpContext = "Please Help Me";

        var myEllipsisControls = [myCopyString, myHelpString];

        $(function () {

            var printString = "";
            var len = myEllipsisControls.length;
            for (i = 0; i < len; i++) {
                printString += myEllipsisControls[i];
            }
            document.getElementById("page-control-content").innerHTML = printString;
            document.getElementById("dvHelp").addEventListener("click", function () {
                alert(myHelpContext);
            }, false);
        });

    </script>


    <script>

        $(function () {

            document.getElementById("add-button").setAttribute("disabled", "disabled");
            document.getElementById("back-button").setAttribute("disabled", "disabled");
            //document.getElementById("inner-header-dropdown").style.visibility = "hidden";

        });

        var filters = 5;
        var printString = "";

        for (i = 0; i < filters; i++) {
            printString += "<div id='filter" + i + "'></div>";
        }

        document.getElementById("filterContainer").innerHTML = printString;

        document.getElementById("filter").style.height = "15vh";
        function googleFilter() {

            //var tempHeight = document.querySelector(".inner-header").style.getPropertyValue("--header-height").trim();
            var tempPos = document.getElementById("filter").style.getPropertyValue("--header-pos").trim();

            if (tempPos === "-15vh") {

                document.getElementById("filter").style.setProperty("--header-pos", "" + 0);

            } else {

                document.getElementById("filter").style.setProperty("--header-pos", -15 + "vh");

            }

        }

    </script>

    <div class="" id="data-content">

        <div id="dvTable" class="" style="height: 100%; width: 100%; max-height: 100%;">

            <div id="dvMyTable" class="" style="width: 100%; height: 100%; max-height: 100%;">
            </div>
        </div>
    </div>





</asp:Content>
