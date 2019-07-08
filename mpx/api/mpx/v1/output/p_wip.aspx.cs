using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = "SELECT DISTINCTROW tblProdFore.ProdDesc, tblRsProd.WIP, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsProd.WID FROM (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) INNER JOIN zstblwhatif ON tblRsProd.WID = zstblwhatif.WID where (((IIf([zstblwhatif].[display], [zstblwhatif].[Name], '_skip')) <> '_skip'))";

    public static readonly int[] myArray = new int[] { 0, 2, 1 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}

//Name = 0
//Scenario = 2
//WIP = 1