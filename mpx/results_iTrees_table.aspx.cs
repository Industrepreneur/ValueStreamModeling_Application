using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

public partial class results_iTrees_table : IbomResultPage {

    public results_iTrees_table() {
        PAGENAME = "results_iTrees_table.aspx";
        FIELDS = new string[] { "partdesc", "description", "Level", "partdesc", "EndTime", "StartTime", "FlowTime", "LTEquip", "LTLabor", "LTSetup", "LTRun", "LTWaitLot" };
        HEADERS = new string[] { null, "What-If Scenario", "Level", "Product Name", "End Time", "Start Time", "MCT", "Time Waiting for Equip", "Time Waiting for Labor", "Time for Setup", "Time for Run", "Time Waiting for Rest of Lot" };

        tableQueryString = "SELECT DISTINCTROW IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip') AS description, tblRsIbomTree.PartDesc, tblRsIbomTree.Level, tblRsIbomTree.EndTime, tblRsIbomTree.StartTime, tblRsIbomTree.Slack, tblRsIbomTree.FlowTime, tblRsIbomTree.LTEquip, tblRsIbomTree.LTLabor, tblRsIbomTree.LTSetup, tblRsIbomTree.LTRun, tblRsIbomTree.LTWaitLot, tblRsIbomTree.LTWaitAsm, tblRsIbomTree.Wid, tblRsIbomTree.maxendtime, tblRsIbomTree.Count "
                + " FROM tblRsIbomTree INNER JOIN zstblwhatif ON tblRsIbomTree.Wid = zstblwhatif.WID  WHERE (((IIf([zstblwhatif].[display],[zstblwhatif].[name],'_skip'))<>'_skip'))";

        defaultSortString = "ORDER BY tblRsIbomTree.Wid, tblRsIbomTree.maxendtime, tblRsIbomTree.Count";

        sortedTableName = "tblRsIbom_Tree";  //NONE today ??
        wantSort = false;

        value = 0;

        InitializeUnits();
        for (int i = 6; i < UNITS.Length; i++) {
            UNITS[i] = true;
        }
    }



    protected void Page_Load(object sender, EventArgs e) {

        //btnSort.Enabled = false;
        if (dropListProducts.Items.Count > 0) {
            if (dropListProducts.SelectedItem == null) {
                dropListProducts.Items[0].Selected = true;
            }
        }
        try {
            if (!Page.IsPostBack) {
                if (dropListProducts.SelectedItem != null) {
                    MakeResultsTree(dropListProducts.SelectedValue); // creates the poles table and the image of the graph
                    dlZoom.SelectedIndex = 2;
                    LoadTreesGraph();
                }
                this.SetTableData();
            }
            base.Page_Load(sender, e);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void LoadTreesGraph() {
        string treeImgFullPath = GetDirectory() + userDir + "Graphs//" + MyUtilities.TREES_IMG_NAME;
        string treeImgRelPath = "App_Data/" + MyUtilities.clean(userDir, '\\') + "/Graphs/" + MyUtilities.TREES_IMG_NAME;

        string browserPath = GetMainDirectory() + BROWSER_DIR + "//" + userDir + "Graphs//" + MyUtilities.TREES_IMG_NAME;
        string browserRelPath = BROWSER_DIR + "/" + MyUtilities.clean(userDir, '\\') + "/Graphs/" + MyUtilities.TREES_IMG_NAME + "?" + DateTime.Now.Ticks;
        if (File.Exists(treeImgFullPath)) {
            try {
                try {
                    File.Delete(browserPath);
                } catch (Exception) { }
                File.Copy(treeImgFullPath, browserPath);
            } catch (Exception) {}
            Bitmap image = new Bitmap(browserPath);
            int width = image.Width;
            double zoom = double.Parse(dlZoom.SelectedValue) / 100;
            int finalWidth = (int)Math.Round(width * zoom);
            LiteralControl lit = new LiteralControl("<img src=\"" + browserRelPath + "\" alt=\"Trees Graph\" //style=\"width:" + finalWidth + "px; margin-bottom:20px;\" />");
            pictureHolder.Controls.Add(lit);
            image.Dispose();
        } else {
            Master.ShowErrorMessage("No data available. Please run MPX first.");
        }
    }

    protected void MakeResultsTree(string productId) {
        try {
            ClassE classE = new ClassE(GetDirectory() + userDir);
            classE.setGlobalVar();
            int prodId = int.Parse(productId);
            classE.m_makeResultsTree(prodId);
            classE.place_tree(prodId);
            classE.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
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

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        try {
            string dataFile = GetDirectory() + userDir + "\\" + MAIN_USER_DATABASE;
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

    private void FillDlZoom() {
        dlZoom.Items.Clear();
        foreach (int value in MyUtilities.ZOOM_LEVELS) {
            ListItem item = new ListItem(value + "%", value + "");
            dlZoom.Items.Add(item);
        }
    }

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e) {
        try {
            string prodId = dropListProducts.SelectedValue;
            MakeResultsTree(prodId);
            RefreshData();
            LoadTreesGraph();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }


    protected void dlZoom_SelectedIndexChanged(object sender, EventArgs e) {
        LoadTreesGraph();
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