using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = " SELECT DISTINCTROW tblProdFore.ProdDesc AS Name, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS Description, tblRsProd.TotalGoodProd, tblRsProd.ShippedProd, tblRsProd.GoodForAsmProd, tblRsProd.ScrapInAsm, tblRsProd.Scrap, tblRsProd.ProdID, zstblwhatif.familyid, tblRsProd.WID "
                          + " FROM zstblwhatif INNER JOIN (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) ON zstblwhatif.WID = tblRsProd.WID"
                          + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip' ))<>'_skip'))";

    public static readonly int[] myArray = new int[] { 0, 1, 3, 4, 6, 5 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}

//Name = 0
//Scenario = 1
//TotalGood = 2
//Shipped = 3
//Good = 4
//ScrapAsm = 6
//Scrap = 5