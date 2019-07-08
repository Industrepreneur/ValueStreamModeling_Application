using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_products_ibom : WhatifGridPage
{
    ProductDelegatePage helperProduct;
    
    private string[] TBL_IBOM_HEADERS = { "Parent", "Component", "Units per Assembly" };
    private string[] TBL_IBOM_FIELDS = { "ParentName", "compName", "UPA" };

    private HiddenField hdnCopyMode;

    protected static class CopyMode {
        public const int ASSEMBLY = 0;
        public const int ALL_IBOM = 1;
    }

    protected string IbomStringHeaders {
        get {
            if (ViewState["IbomStringHeaders"] == null) {
                ViewState["IbomStringHeaders"] = "";
            }
            return (string)ViewState["IbomStringHeaders"];
        }
        set { ViewState["IbomStringHeaders"] = value; }
    }

    protected string IbomString {
        get {
            if (ViewState["IbomString"] == null) {
                ViewState["IbomString"] = "";
            }
            return (string)ViewState["IbomString"];
        }
        set { ViewState["IbomString"] = value; }
    }

    public whatif_products_ibom() {
        PAGENAME = "whatif_products_ibom.aspx";
        featureHelper = new IbomDelegate();
        int value = 4;
        helperProduct = new ProductDelegatePage(value);
        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;

    }

    protected void Page_Load(object sender, EventArgs e) {
        grid.Columns[0].Visible = false;
        base.Page_Load(sender, e);
        if (!Page.IsPostBack) {
            SetIBOMStructureData();
        }

    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;
        FillIBOMStructure();

    }

    protected override void SetData() {
        {
            ClassE calc = new ClassE(GetDirectory() + userDir);

            calc.setGlobalVar();
            calc.runsql("DELETE * FROM tblbomTree;");
            if (dropListProducts.Items.Count > 0 && dropListProducts.SelectedItem == null) {
                dropListProducts.SelectedIndex = 0;
            }
            try {
                calc.MakeIbomTree(0, int.Parse(dropListProducts.SelectedValue), 0);
            } catch (Exception) { }
            calc.Close();
        }
        if (!checkAllSubComponents.Checked) {
            tableQueryString += " WHERE Show = -1";
        }
        base.SetData();
    }

    protected void SetIBOMStructureData() {
        if (dropListProducts2.Items.Count == 0) {
            try {
                dropListProducts2.DataBind();
            } catch (Exception) {
                Master.ShowErrorMessage();
            }
        }

        if (dropListProducts2.Items.Count > 0 && dropListProducts2.SelectedItem == null) {
            dropListProducts2.SelectedIndex = 0;
        }
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string commandRouting = "SELECT tblIbom.*  FROM tblIbom";
        if (dropListProducts.Items.Count > 0) {
            commandRouting += " WHERE (((tblIbom.ParentID)=" + dropListProducts2.SelectedValue + ")) ORDER BY CompID";
        }
        commandRouting += ";";
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                DataView dv = dt.DefaultView;
                gridBuildIbom.DataSource = dv;
                gridBuildIbom.DataBind();
                connec.Close();
            } catch (Exception ex) {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                string message = ex.Message;
            }
        }
    }

    


    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        pnlMenu.Controls.Add(new LiteralControl("<h2>What-If: Products</h2>"));
        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        srcProductsList.DataFile = dataFile;
        srcProductStructure.DataFile = dataFile;
        try {
            dropListProducts.DataBind();
        } catch (Exception) {
            Master.ShowErrorMessage();
        }
        Control buttondiv = getButtonDiv();
        Button btnCopyTable = buttondiv.FindControl(PageControls.BTN_COPY_TABLE) as Button;
        extenderCopy.TargetControlID = copyAllDummy.ID;
        btnCopyTable.Click += btnCopyTable_Click;

        hdnCopyMode = new HiddenField();
        pnlCopyTable.Controls.Add(hdnCopyMode);

        tableSync = new TableSynchronization(userDir);

    }

    protected void btnCopyTable_Click(object sender, EventArgs e) {
        if (rdbtnTableWithHeaders.Checked) {
            txtCopyTable.Text = TableStringHeaders + TableString;
        } else {
            txtCopyTable.Text = TableString;
        }
        boxCheckAll.Checked = false;
        hdnCopyMode.Value = "" + CopyMode.ASSEMBLY;
        extenderCopy.Show();

    }


    protected override Control getButtonDiv() {
        return buttondiv;
    }

    protected override Panel GetSecondPanel() {
        return secondPanel;
    }

    protected override Panel GetThirdPanel() {
        return thirdPanel;
    }

    protected override Panel GetFourthPanel() {
        return fourthPanel;
    }

    protected override Panel GetFifthPanel() {
        return fourthPanel;
    }

    protected void dropListProducts2_SelectedIndexChanged(object sender, EventArgs e) {
        string value = dropListProducts2.SelectedItem.Text;
        this.SetIBOMStructureData();
    }

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e) {
        string value = dropListProducts.SelectedValue;
        this.SetData();
    }

    protected void gridRouting_RowEditing(object sender, GridViewEditEventArgs e) {

    }
    protected void gridRouting_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {

    }
    protected void gridRouting_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }


    protected void gridRouting_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }

    protected void gridRouting_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT_ROUTING) as Button;
            string tooltip = "Double-click to edit component";
            if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                tooltip = "Double-click to update component";
                /*e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Update$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:normal;";
                e.Row.ToolTip = "Double-click to update row";*/
            }
            if (btnToClick != null) {
                string btnJavascript = ClientScript.GetPostBackClientHyperlink(
                btnToClick, "");
                for (int columnIndex = 1; columnIndex <
                e.Row.Cells.Count; columnIndex++) {
                    // Add the column index as the event argument parameter
                    string js = btnJavascript.Insert(btnJavascript.Length - 2,
                        columnIndex.ToString());
                    // Add this javascript to the onclick Attribute of the cell
                    e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                    // Add a cursor style to the cells
                    e.Row.Cells[columnIndex].Attributes["style"] +=
                        "cursor:pointer;cursor:hand;";
                    e.Row.Cells[columnIndex].ToolTip = tooltip;
                    if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                        try {
                            Control control = e.Row.Cells[columnIndex].FindControl(BUILD_IBOM_GRID_IDS[columnIndex]);
                            if (control is TextBox) {
                                (control as TextBox).Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                            } else if (control is AjaxControlToolkit.ComboBox) {
                                TextBox textBox = (control as AjaxControlToolkit.ComboBox).FindControl(control.ID + "_TextBox") as TextBox;
                                if (textBox != null) {
                                    textBox.Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                                }

                            }
                        } catch (Exception) { }
                    }
                }
            }

        }

    }

    protected void gridRouting_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btnSender = e.CommandSource as Button;
        GridViewRow row = btnSender.NamingContainer as GridViewRow;
        if (e.CommandName.Equals("Edit")) {
            gridBuildIbom.EditIndex = row.RowIndex;
        } else if (e.CommandName.Equals("CancelUpdate")) {
            gridBuildIbom.EditIndex = -1;
        } else if (e.CommandName.Equals("Update")) {
            var newValues = GetValues(row);
            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            OleDbCommand cmd = new OleDbCommand("UPDATE tblIbom SET UPA = ?  WHERE IbomID = ?;", connec);

            {
                try {
                    connec.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("UPA", newValues["UPA"]);
                    cmd.Parameters.AddWithValue("IbomID", gridBuildIbom.DataKeys[row.RowIndex]["IbomID"]);

                    int result = cmd.ExecuteNonQuery();
                    gridBuildIbom.EditIndex = -1;
                    connec.Close();

                } catch {
                    try {
                        connec.Close();
                        connec = null;
                    } catch { }
                    Master.ShowErrorMessage(DbUse.UPDATE_DATA_ERROR_MSG);
                 }
            }
        }
        //RefreshParentProducts();
        this.SetData();
        SetIBOMStructureData();
        if (e.CommandName.Equals("Edit")) {
            try {
                //int _rowIndex = int.Parse(e.CommandArgument.ToString());
                int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                Control editControl = gridBuildIbom.Rows[row.RowIndex].FindControl(BUILD_IBOM_GRID_IDS[_columnIndex]) as Control;
                // focus on the double-clicked control
                if (editControl != null && (editControl is TextBox || editControl is AjaxControlToolkit.ComboBox || editControl is CheckBox)) {
                    editControl.Focus();

                }

            } catch (Exception) {

            }

        }
    }

    private string[] BUILD_IBOM_GRID_IDS = { null, "txtCompName", "txtUPA" };

    private void FillIBOMStructure() {

        TemplateField template = new TemplateField();
        GridViewTemplate itemTemplate;

        itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "compName", "lblCompName");
        template.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["compName"] = ((Label)container.FindControl("lblCompName")).Text;
                dict["UPA"] = ((Label)container.FindControl("lblUPA")).Text;
                return dict;
            });

        List<string> opers = new List<string>();
        opers.Add("ProdID");
        opers.Add("ProdDesc");
        GridViewTemplate editItemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "compName", "lblCompName");
        template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editItemTemplate.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["compName"] = ((Label)container.FindControl("lblCompName")).Text;
                dict["UPA"] = ((TextBox)container.FindControl("txtUPA")).Text;
                return dict;
            });

        template.HeaderTemplate = new GridViewTemplate("Component", "Name of the component for the product selected in the above drop-down list.", "lblCompName");
        gridBuildIbom.Columns.Add(template);
        /************************************************************/

        TemplateField template2 = new TemplateField();
        GridViewTemplate itemTemplate2 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "UPA", "lblUPA");
        template2.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate2.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["UPA"] = ((Label)container.FindControl("lblUPA")).Text;
                dict["compName"] = ((Label)container.FindControl("lblCompName")).Text;
                return dict;
            });

        GridViewTemplate editTemplate2 = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, "UPA", "txtUPA");
        template2.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate2.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["UPA"] = ((TextBox)container.FindControl("txtUPA")).Text;
                dict["compName"] = ((Label)container.FindControl("lblCompName")).Text;
                return dict;
            });

        template2.HeaderTemplate = new GridViewTemplate("Units per Parent Product Assembly", "Number of units needed for the next level of assembly.", "lblUPA");
        template2.SortExpression = "UPA";
        gridBuildIbom.Columns.Add(template2);

        //gridRouting.Columns[0].Visible = false;
    }

        /************************************************************/

    
    
    protected void checkAllSubComponents_CheckedChanged(object sender, EventArgs e) {
        this.SetData();
    }
    protected void btnCopyAllIbom_Click(object sender, EventArgs e) {
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand("SELECT * FROM tblibom", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                connec.Close();
                for (int i = 0; i < TBL_IBOM_FIELDS.Length; i++) {
                    IbomStringHeaders += TBL_IBOM_HEADERS[i];
                    if (i == TBL_IBOM_FIELDS.Length - 1) {
                        IbomStringHeaders += "\r\n";
                    } else {
                        IbomStringHeaders += "\t";
                    }
                }
                for (int j = 0; j < dt.Rows.Count; j++) {
                    for (int i = 0; i < TBL_IBOM_FIELDS.Length; i++) {
                        IbomString += dt.Rows[j][TBL_IBOM_FIELDS[i]].ToString();
                        if (i == TBL_IBOM_FIELDS.Length - 1) {
                            IbomString += "\r\n";
                        } else {
                            IbomString += "\t";
                        }
                    }
                }
                if (rdbtnTableWithHeaders.Checked) {
                    txtCopyTable.Text = IbomStringHeaders + IbomString;
                } else {
                    txtCopyTable.Text = IbomString;
                }
                boxCheckAll.Checked = false;
                hdnCopyMode.Value = "" + CopyMode.ALL_IBOM;
                extenderCopy.Show();
            } catch (Exception) {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage();
            }
        }
    }

    protected override void rdbtnTable_CheckedChanged(object sender, EventArgs e) {
        int copyMode;
        try {
            copyMode = int.Parse(hdnCopyMode.Value);
        } catch (Exception) {
            copyMode = CopyMode.ASSEMBLY;
        }
        if (rdbtnTableWithHeaders.Checked) {
            if (copyMode == CopyMode.ASSEMBLY) {
                txtCopyTable.Text = TableStringHeaders + TableString;
            } else {
                txtCopyTable.Text = IbomStringHeaders + IbomString;
            }
        } else {
            if (copyMode == CopyMode.ASSEMBLY) {
                txtCopyTable.Text = TableString;
            } else {
                txtCopyTable.Text = IbomString;
            }
        }
        boxCheckAll.Checked = false;
        extenderCopy.Show();
    }

    protected override bool LocalTablesLinked() {
        if (!UpdateSql("SELECT ProdID FROM tblProdfore;")) {
            return false;
        }
        
        return true;
    }

}