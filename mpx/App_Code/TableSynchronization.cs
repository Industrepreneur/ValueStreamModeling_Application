using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TableSynchronization
/// </summary>
public class TableSynchronization {
    protected string userDir;

    protected const string MAIN_USER_DATABASE = DbPage.MAIN_USER_DATABASE;

    public TableSynchronization(string userDir) {
        this.userDir = userDir;
    }

    public void SyncTablesOnDefaultRouting() {

    }

    protected virtual string GetDirectory() {
        return DbUse.GetMainDirectory() + "App_Data\\";
    }

    public virtual void SyncTables() {

    }

    public void SyncTablesOnDefaultRouting(int prodid) {
        UpdateEquipNames();
        UpdateOperNames();
        UpdateOpNumbers();
        string prodName = GetDatabaseField("ProdDesc", "ProdID", prodid, "tblprodfore");
        UpdateSql("UPDATE tbloperfrto SET ProdDesc = '" + prodName + "' WHERE Partfore = " + prodid + ";");


    }

    public void SyncTablesOnDefaultRoutingAll() {
        UpdateEquipNames();
        UpdateOperNames();
        using (OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE + ";")) {

            OleDbCommand cmdSelect = new OleDbCommand("SELECT ProdID, ProdDesc FROM tblprodfore;", connec);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmdSelect);
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows) {
                    try {
                        OleDbCommand cmd = new OleDbCommand("UPDATE tbloperfrto SET ProdDesc = ? WHERE PartFore = ?", connec);
                        cmd.Parameters.AddWithValue("ProdDesc", row["ProdDesc"]);
                        cmd.Parameters.AddWithValue("PartFore", row["ProdID"]);
                        cmd.ExecuteNonQuery();
                    } catch (Exception) { }

                }
                connec.Close();
            } catch (Exception) {
                try {
                    connec.Close();
                } catch { }
            }
        }
    }

    public void UpdateEquipNames() {
        UpdateSql("UPDATE tblOper INNER JOIN tblEquip ON tblOper.EqId = tblEquip.EquipID SET tblOper.EquipDesc = tblEquip.EquipDesc;");
        
    }

    public void UpdateOperNames() {
        UpdateSql("UPDATE tblOperFrTo INNER JOIN tblOper ON tblOperFrTo.OpNumF = tblOper.OpID SET tblOperFrTo.FromOpName = tblOper.OpNam;");
        UpdateSql("UPDATE tblOperFrTo INNER JOIN tblOper ON tblOperFrTo.OpNumT = tblOper.OpID SET tblOperFrTo.ToOpName = tblOper.OpNam;");
        
    }

    public void UpdateOpNumbers() {
        UpdateSql("UPDATE tblOperFrTo INNER JOIN tblOper ON tblOper.OpID = tblOperFrTo.OpNumF SET tblOperFrTo.fromnum = tblOper.OpNum;");
        UpdateSql("UPDATE tblOpFrTo_d INNER JOIN tblOper ON tblOper.OpID = tblOpFrTo_d.OpNumF SET tblOpFrTo_d.fromnum = tblOper.OpNum;");

        UpdateSql("UPDATE tblOperFrTo INNER JOIN tblOper ON tblOper.OpID = tblOperFrTo.OpNumT SET tblOperFrTo.tonum = tblOper.OpNum;");
        UpdateSql("UPDATE tblOpFrTo_d INNER JOIN tblOper ON tblOper.OpID = tblOpFrTo_d.OpNumT SET tblOpFrTo_d.tonum = tblOper.OpNum;");

    }

    public bool UpdateSql(string command) {
        return DbUse.RunSql(command, "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + GetDirectory() + userDir + MAIN_USER_DATABASE);
    }

    public string GetDatabaseField(string field, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT " + field + " FROM " + table + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string key, string keyValue, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT [" + key + "], [" + field + "] FROM " + table + " WHERE [" + key + "] = '" + keyValue + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string key1, int keyValue1, string key2, int keyValue2, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT [" + key1 + "], [" + key2 + "], [" + field + "] FROM " + table + " WHERE [" + key1 + "] = " + keyValue1 + " AND [" + key2 + "] = " + keyValue2 +";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string key, int keyValue, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT [" + key + "], [" + field + "] FROM " + table + " WHERE [" + key + "] = " + keyValue + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }
}