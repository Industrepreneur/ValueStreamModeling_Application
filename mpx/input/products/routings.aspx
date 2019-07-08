<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="routings.aspx.cs" Inherits="routings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="hidden">
        <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    </div>

       <script>

           
        var myAdvancedString = "<li><div id='dvToggle' class='mnuDropIcon' title='Toggle Data Table Visibility' onmousedown='showAdvanced()'><label id='lblToggle' class='icon-menu'><i class='fas fa-eye-slash fa-fw-slash row-icon'></i></label><span id='spnToggle'>SHOW/HIDE</span></div></li>";
           
        var myDefaultRoutingString = "<li><div class='mnuDropIcon' id='dvPrint' title='Create the Default Routing' onmousedown='clickDefault()'><label id='btn-print' class='icon-menu'><i class='fas fa-sort fa-fw row-icon'></i></label><span id='spnPrint'>TOGGLE AXIS</span></div></li>";
        var myHelpString = "<li><div class='mnuDropIcon' title='Click for page instructions' id='dvHelp'><label class='icon-menu'><i class='fas fa-question-circle fa-fw row-icon'></i><span id='spnHelp'>HELP</span></label></div>";
        var myHelpContext = "Please Help Me";

        var myEllipsisControls = [myAdvancedString, myDefaultRoutingString, myHelpString];

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

           function clickDefault() {
               myVar = $("#btnDefaultRoutingPart");
               myVar.click();
           }
           
     
    </script>


    <script>

         $(function () {

             
             document.getElementById("dropProduct").removeAttribute("disabled");

         });

    </script>

    
        <div style="position: fixed;">
          
                    <asp:DropDownList ClientIDMode="Static" ID="dropListProducts" runat="server" AutoPostBack="True"
                        DataSourceID="srcProductsList" DataTextField="ProdDesc"
                        DataValueField="ProdID" OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged" CssClass="menu-drop">
                    </asp:DropDownList>

                </div>

                <asp:AccessDataSource ID="srcProductsList"
                    SelectCommand="SELECT [ProdID], [ProdDesc] FROM [tblProdFore]"
                    runat="server"></asp:AccessDataSource>

    <asp:Button ClientIDMode="Static" ID="btnDefaultRoutingPart"  OnClick="btnDefaultRoutingPart_Click" OnClientClick="return confirm('Are you sure you want to add default routing to the selected product and thus delete all its current routings?')" runat="server" Text="Add Default Routing" CssClass="hidden"/>
          


                                <div class="mnuDropIcon hidden" id="buttondiv" runat="server"></div>

                            

                            <%--<li>
                                <div class="mnuDropIcon" title="Insert the default routing which assumes 100% of Lots on average will flow to the next operation based on the Operation Number" id="dvSaveAll">
                                    <asp:Label  ToolTip="" runat="server" Text="<i class='fas fa-plus-circle fa-fw row-icon'></i><span>LOAD DEFAULT</span>" AssociatedControlID="btnDefaultRoutingPart" CssClass="icon-menu"> </asp:Label>
                                    <input id="btnDefaultRoutingPart" type="button" runat="server" onserverclick="btnDefaultRoutingPart_Click"
                                        onclick="if (confirm('This will delete the current Routing and insert the default flow between existing Operations') == true) { __doPostBack('btnDefaultRoutingPart', ''); } else { return false; }" value="insert new record" class="hidden" />
                                </div>

                            </li>--%>
                 



                <%--<asp:Button ID="btnDefaultRoutingPart"  OnClick="btnDefaultRoutingPart_Click" OnClientClick="return confirm('Are you sure you want to add default routing to the selected product and thus delete all its current routings?')" runat="server" Text="Add Default Routing" CssClass="hidden"/>--%>
                <%--<asp:Button ID="btnShowTimes" OnClick="btnShowTimes_Click" runat="server" Text="Show Real Times" />--%>
            </div>
    


    <div class="datatable">
        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
        <asp:UpdatePanel ID="pnlUpdateGrid" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <asp:Panel ID="gridPanel" CssClass="gridPanel" ScrollBars="Auto" runat="server">
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>


    <asp:Label ID="lblErrorRealTimes" Visible="false" CssClass="errorLabel" runat="server"></asp:Label>

    <div class="hidden">
        <asp:Panel ID="secondPanel" runat="server">
        </asp:Panel>


        <asp:Panel ID="thirdPanel" runat="server">
        </asp:Panel>

        <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>

        <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>
    </div>


    <div>
        <ajaxToolkit:TextBoxWatermarkExtender ID="TBWEinsertTable" runat="server"
            WatermarkText="Paste the routing records here."
            TargetControlID="txtInsertTable"
            WatermarkCssClass="watermark" />

        <input type="hidden" id="selectedRowId" name="selectedRowId" />

        <asp:Label runat="server" ID="lblRow" Visible="false" />
        <asp:LinkButton runat="server" ID="btnDummy" />
        <asp:LinkButton runat="server" ID="btnDummy2" />
        <asp:LinkButton runat="server" ID="btnDummy3" />
    </div>


</asp:Content>

