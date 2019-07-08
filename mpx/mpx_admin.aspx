<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mpx_admin.aspx.cs" Inherits="mpx_admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Value Stream Modeling Admin</title>
    <link href="mpxbuttons.css" rel="stylesheet" type="text/css" />
    <link href="mpxstyle.css" rel="stylesheet" type="text/css" />
    <link href="ajaxStyles.css" rel="stylesheet" type="text/css" />
 
    <link id="Link2" rel="shortcut icon" type="image/x-icon" href="~/favicon.ico" />
    <link id="Link1" runat="server" rel="icon" href="~/favicon.ico" type="image/ico" />
    
    <link href='https://fonts.googleapis.com/css?family=Roboto:500,400' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Roboto+Condensed:400,300' rel='stylesheet' type='text/css'/>
    <link href='https://fonts.googleapis.com/css?family=Droid+Serif' rel='stylesheet' type='text/css'/>
    <script>
        var pageLoaded = false;
        function enableForm() {
            pageLoaded = true;
        }
        window.onload = enableForm;
        function ShowProgress() {
            setTimeout(function () {
                try {
                    var modalPopupLoading = $find('behLoadingPopup');
                    modalPopupLoading.show();

                }
                catch (ex) {

                }
            }, 100);
        }
        //function loadXMLDoc() {
        //    var xmlhttp;
        //    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        //        xmlhttp = new XMLHttpRequest();
        //    }
        //    else {// code for IE6, IE5
        //        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        //    }
        //    xmlhttp.onreadystatechange = function () {
        //        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
        //            seconds_left = xmlhttp.responseText;
        //        }
        //    }
        //    xmlhttp.open("GET", "Timer.ashx?" + new Date().getTime(), true);
        //    xmlhttp.send();
        //}



        //var focClientId;
        //var seconds_left = 60 * 30;

        //var interval = setInterval(function () {
        //    --seconds_left;
        //    if (seconds_left > -10) {
        //        if (seconds_left % 10 == 3) {
        //            loadXMLDoc();
        //        }
        //        if (seconds_left < 0) {
        //            document.getElementById('timer_div').innerHTML = "Logout Timer: 00 min 00 s";
        //        } else {
        //            var min = trunc(seconds_left / 60);
        //            var s = seconds_left % 60;
        //            if (min < 10) {
        //                min = "0" + min;
        //            }

        //            if (s < 10) {
        //                s = "0" + s;
        //            }
        //            document.getElementById('timer_div').innerHTML = "Logout Timer: " + min + " min " + s + " s";
        //        }
        //    }
        //}, 1000);

        function trunc(n) {
            return n | 0; // bitwise operators convert operands to 32-bit integers
        }
    </script>
