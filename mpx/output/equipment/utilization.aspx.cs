using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Web.Services;

public partial class E_Utilization : DbPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }

    public E_Utilization () {

          PAGENAME = "/output/equipment/utilization.aspx";
       
    }

}

//string[] FIELDS = new string[] { "EquipID", "EquipDesc", "description", "Total", "SetupUtil", "RunUtil", "LabWaitUtil", "RepUtil", "Idle", "LaborDesc"};
// string[] HEADERS = new string[] { null, "Name", "Scenario", "Total Util %", "Setup %", "Run %", "Wait for Labor %", "Repair %", "Idle %", "Assigned Labor" };
// const string tableQueryString = "SELECT DISTINCTROW tblEquip.EquipDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblLabor.LaborDesc, SetupUtil, RunUtil, LabWaitUtil, RepUtil, Idle, ([Setuputil]+[runUtil]+[repUtil]+[labWaitUtil]) AS Total, tblRsEquip.EquipID, zstblwhatif.familyid, tblRsEquip.WID" +
//    " FROM ((tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID" +
//    " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\") AND ((tblEquip.EquipDesc)<>\"None\"))";// +


//const string graphQueryString = "SELECT DISTINCTROW tblEquip.EquipDesc,  tblRsEquip.SetupUtil AS [Setup Utilization], tblRsEquip.RunUtil AS [Run Utilization], tblRsEquip.LabWaitUtil AS [Wait For Labor], tblRsEquip.RepUtil AS [Repair Utilization], IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsEquip.WID" +
//    " FROM (tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID" +
//    " WHERE (((tblEquip.EquipDesc)<>\"None\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";// +