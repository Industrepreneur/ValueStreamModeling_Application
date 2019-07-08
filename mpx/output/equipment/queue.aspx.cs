using System;

public partial class E_Queue : DbPage
{
  
    public E_Queue() {
       PAGENAME = "/output/equipment/queue.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }


}

//string[] FIELDS = new string[] { "EquipID", "EquipDesc", "description", "QProcess", "QWait", "QTotal" };
// string[] HEADERS = new string[] { null, "Equipment Group Name", "What-If Scenario", "Pieces in Process", "Pieces Waiting", "Total WIP" };
//const string graphQueryString = "SELECT DISTINCTROW tblEquip.EquipDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsEquip.Qprocess, tblRsEquip.QWait, tblRsEquip.Qtotal, tblRsEquip.EquipID, zstblwhatif.familyid" +
//              " FROM (((tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID) INNER JOIN tblgeneral ON zstblwhatif.dummyline = tblgeneral.dummylink" +
//              " WHERE (((tblEquip.EquipDesc)<>\"None\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";
//int[] FIELD_OFFSETS = new int[] { 2, 2 };