</head>
<body>
    
        <form style="padding-top: 0px; margin-top: 0px;" id="mpxForm" method="post" onsubmit="if (pageLoaded) { ShowProgress();return true; } else {alert('The page has not yet fully loaded. Please wait.'); return false; }" runat="server">
    <div class="page shadow">
        
        
            <div class="header">

                <div style="float:right;text-align:right; margin-top:-5px; font-weight:bold;margin-right:10px;">
                <div style="height: 35px; font-size: 13px; font-family: 'Roboto', Verdana;"><span style="color: #b9e0bb;">Build</span><span style="color:#f7b2b3;">-To-</span><span style="color: #b0c8e2;">Demand</span><span style="color: #d7d7d7;">, Inc.</span></div>
                <div>
                    
                    <asp:Label ID="lblUser" Text="Welcome admin!" runat="server" CssClass="userLabel"></asp:Label>
                </div>
                    </div>
                

                <div style="margin-left: 1px; float: left; width:40%;">
                <%--    <div class="countdown"  id="timer_div">Logout Timer: 30 min 00 s</div>--%>
           <%--         <div style="margin-left:9px;">
                        <asp:LinkButton ID="btnResetCountdown" OnClientClick="return true;" CssClass="reset" Text="Reset" OnClick="btnResetCountdown_Click" runat="server"></asp:LinkButton>
                        <asp:LinkButton ID="btnLogout" CssClass="reset" Text="Logout" OnClick="btnLogout_Click"  runat="server"></asp:LinkButton>
                    </div>--%>
                
                </div>
                
               
                <img src="Images/gear.png" height="40" class="mpxTitleImage" />
                <h1 class="mpxTitle" style="margin-bottom:0px;">Value Stream Modeling</h1>
                 
                 <div class="line" style="clear: both;"></div>
            </div>
            <div class="content" style="background-color:#F6F3F3;">
                <div class="tabsNonvisible">
                <h2>Administration</h2>
    <div class="contentPanel" style="padding-left:25px;padding-right:25px;">
        
            <h3>Current Users</h3>
        <asp:Panel ID="pnlSortBy" runat="server" ToolTip="Sort the user table by one or more columns by clicking [Sort] button repeatedly with selected option. The sorting order is built up until you hit [Reset Sorting]. The default has no sorting order displaying users as they were inserted.">
            <asp:Label ID="lblSortBy" Text="Sort By:" runat="server" ></asp:Label>
            <asp:DropDownList runat="server" ID="dropListSorting" >
                <asp:ListItem Text="Insert Order" Value="id" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Username" Value="username"></asp:ListItem>
                <asp:ListItem Text="User Directory Name" Value="usersub"></asp:ListItem>
                <asp:ListItem Text="Email" Value="mail"></asp:ListItem>
                <asp:ListItem Text="Company" Value="company"></asp:ListItem>
            </asp:DropDownList>
        <asp:Button runat="server" ID="btnSort" Text="Sort" CssClass="otherButton" OnClick="btnSort_Click" />
        <asp:Button runat="server" ID="btnResetSort" Text="Reset Sorting" CssClass="otherButton" OnClick="btnResetSort_Click" />
        <asp:RadioButtonList runat="server" ID="lstRdbtnOrder" RepeatDirection="Horizontal">
            <asp:ListItem Text="Ascending" Value="ASC" Selected="true"></asp:ListItem>
            <asp:ListItem Text="Descending" Value="DESC"></asp:ListItem>
        </asp:RadioButtonList>
        </asp:Panel>


            <asp:Panel ID="pnlUsers" CssClass="gridtable" runat="server" >
                <asp:GridView
                        ID="gridUsers"
                        runat="server"
                        AutoGenerateColumns="False"
                        AllowPaging="true"
                        PageSize="10"
                        DataKeyNames="id"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500"
                        EmptyDataText="There are no user records to display."
                        ShowFooter="false"
                        OnRowEditing="gridUsers_RowEditing"
                        OnRowCancelingEdit="gridUsers_RowCancelingEdit"
                        OnRowUpdating="gridUsers_RowUpdating"
                        OnRowDataBound="gridUsers_RowDataBound"
                        OnRowDeleting="gridUsers_RowDeleting"
                        OnRowCommand="gridUsers_RowCommand"
                        OnPageIndexChanging="gridUsers_PageIndexChanging"
                        PagerStyle-CssClass="tablenoborder">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnEdit" Text="Edit" CommandName="Edit" CssClass="otherButton" />
                                    <asp:Button runat="server" ID="btnDelete" Text="Delete" CommandName="Delete" CssClass="otherButton" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="Update" CssClass="updateButton" />
                                    <asp:Button runat="server" ID="btnCanc" Text="Cancel" CommandName="CancelUpdate" CssClass="otherButton" />
                                </EditItemTemplate>
                                
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Username" >
                                <ItemTemplate>
                                    <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("username") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtUsername" runat="server" Text='<%# Bind("username") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="User Directory Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblUsersub" runat="server" Text='<%# Bind("usersub") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtUsersub" runat="server" Text='<%# Bind("usersub") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Email">
                                <ItemTemplate>
                                    <asp:Label ID="lblMail" runat="server" Text='<%# Bind("mail") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMail" runat="server" Text='<%# Bind("mail") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Company">
                                <ItemTemplate>
                                    <asp:Label ID="lblCompany" runat="server" Text='<%# Bind("company") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtCompany" runat="server" Text='<%# Bind("company") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                
            </asp:Panel>
        
        
            <h3>Create New User</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lbl1" Text="Username: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtNewUser" runat="server" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" Text="Password: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" Text="Confirm Password: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" Text="User Directory Name: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserdir" runat="server" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" Text="User Email: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" Text="Company: " runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCompany" runat="server" CssClass="tableItem"></asp:TextBox>
                    </td>
                </tr>
            </table>
        <br />

        <asp:Button ID="btnAddUser" runat="server" Text="Create User" CssClass="otherButton" OnClick="btnAddUser_Click" />

        <br /> <br />
        <asp:Label ID="lblError" runat="server" ForeColor="#cc0000" Visible="false"></asp:Label>
        <br /> <br />
            <asp:Panel ID="pnlError" CssClass="errorPopPanel" Width="250" runat="server" style="display:none">
                    <h3>Error Message</h3>
                    
                    <asp:Label ID="lblGeneralError" runat="server">
                    
                    </asp:Label>
                    <br />
                    <br />

                    <asp:Button Style="width: 60px;" ID="btnOkError" runat="server" Text="Ok" OnClientClick="HidePopup('modalPopupError'); return true;" />
                    

                </asp:Panel>
                <asp:ScriptManager runat="server"></asp:ScriptManager>
                <ajaxToolkit:ModalPopupExtender ID="modalExtenderError" runat="server"
                    TargetControlID="modalDummy"
                    PopupControlID="pnlError"
                    BehaviorID="modalPopupError"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummy" runat="server"></div>

                <asp:Panel ID="pnlInfo" CssClass="msgPanel" Width="250" runat="server" style="display:none">
                    <h3>Info Message</h3>
                    <asp:Label ID="lblGeneralInfo" runat="server">
                    
                    </asp:Label>
                    <br />
                    <br />

                    <asp:Button Style="width: 60px;" ID="btnInfoOk" runat="server" OnClientClick="HidePopup('modalPopupInfo'); return false;" Text="Ok" />


                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="modalExtenderInfo" runat="server"
                    TargetControlID="modalDummy2"
                    PopupControlID="pnlInfo"
                    BehaviorID="modalPopupInfo"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummy2" runat="server"></div>

            <asp:Panel ID="pnlConfirmDelete" CssClass="msgPanel" Width="280" style="display:none;" runat="server">
        <h3>VS Modeling Delete User</h3>
        <asp:Label ID="lblConfirmDelete" Text="Are you sure you want to delete user from the system? All his files and directories as well as his credentials will be lost." runat="server">
                    
        </asp:Label>
        <br />
        <br />
        <asp:HiddenField runat="server" ID="hdnUserId" />
        <asp:Button ID="btnDeleteUser" runat="server" Text="Delete User" Width="130px" OnClick="btnDeleteUser_Click" OnClientClick="HidePopup('modalPopupConfirmDelete'); return true;" />
        <asp:Button ID="btnCancelDeleteUser" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupConfirmDelete'); return true;" OnClick="btnCancelDeleteUser_Click" />
        


    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalExtenderConfirmDelete" runat="server"
        TargetControlID="modalDummy3"
        PopupControlID="pnlConfirmDelete"
        BehaviorID="modalPopupConfirmDelete"
        BackgroundCssClass="modalBackground"
        DropShadow="true">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy3" runat="server"></div>

        <asp:Panel ID="pnlLoadingPopup" runat="server" CssClass="updateProgress" style="display:none">
                <div style="margin-top:13px;">
                    <div style="padding-bottom:10px;" ><img src="Images/gear2.png" width="20px" style="margin-right:10px; margin-left:20px;" /></div>
                    <div style="display:inline-block;margin-top:-28px;float:right; margin-right:30px;">Loading ... </div>
                </div>
            </asp:Panel>

                <ajaxToolKit:ModalPopupExtender 
                ID="mdlLoadingPopup" runat="server" TargetControlID="pnlLoadingPopup" BehaviorID="behLoadingPopup"
                PopupControlID="pnlLoadingPopup" DropShadow="true" BackgroundCssClass="modalBackground2"  />
            </div>
   
                </div>
                </div>
            
        </div>
            <div class="page footer">
        <asp:Label ID="lblLastLogin" Text="Last Login: 5/12/2013 10:44:27 PM; Last Logout: Normal logout." runat="server" CssClass="footerUserInfo"></asp:Label><br />
        <a href="http://www.build-to-demand.com"><img src="Images/logo.png" style="height:68px;margin-left:auto;margin-right:auto; margin-top:1px;margin-bottom:0px;" /></a>
        <div style="margin-top:1px;">
            E-mail: <a href="mailto:Info@Build-To-Demand.com">Info@Build-To-Demand.com</a>
            <br />
            Copyright &copy; 2016  Build-To-Demand, Inc.
        </div>
    </div>
    </form>
</body>
</html>
