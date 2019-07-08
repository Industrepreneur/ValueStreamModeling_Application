using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TableSyncLabor
/// </summary>
public class TableSyncLabor: TableSynchronization
{
	public TableSyncLabor(string userDir): base(userDir)
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public override void SyncTables() {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE + ";");

        OleDbCommand cmdSelect = new OleDbCommand("SELECT LaborId, LaborDesc FROM tbllabor;", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdSelect);
        try {
            connec.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows) {
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tblequip SET LaborDesc = ? WHERE Labor = ?", connec);
                    cmd.Parameters.AddWithValue("LaborDesc", row["LaborDesc"]);
                    cmd.Parameters.AddWithValue("Labor", row["LaborId"]);
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
    }
}