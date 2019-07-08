using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Data.OleDb;
using System.Data;

public partial class L_Utilization : DbPage //was LaborResultsPage
{

    public L_Utilization ()
    {

        PAGENAME = "/output/labor/utilization.aspx";

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }



}

//tblRsOper.l_run_hours, tblRsOper.l_set_hours
//FIELDS = new string[] { "LaborID", "LaborDesc", "description", "[Total Util]", "SetupUtil", "RunUtil", "absutil", "Idle"};
//HEADERS2 = new string[] { null, "Labor", "Scenario", "Total Util %", " Setup Util %", "Run Util %", "Inefficiency %", "Idle %"};
//HEADERS = new string[] { null, "Labor", "Scenario", "Total Utilization", "Setup %", "Run %", "Inefficiency %", "Idle %"};

//tableQueryString = "SELECT DISTINCTROW tblRsLabor.LaborID, tblLabor.LaborDesc, zstblwhatif.WID, tblRsLabor.LaborID, tblLabor.LaborDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsLabor.SetupUtil AS SetupUtil, tblRsLabor.RunUtil AS RunUtil, tblRsLabor.AbsUtil AS absutil, tblRsLabor.Idle AS Idle, ([tblrslabor].[SetupUtil]+[tblrslabor].[RunUtil]+[tblrslabor].[AbsUtil]) AS [Total Util], zstblwhatif.familyid, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[setupUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS SetupUtilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[RUNUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS RunUtilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[absUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS absutilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrslabor].[idle]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS Idlet, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*Int(([tblrslabor].[SetupUtil]+[tblrslabor].[RunUtil]+[tblrslabor].[AbsUtil])*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c])/100 AS [Total Utilt], tblRsLabor.WID " +
//    " FROM tblgeneral INNER JOIN (zstblwhatif INNER JOIN (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) ON zstblwhatif.WID = tblRsLabor.WID) ON tblgeneral.dummylink = zstblwhatif.dummyline" +
//    " WHERE (((tblLabor.LaborDesc)<>\"NONE\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";

//FIELD_OFFSETS = new int[] { 3, 3 };

