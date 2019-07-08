using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TableSyncProduct
/// </summary>
public class TableSyncProduct: TableSynchronization
{
	public TableSyncProduct(string userDir): base(userDir)
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public override void SyncTables() {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");

        OleDbCommand cmdSelect = new OleDbCommand("SELECT ProdId, ProdDesc FROM tblprodfore;", connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmdSelect);
        try {
            connec.Open();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows) {
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tbloper SET ProdDesc = ? WHERE ProdFore = ?", connec);
                    cmd.Parameters.AddWithValue("ProdDesc", row["ProdDesc"]);
                    cmd.Parameters.AddWithValue("ProdFore", row["ProdId"]);
                    cmd.ExecuteNonQuery();
                } catch (Exception) { }
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tblOperFrTo SET ProdDesc = ? WHERE PartFore = ?", connec);
                    cmd.Parameters.AddWithValue("ProdDesc", row["ProdDesc"]);
                    cmd.Parameters.AddWithValue("PartFore", row["ProdId"]);
                    cmd.ExecuteNonQuery();
                } catch (Exception) { }
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tblibom SET compName = ? WHERE CompID = ?", connec);
                    cmd.Parameters.AddWithValue("compName", row["ProdDesc"]);
                    cmd.Parameters.AddWithValue("CompID", row["ProdId"]);
                    cmd.ExecuteNonQuery();
                } catch (Exception) { }
                try {
                    OleDbCommand cmd = new OleDbCommand("UPDATE tblibom SET ParentName = ? WHERE ParentID = ?", connec);
                    cmd.Parameters.AddWithValue("ParentName", row["ProdDesc"]);
                    cmd.Parameters.AddWithValue("ParentID", row["ProdId"]);
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