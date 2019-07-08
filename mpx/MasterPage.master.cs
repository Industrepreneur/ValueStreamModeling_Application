using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.IO;

public partial class MasterPageMPX : System.Web.UI.MasterPage, IMasterPageMPX
{
    protected string DATABASE = "Database22.mdb";

    public const string LOGOUT_PAGE = "/login.aspx";

    private const string LOGOUT_USERID = "none";

    private const string MODEL_COOKIE = "model";

    private const string HELP_FILENAME = "MPX_44_Manual.pdf";
    private const string HELP_TITLE = "MPX Value Stream Modeling Help";

    private const string GENERAL_ERROR_MSG = "An error has occured and the data cannot be displayed.";
    private const string USERNAME_DATABASE = "Database22.mdb";

    private string currentModel = "none";
    private string currentWhatif = "none";
    private string currentAnalysis = "none";
    public string userDir = null;

    private string helpSheetFile = null;
    private string helpSheetTitle = null;

    private bool ModelModified;

    public String headerInsert;

    public String bodyInsert;
    public String contentInsert;
    public String modelText;

    public string username;

    public string lastLogin;

    public string logoutMessage;

    protected ADODB.Connection conn = new ADODB.Connection();

    protected ADODB.Recordset rec = new ADODB.Recordset();

    private bool cookiesEnabled;



    public void setModel(string value)
    {
        // save the model name as cookie
        //Response.Cookies[MODEL_COOKIE].Value = value;
        //Response.Cookies["model"].Expires = DateTime.Now.AddDays(30);
        //int dotPosition = value.LastIndexOf(".");
        //value = value.Remove(dotPosition);
        currentModel = value;

        int dotPosition = currentModel.LastIndexOf(".");
        string currentModelText = currentModel;
        if (dotPosition != -1)
        {
            currentModelText = currentModel.Remove(dotPosition);
        };
        lblModel.Text = currentModelText;

        if (ModelModified)
        {
            currentModelText = currentModelText + "*";
        }
        model.Value = currentModelText;

    }

    public void SetWhatif(string value)
    {  //  not current analysis copy 
        currentWhatif = value;
        lblCurrentWhatif.Text = value;
        if (ModelModified)
        {
            lblCurrentWhatif.Text = lblCurrentWhatif.Text + "*";
        }
    }

