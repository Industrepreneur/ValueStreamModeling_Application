using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_products_oper : WhatifGridPage {
    ProductDelegatePage helperProduct;

    public whatif_products_oper() {
        PAGENAME = "whatif_products_oper.aspx";
        featureHelper = new OperDelegate();

        int value = 2;
        helperProduct = new ProductDelegatePage(value);

        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;
        fieldsNonEditable[3] = true;
        fieldsNonEditable[4] = true;

    }

    protected void Page_Load(object sender, EventArgs e) {
        if (!Page.IsPostBack) {
            TABLE_NAME = "tbloper";
            try {
                SetupDefRoutingWarning();
            } catch (Exception) { }
        }
        base.Page_Load(sender, e);
        grid.Columns[2].Visible = false;
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }



    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        pnlMenu.Controls.Add(new LiteralControl("<h2>What-If:Products</h2>"));
        base.OnInit(e);
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        tableSync = new TableSyncOperRouting(userDir);

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

    protected void btnShowTimes_Click(object sender, EventArgs e) {
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
                    lblErrorRealTimes.Text = "Errors in calculating expressions in operation table:<br/>" + errorMsg;
                    lblErrorRealTimes.Visible = !errorMsg.Trim().Equals(String.Empty);
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
        grid.PageIndex = 0;
        this.SetData();

    }

    protected void SetTableQueryString() {
        if (TABLE_NAME.ToLower().Equals("tbloper")) {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID FROM " + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdFore.ProdID ";
        } else {
            tableQueryString = "SELECT " + TABLE_NAME + ".*, tblProdFore.ProdID, tblProdFore.ProdDesc, tblEquip.EquipId, tblEquip.EquipDesc FROM (" + TABLE_NAME + " INNER JOIN tblProdFore ON " + TABLE_NAME + ".ProdFore = tblProdfore.ProdID) INNER JOIN tblEquip ON " + TABLE_NAME + ".EqId = tblEquip.EquipID ";
        }
    }

    protected override void PageIndexChanging(object sender, GridViewPageEventArgs e) {
        base.PageIndexChanging(sender, e);
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
            comm = "SELECT EquipDesc, EquipId FROM tblequip ORDER BY EquipDesc";
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

    protected override void RowUpdate(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];

        var newValues = this.GetValues(row);



        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            try {
                connec.Open();
                cmd.CommandType = CommandType.Text;
                for (int i = 1; i < FIELDS.Length; i++) {
                    if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                        if (i != 2) {
                            if (CHECKBOXES[i]) {
                                object bla = newValues[FIELDS[i]];
                                cmd.Parameters.AddWithValue(FIELDS[i], ((bool)newValues[FIELDS[i]]) ? "1" : "0");
                            } else {
                                cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(newValues[FIELDS[i]].ToString()));
                            }
                        }
                    }
                }
                cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);

                int result = cmd.ExecuteNonQuery();

                grid.EditIndex = -1;
                connec.Close();

                this.SetData();

                if (mode.Equals("Standard")) {
                    HideAdvancedColumns();
                }
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }



    }

    protected override string GetCommandString(int commandType) {
        if ((commandType == Command.UPDATE || commandType == Command.INSERT)) {
            List<string> selectedFields = new List<string>();
            for (int i = 0; i < FIELDS.Length; i++) {
                if (i != 2 || commandType == Command.INSERT) {
                    if (!ADVANCED_FIELDS[i] || mode.Equals("Advanced")) {
                        selectedFields.Add(FIELDS[i]);
                    }
                }

            }
            return GetCommandString(commandType, selectedFields.ToArray<string>());

        }
        return GetCommandString(commandType, FIELDS);
    }


    protected override void SetData() {
        SetTableQueryString();
        base.SetData();

    }



    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        if (commandType == Command.SELECT) {
            int orderIndex = commandString.ToLower().IndexOf(" order by");
            string order = ";";
            if (orderIndex > -1) {
                order = commandString.Substring(orderIndex);
                commandString = commandString.Substring(0, commandString.ToLower().IndexOf(" order by"));
            }
            commandString += " WHERE (OpNam <> 'DOCK' AND OpNam <> 'STOCK' AND Opnam <> 'SCRAP') " + order;
        }
        return commandString;
    }

    protected void SetupDefRoutingWarning() {
        string userDatabase = GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE;
        if (OperDelegate.DuplicateOperNamesOrNumbers(userDatabase)) {
            lblWarnDefRouting.Text = DbUse.DUPLICATE_OPER_WARNING;
            lblWarnDefRouting.Visible = true;
        } else {
            lblWarnDefRouting.Text = "";
            lblWarnDefRouting.Visible = false;

        }
    }

}