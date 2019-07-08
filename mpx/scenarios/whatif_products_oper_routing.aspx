<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="whatif_products_oper_routing.aspx.cs" Inherits="whatif_products_oper_routing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    <div id="tabsDiv" class="tabs" runat="server">
        <h3>Product Operations/Routing Table</h3>

        <div class="datatable">
            <asp:Label ID="lblDropListProducts" runat="server" Text="Select Product to Display: "></asp:Label>
            <asp:DropDownList ID="dropListProducts" runat="server" AutoPostBack="True"
                DataSourceID="srcProductsList" DataTextField="prodDesc"
                DataValueField="ProdID" OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
            <asp:AccessDataSource ID="srcProductsList"
                SelectCommand="SELECT [ProdID], [ProdDesc] FROM [tblProdFore]"
                runat="server"></asp:AccessDataSource>
            <div id="buttondiv" runat="server">
            </div>
            <asp:Button ID="btnShowTimes" OnClick="btnShowTimes_Click" runat="server" Text="Show Real Times" />
            <br />
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
            
            <asp:Panel ID="gridPanel" CssClass="gridPanel" ScrollBars="Auto" runat="server">
            </asp:Panel>
                    
            </div><!-- extra div needed for two row headers -->
        
        <div class="datatable">
            <asp:Label ID="lblErrorRealTimesOper" Visible="false" CssClass="errorLabel" runat="server"></asp:Label>
            <asp:Panel ID="secondPanel" runat="server">
            </asp:Panel>
            <br />

            <asp:Panel ID="thirdPanel" runat="server">
            </asp:Panel>

            <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>

            <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>
        </div>

            <div class="datatable">
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
                <asp:Panel ID="grid2Panel" runat="server">
                    <asp:GridView
                        ID="gridRouting"
                        runat="server"
                        AutoGenerateColumns="False"
                        DataKeyNames="RecID"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500"
                        FooterStyle-BackColor="#ffa500"
                        EmptyDataText="There are no data records to display."
                        ShowFooter="false"
                        OnRowEditing="gridRouting_RowEditing"
                        OnRowCancelingEdit="gridRouting_RowCancelingEdit"
                        OnRowUpdating="gridRouting_RowUpdating"
                        OnRowDataBound="gridRouting_RowDataBound"
                        OnRowCommand="gridRouting_RowCommand">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnEdit" Text="Edit" CommandName="Edit" CssClass="otherButton" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="Update" CssClass="updateButton" />
                                    <asp:Button runat="server" ID="btnCanc" Text="Cancel" CommandName="CancelUpdate" CssClass="otherButton" />
                                </EditItemTemplate>
                                
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </asp:Panel>
                <asp:AccessDataSource ID="srcComboOperTo" runat="server"></asp:AccessDataSource>
                <br />
                <asp:Label ID="lblErrorRealTimesRouting" Visible="false" CssClass="errorLabel" runat="server"></asp:Label>
                <br />
                <asp:Label ID="lblWarnDefRouting" Visible="false" runat="server"></asp:Label>
                
                
            </div>
        </div>


        <input type="hidden" id="selectedRowId" name="selectedRowId" />

        <asp:Label runat="server" ID="lblRow" Visible="false" />
        <asp:LinkButton runat="server" ID="btnDummy" />
        <asp:LinkButton runat="server" ID="btnDummy2" />
        <asp:LinkButton runat="server" ID="btnDummy3" />

        <asp:Panel ID="sortPanelRoutingContainer" runat="server"></asp:Panel>

        

    </div>
</asp:Content>

