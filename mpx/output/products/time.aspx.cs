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

public partial class P_Time : DbPage
{

    public P_Time ()
    {
        PAGENAME = "/output/products/time.aspx";
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }

}

//FIELDS = new string[] { "ProdID", "ProdDesc", "description", "LtWaitAsm", "LTEquip", "LTLabor", "LTSetup", "LTRun", "LTWaitLot" };
//HEADERS = new string[] { null, "Product Name", "What-If Scenario", "Out of Area", "Time Waiting for Equip", "Time Waiting for Labor", "Time for Setup", "Time for Run", "Time Waiting for Rest of Lot" };


//graphQueryString = "SELECT DISTINCTROW tblProdFore.ProdDesc, tblRsProd.LTEquip, tblRsProd.flowtime, tblRsProd.LTLabor, tblRsProd.LTSetup, tblRsProd.LTRun, tblRsProd.LTWaitLot, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsProd.WID, tblRsProd.LTWaitAsm "
//    + " FROM (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) INNER JOIN zstblwhatif ON tblRsProd.WID = zstblwhatif.WID "
//    + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip'))<>'_skip'))";


//FIELDS = new string[] { "prodid", "Name", "Description", "FlowTime", "LTEquip", "LTLabor", "LTSetup", "LTRun", "LTWaitLot", "WIP" };
//HEADERS = new string[] { null, "Name", "Scenario", "MCT", "Time Waiting for Equip", "Time Waiting for Labor", "Time for Setup", "Time for Run", "Time Waiting for Rest of Lot", "Average WIP" };
//tableQueryString = " SELECT DISTINCTROW tblProdFore.ProdDesc AS Name, IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS Description, tblRsProd.FlowTime, tblRsProd.LTEquip, tblRsProd.LTLabor, tblRsProd.LTSetup, tblRsProd.LTRun, tblRsProd.LTWaitLot, tblRsProd.LTWaitAsm, tblRsProd.WIP, tblRsProd.ProdID, zstblwhatif.familyid, tblRsProd.WID "
//                  + " FROM zstblwhatif INNER JOIN (tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID) ON zstblwhatif.WID = tblRsProd.WID"
//                  + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip' ))<>'_skip'))";


//FIELD_OFFSETS = new int[] { 3, 3 };