    public void MarkSavedModel()
    {
        string currentModel = GetCurrentModel();
        string currentModelText = currentModel;
        int dotPosition = currentModel.LastIndexOf(".");
        if (dotPosition != -1)
        {
            currentModelText = currentModel.Remove(dotPosition);
        };

        if (ModelModified)
        {
            currentModelText += "*";
        }
        //string text = currentModel;

        if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "changemodeltext", " $get('" + lblModel.ClientID + "').innerHTML = '" + currentModelText + "';", true);
        }
        else
        {
            lblModel.Text = currentModelText;
        }

    }

    public void ResetModel()
    {
        setModel("none");
        //model.Value = "";
        //HttpCookie modelCookie = new HttpCookie(MODEL_COOKIE);
        //modelCookie.Expires = DateTime.Now.AddDays(-1);
        //Response.Cookies.Add(modelCookie);
    }
    private void SetModelLabel()
    {
        /*HttpCookie modelCookie = Request.Cookies[MODEL_COOKIE];
        string text;
        if (!cookiesEnabled) {
            text = "Current Model: unknown";
        } else if (modelCookie == null) {
            text = "Current Model: none";
        } else {
            text = "Current Model: " + modelCookie.Value;
            model.Value = modelCookie.Value;
        }*/
        //modelText = text;
        //lblModel.Text = text;
        int dotPosition = currentModel.LastIndexOf(".");
        string currentModelText = currentModel;
        if (dotPosition != -1)
        {
            currentModelText = currentModel.Remove(dotPosition);
        };

        lblModel.Text = currentModelText;

    }

    protected void SetMainLabels()
    {
        lblUser.Text = "Welcome " + username + "!";
        lblLastLogin.Text = "Last Login: " + lastLogin + "; Last Logout: " + logoutMessage;
        int dotPosition = currentModel.LastIndexOf(".");
        string currentModelText = currentModel;
        if (dotPosition != -1)
        {
            currentModelText = currentModel.Remove(dotPosition);
        };
        lblModel.Text = currentModelText;
        //lblModel.Text = currentModel;
        if (ModelModified)
        {
            lblModel.Text = lblModel.Text + "*";
        }
        SetCurrentWhatifLabel();
        SetCurrentAnalysisLabel();
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Header.DataBind();
        if (!Page.IsPostBack)
        {
            string script = "<SCRIPT language='javascript' type='text/javascript'>function textChangedClick() {eval(\"document.getElementById('" + HiddenBtn.ClientID + "').click()\"); }</SCRIPT>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
        }
        else
        {
            try
            {
                HideLoadingPopup();
            }
            catch (Exception)
            {

            }
        }

        Page.MaintainScrollPositionOnPostBack = true;
        cookiesEnabled = DbUse.CookiesEnabled();
        SetModelLabel();
        //This is where timeout is initialized
     /*   firstTimer.Interval = 12 * 60 * 1000;*/ //!!! disabled temporarily the timer - testing javascript countdown instead
        //firstTimer.Enabled = true;
        SetMainLabels();

        // menutree.CollapseAll();
    }

    protected void timer_Tick(object sender, EventArgs e)
    {
        firstTimer.Enabled = false;
        secondTimer.Interval = 2 * 60 * 1000;
        secondTimer.Tick += new EventHandler<EventArgs>(secondTimer_Tick);
        secondTimer.Enabled = true;
        modalExtender.Show();
        //contentInsert = "<script>timerTick()</script>";
    }

    protected void secondTimer_Tick(object sender, EventArgs e)
    {
        LogoutUser();
        modalExtender2.Show();
        secondTimer.Enabled = false;
        firstTimer.Enabled = false;
    }

    protected void btnRedirect_Click(object sender, EventArgs e)
    {
        Response.Redirect(LOGOUT_PAGE);
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {

    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        LogoutProcess();
    }



    protected void LogoutProcess()
    {
        string logoutMessage = "Normal logout.";
        DbUse.WriteLogoutMessageToDb(logoutMessage);
        LogoutUser();
        DeleteBrowserGraphs();
        Response.Redirect(LOGOUT_PAGE);
    }

    protected void LogoutUser()
    {

        LogFiles logFiles = new LogFiles(this.username);
        logFiles.LogoutLog();
        DbUse.LogoutUser();

    }

    protected void MenuItem_Click(object sender, MenuEventArgs e)
    {
        string value = e.Item.Value;
        switch (value)
        {
            case "account":
                Response.Redirect("account.aspx");
                break;
            case "logout":
                LogoutProcess();
                break;
            case "models":
                Response.Redirect("models.aspx");
                break;
            case "help":
                PrintPdfScript(HELP_FILENAME, HELP_TITLE);
                break;
            case "sheet":
                PrintPdfScript(helpSheetFile, helpSheetTitle);
                break;
            default:
                break;
        }
    }


    public void setUser(string username)
    {
        this.username = username;
    }

    public string getUser()
    {
        return this.username;
    }

    public void setUserdir(string userdir)
    {
        this.userDir = userdir;
    }

    public void setLogoutMessage(string logoutMessage)
    {
        this.logoutMessage = logoutMessage;
    }

    public void setLastLogin(string lastLogin)
    {
        this.lastLogin = lastLogin;
    }

    public void passCurrentModelName(string currentModel)
    {
        this.currentModel = currentModel;
    }

    public void PassCurrentWhatifName(string currentWhatif)
    {
        this.currentWhatif = currentWhatif;
    }

    public void PassCurrentAnalysisName(string currentAnalysis)
    {
        this.currentAnalysis = currentAnalysis;
    }

    public string GetCurrentModel()
    {
        return currentModel;
    }

    public string GetFullCurrentModelLabel()
    {
        string currentModelText = lblModel.Text;
        int dotPosition = currentModelText.LastIndexOf(".");
        if (dotPosition != -1)
        {
            currentModelText = currentModel.Remove(dotPosition);
        };


        return currentModelText;




    }

    public string GetFullCurrentWhatifLabel()
    {
        return lblCurrentWhatif.Text;
    }



    public void ShowErrorMessage()
    {
        ShowErrorMessage(GENERAL_ERROR_MSG);
    }

    public void ShowInfoMessage(string message)
    {
        lblInfoMessage.Text = message;
        if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "changetextInfo", " $get('" + lblInfoMessage.ClientID + "').innerHTML = '" + message + "'; $get('" + btnMessageOk.ClientID + "').focus();", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "focusInfo", " setTimeout(function() { $get('" + btnMessageOk.ClientID + "').focus();}, 300)", true);
        }
        else
        {
            SetFocus(btnMessageOk.ClientID);
            modalExtenderMessage.Show();
        }



    }


    public void ShowErrorMessage(string message)
    {
        ShowErrorMessageAndFocus(message, "");
    }

    public void ShowErrorMessageAndFocus(string message, string clientIdFocus)
    {
        lblGeneralError.Text = message;
        modalExtenderError.Show();
        SetFocus(btnOkError.ClientID);
        btnOkError.OnClientClick = "HideErrorPopup('" + clientIdFocus + "'); return false;";
        if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "changetext", " $get('" + lblGeneralError.ClientID + "').innerHTML = '" + message + "'; $get('" + btnOkError.ClientID + "').focus();", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "changeclick", " $get('" + btnOkError.ClientID + "').onclick = function() { HideErrorPopup('" + clientIdFocus + "'); return false;}", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "focus", " setTimeout(function() { $get('" + btnOkError.ClientID + "').focus();}, 300)", true);
        }
        else
        {
            SetFocus(btnOkError.ClientID);
            btnOkError.OnClientClick = "HideErrorPopup('" + clientIdFocus + "'); return false;";
        }
    }

    public void ShowInfoMessage_Post(string message)
    {
        lblInfoMessagePost.Text = message;
        SetFocus(btnOkMessagePost.ClientID);
        modalPopupInfoPost.Show();
    }

    public void ShowErrorMessage_Post(string message)
    {
        lblErrorMessagePost.Text = message;
        SetFocus(btnErrorMessagePostOk.ClientID);
        extenderErrorPost.Show();
    }

    public string GetLastLogin()
    {
        return lastLogin;
    }

    public void HideMPXpopups()
    {
        modalPopupInfoPost.Hide();
        modalExtenderMessage.Hide();
        modalExtenderError.Hide();
        extenderErrorPost.Hide();
    }

    public void SetFocus(string clientId)
    {
        string script = "<SCRIPT language='javascript' type='text/javascript'>function fnFocus() { eval(\"document.getElementById('" + clientId + "').focus()\") } setTimeout(\"fnFocus()\",200);</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
    }

    public void SetFocus2(string clientId)
    {
        string script = "<SCRIPT language='javascript' type='text/javascript'>function fnFocus2() { eval(\"document.getElementById('" + clientId + "').focus()\") } setTimeout(\"fnFocus2()\",200);</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
    }

    public void ClickOnEnter(string btnClientId)
    {
        //string script = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnClientId + "').click();return false;}} else {return true};";

        string script = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnClientId + "').click();return true;}}";
        mpxForm.Attributes.Add("onkeydown", script);

    }

    public void ClickOnEnter(string btnClientId, TextBox outerControl)
    {
        string script = "doClick('" + btnClientId + "',event)";
        outerControl.Attributes.Add("onKeyPress", script);
    }

    public void ClickOnEnter(string btnClientId, AjaxControlToolkit.ComboBox outerControl)
    {
        string script = "doClick('" + btnClientId + "',event)";
        outerControl.Attributes.Add("onKeyPress", script);
    }

    public void ClickOnEnterF(string btnClientId, TextBox outerControl)
    {
        string script = "doClick('" + btnClientId + "',event);";
        outerControl.Attributes.Add("onkeydown", script);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        //mpxForm.Attributes.Add("onkeydown", "");

    }

    public void PassModelModified(bool modified)
    {
        ModelModified = modified;
    }

    public void SetModelModified(bool modified)
    {


        if (ModelModified != modified)
        {
            ModelModified = modified;
        }
    }

    protected void btnResetCountdown_Click(object sender, EventArgs e)
    {

    }

    public string GetCurrentWhatif()
    {
        return currentWhatif;
    }

    public string GetCurrentAnalysis()
    {
        return currentAnalysis;
    }

    public string GetFullCurrentWhatif()
    {  //not an analysis copy
        return lblCurrentWhatif.Text;
    }

    public string GetFullCurrentAnalysisLabel()
    {
        return lblCurrentAnalysis.Text;
    }

    public void SetCurrentAnalysisLabel()
    {
        if (currentAnalysis != null && !currentAnalysis.Equals(""))
        {
            lblCurrentAnalysis.Text = currentAnalysis;
            lblCurrentAnalysis.Visible = true;
        }
        else
        {
            lblCurrentAnalysis.Visible = false;
        }
    }

    public void SetCurrentWhatifLabel()
    {
        if (currentWhatif != null && !currentWhatif.Equals(""))
        {
            lblCurrentWhatif.Text = currentWhatif;
            lblCurrentWhatif.Visible = true;
        }
        else
        {
            lblCurrentWhatif.Visible = false;
        }
    }

    public void DeleteBrowserGraphs()
    {
        if (userDir != null)
        {
            string graphsDirectoryPath = DbUse.GetMainDirectory() + DbPage.BROWSER_DIR + "\\" + userDir + "Graphs";
            if (Directory.Exists(graphsDirectoryPath))
            {
                string[] files = Directory.GetFiles(graphsDirectoryPath);
                try
                {
                    foreach (string file in files)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }
                catch (Exception) { }

            }
            else
            {
                try
                {
                    // creates the graphs directory again
                    Directory.CreateDirectory(graphsDirectoryPath);
                }
                catch (Exception) { }
            }
        }

    }

    public void HideLoadingPopup()
    {
        mdlLoadingPopup.Hide();
    }

    public void SetHelpSheet(string filename, string title)
    {
        filename.Replace(" ", "%20");
        this.helpSheetFile = "HelpSheets/" + filename;
        this.helpSheetTitle = title.Replace(" ", "");
    }

    protected void PrintPdfScript(string filename, string title)
    {
        if (filename != null)
        {
            string script = "<SCRIPT language='javascript' type='text/javascript'>var pdf=window.open('" + filename + "','PDF', 'width=1000,height=800'); pdf.moveTo(200,100);function check() {" +
         " if(pdf.document) { try {  pdf.document.title = \"" + title + "\"; } catch (err) { } } else { setTimeout(check, 10); } } check();</SCRIPT>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script_pdf", script);

        }
    }



    public void Unnamed_Click(object sender, EventArgs e)
    {
        TableSynchronization tableSync = new TableSyncLabor(userDir);
        tableSync.SyncTables();
    }
}
