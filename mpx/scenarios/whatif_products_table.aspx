<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="whatif_products_table.aspx.cs" Inherits="whatif_products_table" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel ID="pnlMenu" runat="server" ></asp:Panel>
    <div id="tabsDiv" class="tabs" runat="server">
        <h3>Products Input Table</h3>
                
            <div class="datatable">
                <div id="buttondiv" runat="server">
                </div>
                <br />
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
                <asp:UpdatePanel ID="pnlUpdateGrid" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                <asp:Panel ID="gridPanel" CssClass="gridPanel" ScrollBars="Auto" runat="server">
                </asp:Panel>
                    </ContentTemplate>
                    </asp:UpdatePanel>

                <asp:Panel ID="secondPanel" runat="server">
                </asp:Panel>
                <br />

                <asp:Panel ID="thirdPanel" runat="server">
                </asp:Panel>

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
    </div>
</asp:Content>
