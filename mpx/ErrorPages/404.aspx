<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="404.aspx.cs" Inherits="ErrorPages_404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="height:100%;width:100%;background-color:white; display:flex;">
        <div style="display:flex; margin:auto;">
        <div style="padding: 0px 12.5vw 0px 0px">
            <i class="fa fa-map-marker" style="color:#50A2A7; font-size:40vh; padding: 10px;"></i>
        </div>
        <div style="margin:auto;padding: 0px 12.5vw 0px 9vw;">
            <i class="fa fa-code" style="font-size: 4vh;"></i><span style="font-size: 4vh; text-align:left;"> ERROR 404</span>
            <p>You have navigated to a forbidden directory or nonexistent page!</p>
            <p>Please find your way around the site using the navigation menu.</p>
        </div>
            </div>
    </div>

</asp:Content>

