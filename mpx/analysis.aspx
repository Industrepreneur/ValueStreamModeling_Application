<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="analysis.aspx.cs" Inherits="analysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
     function loadProgress() {
           var xmlhttp;
           if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
               xmlhttp = new XMLHttpRequest();
           }
           else {// code for IE6, IE5
               xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
           }
           xmlhttp.onreadystatechange = function () {
               if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                   document.getElementById('progressDiv').innerHTML = xmlhttp.responseText;
                   if (xmlhttp.responseText.toLowerCase().indexOf("saving") == -1) {
                       document.getElementById('btnCancelCalc').style.display = 'block';
                   }
               }
           }
           xmlhttp.open("GET", "Progress.ashx?type=a&b=" + new Date().getTime(), true);
           xmlhttp.send();
     }

     var progressClock;

     function scriptSaveAnalysis() {
         document.getElementById('progressDiv').innerHTML = "Creating/Saving Vision Analysis...";
         document.getElementById('btnCancelCalc').style.display = 'none';
         $find('modalPopupLoadingBehavior').show();
     }

     function startProgressCheck() {
         document.getElementById('progressDiv').innerHTML = "";
         document.getElementById('btnCancelCalc').style.display = 'none';
         setTimeout("loadProgress()", 1000);
         try {
             progressClock = setInterval("loadProgress()", 10000);
         } catch (e) {

         }

     }
     

     function stopProgressCheck() {
         try {
             window.clearInterval(progressClock);
         } catch (e) {

         }
     }

     function stopCalculations() {
         document.getElementById('btnCancelCalc').style.display = 'none';
         stopProgressCheck();
         document.getElementById('progressDiv').innerHTML = "Canceling calculations...";

         var xmlhttp;
         if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
             xmlhttp = new XMLHttpRequest();
         }
         else {// code for IE6, IE5
             xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
         }
         xmlhttp.onreadystatechange = function () {
             if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                 document.getElementById('progressDiv').innerHTML = xmlhttp.responseText;
             }
         }
         xmlhttp.open("GET", "StopCalc.ashx?" + new Date().getTime(), true);
         xmlhttp.send();
     }

     
     </script>
  <!--   new !!!  <style type="text/css">
        .editItem
        {}
    </style> -->
	
    <style type="text/css">
        .arrowButton
        {}
    </style>
	
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <h2>Vision Analysis</h2>
    <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    <div id="tabsDiv" class="tabs" runat="server">
        <div class="datacontent">
            <asp:MultiView ID="AnalysisMultiView" runat="server">
                <asp:View ID="ViewCaseLoad" runat="server">
                    <div style="float: left; margin-right: 120px;">
                        <h3 style="text-align: left;">My Vision Analyses</h3>
                        <asp:ListBox ID="lstCases" runat="server" Width="300px" Height="200px" Font-Names="Verdana" DataTextField="Name" DataValueField="AnalysisID" ></asp:ListBox>
                        <br />
                        <asp:Button ID="btnLoadCase" runat="server" Text="Reopen Vision Analysis" OnClick="btnLoadCase_Click"></asp:Button>
                        <asp:Button ID="btnDeleteCase" runat="server" Text="Delete Vision Analysis" OnClick="btnDeleteCase_Click" TabIndex="8"
                            OnClientClick="return confirm('Are you sure you want to delete the selected Vision Analysis?');"></asp:Button>
                        <br />
                    </div>
                    <div>
                        <h3 style="text-align: left;">Start a new Vision Analysis</h3>
                        Enter a name for the new Vision Analysis:
                    <asp:TextBox ID="txtNewCase" runat="server"></asp:TextBox>
                        <br />
                        <asp:Button ID="btnNewCase" runat="server" Text="Start a New Vision Analysis" OnClick="btnNewCase_Click" />
                        <asp:ImageButton ID="btnAnalysisHelp" runat="server" ToolTip="Show Vision Analysis Purpose" ImageUrl="~/Images/help_icon.png" style="width:32px; margin-left:0px; margin-bottom:-8px; margin-top:10px;" />
                        <br />
                    </div>
                </asp:View>
                <asp:View ID="ViewCaseSettings" runat="server">
                    <h3>Analysis Settings</h3>
                    <h3 style="text-align: left;"><%= GetFullCurrentCaseLabel() %></h3>
                    <div class="cmdbuttonMainSmaller">
                        <asp:LinkButton ID="btnRun1" runat="server" Text="Run Analysis" OnClick="btnRun_Click" style="padding-left:40px;padding-right:40px;"></asp:LinkButton>
                      
                        &nbsp;&nbsp;&nbsp;&nbsp;
                  

                        <asp:LinkButton runat="server" Text="Close Analysis" ID="btnClose" OnClick="btnClose_Click"></asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                          <asp:ImageButton ID="btnAnalysisHelp2" runat="server" ToolTip="Show Vision Analysis Purpose" ImageUrl="~/Images/help_icon.png" style="width:32px; margin-left:-12px; margin-bottom:-8px; margin-top:5px;" />

                    </div>

                    <h3 style="text-align: left;">General Analysis Description</h3>
                    <table class="simpleTable" style="margin-bottom: 5px;">
                        <tr>
                            <td>
                                <asp:Label ID="lblCaseTitle" runat="server" Text="Analysis Name" CssClass="lblItem"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCaseName" runat="server" Height="19px" MaxLength="55" 
                                    Rows="1" TabIndex="1" CssClass="editItem" Width="166px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="alternativeColor">
                                <asp:Label ID="lblCaseComment" runat="server" Text="Comment" CssClass="lblItem"></asp:Label>
                            </td>
                            <td class="alternativeColor">
                                <asp:TextBox ID="txtCaseComment" TextMode="MultiLine" runat="server" Rows="5" TabIndex="2" CssClass="editItem"></asp:TextBox>
                            </td>
                            
                        </tr>
                    </table>

                      <asp:LinkButton ID="LinkButton12" runat="server" Text="Update" CssClass="updateButton" OnClick="nameupdate_click" style="padding-left:40px;padding-right:40px;"></asp:LinkButton>
                    <br /><br />
                    <h3 style="text-align: left;">Analysis Contents</h3>
                    <div>
                        
                        
                        <div style="float:left;overflow:hidden;margin-bottom:15px;">
                            What-If Scenarios Excluded from Analysis*:
                            <br />
                            <br />
                            <asp:ListBox ID="lstWhatifOut" runat="server" Width="298px" Height="118px" 
                                Font-Names="Verdana"></asp:ListBox>
                            
                        </div>
                        <div style="float:left;overflow:hidden;">
                            <asp:ImageButton ID="btnRight" runat="server" 
                                ImageUrl="~/Images/arrow_right.png" 
                                ToolTip="Add Selected Scenario to Vision Analysis" CssClass="arrowButton" 
                                OnClick="btnRight_Click" Height="127px" Width="210px" />
                            <br />
                        </div>
                        <div style="float:left;margin-bottom:15px;">
                            What-If Scenarios Included in Analysis**:
                            <br />
                            <br />
                            <asp:GridView ID="gridWhatifs" runat="server"></asp:GridView>
                            
                        </div>
                        <div style="margin-top:10px;clear:left;">
                            <p>* Select a What-If Scenario and click on arrow to include the Scenario in the Vision Analysis.<br />
                                
                                ** Click on the button 'Remove from Vision' to exclude the What-If Scenario from the Vision Analysis.<br /><br />
                                   Future changes to basecase and whatifs will follow into the Vision Analysis automatically when [Run Analysis] is clicked again after those changes. <br />
                            </p>
                            </div>
                    </div>
                    <h3 style="text-align: left;">Variable Settings</h3>
                    <h4>Main Variable</h4>
                    <div style="display: inline;">
                        <div style="display: inline-block;">
                            <div style="margin-bottom: 30px;"> 
							
                                <asp:RadioButton runat="server" ID="RadioButton2" Text="Product Demand Multiplier" Checked="true" GroupName="variableType" /> &nbsp;&nbsp;
                                <asp:DropDownList runat="server" ID="dropListProduct">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Department 1</asp:ListItem>
                                    <asp:ListItem>Product 1</asp:ListItem>
                                </asp:DropDownList> &nbsp;&nbsp;
                                <asp:LinkButton ID="LinkButton10" runat="server" Text="Update" CssClass="updateButton" OnClick="droplistprodChange" style="padding-left:40px;padding-right:40px;"></asp:LinkButton>
                            </div>
                            <br />
                            <div style="display: none;">
                                <asp:RadioButton runat="server" ID="rdbtnLabor" Text="Labor Capacity" GroupName="variableType" />
                                <asp:DropDownList runat="server" ID="dropListLabor">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Labor 1</asp:ListItem>

                                </asp:DropDownList>
                                <br />
                                <asp:RadioButton runat="server" ID="RadioButton1" Text="Equipment Capacity" GroupName="variableType" />
                                <asp:DropDownList runat="server" ID="dropListEquip">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Equip 1</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                        </div>
                        <div style="display: none; margin-bottom: -10px;">
                            <table style="margin-bottom: 0px; margin-left: 20px;">
                                <tr>
                                    <td>From:&nbsp;</td>
                                    <td>
                                        <asp:TextBox ID="txtMainVariableFrom" runat="server" Text="1"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>To:&nbsp;</td>
                                    <td>
                                        <asp:TextBox ID="txtMainVariableTo" runat="server" Text="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Step size:&nbsp;</td>
                                    <td>
                                        <asp:TextBox ID="txtMainVariableStep" runat="server" Text="1"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div style="display: none;">
                        <h4>Optional Parameters</h4>
                        <div style="display: inline;">
                            <div style="display: inline-block; margin-right: 50px;">
                                Select optional parameter for analysis:
                        <br />
                                <br />
                                <asp:RadioButton runat="server" ID="rdbtnOptLabor" Text="Labor Capacity" Checked="true" GroupName="parameterType" />
                                <asp:DropDownList runat="server" ID="dropListLaborOpt">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Labor 1</asp:ListItem>

                                </asp:DropDownList>
                                <br />
                                <asp:RadioButton runat="server" ID="rdbtnOptEquip" Text="Equipment Capacity" GroupName="parameterType" />
                                <asp:DropDownList runat="server" ID="dropListEquipOpt">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Equip 1</asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <div style="display: none;">
                                    <asp:RadioButton runat="server" ID="rdbtnOptProduct" Text="Product Demand" GroupName="parameterType" />

                                    <asp:DropDownList runat="server" ID="dropListProdOpt">
                                    </asp:DropDownList>
                                </div>
                                <br />
                                <br />
                                Parameter value:&nbsp;
                        <asp:TextBox ID="txtParameter" runat="server" Text="1"></asp:TextBox>
                                <div class="cmdbutton">
                                    <asp:LinkButton ID="btnAddParameter" runat="server" Text="Add Parameter" ToolTip="Add analysis output with selected parameter value" OnClick="btnAddParameter_Click"></asp:LinkButton>
                                </div>
                            </div>
                            <div style="display: inline-block;">
                                Current Parameter List:
                            <br />
                                <br />
                                <asp:ListBox ID="lstParameters" Height="110" runat="server"></asp:ListBox>
                                <br />
                                <div style="display: inline-block; margin-top: 15px;" class="cmdButtonRowMini">
                                    <asp:LinkButton ID="btnDeleteParameter" Text="Delete Parameter" runat="server" OnClick="btnDeleteParameter_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="cmdbuttonMain">
                        <asp:LinkButton ID="btnRun2" runat="server" Text="Run Analysis" OnClick="btnRun_Click" ></asp:LinkButton>
                    </div>
                    <br />
                    <asp:Panel ID="pnlError" CssClass="errorPopPanel" Width="250" runat="server" style="display:none">
                     <h3>Error Message</h3>
                    
                     <asp:Label ID="lblGeneralError" runat="server">
                     </asp:Label>
                      <br />
                      <asp:Button Style="width: 60px;" ID="btnOkError" runat="server" Text="Ok" OnClientClick="HidePopup('modalPopupError'); return true;" />
                    </asp:Panel>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server"></asp:ScriptManagerProxy>
                <ajaxToolkit:ModalPopupExtender ID="modalExtenderError" runat="server"
                    TargetControlID="modalDummy"
                    PopupControlID="pnlError"
                    BehaviorID="modalPopupError"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                    <br />
                    <br />
                </asp:View>
                <asp:View ID="ViewCaseOutput" runat="server">
                    <h3>Analysis Output</h3>
                    
                    <br />
                    Select Output Variable:
                        <br />
                    <br />
                    <asp:RadioButton runat="server" ID="rdbtnOutputGeneral" Text="General" Checked="true" GroupName="outputType" />
                    <asp:DropDownList runat="server" ID="dropListOutputGeneral">
                        <asp:ListItem Text="Throughput" Value="TotalProd"></asp:ListItem>
                        <asp:ListItem Text="Scrap" Value="Scrap"></asp:ListItem>
                        <asp:ListItem Text="Total WIP" Value="WIP"></asp:ListItem>
                        <asp:ListItem Text="Average MCT" Value="FlowTime"></asp:ListItem>
                    </asp:DropDownList>
                    &nbsp; for product: &nbsp;
                    <asp:DropDownList runat="server" ID="dropListOutputProductType">

                    </asp:DropDownList>
                    <br />
                    
                        <asp:RadioButton runat="server" ID="rdbtnOutputLabor" Text="Labor" GroupName="outputType" />
                        <asp:DropDownList runat="server" ID="dropListOutputLabor">
                            <asp:ListItem Text="Utilization" Value="Util"></asp:ListItem>
                            <asp:ListItem Text="WIP" Value="WIP"></asp:ListItem>

                        </asp:DropDownList>
                    &nbsp; for labor: &nbsp;
                        <asp:DropDownList runat="server" ID="dropListOutputLaborType">

                        </asp:DropDownList>
                        <br />
                        <asp:RadioButton runat="server" ID="rdbtnOutputEquip" Text="Equipment" GroupName="outputType" />
                        <asp:DropDownList runat="server" ID="dropListOutputEquip">
                            <asp:ListItem Text="Utilization" Value="Util"></asp:ListItem>
                            <asp:ListItem Text="WIP" Value="WIP"></asp:ListItem>
                        </asp:DropDownList>
                    &nbsp; for equipment: &nbsp;
                    <asp:DropDownList runat="server" ID="dropListOutputEquipType">

                        </asp:DropDownList>
                        <br />
                    <div style="display: none;">
                        <asp:RadioButton runat="server" ID="rdbtnOutputProduct" Text="Product" GroupName="outputType" />
                        <asp:DropDownList runat="server" ID="DropDownList6">
                            <asp:ListItem>Throughput</asp:ListItem>
                            <asp:ListItem>MCT</asp:ListItem>
                            <asp:ListItem>WIP</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <br />
                    <br />
                    <div class="cmdbutton">
                        <asp:LinkButton runat="server" ID="btnShowAnalysis" Text="Show Results" OnClick="btnShowAnalysis_Click"></asp:LinkButton>&nbsp;&nbsp;
                        <asp:ImageButton ID="btnAnalysisHelp3" runat="server" ToolTip="Show Vision Analysis Purpose" ImageUrl="~/Images/help_icon.png" style="width:32px; margin-left:-12px; margin-bottom:-8px; margin-top:5px;" />
                    </div>
                    <br />
                    <br />
                    <asp:Label ID="lblNoResults" runat="server" Visible="false" Text="There is no data to display. Please run the Vision Analysis first."></asp:Label>
                    <asp:Panel ID="pnlOutputGraphs" runat="server" Visible="false">
                        <div style="float: left; margin-bottom: 20px; margin-right: 20px;">
                            <asp:Panel ID="pnlChart" runat="server" ScrollBars="Auto">

                                <asp:Chart ID="chart" runat="server">
                                    <Titles>
                                        <asp:Title Text="" Font="Trebuchet MS, 18pt, style=Bold">
                                        </asp:Title>
                                    </Titles>

                                    <Series>
                                    </Series>
                                    <ChartAreas>

                                        <asp:ChartArea Name="Results"></asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </asp:Panel>
                        </div>
                        <div style="display: table; margin-bottom: 20px;">
                            <asp:Panel ID="pnlTableButtons" runat="server"></asp:Panel>
                            <asp:Panel ID="pnlGrid" runat="server" ScrollBars="Auto">
                                
                                <asp:GridView ID="grid" Style="margin-top: 5px;"
                                    runat="server">
                                    <Columns>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                        <div style="clear:both;"></div>
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
            <asp:Panel ID="pnlWantSaveModel" CssClass="msgPanel" Style="display: none;" Width="250" runat="server">
                <h3>Save Current What-If Scenario?</h3>
                <asp:Label ID="lblWantSaveModel" Text="Do you wish to save the current What-If Scenario?" runat="server">
                    
                </asp:Label>
                <br />
                <br />

                <asp:Button Style="width: 60px;" ID="btnWantSaveWhatif" runat="server" Text="Save" OnClick="btnWantSaveWhatif_Click" OnClientClick="HideWantSaveModel();return true;" />
                <asp:Button ID="btnDontSaveWhatif" runat="server" Text="Don't Save" OnClick="btnDontSaveWhatif_Click" OnClientClick="HideWantSaveModel();return true;" />
                <asp:Button ID="btnCancelOpen" runat="server" Text="Cancel" OnClientClick="HideWantSaveModel();return false;" />
                <asp:HiddenField ID="hdnAction" runat="server" />

            </asp:Panel>

            <ajaxToolkit:ModalPopupExtender ID="modalPopupWantSaveWhatif" runat="server"
                TargetControlID="dummyWantSave"
                PopupControlID="pnlWantSaveModel"
                BehaviorID="modalPopupWantSaveBehavior"
                BackgroundCssClass="modalBackground"
                DropShadow="true">
            </ajaxToolkit:ModalPopupExtender>
            <div id="dummyWantSave" runat="server"></div>

            <asp:Panel ID="pnlAnalysisHelp" CssClass="msgPanel" Style="display: none;" Width="400" runat="server">
                <h3>About Value Stream Modeling Vision Analysis</h3>
                In order for a user to present a series of choices and different circumstances to management and other people a method to create graphs that represent a vision of what happens when has been added to the program.  This method is called Vision Analysis.
        <br />
                <br />

                Vision Analysis is a way for a user to create and view a series of curves with out needing to create each individual point.  Each vision consists of 1 or more whatif scenarios to be included in the analysis.  The user also chooses how to vary demand for 1 or more or all products.  After the calculations are completed (1 for each demand level and whatif combination) the user can then request a graph of the throughput, WIP level or MCT (manufacturing Critical-path Time) as the demand changes.  Each curve represents a whatif and visa versa each whatif is a curve in the graph.
        <br />
                <br />


                <asp:Button style="width: 60px;" id="btnAnalysisHelpOk" runat="server"  OnClientClick="return false;"  Text="Ok"/>

            </asp:Panel>

            <ajaxToolkit:ModalPopupExtender ID="extenderHelp" runat="server"
                TargetControlID="dummyHelp"
                OkControlID="btnAnalysisHelpOk"
                PopupControlID="pnlAnalysisHelp"
                BehaviorID="modalPopupHelpBehavior"
                BackgroundCssClass="modalBackground"
                DropShadow="true">
            </ajaxToolkit:ModalPopupExtender>
            <div id="dummyHelp" runat="server"></div>
            <div id="btnDummy3" runat="server"></div>
            <asp:Panel ID="pnlNewAnalysisName" runat="server"></asp:Panel>

            <asp:Panel ID="pnlLoading" runat="server" CssClass="msgPanel" style="display: none;">
            <h3>Value Stream Modeling Vision Analysis</h3>
            <asp:Label ID="lblLoading" style="margin-bottom:30px;" Text="Vision Analysis in progress...<br />This might take a while. Please wait."  runat="server"></asp:Label>
            <br /> <br />
            <div id="progressDiv"></div>
                <br />
                <button id="btnCancelCalc" onclick="stopCalculations();" type="button" style="display:none;">Cancel Calculations</button>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="modalPopupLoading" runat="server"
                    TargetControlID="modalDummyAnalysis"
                    PopupControlID="pnlLoading"
                    BehaviorID="modalPopupLoadingBehavior"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummyAnalysis" runat="server"></div>
        </div>
        <asp:Panel ID="pnlCopyTableContainer" runat="server"></asp:Panel>
    </div>
</asp:Content>

