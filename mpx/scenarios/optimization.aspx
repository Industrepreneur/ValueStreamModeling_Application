<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="optimization.aspx.cs" Inherits="Runchoices" %>


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
                }
            }
            xmlhttp.open("GET", "Progress.ashx?type=r&b=" + new Date().getTime(), true);
            xmlhttp.send();
        }

        var progressClock;

        function scriptVerifyData() {
            document.getElementById('progressDiv').innerHTML = "Verifying data...";
            document.getElementById('btnCancelCalc').style.display = 'none';
            $find('modalPopupLoadingBehavior').show();
        }

        function startProgressCheck() {
            document.getElementById('progressDiv').innerHTML = "Verifying data...";
            document.getElementById('btnCancelCalc').style.display = 'block';
            setTimeout("loadProgress()", 1000);
            try {
                progressClock = setInterval("loadProgress()", 5000);
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
            document.getElementById('progressDiv').innerHTML = "Cancelling calculations...";

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
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div>
        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
        <h2>Run Model</h2>
        <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
        <div id="tabsDiv" class="tabs" runat="server">
            <div class="datacontent">
                <asp:MultiView ID="MPXRunChoices" runat="server">

                    <asp:View ID="PageR1" runat="server">


                        <h3>Basic Calculations</h3>
                        <div style="text-align: center;" class="cmdbutton">
                            <asp:LinkButton ID="Buttonr1_v" runat="server"
                                Text="          --- Verify input data ---         "
                                OnClick="buttonr1_v_click" />
                            <br />


                            <asp:LinkButton ID="Buttonr1_b" runat="server" OnClick="buttonr1_b_click"
                                Text=" --  Calculate results for current case --" />
                            <br />

                            <asp:LinkButton ID="Buttonr1_a" runat="server" OnClick="buttonr1_a_click"
                                Text="---  Calculate results for all (Base Case and What-If Scenarios) as needed ---" />
                            <br />

                        </div>
                        
                    </asp:View>
                    <asp:View ID="PageR2" runat="server">

                        <h3>Lot Size Optimization</h3>
                        <h4 style="margin-bottom: 5px; padding-bottom: 5px;">Parts to Include</h4>
                        <div style="margin-bottom: 3px;">Choose parts for lot size optimization:</div>
                        <div style="width: 480px;">
                            <asp:GridView
                                ID="gridLotsizes"
                                runat="server"
                                AutoGenerateColumns="False"
                                DataKeyNames="ProdID"
                                RowStyle-CssClass="datatable-rowstyle"
                                AlternatingRowStyle-BackColor="White"
                                HeaderStyle-BackColor="#ffa500"
                                FooterStyle-BackColor="#ffa500"
                                DataSourceID="srcOptimizeTable"
                                EmptyDataText="There are no parts defined yet."
                                AllowPaging="true"
                                OnPageIndexChanging="gridLotsizes_PageIndexChanging"
                                OnRowDataBound="gridLotsizes_RowDataBound">

                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Part Name</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblProdDesc" Text='<%# Bind("ProdDesc") %>' runat="server"></asp:Label>
                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <HeaderTemplate>Total Unit Value</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtVal" Text='<%# Bind("Value") %>' runat="server"></asp:TextBox>
                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <HeaderTemplate>Opt. Lotsize</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="checkLotsize" Checked='<%# Bind("optimizelotsize") %>' runat="server" />
                                        </ItemTemplate>

                                    </asp:TemplateField>
                                    
                                </Columns>
                            </asp:GridView>

                        </div>
                        <div class="cmdButtonTable">
                            <table>
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="btnSelectLotsize" Text="Select all Lotsizes" runat="server" OnClick="btnSelectLotsize_Click" /></td>
                                
                                    <td>

                                        <asp:LinkButton ID="btnClearLotsize" Text="Clear all Lotsizes" OnClick="btnClearLotsize_Click" runat="server" /></td>
                               <td><asp:LinkButton ID="btnResetLotsizes" Width="80px" Text="Reset" OnClick="btnResetLotsizes_Click" runat="server" /></td>
                                     </tr>
                            
                                
                                

                            
                                <tr>
                                    <td>
                                        </td>

                                </tr>
                            </table>
                        </div>

                        <asp:AccessDataSource ID="srcOptimizeTable"
                            SelectCommand="SELECT ProdID, ProdDesc, Value, optimizelotsize, optimizetbatch FROM tblprodfore;"
                            runat="server"></asp:AccessDataSource>
                        <div style="margin-top:10px;">
                            <h4 style="margin-bottom: 5px; padding-bottom: 5px;">Retain Recent Modifications with What-If Scenario</h4>
                            <asp:RadioButtonList ID="RadioButtonList3" runat="server" CssClass="rbl"
                                RepeatDirection="Horizontal">
                                <asp:ListItem Value="-1">Retain recent modifications with What-If Scenario</asp:ListItem>
                                <asp:ListItem Value="0">Do not retain recent modifications</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        
                        <div style="margin-top:10px;">
                            <h4 style="margin-bottom: 5px; padding-bottom: 5px;">Save Results In</h4>
                            <div style="margin-bottom: 7px;">Choose where the new lot size information is to be kept:</div>
                            <div>
                                <asp:RadioButtonList ID="RadioButtonList1" CssClass="rbl" runat="server" Visible="False"
                                    RepeatDirection="Horizontal">
                                    <asp:ListItem Value="-1">Keep lot size changes  in Base Case</asp:ListItem>
                                    <asp:ListItem Value="0">Start new What-If Scenario</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:RadioButtonList ID="RadioButtonList2" CssClass="rbl" runat="server"
                                    RepeatDirection="Horizontal">
                                    <asp:ListItem Value="-1">Keep lot size changes in What-If Scenario</asp:ListItem>
                                    <asp:ListItem Value="0">Start new What-If Scenario</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <div style="margin-left: 370px;">
                                Name for new What-If Scenario:&nbsp;&nbsp;
                            <asp:TextBox ID="TextBox9" runat="server"></asp:TextBox>
                            </div>


                            
                            <br />
                            <br />
                            <div class="cmdbuttonMain">
                                <asp:LinkButton ID="Button38" runat="server" OnClick="buttonr4_opt"
                                    Text="Update &amp; Optimize" ViewStateMode="Enabled" />

                                <asp:LinkButton ID="btnWriteTableValues" runat="server" OnClick="btnWriteTableValues_Click" Text="Update Only"></asp:LinkButton>
                            </div>
                            <br />
                            <br />
                            <asp:TextBox ID="TextBox22" runat="server" BorderStyle="None" Height="40px"
                                ReadOnly="True" Rows="1" TextMode="MultiLine" Visible="False" Width="1105px">comments ???</asp:TextBox>
                            <br />
                            <br />
                            <asp:Button ID="Buttonr4_exe2" runat="server"
                                Text="OK - Otimization Cancelled.   Production Must be achieved before starting optimization."
                                Visible="False" />
                            <br />
                            <br />
                        </div>
                    </asp:View>

                    <asp:View ID="viewResults" runat="server">
                        <h3>Errors &amp; Results Summary</h3>
                        <asp:Panel ID="pnlErrors" runat="server">
                            <h4>Warnings/Errors</h4>
                            <asp:Panel ID="Panel1"  runat="server" BorderStyle="none"  CssClass="resultPanel">
                                <asp:Label ID="txtErrors" runat="server" ></asp:Label>
                            </asp:Panel>
                            
                        </asp:Panel>
                        <asp:Panel ID="pnlResults" runat="server">
                            <h4>Results Summary</h4>
                            <asp:Panel  runat="server"   CssClass="resultPanel">
                                <asp:Label ID="txtResults" runat="server"></asp:Label>
                            </asp:Panel>
                            <br />
                            
                            <div class="cmdbutton">
                                <div class="btnrow">
                                    <asp:Panel ID="pnlResultsButtons" runat="server">

                                        <asp:LinkButton ID="Button7" runat="server" OnClick="buttonshowlabor"
                                            Text="Show Labor Results  " UseSubmitBehavior="False" />

                                        <asp:LinkButton ID="Button9" runat="server" OnClick="buttonshowequip"
                                            Text="Show Equipment Results " UseSubmitBehavior="False" />
                                        <asp:LinkButton ID="Button10" runat="server" OnClick="buttonshowproduct"
                                            Text="Show Product Results" UseSubmitBehavior="False" />

                                    </asp:Panel>
                                </div>
                            </div>
                        </asp:Panel>
                        
                    </asp:View>


                </asp:MultiView>
            </div>
        </div>
        <asp:Panel ID="pnlLoading" runat="server" CssClass="msgPanel" style="display: none;">
            <h3>Value Stream Modeling Run</h3>
            <asp:Label ID="lblLoading" style="margin-bottom:30px;" Text="Verification and calculation in progress...<br />This might take a while. Please wait."  runat="server"></asp:Label>
            <br /> <br />
            <div id="progressDiv"></div>
                <br />
                <button id="btnCancelCalc" onclick="stopCalculations();" type="button" style="display:none;">Cancel Calculations</button>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="modalPopupLoading" runat="server"
                    TargetControlID="modalDummyDiv"
                    PopupControlID="pnlLoading"
                    BehaviorID="modalPopupLoadingBehavior"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummyDiv" runat="server"></div>
    </div>
</asp:Content>

