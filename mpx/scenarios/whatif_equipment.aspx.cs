using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_equipment : WhatifGridPage {

    public whatif_equipment() {

        PAGENAME = "whatif_equipment.aspx";
        
        featureHelper = new EquipmentDelegate();

        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;

        
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);

    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        return featureHelper.GetCommandString(commandType, commandString);
    }




    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        base.OnInit(e);
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        tableSync = new TableSyncEquip(userDir);
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



    protected override List<string> GetDropList(string name) {
        List<string> dropList = new List<string>();
        string comm = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        if (name.Equals("LaborDesc")) {
            comm = "SELECT LaborDesc, LaborId FROM tbllabor ORDER BY LaborDesc;";
        } else if (name.Equals("EquipTypeName")) {
            comm = "SELECT EquipTypeName, TypeId FROM tblEquipType";
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


}