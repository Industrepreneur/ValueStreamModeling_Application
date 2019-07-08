<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="group_models.aspx.cs" Inherits="group_models" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .arrowButtonGroup
        {}
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div style="background-color:#F6F3F3;" class="tabsNonvisible">

        <div style="padding-left:15px;padding-right:15px;padding-bottom:15px;">
            <h2 style="margin-bottom: 0px;">Group Models</h2>

            <div style="float: left; overflow: hidden;">
                <h3>My Models</h3>
                <div style="width:300px;">List of current models:</div>
                <br />
                <asp:ListBox ID="lstPersonModels" runat="server" Width="300px" Height="200px" Font-Names="Verdana">
                    <asp:ListItem>Gthubs.mdb</asp:ListItem>
                </asp:ListBox>
                <br /><br />
                 <asp:Label ID="lblError" runat="server" ForeColor="#cc0000" Visible="false"></asp:Label>
                    <br /> <br />

                 
                    
                <ajaxToolkit:ModalPopupExtender ID="modalExtenderConfirmDelete" runat="server"
                    TargetControlID="modalDummy3"
                    PopupControlID="pnlConfirmDelete"
                    BehaviorID="modalPopupConfirmDelete"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummy3" runat="server"></div>

              
            <asp:Panel ID="pnlError" CssClass="errorPopPanel" Width="250" runat="server" style = "display:none" >
                    <h3>Error Message</h3>
                    
                      <br />  <br />
                    <asp:Label ID="lblGeneralError" runat="server">
                    
                    </asp:Label>
                    
                    <br />
                    <br />
                    <asp:HiddenField runat="server" ID="hdnUserId" />

                    <asp:Button ID="btnDelete" runat="server" Text="Remove Model" OnClick="btnDeleteModel"   ></asp:Button>
                    
                  
                    <br />
                    <br />
                       
                    <asp:Button Style="width: 60px;" visible = "false"  ID="btnOkError" runat="server" Text="Ok" OnClientClick="HidePopup('modalPopupError'); return true;" />
                    

                </asp:Panel>
         
                <ajaxToolkit:ModalPopupExtender ID="modalExtenderError" runat="server"
                    TargetControlID="modalDummy"
                    PopupControlID="pnlError"
                    BehaviorID="modalPopupError"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
                <div id="modalDummy" runat="server"></div>
               
       

            </div>
            <div style="float: left; overflow: hidden; margin-top: 160px; margin-right: 5px; height: 122px; width: 175px;">
             <asp:ImageButton ID="btnRight" runat="server" ImageUrl="Images/arrow.png" 
                    CssClass="arrowButtonGroup" Enabled="true" 
                    ToolTip="Click here to copy model from user models to group models (COPY NOT MOVE)" 
                    ImageAlign="TextTop" OnClick="btnRight_Click" Height="120px" 
                    Width="140px"/>
              
                <br />
             
                
                <br />

            </div>
            <div style="float: left;">
                <h3>Group Models</h3>
                <div>
                    Select Group:
       
	            <asp:DropDownList ID="DropDownList2" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="dropList2_SelectedIndexChange" DataTextField="Group_name" DataValueField="Group_id">
                </asp:DropDownList>
                 <br />
                    <asp:Label ID="errormessage" ForeColor="Red"  Text="  " runat="server" ></asp:Label> 

                    <br />
                    <br />
                    <asp:Panel ID="pnlSortBy" runat="server" ToolTip="Sort the user table by one or more columns by clicking [Sort] button repeatedly with selected option. The sorting order is built up until you hit [Reset Sorting]. The default has no sorting order displaying users as they were inserted.">
            <asp:Label ID="lblSortBy" Text="Sort By:" runat="server" ></asp:Label>
            <asp:DropDownList runat="server" ID="dropListSorting" >
                <asp:ListItem Text="Filename" Value="File_name" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Owner" Value="Username"></asp:ListItem>
                <asp:ListItem Text="Date ADDED" Value="Date_created"></asp:ListItem>
                <asp:ListItem Text="Comments" Value="Comments"></asp:ListItem>
            </asp:DropDownList>
        <asp:Button runat="server" ID="btnSort" Text="Sort" CssClass="otherButton" OnClick="btnSort_Click" />
        <asp:Button runat="server" ID="btnResetSort" Text="Reset Sorting" CssClass="otherButton" OnClick="btnResetSort_Click" />
        <asp:RadioButtonList runat="server" ID="lstRdbtnOrder" RepeatDirection="Horizontal">
            <asp:ListItem Text="Ascending" Value="ASC" Selected="true"></asp:ListItem>
            <asp:ListItem Text="Descending" Value="DESC"></asp:ListItem>
        </asp:RadioButtonList>

            <br /> <asp:Label ID="Label2" Text="All files from All groups in which you are a member." runat="server" ></asp:Label> 
                    <br /> <br />
                    <asp:Label ID="leadermsg" Text="Leader status ? " runat="server" ></asp:Label>  <br /> 
                    <asp:Label ID="leadermsg2" Text="Leader status ? " runat="server" ></asp:Label><br /><br />
            <asp:Panel ID="Panel1" CssClass="errorPopPanel" Width="250" runat="server" style="display:none">
                    <h3>Error Message</h3>
                    
                    <asp:Label ID="Label1" runat="server">
                    
                    </asp:Label>
                    <br />
                    <br />

                    <asp:Button Style="width: 60px;" ID="Button1" runat="server" Text="Ok" OnClientClick="HidePopup('modalPopupError'); return true;" />
                    

                </asp:Panel>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
                    TargetControlID="modalDummy"
                    PopupControlID="pnlError"
                    BehaviorID="modalPopupError"
                    BackgroundCssClass="modalBackground"
                    DropShadow="true">
                </ajaxToolkit:ModalPopupExtender>
        </asp:Panel>
                        
                    <asp:GridView   
	                    ID="gridGroupModels"
                        runat="server"
                        AutoGenerateColumns="False"
                        AllowPaging="True"  
                        PageSize ="10"  
				        DataKeyNames="File_id"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500"
                        EmptyDataText="There are no group models to display."
                        ShowFooter="false"
                        OnRowEditing="gridGroupModels_RowEditing"
                        OnRowCancelingEdit="gridGroupModels_RowCancelingEdit"
                        OnRowUpdating="gridGroupModels_RowUpdating"
                        OnRowDataBound="gridGroupModels_RowDataBound"
                        OnRowDeleting="gridGroupModels_RowDeleting"
                        OnRowCommand="gridGroupModels_RowCommand"
                        OnPageIndexChanging="gridGroupModels_PageIndexChanging"
                        PagerStyle-CssClass="tablenoborder" 
                        style="margin-left: 20px; margin-right: 20px">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnCopyLeft" Text="Copy To User" CommandName="Copy" CssClass="otherButton" />
                                    <asp:Button runat="server" ID="btnEdit" Text="Edit" CommandName="Edit" CssClass="otherButton" />
                                    <asp:Button runat="server" ID="btnDelete" Text="Delete" CommandName="Delete" CssClass="otherButton" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Button runat="server" ID="btnUpdate" Text="Update" CommandName="Update" CssClass="updateButton" />
                                    <asp:Button runat="server" ID="btnCanc" Text="Cancel" CommandName="CancelUpdate" CssClass="otherButton" />
                                </EditItemTemplate>
                                
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Filename" >
                                <ItemTemplate>							
                                    <asp:Label ID="lblFilename" runat="server" Text='<%# Bind("File_name") %>' Enabled="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtFilename" runat="server" Text='<%# Bind("File_name") %>' Enabled="false"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>

                              <asp:TemplateField HeaderText="Group" Visible ="true"  >
                             <ItemTemplate>							
                                    <asp:Label ID="lblGroupname" runat="server" Text='<%# Bind("Groupname") %>' Width ="100px" Enabled="false" ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtGroupname" runat="server" Text='<%# Bind("Groupname") %>'  Enabled="false"  ></asp:Label>
                                </EditItemTemplate>
                                 </asp:TemplateField>
                             
                               
                           
                            <asp:TemplateField HeaderText="File Owner">
                                <ItemTemplate>
                                    <asp:Label ID="lblFileOwner" runat="server" Text='<%# Bind("File_owner") %>' Enabled="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtFileOwner" runat="server" Text='<%# Bind("File_owner") %>' Enabled="false"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="F" Visible ="false"  >
                             <ItemTemplate>							
                                    <asp:Label ID="lblFileid" runat="server" Text='<%# Bind("file_id") %>' Width ="100px" Enabled="false" ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtFileid" runat="server" Text='<%# Bind("file_id") %>'  Enabled="false"  ></asp:Label>
                                </EditItemTemplate>
                                 </asp:TemplateField>

                            <asp:TemplateField HeaderText="Date Added (GMTime)">
                                <ItemTemplate>
                                    <asp:Label ID="lblDate" runat="server" Text='<%# Bind("Date_created") %>' Enabled="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtDate" runat="server" Text='<%# Bind("Date_created") %>' Enabled="false"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="File Size">
                                <ItemTemplate>
                                    <asp:Label ID="lblFileSize" runat="server" Text='<%# Bind("File_size") %>' Enabled="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="txtFileSize"  runat="server" Text='<%# Bind("File_size") %>' Enabled="false"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Comments"  visible="true" >
                                <ItemTemplate>
                                    <asp:Label ID="lblComments" runat="server"  Text='<%# Bind("Comments")  %>' ></asp:Label >
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtComments" runat="server"  Text='<%# Bind("Comments") %>' ></asp:Textbox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <PagerStyle CssClass="tablenoborder"></PagerStyle>

                        <RowStyle CssClass="datatable-rowstyle"></RowStyle>
                    </asp:GridView>
                
                    <br />

                </div>
           
            <div style="clear: left;">
                

                
                    <br />

                </div>
            </div>
            <div style="clear: left;">
                
            </div>
        </div>
    </div>
</asp:Content>

