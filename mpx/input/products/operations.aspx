<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="operations.aspx.cs" Inherits="operations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="hidden">
        <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    </div>


    <header class="inner-header">
        <div class="header-wrapper">

            <div class="header-wrapper-drop">
                <div id="lblDrop" class="icon-drop" onmousedown=""><div class="icon-wrapper"><i class='fa fa-ellipsis-v'></i></div>
                <asp:DropDownList ID="dropListProducts" runat="server" AutoPostBack="True"
                    DataSourceID="srcProductsList" DataTextField="ProdDesc"
                    DataValueField="ProdID" OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged" CssClass="menu-drop">
                </asp:DropDownList>

                </div>

                <asp:AccessDataSource ID="srcProductsList"
                    SelectCommand="SELECT [ProdID], [ProdDesc] FROM [tblProdFore]"
                    runat="server"></asp:AccessDataSource>


            </div>
            <%--            <span class="header-wrapper-text">OPERATIONS</span>--%>

            <div class="page-controls-container">

                <ul class="mnuDrop">

                    <li>
                        <a class="fas fa-ellipsis-v"></a>
                        <ul>
                          
                            <li>
                                <%--  <div title="Show/Hide additional columns" class="mnuDropIcon">--%>
                                <%-- THIS WILL NOT WORK WITH DROPDOWN STYLING...NEED TO IMPLEMENT CONTROL TECHNIQUE USED IN GENERAL.ASPX (SORT/COPY/ADVANCED) --%>
                                <div class="mnuDropIcon" id="buttondiv" runat="server"></div>

                            </li>
                            <li>
                                <div class="mnuDropIcon" title="Click for page instructions" id="dvHelp">
                                    <label class="icon-menu"><i class="fas fa-question-circle fa-fw row-icon"></i><span id="spnHelp">INSTRUCTIONS</span></label>
                                </div>
                                <script>
                                    document.getElementById("dvHelp").addEventListener("click", function () {
                                        alert("Operations required to produce a complete Product using Equipment and Labor");
                                    }, false);
                                </script>
                            </li>


                        </ul>
                </ul>






                <%-- <asp:Button ID="btnShowTimes" OnClick="btnShowTimes_Click" runat="server" Text="Show Real Times" />--%>
                <%-- <asp:Button ID="btnDefaultRoutingPart" OnClick="btnDefaultRoutingPart_Click" OnClientClick="return confirm('Are you sure you want to add default routing to the selected product and thus delete all its current routings?')" runat="server" Text="Add Default Routing" />--%>
            </div>

        </div>
    </header>

    <%-- <div class="content-wrapper">--%>

    <div class="datatable" style="max-height: 50%;">
        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

        <asp:Panel ID="gridPanel" CssClass="gridPanel" runat="server"></asp:Panel>
        <asp:Label ID="lblErrorRealTimesOper" Visible="false" CssClass="errorLabel" runat="server"></asp:Label>
    </div>

    <!-- extra div needed for two row headers -->
    <div class="hidden">


        <asp:Panel ID="secondPanel" runat="server">
        </asp:Panel>


        <asp:Panel ID="thirdPanel" runat="server">
        </asp:Panel>

        <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>

        <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>
    </div>
    <div class="hidden">
        <div class="page-divider">
            <div class="header-wrapper">
                <span class="header-wrapper-text">ROUTING</span>


            </div>
        </div>
        <div class="datatable" style="top: 58%; max-height: 37%;">
            <div class="hidden">
                <asp:Label ID="lblSelectedOper" runat="server" Text="Select Routings: "></asp:Label>
                <asp:DropDownList
                    ID="comboSelectedOper"
                    runat="server"
                    DataSourceID="srcSelectedOper"
                    DataValueField="OpID"
                    DataTextField="OpNam"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="comboSelectedOper_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:AccessDataSource ID="srcSelectedOper"
                    runat="server"></asp:AccessDataSource>
                <div id="buttondivRouting" runat="server">
                    <asp:Button ID="btnRoutingShowTimes" runat="server" Text="Show Real Percentage" OnClick="btnRoutingShowTimes_Click" />
                </div>
            </div>

            <asp:Panel ID="grid2Panel" class="" runat="server">
                <asp:GridView
                    ID="gridRouting"
                    runat="server"
                    CssClass="gridPanel"
                    AutoGenerateColumns="False"
                    DataKeyNames="RecID"
                    RowStyle-CssClass="datatable-rowstyle"
                    EmptyDataText="There are no data records to display."
                    ShowFooter="true"
                    OnRowEditing="gridRouting_RowEditing"
                    OnRowCancelingEdit="gridRouting_RowCancelingEdit"
                    OnRowUpdating="gridRouting_RowUpdating"
                    OnRowDataBound="gridRouting_RowDataBound"
                    OnRowDeleting="gridRouting_RowDeleting"
                    OnRowCommand="gridRouting_RowCommand">

                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" Text="<i class='fa fa-pencil row-icon'></i>" CssClass="icon-button" AssociatedControlID="btnEdit"></asp:Label>
                                <asp:Button runat="server" ID="btnEdit" Text="Edit" CommandName="Edit" CssClass="menu-button" />
                                <asp:Label runat="server" Text="<i class='fa fa-times row-icon'></i>" CssClass="icon-delete" AssociatedControlID="btnDelete"></asp:Label>
                                <asp:Button runat="server" ID="btnDelete" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete the record?');" CssClass="menu-button" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="Update" CssClass="updateButton" />
                                <asp:Button runat="server" ID="btnCanc" Text="Cancel" CommandName="CancelUpdate" CssClass="otherButton" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label runat="server" Text="<i class='fa fa-plus icon-add'></i>" CssClass="row-add" AssociatedControlID="btnInsert"></asp:Label>
                                <asp:Button ID="btnInsert" runat="server" Text="Add" CommandName="Insert" CssClass="menu-button" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:AccessDataSource ID="srcComboOperTo" runat="server"></asp:AccessDataSource>

            <asp:Label ID="lblErrorRealTimesRouting" Visible="false" CssClass="errorLabel" runat="server"></asp:Label>

            <div class="hidden">
                <asp:Panel ID="pnlInsertRouting" runat="server">
                    <asp:TextBox ID="txtInsertRouting" runat="server" TextMode="MultiLine" Rows="6" Width="500" ToolTip="Insert Routings for the selected product."></asp:TextBox>

                    <asp:Button ID="btnInsertrouting" runat="server" Text="Insert" OnClick="btnInsertrouting_Click" />
                </asp:Panel>
                <ajaxToolkit:TextBoxWatermarkExtender ID="TBWE2" runat="server"
                    TargetControlID="txtInsertRouting"
                    WatermarkText="Paste the routings of the selected product here."
                    WatermarkCssClass="watermark" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="TBWEinsertOperTable" runat="server"
                    WatermarkText="Paste the operations of the selected product here."
                    TargetControlID="txtInsertRouting"
                    WatermarkCssClass="watermark" />

            </div>



            <asp:Label ID="lblWarnDefRouting" Visible="false" runat="server"></asp:Label>

        </div>
    </div>




    <input type="hidden" id="selectedRowId" name="selectedRowId" />

    <asp:Label runat="server" ID="lblRow" Visible="false" />
    <asp:LinkButton runat="server" ID="btnDummy" />
    <asp:LinkButton runat="server" ID="btnDummy2" />
    <asp:LinkButton runat="server" ID="btnDummy3" />

    <asp:Panel ID="sortPanelRoutingContainer" runat="server"></asp:Panel>
</asp:Content>

