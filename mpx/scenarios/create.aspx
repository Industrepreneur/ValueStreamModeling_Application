<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" Inherits="whatif_c" CodeFile="create.aspx.cs" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <h2>What-If Scenario Control Page</h2>
    <div class="tabsNonvisible">
        <div class="datacontent">
            <asp:MultiView ID="MPXWhatfcontrolChoices" runat="server">
                <asp:View ID="PageR1" runat="server">
                    <asp:Panel ID="panelViewWhatifs" runat="server" Visible="false">
                        <h3>View Whatifs</h3>
                        <asp:Label ID="Label16" runat="server" Text="List of Existing Whatifs:   "></asp:Label>
                        <asp:DropDownList ID="DropDownList3" runat="server"
                            DataSourceID="AccessDataSource1" DataTextField="Name" DataValueField="WID"
                            Height="24px" Width="119px">
                        </asp:DropDownList>

                        <br />
                    </asp:Panel>
                    <div style="float: left; margin-right: 120px;">
                        <h3>My What-If Scenarios</h3>
                        <asp:ListBox ID="lstWhatifs" runat="server" Width="300px" Height="200px" Font-Names="Verdana" DataSourceID="AccessDataSource1"
                            DataTextField="Name" DataValueField="WID"></asp:ListBox>
                        <br />
                        <asp:Button ID="btnLoadWhatif" runat="server" Text="Load What-If Scenario" OnClick="btnLoadWhatif_Click" TabIndex="7"></asp:Button>
                        <asp:Button ID="btnDeleteWhatif" runat="server" Text="Delete What-If Scenario" OnClick="btnDeleteWhatif_Click" TabIndex="8"
                            OnClientClick="return confirm('Are you sure you want to delete the selected What-If Scenario?');"></asp:Button>
                        <br />
                    </div>
                    <div>
                        <h3>Start a new What-If Scenario</h3>
                        Enter a name for the new What-If Scenario:
                    <asp:TextBox ID="txtNewWhatif" runat="server"></asp:TextBox>
                        <br />
                        <asp:Button ID="btnNewWhatif" runat="server" Text="Start a New What-If Scenario" OnClick="btnNewWhatif_Click" />

                        <br />
                    </div>
                    <asp:Panel ID="panelManage" Visible="false" runat="server">
                        <h3>Manage Whatifs</h3>
                        <asp:Label ID="Label17" runat="server" Height="30px"
                            Text="Name for NEW Whatif:   "></asp:Label>
                        <asp:TextBox ID="TextBox17" runat="server" MaxLength="666"></asp:TextBox>
                        <asp:Button ID="Button_new0" runat="server" Height="28px"
                            OnClick="Buttonr1_new_Click" Text="Start a new What-if         "
                            Width="185px" />
                        <br />
                        <asp:Button ID="Button_load" runat="server" OnClick="Buttonr1_load_Click"
                            Text="Load Previous Whatif" Width="185px" Height="28px" />
                        <asp:Label ID="Label18" runat="server" Height="30px"
                            Text="Name of Whatif To Load:"></asp:Label>

                        <asp:DropDownList ID="DropDownList1" runat="server"
                            DataSourceID="AccessDataSource1" DataTextField="Name" DataValueField="WID"
                            Height="24px" Width="119px">
                        </asp:DropDownList>
                        <br />
                        <br />
                        <asp:Button ID="Button_del" runat="server" Height="28px"
                            OnClick="Buttonr1_del_Click" Text="Delete Previous Whatif" Width="185px" />
                        <asp:Label ID="Label19" runat="server" Height="30px"
                            Text="Name of Whatif To Delete:   "></asp:Label>

                        <asp:DropDownList ID="DropDownList2" runat="server"
                            DataSourceID="AccessDataSource1" DataTextField="Name" DataValueField="WID"
                            Height="24px" Width="119px">
                        </asp:DropDownList>

                        <br />
                        <asp:AccessDataSource ID="AccessDataSource1" runat="server"
                            SelectCommand="SELECT [WID], [Name] FROM [tblWhatIf] WHERE FamilyID = 0;"></asp:AccessDataSource>
                        <br />
                        <br />
                        <asp:TextBox ID="textbox10" runat="server" Height="68px" TextMode="MultiLine"
                            Width="736px" Visible="False"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Label ID="Label3" runat="server" Height="30px"
                            Text="                   ERROR/Warning/Question is here (or invisible)"
                            Visible="False"></asp:Label>


                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                
                <br />
                        <br />
                        <br />
                        <br />
                        <asp:Button ID="b2" runat="server" BorderStyle="Inset" Height="28px"
                            Text="testing ,,, next page" OnClick="Buttonb2_Click" Width="185px" />
                        <br />
                        <br />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="PageR2" runat="server">
                    <h3><%= Master.GetFullCurrentWhatifLabel() %></h3>
                    <table class="simpleTable" style="margin-bottom: 5px;">
                        <tr>
                            <td>
                                <asp:Label ID="lblWhatifTitle" runat="server" Text="Whatif Name" CssClass="lblItem"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtWhatifName" runat="server" Height="19px" MaxLength="45" Rows="1" TabIndex="1" CssClass="editItem"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="alternativeColor">
                                <asp:Label ID="lblWhatifComment" runat="server" Text="Comment" CssClass="lblItem"></asp:Label>
                            </td>
                            <td class="alternativeColor">
                                <asp:TextBox ID="txtWhatifComment" TextMode="MultiLine" runat="server" Rows="5" TabIndex="2" CssClass="editItem"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <div class="cmdButtonRowMini">
                        <asp:LinkButton ID="Button_edit2" runat="server"
                            OnClick="Button_edit_Click" Text="Update" />
                        <asp:LinkButton ID="btnResetNameComment" runat="server" Text="Reset" OnClick="btnResetNameComment_Click" />
                    </div>
                    <asp:Panel ID="panelGreg" runat="server" Visible="false">
                        <asp:Label ID="lblSurveyPageR2" Text="Whatif is loaded:"
                            Font-Bold="True" Font-Size="Large"
                            runat="server"></asp:Label>
                        <asp:TextBox ID="widname" runat="server">-widname - </asp:TextBox>

                        <asp:Label ID="Label21" runat="server" Text="Edit Name of Current Whatif"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                
                <br />
                        <br />
                        <asp:Label ID="Label20" runat="server" Text="Comments about what-if"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
                
                <br />
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </asp:Panel>
                    <br />
                    <h3>What-If Modifications</h3>
                    <div class="cmdbuttonCol">
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="Button_edit3" runat="server"
                                        OnClick="Button_edit_rec_Click" Width="370px" 
                                        Text="View Current What-If Modifications (current Scenario)" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="Button_save2" runat="server" Width="368px"
                                        OnClick="Button_save_Click" 
                                        Text="Retain Current What-If Modifications with Scenario" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="Button_save3" runat="server" Width="368px"
                                        OnClick="Button_replace_Click" Text="Replace Base Case with this What-If Scenario"
                                        Font-Bold="True" /></td>
                            </tr>
                        </table>
                    </div>

                    <asp:Label ID="Label4" runat="server" BorderStyle="Solid" Font-Bold="True"
                        Font-Names="Arial Rounded MT Bold" Font-Underline="True" ForeColor="#CC0000"
                        Height="20px"
                        Text="                  ERROR/Warning/Question is here (or invisible)"
                        Visible="False"></asp:Label>
                    <br />

                    <hr size="2px" />
                    <h3>Close Current What-If Scenario</h3>
                    <div class="cmdbuttonCol">
                        <asp:LinkButton ID="Buttonr1_v2" runat="server"
                            OnClick="Button_saveClose_Click"
                            Text="Retain What-If Modifications with Scenario What-If &amp; Close What-If Scenario &amp; Return to Base Case" />
                        <br />
                        <br />
                        <asp:LinkButton ID="Button_NS_Close2" runat="server"
                            OnClick="Button_NS_Close2_Click"
                            Text="Do Not Retain What-If Modifications. Close What-If &amp; Return to Base Case" />
                        <br />
                    </div>
                    <h3>Retain as New What-If Scenario</h3>
                    <asp:Label ID="Label22" runat="server" Text="Name for New What-If Scenario: "></asp:Label><asp:TextBox ID="TextBox16" runat="server"></asp:TextBox>

                    <div class="cmdbuttonCol" style="margin-top: 10px;">
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="Buttonr1_v4" runat="server"
                                        OnClick="Button_SaveAs_Click" Text="Retain What-If Scenario as NEW What-If Scenario" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="Buttonr1_v7" runat="server"
                                        OnClick="Button_sas_close_Click"
                                        Text="Retain What-If Scenario as NEW What-If Scenario &amp; Close What-If Scenario &amp; Return to Base Case" /></td>
                            </tr>



                        </table>
                    </div>
                    <br />
                    <asp:Panel ID="panelGreg2" Visible="false" runat="server">
                        <asp:Label ID="Label25" runat="server" Text="List of Current What ifs "></asp:Label>
                        &nbsp;&nbsp;<br />
                        <asp:DropDownList ID="DropDownList4" runat="server"
                            DataSourceID="AccessDataSource1" DataTextField="Name" DataValueField="WID"
                            Height="24px" Width="119px">
                        </asp:DropDownList>
                        <br />
                        <br />
                        <br />
                        <asp:Button ID="a21" runat="server" BorderStyle="Inset" Height="28px"
                            Text="testing ,,, prev page" OnClick="Buttonb1_Click" Width="185px" />
                        <asp:Button ID="a23" runat="server" BorderStyle="Inset" Height="28px"
                            Text="testing ,,, next page" OnClick="Buttonb3_Click" Width="185px" />
                        <br />
                    </asp:Panel>
                </asp:View>

                <asp:View ID="View1" runat="server">
                    <br />
                    <br />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label7" runat="server" Font-Bold="True"
                    Font-Size="Large" Text="View What-If Modifications"></asp:Label>
                    <br />
                    <br />
                    &nbsp;<asp:Label ID="Label8" runat="server"
                        Text=" Table goes here:"></asp:Label>

                    <br />
                    <br />


                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="Button1" runat="server" BorderStyle="Inset"
                        Height="25px" OnClick="Button_endedit_Click" Text="End View of Whatif records"
                        Width="257px" />
                    <br />
                    <br />

                    <br />
                    <br />
                </asp:View>

                <asp:View ID="WhatifRecords" runat="server">
                    <h3>What-If Modifications</h3>
                    <asp:GridView
                        ID="gridWhatifRecords"
                        runat="server"
                        AutoGenerateColumns="False"
                        DataKeyNames="AuditID"
                        AllowPaging="true"
                        OnPageIndexChanging="gridWhatifRecords_PageIndexChanging"
                        OnRowDataBound="gridWhatifRecords_RowDataBound"
                        RowStyle-CssClass="datatable-rowstyle"
                        AlternatingRowStyle-BackColor="White"
                        HeaderStyle-BackColor="#ffa500"
                        FooterStyle-BackColor="#ffa500"
                        DataSourceID="srcWhatifRecords">

                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>Table</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblTableName" Text='<%# Bind("TableE") %>' runat="server"></asp:Label>
                                </ItemTemplate>

                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Field</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblFieldName" Text='<%# Bind("FieldE") %>' runat="server" />
                                </ItemTemplate>

                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Base Case</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblBasecase" Text='<%# Bind("OldShow") %>' runat="server" />
                                </ItemTemplate>

                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>What-If</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblWhatif" Text='<%# Bind("NewShow") %>' runat="server" />
                                </ItemTemplate>

                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:AccessDataSource ID="srcWhatifRecords"
                        runat="server"></asp:AccessDataSource>

                    <div class="cmdbutton">

                        <asp:LinkButton ID="btnSaveWhatifRecords" runat="server"
                            OnClick="Button_save_Click" Text="Retain What-If Modifications with Scenario" />
                        <asp:LinkButton ID="Button_editend4" runat="server"
                            OnClick="Button_endedit_Click" Text="Go Back" />
                    </div>
                </asp:View>

            </asp:MultiView>

        </div>
    </div>
</asp:Content>
