using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class routings : InputGridPage {

    ProductDelegatePage helperProduct;

    protected string TABLE_NAME_ROUTING
    {
        get
        {
            if (ViewState["TABLE_NAME_ROUTING"] == null)
            {
                ViewState["TABLE_NAME_ROUTING"] = "";
            }
            return (string)ViewState["TABLE_NAME_ROUTING"];
        }
        set { ViewState["TABLE_NAME_ROUTING"] = value; }
    }

    public routings() {
        PAGENAME = "/input/products/routings.aspx";
        featureHelper = new RoutingTableDelegate();

        int value = 3;
        helperProduct = new ProductDelegatePage(value);

        

        wantCopyButton = false;
    }



    protected void Page_Load(object sender, EventArgs e) {
        TABLE_NAME_ROUTING = "tblOperFrTo";
        if (!Page.IsPostBack) {
            tableSync.UpdateOpNumbers();
        }
        base.Page_Load(sender, e);
        //HideCopyButton();
        //ManageFooterRow();
        //ManageEditRow();
        
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        //pnlMenu.Controls.Add(new LiteralControl("<h2>Products</h2>"));
        base.OnInit(e);
        if (IsWhatifMode()) {
            Response.Redirect("whatif_" + PAGENAME);
        }
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        srcProductsList.DataFile = dataFile;
        dropListProducts.DataBind();
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        tableSync = new TableSynchronization(userDir);

        string sheet = "Cheat Sheat Product Operations and Routings Page";
        Master.SetHelpSheet(sheet + ".pdf", sheet);

    }

    protected override string GetCommandString(int commandType, string[] selectedFields)
    {
        string commandString = base.GetCommandString(commandType, selectedFields);
        string selectedProduct = (dropListProducts.SelectedItem == null) ? "''" : dropListProducts.SelectedValue;
        return (featureHelper as RoutingTableDelegate).GetCommandString(commandType, commandString, selectedProduct);
    }

    protected override void SyncTables() {
        tableSync.UpdateOpNumbers();
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

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e)
    {
        string value = dropListProducts.SelectedValue;
        this.SetData();
    }

    //protected void btnShowTimes_Click(object sender, EventArgs e) {
    //    if (btnShowTimes.Text.Equals("Show Real Percentage")) {
    //        btnDefaultRoutingPart.Enabled = false;
    //        grid.Columns[0].Visible = false;
    //        if (grid.FooterRow != null) {
    //            grid.FooterRow.Visible = false;
    //        }
    //        grid.ShowFooter = false;
    //        thirdPanel.Visible = false;
    //        this.TABLE_NAME = "tblOpFrTo_d";
    //        btnShowTimes.Text = "Edit Routings";
    //        {
    //            ClassF calc = new ClassF(GetDirectory() + userDir);
    //            try {
    //                calc = new ClassF(GetDirectory() + userDir);
    //                calc.setGlobalVar();
    //                calc.runsqlado("DELETE * FROM tblOper_d;");
    //                calc.runsqlado("DELETE * FROM zstblerrors;");
    //                calc.inOperRoutingPages = true;
    //                calc.MakeActualroute_all();
    //                string errorMsg = calc.GetErrorMessage();
    //                lblErrorRealTimes.Text = "Errors in calculating expressions in routing table:<br/>" + errorMsg;
    //                lblErrorRealTimes.Visible = !errorMsg.Trim().Equals(String.Empty);
    //            } catch (Exception ex) {
    //                logFiles.ErrorLog(ex);
    //                Master.ShowErrorMessage("An error has occured while calculating expressions.");
    //            } finally {
    //                calc.Close();
    //            }
    //        }
    //        SetTableStrings();
    //    } else {
    //        btnDefaultRoutingPart.Enabled = true;
    //        grid.Columns[0].Visible = true;
    //        if (grid.FooterRow != null) {
    //            grid.FooterRow.Visible = true;
    //        }
    //        grid.ShowFooter = true;
    //        thirdPanel.Visible = true;
    //        this.TABLE_NAME = "tblOperFrTo";
    //        btnShowTimes.Text = "Show Real Percentage";
    //        SetTableStrings();
    //    }

    //    grid.PageIndex = 0;
    //    this.SetData();
        

    //}

    protected override void btnInsertTable_Click(object sender, EventArgs e) {
        string fields = txtInsertTable.Text;
        txtInsertTable.Text = "";
        string[] lines = fields.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++) {
            string[] entries = lines[i].Split(new Char[] { '\t', ';', ',' });
            if (!InsertRoutingLine(entries)) {
                txtInsertTable.Text += lines[i] + "\n";
            } else {
                Master.SetModelModified(true);
            }
        }
        if (!txtInsertTable.Text.Equals("")) {
            extenderInfo.Show();
        }
    }

    protected override List<string> GetDropList(string name) {
        List<string> dropList = new List<string>();
        string comm = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        if (name.Equals("ProdDesc")) {
            comm = "SELECT ProdDesc, ProdId FROM tblprodfore";
        } else if (name.Equals("opnam1") || name.Equals("opnam2")) {
            comm = "SELECT DISTINCT OpNam FROM tbloper ORDER BY OpNam";
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

    protected void btnDefaultRoutingPart_Click(object sneder, EventArgs e)
    {
        
        ClassF classF = new ClassF(GetDirectory() + userDir);
        try
        {
            classF.addfromto_part(int.Parse(dropListProducts.SelectedValue));
            tableSync.SyncTablesOnDefaultRouting(int.Parse(dropListProducts.SelectedValue));
        }
        catch (Exception) { }
        classF.Close();
        SetData();
        
    }

    //protected override void PageIndexChanging(object sender, GridViewPageEventArgs e) {
    //    if (btnShowTimes.Text.Equals("Show Real Percentage")) {
    //        TABLE_NAME = "tblOperFrTo";
    //        SetTableStrings();
    //    } else {
    //        TABLE_NAME = "tblOpFrTo_d";
    //        SetTableStrings();
    //    }
    //    base.PageIndexChanging(sender, e);
    //}

    protected override void RowUpdate(int rowIndex)
    {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);

        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand("UPDATE tblOperFrTo SET OpNumF = ?, FromOpName = ?, OpNumT = ?, ToOpName = ?, Per = ? WHERE RecID = ?;", connec);
   
        OleDbCommand comm = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + newValues["opnam2"] + "';", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(comm);

        {
            try
            {
                connec.Open();
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                cmd.CommandType = CommandType.Text;
            
                    OleDbCommand comm2 = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + dropListProducts.SelectedValue + " AND OpNam = '" + newValues["opnam1"] + "';", connec);
                    OleDbDataAdapter adapter2 = new OleDbDataAdapter(comm2);
                    DataTable dt = new DataTable();
                    adapter2.Fill(dt);
                    cmd.Parameters.AddWithValue("OpNumF", dt.Rows[0]["OpID"]);
                    cmd.Parameters.AddWithValue("FromOpName", newValues["opnam1"]);
                

                cmd.Parameters.AddWithValue("OpNumT", ds.Rows[0]["OpID"]);
                cmd.Parameters.AddWithValue("ToOpName", newValues["opnam2"]);
                cmd.Parameters.AddWithValue("Per", newValues["Per"]);
                cmd.Parameters.AddWithValue("RecID", grid.DataKeys[row.RowIndex]["RecID"]);

                int result = cmd.ExecuteNonQuery();
                grid.EditIndex = -1;
                this.SetData();
                tableSync.UpdateOpNumbers();
                connec.Close();
                SetModelModified(true, false);
            }
            catch
            {
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch
                {
                    Master.ShowErrorMessage("An error has occured and the data could not get updated.");
                }
            }
            
        }
        
    }
        



    //protected override void RowUpdate(int rowIndex) {
    //    GridViewRow row = grid.Rows[rowIndex];

    //    var newValues = this.GetValues(row);

    //    connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
    //    OleDbCommand cmd = new OleDbCommand("UPDATE tblOperFrTo SET ProdDesc=?, fromopname = ?, ToOpName = ?, Per = ? WHERE RecID = ?;", connec);


    //    {
    //        cmd.CommandType = CommandType.Text;
    //        cmd.Parameters.AddWithValue("ProdDesc", MyUtilities.clean(dropListProducts.SelectedItem.Text));
    //        cmd.Parameters.AddWithValue("FromOpName", MyUtilities.clean(newValues[FIELDS[1]].ToString()));
    //        cmd.Parameters.AddWithValue("ToOpName", MyUtilities.clean(newValues[FIELDS[2]].ToString()));
    //        cmd.Parameters.AddWithValue("Per", MyUtilities.clean(newValues[FIELDS[3]].ToString()));
    //        cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
    //        try {
    //            connec.Open();
    //            int result = cmd.ExecuteNonQuery();

    //            grid.EditIndex = -1;

    //            this.SetData();
    //            connec.Close();

    //        } catch (Exception ex) {
    //            logFiles.ErrorLog(ex);
    //            try {
    //                connec.Close();
    //                connec = null;
    //            } catch { }
    //            Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
    //        }
    //    }
    //}

    protected override void InsertRow(bool goToEdit) {
        GridViewRow row = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];

        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = row.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }

        }
        string[] entries = new string[3];
        for (int i = 1; i < 4; i++) {
            string value;
            if (COMBOS[i]) {
                value = ((AjaxControlToolkit.ComboBox)txtControls[i]).Text;
            } else {
                value = ((TextBox)txtControls[i]).Text;
            }
            entries[i - 1] = value;
        }
        if (!InsertRoutingLine(entries)) {
            Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
        }
    }

    protected bool InsertRoutingLine(string[] entries) {
        bool wasInserted = false;
        for (int i=0; i< entries.Length; i++) {
            entries[i] = MyUtilities.clean(entries[i]);
            if (entries[i].Trim().Length > 0) {
                entries[i] = entries[i].Trim();
            }
        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = "INSERT into tblOperFrTo (PartFore, OpNumF, OpNumT, Per, fromopname, ToOpName, ProdDesc) VALUES ( ?, ?, ?, ?, ?, ?, ?);";
        OleDbCommand cmd = new OleDbCommand(command, connec);
        OleDbCommand comm2 = new OleDbCommand("SELECT ProdID FROM tblprodfore WHERE ProdDesc = '" + dropListProducts.SelectedItem.Text + "';", connec);
        OleDbDataAdapter adapter2 = new OleDbDataAdapter(comm2);
        {

            try {
                connec.Open();
                DataTable ds = new DataTable();
                DataTable ds2 = new DataTable();
                DataTable ds3 = new DataTable();

                adapter2.Fill(ds2);
                OleDbCommand comm = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + ds2.Rows[0]["ProdID"] + " AND OpNam = '" + entries[0] + "';", connec);
                OleDbDataAdapter adapter = new OleDbDataAdapter(comm);
                adapter.Fill(ds);

                OleDbCommand comm3 = new OleDbCommand("SELECT OpID FROM tbloper WHERE ProdFore = " + ds2.Rows[0]["ProdID"] + " AND OpNam = '" + entries[1] + "';", connec);
                OleDbDataAdapter adapter3 = new OleDbDataAdapter(comm3);
                adapter3.Fill(ds3);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("PartFore", ds2.Rows[0]["ProdID"]);
                cmd.Parameters.AddWithValue("OpNumF", ds.Rows[0]["OpID"]);
                cmd.Parameters.AddWithValue("OpNumT", ds3.Rows[0]["OpID"]);
                cmd.Parameters.AddWithValue("Per", entries[2]);
                cmd.Parameters.AddWithValue("fromopname", entries[0].ToUpper());
                cmd.Parameters.AddWithValue("ToOpName", entries[1].ToUpper());
                cmd.Parameters.AddWithValue("ProdDesc", dropListProducts.SelectedItem.Text);

                int result = cmd.ExecuteNonQuery();
                grid.EditIndex = -1;

                this.SetData();
                wasInserted = true;
                connec.Close();
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                SaveInsertValues(grid.FooterRow, TEXT_BOX_IDS);
            }
        }
        return wasInserted;
    }

    protected void SetTableStrings() {
        tableQueryString = "SELECT " + TABLE_NAME + ".RecID,tblProdFore.ProdDesc, tblOper.OpNam AS opnam1, tblOper_1.OpNam AS opnam2, Per, fromnum, tonum " +
                    " FROM " + TABLE_NAME + " INNER JOIN ((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblOper AS tblOper_1 ON tblProdFore.ProdID = tblOper_1.ProdFore) ON (" + TABLE_NAME + ".OpNumT = tblOper_1.OpID) AND (" + TABLE_NAME + ".OpNumF = tblOper.OpID)";
                     
                    //+ " WHERE (((tblOper.OpNam)<>\"stock\" And (tblOper.OpNam)<>\"scrap\") AND ((tblOper_1.OpNam)<>\"dock\"))";
        //defaultSortString = " ORDER BY " + TABLE_NAME + ".ProdDesc";
        //defaultSortString = " ORDER BY tblProdfore.ProdDesc";
        
    }

    protected override void SetData() {
        SetTableStrings();
        tableSync.UpdateOpNumbers();
        base.SetData();
        if (grid.FooterRow != null) {
            AjaxControlToolkit.ComboBox comboProduct = grid.FooterRow.FindControl("comboEdit1") as AjaxControlToolkit.ComboBox;
            AjaxControlToolkit.ComboBox comboOper = grid.FooterRow.FindControl("comboEdit2") as AjaxControlToolkit.ComboBox;
            if (comboProduct != null && comboProduct.Items.Count == 0) {
                thirdPanel.Visible = false;
                Master.ShowErrorMessage("No products are defined yet. Please go to the input products page and create a product first.");
            } else if (comboOper != null && comboOper.Items.Count == 0) {
                Master.ShowErrorMessage("No operations are defined yet. Please go to the input operations page and create some operation.");
                thirdPanel.Visible = false;

            }
        }
    }

    protected void ManageEditRow() {
        if (grid.EditIndex != -1) {
            GridViewRow editRow = grid.Rows[grid.EditIndex];
            AjaxControlToolkit.ComboBox comboProduct = editRow.FindControl(TEXT_BOX_IDS[1]) as AjaxControlToolkit.ComboBox;
            AjaxControlToolkit.ComboBox comboFrom = editRow.FindControl(TEXT_BOX_IDS[2]) as AjaxControlToolkit.ComboBox;
            AjaxControlToolkit.ComboBox comboTo = editRow.FindControl(TEXT_BOX_IDS[3]) as AjaxControlToolkit.ComboBox;

            int productId;
            try {
                if (comboProduct != null) {
                    string id = GetDatabaseField("ProdID", "ProdDesc", comboProduct.SelectedValue, "tblprodfore");
                    productId = int.Parse(id);
                    List<string> operList = GetOperList("opnam1", productId);
                    if (comboFrom != null) {
                        FillComboOper(comboFrom, operList);
                    }
                    if (comboTo != null) {
                        FillComboOper(comboTo, operList);
                    }
                }
            } catch (Exception) { }

        }
    }

    protected void ManageFooterRow() {
        if (grid.FooterRow != null && grid.FooterRow.Cells.Count > 4) {
            //grid.FooterRow.
            //GridViewRow footerRow = grid.FooterRow;
            //TextBox opFr = footerRow.Controls[5] as TextBox;
            //opFr.Enabled = false;
            //TextBox opTo = footerRow.Controls[6] as TextBox;
            //opTo.Enabled = false;
            //AjaxControlToolkit.ComboBox comboProduct = footerRow.FindControl(TEXT_BOX_IDS[1]) as AjaxControlToolkit.ComboBox;
            //AjaxControlToolkit.ComboBox comboFrom = footerRow.FindControl(TEXT_BOX_IDS[2]) as AjaxControlToolkit.ComboBox;
            //AjaxControlToolkit.ComboBox comboTo = footerRow.FindControl(TEXT_BOX_IDS[3]) as AjaxControlToolkit.ComboBox;
            //if (dropListProducts != null) {
            //    if (dropListProducts.SelectedValue == null || dropListProducts.SelectedValue.Equals("")) {
            //        // disable all other combos and add button
            //        Button btnAdd = grid.FooterRow.FindControl(GridViewTemplate.BTN_INSERT) as Button;
            //        if (btnAdd != null) {
            //            btnAdd.Enabled = false;
            //            btnAdd.ToolTip = "Select a product first.";
            //        }
            //        if (comboFrom != null) {
            //            comboFrom.Enabled = false;
            //            comboFrom.ToolTip = "Select a product first.";
            //        }
            //        if (comboTo != null) {
            //            comboTo.Enabled = false;
            //            comboTo.ToolTip = "Select a product first.";
            //        }
            //    } else {
            //        int productId;
            //        try {
            //            string id = GetDatabaseField("ProdID", "ProdDesc", comboProduct.SelectedValue, "tblprodfore");
            //            productId = int.Parse(id);
            //            List<string> operList = GetOperList("opnam1", productId);
            //            if (comboFrom != null) {
            //                comboFrom.Enabled = true;
            //                FillComboOper(comboFrom, operList);
            //                comboFrom.ToolTip = "";
            //            }
            //            if (comboTo != null) {
            //                comboTo.Enabled = true;
            //                FillComboOper(comboTo, operList);
            //                comboTo.ToolTip = "";
            //            }
            //            Button btnAdd = grid.FooterRow.FindControl(GridViewTemplate.BTN_INSERT) as Button;
            //            if (btnAdd != null) {
            //                btnAdd.Enabled = true;
            //                btnAdd.ToolTip = "";
            //            }

            //        } catch (Exception) { }
            //    }
            //}
        }
    }

    protected List<string> GetOperList(string desc, int productId) {
        List<string> dropList = new List<string>();
        string comm = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        if (desc.Equals("ProdDesc")) {
            comm = "SELECT ProdDesc, ProdId FROM tblprodfore";
        } else if (desc.Equals("opnam1") || desc.Equals("opnam2")) {
            comm = "SELECT OpNam FROM tbloper WHERE Prodfore = " + productId + ";";
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

    protected void FillComboOper(AjaxControlToolkit.ComboBox combo, List<string> opers) {
        string selectedValue = null;
        if (combo.SelectedValue != null) {
            selectedValue = combo.SelectedValue;
        }
        combo.Items.Clear();
        for (int i = 0; i < opers.Count; i++) {
            combo.Items.Add(opers[i]);
        }
        if (selectedValue != null) {
            if (combo.Items.FindByValue(selectedValue) != null) {
                combo.SelectedValue = selectedValue;
            }

        }
    }
}