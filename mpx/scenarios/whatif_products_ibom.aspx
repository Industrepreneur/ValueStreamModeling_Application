<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="whatif_products_ibom.aspx.cs" Inherits="whatif_products_ibom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    <div id="tabsDiv" class="tabs" runat="server">
        <h3>Product IBOM</h3>

        <div class="datatable">
            <h3 style="text-align:left;">View IBOM Structure</h3>
            
            <asp:Button ID="btnCopyAllIbom" Text="Copy All IBOM Records" OnClick="btnCopyAllIbom_Click" runat="server" />
            <br /> <br />
            <div id="copyAllDummy" runat="server"></div>
            <asp:Label ID="lblDropListProducts" runat="server" Text="Choose Assembly: "></asp:Label>
            <asp:DropDownList ID="dropListProducts" runat="server" AutoPostBack="True"
                DataSourceID="srcProductsList" DataTextField="ParentName"
                DataValueField="ParentID" OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:AccessDataSource ID="srcProductsList"
                SelectCommand="SELECT distinct ParentName, ParentID FROM [tblibom] ORDER BY ParentName;"
                runat="server"></asp:AccessDataSource>

            <div id="buttondiv" runat="server">
            </div>
            <br /> 
            <asp:CheckBox ID="checkAllSubComponents" AutoPostBack="true" Text="Show All Subcomponents" OnCheckedChanged="checkAllSubComponents_CheckedChanged" runat="server" />
            <br /><asp:Label ID="lblNote" Text="Note: + preceding Product Name means it has indented Bill of Materials." runat="server"></asp:Label>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
            <asp:Panel ID="gridPanel" CssClass="gridPanel" ScrollBars="Auto" runat="server">
            </asp:Panel>
            <asp:Panel ID="secondPanel" runat="server">
            </asp:Panel>

            <br />
        </div>
        <hr style="background-color: #5E7BAC; color: #5E7BAC; height: 2px; border: 0px; width: 85%" />
        <div class="datatable">
            <h3 style="text-align:left;">Build IBOM Structure</h3>
            <div>
                <asp:Label ID="lblParentProduct" runat="server" Text="Choose Parent Product: "></asp:Label>
                <asp:DropDownList
                    ID="dropListProducts2"
                    runat="server"
                    DataSourceID="srcProductStructure"
                    DataTextField="ProdDesc"
                    DataValueField="ProdId"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="dropListProducts2_SelectedIndexChanged">
                </asp:DropDownList>

                <asp:AccessDataSource ID="srcProductStructure"
                    SelectCommand="SELECT ProdId, ProdDesc FROM [tblprodfore] ORDER BY ProdDesc;"
                    runat="server"></asp:AccessDataSource>
            </div>

            <div style="margin-bottom:20px;">
                <asp:Panel ID="grid2Panel" runat="server">
                    <asp:GridView
                        ID="gridBuildIbom"
                        runat="server"
                        AutoGenerateColumns="False"
                        DataKeyNames="IbomID"
                        CssClass="gridMargin"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500"
                        FooterStyle-BackColor="#ffa500"
                        EmptyDataText="There are no components to this product."
                        ShowFooter="false"
                        OnRowEditing="gridRouting_RowEditing"
                        OnRowCancelingEdit="gridRouting_RowCancelingEdit"
                        OnRowUpdating="gridRouting_RowUpdating"
                        OnRowDataBound="gridRouting_RowDataBound"
                        OnRowCommand="gridRouting_RowCommand">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnEdit" Text="Edit" CommandName="Edit" />
                                </ItemTemplate>

                                <EditItemTemplate>
                                    <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="Update" />
                                    <asp:Button runat="server" ID="btnCancelUpdate" Text="Cancel" CommandName="CancelUpdate" />
                                </EditItemTemplate>

                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </asp:Panel>
                <asp:AccessDataSource ID="srcComboOperTo" runat="server"></asp:AccessDataSource>
            </div>
            
            
            <div>
                <asp:Panel ID="thirdPanel" runat="server">
                </asp:Panel>
            </div>
        </div>



        <asp:Panel ID="fourthPanel" runat="server"></asp:Panel>

        <asp:Panel ID="fifthPanel" runat="server"></asp:Panel>

        <div>

            <input type="hidden" id="selectedRowId" name="selectedRowId" />

            <asp:Label runat="server" ID="lblRow" Visible="false" />
            <asp:LinkButton runat="server" ID="btnDummy" />
            <asp:LinkButton runat="server" ID="btnDummy2" />
            <asp:LinkButton runat="server" ID="btnDummy3" />
        </div>
    </div>
</asp:Content>
