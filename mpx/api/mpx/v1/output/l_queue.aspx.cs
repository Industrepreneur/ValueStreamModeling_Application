using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = "SELECT DISTINCTROW tblRsLabor.LaborID, tblLabor.LaborDesc, zstblwhatif.WID, tblRsLabor.LaborID, tblLabor.LaborDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsLabor.SetupUtil AS SetupUtil, tblRsLabor.RunUtil AS RunUtil, tblRsLabor.AbsUtil AS absutil, tblRsLabor.Idle AS Idle, ([tblrslabor].[SetupUtil]+[tblrslabor].[RunUtil]+[tblrslabor].[AbsUtil]) AS [Total Util], tblRsLabor.Qprocess, tblRsLabor.QWait, zstblwhatif.familyid, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[setupUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS SetupUtilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[RUNUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS RunUtilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrsLabor].[absUtil]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS absutilt, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*[tblrslabor].[idle]*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS Idlet, IIf([tbllabor].[grpsiz]>0,[tbllabor].[grpsiz],1)*Int(([tblrslabor].[SetupUtil]+[tblrslabor].[RunUtil]+[tblrslabor].[AbsUtil])*([tbllabor].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c])/100 AS [Total Utilt] " +
           " FROM tblgeneral INNER JOIN (zstblwhatif INNER JOIN (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) ON zstblwhatif.WID = tblRsLabor.WID) ON tblgeneral.dummylink = zstblwhatif.dummyline" +
           " WHERE (((tblLabor.LaborDesc)<>\"NONE\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";

    public static readonly int[] myArray = new int[] { 1, 5, 11, 12 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}

//Name = 1
//Scenario = 5
//Tended = 11
//Waiting = 12