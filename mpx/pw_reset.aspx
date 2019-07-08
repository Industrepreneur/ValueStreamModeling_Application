<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pw_reset.aspx.cs" Inherits="pw_reset" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="mpxstyle.css" rel="stylesheet" type="text/css" />
    <link href='https://fonts.googleapis.com/css?family=Roboto:500,400' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Roboto+Condensed:400,300' rel='stylesheet' type='text/css'/>
    <link href='https://fonts.googleapis.com/css?family=Droid+Serif' rel='stylesheet' type='text/css'/>
    <script type="text/javascript">
        function doFocus(buttonName, e) {
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                var btn = document.getElementById(buttonName);
                if (btn != null && !btn.disabled) {
                    btn.focus();
                }
            }
        }
    </script>
    <title>Value Stream Modeling</title>
</head>
<body>
    
    <form style="padding-top: 0px; margin-top: 0px;" id="mpxForm" method="post" runat="server">
            
    <div class="page shadow">
        
        
            <div class="header">

                <div style="float:right;text-align:right; margin-top:-5px; font-weight:bold;margin-right:10px;">
                <div style="height: 35px; font-size: 13px; font-family: 'Roboto', Verdana;"><span style="color: #b9e0bb;">Build</span><span style="color:#f7b2b3;">-To-</span><span style="color: #b0c8e2;">Demand</span><span style="color: #d7d7d7;">, Inc.</span></div>
                <div>
                    
                </div>
                    </div>
                
                
                
               
                <img src="Images/logo.png" height="40" style="margin-left:38%" class="mpxTitleImage" />
                <h1 class="mpxTitle" style="margin-bottom:5px;">Value Stream Modeling</h1>
                 
                 <div class="line" style="clear: both;"></div>
            </div>
            <div class="content" style="background-color:#FFFFFF;">
                <div style="padding:20px;">
                <h2>Password Reset</h2>
                    <p>Please type and confirm your new password for Value Stream Modeling website.</p>
                <table style="border-collapse:collapse; max-width:100%; white-space:nowrap;">
        <tr>
            <td><asp:Label ID="lblPwd" runat="server" Text="Enter new password: " CssClass="tableItem"></asp:Label></td><td><asp:TextBox ID="txtPwdNew" TextMode="Password" runat="server" CssClass="tableItem" TabIndex="1"></asp:TextBox></td><td><div class="tableItem"><asp:RegularExpressionValidator runat="server" ID="valPwdNew" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,10}"
ErrorMessage="Password must contain: Minimum 8 characters, at least 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Digit and 1 Special Character." ForeColor="Red" ValidationGroup="password" ControlToValidate="txtPwdNew" Display="Dynamic"></asp:RegularExpressionValidator><asp:RequiredFieldValidator runat="server" ID="valPwdNewEmpty" ValidationGroup="password" ErrorMessage="New password is required." ControlToValidate="txtPwdNew" Display="Dynamic"></asp:RequiredFieldValidator></div></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblPwdNew" runat="server" Text="Confirm new password: " CssClass="tableItem"/></td><td><asp:TextBox ID="txtPwdNewConf" TextMode="Password" runat="server" CssClass="tableItem" TabIndex="2"></asp:TextBox></td><td><asp:CompareValidator runat="server" ID="valPwdNewConf" ControlToCompare="txtPwdNew" ControlToValidate="txtPwdNewConf" ErrorMessage="The passwords do not match." ValidationGroup="password" CssClass="tableItem"></asp:CompareValidator></td>
        </tr>
    </table>
           <asp:Button ID="btnChangePwd" runat="server" Text="Change Password" OnClick="btnChangePwd_Click" CausesValidation="true" ValidationGroup="password" CssClass="buttonMain" TabIndex="3" UseSubmitBehavior="false" />
          
           <asp:HiddenField ID="hdnLink" runat="server" />         </div>
    </div>
    </div>
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <ajaxToolkit:ModalPopupExtender ID="modalInfo" runat="server"
                TargetControlID="dummy"
                PopupControlID="pnlInfo"
                BehaviorID="infoBehavior"
                BackgroundCssClass="modalBackground"
                DropShadow="true">
            </ajaxToolkit:ModalPopupExtender>

            <asp:Panel ID="pnlInfo" CssClass="msgPanel" runat="server">
                <h3>Info Message</h3>
                <asp:Label ID="lblInfoModal" Text="Your password was reset successfully.<br/>You can now log in." runat="server"></asp:Label>
                <br />
                <br />

                <asp:Button ID="btnOk" OnClick="btnOk_Click" runat="server" Text="Ok" Width="60" />


            </asp:Panel>
            <div runat="server" id="dummy"></div>

        <div class="page footer">
        
        <a href="http://www.build-to-demand.com"><img src="Images/logo.png" style="height:68px;margin-left:auto;margin-right:auto; margin-top:20px;margin-bottom:0px;" /></a>
        <div style="margin-top:1px;">
            E-mail: <a href="mailto:Info@Build-To-Demand.com">Info@Build-To-Demand.com</a>
            <br />
            Copyright &copy; 2013  Build-To-Demand, Inc.
        </div>
            </div>
       
    </form>
</body>
</html>
