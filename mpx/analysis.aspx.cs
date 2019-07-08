using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;



///   gwwd  to do  on restart .. if in whatif/visionwhatif ?
///   


//   delete whatif -> vision curves 
//  rename whatif  -> rename vision curves...
//  after delete vision        //  delete curves and points and whatif stuff-points only

public partial class analysis : MultiViewOnePage {

    int currentAnalysisId = 0;
    int currentWhatifId = 0;
    int in_close_curr_wid = 0;
    string currentAnalysisName;
    

    protected string continueaction
    {
        get
        {
            if (ViewState["continueaction"] == null)
            {
                ViewState["continueaction"] = "";
            }
            return (string)ViewState["continueaction"];
        }
        set { ViewState["continueaction"] = value; }
    }

    private string TableSortOrder {
        get {
            if (ViewState["TableSortOrder"] == null) {
                ViewState["TableSortOrder"] = "";
            }
            return (string)ViewState["TableSortOrder"];
        }
        set { ViewState["TableSortOrder"] = value; }
    }

    AjaxControlToolkit.ModalPopupExtender extenderNewName;
    Panel pnlNewName;
    Button btnNewNameOk;
    TextBox txtNewName;

    string[] FIELDS_WHATIF;  //  from curve for right side whatifs 
    string[] HEADERS_WHATIF;

    public analysis() {
        PAGENAME = "analysis.aspx";
        addView("Analysis Settings");
        addView("Analysis Output");

        graphQueryString = "SELECT tblRsEquip.WID, tblRsEquip.EquipID, tblRsEquip.Idle FROM tblRsEquip;";

        wantSort = true;

      FIELDS = new string[] { "WID", "Curve", "xValue", "yValue" };
      HEADERS = FIELDS;

       
        FIELDS_WHATIF = new string[] { "analysisid", "CurveName",           "MainVarFrom", "MainVarTo", "MainVarStep"}; //, "Base_wid" };
        HEADERS_WHATIF = new string[] { null,        "Name of What-If Scenario", "From Point", "To Point", "Step Size"};  //, "Curve_WID" };

    }

    private void Page_PreRender(Object sender, System.EventArgs e) {
        this.Form.Attributes.Remove("onsubmit");
        this.Form.Attributes.Add("onsubmit", "ShowProgressRun('" + pnlLoading.ClientID + "'); return true;");
        btnRun1.Attributes.Add("onclick", " this.disabled = true; startProgressCheck(); $find('modalPopupLoadingBehavior').show(); return true;");
        btnRun2.Attributes.Add("onclick", " this.disabled = true; startProgressCheck(); $find('modalPopupLoadingBehavior').show(); return true;");
        //  btnSave.Attributes.Add("onclick", " this.disabled = true; $find('modalPopupLoadingBehavior').show(); return true;");
        btnNewNameOk.Attributes.Add("onclick", " this.disabled = true; $find('modalPopupLoadingBehavior').show(); return true;");

    }


    int inrun = 0;

    protected void Page_Load(object sender, EventArgs e) {

        base.Page_Load(sender, e);
        ClassE classE_act = new ClassE(GetDirectory() + userDir);

        if (!Page.IsPostBack) {


        
           // not needed !!! AddWhatifVarToStepColumns(true);

            btnAnalysisHelp.OnClientClick = "fnSetFocusHelp('" + btnAnalysisHelpOk.ClientID + "'); return false;";
            btnAnalysisHelp2.OnClientClick = btnAnalysisHelp.OnClientClick;
            btnAnalysisHelp3.OnClientClick = btnAnalysisHelp.OnClientClick;

            //btnSaveAs.OnClientClick = "fnSetFocusName('" + txtNewName.ClientID + "'); return false;";
            txtNewName.Attributes.Add("onkeydown", "doFocus('" + btnNewNameOk.ClientID + "', event);");
            txtNewCase.Attributes.Add("onkeydown", "doFocus('" + btnNewCase.ClientID + "', event);");
            lstCases.Attributes.Add("onkeydown", "doFocus('" + btnLoadCase.ClientID + "', event);");
            //lstCases.SelectedIndex = 0;
            SetActiveView(currentAnalysisId == 0 ? -1 : 0);
            currentAnalysisName = (currentAnalysisId == 0 ? "" : classE_act.GetAnalysisName(currentAnalysisId));
            SetnameText();
            SetAnalysisListData();
            SetupVariableLists();
            if (currentAnalysisId != 0) {
                SetWhatifsInOut();  //ReadAnalysisData();
                SetGraphData();   //  wait until needed ??  on return from other page left with graph showing ...  but doesnot go to graph !!
            }
        } else if (currentAnalysisId != 0) {
            Control postBackControl = GetPostBackControl(this.Page);
            if (postBackControl == null || postBackControl.ID == null ||
                (!postBackControl.ID.Equals("btnShowAnalysis") && !postBackControl.ID.Equals(InputPageControls.BTN_OK_SORT) && !postBackControl.ID.Equals(InputPageControls.BTN_CANCEL_SORT) && !postBackControl.ID.Equals("grid"))) {
                     this.SetGraphData();  //  showing graph  ..  not on run ???  or prev page to run, set
            }
        }

       
        if (classE_act.Calc_count() == 0)
        {
            Master.ShowErrorMessage("No products are defined yet. Vision analysis cannot be done.");
            //  need to wait !!! Response.Redirect("products_table.aspx");

            return;
        }


    }

    protected override void OnInit(EventArgs e) {

        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        //InitializeGrid();
        InitializeWhatifGrid();  //  why here ???

        extenderNewName = InputPageControls.GenerateNewNameExtender();
        extenderNewName.TargetControlID = hdnAction.ID;

        pnlNewName = InputPageControls.GenerateNewNamePanel("Enter a new name for the Vision Analysis: ");

        pnlNewAnalysisName.Controls.Add(pnlNewName);
        pnlNewAnalysisName.Controls.Add(extenderNewName);

        btnNewNameOk = pnlNewName.FindControl(InputPageControls.BTN_OK_NEW_NAME) as Button;
        if (btnNewNameOk != null) {
            btnNewNameOk.Click += btnNewNameOk_Click;
        }
        txtNewName = pnlNewName.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;


        currentAnalysisId = GetAnalysisMode();
        currentWhatifId = GetWhatifMode();


    }

    protected override void InitializeComponent() {
        base.InitializeComponent();
        Button btnOkSort = sortPanel.FindControl(InputPageControls.BTN_OK_SORT) as Button;

        Button btnCancelSort = sortPanel.FindControl(InputPageControls.BTN_CANCEL_SORT) as Button;
        btnCancelSort.OnClientClick = "HidePopup('" + InputPageControls.BEHAVIOR_SORT + "'); return true;";
        btnCancelSort.Click += btnCancelSort_Click;

    }

    protected override void btnOkSort_Click(object sender, EventArgs e) {
        TableSortOrder = GetSortOrder();
        SetGraphData();
    }

    protected void btnCancelSort_Click(object sender, EventArgs e) {
        SetSortOrder(TableSortOrder);
        SetGraphData();
    }

    protected void SetSortOrder(string sortString) {
        string[] sortFields = sortString.Split(',');
        for (int i = 0; i < sortFields.Length; i++) {
            string[] fields = sortFields[i].Trim().Split(' ');
            try {
                comboSorts[i].SelectedValue = fields[0].Trim();
                if (fields.Length > 1 && fields[1].Trim().ToLower().Equals("desc")) {
                    rdbtnSortDesc[i].Checked = true;
                } else {
                    rdbtnSortAsc[i].Checked = true;
                }
            } catch (Exception) { }
        }
    }

    protected void SetnameText()
    {
        if (currentAnalysisId != 0)
        {
            txtCaseComment.Text = GetDatabaseField("Comment", "AnalysisId", currentAnalysisId, "tblanalysis");
            txtCaseName.Text = GetDatabaseField("Name", "AnalysisId", currentAnalysisId, "tblanalysis");
        }

    } 
    protected void PreventErrorOnbinding(object sender, EventArgs e) {
        ListBox lstBox = sender as ListBox;

        lstBox.DataBinding -= new EventHandler(PreventErrorOnbinding);

        try {
            lstBox.DataBind();
        } catch (Exception) {

        }
    }

