using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class equipment : InputGridPage {

    public equipment() {

        PAGENAME = "/input/equipment/table.aspx";

        featureHelper = new EquipmentDelegate();

        

    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);

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
        if (IsWhatifMode()) {
            Response.Redirect("whatif_equipment.aspx");
        }
        tableSync = new TableSyncEquip(userDir);
        string sheet = "Cheat Sheat Equipment Input Page";
        Master.SetHelpSheet(sheet + ".pdf", sheet);
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

    

    protected override void SetData() {
        base.SetData();
        GridViewRow row = grid.FooterRow;
        if (row != null) {
            AjaxControlToolkit.ComboBox comboDesc = row.FindControl("comboEdit8") as AjaxControlToolkit.ComboBox;
            if (comboDesc != null) {
                comboDesc.SelectedValue = "NONE";
                comboDesc.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
            }
            AjaxControlToolkit.ComboBox comboEquipType = row.FindControl("comboEdit3") as AjaxControlToolkit.ComboBox;
            if (comboEquipType != null) {
                comboEquipType.SelectedValue = "Standard";
                comboEquipType.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
            }
        }
    }

    protected override bool InsertRecord(string[] entries) {
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
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
                    if (FIELDS[i].ToLower().Equals("labordesc")) {
                        try {
                            string laborId = GetDatabaseField("LaborID", "LaborDesc", value, "tbllabor");
                            
                        } catch (Exception) {
                            // exception means invalid labor data 
                            return false;
                        }
                    }
                    if (CHECKBOXES[i]) {
                        value = value.ToLower().Equals("true") ? "1" : "0";
                    }
                    if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam")) {
                        value = value.ToUpper();
                    } else if (FIELDS[i].Equals("EquipTypeName")) {
                        if (value.ToLower().Equals("delay")) {
                            value = "Delay";
                        } else {
                            value = "Standard";
                        }
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    i++;
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                wasInserted = true;
                connec.Close();
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        FillDefaultInsertRow();
        UpdateSql("UPDATE tblEquip SET EquipType = 0, EquipTypeName = 'Standard' WHERE GrpSiz > 0;");
        UpdateSql("UPDATE tblEquip SET EquipType = 1, EquipTypeName = 'Delay' WHERE GrpSiz = -1;");
        return wasInserted;
    }


}