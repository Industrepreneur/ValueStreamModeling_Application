
using System.Collections.Generic;

using System.Web.Services;


public partial class API : System.Web.UI.Page
{
   
    private static readonly string myQuery = "SELECT DISTINCTROW tblEquip.EquipDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblLabor.LaborDesc, SetupUtil, RunUtil, LabWaitUtil, RepUtil, Idle, ([Setuputil]+[runUtil]+[repUtil]+[labWaitUtil]) AS Total, tblRsEquip.clocktime, tblRsEquip.EquipID, zstblwhatif.familyid, tblRsEquip.WID" +
            " FROM ((tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID" +
            " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\") AND ((tblEquip.EquipDesc)<>\"None\"))";
    private static readonly int[] myArray = new int[] { 0, 1, 3, 5, 4, 6, 2, 9, 7 };

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

//Name = [0]
//   Scenario = [1]
//   Setup = [2]
//   WFL = [3]
//   Run = [4]
//   Repair = [5]
//   LaborAssigned = [6]
//   ClockTime = [7]
//   Idle = [8]