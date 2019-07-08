using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

public partial class P_WIP : DbPage
{

    public P_WIP() {
        PAGENAME = "/output/products/wip.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }


}

//FIELDS = new string[] { "prodid", "ProdDesc", "description", "WIP" };
//HEADERS = new string[] { null, "Name", "Scenario", "Average WIP" };

//defaultSortString = "ORDER BY tblProdFore.ProdDesc, tblRsProd.WID";
//graphQueryString = "SELECT DISTINCTROW tblProdFore.ProdDesc, tblRsProd.WIP, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsProd.WID FROM (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) INNER JOIN zstblwhatif ON tblRsProd.WID = zstblwhatif.WID where (((IIf([zstblwhatif].[display], [zstblwhatif].[Name], '_skip')) <> '_skip'))";
//tableQueryString = "SELECT DISTINCTROW tblProdFore.ProdDesc, tblRsProd.WIP, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsProd.WID FROM (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) INNER JOIN zstblwhatif ON tblRsProd.WID = zstblwhatif.WID where (((IIf([zstblwhatif].[display], [zstblwhatif].[Name], '_skip')) <> '_skip'))";

//FIELDS = new string[] { "prodid", "Name", "Description", "WIP" };
//HEADERS = new string[] { null, "Name", "Scenario", "Average WIP" };
//tableQueryString = " SELECT DISTINCTROW tblProdFore.ProdDesc AS Name, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS Description, tblRsProd.WIP, tblRsProd.ProdID, zstblwhatif.familyid, tblRsProd.WID "
//                  + " FROM zstblwhatif INNER JOIN (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) ON zstblwhatif.WID = tblRsProd.WID"
//                  + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip' ))<>'_skip'))";
//FIELD_OFFSETS = new int[] { 3, 3 };