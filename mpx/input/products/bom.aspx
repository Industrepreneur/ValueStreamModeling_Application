<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="bom.aspx.cs" Inherits="P_IBOM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="hidden">
        <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    </div>


    <header class="inner-header">
        <div class="header-wrapper">
            <div class="header-wrapper-drop">
                <div id="lblDrop" class="icon-drop" onmousedown="">
                    <div class="icon-wrapper"><i class='fa fa-ellipsis-v'></i></div>
                    <asp:DropDownList
                        ID="dropListProducts"
                        runat="server"
                        CssClass="menu-drop"
                        DataSourceID="srcProductStructure"
                        DataTextField="ProdDesc"
                        DataValueField="ProdId"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged">
                    </asp:DropDownList>

                    <asp:AccessDataSource ID="srcProductStructure"
                        SelectCommand="SELECT ProdId, ProdDesc FROM [tblprodfore] ORDER BY ProdDesc;"
                        runat="server"></asp:AccessDataSource>

                </div>




            </div>

            <div class="page-controls-container">

                <ul class="mnuDrop">

                    <li>
                        <a class="fas fa-ellipsis-v"></a>
                        <ul>
                            <li>

                                <div class="mnuDropIcon" id="buttondiv" runat="server"></div>
                                
                                <asp:CheckBox ID="checkAllSubComponents" CssClass="hidden" AutoPostBack="true" OnCheckedChanged="checkAllSubComponents_CheckedChanged" runat="server" />
                            </li>

                    
                            <li>
                                <div class="mnuDropIcon" title="Click for page instructions" id="dvHelp">
                                    <label class="icon-menu"><i class="fas fa-question-circle fa-fw row-icon"></i><span id="spnHelp">INSTRUCTIONS</span></label>
                                </div>
                                <script>
                                    document.getElementById("dvHelp").addEventListener("click", function () {
                                        alert("Select a Product to begin defining how the material will flow through the operations starting from Dock and ending at Stock. Begin by selecting a starting Operation, then an ending Operation, and finally the percent of lots that take this path on average");
                                    }, false);
                                </script>
                            </li>


                        </ul>
                </ul>

            </div>
        </div>
    </header>

    <div class="datatable">

        

        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
         <asp:UpdatePanel ID="pnlUpdateGrid" UpdateMode="Conditional" runat="server">

                    <ContentTemplate>
                        <asp:Panel ID="gridPanel" CssClass="gridPanel" runat="server"></asp:Panel>
                    </ContentTemplate>

                </asp:UpdatePanel>
       
    </div>

 <asp:Panel ID="secondPanel" runat="server">
        </asp:Panel>
    <div class="hidden">

    <div>
        <asp:Panel ID="thirdPanel" runat="server">
        </asp:Panel>
       
    </div>
        <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>

        <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>
        <input type="hidden" id="selectedRowId" name="selectedRowId" />

        <asp:Label runat="server" ID="lblRow" Visible="false" />
        <asp:LinkButton runat="server" ID="btnDummy" />
        <asp:LinkButton runat="server" ID="btnDummy2" />
        <asp:LinkButton runat="server" ID="btnDummy3" />
    </div>

</asp:Content>

