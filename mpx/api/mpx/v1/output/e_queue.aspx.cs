using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = "SELECT DISTINCTROW tblEquip.EquipDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblRsEquip.Qprocess, tblRsEquip.QWait, tblRsEquip.Qtotal, tblRsEquip.EquipID, zstblwhatif.familyid" +
                     " FROM (((tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID) INNER JOIN tblgeneral ON zstblwhatif.dummyline = tblgeneral.dummylink" +
                     " WHERE (((tblEquip.EquipDesc)<>\"None\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))";

    public static readonly int[] myArray = new int[] { 0, 1, 2, 3, 4 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}

//Name = 0
//Scenario = 1
//InP = 2
//InQ = 3
//WIP = 4