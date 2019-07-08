using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

public partial class results_iPoles_table : IbomResultPage {


    public results_iPoles_table() {
        PAGENAME = "results_iPoles_table.aspx";
        FIELDS = new string[] { "partdesc", "description", "Pole", "Level", "partdesc", "EndTime", "StartTime", "FlowTime", "LTEquip", "LTLabor", "LTSetup", "LTRun", "LTWaitLot" };
        HEADERS = new string[] { null, "What-If Scenario", "Pole", "Level", "Product Name", "End Time", "Start Time", "MCT", "Time Waiting for Equip", "Time Waiting for Labor", "Time for Setup", "Time for Run", "Time Waiting for Rest of Lot" };

        tableQueryString = "SELECT DISTINCTROW IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsIbomPoles.PartDesc, tblRsIbomPoles.Pole, "
                         + " tblRsIbomPoles.Level, tblRsIbomPoles.EndTime, tblRsIbomPoles.StartTime, tblRsIbomPoles.Slack, tblRsIbomPoles.FlowTime, tblRsIbomPoles.LTEquip, tblRsIbomPoles.LTLabor, tblRsIbomPoles.LTSetup, tblRsIbomPoles.LTRun, tblRsIbomPoles.LTWaitLot, tblRsIbomPoles.LTWaitAsm "
                         + " FROM tblRsIbomPoles INNER JOIN zstblwhatif ON tblRsIbomPoles.Wid = zstblwhatif.WID"
                         + " WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip'))<>'_skip'))";
        sortedTableName = "tblRsIbom_Pole";  //NONE today ??
        defaultSortString = "ORDER BY tblRsIbomPoles.Wid, tblRsIbomPoles.Count";
        wantSort = false;

        value = 1;

        InitializeUnits();
        for (int i = 7; i < UNITS.Length; i++) {
            UNITS[i] = true;
        }


    }


    protected void Page_Load(object sender, EventArgs e) {
        //txtTest.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
        if (dropListProducts.Items.Count > 0) {
            if (dropListProducts.SelectedItem == null) {
                dropListProducts.Items[0].Selected = true;
            }
        }
        try {
            if (!Page.IsPostBack) {
                if (dropListProducts.SelectedItem != null) {
                    MakeResultsPoles(dropListProducts.SelectedValue); // creates the poles table and the image of the graph
                    dlZoom.SelectedIndex = 2;
                    LoadPolesGraph();
                }
                this.SetTableData();
            }
            base.Page_Load(sender, e);
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        try {
            srcProductsList.DataFile = dataFile;
            dropListProducts.DataBind();
            FillDlZoom();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            if (!TablesLinked()) {
                Master.ShowErrorMessage("An error has occured. Current model '" + Master.GetCurrentModel() + "' is not loaded properly because some tables are missing. Please go to the models page and load the model again.");                
            } else {
                Master.ShowErrorMessage("MPX internal error has occured.");
            }
        }
    }

    protected override GridView getGridView() {
        return grid;
    }

    protected override Chart getChart() {
        return null;
    }

    public override Panel GetSortPanelContainer() {
        return sortPanelContainer;
    }


    public override Control GetSortButtonContainer() {
        return buttondiv;
    }

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e) {
        try {
            string prodId = dropListProducts.SelectedValue;
            MakeResultsPoles(prodId);
            RefreshData();
            LoadPolesGraph();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void MakeResultsPoles(string productId) {
        try {
            ClassE classE = new ClassE(GetDirectory() + userDir);
            classE.setGlobalVar();
            int prodId = int.Parse(productId);
            classE.m_makeResultsPoles(prodId);
            classE.place_poles(prodId);
            classE.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void LoadPolesGraph() {
        string polesImgFullPath = GetDirectory() + userDir + "Graphs//" + MyUtilities.POLES_IMG_NAME;
        string polesImgRelPath = "App_Data" + MyUtilities.clean(userDir, '\\') + "/Graphs/" + MyUtilities.POLES_IMG_NAME;
        string browserPath = GetMainDirectory() + BROWSER_DIR + "//" + userDir + "Graphs//" + MyUtilities.POLES_IMG_NAME;
        string browserRelPath = BROWSER_DIR + "/" +MyUtilities.clean(userDir, '\\') + "/Graphs/" + MyUtilities.POLES_IMG_NAME + "?" + DateTime.Now.Ticks;
        if (File.Exists(polesImgFullPath)) {
            try {
                try {
                    File.Delete(browserPath);
                } catch (Exception) { }
                File.Copy(polesImgFullPath, browserPath);
            } catch (Exception) { }
            Bitmap image = new Bitmap(browserPath);
            int width = image.Width;
            double zoom = double.Parse(dlZoom.SelectedValue) / 100;
            int finalWidth = (int)Math.Round(width * zoom);
            
            LiteralControl lit = new LiteralControl("<img src=\"" + browserRelPath + "\" alt=\"Poles Graph\" style=\"width:" + finalWidth + "px; margin-bottom:20px;\" />");
            pictureHolder.Controls.Add(lit);
        } else {
            Master.ShowErrorMessage("No data available. Please run MPX first.");
        }
    }

    private void FillDlZoom() {
        dlZoom.Items.Clear();
        foreach (int value in MyUtilities.ZOOM_LEVELS) {
            ListItem item = new ListItem(value + "%", value + "");
            dlZoom.Items.Add(item);
        }
    }

    protected void dlZoom_SelectedIndexChanged(object sender, EventArgs e) {
        LoadPolesGraph();
    }

    protected override Panel GetCopyTableContainer() {
        return copyPanelContainer;
    }

    protected override Control GetMenuContainer() {
        return pnlMenu;
    }

    protected override Control GetTabsDiv() {
        return tabsDiv;
    }

    protected override bool LocalTablesLinked() {
        if (!UpdateSql("SELECT ProdID FROM tblProdfore;")) {
            return false;
        }
        return true;
    }
}