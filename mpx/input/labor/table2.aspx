<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="table2.aspx.cs" Inherits="labor" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/ReactUserControl.ascx" TagPrefix="uc1" TagName="ReactUserControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="content-wrapper">
        <uc1:ReactUserControl runat="server" ID="ReactUserControl" />
    </div>

</asp:Content>

