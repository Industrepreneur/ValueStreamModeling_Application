<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="results_iTrees_table.aspx.cs" Inherits="results_iTrees_table" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2>IBOM Trees & Poles</h2>
    <asp:Panel ID="pnlMenu" runat="server"></asp:Panel>
    <div id="tabsDiv" class="tabs" runat="server">
        <h3>IBOM Trees Table and Graph</h3>
        <h4><%= Master.GetFullCurrentWhatifLabel() %></h4>
        <div class="datatable">
            <asp:Label ID="lblChooseProduct" runat="server" Text="Select Assembly: "></asp:Label>
            <asp:DropDownList ID="dropListProducts" runat="server" AutoPostBack="True"
                DataSourceID="srcProductsList" DataTextField="prodDesc"
                DataValueField="ProdID" OnSelectedIndexChanged="dropListProducts_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
            <asp:AccessDataSource ID="srcProductsList"
                SelectCommand="SELECT [ProdID], [ProdDesc] FROM [tblProdFore] ORDER BY ProdDesc"
                runat="server"></asp:AccessDataSource>
            <div id="buttondiv" runat="server"></div>
            <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto">
                <asp:GridView ID="grid"
                    runat="server">
                </asp:GridView>
            </asp:Panel>
            <br />
            <h3>IBOM Tree Graph</h3>
            <asp:Label ID="lblZoom" Text="Zoom: " runat="server"></asp:Label><asp:DropDownList ID="dlZoom" runat="server" OnSelectedIndexChanged="dlZoom_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            <asp:Panel ScrollBars="Auto" CssClass="scrollPanel" ID="pictureHolder" runat="server">
            </asp:Panel>
        </div>
        <asp:Panel runat="server" ID="sortPanelContainer"></asp:Panel>
        <asp:Panel runat="server" ID="copyPanelContainer"></asp:Panel>
    </div>
</asp:Content>

