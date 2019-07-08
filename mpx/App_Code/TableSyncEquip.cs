using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TableSyncEquip
/// </summary>
public class TableSyncEquip: TableSynchronization
{
	public TableSyncEquip(string userDir): base(userDir)
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public override void SyncTables() {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE + ";");

        OleDbCommand cmdSelect = new OleDbCommand("SELECT EquipId, EquipDesc FROM tblequip;", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdSelect);
        try {
            connec.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows) {
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tbloper SET EquipDesc = ? WHERE EqId = ?", connec);
                    cmd.Parameters.AddWithValue("EquipDesc", row["EquipDesc"]);
                    cmd.Parameters.AddWithValue("EqId", row["EquipId"]);
                    cmd.ExecuteNonQuery();
                } catch (Exception) { }

            }
            connec.Close();
        } catch (Exception) {
            try {
                connec.Close();
            } catch { }
        }

        cmdSelect = new OleDbCommand("SELECT LaborId, LaborDesc FROM tbllabor;", connec);
        adapter = new OleDbDataAdapter(cmdSelect);
        try {
            connec.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows) {
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tblequip SET Labor = ? WHERE LaborDesc = ?", connec);
                    cmd.Parameters.AddWithValue("Labor", row["LaborId"]);
                    cmd.Parameters.AddWithValue("LaborDesc", row["LaborDesc"]);
                    cmd.ExecuteNonQuery();
                } catch (Exception) { }

            }
            connec.Close();
        } catch (Exception) {
            try {
                connec.Close();
                connec = null;
            } catch { }
        }
        UpdateSql("UPDATE tblequip SET EquipType = 0 WHERE EquipTypeName = 'Standard';");
        UpdateSql("UPDATE tblequip SET EquipType = 1 WHERE equiptypename = 'Delay' ;");
    }
}