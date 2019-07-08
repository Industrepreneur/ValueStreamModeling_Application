using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Collections.Specialized;
using System.Text;

public partial class operations : InputGridPage {


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

    public operations() {
        PAGENAME = "/input/products/operations.aspx";
        featureHelper = new ProdOperRoutingDelegate();

        int value = 1;
        helperProduct = new ProductDelegatePage(value);

        wantTwoHeaders = true;

        //this.nonEdits = true;
        //fieldsNonEditable = new bool[FIELDS.Length];
        //fieldsNonEditable[1] = true;
    }

    protected void Page_Load(object sender, EventArgs e) {
        if (dropListProducts.Items.Count > 0) {
            if (dropListProducts.SelectedItem == null) {
                dropListProducts.Items[0].Selected = true;
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
        } else {
            TABLE_NAME = "tbloper";
            TABLE_NAME_ROUTING = "tblOperFrTo";
            SetOperationSelection();
            gridRouting.ShowFooter = false;
            SetRoutingData();
            grid.ShowFooter = false;
            thirdPanel.Visible = false;
            pnlInsertRouting.Visible = false;
            Button btnSortRouting = buttondivRouting.FindControl(InputPageControls.BTN_SORT2) as Button;
            btnSortRouting.Enabled = false;
            Master.ShowErrorMessage("No products are defined yet. Please go to the input products page and create a product first.");
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
        pnlMenu.Controls.Add(new LiteralControl("<h2>Products</h2>"));
        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        if (IsWhatifMode()) {
            Response.Redirect("whatif_" + PAGENAME);
        }
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        TBWEinsertOperTable.TargetControlID = txtInsertTable.ID;
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        srcProductsList.DataFile = dataFile;
        dropListProducts.DataBind();
        srcSelectedOper.DataFile = dataFile;
        srcComboOperTo.DataFile = dataFile;
        FillGridRouting();
        tableSync = new TableSyncOperRouting(userDir);

        string sheet = "Cheat Sheat Product Operations and Routings Page";
        Master.SetHelpSheet(sheet + ".pdf", sheet);

    }


    //THIS IS THE FROM-TO ROUTING GRID DEFINITION
    //HOW IS THIS GETTING THE BUTTON TYPE?
    private string[] ROUTING_GRID_IDS = { null, "txtOperFr", "txtOperTo", "txtPer" };

    private void FillGridRouting() {

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
        GridViewTemplate editTemplate3 = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.COMBODATA, "opnam1", "txtOperFr", opers);
        editTemplate3.newer = true;
        template3.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate3.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperTo")).Text;
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["opnam1"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });
        template3.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.COMBODATA, "opnam1", "txtOperFr", opers);
        template3.HeaderTemplate = new GridViewTemplate("From Operation", "Operation that the product is coming from.", "lblOperFrom");
        template3.SortExpression = "FromOpName";
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

        editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.COMBODATA, "ToOpName", "txtOperTo", opers);
        editTemplate.newer = true;
        template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
            editTemplate.InstantiateIn(container);
        },
            delegate(Control container) {
                OrderedDictionary dict = new OrderedDictionary();
                dict["ToOpName"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperTo")).Text;
                dict["Per"] = ((TextBox)container.FindControl("txtPer")).Text;
                dict["opnam1"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.COMBODATA, "ToOpName", "txtOperTo", opers);
        template.HeaderTemplate = new GridViewTemplate("To Operation", "Operation that the product is arriving at.", "lblOperTo");
        template.SortExpression = "ToOpName";
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
                dict["ToOpName"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });

        template2.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.DATA, "Per", "txtPer");
        template2.HeaderTemplate = new GridViewTemplate("Percent Routed", "Percent of the product that is being routed between two operations.", "lblPer");
        template2.SortExpression = "Per";
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
                dict["ToOpName"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperFr")).Text;
                dict["tonum"] = ((Label)container.FindControl("txttonum")).Text;
                dict["fromnum"] = ((Label)container.FindControl("txtfromnum")).Text;
                return dict;
            });


        //THIS IS THE NON-EDITABLE FOOTER!!!!
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
                dict["ToOpName"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperTo")).Text;
                dict["opnam1"] = ((AjaxControlToolkit.ComboBox)container.FindControl("txtOperFr")).Text;
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
        string commandRouting = "SELECT " + TABLE_NAME_ROUTING + ".RecID, tblProdFore.ProdDesc, tblOper.OpNam AS opnam1, tblOper_1.OpNam AS ToOpName, " + TABLE_NAME_ROUTING + ".tonum, " + TABLE_NAME_ROUTING + ".fromnum, " + TABLE_NAME_ROUTING + ".Per as Per" +
            " FROM " + TABLE_NAME_ROUTING + " INNER JOIN ((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblOper AS tblOper_1 ON tblProdFore.ProdID = tblOper_1.ProdFore) ON (" + TABLE_NAME_ROUTING + ".OpNumT = tblOper_1.OpID) AND (" + TABLE_NAME_ROUTING + ".OpNumF = tblOper.OpID)" +
            " WHERE (((tblProdFore.ProdDesc)=\"" + product + "\"))"; // AND ((tblOper.OpNam)<>\"stock\" And (tblOper.OpNam)<>\"scrap\") AND ((tblOper_1.OpNam)<>\"dock\"))";
        if (!selectedOper.Equals("All")) {
            commandRouting += whereOperPart;
        }
        string orderBy;
        try {
            orderBy = GetOrderBy("tbloperfrto");
        } catch (Exception) {
            orderBy = "";
        }
        if (orderBy.Trim().Equals("")) {
            orderBy = " ORDER BY RecID";
        }
        commandRouting += orderBy + ";";
        //commandRouting += " ORDER BY Per;";

        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                bool empty = false;
                ;
                if (dt.Rows.Count == 0) {
                    empty = true;
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                }
                DataView dv = dt.DefaultView;
                gridRouting.DataSource = dv;
                gridRouting.DataBind();
                if (empty) {
                    // hide the buttons
                    GridViewRow row = gridRouting.Rows[0];
                    Button button = row.FindControl("btnEdit") as Button;
                    if (button != null) {
                        button.Visible = false;
                    }
                    button = row.FindControl("btnDelete") as Button;
                    if (button != null) {
                        button.Visible = false;
                    }

                }
                TextBox txtPer = gridRouting.FooterRow.FindControl("txtPer") as TextBox;
                if (txtPer != null && txtPer.Text.Trim().Equals(String.Empty)) {
                    txtPer.Text = "0";
                }
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

    protected void FillComboOperFr(AjaxControlToolkit.ComboBox ddl) {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string commandRouting = "SELECT OpID, OpNam from tblOper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND (tblOper.OpNam)<>\"stock\" AND (tblOper.OpNam)<>\"scrap\" ORDER BY OpNam;";
        ;
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try {
                connec.Open();
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                if (ddl != null) {
                    ddl.Items.Clear();
                    for (int i = 0; i < ds.Rows.Count; i++) {
                        ListItem item = new ListItem(ds.Rows[i]["OpNam"].ToString());
                        ddl.Items.Add(item);
                    }
                }
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

    protected override void RowUpdate(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);

        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);
        try {
            int opId = Convert.ToInt32(grid.DataKeys[rowIndex].Value.ToString());
            string opName = MyUtilities.clean(newValues["OpNam"].ToString());
            int opNumber = Convert.ToInt32(newValues["OpNum"].ToString());
            int prodId = Convert.ToInt32(dropListProducts.SelectedValue);

            string oldOpName = GetDatabaseField("OpNam", "OpID", opId, "tblOper");
            int oldOpNum = Int32.MinValue;
            try {
                oldOpNum = int.Parse(GetDatabaseField("OpNum", "OpID", opId, "tblOper"));
            } catch (Exception) {
                
            }
            if (opName.Equals("DOCK") || opName.Equals("STOCK") || opName.Equals("SCRAP")) {
                if (oldOpName.ToUpper().Equals(opName)) {
                    throw new Exception("Attempt to edit operation names DOCK, STOCK, SCRAP.");
                } else {
                    throw new Exception("Attempt to use reserved operation names DOCK, STOCK, SCRAP.");
                }
            }

            // fill in parameters
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++) {
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (CHECKBOXES[i]) {
                        cmd.Parameters.AddWithValue(FIELDS[i], ((bool)newValues[FIELDS[i]]) ? "1" : "0");
                    } else {
                        cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(newValues[FIELDS[i]].ToString()));
                    }
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);

            connec.Open();
            int result = cmd.ExecuteNonQuery();
            grid.EditIndex = -1;
            connec.Close();
            //UpdateSql("UPDATE tblOper SET OpNum = " + opNumber + " WHERE OpNam = '" + opName + "' AND ProdFore = " + prodId + ";");
            this.SetData();
            this.SetRoutingData();

            if (mode.Equals("Standard")) {
                HideAdvancedColumns();
            }
            string operMessage = GetOperInfoMessage(opName, opNumber, prodId, opId, oldOpName, oldOpNum);
            if (!operMessage.Equals("")) {
                Master.ShowInfoMessage(operMessage);
            }
            
        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
            try {
                connec.Close();
            } catch (Exception) { }
            
            Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
        }

    }



    protected void gridRouting_RowEditing(object sender, GridViewEditEventArgs e) {

    }
    protected void gridRouting_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {

    }
    protected void gridRouting_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }

    protected override void InsertRow(bool goToEdit) {
        GridViewRow row = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = row.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            cmd.Parameters.AddWithValue("ProdFore", int.Parse(dropListProducts.SelectedValue));
            cmd.Parameters.AddWithValue("ProdDesc", dropListProducts.SelectedItem.Text);
            string desc = "";
            string opnum = "";
            for (int i = 1; i < txtControls.Length; i++) {
                string value;
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (COMBOS[i]) {
                        if (((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedItem != null) {
                            value = ((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedValue;
                        } else {
                            value = MyUtilities.clean(((AjaxControlToolkit.ComboBox)txtControls[i]).Text);
                        }
                    } else if (CHECKBOXES[i]) {
                        value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                    } else {
                        value = MyUtilities.clean(((TextBox)txtControls[i]).Text);
                    }
                    if ((FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam"))) {
                        value = value.ToUpper();

                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    if (i == 1) {
                        desc = value;
                    } else if (i == 2) {
                        opnum = value;
                    }
                }
            }
            try {
                int prodId = int.Parse(dropListProducts.SelectedValue);
                int opNumber = int.Parse(opnum);
                string operName = desc;
                if (operName.Trim().Equals(String.Empty)) {
                    throw new Exception("Cannot insert oper line. Empty operation name.");
                }
                if (operName.Equals("DOCK") || operName.Equals("STOCK") || operName.Equals("SCRAP")) {
                    throw new Exception("Cannot insert oper line. Attempt to use reserved operation names DOCK, STOCK, SCRAP.");
                }
                
                connec.Open();
                try {
                    int result = cmd.ExecuteNonQuery();
                } catch (Exception excep) {
                    Exception excepNew = new Exception("Error in executing insert query. Command string: " + cmd.CommandText + ". Exception message: " + excep.Message, excep);
                    throw excepNew;
                }
                connec.Close();
                //UpdateSql("UPDATE tblOper SET OpNum = " + opNumber + " WHERE OpNam = '" + operName + "' AND ProdFore = " + prodId + ";");
                this.SetData();
                //SetRoutingData();
                if (goToEdit) {
                    try {
                        SetData();
                        int id = int.Parse(GetId(desc));
                        GoToEditMode(id);
                    } catch (Exception) { }
                }
                string operMessage = GetOperInfoMessage(operName, opNumber, prodId);
                if (!operMessage.Equals("")) {
                    Master.ShowInfoMessage(operMessage);
                }
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                SaveInsertValues(grid.FooterRow, TEXT_BOX_IDS);
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
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
                        "cursor:pointer;";

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

                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.DataBind();


                    try
                    {

                        foreach (Control control in cell.Controls)
                        {
                            foreach (Control control2 in control.Controls)
                            {
                                if (control2 is Label)
                                {
                                    Label lbl = control2 as Label;
                                    lbl.CssClass = "padding";
                                    double num = Double.Parse(lbl.Text);

                                    cell.HorizontalAlign = HorizontalAlign.Center;
                                    break;
                                }
                                else if (control2 is TextBox || control2 is AjaxControlToolkit.ComboBox || control2 is CheckBox)
                                {
                                    cell.HorizontalAlign = HorizontalAlign.Center;
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }
        // e.Row.RowState == DataControlRowState.Edit DOESN'T WORK for alternating row!!!
        if (e.Row.RowType == DataControlRowType.DataRow && ((e.Row.RowState & DataControlRowState.Edit) > 0)) {

            AjaxControlToolkit.ComboBox ddl = (AjaxControlToolkit.ComboBox)e.Row.FindControl("txtOperTo");

            FillComboOperTo(ddl);
            ddl.DataBind();

            ddl = (AjaxControlToolkit.ComboBox)e.Row.FindControl("txtOperFr");
            FillComboOperFr(ddl);
            ddl.DataBind();
        } else if (e.Row.RowType == DataControlRowType.Footer) {
            AjaxControlToolkit.ComboBox ddl = (AjaxControlToolkit.ComboBox)e.Row.FindControl("txtOperTo");
            FillComboOperTo(ddl);
            ddl.DataBind();
            ddl = (AjaxControlToolkit.ComboBox)e.Row.FindControl("txtOperFr");
            FillComboOperFr(ddl);
            ddl.DataBind();
        }
        if (e.Row.RowType == DataControlRowType.Footer) {
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_INSERT) as Button;
            string tooltip = "Double-click to add routing";
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

    protected void FillComboOperTo(AjaxControlToolkit.ComboBox ddl) {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string commandRouting = "SELECT OpID, OpNam from tblOper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND [OpNam]<>\"dock\" ORDER BY OpNam;";
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try {
                connec.Open();
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                if (ddl != null) {
                    ddl.Items.Clear();
                    for (int i = 0; i < ds.Rows.Count; i++) {
                        ListItem item = new ListItem(ds.Rows[i]["OpNam"].ToString());
                        ddl.Items.Add(item);
                    }
                }
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

    protected void SetComboOpList() {
        srcComboOperTo.SelectCommand = "SELECT OpID, OpNam from tblOper WHERE ProdFore = " + dropListProducts.SelectedValue;
    }


    protected void gridRouting_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }

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
        } else if (e.CommandName.Equals("Delete")) {
            OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            OleDbCommand cmd = new OleDbCommand("DELETE FROM [tblOperFrTo] WHERE (RecID = @RecID)", connec);
            cmd.Parameters.AddWithValue("RecID", gridRouting.DataKeys[row.RowIndex]["RecID"]);

            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                gridRouting.EditIndex = -1;
                connec.Close();
                SetModelModified(true, true);
            } catch (Exception exp) {
                logFiles.ErrorLog(exp);
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        } else if (e.CommandName.Equals("Insert")) {
            TextBox txtPer = gridRouting.FooterRow.FindControl("txtPer") as TextBox;
            AjaxControlToolkit.ComboBox comboOperTo = gridRouting.FooterRow.FindControl("txtOperTo") as AjaxControlToolkit.ComboBox;

            string[] entries;
            if (comboSelectedOper.SelectedValue.Equals("-1")) {
                // from oper column is visible
                entries = new string[3];
                AjaxControlToolkit.ComboBox comboOperFr = gridRouting.FooterRow.FindControl("txtOperFr") as AjaxControlToolkit.ComboBox;
                entries[0] = MyUtilities.clean(comboOperFr.Text);
                entries[1] = MyUtilities.clean(comboOperTo.Text);
                entries[2] = MyUtilities.clean(txtPer.Text);
            } else {
                entries = new string[2];
                entries[0] = MyUtilities.clean(comboOperTo.Text);
                entries[1] = MyUtilities.clean(txtPer.Text);
            }

            if (!InsertRoutingLine(entries)) {
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            } else {
                SetModelModified(true, true);
            }
            tableSync.UpdateOpNumbers();
        } else if (e.CommandName.Equals("CancelUpdate")) {
            gridRouting.EditIndex = -1;
        } else if (e.CommandName.Equals("Update")) {
            var newValues = GetValues(row);
            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            OleDbCommand cmd;
            if (comboSelectedOper.SelectedValue.Equals("-1")) {
                cmd = new OleDbCommand("UPDATE tblOperFrTo SET OpNumF = ?, FromOpName = ?, OpNumT = ?, ToOpName = ?, Per = ? WHERE RecID = ?;", connec);
            } else {
                cmd = new OleDbCommand("UPDATE tblOperFrTo SET OpNumT = ?, ToOpName = ?, Per = ? WHERE RecID = ?;", connec);
            }
            OleDbCommand comm = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + newValues["ToOpName"] + "';", connec);
            OleDbDataAdapter adapter = new OleDbDataAdapter(comm);

            {
                try {
                    connec.Open();
                    DataTable ds = new DataTable();
                    adapter.Fill(ds);
                    cmd.CommandType = CommandType.Text;
                    if (comboSelectedOper.SelectedValue.Equals("-1")) {
                        OleDbCommand comm2 = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + newValues["opnam1"] + "';", connec);
                        OleDbDataAdapter adapter2 = new OleDbDataAdapter(comm2);
                        DataTable dt = new DataTable();
                        adapter2.Fill(dt);
                        cmd.Parameters.AddWithValue("OpNumF", dt.Rows[0]["OpID"]);
                        cmd.Parameters.AddWithValue("FromOpName", newValues["opnam1"]);
                    }

                    cmd.Parameters.AddWithValue("OpNumT", ds.Rows[0]["OpID"]);
                    cmd.Parameters.AddWithValue("ToOpName", newValues["ToOpName"]);
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
                    } catch {
                        Master.ShowErrorMessage("An error has occured and the data could not get updated.");
                    }
                }
                tableSync.UpdateOpNumbers();
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
    //protected void btnShowTimes_Click(object sender, EventArgs e) {
    //    btnShowTimes.Enabled = false;
    //    if (btnShowTimes.Text.Equals("Show Real Times")) {
    //        grid.Columns[0].Visible = false;
    //        if (grid.FooterRow != null) {
    //            grid.FooterRow.Visible = false;
    //        }
    //        grid.ShowFooter = false;
    //        thirdPanel.Visible = false;
    //        this.TABLE_NAME = "tblOper_d";
    //        btnShowTimes.Text = "Edit Operations";
    //        {
    //            ClassF calc = new ClassF(GetDirectory() + userDir);
    //            try {
    //                calc = new ClassF(GetDirectory() + userDir);
    //                calc.setGlobalVar();
    //                calc.runsqlado("DELETE * FROM tblOper_d;");
    //                calc.runsqlado("DELETE * FROM zstblerrors;");
    //                calc.inOperRoutingPages = true;
    //                calc.MakeActualoper_all();
    //                string errorMsg = calc.GetErrorMessage();
    //                lblErrorRealTimesOper.Text = "Errors in calculating expressions in operation table:<br/>" + errorMsg;
    //                lblErrorRealTimesOper.Visible = !errorMsg.Trim().Equals(String.Empty);

    //            } catch (Exception ex) {
    //                logFiles.ErrorLog(ex);
    //                Master.ShowErrorMessage("An error has occured while calculating expressions.");
    //            } finally {
    //                calc.Close();
    //            }
    //        }
    //    } else {
    //        grid.Columns[0].Visible = true;
    //        if (grid.FooterRow != null) {
    //            grid.FooterRow.Visible = true;
    //        }
    //        grid.ShowFooter = true;
    //        thirdPanel.Visible = true;
    //        this.TABLE_NAME = "tbloper";
    //        btnShowTimes.Text = "Show Real Times";
    //    }

    //    this.SetData();
    //    btnShowTimes.Enabled = true;
    //}

    protected void btnDefaultRoutingPart_Click(object sender, EventArgs e) {
        ClassF classF = new ClassF(GetDirectory() + userDir);
        try {
            classF.addfromto_part(int.Parse(dropListProducts.SelectedValue));
            tableSync.SyncTablesOnDefaultRouting(int.Parse(dropListProducts.SelectedValue));
        } catch (Exception) { }
        classF.Close();
        this.SetData();
        SetRoutingData();
    }

    protected override void grid_RowCommand(object sender, GridViewCommandEventArgs e) {
        base.grid_RowCommand(sender, e);
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }
        if (e.CommandName.Equals("Insert") || e.CommandName.Equals("Delete") || e.CommandName.Equals("Update")) {
            SetOperationSelection();
            tableSync.UpdateOpNumbers();
            SetRoutingData();
            try {
                SetupDefRoutingWarning(int.Parse(dropListProducts.SelectedValue));
            } catch (Exception) { }
        }
    }

    protected bool InsertRoutingLine(string[] entries) {
        string[] entriesInOrder = new string[entries.Length];
        for (int i = 0; i < entries.Length; i++) {
            if (entries[i].Trim().Length != 0) {
                entries[i] = entries[i].Trim();
            }
        }
        if (entries.Length > 2) {
            entriesInOrder[0] = MyUtilities.clean(entries[1]);
            entriesInOrder[1] = MyUtilities.clean(entries[2]);
            entriesInOrder[2] = MyUtilities.clean(entries[0]);
        } else {
            entriesInOrder = entries;
        }
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = "INSERT into tblOperFrTo (PartFore, OpNumF, OpNumT, Per, fromopname, ToOpName, ProdDesc) VALUES ( ?, ?, ?, ?, ?, ?, ?);";
        OleDbCommand cmd = new OleDbCommand(command, connec);
        OleDbCommand comm = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + entriesInOrder[0] + "';", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(comm);
        OleDbCommand comm2 = null;
        OleDbDataAdapter adapter2 = null;
        if (entries.Length > 2) {
            comm2 = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + entriesInOrder[2].Trim() + "';", connec);
            adapter2 = new OleDbDataAdapter(comm2);
        }
        {

            try {
                connec.Open();
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                DataTable dt = new DataTable();

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("PartFore", dropListProducts.SelectedValue);
                if (entries.Length > 2) {
                    adapter2.Fill(dt);
                    cmd.Parameters.AddWithValue("OpNumF", dt.Rows[0]["OpID"]);
                } else {
                    if (comboSelectedOper.SelectedValue.Equals("-1")) {
                        // too few arguments
                        throw new Exception();
                    }
                    cmd.Parameters.AddWithValue("OpNumF", comboSelectedOper.SelectedValue);
                }
                cmd.Parameters.AddWithValue("OpNumT", ds.Rows[0]["OpID"]);
                cmd.Parameters.AddWithValue("Per", entriesInOrder[1]);
                if (entries.Length > 2) {
                    cmd.Parameters.AddWithValue("fromopname", entriesInOrder[2].ToUpper());
                } else {
                    cmd.Parameters.AddWithValue("fromopname", comboSelectedOper.SelectedItem.Text);
                }
                cmd.Parameters.AddWithValue("ToOpName", entriesInOrder[0].ToUpper());
                cmd.Parameters.AddWithValue("ProdDesc", dropListProducts.SelectedItem.Text);

                int result = cmd.ExecuteNonQuery();
                gridRouting.EditIndex = -1;

                this.SetRoutingData();
                wasInserted = true;
                connec.Close();
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        return wasInserted;
    }
    protected void btnInsertrouting_Click(object sender, EventArgs e) {
        string fields = txtInsertRouting.Text;
        txtInsertRouting.Text = "";
        string[] lines = fields.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++) {
            string[] entries = lines[i].Split(new Char[] { '\t', ';', ',' });
            if (!InsertRoutingLine(entries)) {
                txtInsertRouting.Text += lines[i] + "\n";
            } else {
                SetModelModified(true);
            }
        }
        if (!txtInsertRouting.Text.Equals("")) {
            extenderInfo.Show();
        }
    }
    protected void btnRoutingShowTimes_Click(object sender, EventArgs e) {
        btnRoutingShowTimes.Enabled = false;
        if (btnRoutingShowTimes.Text.Equals("Show Real Percentage")) {
            gridRouting.Columns[0].Visible = false;
            if (gridRouting.FooterRow != null) {
                gridRouting.FooterRow.Visible = false;
            }
            gridRouting.ShowFooter = false;
            pnlInsertRouting.Visible = false;
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
            if (gridRouting.FooterRow != null) {
                gridRouting.FooterRow.Visible = true;
            }
            gridRouting.ShowFooter = true;
            pnlInsertRouting.Visible = true;
            this.TABLE_NAME_ROUTING = "tblOperFrTo";
            btnRoutingShowTimes.Text = "Show Real Percentage";
        }
        this.SetData();
        this.SetRoutingData();
        btnRoutingShowTimes.Enabled = true;
    }


    protected override void SetData() {
        SetTableQueryString();
        base.SetData();
        GridViewRow row = grid.FooterRow;
        if (row != null) {
            AjaxControlToolkit.ComboBox comboDesc = row.FindControl("comboEdit3") as AjaxControlToolkit.ComboBox;
            if (comboDesc != null) {
                comboDesc.SelectedValue = "NONE";
                comboDesc.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
            }
        }
    }

    protected void ShowHideFromOperColumn(bool show) {
        foreach (GridViewRow row in gridRouting.Rows) {
            AjaxControlToolkit.ComboBox combo = row.FindControl("txtOperFr") as AjaxControlToolkit.ComboBox;
            if (combo != null) {
                combo.Enabled = show;
            }
        }
        if (gridRouting.FooterRow != null) {
            AjaxControlToolkit.ComboBox combo = gridRouting.FooterRow.FindControl("txtOperFr") as AjaxControlToolkit.ComboBox;
            combo.Enabled = show;
        }
        gridRouting.Columns[1].Visible = show;

    }

    protected void SetTableQueryString() {
        if (TABLE_NAME.ToLower().Equals("tbloper")) {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID FROM " + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdFore.ProdID ";
        } else {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID, tblProdFore.ProdDesc, tblEquip.EquipId, tblEquip.EquipDesc FROM (" + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdfore.ProdID) INNER JOIN tblEquip ON " + TABLE_NAME + ".EqId = tblEquip.EquipID ";
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

    protected override bool InsertRecord(string[] entries) {
        bool wasInserted = false;
        if (dropListProducts.SelectedValue != null) {
            string prodDesc = dropListProducts.SelectedItem.Text;
            int prodId = int.Parse(dropListProducts.SelectedValue);

            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            string command = GetCommandString(Command.INSERT);
            OleDbCommand cmd = new OleDbCommand(command, connec);

            {

                cmd.Parameters.AddWithValue("ProdFore", prodId);
                cmd.Parameters.AddWithValue("ProdDesc", prodDesc);
                int i = 1;
                for (int j = 0; j < entries.Length && i < FIELDS.Length; j++) {
                    if (mode.Equals("Standard")) {
                        while (i < FIELDS.Length && ADVANCED_FIELDS[i]) {
                            i++;
                        }
                    }
                    if (i < FIELDS.Length) {
                        string value = MyUtilities.clean(entries[j]);
                        if (value.Trim().Length > 0) {
                            value = value.Trim();
                        }
                        if (FIELDS[i].ToLower().Equals("equipdesc")) {
                            try {
                                string equipId = GetDatabaseField("EquipID", "EquipDesc", value, "tblequip");
                            } catch (Exception exp) {
                                logFiles.ErrorLog(exp);
                                // exception means invalid equip data 
                                return false;
                            }
                        }
                        if (CHECKBOXES[i]) {
                            value = value.ToLower().Equals("true") ? "1" : "0";
                        }
                        if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam")) {
                            value = value.ToUpper();
                        }
                        cmd.Parameters.AddWithValue(FIELDS[i], value);
                        i++;
                    }
                }
                try {
                    string operName = MyUtilities.clean(entries[0]);
                    int opNum =  int.Parse(MyUtilities.clean(entries[1]));
                    if (operName.Trim().Equals(String.Empty)) {
                        throw new Exception("Cannot insert oper line. Empty operation name.");
                    }
                    if (operName.Equals("DOCK") || operName.Equals("STOCK") || operName.Equals("SCRAP")) {
                        throw new Exception("Cannot insert oper line. Attempt to use reserved operation names DOCK, STOCK, SCRAP.");
                    }

                    connec.Open();
                    try {
                        int result = cmd.ExecuteNonQuery();
                    } catch (Exception excep) {
                        Exception excepNew = new Exception("Error in executing insert query. Command string: " + cmd.CommandText + ". Exception message: " + excep.Message, excep);
                        throw excepNew;
                    }
                    
                    wasInserted = true;
                    connec.Close();

                    //UpdateSql("UPDATE tblOper SET OpNum = " + opNum + " WHERE OpNam = '" + operName + "' AND ProdFore = " + prodId + ";");
                    this.SetData();
                    //SetRoutingData();
                } catch (Exception exp) {
                    logFiles.ErrorLog(exp);
                    try {
                        connec.Close();
                        connec = null;
                    } catch { }
                }
            }
            FillDefaultInsertRow();
        }
        return wasInserted;
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

    protected bool SameOperNameNumExists(string opName, int prodId, int opNum) {
        return SameOperNameNumExists(opName, prodId, -1);
    }


    protected bool SameOperNameNumExists(string opName, int prodId, int opNum, int opId) {
        return OperDelegate.SameOperNameNumExists(opName, prodId, opNum, opId, GetDirectory() + userDir + MAIN_USER_DATABASE, logFiles);

    }

    protected bool SameOperNameDiffNumExists(string opName, int opNum, int prodId) {
        return SameOperNameDiffNumExists(opName, opNum, prodId, -1);
    }

    protected bool SameOperNameDiffNumExists(string opName, int opNum, int prodId, int opId) {
        return OperDelegate.SameOperNameDiffNumExists(opName, opNum, prodId, opId, GetDirectory() + userDir + MAIN_USER_DATABASE, logFiles);
    }

    protected bool SameOperNumDiffNameExists(string opName, int opNum, int prodId, int opId) {
        return OperDelegate.SameOperNumDiffNameExists(opName, opNum, prodId, opId, GetDirectory() + userDir + MAIN_USER_DATABASE, logFiles);
    }

    protected bool SameOperNumDiffNameExists(string opName, int opNum, int prodId) {
        return SameOperNumDiffNameExists(opName, opNum, prodId, -1);
    }

    protected string GetOperInfoMessage(string opName, int opNumber, int prodId, int opId, string oldOpName, int oldOpNum) {
        string operMessage = "";
        if ((oldOpName == null || !oldOpName.ToUpper().Equals(opName)) && SameOperNameNumExists(opName, prodId, opNumber, opId)) {
            operMessage += DbUse.OPER_SECONDARY_MSG + "<br/>";
        }
        bool routingWarning = false;
        if ((oldOpNum != opNumber || (oldOpName == null || !oldOpName.ToUpper().Equals(opName))) && SameOperNameDiffNumExists(opName, opNumber, prodId, opId)) {
            operMessage += DbUse.OPER_SAME_NAME_DIFF_NUM + "<br/>";
            routingWarning = true;
        }
        if ((oldOpNum != opNumber || (oldOpName == null || !oldOpName.ToUpper().Equals(opName))) && SameOperNumDiffNameExists(opName, opNumber, prodId, opId)) {
            operMessage += DbUse.OPER_DIFF_NAME_SAME_NUM + "<br/>";
            routingWarning = true;
        }
        if (routingWarning) {
            operMessage += DbUse.DEFAULT_ROUTING_WARNING;
        }
        if (!operMessage.Equals("")) {
            Master.ShowInfoMessage(operMessage);
        }
        return operMessage;
    }

    protected string GetOperInfoMessage(string opName, int opNumber, int prodId) {
        return GetOperInfoMessage(opName, opNumber, prodId, -1, null, Int32.MinValue);
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