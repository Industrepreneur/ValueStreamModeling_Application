<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="account.aspx.cs" Inherits="account" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2>Account</h2>
    <div style="padding-left: 15px; padding-bottom: 15px;" class="tabsNonvisible">
        <div>
            <h3>Set Email Address</h3>
            <p>Setup the email address for resetting password and keep up to date about Value Stream Modeling web application.</p>
            <table style="border-collapse: collapse; max-width: 100%; white-space: nowrap;">
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Enter your email address: " CssClass="tableItem"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="tableItem"></asp:TextBox></td>
                    <td>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail" ID="valEmail" ValidationGroup="email" ValidationExpression="^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$" ErrorMessage="Invalid email format." CssClass="tableItem" Display="Dynamic"></asp:RegularExpressionValidator></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Confirm your password: " CssClass="tableItem"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtEmailPwd" TextMode="Password" runat="server" CssClass="tableItem"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator runat="server" ID="valPwdReq" ValidationGroup="email" ErrorMessage="To change your email current password is required." ControlToValidate="txtEmailPwd" CssClass="tableItem" Display="Dynamic"></asp:RequiredFieldValidator></td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnSetEmail" runat="server" Text="Set Email" OnClick="btnSetEmail_Click" CssClass="buttonMain" CausesValidation="true" ValidationGroup="email" />
        </div>
        <div>
            <h3>Change Password</h3>
            <p>You can change your password</p>
            <table style="border-collapse: collapse; max-width: 100%; white-space: nowrap;">
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Enter your current password: " CssClass="tableItem"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtPwdCurr" TextMode="Password" runat="server" CssClass="tableItem"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator runat="server" ID="valPwdCurr" ValidationGroup="password" ErrorMessage="Current password is required." ControlToValidate="txtPwdCurr" CssClass="tableItem" Display="Dynamic"></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Enter new password: " CssClass="tableItem"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtPwdNew" TextMode="Password" runat="server" CssClass="tableItem"></asp:TextBox></td>
                    <td>
                        <asp:RegularExpressionValidator runat="server" ID="valPwdNew" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,64}" 
                            ErrorMessage="Password should be between 8 and 64 characters, and contain at least 1 Uppercase letter, 1 Lowercase letter, and 1 number. Special characters are not allowed" ForeColor="Red" ValidationGroup="password" ControlToValidate="txtPwdNew" Display="Dynamic"></asp:RegularExpressionValidator><asp:RequiredFieldValidator runat="server" ID="valPwdNewEmpty" ValidationGroup="password" ErrorMessage="New password is required." ControlToValidate="txtPwdNew" CssClass="tableItem" Display="Dynamic"></asp:RequiredFieldValidator></td>
                    <%--(?=.*[$@$!%*?&])--%>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Confirm new password: " CssClass="tableItem"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtPwdNewConf" TextMode="Password" runat="server" CssClass="tableItem"></asp:TextBox></td>
                    <td>
                        <asp:CompareValidator runat="server" ID="valPwdNewConf" ControlToCompare="txtPwdNew" ControlToValidate="txtPwdNewConf" ErrorMessage="The passwords do not match." ValidationGroup="password" CssClass="tableItem" Display="Dynamic"></asp:CompareValidator></td>
                </tr>
            </table>
            <br />
            <asp:Button ID="btnChangePwd" runat="server" Text="Change Password" OnClick="btnChangePwd_Click" CssClass="buttonMain" CausesValidation="true" ValidationGroup="password" />
        </div>
    </div>
</asp:Content>

