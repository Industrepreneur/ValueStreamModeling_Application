using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_products_oper_routing : WhatifGridPage {
    protected string TABLE_NAME_ROUTING {
        get {
            if (ViewState["TABLE_NAME_ROUTING"] == null) {
                ViewState["TABLE_NAME_ROUTING"] = "";
            }
            return (string)ViewState["TABLE_NAME_ROUTING"];
        }
        set { ViewState["TABLE_NAME_ROUTING"] = value; }
    }

    ProductDelegatePage helperProduct;

    public whatif_products_oper_routing() {
        PAGENAME = "whatif_products_oper_routing.aspx";
        featureHelper = new ProdOperRoutingDelegate();

        int value = 1;
        helperProduct = new ProductDelegatePage(value);

        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;
        fieldsNonEditable[2] = true;

        wantTwoHeaders = true;
        

    }

    protected void Page_Load(object sender, EventArgs e) {

        if (dropListProducts.SelectedItem == null) {
            dropListProducts.SelectedIndex = 0;
        }
        if (!Page.IsPostBack) {
            TABLE_NAME = "tbloper";
            TABLE_NAME_ROUTING = "tblOperFrTo";
            SetOperationSelection();
            if (comboSelectedOper.Items.Count > 0) {
                comboSelectedOper.SelectedValue = "-1"; // default is from 'all' operations
            }
            tableSync.UpdateOpNumbers();
            SetRoutingData();
            try {
                SetupDefRoutingWarning(int.Parse(dropListProducts.SelectedValue));
            } catch (Exception) { }
        }

        try {
            base.Page_Load(sender, e);
        } catch (Exception) { }
    }



    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        string selectedProduct = (dropListProducts.SelectedItem == null) ? "''" : dropListProducts.SelectedValue;
        return (featureHelper as ProdOperRoutingDelegate).GetCommandString(commandType, commandString, selectedProduct);
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
        dropListProducts.DataBind();
        srcSelectedOper.DataFile = dataFile;
        srcComboOperTo.DataFile = dataFile;
        FillGridRouting();
        tableSync = new TableSyncOperRouting(userDir);
        //srcSortRouting.DataFile = dataFile;
        //InitializeSortRoutingComp();
    }

    private void FillGridRouting() {
        /*gridRouting.BorderStyle = BorderStyle.Solid;
        gridRouting.BorderWidth = 2;
        gridRouting.BorderColor = System.Drawing.Color.Black;*/

        List<string> opers = new List<string>();
        opers.Add("OpNam");

        TemplateField template3 = new TemplateField();
        GridViewTemplate itemTemplate3 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "opnam1", "lblOperFr");
        template3.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate3.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((Label)container.FindControl("lblOperTo")).Text;
                dict["Per"] = ((Label)container.FindControl("lblPer")).Text;
                dict["opnam1"] = ((Label)container.FindControl("lblOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("lbltonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("lblfromnum")).Text;
                return dict;
            });
        GridViewTemplate editTemplate3 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "opnam1", "txtOperFr");
        template3.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate3.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((Label)container.FindControl("txtOperTo")).Text;
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["opnam1"] = ((Label)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });
        template3.HeaderTemplate = new GridViewTemplate("From Operation", "Operation that the product is coming from.", "lblOperFrom");
        gridRouting.Columns.Add(template3);

        TemplateField template = new TemplateField();
        GridViewTemplate itemTemplate;

        itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "ToOpName", "lblOperTo");
        template.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((Label)container.FindControl("lblOperTo")).Text;
                dict["Per"] = ((Label)container.FindControl("lblPer")).Text;
                dict["opnam1"] = ((Label)container.FindControl("lblOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("lbltonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("lblfromnum")).Text;
                return dict;
            });

        GridViewTemplate editTemplate;

        editTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "ToOpName", "txtOperTo");
        template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((Label)container.FindControl("txtOperTo")).Text;
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["opnam1"] = ((Label)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template.HeaderTemplate = new GridViewTemplate("To Operation", "Operation that the product is arriving at.", "lblOperTo");
        gridRouting.Columns.Add(template);
        /************************************************************/

        TemplateField template2 = new TemplateField();
        GridViewTemplate itemTemplate2 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "Per", "lblPer");
        template2.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate2.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((Label)container.FindControl("lblPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("lblOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("lblOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("lbltonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("lblfromnum")).Text;
                return dict;
            });

        GridViewTemplate editTemplate2 = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, "Per", "txtPer");
        template2.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate2.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template2.HeaderTemplate = new GridViewTemplate("Percent Routed", "Percent of the product that is being routed between two operations.", "lblPer");
        gridRouting.Columns.Add(template2);

        TemplateField template4 = new TemplateField();
        GridViewTemplate itemTemplate4 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "fromnum", "lblfromnum");
        template4.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate4.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((Label)container.FindControl("lblPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("lblOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("lblOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("lbltonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("lblfromnum")).Text;
                return dict;
            });

        GridViewTemplate editTemplate4 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "fromnum", "txtfromnum");
        template4.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate4.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template4.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.NONEDITABLE_DATA, "fromnum", "txtfromnum");
        template4.HeaderTemplate = new GridViewTemplate("From Operation Number", "Number of the operation that the product is coming from.", "lblfromnum");
        template4.SortExpression = "Per";
        gridRouting.Columns.Add(template4);

        TemplateField template5 = new TemplateField();
        GridViewTemplate itemTemplate5 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "tonum", "lbltonum");
        template5.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            itemTemplate5.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((Label)container.FindControl("lblPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("lblOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("lblOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("lbltonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("lblfromnum")).Text;
                return dict;
            });

        GridViewTemplate editTemplate5 = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, "tonum", "txttonum");
        template5.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate5.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["ToOpName"] = ((Label)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((Label)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template5.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.NONEDITABLE_DATA, "tonum", "txttonum");
        template5.HeaderTemplate = new GridViewTemplate("To Operation Number", "Number of the operation that the product is arriving at.", "lbltonum");
        gridRouting.Columns.Add(template5);

        //gridRouting.Columns[0].Visible = false;
    }

    protected void SetRoutingData() {
        string selectedOper = (comboSelectedOper.SelectedItem == null) ? "" : comboSelectedOper.SelectedItem.Text;
        string product = (dropListProducts.SelectedItem == null) ? "" : dropListProducts.SelectedItem.Text;
        string whereOperPart = " AND (tblOper.OpNam = '" + selectedOper + "')";
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        /*string commandRouting = "SELECT tblOperFrTo.RecID, tblProdFore.ProdDesc, tblOper.OpNam AS opnam1, tblOper_1.OpNam AS ToOpName, tblOperFrTo.Per" +
" FROM tblOperFrTo INNER JOIN ((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblOper AS tblOper_1 ON tblProdFore.ProdID = tblOper_1.ProdFore) ON (tblOperFrTo.OpNumT = tblOper_1.OpID) AND (tblOperFrTo.OpNumF = tblOper.OpID)" +
" WHERE (((tblProdFore.ProdDesc)=\"" + product + "\") AND ((tblOper.OpNam)<>\"stock\" And (tblOper.OpNam)<>\"scrap\") AND ((tblOper_1.OpNam)<>\"dock\")) AND (tblOper.OpNam = '" + selectedOper + "');";
        */
        string commandRouting = "SELECT " + TABLE_NAME_ROUTING + ".RecID, tblProdFore.ProdDesc, tblOper.OpNam AS opnam1, tblOper_1.OpNam AS ToOpName, " + TABLE_NAME_ROUTING + ".Per, " + TABLE_NAME_ROUTING + ".fromnum," + TABLE_NAME_ROUTING + ".tonum" +
            " FROM " + TABLE_NAME_ROUTING + " INNER JOIN ((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblOper AS tblOper_1 ON tblProdFore.ProdID = tblOper_1.ProdFore) ON (" + TABLE_NAME_ROUTING + ".OpNumT = tblOper_1.OpID) AND (" + TABLE_NAME_ROUTING + ".OpNumF = tblOper.OpID)" +
            " WHERE (((tblProdFore.ProdDesc)=\"" + product + "\"))"; // AND ((tblOper.OpNam)<>\"stock\" And (tblOper.OpNam)<>\"scrap\") AND ((tblOper_1.OpNam)<>\"dock\"))";
        if (!selectedOper.Equals("All")) {
            commandRouting += whereOperPart;
        }
        string orderBy;
        try {
            orderBy = GetOrderBy(sortedTableName2);
        } catch (Exception) {
            orderBy = "";
        }
        if (orderBy.Trim().Equals("")) {
            orderBy = " ORDER BY RecID";
        }
        commandRouting += orderBy + ";";
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataView dv = dt.DefaultView;
                gridRouting.DataSource = dv;
                gridRouting.DataBind();
                
                connec.Close();
                if (!selectedOper.Equals("All")) {
                    ShowHideFromOperColumn(false);
                }
            } catch (Exception ex) {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage();
            }
        }
    }

    protected void SetOperationSelection() {
        SetOperationSelection(false);
    }

    protected void SetOperationSelection(bool productChanged) {
        string product;
        if (dropListProducts.SelectedValue == null || dropListProducts.SelectedValue.Equals("")) {
            product = "''";
        } else {
            product = dropListProducts.SelectedValue;
        }
        SetRoutingLabel();
        string selectedValue = comboSelectedOper.SelectedValue;

        srcSelectedOper.SelectCommand = "SELECT [OpID], [OpNam] FROM tblOper WHERE [ProdFore] = " + product + " AND (tblOper.OpNam)<>\"stock\" AND (tblOper.OpNam)<>\"scrap\";";
        try {
            comboSelectedOper.DataBind();
            comboSelectedOper.Items.Add(new ListItem("All", "-1"));

            if (!productChanged && selectedValue != null && comboSelectedOper.Items.FindByValue(selectedValue) != null) {
                // same product and there was selected value
                comboSelectedOper.SelectedValue = selectedValue;
            } else {
                // different product or the selected item was deleted
                comboSelectedOper.SelectedValue = "-1";
            }
            SetupOperColumns(comboSelectedOper.SelectedValue);
        } catch (Exception) {
            Master.ShowErrorMessage("An error has occured in operation selection for routings.");
        }
    }


    protected void SetRoutingLabel() {
        string product;
        if (dropListProducts.SelectedItem == null) {
            product = "";
        } else {
            product = dropListProducts.SelectedItem.Text;
        }
        lblSelectedOper.Text = "Routings of " + product + " from Operation: ";
    }

    protected void comboSelectedOper_SelectedIndexChanged(object sender, EventArgs e) {
        string value = comboSelectedOper.SelectedValue;
        SetupOperColumns(value);
        this.SetRoutingData();
    }

    protected void SetupOperColumns(string selectedValue) {
        if (selectedValue.Equals("-1")) {
            ShowHideFromOperColumn(true);
        } else {
            ShowHideFromOperColumn(false);
        }
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

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e) {
        string value = dropListProducts.SelectedValue;
        this.SetData();
        SetOperationSelection(true);
        this.SetRoutingData();
    }

    protected void gridRouting_RowEditing(object sender, GridViewEditEventArgs e) {

    }
    protected void gridRouting_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {

    }
    protected void gridRouting_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }


    protected void SetComboOpList() {
        srcComboOperTo.SelectCommand = "SELECT OpID, OpNam from tblOper WHERE ProdFore = " + dropListProducts.SelectedValue;
    }

    // maybe won't need
    protected override List<string> GetDropList(string name) {
        List<string> dropList = new List<string>();
        string comm = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        if (name.Equals("ProdDesc")) {
            comm = "SELECT ProdDesc, ProdId FROM tblprodfore ORDER BY ProdDesc";
        } else if (name.ToLower().Equals("opnam")) {
            comm = "SELECT DISTINCT OpNam FROM tbloper ORDER BY OpNam";
        } else if (name.Equals("EquipDesc")) {
            comm = "SELECT EquipDesc, EquipID FROM tblequip ORDER BY EquipDesc";
        }
        OleDbCommand cmd = new OleDbCommand(comm, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++) {
                    dropList.Add(dt.Rows[i][0].ToString()); // TODO finish getting full value/text fields
                }

                connec.Close();

            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        return dropList;
    }

    protected void gridRouting_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btnSender = e.CommandSource as Button;
        GridViewRow row = btnSender.NamingContainer as GridViewRow;
        if (e.CommandName.Equals("Edit")) {
            gridRouting.EditIndex = row.RowIndex;
        } else if (e.CommandName.Equals("CancelUpdate")) {
            gridRouting.EditIndex = -1;
        } else if (e.CommandName.Equals("Update")) {
            var newValues = GetValues(row);
            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            OleDbCommand cmd;
            cmd = new OleDbCommand("UPDATE tblOperFrTo SET Per = ? WHERE RecID = ?;", connec);
            
            {
                try {
                    connec.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("Per", newValues["Per"]);
                    cmd.Parameters.AddWithValue("RecID", gridRouting.DataKeys[row.RowIndex]["RecID"]);

                    int result = cmd.ExecuteNonQuery();
                    gridRouting.EditIndex = -1;
                    connec.Close();
                    SetModelModified(true, false);
                } catch {
                    try {
                        connec.Close();
                        connec = null;
                    } catch { }
                }
            }
        }
        SetRoutingData();
        if (e.CommandName.Equals("Edit")) {
            try {
                //int _rowIndex = int.Parse(e.CommandArgument.ToString());
                int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                Control editControl = gridRouting.Rows[row.RowIndex].FindControl(ROUTING_GRID_IDS[_columnIndex]) as Control;
                // focus on the double-clicked control
                if (editControl != null && (editControl is TextBox || editControl is AjaxControlToolkit.ComboBox || editControl is CheckBox)) {
                    editControl.Focus();

                }

            } catch (Exception) {

            }

        }
    }

    private string[] ROUTING_GRID_IDS = { null, "txtOperFr", "txtOperTo", "txtPer" };

    protected void btnShowTimes_Click(object sender, EventArgs e) {
        btnShowTimes.Enabled = false;
        if (btnShowTimes.Text.Equals("Show Real Times")) {
            grid.Columns[0].Visible = false;
            this.TABLE_NAME = "tblOper_d";
            btnShowTimes.Text = "Edit Operations";
            {
                ClassF calc = new ClassF(GetDirectory() + userDir);
                try {
                    calc = new ClassF(GetDirectory() + userDir);
                    calc.setGlobalVar();
                    calc.runsqlado("DELETE * FROM tblOper_d;");
                    calc.runsqlado("DELETE * FROM zstblerrors;");
                    calc.inOperRoutingPages = true;
                    calc.MakeActualoper_all();
                    string errorMsg = calc.GetErrorMessage();
                    lblErrorRealTimesOper.Text = "Errors in calculating expressions in operation table:<br/>" + errorMsg;
                    lblErrorRealTimesOper.Visible = !errorMsg.Trim().Equals(String.Empty);
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                    Master.ShowErrorMessage("An error has occured while calculating expressions.");
                } finally {
                    calc.Close();
                }
            }
        } else {
            grid.Columns[0].Visible = true;
            this.TABLE_NAME = "tbloper";
            btnShowTimes.Text = "Show Real Times";
        }

        this.SetData();
        btnShowTimes.Enabled = true;
    }

    protected override void grid_RowCommand(object sender, GridViewCommandEventArgs e) {
        base.grid_RowCommand(sender, e);
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }
        if (e.CommandName.Equals("Update")) {
            SetOperationSelection();
            tableSync.UpdateOpNumbers();
            SetRoutingData();
        }
    }

    protected void gridRouting_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT_ROUTING) as Button;
            string tooltip = "Double-click to edit routing";
            if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                tooltip = "Double-click to update routing";
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
                            Control control = e.Row.Cells[columnIndex].FindControl(ROUTING_GRID_IDS[columnIndex]);
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

    
    
    protected void btnRoutingShowTimes_Click(object sender, EventArgs e) {
        btnRoutingShowTimes.Enabled = false;
        if (btnRoutingShowTimes.Text.Equals("Show Real Percentage")) {
            gridRouting.Columns[0].Visible = false;
            
            this.TABLE_NAME_ROUTING = "tblOpFrTo_d";
            btnRoutingShowTimes.Text = "Edit Routings";
            {
                ClassF calc = new ClassF(GetDirectory() + userDir);
                try {
                    calc = new ClassF(GetDirectory() + userDir);
                    calc.setGlobalVar();
                    calc.runsqlado("DELETE * FROM tblOpFrTo_d;");
                    calc.runsqlado("DELETE * FROM zstblerrors;");
                    calc.inOperRoutingPages = true;
                    calc.MakeActualroute_all();
                    string errorMsg = calc.GetErrorMessage();
                    lblErrorRealTimesRouting.Text = "Errors in calculating expressions in routing table:<br/>" + errorMsg;
                    lblErrorRealTimesRouting.Visible = !errorMsg.Trim().Equals(String.Empty);
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                    Master.ShowErrorMessage("An error has occured while calculating expressions.");
                } finally {
                    calc.Close();
                }
            }
        } else {
            gridRouting.Columns[0].Visible = true;
            this.TABLE_NAME_ROUTING = "tblOperFrTo";
            btnRoutingShowTimes.Text = "Show Real Percentage";
        }

        this.SetData();
        btnRoutingShowTimes.Enabled = true;
    }

    protected override void SetData() {
        SetTableQueryString();
        base.SetData();
    }

    protected void ShowHideFromOperColumn(bool show) {
        gridRouting.Columns[1].Visible = show;

    }

    protected void SetTableQueryString() {
        if (TABLE_NAME.ToLower().Equals("tbloper")) {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID FROM " + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdFore.ProdID";
        } else {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID, tblProdFore.ProdDesc, tblEquip.EquipId, tblEquip.EquipDesc FROM (" + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdfore.ProdID) INNER JOIN tblEquip ON " + TABLE_NAME + ".EqId = tblEquip.EquipID";
        }
    }

    protected override Control GetSortButtonContainer2() {
        return buttondivRouting;
    }

    protected override Panel GetSortPanelContainer2() {
        return sortPanelRoutingContainer;
    }

    protected override void btnOkSort2_Click(object sender, EventArgs e) {
        base.btnOkSort2_Click(sender, e);
        this.SetRoutingData();
    }

    protected override bool LocalTablesLinked() {
        if (!UpdateSql("SELECT ProdID FROM tblProdfore;")) {
            return false;
        }
        if (!UpdateSql("SELECT OpID FROM tblOper;")) {
            return false;
        }
        if (!UpdateSql("SELECT RecID FROM tblOperFrTo;")) {
            return false;
        }
        return true;
    }

    protected void SetupDefRoutingWarning(int prodid) {
        string userDatabase = GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE;
        if (OperDelegate.DuplicateOperNamesOrNumbers(userDatabase, prodid)) {
            lblWarnDefRouting.Text = DbUse.DUPLICATE_OPER_WARNING;
            lblWarnDefRouting.Visible = true;
        } else {
            lblWarnDefRouting.Text = "";
            lblWarnDefRouting.Visible = false;

        }
    }

    

    
}