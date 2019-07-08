<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="table.aspx.cs" Inherits="products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div style="display: none;">
        <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    </div>

      <script>
          //THIS NEEDS TO BE LINKED TO STEVE'S FUNCTION
          var myChartString = "<li><div id='dvToggle' class='mnuDropIcon' title='Toggle Data Table Visibility' onmousedown='showAdvanced()'><label id='lblToggle' class='icon-menu'><i class='fas fa-eye-slash fa-fw-slash row-icon'></i></label><span id='spnToggle'>SHOW/HIDE</span></div></li>";
          var myHelpString = "<li><div class='mnuDropIcon' title='Click for page instructions' id='dvHelp'><label class='icon-menu'><i class='fas fa-question-circle fa-fw row-icon'></i><span id='spnHelp'>HELP</span></label></div>";
          var myHelpContext = "Please Help Me";

          var myEllipsisControls = [myChartString, myHelpString];

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

   
                                <div class="mnuDropIcon hidden" id="buttondiv" runat="server"></div>




    <div class="content-wrapper">

        <div class="datatable">

            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

            <div class="table-header"></div>
            <div class="table-body">
                <asp:UpdatePanel ID="pnlUpdateGrid" UpdateMode="Conditional" runat="server">

                    <ContentTemplate>
                        <asp:Panel ID="gridPanel" CssClass="gridPanel" runat="server"></asp:Panel>
                    </ContentTemplate>

                </asp:UpdatePanel>
            </div>
            <div class="table-foot"></div>


        </div>

        <div class="panels" style="display: none;">

            <div id="pnl-second">
                <asp:Panel ID="secondPanel" runat="server"></asp:Panel>
            </div>

            <div id="pnl-third" style="display: none;">
                <asp:Panel ID="thirdPanel" runat="server"></asp:Panel>
            </div>

            <div id="pnl-fourth">
                <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>
            </div>

            <div id="pnl-fifth">
                <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>
            </div>

            <ajaxToolkit:TextBoxWatermarkExtender ID="TBWEinsertTable" runat="server"
                WatermarkText="Paste the equipment records here."
                TargetControlID="txtInsertTable"
                WatermarkCssClass="watermark" />
        </div>

    </div>




    <div>

        <input type="hidden" id="selectedRowId" name="selectedRowId" />
        <asp:Label runat="server" ID="lblRow" Visible="false" />
        <asp:LinkButton runat="server" ID="btnDummy" />
        <asp:LinkButton runat="server" ID="btnDummy2" />
        <asp:LinkButton runat="server" ID="btnDummy3" />

    </div>



</asp:Content>

