using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = "SELECT DISTINCTROW tblProdFore.ProdDesc, tblRsProd.LTEquip, tblRsProd.flowtime, tblRsProd.LTLabor, tblRsProd.LTSetup, tblRsProd.LTRun, tblRsProd.LTWaitLot, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsProd.WID, tblRsProd.LTWaitAsm "
            + " FROM (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) INNER JOIN zstblwhatif ON tblRsProd.WID = zstblwhatif.WID "
            + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip'))<>'_skip'))";

    public static readonly int[] myArray = new int[] { 0, 7, 8, 1, 3, 4, 5, 6 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}

//Name = 0
//Scenario = 7
//OOA = 8
//WFE = 1
//WFL = 3
//Setup = 4
//Run = 5
//WFLot = 6