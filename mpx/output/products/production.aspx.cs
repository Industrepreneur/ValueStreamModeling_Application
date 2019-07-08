using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web.Services;


public partial class P_Production : DbPage
{

    

    public P_Production ()
    {
        PAGENAME = "/output/products/production.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }
}

//FIELDS = new string[] { "prodid", "Name", "Description", "ShippedProd", "GoodForAsmProd", "ScrapInAsm", "Scrap" };
//HEADERS = new string[] { null, "Name", "Scenario", "Shipped", "Good Components", "Scrap Components", "Scrap Assemblies" };
//tableQueryString = " SELECT DISTINCTROW tblProdFore.ProdDesc AS Name, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS Description, tblRsProd.ShippedProd, tblRsProd.GoodForAsmProd, tblRsProd.ScrapInAsm, tblRsProd.Scrap, tblRsProd.ProdID, zstblwhatif.familyid, tblRsProd.WID "
//                  + " FROM zstblwhatif INNER JOIN (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) ON zstblwhatif.WID = tblRsProd.WID"
//                  + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip' ))<>'_skip'))";


//FIELD_OFFSETS = new int[] { 3, 3 };