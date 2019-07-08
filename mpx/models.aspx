<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" EnableEventValidation="false"
    CodeFile="models.aspx.cs" Inherits="models" %>

<%@ MasterType VirtualPath="~/MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%= pageInsert %>
    <div style="background-color:#eaedf1; height:100%; width:100%;">
    <div class="models" style="float: right;">
        <h3>Upload Models</h3>
        <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="uploadControl">
                    <asp:FileUpload ID="fuUpload" runat="server" TabIndex="10" AccessKey="C" />
                </div>
                <asp:Button ID="btnUpload" runat="server" Text="Upload" TabIndex="11" OnClick="btnUpload_Click"></asp:Button>

            </ContentTemplate>
           <%-- <Triggers>
                <asp:PostBackTrigger ControlID="btnUpload" />
            </Triggers>--%>
        </asp:UpdatePanel>
    </div>
    <div class="models">
        <h3 style="text-transform:uppercase;">
            Current Model: <% =Master.GetFullCurrentModelLabel() %>
        </h3>
        <asp:Button ID="btnSave" CssClass="stdButton" runat="server" Text="Save Model" OnClick="SaveModel" ToolTip="Saves all changes in the current model." TabIndex="1" />
        <asp:Button ID="btnSaveAs" CssClass="stdButton" runat="server" Text="Save Model As" OnClick="SaveModelAs" TabIndex="2" ToolTip="Saves the current model under a new name. The original model remains unchanged." />
        <asp:Button ID="btnRenameModel" CssClass="stdButton" runat="server" Text="Rename Model" OnClick="RenameModel" TabIndex="3" ToolTip="Renaming a model changes the model name of an existing model." />

        <br />
        <asp:Button ID="btnNewModel" CssClass="stdButton" runat="server" Text="New Model" TabIndex="5" OnClick="btnNewModel_Click" />
        <br />
        <br />

        <asp:Button ID="btnResetFiles" CssClass="stdButton" runat="server" Text="Reset Default Files" ToolTip="Resets all default files and the demonstration model Gthubs.mdb." TabIndex="5" OnClick="btnResetFiles_Click" />
        <br />
        <br />
        <asp:Label ID="lblJump" runat="server" Text="After Login go to:"></asp:Label>
        <br />
        <asp:RadioButtonList runat="server" ID="lstRdbtnJump" AutoPostBack="true" CssClass="radioSepar" OnSelectedIndexChanged="lstRdbtnJump_SelectedIndexChanged">
            <asp:ListItem Text="Models page" Value="0"></asp:ListItem>
            <asp:ListItem Text="Last page visited at valuestreammodel.com" Value="1"></asp:ListItem>
        </asp:RadioButtonList>
    </div>


    <div class="models">
        <h3>My Models</h3>
        <asp:ListBox ID="lstModels" runat="server" Width="300px" Height="200px" TabIndex="6" Font-Names="Verdana"></asp:ListBox>
        <br />
        <asp:Button ID="btnOpen" runat="server" Text="Open Model" OnClick="OpenModel" ToolTip="Load Existing Model" TabIndex="7"></asp:Button>
        <asp:Button ID="btnDelete" runat="server" Text="Delete Model" OnClick="DeleteModel" TabIndex="8"
            OnClientClick="return confirm('Are you sure you want to delete the selected model?');"></asp:Button>
        <br />
        <asp:Button ID="btnDownload" runat="server" Text="Download Model" TabIndex="9" OnClick="DownloadModel" OnClientClick="setTimeout(function () { var modalPopupLoading = $find('behLoadingPopup'); modalPopupLoading.hide(); }, 1000);" />


    </div>



    <div class="models" style="display: none;">
        <h3>My Outputs</h3>
        <asp:ListBox ID="lstOutputs" runat="server" Width="300px" Height="200px" Font-Names="Verdana" ToolTip="Double-click to open model"></asp:ListBox>
        <br />
        <asp:Button ID="btnDownloadOutputs" runat="server" Text="Download" OnClick="DownloadOutput" />
        <asp:Button ID="btnDeleteOutput" runat="server" Text="Delete" OnClick="DeleteOutput"
            OnClientClick="return confirm('Are you sure you want to delete the selected output file?');" />
        <br />
        <asp:Label ID="lblOutputStatus" CssClass="statusLabel" runat="server" />
    </div>
    <div>

        <%--THIS IS WHERE NEW MODEL POPUP IS INJECTED?--%>
        <asp:Panel ID="pnlNewNameWrap" runat="server">
            <asp:LinkButton runat="server" ID="btnDummy3"></asp:LinkButton>
        </asp:Panel>
    </div>
        </div>
    <%--SAVE ANY CHANGES MESSAGE PANEL--%>
    <asp:Panel ID="pnlWantSaveModel" CssClass="popPanel" Style="display: none;" Width="250" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">Save?</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="lblWantSaveModel" Text="Do you wish to save the current model?" runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button Style="width: 60px;" ID="btnWantSaveModel" runat="server" Text="Save" OnClick="btnWantSaveModel_Click" OnClientClick="HideWantSaveModel();return true;" />
            <asp:Button ID="btnDontSaveModel" runat="server" Text="Don't Save" OnClick="btnDontSaveModel_Click" OnClientClick="HideWantSaveModel();return true;" />
            <asp:Button ID="btnCancelOpen" runat="server" Text="Cancel" OnClientClick="HideWantSaveModel();return false;" />
            <asp:HiddenField ID="hdnAction" runat="server" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalPopupWantSaveModel" runat="server"
        TargetControlID="modalDummy5"
        PopupControlID="pnlWantSaveModel"
        BehaviorID="modalPopupWantSaveBehavior"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy5" runat="server"></div>

    <%--OPEN CURRENT Open MESSAGE--%>
    <asp:Panel ID="pnlUnsaveModel" CssClass="popPanel" Style="display: none;" Width="280" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">Reload?</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="lblInquiry" Text="The model you intended to open is the current model. Do you wish to reload the current model and lose all unsaved changes?" runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnLoadAgain" runat="server" Text="Reload Current Model" OnClick="btnLoadAgain_Click" OnClientClick="HidePopup('modalPopupUnsaveModel'); return true;" />
            <asp:Button ID="btnCancelUnsave" runat="server" Text="Cancel" OnClientClick="HidePopup('modalPopupUnsaveModel');return false;" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalUnsave" runat="server"
        TargetControlID="modalDummy6"
        PopupControlID="pnlUnsaveModel"
        BehaviorID="modalPopupUnsaveModel"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy6" runat="server"></div>

    <%--DELETE CURRENT MESSAGE--%>
    <asp:Panel ID="pnlDeleteCurrent" CssClass="popPanel" Width="280" Style="display: none;" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">Delete?</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="Label1" Text="The model you intended to delete is currently open. Do you wish to clear its data or close and delete the entire model file?" runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnClearModel" runat="server" Text="Clear Model" OnClick="btnClearModel_Click" OnClientClick="HidePopup('modalPopupDeleteCurrent'); return true;" />
            <asp:Button ID="btnDeleteReally" runat="server" Text="Delete Model" OnClick="btnDeleteReally_Click" OnClientClick="HidePopup('modalPopupDeleteCurrent'); return true;" />
            <asp:Button ID="btnCancelDeleteCurrent" runat="server" Text="Cancel" OnClientClick="HidePopup('modalPopupDeleteCurrent'); return false;" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalDeleteCurrent" runat="server"
        TargetControlID="modalDummy7"
        PopupControlID="pnlDeleteCurrent"
        BehaviorID="modalPopupDeleteCurrent"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy7" runat="server"></div>

    <%--CREATE WITH SAME NAME MESSAGE--%>
    <asp:Panel ID="pnlRewriteModel" CssClass="popPanel" Width="280" Style="display: none;" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">Overwrite?</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="Label2" Text="A model with the same name already exists. Do you wish to overwrite the model by the upload file?" runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnRewriteModel" runat="server" Text="Rewrite Model" OnClick="btnRewriteUpload_Click" OnClientClick="HidePopup('modalPopupRewriteModel'); return true;" />
            <asp:Button ID="btnCancelRewrite" runat="server" Text="Cancel" OnClick="btnCancelRewriteUpload_Click" OnClientClick="HidePopup('modalPopupRewriteModel'); return true;" />
        </div>


    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalRewriteUpload" runat="server"
        TargetControlID="modalDummy8"
        PopupControlID="pnlRewriteModel"
        BehaviorID="modalPopupRewriteModel"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy8" runat="server"></div>

    <%--WHAT IF IS OPEN MESSAGE--%>
    <asp:Panel ID="pnlGoToWhatif" CssClass="popPanel" Width="280" Style="display: none;" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">What-if in Progress</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="Label3" Text="The current model is running in What-If Scenario. If you wish to save/save as/rename the current model first you need to go to What-If Scenario page and close the current What-If Scenario." runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnGoToWhatif" runat="server" Text="Go to What-If Page" Width="130px" OnClick="btnGoToWhatif_Click" OnClientClick="HidePopup('modalPopupWhatif'); return true;" />
            <asp:Button ID="btnCancelGoToWhatif" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupWhatif'); return false;" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="modalGoToWhatif" runat="server"
        TargetControlID="modalDummy9"
        PopupControlID="pnlGoToWhatif"
        BehaviorID="modalPopupWhatif"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy9" runat="server"></div>

    <%--RESET CORE FILES--%>
    <asp:Panel ID="pnlResetFiles" CssClass="popPanel" Style="display: none; width:unset;" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">Reset Default Files</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="lblResetFiles" Text="Reseting model files will remove any changes in the demonstration model file Gthubs.mdb.<br /><br />This will reset the core database file and any sorting preferances.<br /><br />Do you really wish to reset all default files?" runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnResetFilesReally" runat="server" Text="Reset Default Files" Width="130px" OnClick="btnResetFilesReally_Click" OnClientClick="HidePopup('modalPopupResetFiles'); return true;" />
            <asp:Button ID="btnCancelResetFilesReally" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupResetFiles'); return false;" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="extenderResetFiles" runat="server"
        TargetControlID="modalDummy10"
        PopupControlID="pnlResetFiles"
        BehaviorID="modalPopupResetFiles"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy10" runat="server"></div>

    <%--FILE OPEN RESET FILES--%>
    <asp:Panel ID="pnlResetFilesWhatif" CssClass="popPanel" Width="280" Style="display: none;" runat="server">
        <div class="popHeader">
            <span class="popHeaderText">What-if in Progress</span>
        </div>
        <div class="popMessage">
            <asp:Label ID="Label4" Text="The current model is running in What-If Scenario. If you wish to reset all Value Stream Modeling default files first you need to go to What-If Scenario page and close the current What-If Scenario." runat="server"></asp:Label>
        </div>
        <div class="popSubmit">
            <asp:Button ID="btnGoToWhatifReset" runat="server" Text="Go to What-If Page" Width="130px" OnClick="btnGoToWhatif_Click" OnClientClick="HidePopup('modalPopupResetFilesWhatif'); return true;" />
            <asp:Button ID="btnCancelWhatifReset" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupResetFilesWhatif'); return false;" />
        </div>

    </asp:Panel>

    <ajaxToolkit:ModalPopupExtender ID="extenderResetFilesWhatif" runat="server"
        TargetControlID="modalDummy11"
        PopupControlID="pnlResetFilesWhatif"
        BehaviorID="modalPopupResetFilesWhatif"
        BackgroundCssClass="modalBackground"
        DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="modalDummy11" runat="server"></div>
</asp:Content>
