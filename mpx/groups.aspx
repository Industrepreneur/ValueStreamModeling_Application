<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true"  Inherits="Groups" CodeFile="groups.aspx.cs"  %>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2> Groups </h2>
    <div class="groups" style="float: none">
        Select Group:    
        <asp:DropDownList ID="dropSelectGroup" runat="server"  AutoPostBack="true" ToolTip="Select Group to View" Width="200px"  DataTextField="group_name" DataValueField="group_id" OnSelectedIndexChanged="dropSelectGroup_SelectedIndexChange"></asp:DropDownList>
         
        
        <br /><br />
          <asp:Label ID="chosengroupname" runat="server" visible = "false" Text =" "> </asp:Label>
          <asp:Label ID="curgrpname" runat="server" Text =""> </asp:Label>
           <br /> <br />
        <asp:Button ID="btnCreateNewGroup" runat="server" ToolTip="Create a New Group" Text="Create New Group" CssClass="stdButton" OnClick="CreateGroup_Click"></asp:Button>
        <asp:Button ID="btnDeleteGroup" runat="server" ToolTip="Delete the Current Group" Text="Delete Group" CssClass="stdButton" OnClick="ConfirmGroupDelete"></asp:Button>
     
       
      

      
    </div>

    <br /> 
            <asp:Panel ID="pnlError" CssClass="errorPopPanel" Width="250" runat="server" style="display:none">
                    <h3>Error Message</h3>
                    
                    <asp:Label ID="lblGeneralError" runat="server">
                    
                    </asp:Label>
                    <br />
                    <br />

                    <asp:Button Style="width: 60px;" ID="btnOkError" runat="server" Text="Ok" OnClientClick="HidePopup('modalPopupError'); return true;" />
                    

                </asp:Panel>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
                <ajaxToolkit:ModalPopupExtender ID="modalExtenderError" runat="server"
                    TargetControlID="modalDummy"
                    PopupControlID="pnlError"
                    BehaviorID="modalPopupError"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
       
       <asp:Label ID="leadermsg" Text="Leader status ? " runat="server" ></asp:Label>
        <h3>Current Group Members:</h3>
        <asp:GridView
                        ID="groupmembers"
                        runat="server"
                        AutoGenerateColumns="False"
                        AllowPaging="true"
                        PageSize="10"
                        DataKeyNames="Username"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500" 
                        EmptyDataText="There are no members of this group."
                        ShowFooter="false"
                        OnRowEditing="groupmembers_RowEditing"
                        OnRowCancelingEdit="groupmembers_RowCancelingEdit"
                        OnRowUpdating="groupmembers_RowUpdating"
                        OnRowDataBound="groupmembers_RowDataBound"
                        OnRowDeleting="groupmembers_RowDeleting"
                        OnRowCommand="groupmembers_RowCommand"
                        OnPageIndexChanging="groupmembers_PageIndexChanging"
                        PagerStyle-CssClass="tablenoborder">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnEdit"  Text="Change Leader Status" CommandName="Flip" CssClass="otherButton" />
                                    <asp:Button runat="server" ID="btnDelete" Text="Delete" CommandName="Delete" CssClass="otherButton" />
                                </ItemTemplate>                                                           
                            </asp:TemplateField>

                         
                                
                      
                            <asp:TemplateField HeaderText="  Username  " >
                                <ItemTemplate>
                                    <asp:Label ID="lblUsername"  Enabled='false' runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtUsername" Enabled="true" runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                           
                         
                     
                            <asp:TemplateField HeaderText=" Leader Status ">
                                <ItemTemplate>
                                    <asp:Label ID="lblUserlead" style="text-align: center;"  runat="server" Text='<%# Bind("leader") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtUserlead" style="text-align: center;"  runat="server" Text='<%# Bind("leader") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
        <br />
    
       
        
     
        <br />
        <asp:Panel ID="pnldeleteUser" CssClass="msgPanel" style="display:none;" Width="280" runat="server">

         <asp:Label ID="lblConfirmDelete" Text="Are you sure you want to delete user from the group? All his/her files in the group models will NOT be lost." runat="server">
         </asp:Label> <br /><br />
       
        <asp:HiddenField runat="server" ID="hdnUserId" />
        <asp:Button ID="btnDeleteUser" runat="server" Text="Remove User from Group" Width="170px" OnClick="btnDeleteUser_Click" OnClientClick="HidePopup('modalPopupConfirmDelete'); return true;" />
        <asp:Button ID="btnCancelDeleteUser" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupConfirmDelete'); return true;" OnClick="btnCancelDeleteUser_Click" />
        </asp:Panel>
         <ajaxToolkit:ModalPopupExtender ID="modalExtenderConfirmDelete" runat="server"
            TargetControlID="modalDummy3"
            PopupControlID="pnldeleteUser"
            BehaviorID="modalPopupConfirmDelete"
            BackgroundCssClass="modalBackground"
            DropShadow="true">
        </ajaxToolkit:ModalPopupExtender>
        <div id="modalDummy3" runat="server"></div>

         <asp:Panel ID="pnlConfirmDelete" CssClass="msgPanel" style="display:none;" Width="280" runat="server">

         <asp:Label ID="lblConfirmGroup" Text="Are you sure you want to delete the group? ALL FILES IN THE GROUP WILL BE LOST." runat="server">
         </asp:Label><br /><br />

         <asp:Button ID="btnDeleteGroup2" runat="server" Text="Remove Group" Width="170px" OnClick="btnDeleteGroup_Click" OnClientClick="HidePopup('modalPopupConfirmDelete2'); return true;" />
         <asp:Button ID="btnCancelDeleteGroup" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupConfirmDelete2'); return true;" OnClick="btnCancelDeleteGroup_Click" />
        
             </asp:Panel>

         <ajaxToolkit:ModalPopupExtender ID="modalExtenderConfirmDelete2" runat="server"
            TargetControlID="modalDummy2"
            PopupControlID="pnlConfirmDelete"
            BehaviorID="modalPopupConfirmDelete2"
            BackgroundCssClass="modalBackground"
            DropShadow="true">
        </ajaxToolkit:ModalPopupExtender>
        <div id="modalDummy2" runat="server"></div>

        
         <asp:Panel ID="pnlAddGroup" CssClass="msgPanel" style="display:none;" Width="280" runat="server">

        <asp:Label ID="labeladd" Text = " Name of Group To Add:  " runat="server"   /> <br /> <br />
        <asp:TextBox ID="AddGroupName" runat="server"  Width="270px"  /><br /><br />
        <asp:Button ID="Button1" runat="server" Text="Create Group" Width="170px" OnClick="btnCreateGroup_Click" OnClientClick="HidePopup('ModalPopupExtender3'); return true;" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('ModalPopupExtender3'); return true;" OnClick="btnCancelCreateGroup_Click" />
        
           </asp:Panel>

         <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender3" runat="server"
            TargetControlID="modalDummy4"
            PopupControlID="pnlAddGroup"
            BehaviorID="ModalPopupExtender3"
            BackgroundCssClass="modalBackground"
            DropShadow="true">
        </ajaxToolkit:ModalPopupExtender>
        <div id="modalDummy4" runat="server"></div>

        <div>
        <h3>Add New User to Group</h3>
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
                        <asp:Label ID="lblLeader" Text="Is Leader:" runat="server" CssClass="tableItem"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkIsLeader" Text=" " runat="server" CssClass="tableItem"></asp:CheckBox>
                    </td>
                </tr>
            </table>
        <br />
        <asp:Button ID="btnAddUser" Text="Add User" runat="server" OnClick="btnAddUser_Click"></asp:Button>
    </div>
    

</asp:Content>

