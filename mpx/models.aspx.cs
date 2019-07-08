using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data.OleDb;

public partial class models : DbPage {

    protected AjaxControlToolkit.ModalPopupExtender extenderNewModel;
    protected Panel pnlNewModelPopup;
    protected HiddenField hdnNewNameMode;

    protected const string DEMONSTRATION_MODEL = "Gthubs.mdb";

    protected Label lblNewName;

    private string PartialDir(string dir) {
        return dir.Substring(dir.IndexOf(userDir));
    }

    public models() {
        PAGENAME = "/models.aspx";
    }

    public string pageInsert;

    private class SaveMode {
        public const int CURRENT_MODEL = 0;
        public const int NEW_MODEL = 1;
        public const int RENAME_MODEL = 2;

    }

    private class ActionAfterSave {
        public const int OPEN_MODEL = 0;
        public const int NEW_MODEL = 1;
    }

    private const string OPEN_MODEL_LISTBOX_ARGUMENT = "open";


    private void Page_Load(object sender, System.EventArgs e) {

        base.Page_Load(sender, e);

        if (Request.QueryString["timeout"] != null)
        {
            Response.Write("<script>alert('Your session has timed out');</script>");
        }

        btnSave.Enabled = !(Master.GetCurrentModel().Equals("none") || Master.GetCurrentModel().Equals(""));
        btnSaveAs.Enabled = btnSave.Enabled;
        btnRenameModel.Enabled = btnSave.Enabled;
        btnUpload.Enabled = fuUpload.HasFile;
        //btnClearMdl.Enabled = btnSave.Enabled;
        if (!this.IsPostBack) {
            //  where double click is setup ?  
            var tempString = this.Request.Headers["Referer"];
            if (tempString != null && tempString.Contains("models.aspx") )
            {
                Master.ShowErrorMessage("Please open or create a new model");
            }
            lstModels.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(lstModels, OPEN_MODEL_LISTBOX_ARGUMENT));
            HttpCookie lastPageCookie = Request.Cookies[DbUse.LASTPAGE_COOKIE];
            try {
                lstRdbtnJump.SelectedValue = (lastPageCookie == null) ? "0" : "1";
            } catch (Exception) {
                lstRdbtnJump.SelectedIndex = 0;
            }
            string modelsDir = GetDirectory() + userDir + "\\" + MODELS;
            string outputsDir = GetDirectory() + userDir + "\\" + OUTPUTS;
            try {
                ShowFilesIn(modelsDir, lstModels);
            } catch (Exception) { 
                Master.ShowErrorMessage("An error has occured. Missing user data.");
            }
            //string btnJavascript = ClientScript.GetPostBackClientHyperlink(
            //            btnOpen, "");
            //lstModels.Attributes["ondblclick"] = btnJavascript;
            //ShowFilesIn(outputsDir, lstOutputs);
        } else {
            
            //  do double click event NEW CODE TO WHATIFS / VISION...  XXXX 
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == OPEN_MODEL_LISTBOX_ARGUMENT) {
                OpenModelFromList();
            }
        } 

    }

    private void OpenModelFromList() {
        string userDb;
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot open a model because calculations are currently in process. Please wait.");
            return;
        }
        if (lstModels.SelectedIndex == -1) {
            Master.ShowErrorMessage("No model is selected. Please select a model to open.");
        } else {
            userDb = lstModels.SelectedItem.Text;
            string currentModel = Master.GetCurrentModel();
            if (!currentModel.Equals("none")) {
                if (!currentModel.Equals(userDb) && GetModelModified()) {
                    hdnAction.Value = ActionAfterSave.OPEN_MODEL.ToString();
                    Master.SetFocus2(btnWantSaveModel.ClientID);
                    modalPopupWantSaveModel.Show();
                } else if (currentModel.Equals(userDb) && GetModelModified()) {
                    Master.SetFocus2(btnLoadAgain.ClientID);
                    modalUnsave.Show();
                } else {
                    bool opened = OpenModel(userDb);
                    if (opened == false)
                    {
                        Master.ShowInfoMessage("Model '" + userDb + "' can not be opened. It is not in correct Access database 200x format.");
                        return;
                    }
                    AddEquipTypeNameColumn();
                    AddGraphMLcolumn(); 
                }
            } else {
                bool opened = OpenModel(userDb);
                if (opened == false)
                {
                    Master.ShowInfoMessage("Model '" + userDb + "' can not be opened. It is not in correct Access database 200x format.");
                    return;
                }
                AddEquipTypeNameColumn();
                AddGraphMLcolumn();

            }
        }
    }

    protected void OpenModel(object sender, EventArgs e) {
        OpenModelFromList();

    }



    private void ShowFilesIn(string dir, ListBox lstBox) {
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        lstBox.Items.Clear();
        foreach (FileInfo fileItem in dirInfo.GetFiles()) {
            lstBox.Items.Add(fileItem.Name);
        }
    }

    protected void UploadModel() {
        string filedir = GetDirectory() + userDir + MODELS;
        string filepath = filedir + "\\" + fuUpload.FileName;
        try {
            fuUpload.SaveAs(filepath);
            ShowFilesIn(filedir, lstModels);
            Master.ShowInfoMessage("Model '" + fuUpload.FileName + "' successfully uploaded.");
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Could not upload model '" + fuUpload.FileName + "'.");
        }
    }

    protected void btnRewriteUpload_Click(object sender, EventArgs e) {
        string filename = "";
        try {
            filename = MoveUploadFromTemp();
            Master.ShowInfoMessage("Model '" + filename + "' was successfully rewritten.");
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("An error has occured and the model could not rewrite.");
        }
    }

    protected void btnCancelRewriteUpload_Click(object sender, EventArgs e) {
        DeleteUploadTemp();
    }

    private void StoreUploadTemp() {
        string filepath = GetDirectory() + userDir + DbUse.UPLOAD_TEMP_DIR + "\\" + fuUpload.FileName;
        fuUpload.SaveAs(filepath);

    }

    private void DeleteUploadTemp() {
        string dir = GetDirectory() + userDir + "\\" + DbUse.UPLOAD_TEMP_DIR;
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        foreach (FileInfo fileItem in dirInfo.GetFiles()) {
            try {
                fileItem.Delete();
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
            }
        }
    }

    protected string MoveUploadFromTemp() {
        string filename = "";
        string dir = GetDirectory() + userDir + DbUse.UPLOAD_TEMP_DIR;
        string modelsDir = GetDirectory() + userDir + MODELS + "\\";
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        string dirNew = GetDirectory() + userDir + MODELS + "\\";
        string currentModel = Master.GetCurrentModel().ToLower();
        foreach (FileInfo fileItem in dirInfo.GetFiles()) {
            try {
                filename = fileItem.Name;
                string filepath = dirNew + filename;
                if (filename.ToLower().Equals(currentModel)) {
                    throw new Exception("Cannot rewrite the model. The model you intended to rewrite is the model which is currently open.");
                } else {
                    File.Delete(filepath);
                    fileItem.MoveTo(filepath);
                }

            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
            }
        }
        return filename;
    }

    protected void btnUpload_Click(object sender, EventArgs e) {
        if (fuUpload.HasFile) {
            string filedir = GetDirectory() + userDir + MODELS;
            string filename = fuUpload.FileName;

            string currentModel = Master.GetCurrentModel().ToLower();
            if (filename.ToLower().Equals(currentModel)) {
                Master.ShowErrorMessage("Cannot upload the model because a model with the same name is currently open and cannot be overwritten.");
            } else {
                string extension = filename.Substring(filename.LastIndexOf('.'));
                if (extension.ToLower().Equals(".mdb") || extension.ToLower().Equals(".accdb") || extension.ToLower().Equals(".mpx") || extension.ToLower().Equals(".mct")) {
                    if (fuUpload.PostedFile.ContentLength > MAX_MODEL_SIZE_IN_BYTES) {
                        Master.ShowErrorMessage("Cannot upload the model because the file is too big.");
                    } else {
                        string[] models = new string[lstModels.Items.Count];
                        for (int i = 0; i < lstModels.Items.Count; i++) {
                            models[i] = lstModels.Items[i].Text.ToLower();
                            if (filename.ToLower().Equals(models[i])) {
                                StoreUploadTemp();
                                modalRewriteUpload.Show();
                                Master.SetFocus2(btnCancelRewrite.ClientID);
                                return;
                            }
                        }
                        UploadModel();
                    }

                } else {
                    Master.ShowErrorMessage("You can only upload '.mct', '.mdb', '.accdb' or '.mpx' database models.");
                }
            }
        } else {
            Master.ShowErrorMessage("No file is selected for uploading. Please choose a model to upload.");
        }
    }

    private const int MAX_MODEL_SIZE_IN_BYTES = 10000000; // max model size is 10 MB

    protected void DeleteModel(object sender, EventArgs e) {
        ListItem file = lstModels.SelectedItem;
        string message = "";
        string currentModel = Master.GetCurrentModel();
        if (file != null) {
            string filedir = GetDirectory() + userDir + MODELS;
            string filepath = filedir + "\\" + file;
            try {

                if (file.Value.Equals(currentModel)) {
                    if (DbUse.InRunProcess(userDir)) {
                        Master.ShowErrorMessage("Cannot delete the current model because calculations are still running. Please wait.");
                        return;
                    }
                    
                    Master.SetFocus(btnClearModel.ClientID);
                    modalDeleteCurrent.Show();
                } else {
                    message = "Model '" + file + "' ";
                    File.Delete(filepath);
                    message += " deleted.";
                    ShowFilesIn(filedir, lstModels);
                    Master.ShowInfoMessage(message);
                }

            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                Master.ShowErrorMessage("An error has occured and the model cannot be deleted.");
            }


        } else {
            message = "No model selected for deletion. Please select a model to delete.";
            Master.ShowErrorMessage(message);

        }


    }

    // does not work - color of selected items is given by the browser
    protected void lstFiles_ColorItems(object sender, EventArgs e) {
        foreach (ListItem item in lstModels.Items) {
            if (item.Selected) {
                item.Attributes["style"] = "background-color:#647797";
            } else {
                item.Attributes["style"] = "background-color:transparent";
            }
        }
    }

    protected void DownloadModel(object sender, EventArgs e) {
        string message;
        string userDb;
        if (lstModels.SelectedIndex == -1) {
            message = "No model is selected for downloading. Please select a model to download.";
            Master.ShowErrorMessage(message);
        } else {
            userDb = lstModels.SelectedItem.Text;
            string filedir = GetDirectory() + userDir + MODELS + "\\";
            string filepath = filedir + userDb;
            DownloadFile(filepath);
        }
    }

    protected void DownloadOutput(object sender, EventArgs e) {
        string userFile;
        if (lstOutputs.SelectedIndex == -1) {
            Master.ShowErrorMessage("No output file is selected for downloading. Please selecte the output to download.");
        } else {
            userFile = lstOutputs.SelectedItem.Text;
            string filedir = GetDirectory() + userDir + OUTPUTS + "\\";
            string filepath = filedir + userFile;
            DownloadFile(filepath);
        }
    }

    private void DownloadFile(string filepath) {
        FileInfo objFileInfo = new FileInfo(filepath);
        Response.Clear();
        Response.AddHeader("Content-Disposition", "attachment;filename=" + objFileInfo.Name);//Add File name to dialog display
        Response.AddHeader("Content-Length", objFileInfo.Length.ToString());//Add the file length to dialog display
        Response.ContentType = "application/octet-stream";
        Response.TransmitFile(objFileInfo.FullName);//download your file form servic
        Response.End();
    }

    protected void DeleteOutput(object sender, EventArgs e) {
        string message = "";
        ListItem file = lstOutputs.SelectedItem;
        if (file != null) {
            string filedir = GetDirectory() + userDir + OUTPUTS + "\\";
            string filepath = filedir + file;
            try {
                File.Delete(filepath);
                message = "File '" + file + "' successfully deleted.";
                ShowFilesIn(filedir, lstOutputs);
                Master.ShowInfoMessage(message);
            } catch (Exception) {
                Master.ShowErrorMessage("An error has occured and the output file could not be deleted.");
            }
        } else {
            message = "No output selected for deletion. Please select the output file to delete.";
            Master.ShowErrorMessage(message);
        }
    }

    protected void SaveModelAs(object sender, EventArgs e) {
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot save the model because calculations are still running. Please wait.");
            return;
        }
        hdnNewNameMode.Value = SaveMode.CURRENT_MODEL.ToString();

        string modelName = Master.GetCurrentModel();
        if (modelName == null || modelName.Equals("") || modelName.Equals("none")) {
            string message = "Cannot save the model because no model is loaded.";
            Master.ShowErrorMessage(message);
        } else if (IsWhatifMode()) {
            Master.SetFocus2(btnGoToWhatif.ClientID);
            modalGoToWhatif.Show();
        } else {
            TextBox txtNewName = pnlNewModelPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
            txtNewName.Text = modelName;
            lblNewName.Text = "Save current model as: ";
            extenderNewModel.Show();
            Master.SetFocus(txtNewName.ClientID);
        }

    }

    protected void SaveModel(object sender, EventArgs e) {
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot save the model because calculations are still running. Please wait.");
            return;
        }
        hdnNewNameMode.Value = SaveMode.CURRENT_MODEL.ToString();

        string modelName = Master.GetCurrentModel();
        if (modelName == null || modelName.Equals("") || modelName.Equals("none")) {
            string message = "Cannot save the model because no model is loaded.";
            Master.ShowErrorMessage(message);
        } else if (IsWhatifMode()) {
            Master.SetFocus(btnGoToWhatif.ClientID);
            modalGoToWhatif.Show();
        } else {
            SaveModel(SaveMode.CURRENT_MODEL, modelName);
        }
    }

    protected bool SaveModel(int saveMode, string name) {
        return SaveModel(saveMode, name, true);
    }


    protected bool SaveModel(int saveMode, string name, bool showMessage) {
        string currDbDir = GetDirectory() + userDir;
        string modelDir = currDbDir + MODELS + "\\";
        string modelName = modelDir + name;
        string modelInitialName;
        string originalModel = Master.GetCurrentModel();
        if (saveMode == SaveMode.CURRENT_MODEL || saveMode == SaveMode.RENAME_MODEL) {
            modelInitialName = modelDir + CURRENT_DATABASE;
            {
                ClassF classF = new ClassF(GetDirectory() + userDir);
                classF.Open();
                classF.eliminate_nulls0();
                classF.check_dock_stock();
                //classF.checkDOCKSTOCK();
                //classF.clear_all_id_name_flags(); ????? CANNOT FIND
                classF.set_ids2names();
                classF.Close();
            }
        } else if (saveMode == SaveMode.NEW_MODEL) {
            modelInitialName = modelDir + NEW_MODEL_DATABASE;
        }
        string message = "Model '" + name + "'";
        try {
            if (saveMode == SaveMode.NEW_MODEL) {
                if (!File.Exists(currDbDir + NEW_MODEL_DATABASE)) {
                    File.Copy(GetDirectory() + NEW_MODEL_DATABASE, currDbDir + NEW_MODEL_DATABASE, true);
                }
                File.Copy(currDbDir + NEW_MODEL_DATABASE, modelName, true); // save the model
                message += " was created ";
                //File.Move(modelInitialName, modelName);
            } else {
                if (saveMode == SaveMode.CURRENT_MODEL) {
                    File.Copy(currDbDir + CURRENT_DATABASE, modelName, true); // save the model
                } else if (saveMode == SaveMode.RENAME_MODEL) {
                    File.Copy(currDbDir + CURRENT_DATABASE, modelName, true); // save the model
                    File.Delete(modelDir + originalModel); // delete the original model
                }
                message += " saved successfuly.";
            }
            SetModelModified(false);

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            if (showMessage) {
                message += " could not save. ";
                Master.ShowErrorMessage("An error has occured." + message);
            }
            return false;
        }
        CleanUpAfterSave(saveMode, originalModel, name, message, showMessage);
        return true;
    }

    private void CleanUpAfterSave(int saveMode, string originalModel, string savedName, string userMessage, bool showMessage) {
        if ((saveMode == SaveMode.CURRENT_MODEL && !savedName.Equals(originalModel)) || saveMode == SaveMode.RENAME_MODEL) {
            Master.setModel(savedName);
            SetCurrentModelInDb(savedName);
            string modelsDir = GetDirectory() + userDir + "\\" + MODELS;
            ShowFilesIn(modelsDir, lstModels);
            if (showMessage) {
                if (saveMode == SaveMode.RENAME_MODEL) {
                    Master.ShowInfoMessage("Model '" + originalModel + "' was successfuly renamed to '" + savedName + "'.");
                } else {
                    Master.ShowInfoMessage_Post(userMessage);
                }
            }
        } else if (saveMode == SaveMode.NEW_MODEL) {
            if (OpenModel(savedName, false)) {
                Master.setModel(savedName);
                string modelsDir = GetDirectory() + userDir + "\\" + MODELS;
                ShowFilesIn(modelsDir, lstModels);
                if (showMessage) {
                    Master.ShowInfoMessage(userMessage + " and opened successfuly.");
                }
            } else {
                if (showMessage) {
                    string filedir = GetDirectory() + userDir + MODELS;
                    ShowFilesIn(filedir, lstModels);
                    Master.ShowErrorMessage_Post("New model was created but an error has occured and it cannot be opened.");
                }
            }
        } else {
            if (showMessage) {
                Master.ShowInfoMessage(userMessage);
            }
        }
    }

    protected void btnNewModel_Click(object sender, EventArgs e) {
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot start a new model because calculations are still running in the current model. Please wait.");
            return;
        }
        hdnAction.Value = ActionAfterSave.NEW_MODEL.ToString();
        string currentModel = Master.GetCurrentModel();
        if (currentModel.Equals("none") || !GetModelModified()) {
            ContinueAfterSave(); // skips the saving part of current model

        } else {
            // ask the user if he wants to save the current model via pop up
            Master.SetFocus(btnWantSaveModel.ClientID);
            modalPopupWantSaveModel.Show();

        }

    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        base.OnInit(e);
        tableSync = new TableSynchronization(userDir);
        //string sheet = "Cheat Sheat Models Page";
        //Master.SetHelpSheet(sheet + ".pdf", sheet);
    }

    //SAME PANEL USED FOR MULTIPLE BUTTONS
    protected void InitializeComponent() {
        pnlNewModelPopup = InputPageControls.GenerateNewNamePanel("Save the model as: ");
        extenderNewModel = InputPageControls.GenerateNewNameExtender();

        pnlNewNameWrap.Controls.Add(pnlNewModelPopup);
        lblNewName = pnlNewModelPopup.FindControl(InputPageControls.LBL_NEW_NAME) as Label;
        pnlNewNameWrap.Controls.Add(extenderNewModel);
        hdnNewNameMode = pnlNewModelPopup.FindControl(InputPageControls.HIDDEN_MODE) as HiddenField;
        Button btnNewNameOk = pnlNewModelPopup.FindControl(InputPageControls.BTN_OK_NEW_NAME) as Button;
        btnNewNameOk.Click += new EventHandler(btnNewNameOk_Click);
        


        TextBox txtNewName = pnlNewModelPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
        Master.SetFocus2(txtNewName.ClientID);
        lblNewName.ToolTip = "The name must not be empty. Characters ' \" * \\ / & < > : ; # & are not allowed.";
        //txtNewName.Attributes.Add("onkeydown", "doFocus('" + btnNewNameOk.ClientID + "', event);");
        //Master.ClickOnEnterF(btnNewNameOk.ClientID, txtNewName);
    }

    protected void btnNewNameOk_Click(object sender, EventArgs e) {
        TextBox txtNewName = pnlNewModelPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
        if (txtNewName == null) { return; }
        string name = txtNewName.Text;
        name = name.Trim();

        if (!IsModelNameValid(name)) {
            //txtNewName.Text = "";
            extenderNewModel.Show();
            Master.ShowErrorMessageAndFocus("Invalid model name. Please try again.", txtNewName.ClientID);

        } else {
            if (!IsSuffixValid(name)) {
                name += ".mct";
            }
            string[] models = new string[lstModels.Items.Count];
            for (int i = 0; i < lstModels.Items.Count; i++) {
                models[i] = lstModels.Items[i].Text.ToLower();
                if (name.ToLower().Equals(models[i])) {
                    extenderNewModel.Show();
                    Master.ShowErrorMessageAndFocus("A model with the same name already exists. Please choose a different name.", txtNewName.ClientID);
                    return;
                }
            }
            int saveMode = int.Parse(hdnNewNameMode.Value);
            bool modelSaved = SaveModel(saveMode, name);


        }
    }

    private bool IsSuffixValid(string modelName) {
        bool valid = false;
        int suffixIndex = modelName.ToLower().LastIndexOf(".mpx");
        if (suffixIndex != -1 && (suffixIndex == modelName.Length - 4)) {
            valid = true;
        }
        suffixIndex = modelName.ToLower().LastIndexOf(".mct");
        if (suffixIndex != -1 && (suffixIndex == modelName.Length - 4)) {
            valid = true;
        }
        suffixIndex = modelName.ToLower().LastIndexOf(".mdb");
        if (suffixIndex != -1 && (suffixIndex == modelName.Length - 4)) {
            valid = true;
        }
        suffixIndex = modelName.ToLower().LastIndexOf(".accdb");
        if (suffixIndex != -1 && (suffixIndex == modelName.Length - 6)) {
            valid = true;
        }
        return valid;
    }

    private bool IsModelNameValid(string modelName) {
        bool valid = true;
        string name = modelName.ToLower();
        if (name.Contains("*") || name.Contains("\\") || name.Contains("/") || name.Contains("\'") || name.Contains("\"") || name.Contains("&") || name.Contains("#") || name.Contains("<") || name.Contains(">") || name.Contains("@") || name.Contains(":") || name.Contains(";")) {
            valid = false;
        } else if (name.Equals("") || name.Equals(".mdb") || name.Equals(".accdb") || name.Equals(".mpx") || name.Equals(".mct") || name.Equals(NEW_MODEL_DATABASE.ToLower()) || name.Equals("initial")) {
            valid = false;
        }
        return valid;
    }

    protected void RenameModel(object sender, EventArgs e) {
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot rename the model because calculations are still running. Please wait.");
            return;
        }
        string currentModel = Master.GetCurrentModel();
        if ((currentModel != null) && !currentModel.Equals("") && !currentModel.Equals("none")) {
            if (IsWhatifMode()) {
                Master.SetFocus2(btnGoToWhatif.ClientID);
                modalGoToWhatif.Show();
            } else {
                lblNewName.Text = "Enter a new name for the current model: ";
                hdnNewNameMode.Value = SaveMode.RENAME_MODEL.ToString();
                TextBox txtNewName = pnlNewModelPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;

                extenderNewModel.Show();
                Master.SetFocus(txtNewName.ClientID);
            }
        } else {
            Master.ShowErrorMessage("Cannot rename the current model because no model is loaded.");
        }

    }


    // gets called on model opening or new model
    protected void btnWantSaveModel_Click(object sender, EventArgs e) {
        if (IsWhatifMode()) {
            Master.SetFocus2(btnGoToWhatif.ClientID);
            modalGoToWhatif.Show();
        } else {
            string modelName = Master.GetCurrentModel();
            if (!SaveModel(SaveMode.CURRENT_MODEL, modelName, false)) {
                Master.ShowErrorMessage("An error has occured and the current model could not save.");
            } else {
                ContinueAfterSave();
            }
        }
    }

    // gets called on model opening or new model
    protected void btnDontSaveModel_Click(object sender, EventArgs e) {
        if (IsWhatifMode()) {
            ReturnToBasecase();
        }
        ContinueAfterSave();
    }

    protected void ContinueAfterSave() {
        int action = int.Parse(hdnAction.Value);
        if (action == ActionAfterSave.OPEN_MODEL) {
            string userDb = lstModels.SelectedItem.Text;
            if( OpenModel(userDb) == false) { 
                        Master.ShowInfoMessage("Model '" + userDb + "' can not be opened. It is not in correct Access database 200x format.");
                        return;
                    };
        } else if (action == ActionAfterSave.NEW_MODEL) {
            hdnNewNameMode.Value = SaveMode.NEW_MODEL.ToString();
            TextBox txtNewName = pnlNewModelPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
            txtNewName.Text = "";
            lblNewName.Text = "Enter a name for the new model: ";
            Master.SetFocus(txtNewName.ClientID);
            
            extenderNewModel.Show();

        }
        SetModelModified(false);
    }

    protected void ReturnToBasecase() {
        ClassF mpxClass = new ClassF(GetDirectory() + userDir);
        mpxClass.LoadBaseCase();
        //mpxClass.dowhatif_all_end(); // don't know if that's of any use...
        mpxClass.Close();
        Master.PassCurrentWhatifName(""); // hide the current whatif label in master page
        Master.SetCurrentWhatifLabel();
    }

    protected void btnLoadAgain_Click(object sender, EventArgs e) {
        if (IsWhatifMode()) {
            ReturnToBasecase();
        }
        if (OpenModel(Master.GetCurrentModel(), false)) {
            AddEquipTypeNameColumn();
            //AddGraphMLcolumn();
            Master.ShowInfoMessage("Current model was loaded again.");
            SetModelModified(false);
        } else {
            Master.ShowErrorMessage_Post("An error has occured and the model could not get loaded again.");
        }
    }
    protected void btnClearModel_Click(object sender, EventArgs e) {
        string currModel = Master.GetCurrentModel();
        UnlinkTables();
        SaveModel(SaveMode.NEW_MODEL, currModel, false);
        OpenModel(currModel, false);
        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();

        //Master.HideMPXpopups();
        string message = "Model '" + currModel + "' was successfully cleared of its data.";
        SetModelModified(true);
        Master.ShowInfoMessage_Post(message);

    }
    protected void btnDeleteReally_Click(object sender, EventArgs e) {
        string currModel = Master.GetCurrentModel();
        string filedir = GetDirectory() + userDir + MODELS;
        string filepath = filedir + "\\" + currModel;
        string message = "Model '" + currModel + "' ";
        try {
            UnlinkTables();
            SetModelModified(false);
            ResetCurrentModelInDb();
            Master.ResetModel();

            File.Delete(filepath);
            ShowFilesIn(filedir, lstModels);
            message += "was closed and deleted.";
            Master.ShowInfoMessage(message);
        } catch (Exception) {
            Master.ShowErrorMessage_Post("An error has occured and the current model could not be deleted.");

        }


    }

    protected void AddEquipTypeNameColumn() {

        string alterTable = "Alter table tblequip ADD COLUMN equiptypename Text (15) ";
        CalcClass calcClass = new CalcClass(GetDirectory() + userDir + "curr_mpx.mdb");
        calcClass.runsql(alterTable);
        calcClass.runsql("UPDATE tblequip SET equiptypename = 'Standard'  WHERE EquipType = 0;");
        calcClass.runsql("UPDATE tblequip SET equiptypename = 'Delay'  WHERE EquipType = 1;");

        calcClass = new CalcClass(GetDirectory() + userDir + MAIN_USER_DATABASE);
        //calcClass.runsql(alterTable);
        alterTable = "Alter table zztblequip ADD COLUMN equiptypename Text (15) ";
        calcClass.runsql(alterTable);

    }

    protected void AddGraphMLcolumn() {
        if (!ColumnExists("graphML", "tblprodfore")) {
            string alterTable = "Alter table tblprodfore ADD COLUMN  graphML Memo;";
            CalcClass calcClass = new CalcClass(GetDirectory() + userDir + "curr_mpx.mdb");
            calcClass.runsql(alterTable);
        }
    }

    private bool ColumnExists(string column, string table) {
        bool exists = true;
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + "curr_mpx.mdb" + ";");
        string[] restrictions = new string[4] { null, null, table, column };
        {
            try {
                connec.Open();
                DataTable schema = connec.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
                if (schema.Rows.Count <= 0) {
                    exists = false;
                }
                connec.Close();
            } catch (Exception) {
                try {
                    connec.Close();
                } catch (Exception) { }
                exists = false;
            }
        }
        // Master.ShowErrorMessage("graphML column exists in the database: " + exists);
        return exists;
    }

    protected void btnGoToWhatif_Click(object sender, EventArgs e) {
        Response.Redirect("whatif_c.aspx");
    }

    protected void btnResetFiles_Click(object sender, EventArgs e) {
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot reset the default files because calculations are running in the current model. Please wait.");
            return;
        } else if (!Master.GetCurrentModel().Equals("none") && !Master.GetCurrentModel().Equals(String.Empty) && IsWhatifMode()) {
            //Master.ShowErrorMessage("Cannot reset default files because current model is running in whatif mode.");
            Master.SetFocus2(btnGoToWhatifReset.ClientID);
            extenderResetFilesWhatif.Show();
            return;
        }
        Master.SetFocus2(btnCancelResetFilesReally.ClientID);
        extenderResetFiles.Show();
    }

    protected void ResetFiles() {
        try {
            if (!Master.GetCurrentModel().Equals("none")) {
                UnlinkTables();
                
            }
        } catch (Exception) {
            try {
                SetModelModified(false);
                ResetCurrentModelInDb();
                Master.ResetModel();
            } catch (Exception) { }
        }

        string modelFilePath = GetDirectory() + DbPage.MAIN_USER_DATABASE;
        string userModelFilePath = GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE;
        string errorMessage = "";
        try {
            try {
                File.Delete(userModelFilePath);
            } catch (Exception ex) { 
                logFiles.ErrorLog(ex);
            }
            File.Copy(modelFilePath, userModelFilePath, true);
        } catch (Exception ex2) {
            logFiles.ErrorLog(ex2);
            errorMessage += "Error overwriting main Value Stream Modeling file. ";
        }
        userModelFilePath = GetDirectory() + userDir + DbPage.NEW_MODEL_DATABASE;
        modelFilePath = GetDirectory() + DbPage.NEW_MODEL_DATABASE;
        try {
            try {
                File.Delete(userModelFilePath);
            } catch (Exception exp) {
                logFiles.ErrorLog(exp);
            }
            File.Copy(modelFilePath, userModelFilePath, true);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            errorMessage += "Error overwriting initial model file. ";
        }
        userModelFilePath = GetDirectory() + userDir + DbPage.MODELS + "\\" + DbPage.DEMONSTRATION_DATABASE;
        modelFilePath = GetDirectory() + DbPage.DEMONSTRATION_DATABASE;
        try {
            try {
                File.Delete(userModelFilePath);
            } catch (Exception) { }
            File.Copy(modelFilePath, userModelFilePath, true);
        } catch (Exception) {
            errorMessage += "Error overwriting demonstration model file " + DbPage.DEMONSTRATION_DATABASE + ". ";
        }
        if (!Master.GetCurrentModel().Equals("none")) {
           if (LinkTables() != -1) {
                errorMessage += "Error in linking current model to the main Value Stream Modeling file. ";
           } else {
                AddEquipTypeNameColumn(); 
           }
        }
        if (!errorMessage.Equals("")) {
            Master.ShowErrorMessage(errorMessage);
        } else {
            CopyVersionFile();
            Master.ShowInfoMessage("Value Stream Modeling default files reset successfully.");
        }
    }

    protected void btnResetFilesReally_Click(object sender, EventArgs e) {
        ResetFiles();
    }
    
    protected void lstRdbtnJump_SelectedIndexChanged(object sender, EventArgs e) {
        string selectedValue = lstRdbtnJump.SelectedValue;
        HttpCookie lastPageCookie = new HttpCookie(DbUse.LASTPAGE_COOKIE);
        lastPageCookie.Value = "y";
        if (!selectedValue.Equals("0")) {
            lastPageCookie.Expires = DateTime.Now.AddYears(50);
        } else {
            lastPageCookie.Expires = DateTime.Now.AddDays(-1);
        }
        Response.Cookies.Add(lastPageCookie);
    }

    

    
}