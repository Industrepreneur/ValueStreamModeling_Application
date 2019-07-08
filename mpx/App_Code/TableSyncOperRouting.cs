using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TableSyncOperRouting
/// </summary>
public class TableSyncOperRouting: TableSynchronization
{
	public TableSyncOperRouting(string userDir): base(userDir)
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public override void SyncTables() {
        
        UpdateSql("UPDATE tblOper INNER JOIN tblEquip ON tblOper.EquipDesc = tblEquip.EquipDesc SET EqId = tblEquip.EquipID;");
       

    }
}