
using System.Collections.Generic;

using System.Web.Services;


public partial class API : System.Web.UI.Page
{

    private static readonly string myQuery = "SELECT DISTINCTROW tblRsLabor.LaborID, tblLabor.LaborDesc, zstblwhatif.WID, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsLabor.SetupUtil AS SetupUtil, tblRsLabor.RunUtil AS RunUtil, tblRsLabor.AbsUtil AS absutil, tblRsLabor.clocktime, zstblwhatif.familyid, tblRsLabor.WID " +
            " FROM tblgeneral INNER JOIN (zstblwhatif INNER JOIN (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) ON zstblwhatif.WID = tblRsLabor.WID) ON tblgeneral.dummylink = zstblwhatif.dummyline" +
            " WHERE (((tblLabor.LaborDesc)<>\"NONE\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";
    private static readonly int[] myArray = new int[] { 1, 3, 6, 4, 5, 7 };

    [WebMethod(EnableSession = true)]
    public static string GetMaxUtil()
    {

        return Query.GetMaxUtil();

    }

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }
      
}

//Name = [1]
//   Scenario = [3]
//   Ineff = [6]
//   Setup = [4]
//   Run = [5]
//   ClockTime = [7]