using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_c : DbPage {

    ClassF classE1_1;

    public whatif_c() {
        PAGENAME = "whatif_c.aspx";
    }


    // GREG - please add dowhatif_all_start() and dowhatif_all_end() where applicable

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
        
        string str1 = "";
        short x1;
        int ret;

        //if (DbUse.InRunProcess(userDir)) {
        //    ShowWhatifs();
        //    Master.ShowErrorMessage("Cannot load information about What-If Scenarios because calculations are still running in the current model. Please wait and come back later.");
        //    return;
        //}
        try {
        classE1_1.Open();

        ret = classE1_1.testresetwid(classE1_1.glngwid);  ///  sees if glngwid has a record in tblwhatif ...
        if (ret == 0) {
            classE1_1.glngwid = 0;  //  duplicate here !!!
        }

        if (!Page.IsPostBack) {

            //  add test for glngwid in tblwhatif!!!!  else set to 0 
            //  when saving model, closing, ...  check if glngwid != 0   give user choice of save, quit or new whatif name

            ShowWhatifs();
            if (classE1_1.glngwid == 0) {
                classE1_1.dowhatif_all_start();
                MPXWhatfcontrolChoices.SetActiveView(PageR1);


            } else {

                set_page2_words();
                RefreshWhatifRecords();
                SetWhatifRecordsData();
                MPXWhatfcontrolChoices.SetActiveView(PageR2);
                //set words for whatif name, comment etc...
            }


        } else {

            updatingdatasource();
            SetWhatifRecordsData();
            /*     //  notupdating datasource!!!
             * 
                 AccessDataSource1.UpdateCommand = "SELECT [WID], [Name] FROM [tblWhatIf]";
                 AccessDataSource1.DataFile = classE1_1.varlocal + "\\mpxmdb.mdb";
                 AccessDataSource1.Update();
                 AccessDataSource1.DataBind();
                 AccessDataSource1.Update();
             */

        }
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }


    public void updatingdatasource() {
        return;
    }
    public void Buttonb1_Click(Object sender, System.EventArgs e) {
        
        try {
        classE1_1.Open();
        classE1_1.dowhatif_all_start();
        MPXWhatfcontrolChoices.SetActiveView(PageR1);
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }
    public void Buttonb2_Click(Object sender, System.EventArgs e) {
        
        try {
            classE1_1.Open();
            classE1_1.dowhatif_all_end();
            set_page2_words();

            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            classE1_1.Close();  
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }
    public void Buttonb3_Click(Object sender, System.EventArgs e) {
        
        try {
            classE1_1.Open();
            classE1_1.dowhatif_all_end();
            MPXWhatfcontrolChoices.SetActiveView(WhatifRecords);
            classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    public string postvalue(string varname) {

        string str1 = "";

        // getting value POST   get or post ??? ........................

        foreach (string query in Request.Form.AllKeys)     //  foreach (string query in Request.QueryString...
        {
            if (query.IndexOf(varname) >= 0) {
                str1 = Request.Form.Get(query);
                if (str1 != null) {
                    if (str1.Length > 0) { str1 = MyUtilities.clean(str1); }
                }
                return str1;
            }
        }

        return "";
    }


    string getvalue(string varname) {

        string str1 = "";

        //tomorrow today ? error !!!!!!!!   not getting value   get of post ??? ........................

        foreach (string query in Request.QueryString) {

            if (query.Equals(varname)) {
                str1 = (string)Request.QueryString.GetValues(query).GetValue(0);
            }


        }
        str1 = MyUtilities.clean(str1);

        return str1;
    }

    void set_page2_words() {
        try {
            Label4.Visible = false;
            string whatifName = classE1_1.Get_whatif_name(classE1_1.glngwid);
            widname.Text = whatifName;
            txtWhatifName.Text = whatifName;
            txtWhatifComment.Text = classE1_1.Get_whatif_comment(classE1_1.glngwid);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }


    public void Buttonr1_new_Click(Object sender, System.EventArgs e) {
        int wid2;
        string str1;
        
        try {
        classE1_1.Open();
        str1 = postvalue("TextBox17");

        str1 = MyUtilities.clean(str1);

        str1 = str1.Trim();
        if (str1.Length == 0) {
            TextBox17.Text = "no name !!";
            return;
        }

        if (classE1_1.glngwid != 0) {
            //  not in basecase ??
            set_page2_words();
            updatingdatasource();
            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            classE1_1.Close();
            return;
        }



        //  assume in base case !!!
        // else must close current whatif return to basecase ...

        str1 = str1.ToUpper();
        wid2 = classE1_1.addnewwhatif(str1, "new What-If Scenario");
        classE1_1.glngwid = wid2;
        classE1_1.saveWid();

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();




        TextBox17.Text = "";
        set_page2_words();
        updatingdatasource();
        MPXWhatfcontrolChoices.SetActiveView(PageR2);
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
        return;
    }
    public void Buttonr1_load_Click(Object sender, System.EventArgs e) {
        int wid2;
        string str1;
        
        try {
        classE1_1.Open();
        //classE1_1.setGlobalVar();  //  sets glngwid etc. 


        int x3;
        x3 = Convert.ToInt32(DropDownList1.SelectedValue);

        wid2 = x3;

        classE1_1.LoadWhatIf(wid2);

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();

        set_page2_words();
        updatingdatasource();
        MPXWhatfcontrolChoices.SetActiveView(PageR2);
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    public void Buttonr1_del_Click(Object sender, System.EventArgs e) {
        int wid2;
        string str1;
        
        try {
        classE1_1.Open();
        //classE1_1.setGlobalVar();  //  sets glngwid etc. 



        //str1 = MyUtilities.clean(getvalue("DropDownList2");
        int x3;
        x3 = Convert.ToInt32(DropDownList2.SelectedItem.Value);  ///  use as wid!!!



        //today test if Wid2 = -1 or = 0 
        classE1_1.DeleteWhatIfAudit(x3);

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();


        updatingdatasource();
        classE1_1.Close();


        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    public void Button_replace_Click(Object sender, System.EventArgs e) {
        
        int wid2;
        string str1;
        try {
        classE1_1.Open();
        //  add code    del current whatif results, //zstblwhatifaudit, clear glngwid, set recalc / save indicators 
        wid2 = classE1_1.glngwid;

        // replace results
        classE1_1.runsqlado("DELETE tblRsEquip.WID, tblRsEquip.* FROM tblRsEquip WHERE (((tblRsEquip.WID)=0));");
        classE1_1.runsqlado("DELETE tblRsProd.WID, tblRsProd.* FROM tblRsProd WHERE (((tblRsProd.WID)=0));");
        classE1_1.runsqlado("DELETE tblRsSummary.WID, tblRsSummary.* FROM tblRsSummary WHERE (((tblRsSummary.WID)=0));");
        classE1_1.runsqlado("DELETE tblRsOper.WID, tblRsOper.* FROM tblRsOper WHERE (((tblRsOper.WID)=0));");
        classE1_1.runsqlado("DELETE tblRsLabor.WID, tblRsLabor.* FROM tblRsLabor WHERE (((tblRsLabor.WID)=0));");

        classE1_1.runsqlado("UPDATE tblRsEquip SET tblRsEquip.WID = 0, tblRsEquip.Whatif = 'Base Case' WHERE (((tblRsEquip.WID)=" + wid2 + ")); ");
        classE1_1.runsqlado("UPDATE tblRsLabor SET tblRsLabor.WID = 0, tblRsLabor.Whatif = 'Base Case' WHERE (((tblRsLabor.WID)=" + wid2 + ")); ");
        classE1_1.runsqlado("UPDATE tblRsOper SET tblRsOper.WID = 0, tblRsOper.Whatif = 'Base Case' WHERE (((tblRsOper.WID)=" + wid2 + ")); ");
        classE1_1.runsqlado("UPDATE tblRsSummary SET tblRsSummary.WID = 0, tblRsSummary.Whatif = 'Base Case' WHERE (((tblRsSummary.WID)=" + wid2 + ")); ");
        classE1_1.runsqlado("UPDATE tblRsProd SET tblRsProd.WID = 0, tblRsProd.Whatif = 'Base Case' WHERE (((tblRsProd.WID)=" + wid2 + ")); ");

        classE1_1.DeleteWhatIfAudit(wid2);
        str1 = "DELETE [ZstblwhatifAudit].* FROM [ZstblwhatifAudit];";
        classE1_1.runsqlado(str1);

        

        classE1_1.glngwid = 0;
        classE1_1.saveWid();
        classE1_1.saveRecalcNeeded(0, -1);
        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();
        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        updatingdatasource();
        ShowWhatifs();
        classE1_1.dowhatif_all_start();
        classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = 0;");
        MPXWhatfcontrolChoices.SetActiveView(PageR1);
        classE1_1.Close();

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    public void Button_edit_Click(Object sender, System.EventArgs e) {        // edit what if name/comment 
        int lngid;
        string whatifName;
        string whatifComment;
        try {
        classE1_1.Open();

        lngid = classE1_1.glngwid;

        whatifName = txtWhatifName.Text;
        whatifName = MyUtilities.clean(whatifName).Trim();

        whatifComment = txtWhatifComment.Text;
        whatifComment = MyUtilities.clean(whatifComment);  //  

        if (whatifName.Length == 0) {
            //Label4.Text = "Save name/comment not saved.  No name was provided. ";
            //Label4.Visible = true;
            Master.ShowErrorMessage("Cannot save name and comment. The name cannot be empty.");
        } else if (!GetCurrentWhatif().ToUpper().Equals(whatifName.ToUpper()) && WhatifExists(whatifName)) {
            Master.ShowErrorMessage("Cannot save name and comment because a whatif with the same name already exists. Please choose a different whatif name.");
        } else {

            classE1_1.updatewidname(lngid, whatifName, whatifComment);
            Master.PassCurrentWhatifName(whatifName);
            Master.SetCurrentWhatifLabel();
            set_page2_words();
            updatingdatasource();
            ShowWhatifs();
        }
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    public void Button_endedit_Click(Object sender, System.EventArgs e) {
        try {
        classE1_1.Open();
        set_page2_words();
        MPXWhatfcontrolChoices.SetActiveView(PageR2);
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
        
    }


    public void Button_edit_rec_Click(Object sender, System.EventArgs e) {        // see  what if records
        MPXWhatfcontrolChoices.SetActiveView(WhatifRecords);
        if (gridWhatifRecords.Rows.Count == 0) {
            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            Master.ShowErrorMessage("There are no whatif records to view.");
        }
    }


    public void Button_save_Click(Object sender, System.EventArgs e) {
        
        int lngid;
        try {
        classE1_1.Open();

        lngid = classE1_1.glngwid;

        classE1_1.SaveWhatIfAudit(lngid);
        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();


        set_page2_words();
        MPXWhatfcontrolChoices.SetActiveView(PageR2);
        classE1_1.Close();

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    public void Button_saveClose_Click(Object sender, System.EventArgs e) {
        int lngid;
        string str1;
        string str2;
        string str3;

        
        try {
        classE1_1.Open();
        classE1_1.SetBasicModelInfo();  //  sets glngwid etc. 



        classE1_1.SaveWhatIfAudit(classE1_1.glngwid);
        classE1_1.LoadBaseCase();

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();

        classE1_1.dowhatif_all_start();
        classE1_1.Close();
        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        MPXWhatfcontrolChoices.SetActiveView(PageR1);

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }
    public void Button_NS_Close2_Click(Object sender, System.EventArgs e) {
        
        try {
        //  no save 
        classE1_1.Open();
        classE1_1.SetBasicModelInfo();  //  sets glngwid etc. 

        classE1_1.LoadBaseCase();


        classE1_1.dowhatif_all_start();
        classE1_1.Close();
        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        MPXWhatfcontrolChoices.SetActiveView(PageR1);

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    public void Button_SaveAs_Click(Object sender, System.EventArgs e) {
        
        int wid2;
        string str2;
        string str2n;
        try {
        classE1_1.Open();

        str2 = postvalue("TextBox16");
        str2 = MyUtilities.clean(str2);
        str2.Trim();
        if (str2.Length == 0) {
            Label4.Text = "Save As new whatif not Done.  No name for new Whatif";
            //Label4.Visible = true;
            Master.ShowErrorMessage("Could not save as new What-If because no name was entered. Please enter a name for new What-If Scenario.");
            return;
        }

        str2n = "copy of whatif ...";

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();

        wid2 = classE1_1.addnewwhatif(str2, str2n);

        classE1_1.glngwid = wid2;
        classE1_1.saveWid();
        classE1_1.SaveWhatIfAudit(wid2);

        string whatifName = classE1_1.Get_whatif_name(classE1_1.glngwid);
        Master.PassCurrentWhatifName(whatifName);
        Master.SetCurrentWhatifLabel();
        set_page2_words();
        updatingdatasource();
        classE1_1.Close();
        MPXWhatfcontrolChoices.SetActiveView(PageR2);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    public void Button_sas_close_Click(Object sender, System.EventArgs e) {        //  sas  save as
         
        int wid2;
        string str2;
        string str3;

        try {
        classE1_1.Open();
        classE1_1.SetBasicModelInfo();  //  sets glngwid etc. 


        str2 = MyUtilities.clean(postvalue("TextBox16"));
        str2.Trim();
        if (str2.Length == 0) {
            Label4.Text = "Save As new whatif not Done.  No name for new Whatif";
            //Label4.Visible = true;
            Master.ShowErrorMessage("Could not save as new What-If because no name was entered. Please enter a name for new What-If Scenario.");
            return;
        }

        str3 = "copy of whatif ...";

        wid2 = classE1_1.addnewwhatif(str2, str3);

        classE1_1.glngwid = wid2;
        classE1_1.saveWid();
        classE1_1.SaveWhatIfAudit(wid2);
        classE1_1.LoadBaseCase();

        classE1_1.model_modified = -1;
        classE1_1.saveModel_modified();


        updatingdatasource();
        classE1_1.dowhatif_all_start();
        classE1_1.Close();

        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        ShowWhatifs();

        MPXWhatfcontrolChoices.SetActiveView(PageR1);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        if (!LocalTablesLinked()) {
            ResetModelGoToModels();
        }
        try {
        classE1_1 = new ClassF(GetDirectory() + userDir);
        classE1_1.Close();
        SetWhatifRecordsCommands();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
        }
    }

    protected void btnLoadWhatif_Click(object sender, EventArgs e) {
        if (lstWhatifs.SelectedItem == null) {
            Master.ShowErrorMessage("No What-If Scenario is selected. Please select the What-If Scenario to load.");
        } else {
            try {
            int whatifId = int.Parse(lstWhatifs.SelectedValue);
            classE1_1.Open();
            classE1_1.LoadWhatIf(whatifId);

            classE1_1.model_modified = -1;
            classE1_1.saveModel_modified();

            set_page2_words();
            updatingdatasource();
            classE1_1.Close();
            Master.PassCurrentWhatifName(lstWhatifs.SelectedItem.Text);
            Master.SetCurrentWhatifLabel();
            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage("MPX internal error has occured.");                
            }
        }


    }
    protected void btnDeleteWhatif_Click(object sender, EventArgs e) {
        if (lstWhatifs.SelectedItem == null) {
            Master.ShowErrorMessage("No whatif is selected. Please select the whatif to delete.");
        } else {
            try {
            int whatifId = int.Parse(lstWhatifs.SelectedValue);

            classE1_1.Open();
            //today test if Wid2 = -1 or = 0 
            classE1_1.DeleteWhatIfAudit(whatifId);

            classE1_1.model_modified = -1;
            classE1_1.saveModel_modified();
            classE1_1.Close();
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage("MPX internal error has occured.");               
            }
        }
        ShowWhatifs();
    }

    protected void ShowWhatifs() {
        try {
            lstWhatifs.DataBind();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    protected void btnNewWhatif_Click(object sender, EventArgs e) {
        int wid2;
        try {
        classE1_1.Open();

        string whatifName = MyUtilities.clean(txtNewWhatif.Text);

        whatifName = whatifName.Trim();
        if (whatifName.Length == 0) {
            Master.ShowErrorMessage("Invalid name for What-If Scenario. Please enter a name for the new What-If Scenario.");
        } else if (classE1_1.glngwid != 0) {
            //  not in basecase ??
            set_page2_words();
            updatingdatasource();
            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            classE1_1.Close();
        } else if (WhatifExists(whatifName)) {
            Master.ShowErrorMessage("Cannot create a new What-If Scenario because a What-If Scenario with the same name already exists. Please choose a different name for the new What-If Scenario.");
        } else {

            //  assume in base case !!!
            // else must close current whatif return to basecase ...

            whatifName = whatifName.ToUpper();
            wid2 = classE1_1.addnewwhatif(whatifName, "new What-If Scenario");
            classE1_1.glngwid = wid2;
            classE1_1.saveWid();

            classE1_1.model_modified = -1;
            classE1_1.saveModel_modified();

            Master.PassCurrentWhatifName(whatifName);
            Master.SetCurrentWhatifLabel();

            txtNewWhatif.Text = "";
            set_page2_words();
            updatingdatasource();
            MPXWhatfcontrolChoices.SetActiveView(PageR2);
            ShowWhatifs();
            //classE1_1.runsql("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + wid2 + ";");
        }
        classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    protected bool WhatifExists(string whatifName) {
        bool exists = false;
        for (int i = 0; i < lstWhatifs.Items.Count && !exists; i++) {
            if (lstWhatifs.Items[i].Text.ToUpper().Equals(whatifName.ToUpper())) {
                exists = true;
            }
        }
        return exists;
    }


    protected void btnResetNameComment_Click(object sender, System.EventArgs e) {
        try {
            classE1_1.Open();
            set_page2_words();
            classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
        }
    }

    protected void SetWhatifRecordsData() {
        try {
            gridWhatifRecords.DataBind();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
        }
    }

    protected void SetWhatifRecordsCommands() {
        srcWhatifRecords.SelectCommand = "SELECT AuditID, TableE, FieldE, OldShow, NewShow, Descripton FROM zstblWhatifAudit WHERE WID = " + classE1_1.glngwid + ";";
        srcWhatifRecords.DataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        AccessDataSource1.DataFile = srcWhatifRecords.DataFile;
    }

    

    protected void gridWhatifRecords_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        gridWhatifRecords.PageIndex = e.NewPageIndex;
    }
    protected void gridWhatifRecords_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            foreach (TableCell cell in e.Row.Cells) {
                try {

                    foreach (Control control in cell.Controls) {
                        if (control is Label) {
                            Label txt = control as Label;
                            
                            try {
                                double num = double.Parse(txt.Text);
                                cell.HorizontalAlign = HorizontalAlign.Center;
                            } catch (Exception) { }
                            
                            break;
                        }
                    }

                } catch (Exception) {

                }
            }
        }
    }

    protected void RefreshWhatifRecords() {
        try {
            classE1_1.dowhatif_all_end();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    protected override bool LocalTablesLinked() {
        if (!UpdateSql("SELECT WID FROM tblWhatif;")) {
            return false;
        }
        return true;
    }
}