    private void SetAnalysisListData() {
        using (OleDbConnection oleConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE)) {
            OleDbCommand oleCmd = new OleDbCommand("SELECT tblAnalysis.* FROM tblAnalysis;", oleConn);
            OleDbDataAdapter adapter = new OleDbDataAdapter(oleCmd);
            try {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                lstCases.DataSource = dt;
                lstCases.DataBind();
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage("Error in reading list of existing Vision Analyses.");
            } finally {

                try { oleConn.Close(); } catch (Exception) { }
            }
        }
    }

    protected override void SetActiveView(int itemNum) {
        if (itemNum == -1) {
            productMenu.Visible = false;
            tabsDiv.Attributes.Add("class", "tabsNonvisible");
        } else {
            productMenu.Visible = true;
            tabsDiv.Attributes.Add("class", "tabs");
            productMenu.Items[itemNum].Selected = true;

        }
        multiView.ActiveViewIndex = itemNum + 1;
    }

    protected override Control GetMenuContainer() {
        return pnlMenu;
    }

    protected override Control GetTabsDiv() {
        return tabsDiv;
    }

    protected override MultiView GetMultiView() {
        return AnalysisMultiView;
    }

    protected void btnLoadCase_Click(object sender, EventArgs e) {
        if (lstCases.SelectedItem == null) {
            Master.ShowErrorMessage("No Vision Analysis is selected. Please select the Vision Analysis to load.");
            return;
        }
        try {
            LoadAnalysis(int.Parse(lstCases.SelectedItem.Value));
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("An error has occured. Cannot load Vision Analysis.");
        }

        SetActiveView(0);
    }

    public void droplistprodChange(object sender, EventArgs e)
    {
        droplistprodChange0();
    }
 
    
 public void droplistprodChange0()  { 
        //  changed mainvar ...

      
        string mainvar = dropListProduct.SelectedItem.Value;
        string str1 = "UPDATE tblanalysis SET tblanalysis.MainVar = '" + mainvar +"' WHERE (((tblanalysis.AnalysisID)= " + currentAnalysisId + ")); ";
            UpdateSql(str1);
        str1 = "UPDATE tblanalysiscurve SET tblanalysiscurve.recalc = True WHERE (((tblanalysiscurve.AnalysisID)=" + currentAnalysisId +"));";
            UpdateSql(str1);

    }

    private void LoadAnalysis(int analysisId) {

      //    analysis name etc. for master page ...

        ClassE classE = new ClassE(GetDirectory() + userDir);
        string name;
        string comment;

        try {
            name = classE.GetAnalysisName(analysisId);
            comment = (string) GetDatabaseField("Comment", "analysisId", analysisId, "tblanalysis");
            currentAnalysisName = name;
            currentAnalysisId = analysisId;
            Master.PassCurrentAnalysisName(name);
            Master.SetCurrentAnalysisLabel();
            classE.Close();
        } catch (Exception ex) {
            classE.Close();
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("An error has occured while loading Vision Analysis.");
            return;
        }
       
        UpdateSql("UPDATE zstblstate SET AnalysisID = " + analysisId + ";");
        
        name = MyUtilities.clean(name);
        comment = MyUtilities.clean(comment);

        txtCaseName.Text = name;
        txtCaseComment.Text = comment;
        UpdateSql("UPDATE tblAnalysis SET Name = '" + name + "', Comment = '" + comment + "' WHERE AnalysisID = " + currentAnalysisId + ";");

        SetAnalysisListData();
        SetWhatifsInOut();  //ReadAnalysisData();
    }

    private bool nameValid;

    private bool CreateNewAnalysis(string name) {
        return CreateNewAnalysis(name, "New Vision Analysis");
    }

    private bool CreateNewAnalysis(string name, string comment) {
        nameValid = true;
        if (name.Equals(String.Empty)) {
            Master.ShowErrorMessageAndFocus("Invalid name for Vision Analysis.", txtNewName.ClientID);
            nameValid = false;
            return false;
        }

        if (AnalysisExists(name)) {
            Master.ShowErrorMessageAndFocus("Another Vision Analysis with the same name exists. Please select a different name.", txtNewName.ClientID);
            nameValid = false;
            return false;
        }

        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT tblAnalysis.* FROM tblAnalysis;";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);

        if (!adoOpened || !adoRecOpened) {
            Master.ShowErrorMessage("Error in creating new Vision Analysis. Cannot read data.");
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            return false;
        }

        try {
            rec.AddNew();
            currentAnalysisId = int.Parse(rec.Fields["AnalysisID"].Value.ToString());
            rec.Fields["Comment"].Value = comment;
            rec.Fields["Name"].Value = name;
            rec.Fields["mainVar"].Value = "2:-1"; //  all products ....
       
            rec.Update();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Error in creating new Vision Analysis. Cannot add new vision analysis.");
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            return false;
        }
        DbUse.CloseAdoRec(rec);
        DbUse.CloseAdo(conn);


        Master.PassCurrentAnalysisName(name);
        Master.SetCurrentAnalysisLabel();
        UpdateSql("UPDATE zstblstate SET AnalysisID = " + currentAnalysisId + ";");
        return true;
    }

    protected void btnDeleteCase_Click(object sender, EventArgs e) {
        if (lstCases.SelectedItem == null) {
            Master.ShowErrorMessage("No Vision Analysis is selected. Please select the Vision Analysis to delete.");
            return;
        }
        int analysisId = 0;
        try {
            analysisId = int.Parse(lstCases.SelectedItem.Value);
        } catch (Exception) {
            return;
        }

        UpdateSql("DELETE tblAnalysis.* FROM tblAnalysis WHERE AnalysisID = " + analysisId + ";");

        //  delete curves and points and whatif stuff 
        //xxx  check it out   with data etc....
        string str1 = "DELETE tblanalysiscurve.*, tblanalysiscurve.AnalysisID FROM tblanalysiscurve WHERE (((tblanalysiscurve.AnalysisID)=" + analysisId +"));";
        UpdateSql(str1);

        ClassF classF = new ClassF(GetDirectory() + userDir);
        List<int> pointsUsed = new List<int>();
        try {
            ADODB.Recordset recPoints = new ADODB.Recordset();
            bool recPointsOpened = DbUse.OpenAdoRec(classF.globaldb, recPoints, "SELECT WID FROM tblAnalysisPoints WHERE AnalysisID = " + analysisId + ";");
            if (recPointsOpened) {
                while (!recPoints.EOF) {
                    pointsUsed.Add((int)recPoints.Fields["WID"].Value);
                    recPoints.MoveNext();
                }
                DbUse.CloseAdoRec(recPoints);
            }
            foreach (int whatifPoint in pointsUsed) {
                classF.DeleteWhatIfAudit(whatifPoint);   //  deleteing whatif (points ?)
            }
            classF.model_modified = -1;
            classF.saveModel_modified();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("An error has occured while deleting Vision Analysis Points.");
        }

        UpdateSql("delete tblabalysiscurve.* from tblanalysiscurve where analysisid = "+ analysisId +" ;");
        UpdateSql("delete tblabalysispoints.* from tblanalysispoints where analysisid = " + analysisId + " ;");

      

        classF.Close();
        SetAnalysisListData();
    }
    protected void btnNewCase_Click(object sender, EventArgs e) {
        currentAnalysisName = MyUtilities.clean(txtNewCase.Text);
        if (CreateNewAnalysis(currentAnalysisName)) {
            txtNewCase.Text = "";
            SetAnalysisListData();
            LoadAnalysis(currentAnalysisId);
            SetActiveView(0);
        }
    }

    protected string GetFullCurrentCaseLabel() {
        return Master.GetFullCurrentAnalysisLabel();
    }

    protected override void productMenu_MenuItemClick(object sender, MenuEventArgs e) {
        int itemNum = int.Parse(e.Item.Value);
        try {
            productMenu.Items[itemNum].Selected = true;
            multiView.ActiveViewIndex = itemNum + 1;
        } catch (Exception) { }

    }

    protected void btnEditCase_Click(object sender, EventArgs e) {
        UpdateAnalysisData();
    }

    protected void xxbtnResetNameComment_Click(object sender, EventArgs e) {
      ///  ReadAnalysisData();
    }

    protected void btnRight_Click(object sender, ImageClickEventArgs e) {
        

        if (lstWhatifOut.SelectedItem != null) {
            ListItem item = lstWhatifOut.SelectedItem;
            lstWhatifOut.SelectedIndex = -1;
            lstWhatifOut.Items.Remove(item);
         
      
            try
            {
                
                  
                DataRow row = dtWhatifsIn.NewRow();  //  dtwhatifsin has the data !!!
                row[FIELDS_WHATIF[0]] = currentAnalysisId;
               // row[FIELDS_WHATIF[1]] = item.Value;
                row[FIELDS_WHATIF[1]] = item.Text;
                row[FIELDS_WHATIF[2]] = DEFAULT_MAIN_VAR_VALUES[0];
                row[FIELDS_WHATIF[3]] = DEFAULT_MAIN_VAR_VALUES[1];
                row[FIELDS_WHATIF[4]] = DEFAULT_MAIN_VAR_VALUES[2];

                //  add row to tblanacur
                ClassE classE_act = new ClassE(GetDirectory() + userDir);
                int whatifId = classE_act.get_wid(item.Text);
                string str1 = "INSERT INTO tblanalysiscurve ( AnalysisID, Base_wid, Curvename, MainVarFrom, MainVarTo, MainVarStep, recalc ) SELECT "+currentAnalysisId +" AS Expr1, " + whatifId +" AS Expr2, '"+item.Text + "' AS Expr3, "+ DEFAULT_MAIN_VAR_VALUES[0] +" AS Expr4, "+ DEFAULT_MAIN_VAR_VALUES[1] +" AS Expr14, "+ DEFAULT_MAIN_VAR_VALUES[2] +" AS Expr24, -1 as exprver ;";
                    UpdateSql(str1);
               

             
                dtWhatifsIn.Rows.Add(row);
    

                gridWhatifs.DataSource = dtWhatifsIn;
                gridWhatifs.DataBind();
            } catch (Exception) { }



        }
    }

    protected float[] DEFAULT_MAIN_VAR_VALUES = new float[] { (float) Math.Round(0.5f, 6), (float) Math.Round(1.5f, 6), (float) Math.Round(0.1f, 6) };

   
   
    protected void btnClose_Click(object sender, EventArgs e) {
        continueaction = Action.SAVE;
        CloseAnalysis();
        SetActiveView(-1);
    }

    protected void btnShowAnalysis_Click(object sender, EventArgs e) {
        SetGraphData();

    }

    protected void btnShowAnalysis_Click0()
    {
        continueaction = Action.RUN;
        inrun = -1;
        SetGraphData();
        inrun = 0;

    }


    protected override void SetGraphData() {
        try {
            string mainVarPoint = GetDatabaseField("MainVar", "AnalysisID", currentAnalysisId, "tblAnalysis");
            string xAxisTitle = GetVariableDescription(mainVarPoint);

            string yAxisTitle = "";

            string chartloc = "";
         



            chart.ImageStorageMode = ImageStorageMode.UseImageLocation;
            string simpleUserdir = MyUtilities.clean(userDir, '\\');
            chart.ImageLocation = "~/" + DbPage.BROWSER_DIR + "/" + simpleUserdir + "/Graphs/Chart_#SEQ(500,3)";
            

            //gwwd added   remove old charts !!!
            //chartloc = "~/" + DbPage.BROWSER_DIR + "/" + simpleUserdir + "/Graphs/Chart_*.*";
            //File.Delete(chartloc);

            chart.CssClass = "chart";
            // set legend
            Legend legend = new Legend();
            legend.LegendStyle = LegendStyle.Column;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            legend.Docking = Docking.Right;
            legend.IsTextAutoFit = true;
            legend.ItemColumnSpacing = 100;
            chart.Legends.Add(legend);
            chart.Width = 900;
            chart.Height = 550;
            // format axis (titles and labels)
            ChartArea chartArea = chart.ChartAreas[0];
            chartArea.AxisY.Title = yAxisTitle;
            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisY.LabelStyle.Font = new Font("Trebuchet MS", 10.25F, FontStyle.Bold);
            chartArea.AxisX.LabelStyle.Font = chartArea.AxisY.LabelStyle.Font;
            chartArea.AxisY.TitleFont = new Font("Trebuchet MS", 15.25F, FontStyle.Bold);
            chartArea.AxisX.TitleFont = chartArea.AxisY.TitleFont;
            chartArea.AxisX.TextOrientation = TextOrientation.Auto;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.BorderWidth = 1;
            chartArea.BorderColor = Color.Black;
            chartArea.BorderDashStyle = ChartDashStyle.Solid;

            if (inrun != -1)  FillGraphAndTable();

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Error in reading Vision Analysis output graph and table.");
            pnlOutputGraphs.Visible = false;
        }
    }

    private string GetOutputSqlQuery(string property) {
        string sqlQuery = "";
        string[] properties = property.Split(':');
        string type = properties[0];
        string scope = properties[1];

        if (type.Equals(PROD.ToString())) {

            if (rdbtnOutputGeneral.Checked) {
                if (scope.Equals(ALL) || scope.Equals(DEPT)) {
                    string prodFamily = scope.Equals(ALL) ? " " : properties[2];
                    sqlQuery = "SELECT tblRsSummary.WID, tblRsSummary.TotalProd, tblRsSummary.WIP, tblRsSummary.FlowTime, tblRsSummary.Scrap, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.Curveid FROM tblRsSummary  INNER JOIN tblAnalysisPoints ON tblRsSummary.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblRsSummary.[Product family] = '" + prodFamily + "' ORDER BY Curveid, MainVarValue;";
                } else if (scope.Equals(ITEM)) {
                    string id = properties[2];

                    sqlQuery = "SELECT tblRsProd.WID, tblRsProd.TotalGoodProd as TotalProd, tblRsProd.WIP, tblRsProd.FlowTime, tblRsProd.Scrap, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.Curveid FROM tblRsProd INNER JOIN tblAnalysisPoints ON tblRsProd.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblRsProd.ProdID = " + id + " ORDER BY Curveid, MainVarValue;";
                } else {
                    return null;
                }

            } else {
                return null;
            }

        } else if (type.Equals(LABOR.ToString())) {
            if (scope.Equals(ITEM)) {
                string id = properties[2];
                sqlQuery = "SELECT tblLabor.LaborID, (SUM((SetupUtil+RunUtil+AbsUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QProcess+QWait) AS WIP, tblRsLabor.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.Curveid FROM (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) INNER JOIN tblAnalysisPoints ON tblRsLabor.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblLabor.LaborID = " + id + " GROUP BY tblRsLabor.WID, tblLabor.LaborID, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue;";
            } else if (scope.Equals(DEPT)) {
                string dept = properties[2];
                sqlQuery = "SELECT tblLabor.LaborDept, (SUM((SetupUtil+RunUtil+AbsUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QProcess+QWait) AS WIP, tblRsLabor.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid FROM (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) INNER JOIN tblAnalysisPoints ON tblRsLabor.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblLabor.LaborDept = '" + dept + "' GROUP BY tblRsLabor.WID, tblLabor.LaborDept, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue";
                //sqlQuery = "SELECT LaborID, SetupUtil, RunUtil, AbsUtil, QProcess, QWait, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.Curve FROM tblRsLabor INNER JOIN tblAnalysisPoints ON tblRsLabor.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblRsLabor.LaborID = " + id + " ORDER BY Curve, MainVarValue;";                
            } else if (scope.Equals(ALL)) {
                sqlQuery = "SELECT (SUM((SetupUtil+RunUtil+AbsUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QProcess+QWait) AS WIP, tblRsLabor.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid FROM (tblRsLabor INNER JOIN tblLabor ON tblRsLabor.LaborID = tblLabor.LaborID) INNER JOIN tblAnalysisPoints ON tblRsLabor.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " GROUP BY tblRsLabor.WID, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue";
            } else {
                return null;
            }
        } else if (type.Equals(EQUIP.ToString())) {
            if (scope.Equals(ITEM)) {
                string id = properties[2];
                sqlQuery = "SELECT tblEquip.EquipID, (SUM((LabWaitUtil+RepUtil+RunUtil+SetupUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QTotal) as WIP, tblRsEquip.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid FROM (tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblAnalysisPoints ON tblRsEquip.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblEquip.EquipID = " + id + " GROUP BY tblEquip.EquipID, tblRsEquip.WID, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue;";
            } else if (scope.Equals(DEPT)) {
                string dept = properties[2];
                sqlQuery = "SELECT tblEquip.EquipDept, (SUM((LabWaitUtil+RepUtil+RunUtil+SetupUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QTotal) as WIP, tblRsEquip.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid FROM (tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblAnalysisPoints ON tblRsEquip.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " AND tblEquip.EquipDept = '" + dept + "' GROUP BY tblEquip.EquipDept, tblRsEquip.WID, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue;";
            } else if (scope.Equals(ALL)) {
                sqlQuery = "SELECT (SUM((LabWaitUtil+RepUtil+RunUtil+SetupUtil) * IIf([GrpSiz]<0,1,[GrpSiz]))/SUM(IIf([GrpSiz]<0,1,[GrpSiz]))) AS Util, SUM(QTotal) AS WIP, tblRsEquip.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid FROM (tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblAnalysisPoints ON tblRsEquip.WID = tblAnalysisPoints.WID WHERE tblAnalysisPoints.AnalysisID = " + currentAnalysisId + " GROUP BY tblRsEquip.WID, tblAnalysisPoints.WID, tblAnalysisPoints.AnalysisID, tblAnalysisPoints.MainVarValue, tblAnalysisPoints.curveid ORDER BY curveid, MainVarValue;";
            } else {
                return null;
            }
        } else {
            return null;
        }

        return sqlQuery;
    }

    private void FillGraphAndTable() {
        // get all curves
        string sqlQuery = null;

        DropDownList dropListSelectedOutput = null;
        DropDownList dropListOutputType = null;
        if (rdbtnOutputGeneral.Checked) {
            dropListSelectedOutput = dropListOutputGeneral;
            dropListOutputType = dropListOutputProductType;
        } else if (rdbtnOutputLabor.Checked) {
            dropListSelectedOutput = dropListOutputLabor;
            dropListOutputType = dropListOutputLaborType;
        } else if (rdbtnOutputEquip.Checked) {
            dropListSelectedOutput = dropListOutputEquip;
            dropListOutputType = dropListOutputEquipType;
        } else {
            return;
        }

        string mainVarPoint = GetDatabaseField("MainVar", "AnalysisID", currentAnalysisId, "tblAnalysis");
        string xAxisTitle = GetVariableDescription(mainVarPoint);

        string yVarString = dropListSelectedOutput.SelectedItem.Value; // table key for y value
        string yVarDesc = dropListSelectedOutput.SelectedItem.Text; // y variable title

        sqlQuery = GetOutputSqlQuery(dropListOutputType.SelectedItem.Value);
        string yAxisTitle = GetVariableDescription(dropListOutputType.SelectedItem.Value, yVarDesc); // y variable full title

        ChartArea chartArea = chart.ChartAreas[0];
        chartArea.AxisY.Title = yAxisTitle;
        grid.Columns[2].HeaderText = yAxisTitle;

        // change table x header to match graph axis
        grid.Columns[1].HeaderText = xAxisTitle;

        // fill sort combo choices appropriately
        FillSortCombos(new string[] { xAxisTitle, yAxisTitle });

        TableStringHeaders = "Curve\t" + xAxisTitle + "\t" + yAxisTitle + "\r\n";

        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn(FIELDS[0], typeof(long)));
        dt.Columns.Add(new DataColumn(FIELDS[1], typeof(string)));
        dt.Columns.Add(new DataColumn(FIELDS[2], typeof(double)));
        dt.Columns.Add(new DataColumn(FIELDS[3], typeof(double)));

        ADODB.Connection globalConn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        string path = GetDirectory() + userDir + MAIN_USER_DATABASE;
        DbUse.OpenAdo(globalConn, path);
        DbUse.OpenAdoRec(globalConn, rec, sqlQuery);
        int currentCurve = 0;
        Series series = null;
        string seriesName = null;

        try {
            while (!rec.EOF) {
                int curveNum = (int)rec.Fields["CurveID"].Value;
                int whatifId = (int)rec.Fields["WID"].Value;
                if (currentCurve != curveNum) {
                    currentCurve = curveNum;
                    seriesName = GetDatabaseField("Name", "WID", whatifId, "tblWhatif");
                    int lastLetter = seriesName.ToLower().LastIndexOf("point");
                    if (lastLetter != -1) {
                        seriesName = seriesName.Substring(0, lastLetter - 1);
                    }
                    series = new Series();
                    series.IsVisibleInLegend = true;
                    series.ChartType = SeriesChartType.Line;
                    series.Name = seriesName;
                    series.BorderWidth = 3;
                    chart.Series.Add(series);  //  error here 

                }
                double x = Math.Round(float.Parse(rec.Fields["MainVarValue"].Value.ToString()), 6);
                double y = Math.Round(float.Parse(rec.Fields[yVarString].Value.ToString()), 6);
                double flowTime = -1;
                if (rdbtnOutputGeneral.Checked) {
                    flowTime = Math.Round((float)rec.Fields["FlowTime"].Value, 6);
                }

                if ((!yVarString.Equals("FlowTime") && !yVarString.Equals("WIP")) || flowTime != 0) {
                    series.Points.AddXY(x, y);

                    DataRow rowNew = dt.NewRow();
                    rowNew["WID"] = whatifId;
                    rowNew["Curve"] = seriesName;
                    rowNew["xValue"] = x;
                    rowNew["yValue"] = y;



                    dt.Rows.Add(rowNew);
                }


                rec.MoveNext();
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Error in creating Vision Analysis output graph.");
        } finally {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(globalConn);
        }

        try {
            DataView dv = dt.DefaultView;
            dv.Sort = TableSortOrder;
            grid.DataSource = dv;
            grid.DataBind();

            DataTable sortedTable = dv.ToTable();
            for (int i = 0; i < sortedTable.Rows.Count; i++) {
                DataRow row = sortedTable.Rows[i];
                TableString += row["Curve"] + "\t" + row["xValue"] + "\t" + row["yValue"] + "\r\n";
            }

            // set the text for copying
            if (rdbtnTableWithHeaders.Checked) {
                txtCopyTable.Text = TableStringHeaders + TableString;
            } else {
                txtCopyTable.Text = TableString;
            }

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Error reading Vision Analysis output table data.");
        }
        bool visible = dt.Rows.Count != 0;
        pnlOutputGraphs.Visible = visible;
        lblNoResults.Visible = !visible;


    }


    protected override void grid_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        grid.PageIndex = e.NewPageIndex;
        SetGraphData();
    }


    // generate columns in the table grid control
    protected void InitializeGrid() {
        grid.AutoGenerateColumns = false;
        grid.DataKeyNames = new string[] { FIELDS[0] };
        grid.AllowPaging = true;
        grid.PageIndexChanging += grid_PageIndexChanging;
        grid.RowDataBound += grid_RowDataBound;
        grid.AllowSorting = false;
        grid.HeaderStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        grid.RowStyle.CssClass = "datatable-rowstyle";
        grid.AlternatingRowStyle.BackColor = Color.White;
        grid.ShowHeaderWhenEmpty = true;
        grid.EmptyDataText = "There are no data records to display.";
        grid.BorderWidth = 3;
        grid.BorderColor = Color.Black;
        for (int i = 1; i < FIELDS.Length; i++) {
            TemplateField template = new TemplateField();
            SimpleGridViewTemplate itemTemplate = new SimpleGridViewTemplate(ListItemType.Item, FIELDS[i]);
            template.ItemTemplate = itemTemplate;
            template.HeaderText = FIELDS[i];
            grid.Columns.Add(template);
        }

    }

    // generate columns in the table grid control
    protected void InitializeWhatifGrid() {
        TEXT_BOX_IDS = new string[FIELDS_WHATIF.Length];
        LABEL_IDS = new string[FIELDS_WHATIF.Length];
        for (int i = 0; i < TEXT_BOX_IDS.Length; i++) {
            TEXT_BOX_IDS[i] = "txt" + CommonGridPage.BASE_CONTROL_NAME + i;
            LABEL_IDS[i] = "lbl" + CommonGridPage.BASE_CONTROL_NAME + i;
        }

        gridWhatifs.AutoGenerateColumns = false;
        gridWhatifs.DataKeyNames = new string[] { FIELDS_WHATIF[1] };  //  all ok !!!
        gridWhatifs.AllowPaging = true;
        gridWhatifs.PageIndexChanging += gridWhatifs_PageIndexChanging;
        gridWhatifs.RowCommand += gridWhatifs_RowCommand;
        gridWhatifs.RowDeleting += gridWhatifs_RowDeleting;
        gridWhatifs.RowEditing += gridWhatifs_RowEditing;
        gridWhatifs.RowUpdating += gridWhatifs_RowUpdating;
        gridWhatifs.RowDataBound += gridWhatifs_RowDataBound;
        gridWhatifs.AllowSorting = false;
        gridWhatifs.HeaderStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        gridWhatifs.RowStyle.CssClass = "datatable-rowstyle";
        gridWhatifs.AlternatingRowStyle.BackColor = Color.White;
        gridWhatifs.ShowHeaderWhenEmpty = true;
        gridWhatifs.EmptyDataText = "There are no data records to display.";
        gridWhatifs.BorderWidth = 3;
        gridWhatifs.BorderColor = Color.Black;


        TemplateField templ = new TemplateField();
        GridViewTemplate btnTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.BUTTONS, FIELDS_WHATIF[0], CommonGridPage.BASE_CONTROL_NAME + 0);
        btnTemplate.whatif = false;
        btnTemplate.wantCopyButton = false;
        btnTemplate.analysisGrid = true;

        templ.ItemTemplate = btnTemplate;


        //  gwwd testing ???
        templ.EditItemTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.BUTTONS, FIELDS_WHATIF[0], null);
        gridWhatifs.Columns.Add(templ);

        for (int i = 1; i < FIELDS_WHATIF.Length; i++) {
            TemplateField template = new TemplateField();
            GridViewTemplate itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS_WHATIF[i], LABEL_IDS[i]);
            template.HeaderText = HEADERS_WHATIF[i];

            template.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
                itemTemplate.InstantiateIn(container);
            },
                delegate(Control container) {
                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS_WHATIF.Length; j++) {
                        dict[FIELDS_WHATIF[j]] = ((Label)container.FindControl(LABEL_IDS[j])).Text;
                    }
                    return dict;
                });


            // edit mode
            GridViewTemplate editTemplate;
            if (i==1) {
                editTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS_WHATIF[i], TEXT_BOX_IDS[i]);

            } else {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, FIELDS_WHATIF[i], TEXT_BOX_IDS[i]);
            }

            template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
                editTemplate.InstantiateIn(container);
            },
                delegate(Control container) {
                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS_WHATIF.Length; j++) {
                        if (j == 1) {
                            dict[FIELDS_WHATIF[j]] = ((Label)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        } else {
                            dict[FIELDS_WHATIF[j]] = ((TextBox)container.FindControl(TEXT_BOX_IDS[j])).Text;
                            //xxx  ?? here 
                        }
                    }
                    return dict;
                });

            gridWhatifs.Columns.Add(template);
        }


    }


    private void CreateRunFile() {
        DbUse.CreateRunFile(GetDirectory() + userDir, username);
    }

    private void DeleteRunFile() {
        DbUse.DeleteRunFile(GetDirectory() + userDir, username);
    }

    protected void nameupdate_click(object sender, EventArgs e)
    {
          UpdateAnalysisData();
    }

 

    private bool UpdateAnalysisData()
    {
        string name = MyUtilities.clean(txtCaseName.Text);
        string comment = MyUtilities.clean(txtCaseComment.Text);

        if (name.Equals(String.Empty))
        {
            Master.ShowErrorMessage("Invalid name for Vision Analysis.");
            return false;
        }
        if (AnalysisExists(name, currentAnalysisId))
        {
            Master.ShowErrorMessage("Another Vision Analysis with the same name exists. Please select a different name.");
            return false;
        }
        bool ret;
        ret = UpdateSql("UPDATE tblAnalysis SET Name = '" + name + "', Comment = '" + comment + "' WHERE AnalysisID = " + currentAnalysisId + ";");

        if (ret == false) return false;

        currentAnalysisName = name;
        Master.PassCurrentAnalysisName(name);
        Master.SetCurrentAnalysisLabel();
        UpdateSql("UPDATE zstblstate SET AnalysisID = " + currentAnalysisId + ";");

        SetAnalysisListData();

        return true;
    }



    protected void btnRun_Click(object sender, EventArgs e) {
        continueaction = Action.RUN;
        if (currentWhatifId != 0) {

            continueaction = Action.RUN;
            btnWantSaveWhatif.OnClientClick = "HideWantSaveModel(); startProgressCheck(); $find('modalPopupLoadingBehavior').show(); return true;";
            btnDontSaveWhatif.OnClientClick = "HideWantSaveModel(); startProgressCheck(); $find('modalPopupLoadingBehavior').show(); return true;";
            Master.SetFocus(btnWantSaveWhatif.ClientID);
            modalPopupWantSaveWhatif.Show();
        }
        else if (UpdateAnalysisData())   
            //  not needed && SaveAnalysisSettings()
        {   
            try {
                RunAnalysis();
                btnShowAnalysis_Click0();
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage(ex.Message);
            }

        }
    }


    


    public void ShowErrorMessage(string message)
    {
        lblGeneralError.Text = message;
        modalExtenderError.Show();
        SetFocus(btnOkError.ClientID);

       
       
    }

    private void xxdeleteOldPoints(long curveid) { 

        List<int> pointsNotUsed = new List<int>();
        ClassF classF = new ClassF(GetDirectory() + userDir);
        ADODB.Recordset recPoints = new ADODB.Recordset();
        bool recPointsOpened = DbUse.OpenAdoRec(classF.globaldb, recPoints, "SELECT WID FROM tblAnalysisPoints WHERE AnalysisID = " + currentAnalysisId + " AND curveid = "+curveid +";");
        if (recPointsOpened) {
            while (!recPoints.EOF) {
                pointsNotUsed.Add((int)recPoints.Fields["WID"].Value);
                recPoints.MoveNext();
            }
            DbUse.CloseAdoRec(recPoints);
        }
        foreach (int whatifPoint in pointsNotUsed) {
            classF.DeleteWhatIfAudit(whatifPoint);
        }

    }

    private int Update_data(string pointValue)
    {

        string[] properties = pointValue.Split(';');
        string table;
        string itemIdDesc;
        string itemDeptDesc;
        string field;

        bool multiplier = false;
        string command = "";

        foreach (string property in properties)
        {
            string[] change = property.Split(':');
            string type = change[0];
            if (type.Equals(PROD.ToString()))
            {
                table = "tblProdfore";
                itemIdDesc = "ProdID";
                itemDeptDesc = "ProdDept";
                field = "DemandFac";
                multiplier = true;

            }
            else if (type.Equals(LABOR.ToString()))
            {
                table = "tblLabor";
                itemIdDesc = "LaborID";
                itemDeptDesc = "LaborDept";
                field = "GrpSiz";
            }
            else if (type.Equals(EQUIP.ToString()))
            {
                table = "tblEquip";
                itemIdDesc = "EquipID";
                itemDeptDesc = "EquipDept";
                field = "GrpSiz";
            }
            else
            {
                break;
            }
            string scope = change[1];
            string num;
            if (scope.Equals(ITEM))
            {
                string id = change[2];
                num = change[3];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num + " WHERE " + itemIdDesc + " = " + id + ";" :
                    "UPDATE " + table + " SET " + field + " = " + num + " WHERE " + itemIdDesc + " = " + id + ";";
            }
            else if (scope.Equals(DEPT))
            {
                string dept = change[2];
                num = change[3];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num + " WHERE " + itemDeptDesc + " = '" + dept + "';" :
                    "UPDATE " + table + " SET " + field + " = " + num + " WHERE " + itemDeptDesc + " = '" + dept + "';";
            }
            else if (scope.Equals(ALL))
            {
                num = change[2];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num : "UPDATE " + table + " SET " + field + " = " + num;
                if (!type.Equals(PROD.ToString()))
                {
                    command += " WHERE <> 'NONE'";
                }
                command += ";";

            }
            else
            {
                break;
            }
            UpdateSql(command);
            
        }
        return -1;
    }
       
    private void RunAnalysis() {

        ClassF analClass = new ClassF(GetDirectory() + userDir); 
        ClassA classA_act = new ClassA(GetDirectory() + userDir);
        int curvewid;
        int curveid;
        float xvalue;
        float xTo;
        float xStep;
        int whatifId = 0;
        string whatifName = "";
        int i;
        string curveName = "";
        string pointValue;



        if (classA_act.Calc_count() ==0) {
            Master.ShowErrorMessage("No products are defined yet. Vision Analysis cannot be done.");
            Response.Redirect("analysis.aspx");
            return;
        }

        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
 
        UpdateSql("UPDATE zstblstate SET AnalysisID = " + currentAnalysisId + ";");

        //  reload currentwhatif  at end !!!!
        /// gwwd 

            // check on running all curves / whatifs associated with the analysis
         string str1 = " SELECT tblanalysiscurve.AnalysisID, Count(tblanalysiscurve.curveid) AS Count1 FROM tblanalysiscurve GROUP BY tblanalysiscurve.AnalysisID HAVING (((tblanalysiscurve.AnalysisID)=" + currentAnalysisId + "));";
            ADODB.Recordset recCurve0 = new ADODB.Recordset();
            bool recOpened = DbUse.OpenAdoRec(analClass.globaldb, recCurve0, str1);

            if (!recOpened )
            {
                DbUse.CloseAdoRec(recCurve0);
                DbUse.CloseAdo(conn);
                Master.ShowErrorMessage("Error reading Vision Analysis What-If Scenario data.");
                return;
            }
            

        if ( (int)recCurve0.Fields["count1"].Value == 0)  
        {
            Master.ShowErrorMessage("No whatif/basecase scenarios/Curves are included in the Vision Analysis yet.");
            Response.Redirect("analysis.aspx");
            recCurve0.Close ();
            return;
        }
        recCurve0.Close();


        
        if (DbUse.InRunProcess(analClass.localdir)) {
            logFiles.DuplicateRunEndLog();
            throw new Exception("Cannot calculate Vision Analysis. The calculations are still in process from the previous run. Please wait.");
        }
        bool analysisSuccess = true;
        bool pointsOk = true;
        string cookieid = MyUtilities.clean(Request.Cookies[DbUse.LOGIN_COOKIE].Value);

        string analysisErrorMessage = "";
         try {
           

            clean_tblanalysispoints();  //  erase points/whatifs from this analysis (where recalc needed) before run

            CreateRunFile();
            
            // run all curves associated with the analysis as needed;
            ADODB.Recordset recCurve = new ADODB.Recordset();
             recOpened = DbUse.OpenAdoRec(analClass.globaldb, recCurve, "SELECT tblAnalysisCurve.* FROM tblAnalysisCurve WHERE (AnalysisID = " + currentAnalysisId + " and (recalc = true));");

            int totalCalc = 0;
            int currentCalc = 0;
            long lastTime = 0;
            float x1;
            while (!recCurve.EOF) {
                x1 = (float)recCurve.Fields["MainVarTo"].Value;
                x1 = (float)recCurve.Fields["MainVarFrom"].Value;
                x1 =  (float)recCurve.Fields["MainVarStep"].Value;
                int num = (int) ( .999 +    ((float)recCurve.Fields["MainVarTo"].Value - (float)recCurve.Fields["MainVarFrom"].Value) / (float)recCurve.Fields["MainVarStep"].Value);
                totalCalc+= num;
                recCurve.MoveNext();
            }
       

            if (totalCalc == 0)
            {
                 SetActiveView(1);
               
                 recCurve.Close();
                 lblNoResults.Visible = false;
                 Master.ShowInfoMessage("No calculations are needed for this Vision at this time.  The Vision Analysis calculations completed.");  
                 DeleteRunFile();
                 if (currentWhatifId != 0)
                 {
                     analClass.LoadWhatIf(currentWhatifId);
                 }
                  // SetGraphData();  //  needed ? gwwd 
                 return;
            }

            recCurve.MoveFirst();

            //  looping on curves 
            //  delete all old points/whatifs for this curve 
            //  looping on points ...
            //  create point 
            //  create whatif for point 
            //  run wid
            
       


          

             

             analClass.LoadBaseCase();  //  check wid  should be 0 already!!!

            while (!recCurve.EOF) {
            //  looping on curves 

             /*   //  don't need wid ???
                string wid1 = recCurve.Fields["Base_wid"].Value.ToString();
                int wid = Convert.ToInt32(wid1);
            */

                curveName = (string) recCurve.Fields["curvename"].Value;
                curveid = (int) recCurve.Fields["curveid"].Value;
                curvewid = (int) recCurve.Fields["Base_wid"].Value;
                
                xvalue = (float) recCurve.Fields["mainVarFrom"].Value;
                xTo = (float) recCurve.Fields["mainvarTo"].Value;
                xStep = (float) recCurve.Fields["mainvarStep"].Value;

                //  delete old points etc in curve
                //  do this at start ???
                //deleteOldPoints(curveid);

                //  start whatif ...
              
              

                i = 0;
               
                for (; xvalue < xTo; xvalue += xStep)
                {
                    i++;
                    //  looping on points ...
                    analClass.dowhatif_all_start();
                    analClass.LoadWhatIf(curvewid);

                   

                    if (totalCalc > 0)
                    {
                        DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.userid = '" + cookieid + "';");
                        DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = " + totalCalc + ", calc = " + currentCalc + ", lastCheck = " +  DateTime.Now.Ticks + ", cancel = 0 WHERE userlist.userid = '" + cookieid + "';");
                    }

                    if (CalcClass.CalculationsCancelled(cookieid))
                    {         //xxx  handle Whatifs situation 
                        analClass.dowhatif_all_end();
                        analClass.LoadBaseCase();
                        break;
                    }
                    currentCalc++;
                    long currTime = DateTime.Now.Ticks;
                    long timePerCalc = -1;
                    if (currentCalc > 1)
                    {
                        timePerCalc = currTime - lastTime;
                    }
                    DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET calc = " + currentCalc + ", lastCheck = " + currTime + ", timePerCalc = " + timePerCalc + " WHERE userlist. userid = '" + cookieid + "';");
                    lastTime = currTime;



                    //  Apply xvalue
                    //       save as new wid  
                    //       get newwid
                    //       do Calc
                    //

                    //  Apply xvalue
                    // string str2 = "";
                    // UpdateSql (str2);
                    pointValue = GetDatabaseField("mainVar", "analysisId", currentAnalysisId, "tblanalysis");
                    pointValue += ":" + xvalue.ToString();

                    Update_data(pointValue);
                     ///  applychange!!!;

                    //  save as new WID
                    whatifName = "Vision: " + currentAnalysisName + " Curve: " + curveName + " Point: " + i.ToString();

                    whatifId = analClass.addnewwhatif(whatifName, "Analysis Point What-If Scenario");
                    analClass.glngwid = whatifId;

                    addpoint( curveid, xvalue, whatifId);



                    analClass.saveWid();

                    // make sure the whatif won't show up in std output graphs/user whatifs list
                    UpdateSql("UPDATE tblWhatif SET FamilyID = " + 1 + ", display = 0 WHERE WID = " + whatifId + ";");


                    //       do Calc

                    CreateRunFile();
                    analClass.runsqlado("UPDATE tblWhatif SET recalc = true WHERE WID = " + whatifId + ";"); 
                    int ret1 = analClass.RunDLL();
                    analClass.runsqlado("UPDATE tblWhatif SET display = 0 WHERE WID = " + whatifId + ";"); // mark the whatifs as not visible in graphs
                    analClass.dowhatif_all_end();

                    // save whatif records  so we can loadbasecase !!!
                    analClass.SaveWhatIfAudit(analClass.glngwid);
                    analClass.LoadBaseCase();



                    try
                    {
                        if (ret1 != -1)
                        {
                            //string currWhatifPointName = analClass.Get_whatif_name(whatifName);
                            analysisErrorMessage += "<br/>Error in calculations in What-If point " + whatifName + ".";
                            throw new Exception("Errors/warnings in vision analysis calculations." + Master.GetFullCurrentAnalysisLabel() + " (id " + currentAnalysisId + "), Whatif point: " + whatifName + " (id " + whatifId + "). ");
                        }
                    }
                    catch (Exception excep)
                    {
                        pointsOk = false;
                        logFiles.ErrorLog(excep);
                    }

                } // end loop on points

                bool test1 = (bool) recCurve.Fields["recalc"].Value;
                recCurve.Fields["recalc"].Value = false;
                recCurve.Update();

            recCurve.MoveNext();
               
           // UpdateSql("UPDATE tblanalysiscurve SET [recalc] = false WHERE (((tblanalysiscurve.Curvename)='" + curveName + "') AND ((tblanalysiscurve.AnalysisID)=" + currentAnalysisId+"));");
                       
            }  //  end curve loop all curves in this vision


           
                  
            DbUse.CloseAdoRec(recCurve);

            //  return to wid at start of run...

            if (currentWhatifId != 0) {
                analClass.LoadWhatIf(currentWhatifId);
            }
        } catch (Exception ex) {
            analysisSuccess = false;
            logFiles.ErrorLog(ex);
        }
        DeleteRunFile();
        analClass.Close();

        if (currentWhatifId != 0)
        {
            analClass.LoadWhatIf(currentWhatifId);
        }
        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.userid = '" + cookieid + "';"); 
        if (!analysisSuccess) {
            throw new Exception("An internal error has occured during the calculations. Some of the calculations might not have finished.");
        } else if (!pointsOk) {
            //throw new Exception("There were errors in calculations. " + analysisErrorMessage + " Please go tu run page and run model and see the errors/results.");
            throw new Exception("Vision analysis has warnings/errors from some of the calculations. Analysis continued on where possible.");
        } else {
            SetActiveView(1);
            lblNoResults.Visible = false;
            Master.ShowInfoMessage("The Vision Analysis calculations completed.");
        }

        
        return;

    }

    private const int MAX_ANALYSIS_POINTS = 12;

   
 
    public string GetVariableDescription(string properties, string description) {
        string descriptionStart = "";
        string table = "";
        string idName = "";
        string descName = "";

        string[] property = properties.Split(':');
        string type = property[0];
        if (type.Equals(PROD.ToString())) {
            descriptionStart = "Product";

            table = "tblProdfore";
            idName = "ProdID";
            descName = "ProdDesc";

        } else if (type.Equals(LABOR.ToString())) {
            descriptionStart = "Labor";

            table = "tblLabor";
            idName = "LaborID";
            descName = "LaborDesc";

        } else if (type.Equals(EQUIP.ToString())) {
            descriptionStart = "Equipment";

            table = "tblEquip";
            idName = "EquipID";
            descName = "EquipDesc";
        }
        string scope = property[1];
        string name = "";
        string num = "";
        if (scope.Equals(ITEM)) {
            string id = property[2];
            name = GetDatabaseField(descName, idName, int.Parse(id), table) + " ";
            if (property.Length > 3) {
                num = property[3];
            }
        } else if (scope.Equals(DEPT)) {
            name = "Department " + property[2] + " ";
            if (property.Length > 3) {
                num = property[3];
            }
        } else if (scope.Equals(ALL)) {
            name = "";
            if (property.Length > 2) {
                num = property[2];
            }
            descriptionStart = "All " + descriptionStart + (type.Equals(PROD.ToString()) ? "s " : " ");
        }
        if (description.ToUpper().Equals("UTILIZATION")) {
            description += " %";
        }
        return descriptionStart + " " + name + description + " " + num;
    }

    public string GetVariableDescription(string properties) {
        string[] property = properties.Split(':');
        string type = property[0];
        string description = "";
        if (type.Equals(PROD.ToString())) {
            description = "Demand Factor";
        } else if (type.Equals(LABOR.ToString())) {
            description = "Group Size";
        } else if (type.Equals(EQUIP.ToString())) {
            description = "Group Size";
        }
        return GetVariableDescription(properties, description);

    }

    private void addpoint(int curveid, float mvalue, int wid)
    {

        string str1 = "INSERT INTO tblanalysispoints ( AnalysisID, Curveid, MainVarValue, WID ) SELECT " + currentAnalysisId + " AS Expr1, " + curveid + " AS Expr2, " + mvalue + " AS Expr3, " + wid + " AS Expr4";

        UpdateSql(str1);

        return;
    }
        


    private int xxCreateAnalysisWhatifPoint(string mainValue, string parameter, string pointName, string curve, int initWID) {
        // assume starting from curveWID  return new wid 

        ///  xxx  creating points ???
        ///  
        // get the main variable type and value
        string pointValue = GetDatabaseField("MainVar", "AnalysisID", currentAnalysisId, "tblAnalysis");
        pointValue += ":" + mainValue;

        string whatifName = pointName;
        Class2 classE1_1 = new Class2(GetDirectory() + userDir);

        if (initWID != 0) {
            // load initial whatif
            classE1_1.LoadWhatIf(initWID);
        }
        int whatifId = classE1_1.addnewwhatif(whatifName, "Analysis Point What-If Scenario");
        classE1_1.glngwid = whatifId;
        classE1_1.saveWid();

        // make sure the whatif won't show up in std output graphs/user whatifs list
        classE1_1.runsqlado("UPDATE tblWhatif SET FamilyID = " + 1 + ", display = 0 WHERE WID = " + whatifId + ";");

        // add the curve point to tblAnalysisPoints
        // classE1_1.runsqlado("INSERT INTO tblAnalysisPoints (WID, AnalysisID, MainVarValue, Parameter, Curveid, curveWID, recalc) VALUES " + " (" + whatifId + ", " + currentAnalysisId + ", " + mainValue + ", '" + parameter + "', " + curve + ", " + initWID + ", -1 ;");
        classE1_1.runsqlado("INSERT INTO tblAnalysisPoints (WID, AnalysisID, MainVarValue,  Curveid, curveWID, recalc) VALUES " + " (" + whatifId + ", " + currentAnalysisId + ", " + mainValue + ", " + curve + ", " + initWID + ", -1 ;");

        string[] properties = pointValue.Split(';');
        string table;
        string itemIdDesc;
        string itemDeptDesc;
        string field;

        bool multiplier = false;
        string command = "";

        foreach (string property in properties) {
            string[] change = property.Split(':');
            string type = change[0];
            if (type.Equals(PROD.ToString())) {
                table = "tblProdfore";
                itemIdDesc = "ProdID";
                itemDeptDesc = "ProdDept";
                field = "DemandFac";
                multiplier = true;

            } else if (type.Equals(LABOR.ToString())) {
                table = "tblLabor";
                itemIdDesc = "LaborID";
                itemDeptDesc = "LaborDept";
                field = "GrpSiz";
            } else if (type.Equals(EQUIP.ToString())) {
                table = "tblEquip";
                itemIdDesc = "EquipID";
                itemDeptDesc = "EquipDept";
                field = "GrpSiz";
            } else {
                break;
            }
            string scope = change[1];
            string num;
            if (scope.Equals(ITEM)) {
                string id = change[2];
                num = change[3];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num + " WHERE " + itemIdDesc + " = " + id + ";" :
                    "UPDATE " + table + " SET " + field + " = " + num + " WHERE " + itemIdDesc + " = " + id + ";";
            } else if (scope.Equals(DEPT)) {
                string dept = change[2];
                num = change[3];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num + " WHERE " + itemDeptDesc + " = '" + dept + "';" :
                    "UPDATE " + table + " SET " + field + " = " + num + " WHERE " + itemDeptDesc + " = '" + dept + "';";
            } else if (scope.Equals(ALL)) {
                num = change[2];
                command = multiplier ? "UPDATE " + table + " SET " + field + " = " + field + " * " + num : "UPDATE " + table + " SET " + field + " = " + num;
                if (!type.Equals(PROD.ToString())) {
                    command += " WHERE <> 'NONE'";
                }
                command += ";";

            } else {
                break;
            }
            classE1_1.runsqlado(command);
        }

        // refresh whatif records
        classE1_1.dowhatif_all_end();
        // save whatif records
        classE1_1.SaveWhatIfAudit(whatifId);
        classE1_1.LoadBaseCase();
        whatifId = 0;
        //  save to state ? 
        classE1_1.saveWid();
        
        classE1_1.dowhatif_all_start();
        classE1_1.Close();

        return whatifId;

    }

    private void aaDeleteCurrentAnalysisPoint(int whatifId) {
        ClassF classE1_1 = new ClassF(GetDirectory() + userDir);
        try {
            classE1_1.Open();
            //today test if Wid2 = -1 or = 0 
            classE1_1.DeleteWhatIfAudit(whatifId);

            classE1_1.model_modified = -1;
            classE1_1.saveModel_modified();
            classE1_1.Close();
        } catch (Exception ex) {
            classE1_1.Close();
            logFiles.ErrorLog(ex);
        }
    }

    private void SetWhatifsInOut() {
     
        lstWhatifOut.Items.Clear();
        dtWhatifsIn.Rows.Clear();

        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        ADODB.Recordset recExistingCurve = new ADODB.Recordset();

           //  UPdating widname - curve name....
        string str1 = "UPDATE tblanalysiscurve INNER JOIN zstblwhatif ON tblanalysiscurve.Base_wid = zstblwhatif.WID SET tblanalysiscurve.Curvename = [zstblwhatif].[name]; ";

        UpdateSql(str1);

      
        

       //    load whatgrid on right - list of curves in the model....

        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        bool recOpened = DbUse.OpenAdoRec(conn, rec, "SELECT zstblWhatif.* FROM zstblWhatif WHERE FamilyID = 0;");
        
        bool recCurveOpened = DbUse.OpenAdoRec(conn, recExistingCurve, "SELECT base_WID, MainVarFrom, MainVarTo, MainVarStep, AnalysisID FROM tblAnalysiscurve WHERE AnalysisID = " + currentAnalysisId + " ORDER BY base_wid;"); 
        if (!adoOpened || !recOpened || !recCurveOpened) {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdoRec(recExistingCurve);
            DbUse.CloseAdo(conn);
            Master.ShowErrorMessage("Error reading Vision Analysis What-If Scenario data.");
            return;
        }
        bool noWhatifsIn = false;
        int whatifId = 0;

        try {
            //  all whatifs !!!
            while (!rec.EOF) {
                whatifId = (int)rec.Fields["WID"].Value;

               
                string whatifName;
                if (whatifId == 0) {
                    whatifName = "Base Case";
                } else {
                    whatifName = (string)rec.Fields["Name"].Value;
                }

                try {
                    //xxx  ?  if empty ???
                /*    string field = GetDatabaseField("curveid", "base_WID", whatifId, "tblAnalysisPoints");
                    recExistingCurve.Filter = "curveWID = " + whatifId;
                    if (recExistingCurve.EOF) {
                        recExistingCurve.Filter = "";
                        throw new Exception("not found");
               */    //  } else {
                        // curves into right side list ...
                    //  if NOT  empty ...
                    
               //  find if analysisid in curves table...  if analysisid 
                    string str2 = "SELECT base_WID, MainVarFrom, MainVarTo, MainVarStep, AnalysisID FROM tblAnalysiscurve WHERE ((AnalysisID =  " + currentAnalysisId
                         + ") And  (Base_WID =  " + whatifId + ")) ;";
                    DbUse.OpenAdoRec(conn, recExistingCurve, str2);
                    bool test = recExistingCurve.EOF;
                    bool test2 = recExistingCurve.BOF;
                      if (!test)
                      {
                          DataRow row = dtWhatifsIn.NewRow();

                          //  update mainvar on screen !!!
                          dropListProduct.SelectedValue = (string)GetDatabaseField("MainVar", "AnalysisID", currentAnalysisId, "tblAnalysis");

                         
                          row[FIELDS_WHATIF[0]] = currentAnalysisId;
                          // row[FIELDS_WHATIF[1]] = whatifId;
                          row[FIELDS_WHATIF[1]] = whatifName;
                          row[FIELDS_WHATIF[2]] = recExistingCurve.Fields[FIELDS_WHATIF[2]].Value;
                          row[FIELDS_WHATIF[3]] = recExistingCurve.Fields[FIELDS_WHATIF[3]].Value;
                          row[FIELDS_WHATIF[4]] = recExistingCurve.Fields[FIELDS_WHATIF[4]].Value;
                          dtWhatifsIn.Rows.Add(row);
                      }
                      else
                      {
                          ListItem item = new ListItem();
                          item.Text = whatifName;
                          lstWhatifOut.Items.Add(item);
                      }
                   
                } catch (Exception exep) {
                    if (exep.Message.IndexOf("not found") >= 0) {
                        if (noWhatifsIn && currentWhatifId == whatifId) {
                            //lstWhatifIn.Items.Add(new ListItem(whatifName, whatifId + ""));
                            DataRow row = dtWhatifsIn.NewRow();
                            row[FIELDS_WHATIF[0]] = currentAnalysisId;
                           //ow[FIELDS_WHATIF[1]] = whatifId;  //  check on this   start analkysysid  ???
                            row[FIELDS_WHATIF[1]] = whatifName;
                            row[FIELDS_WHATIF[2]] = DEFAULT_MAIN_VAR_VALUES[0];
                            row[FIELDS_WHATIF[3]] = DEFAULT_MAIN_VAR_VALUES[1];
                            row[FIELDS_WHATIF[4]] = DEFAULT_MAIN_VAR_VALUES[2];
                            dtWhatifsIn.Rows.Add(row);

                            /* adding line to tblanacur
                             *  ClassE classE_act = new ClassE(GetDirectory() + userDir);
                int whatifId = classE_act.get_wid(item.Text);
                string str1 = "INSERT INTO tblanalysiscurve ( AnalysisID, Base_wid, Curvename, MainVarFrom, MainVarStep, MainVarTo ) SELECT "+currentAnalysisId +" AS Expr1, " + whatifId +" AS Expr2, '"+item.Text + "' AS Expr3, "+ DEFAULT_MAIN_VAR_VALUES[0] +" AS Expr4, "+ DEFAULT_MAIN_VAR_VALUES[1] +" AS Expr14, "+ DEFAULT_MAIN_VAR_VALUES[2] +" AS Expr24 ;";
                    UpdateSql(str1);
                             * */
               

                        } else {
                            lstWhatifOut.Items.Add(new ListItem(whatifName, whatifId + ""));
                        }
                    }
                }
                rec.MoveNext();

            }
        }
        catch (Exception ex)    {
            Master.ShowErrorMessage("An error has occured in setting What-If Scenarios included in Vision Analysis." + ex.Message);
        }
        DbUse.CloseAdoRec(recExistingCurve);
        DbUse.CloseAdoRec(rec);
        DbUse.CloseAdo(conn);

        try {
            gridWhatifs.DataSource = dtWhatifsIn;
            gridWhatifs.DataBind();
        } catch (Exception exp) {
            Master.ShowErrorMessage("An error has occured in setting What-If Scenarios included in Vision Analysis.");
        }

    }

    protected bool AnalysisExists(string analysisName, int analysisId) {
        bool exists = false;
        for (int i = 0; i < lstCases.Items.Count && !exists; i++) {
            if (lstCases.Items[i].Text.ToUpper().Equals(analysisName.ToUpper()) && !lstCases.Items[i].Value.Equals(analysisId + "")) {
                exists = true;
                break;
            }
        }
        return exists;
    }

    protected bool AnalysisExists(string analysisName) {
        bool exists = false;
        for (int i = 0; i < lstCases.Items.Count && !exists; i++) {
            if (lstCases.Items[i].Text.ToUpper().Equals(analysisName.ToUpper())) {
                exists = true;
                break;
            }
        }
        return exists;
    }

    private const string ALL = "-1";
    private const string ITEM = "1";
    private const string DEPT = "0";

    private const short LABOR = ClassA.Labor;
    private const short EQUIP = ClassA.equip;
    private const short PROD = ClassA.product;



    private void SetupVariableLists() {
        SetupVariableList(PROD.ToString(), false);
        SetupVariableList(LABOR.ToString(), true);
        SetupVariableList(EQUIP.ToString(), true);
        SetupVariableList(PROD.ToString(), true);

        // output product list
        SetupVariableList(PROD.ToString(), dropListOutputProductType);
        SetupVariableList(LABOR.ToString(), dropListOutputLaborType);
        SetupVariableList(EQUIP.ToString(), dropListOutputEquipType);
    }

    private void SetupVariableList(string type, DropDownList dropListItem, string[] fields, string table, string itemDesc) {
        string commItem = "SELECT " + fields[0] + ", " + fields[1] + " FROM " + table + " WHERE " + fields[1] + " <> 'NONE' ORDER BY " + fields[1] + ";";
        string commDept = "SELECT DISTINCT " + fields[2] + " FROM " + table + " ORDER BY " + fields[2] + ";";
        using (OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";")) {
            OleDbCommand cmdEquip = new OleDbCommand(commItem, connec);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmdEquip);

            OleDbCommand cmdDept = new OleDbCommand(commDept, connec);
            OleDbDataAdapter adapterDept = new OleDbDataAdapter(cmdDept);
            {
                try {
                    connec.Open();
                    dropListItem.Items.Clear();
                    if (itemDesc.Equals("Product")) {
                        dropListItem.Items.Add(new ListItem("All " + itemDesc + "s", type + ":" + ALL));
                    } else {
                        dropListItem.Items.Add(new ListItem("All " + itemDesc, type + ":" + ALL));
                    }

                    // add all product departments
                    DataTable dtDept = new DataTable();
                    adapterDept.Fill(dtDept);
                    for (int i = 0; i < dtDept.Rows.Count; i++) {
                        try {
                            dropListItem.Items.Add(new ListItem("Department - " + dtDept.Rows[i][fields[2]].ToString(), type + ":" + DEPT + ":" + dtDept.Rows[i][fields[2]].ToString()));
                        } catch (Exception) { }

                    }

                    // add all products
                    DataTable dtItem = new DataTable();
                    adapter.Fill(dtItem);

                    for (int i = 0; i < dtItem.Rows.Count; i++) {
                        try {
                            dropListItem.Items.Add(new ListItem(itemDesc + " - " + dtItem.Rows[i][fields[1]].ToString(), type + ":" + ITEM + ":" + dtItem.Rows[i][fields[0]].ToString()));
                        } catch (Exception) { }

                    }
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                }
            }
        }
    }

    private void SetupVariableList(string type, DropDownList dropListToFill) {
        string table = "";
        string itemDesc = "";
        string[] fields;
        int itemType = int.Parse(type);

        switch (itemType) {
            case LABOR:
                fields = new string[] { "LaborID", "LaborDesc", "LaborDept" };
                table = "tblLabor";
                itemDesc = "Labor";
                break;
            case EQUIP:
                fields = new string[] { "EquipID", "EquipDesc", "EquipDept" };
                table = "tblEquip";
                itemDesc = "Equipment";
                break;
            case PROD:
                fields = new string[] { "ProdID", "ProdDesc", "ProdDept" };
                table = "tblProdfore";
                itemDesc = "Product";
                break;
            default:
                return;
        }
        SetupVariableList(type, dropListToFill, fields, table, itemDesc);
    }

    private void SetupVariableList(string type, bool optional) {

        DropDownList dropListItem = null;
        int itemType = int.Parse(type);

        switch (itemType) {
            case LABOR:
                dropListItem = optional ? dropListLaborOpt : dropListLabor;
                break;
            case EQUIP:
                dropListItem = optional ? dropListEquipOpt : dropListEquip;
                break;
            case PROD:
                dropListItem = optional ? dropListProdOpt : dropListProduct;
                break;
            default:
                return;
        }
        SetupVariableList(type, dropListItem);

    }

    protected void btnAddParameter_Click(object sender, EventArgs e) {
        string description = "Capacity";
        DropDownList dropList = null;
        if (rdbtnOptLabor.Checked) {
            dropList = dropListLaborOpt;
        } else if (rdbtnOptEquip.Checked) {
            dropList = dropListEquipOpt;
        } else if (rdbtnOptProduct.Checked) {
            dropList = dropListProdOpt;
            description = "Demand";
        } else {
            return;
        }
        string value = MyUtilities.clean(txtParameter.Text);

        try {
            int number = int.Parse(value);
            if (number < 0) {
                Master.ShowErrorMessage("Optional parameter must be greater or equal to 0.");
                return;
            }
            ListItem parameterItem = new ListItem(dropList.SelectedItem.Text + " " + description + " " + number, dropList.SelectedItem.Value + ":" + number);
            if (!lstParameters.Items.Contains(parameterItem)) {
                lstParameters.Items.Add(parameterItem);
            } else {
                Master.ShowErrorMessage("Cannot add the parameter because a parameter with the same value already exists in the Vision Analysis.");
            }
        } catch (Exception) {
            Master.ShowErrorMessage("Invalid optional parameter value.");
        }

    }

    protected void btnDeleteParameter_Click(object sender, EventArgs e) {
        if (lstParameters.SelectedItem == null) {
            Master.ShowErrorMessage("No parameter is selected for deletion.");
            return;
        }
        lstParameters.Items.Remove(lstParameters.SelectedItem);
    }
    protected void btnSort_Click(object sender, EventArgs e) {
        Master.ShowErrorMessage("Sorting option is under construction and currently unavailable.");
    }
    protected void btnCopy_Click(object sender, EventArgs e) {
        Master.ShowErrorMessage("Copy option is under construction and currently unavailable.");
    }
   

    protected void btnNewNameOk_Click(object sender, EventArgs e) {
        if (txtNewName != null) {
            currentAnalysisName = MyUtilities.clean(txtNewName.Text);
            int lastAnalysisId = currentAnalysisId;
            if (CreateNewAnalysis(currentAnalysisName))
            {
                txtNewName.Text = "";
                Master.PassCurrentAnalysisName(currentAnalysisName);
                txtCaseName.Text = currentAnalysisName;
                Master.SetCurrentAnalysisLabel();

                if (UpdateSql("UPDATE zstblstate SET AnalysisID = " + currentAnalysisId + ";")) { 
                    //
                    //
                    SetAnalysisListData();
                    Master.ShowInfoMessage("Vision Analysis '" + currentAnalysisName + "' created successfully."); 
                }
            } else if (!nameValid) {
                extenderNewName.Show();
            }
        }
    }

   

    protected override Panel GetCopyTableContainer() {
        return pnlCopyTableContainer;
    }

    public override Panel GetSortPanelContainer() {
        return pnlCopyTableContainer;
    }

    public override Control GetSortButtonContainer() {
        return pnlTableButtons;
    }

    protected override GridView getGridView() {
        return grid;
    }

    protected override Chart getChart() {
        return chart;
    }

    protected override void RefreshData() {
         SetGraphData();
    }

    protected void FillSortCombos(string[] headers) {
        for (int i = 0; i < comboSorts.Length; i++) {
            AjaxControlToolkit.ComboBox combo = comboSorts[i];
            string selectedText = null;
            string selectedValue = null;
            if (combo.SelectedItem != null) {
                selectedText = combo.SelectedItem.Text;
                selectedValue = combo.SelectedItem.Value;
                combo.SelectedIndex = -1;
            }
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Curve", "Curve"));
            combo.Items.Add(new ListItem(headers[0], "xValue"));
            combo.Items.Add(new ListItem(headers[1], "yValue"));

            if (selectedText != null) {
                int itemIndex = combo.Items.IndexOf(new ListItem(selectedText, selectedValue));
                if (itemIndex != -1) {
                    combo.SelectedIndex = itemIndex;
                }
            }
        }
    }



    protected string GetSortOrder() {
        List<string> orderingFields = new List<string>();
        for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
            if (comboSorts[i].SelectedValue != null && !comboSorts[i].SelectedValue.Trim().Equals("")) {
                string orderingField = comboSorts[i].SelectedValue;
                orderingField += (rdbtnSortAsc[i].Checked) ? " " : " desc";
                if (!orderingFields.Contains(orderingField)) {
                    orderingFields.Add(orderingField);
                }
            }
        }
        StringBuilder orderString = new StringBuilder("");
        if (orderingFields.Count > 0) {

            for (int i = 0; i < orderingFields.Count; i++) {
                orderString.Append(orderingFields.ElementAt(i));
                if (i != orderingFields.Count - 1) {
                    orderString.Append(", ");
                }
            }
        }
        return orderString.ToString();
    }


    protected override void SaveSortingOrder(AjaxControlToolkit.ComboBox[] combos, RadioButton[] radioAsc, string sortTableName) {
        return;
    }



    private string[] TEXT_BOX_IDS;
    private string[] LABEL_IDS;

    protected void gridWhatifs_RowEditing(object sender, GridViewEditEventArgs e) {

    }

    protected void gridWhatifs_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }

    protected void gridWhatifs_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }

    protected void gridWhatifs_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }

        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        if (e.CommandName.Equals("Update")) {
            RowUpdate(rowIndex);
            SetModelModified(true);
        } else if (e.CommandName.Equals("CancelUpdate")) {
            RowUpdateCancel();
        } else if (e.CommandName.Equals("Edit")) {
            gridWhatifs.EditIndex = rowIndex;
            try {
                gridWhatifs.DataSource = dtWhatifsIn;
                gridWhatifs.DataBind();
            } catch (Exception) { }
            try {
                //int _rowIndex = int.Parse(e.CommandArgument.ToString());
                int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                Control editControl = gridWhatifs.Rows[rowIndex].FindControl(TEXT_BOX_IDS[_columnIndex]) as Control;
                if (editControl != null && (editControl is TextBox)) {
                    editControl.Focus();

                }

            } catch (Exception) {

            }
            
        } else if (e.CommandName.Equals("Delete")) {
            RemoveRow(rowIndex);
        }
        try {
            Master.HideLoadingPopup();
        } catch (Exception) { }
    }

    private void RemoveRow(int rowIndex) {
        string whatifName = (string)gridWhatifs.DataKeys[rowIndex].Value;
    //    DataRow dataRow = dtWhatifsIn.Rows.Find(whatifId);  //  no primary key in table !!!
    //    if (dataRow != null) {
    //        whatifName = dataRow[FIELDS_WHATIF[1]].ToString();
    //    }
        try {
            //dtWhatifsIn.Rows.Remove(dataRow);
            //gridWhatifs.DataSource = dtWhatifsIn;
            //gridWhatifs.DataBind();
            //   Remove from tblanalysiscurve
            UpdateSql(" DELETE tblanalysiscurve.Curvename, tblanalysiscurve.AnalysisID, tblanalysiscurve.* FROM tblanalysiscurve WHERE (((tblanalysiscurve.Curvename)='" + whatifName +"') AND ((tblanalysiscurve.AnalysisID)=" +currentAnalysisId +"));");

            // rebuild  gridwhatifs 
            SetWhatifsInOut();
          
        } catch (Exception) { }

       // int selectedIndex = lstWhatifOut.SelectedIndex;
       // lstWhatifOut.Items.Add(new ListItem(whatifName, whatifName + ""));
       // lstWhatifOut.SelectedIndex = selectedIndex;

    }



    protected void gridWhatifs_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        grid.PageIndex = e.NewPageIndex;
        try {
            gridWhatifs.DataSource = dtWhatifsIn;
            gridWhatifs.DataBind();
        } catch (Exception) { }
    }

    protected void RowUpdateCancel() {
        gridWhatifs.EditIndex = -1;
        try {
            gridWhatifs.DataSource = dtWhatifsIn;
            gridWhatifs.DataBind();
        } catch (Exception) { }

    }

    protected void RowUpdate(int rowIndex) {
        GridViewRow gridRow = gridWhatifs.Rows[rowIndex];
        Control[] controls = new Control[TEXT_BOX_IDS.Length];

        float mainVarFrom = 0;
        float mainVarTo = 0;
        float mainVarStep = 0;
        string whatifName = null;
        string whatifName2 = null;
        ClassE classE_act = new ClassE(GetDirectory() + userDir);
      

        whatifName = (string)gridWhatifs.DataKeys[rowIndex].Value; // value of the datakey 
        int whatifID = classE_act.get_wid(whatifName);
        whatifName = classE_act.Get_whatif_name(whatifID);

        for (int i = 1; i < 5; i++) {
            controls[i] = gridRow.FindControl(TEXT_BOX_IDS[i]);
            if (controls[i] != null) {
                if (i == 1) {
                    whatifName2 = (controls[i] as Label).Text;
                } else {
                    try {
                        float value = float.Parse(MyUtilities.clean((controls[i] as TextBox).Text));
                        switch (i) {
                            case 2:
                                mainVarFrom = value;
                                break;
                            case 3:
                                mainVarTo = value;
                                break;
                            case 4:
                                mainVarStep = value;
                                break;
                            default:
                                break;
                        }

                    } catch (FormatException) {
                        Master.ShowErrorMessage("Invalid value for Column " + i+1 + ", What-If Scenario " + whatifName + ", field " + HEADERS_WHATIF[i] + ". ");
                        return;
                    }
                }
            }
        }

        if (mainVarFrom > mainVarTo) {
            Master.ShowErrorMessage("  From value is less than To value. (Min > Max)");
            return;
        }
        if (mainVarStep*.99 > (mainVarTo - mainVarFrom)) {
            Master.ShowErrorMessage("Step size is too big. It is bigger than min to max value.");
            return;
        }
        if ((mainVarTo - mainVarFrom)/mainVarStep > MAX_ANALYSIS_POINTS) {
            Master.ShowErrorMessage("Too many steps in What-If Scenario. The limit is " + MAX_ANALYSIS_POINTS);
            return;
        }
        try
        {
            DataRow dataRow = dtWhatifsIn.Rows.Find(whatifName); //  xxx no primary key ... ok ???
            if (dataRow != null)
            {
                dataRow["MainVarFrom"] = mainVarFrom;
                dataRow["MainVarTo"] = mainVarTo;
                dataRow["MainVarStep"] = mainVarStep;
            }
            string str1 = "UPDATE tblanalysiscurve SET recalc = true, tblanalysiscurve.MainVarFrom = " + mainVarFrom + ", tblanalysiscurve.MainVarTo = " + mainVarTo + ", tblanalysiscurve.MainVarStep = " + mainVarStep + " WHERE (((tblanalysiscurve.AnalysisID)= " + currentAnalysisId + ") AND ((tblanalysiscurve.Curvename)='" + whatifName + " '));"; 
                UpdateSql(str1);
           
            gridWhatifs.EditIndex = -1;
        }
        catch (Exception ex) { Master.ShowErrorMessage(ex.Message); }
        try {
            gridWhatifs.DataSource = dtWhatifsIn;
            gridWhatifs.DataBind();
        } catch (Exception) { }


    }

    protected void gridWhatifs_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT) as Button;
            string tooltip = "Double-click to edit What-If Scenario main variable settings";
            if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                tooltip = "Double-click to update What-If Scenario main variable settings";
                
            }

            if (btnToClick != null) {
                string btnJavascript = ClientScript.GetPostBackClientHyperlink(
                btnToClick, "");
                for (int columnIndex = 1; columnIndex <
                e.Row.Cells.Count; columnIndex++) {
                    // Add the column index as the event argument parameter
                    string js = btnJavascript.Insert(btnJavascript.Length - 2,
                        columnIndex.ToString());
                    // Add this javascript to the onclick Attribute of the cell
                    e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                    // Add a cursor style to the cells
                    e.Row.Cells[columnIndex].Attributes["style"] +=
                        "cursor:pointer;cursor:hand;";
                    e.Row.Cells[columnIndex].ToolTip = tooltip;
                    if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                        try {
                            Control control = e.Row.Cells[columnIndex].FindControl(TEXT_BOX_IDS[columnIndex]);
                            if (control is TextBox) {
                                (control as TextBox).Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                            }
                        } catch (Exception) { }
                    }

                }

                //e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Edit$" + e.Row.RowIndex);
                //e.Row.Attributes["style"] = "cursor:pointer";
                //e.Row.ToolTip = "Double-click to edit row";
            }

            e.Row.Cells[1].ForeColor = ColorTranslator.FromHtml("black");
            e.Row.Cells[1].Font.Bold = false;
            foreach (TableCell cell in e.Row.Cells) {
                cell.DataBind();
                try {

                    foreach (Control control in cell.Controls) {
                        foreach (Control control2 in control.Controls) {
                            if (control2 is Label) {
                                Label lbl = control2 as Label;
                                lbl.CssClass = "padding";
                                double num = Double.Parse(lbl.Text);
                                cell.HorizontalAlign = HorizontalAlign.Center;
                                break;
                            } else if (control2 is TextBox || control2 is AjaxControlToolkit.ComboBox || control2 is CheckBox) {
                                cell.HorizontalAlign = HorizontalAlign.Center;
                            }
                        }
                    }

                } catch (Exception) {

                }
            }
        }
    }

    protected DataTable dtWhatifsIn {
        get {
            if (ViewState["dtWhatifsIn"] == null) {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn(FIELDS_WHATIF[0], typeof(int)));  //anaid
               // dt.Columns.Add(new DataColumn(FIELDS_WHATIF[1], typeof(int)));   // basewid
                dt.Columns.Add(new DataColumn(FIELDS_WHATIF[1], typeof(string)));  //  widname
                dt.Columns.Add(new DataColumn(FIELDS_WHATIF[2], typeof(float)));   //from
                dt.Columns.Add(new DataColumn(FIELDS_WHATIF[3], typeof(float)));   //to
                dt.Columns.Add(new DataColumn(FIELDS_WHATIF[4], typeof(float)));   //step
               
               dt.PrimaryKey = new DataColumn[] { dt.Columns[1] };  //  ??  xxx correct ??

                ViewState["dtWhatifsIn"] = dt;
            }
            return (DataTable)ViewState["dtWhatifsIn"];
        }
        set { ViewState["dtWhatifsIn"] = value; }
    }
    protected void btnWantSaveWhatif_Click(object sender, EventArgs e) {
        //  close whatif 
        //  whatif save ....
        //  save current values
        //  load basecase //  same at user models loaad ...
             ClassF classF1_1 = new ClassF(GetDirectory() + userDir);
                    classF1_1.dowhatif_all_end(); //  stopped here if in wid at start of run....
                    // save whatif records
                    classF1_1.SaveWhatIfAudit(currentWhatifId);

                classF1_1.LoadBaseCase();
                classF1_1.Close();
        ContinueAction();  
        
    }
    protected void btnDontSaveWhatif_Click(object sender, EventArgs e) {
            //close whatif 
            //load basecase 
        ClassF classF1_1 = new ClassF(GetDirectory() + userDir);
        classF1_1.dowhatif_all_end(); //  stopped here if in wid at start of run....
      

        classF1_1.LoadBaseCase();
        classF1_1.Close();
        ContinueAction();
        
    }

    private void ContinueAction() {


        if (continueaction == Action.SAVE)
        {
            continueaction = "";
            UpdateAnalysisData();
            return;
        }
        if (continueaction == Action.RUN)  //  not saved 
        {      //  save whatif done

            continueaction = "";
            try {
                    RunAnalysis();
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                    Master.ShowErrorMessage(ex.Message);
                }
            
        }
        return;
    }

    

    private class Action {
        public const string SAVE = "0";
        public const string RUN = "1";
        public const string SAVE_AS = "2";
    }

    protected override bool LocalTablesLinked() {
        if (!UpdateSql("SELECT AnalysisID FROM tblAnalysis;")) {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblAnalysisPoints;")) {
            return false;
        }
        return true;
    }

    private void clean_tblanalysispoints()
    {

       string  str1 = "SELECT tblanalysiscurve.AnalysisID, tblanalysiscurve.recalc, tblanalysispoints.WID FROM tblanalysispoints INNER JOIN tblanalysiscurve ON tblanalysispoints.Curveid = tblanalysiscurve.curveid WHERE (((tblanalysiscurve.AnalysisID)="+currentAnalysisId +") AND ((tblanalysiscurve.recalc)=True));";

       List<int> pointsNotUsed = new List<int>();
       ClassF classF = new ClassF(GetDirectory() + userDir);
       ADODB.Recordset recPoints = new ADODB.Recordset();
       bool recPointsOpened = DbUse.OpenAdoRec(classF.globaldb, recPoints, str1);
       if (recPointsOpened)
       {
           while (!recPoints.EOF)
           {
               pointsNotUsed.Add((int)recPoints.Fields["WID"].Value);
               recPoints.MoveNext();
           }
           DbUse.CloseAdoRec(recPoints);
       }
       foreach (int whatifPoint in pointsNotUsed)
       {
           classF.DeleteWhatIfAudit(whatifPoint);
       }


       str1 = "DELETE tblanalysiscurve.AnalysisID, tblanalysiscurve.recalc, tblanalysispoints.* FROM tblanalysispoints INNER JOIN tblanalysiscurve ON tblanalysispoints.Curveid = tblanalysiscurve.curveid WHERE (((tblanalysiscurve.AnalysisID)="+currentAnalysisId +") AND ((tblanalysiscurve.recalc)=True));";
           UpdateSql(str1);



    }

     private bool xxSaveAnalysisSettings(bool saveCurrentWhatif) {
        // save current whatif
        if (currentWhatifId != 0) {
            // refresh whatif records
         
            ClassF classF1_1 = new ClassF(GetDirectory() + userDir);
            if (in_close_curr_wid != 0)
            {
                //  close current wid    save ? or not ? 

                if (in_close_curr_wid  == -1 )   // save wid changes ...
                {
                    //  save current values

                    classF1_1.dowhatif_all_end(); //  stooped here if in wid at start of run....
                    // save whatif records
                    classF1_1.SaveWhatIfAudit(currentWhatifId);
                    
                }

                //  close wid  
                classF1_1.LoadBaseCase();
                in_close_curr_wid = 0;
            }
           
            
            classF1_1.dowhatif_all_start();  //  here is misplaced  not -  ok now 7/1/16 !!!!
            
            currentWhatifId = 0;
            //save into state;
            classF1_1.saveWid();
            classF1_1.Close();
            Master.PassCurrentWhatifName("");
            Master.SetCurrentWhatifLabel();
        }



        List<int> pointsNotUsed = new List<int>();
        ClassF classF = new ClassF(GetDirectory() + userDir);
        ADODB.Recordset recPoints = new ADODB.Recordset();
        bool recPointsOpened = DbUse.OpenAdoRec(classF.globaldb, recPoints, "SELECT WID FROM tblAnalysisPoints WHERE AnalysisID = " + currentAnalysisId + " AND Used = 0;");
        if (recPointsOpened)
        {
            while (!recPoints.EOF)
            {
                pointsNotUsed.Add((int)recPoints.Fields["WID"].Value);
                recPoints.MoveNext();
            }
            DbUse.CloseAdoRec(recPoints);
        }
        foreach (int whatifPoint in pointsNotUsed)
        {
            classF.DeleteWhatIfAudit(whatifPoint);
        }

        // save all settings and create all analysis points (whatifs)
        string varFromString = MyUtilities.clean(txtMainVariableFrom.Text);
        string varToString = MyUtilities.clean(txtMainVariableTo.Text);
        string varStepString = MyUtilities.clean(txtMainVariableStep.Text);
        string mainVarCode = dropListProduct.SelectedItem.Value;

        //  erasing curves missing visions 
        UpdateSql("UPDATE tblanalysis RIGHT JOIN tblanalysiscurve ON tblanalysis.AnalysisID = tblanalysiscurve.AnalysisID SET tblanalysiscurve.analysisid = -1 WHERE (((IsNull([tblanalysis].[analysisid]))=True));");
        UpdateSql("DELETE tblanalysiscurve.* FROM tblanalysiscurve WHERE (((tblanalysiscurve.AnalysisID)=-1));");

        //  erasing points missing visions   
        UpdateSql("UPDATE tblanalysis RIGHT JOIN tblanalysispoints ON tblanalysis.AnalysisID = tblanalysispoints.AnalysisID SET tblanalysispoints.analysisid = -1 WHERE (((IsNull([tblanalysis].[analysisid]))=True));");
        UpdateSql("DELETE tblanalysispoints.* FROM tblanalysispoints WHERE (((tblanalysispoints.AnalysisID)=-1));");

        //  delete points missing curves 
        UpdateSql("UPDATE tblanalysiscurve RIGHT JOIN tblanalysispoints ON tblanalysiscurve.Curveid = tblanalysispoints.Curveid SET tblanalysispoints.curveid = -1 WHERE (((IsNull([tblanalysiscurve].[curveid]))=True));");
        UpdateSql("DELETE tblanalysispoints.* FROM tblanalysispoints WHERE (((tblanalysispoints.Curveid)=-1));");



        classF.model_modified = -1;
        classF.saveModel_modified();
        classF.Close();

        return true;
     }
    
}