<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="table.aspx.cs" Inherits="general" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

      <script>
          //THIS NEEDS TO BE LINKED TO STEVE'S FUNCTION
          var myChartString = "<li><div id='dvToggle' class='mnuDropIcon' title='Toggle Data Table Visibility' onmousedown='showAdvanced()'><label id='lblToggle' class='icon-menu'><i class='fas fa-eye-slash row-icon'></i></label><span id='spnToggle'>SHOW/HIDE</span></div></li>";
          var mySaveString = "<li><div id='' class='mnuDropIcon' title='Save' onmousedown=''><label id='' class='icon-menu'><i class='fas fa-save row-icon'></i></label><span id=''>SAVE CHANGES</span></div></li>";
          var myClearString = "<li><div id='' class='mnuDropIcon' title='Clear unsaved changes' onmousedown=''><label id='' class='icon-menu'><i class='far fa-ban row-icon'></i></label><span id=''>CLEAR</span></div></li>";
          var myHelpString = "<li><div class='mnuDropIcon' title='Click for page instructions' id='dvHelp'><label class='icon-menu'><i class='fas fa-question-circle fa-fw row-icon'></i><span id='spnHelp'>HELP</span></label></div>";
          var myHelpContext = "Please Help Me";

          var myEllipsisControls = [myChartString, mySaveString, myClearString, myHelpString];

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


             document.getElementById("back-button").setAttribute("disabled", "disabled");
             document.getElementById("inner-header-dropdown").style.visibility = "hidden";

         });

    </script>

                            
                                <div class="mnuDropIcon hidden" title="Save changes" id="dvSaveAll">
                                    <asp:Label ID="lblSaveAll" runat="server" Text="<i class='fa fa-save row-icon'></i><span id='spnSaveAll'>SAVE</span>" AssociatedControlID="btnSaveAll" CssClass="icon-menu"></asp:Label>
                                    <asp:Button ID="btnSaveAll" runat="server" Text="SAVE" CausesValidation="true" OnClick="SaveAll" CssClass="hidden" />
                                </div>
                            
                                <div class="mnuDropIcon hidden" title="Clear unsaved changes" id="dvReset">
                                    <asp:Label ID="lblReset" runat="server" Text="<i class='fa fa-refresh'></i> <span id='spnReset'>RESET</span>" AssociatedControlID="btnReset" CssClass="icon-menu" ToolTip=""></asp:Label>
                                    <asp:Button ID="btnReset" Text="RESET" ToolTip="" CausesValidation="false" OnClick="btnReset_Click" runat="server" CssClass="hidden" />
                                </div>
                         
                                <div class="mnuDropIcon hidden" title="Show Optional Parameters" id="dvAdvanced">
                                    <asp:Label ID="lblAdvanced" runat="server" AssociatedControlID="btnAdvanced" Text="<i class='fas fa-eye-slash fa-fw-slash row-icon'></i><span id='spnAdvanced'>SHOW/HIDE</span>" CssClass="icon-menu" ToolTip="Show Optional Parameters"></asp:Label>
                                    <asp:Button ID="btnAdvanced" Text="Show Optional Parameters" CausesValidation="false" OnClick="btnAdvanced_Click" runat="server" CssClass="hidden" />

                                </div>
                       
                            
      

    <div class="content-wrapper">
        <table class="simpleTableText" >
            <thead>
                <tr>
                    <th id="col_1" scope="col">
                        <asp:Label ID="Label1" runat="server" Text="Name" CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_2" scope="col">
                        <asp:Label ID="Label2" runat="server" Text="Forecast Time Period" ToolTip="Forecast Period, this is the period for the demand and production in the products screen e.g. 500 pieces per (year, quarter, month, week)" CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_5" scope="col">
                        <asp:Label ID="labelcv1" runat="server" Text="# of DAYS" ToolTip="The number of working days contained within the Demand Forcast Time Period" CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_3" scope="col">
                        <asp:Label ID="Label3" runat="server" Text="Leadtime UOM" ToolTip="Critical Path Time is measured in Calendar Days" CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_4" scope="col">
                        <asp:Label ID="Labelcv2" runat="server" Text="TIME per DAY" ToolTip="Number of time units in one Working Day." CssClass="lblItem"></asp:Label>

                    </th>
                    <th id="col_6" scope="col">
                        <asp:Label ID="Label4" runat="server" Text="Operation UOM" ToolTip="Unit of Measure for setup and processing times" CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_7" scope="col" class="collapse" runat="server">
                        <asp:Label ID="Label8" runat="server" Text="Max Equipment Utilization" ToolTip="Planning target for utilization levels. Suggested between 80% to 95%, do not exceed 98%"
                            CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_8" scope="col" class="collapse" runat="server">
                        <asp:Label ID="Label5" runat="server" Text="Labor Variability" ToolTip="The amount of variation in the duration of Labor setup and run times following a normal distribution. Suggested above 10%"
                            CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_9" scope="col" class="collapse" runat="server">
                        <asp:Label ID="Label6" runat="server" Text="Equipment Variability" ToolTip="The amount of variation in the duration of Equipment setup and run times following a normal distribution. Suggested above 10%"
                            CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_10" scope="col" class="collapse" runat="server">
                        <asp:Label ID="Label7" runat="server" Text="Product Variability" ToolTip="The amount of variation between the start time of each product lot. Suggested above 10%"
                            CssClass="lblItem"></asp:Label>
                    </th>
                    <th id="col_11" scope="col">
                        <asp:Label ID="Label9" runat="server" Text="Comment" CssClass="lblItem"></asp:Label>
                    </th>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td id="ttl1">
                        <input id="title1" type="text" tabindex="1" runat="server" />
                        <%--<asp:TextBox ID="title1" runat="server" TextMode="SingleLine" TabIndex="1" CssClass="editItem"></asp:TextBox>--%>
                    </td>

                    <%--               <script>
                        document.getElementById("ttl1").addEventListener("mouseover", function () {
                            if (document.getElementById("ttl1").style.backgroundColor == "#f3f3f3") {
                                document.getElementById("ttl1").style.backgroundColor = "#fdfdfd";
                            } else {
                                document.getElementById("ttl1").style.backgroundColor = "#f3f3f3";
                            }
                        
                        }, false);
                        document.getElementById("ttl1").addEventListener("mouseout", function () {
                            if (document.getElementById("ttl1").style.backgroundColor == "#fdfdfd") {
                                document.getElementById("ttl1").style.backgroundColor = "#f3f3f3";
                            } else {
                                document.getElementById("ttl1").style.backgroundColor = "#fdfdfd";
                            }

                        }, false);
                    </script>--%>


                    <td>
                        <asp:DropDownList ID="dtu" runat="server" TabIndex="3" CssClass="editItem">
                            <asp:ListItem>YEAR</asp:ListItem>
                            <asp:ListItem>QUARTER</asp:ListItem>
                            <asp:ListItem>MONTH</asp:ListItem>
                            <asp:ListItem>WEEK</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="cv2" runat="server" MaxLength="45" Rows="1" TabIndex="6" CssClass="editItem align-center">indemand</asp:TextBox>
                    </td>
                    <td class="disabled">
                        <asp:TextBox ID="mcttu" runat="server" Rows="1" Enabled="false" TabIndex="4" CssClass="editItem">mct</asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="cv1" runat="server" MaxLength="45" Rows="1" TabIndex="7" CssClass="editItem align-center">indemand</asp:TextBox>
                    </td>

                    <td>
                        <asp:DropDownList
                            ID="optu" runat="server" MaxLength="45" Rows="1" TabIndex="5" CssClass="editItem">
                            <asp:ListItem>SEC</asp:ListItem>
                            <asp:ListItem>MIN</asp:ListItem>
                            <asp:ListItem>HOUR</asp:ListItem>
                            <asp:ListItem>HOUR * 100</asp:ListItem>
                            <asp:ListItem>HOUR * 1000</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td id="col_7_txt" class="collapse" runat="server">
                        <asp:TextBox ID="utlimit" runat="server" MaxLength="45" Rows="1" TabIndex="8"
                            CssClass="editItem align-center"></asp:TextBox>
                    </td>
                    <td id="col_8_txt" class="collapse" runat="server">
                        <asp:TextBox ID="laborcv" runat="server" MaxLength="45" Rows="1" TabIndex="9"
                            CssClass="editItem align-center"></asp:TextBox>
                    </td>
                    <td id="col_9_txt" class="collapse" runat="server">
                        <asp:TextBox ID="equipcv" runat="server" MaxLength="45" Rows="1" TabIndex="10"
                            CssClass="editItem align-center"></asp:TextBox>
                    </td>
                    <td id="col_10_txt" class="collapse" runat="server">
                        <asp:TextBox ID="partcv" runat="server" MaxLength="45" Rows="1" TabIndex="11"
                            CssClass="editItem align-center">indemand</asp:TextBox>
                    </td>
                    <td>
                        <input id="comment" type="text" runat="server" tabindex="7" />
                        <%--<asp:TextBox ID="comment" TextMode="single" runat="server" Rows="1" TabIndex="2" CssClass="editItem"></asp:TextBox>--%>
                    </td>
                </tr>
            </tbody>

        </table>


    </div>



    <%--***********************************************************************************************--%>
    <%--             <h3>Model Title</h3>
            <div class="contentMargin">
                <table class="simpleTable">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="MPX Model Title" CssClass="lblItem"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="title1" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="1" CssClass="editItem"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="alternativeColor" style="text-align: center;">
                            <asp:Label ID="Label9" runat="server" Text="Comment" CssClass="lblItem"></asp:Label>
                        </td>
                        <td class="alternativeColor">
                            <asp:TextBox ID="comment" TextMode="MultiLine" runat="server" Rows="3" TabIndex="2" CssClass="editItem"></asp:TextBox>
                        </td>
                    </tr>
                </table>

            </div>
            <h3>Time Units</h3>
            <div class="contentMargin">
                <table class="simpleTable">
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Demand Forecast Time Period" ToolTip="Forecast Period, this is the period for the demand and production in the products screen e.g. 500 pieces per (year, quarter, month, week)" CssClass="lblItem"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="dtu" runat="server" TabIndex="3" CssClass="editItem">
                                <asp:ListItem>YEAR</asp:ListItem>
                                <asp:ListItem>QUARTER</asp:ListItem>
                                <asp:ListItem>MONTH</asp:ListItem>
                                <asp:ListItem>WEEK</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>

                    <tr>
                        <td class="alternativeColor">
                            <asp:Label ID="Label3" runat="server" Text="Operational Manufacturing Critical-path Time Unit" ToolTip="MCT time unit, the program uses calendar days." CssClass="lblItem"></asp:Label>
                        </td>
                        <td class="alternativeColor">
                            <asp:TextBox ID="mcttu" runat="server" Height="19px" Rows="1" Enabled="false" TabIndex="4" CssClass="editItem">mct</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Operations Time Unit" ToolTip="Operations Time Unit, this is the time unit for what operation lengths will be measured in e.g. 10 (seconds, minutes, hours, etc.)." CssClass="lblItem"></asp:Label>
                        </td>
                        <td>

                            <asp:DropDownList
                                ID="optu" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="5" CssClass="editItem">
                                <asp:ListItem>SEC</asp:ListItem>
                                <asp:ListItem>MIN</asp:ListItem>
                                <asp:ListItem>HOUR</asp:ListItem>
                                <asp:ListItem>HOUR * 100</asp:ListItem>
                                <asp:ListItem>HOUR * 1000</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>

            </div>
            <h3>Time Unit Conversion</h3>
            <div class="contentMargin">
                <table class="simpleTable" style="float: left;">

                    <tr>
                        <td>
                            <asp:Label ID="labelcv1" runat="server" Text="No. of WORKING DAYS per Demand Forecast Time Period" CssClass="lblItem"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="cv2" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="6" CssClass="editItem">indemand</asp:TextBox>
                        </td>

                    </tr>
                    <tr>
                        <td class="alternativeColor">
                            <asp:Label ID="Labelcv2" runat="server" Text="No. of Operations Time Units in 1 WORKING DAY" ToolTip="Number of time units in one day." CssClass="lblItem"></asp:Label>
                        </td>

                        <td class="alternativeColor">
                            <asp:TextBox ID="cv1" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="7" CssClass="editItem">indemand</asp:TextBox>

                        </td>

                    </tr>
                </table>

                <table class="validatorTableConv">

                    <tr>
                        <td>
                            <asp:CustomValidator ValidateEmptyText="true" runat="server" ID="valCv2" ControlToValidate="cv2" ClientValidationFunction="validator" ErrorMessage="Enter a value > 0!" />

                        </td>
                    </tr>
                    <tr>
                        <td>

                            <asp:CustomValidator ValidateEmptyText="true" runat="server" ID="valCv1c" ControlToValidate="cv1" ClientValidationFunction="validator" ErrorMessage="Enter a value > 0!"></asp:CustomValidator>
                        </td>

                    </tr>
                </table>


            </div>



            <asp:Panel ID="pnlAdvanced" Height="210px" Visible="false" runat="server">
                <h3>Optional Parameters</h3>
                <div class="contentMargin">
                    <table class="simpleTable" style="float: left;">
                        <tr>
                            <td>
                                <asp:Label ID="Label8" runat="server" Text="Maximum Utilization of Resouces (<= 95.0%)" ToolTip="The maximum amount of the theoretical time that a machine or labor group is can be scheduled to operate."
                                    CssClass="lblItem"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="utlimit" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="8"
                                    CssClass="editItem"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="alternativeColor">
                                <asp:Label ID="Label5" runat="server" Text="Variability in Labor Operational Times (>= 10.0%)" ToolTip="The amount of variation in how long an operation requiring labor takes following a normal distribution about the inputted time for the operation."
                                    CssClass="lblItem"></asp:Label>
                            </td>

                            <td class="alternativeColor">
                                <asp:TextBox ID="laborcv" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="9"
                                    CssClass="editItem"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text="Variability in Equipment Operational Times (>= 10.0%)" ToolTip="The amount of variation in how long an operation requiring machines takes following a normal distribution about the inputted time for the operation."
                                    CssClass="lblItem"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="equipcv" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="10"
                                    CssClass="editItem"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="alternativeColor">
                                <asp:Label ID="Label7" runat="server" Text="Variability in Product Start Times  (>= 10.0%)" ToolTip="The variation in overall time that a product takes to be started. Variability between different start time for different lots"
                                    CssClass="lblItem"></asp:Label>
                            </td>
                            <td class="alternativeColor">
                                <asp:TextBox ID="partcv" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="11"
                                    CssClass="editItem">indemand</asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table class="validatorTable">
                        <tr>
                            <td>
                                <asp:CustomValidator runat="server" ID="valUtlimit" ControlToValidate="utlimit" ClientValidationFunction="rangeUtilValidator" ErrorMessage="Invalid value." />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CustomValidator runat="server" ID="valLaborcv" ControlToValidate="laborcv" Type="Double" ClientValidationFunction="validatorCv" ErrorMessage="Invalid value." />

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CustomValidator runat="server" ID="valEquipcv" ControlToValidate="equipcv" ClientValidationFunction="validatorCv" ErrorMessage="Invalid value." />

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CustomValidator runat="server" ID="valPartcv" ControlToValidate="partcv" ClientValidationFunction="validatorCv" ErrorMessage="Invalid value." />

                            </td>
                        </tr>
                    </table>


                </div>
                <div style="float: left;">
                    <asp:Button ID="btnSaveAdvanced" Text="Update Advanced" OnClick="SaveAdvanced" TabIndex="11" runat="server" Visible="false" />
                </div>
            </asp:Panel>
        </div>
        </div>--%>
</asp:Content>



