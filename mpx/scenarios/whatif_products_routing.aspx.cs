using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_products_routing : WhatifGridPage
{
    ProductDelegatePage helperProduct;

    public whatif_products_routing() {
        PAGENAME = "whatif_products_routing.aspx";
        featureHelper = new RoutingDelegate();

        int value = 3;
        helperProduct = new ProductDelegatePage(value);

        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;
        fieldsNonEditable[2] = true;
        fieldsNonEditable[3] = true;
    }

    protected void Page_Load(object sender, EventArgs e) {
        if (!Page.IsPostBack) {
            tableSync.UpdateOpNumbers();
        }
        base.Page_Load(sender, e);

    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }



    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        pnlMenu.Controls.Add(new LiteralControl("<h2>What-If: Products</h2>"));
        base.OnInit(e);
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        tableSync = new TableSynchronization(userDir);

    }

    protected override void SetData() {
        SetTableStrings();
        base.SetData();
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
        if (btnShowTimes.Text.Equals("Show Real Percentage")) {
            grid.Columns[0].Visible = false;
            this.TABLE_NAME = "tblOpFrTo_d";
            btnShowTimes.Text = "Edit Routings";
            {
                ClassF calc = new ClassF(GetDirectory() + userDir);
                try {
                    calc = new ClassF(GetDirectory() + userDir);
                    calc.setGlobalVar();
                    calc.runsqlado("DELETE * FROM tblOper_d;");
                    calc.runsqlado("DELETE * FROM zstblerrors;");
                    calc.inOperRoutingPages = true;
                    calc.MakeActualroute_all();
                    string errorMsg = calc.GetErrorMessage();
                    lblErrorRealTimes.Text = "Errors in calculating expressions in routing table:<br/>" + errorMsg;
                    lblErrorRealTimes.Visible = !errorMsg.Trim().Equals(String.Empty);
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                    Master.ShowErrorMessage("An error has occured while calculating expressions.");
                } finally {
                    calc.Close();
                }
            }
            SetTableStrings();
        } else {
            grid.Columns[0].Visible = true;            
            this.TABLE_NAME = "tblOperFrTo";
            btnShowTimes.Text = "Show Real Percentage";
        }

        grid.PageIndex = 0;
        this.SetData();

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

    protected override void PageIndexChanging(object sender, GridViewPageEventArgs e) {
        if (btnShowTimes.Text.Equals("Show Real Percentage")) {
            TABLE_NAME = "tblOperFrTo";
            SetTableStrings();
        } else {
            TABLE_NAME = "tblOpFrTo_d";
            SetTableStrings();
        }
        base.PageIndexChanging(sender, e);
    }

    protected override void RowUpdate(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];

        var newValues = this.GetValues(row);

        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand("UPDATE tblOperFrTo SET ProdDesc=?, fromopname = ?, ToOpName = ?, Per = ? WHERE RecID = ?;", connec);


        {
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("ProdDesc", MyUtilities.clean(newValues[FIELDS[1]].ToString()));
            cmd.Parameters.AddWithValue("FromOpName", MyUtilities.clean(newValues[FIELDS[2]].ToString()));
            cmd.Parameters.AddWithValue("ToOpName", MyUtilities.clean(newValues[FIELDS[3]].ToString()));
            cmd.Parameters.AddWithValue("Per", MyUtilities.clean(newValues[FIELDS[4]].ToString()));
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();

                grid.EditIndex = -1;

                this.SetData();
                connec.Close();

            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }
    

    protected void SetTableStrings() {
        tableQueryString = "SELECT " + TABLE_NAME + ".RecID, tblProdFore.ProdDesc, tblOper.OpNam AS opnam1, tblOper_1.OpNam AS opnam2, " + TABLE_NAME + ".Per, " + TABLE_NAME + ".fromnum, " + TABLE_NAME + ".tonum" +
                    " FROM " + TABLE_NAME + " INNER JOIN ((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblOper AS tblOper_1 ON tblProdFore.ProdID = tblOper_1.ProdFore) ON (" + TABLE_NAME + ".OpNumT = tblOper_1.OpID) AND (" + TABLE_NAME + ".OpNumF = tblOper.OpID)"; // +
                    //" WHERE (((tblOper.OpNam)<>\"stock\" And (tblOper.OpNam)<>\"scrap\") AND ((tblOper_1.OpNam)<>\"dock\"))";
        defaultSortString = " ORDER BY " + TABLE_NAME + ".ProdDesc";
    }

}