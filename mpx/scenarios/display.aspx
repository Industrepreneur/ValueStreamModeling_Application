<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="display.aspx.cs" Inherits="whatif_display" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="tabsNonvisible">
        <h2>What-If Scenario Results Display</h2>
        <div style="margin:10px;padding:5px;">
            <h3>Show Results from Base Case &amp; What-If Scenarios</h3>
            <asp:CheckBoxList ID="lstCheckWhatifs" runat="server" CssClass="chkboxlist"></asp:CheckBoxList>

            <asp:Button ID="btnSelectAll" Text="Select All Scenarios" runat="server" OnClick="btnSelectAll_Click" CssClass="otherButton" />
            <asp:Button ID="btnClearAllWhatifs" Text="Deselect All Scenarios" runat="server" OnClick="btnClearAllWhatifs_Click" CssClass="otherButton" />
            <asp:Button ID="btnReset" Text="Reset" runat="server" OnClick="btnReset_Click" CssClass="otherButton" />
            <br />
            <br />
            <asp:Button ID="btnSaveShowWhatifs" runat="server" Text="Update" OnClick="btnSaveShowWhatifs_Click" CssClass="updateButton" />
        </div>
    </div>
</asp:Content>

