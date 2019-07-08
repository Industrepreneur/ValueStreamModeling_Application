using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ADODB;
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;


public partial class LoginPage : DbPage
{

    //private int loginCount = 0;
    string txtError;

    private const string REDIRECT_PAGE = "/models.aspx";

    //private const int MAX_TRIES = 3;

    // javascript strings
    private const string SCRIPT_ENTER = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('";
    private const string SCRIPT_CLICK_BUTTON = "').click();return false;}} else {return true}; ";
    private const string ERROR_DUPLICATE_LOGIN = "That Username is currently logged in";
    private const string ERROR_COOKIES = "Please enable cookies in your browser settings";
    private const string ERROR_USER_ALREADY_LOGGED_IN = "This user is already logged in";


    public LoginPage()
    {
        PAGENAME = "/login.aspx";
    }

    Exception COOKIE_EXCEPTION = new Exception();
    Exception BLANK_EXCEPTION = new Exception();
    Exception CAPTCHA_EXCEPTION = new Exception();
    Exception MISSING_EXCEPTION = new Exception();

    //protected void initiateLogin(object sender, EventArgs e)
    //{
    //    string strUsername = Request.Form["txtUsername"];//should use Server.Htmlencode?
    //    strUsername = MyUtilities.clean(strUsername);
    //    string strPassword = Request.Form["txtPassword"];//should use Server.Htmlencode?

    //    bool validToStart = false;
    //    string txtError = "";
    //    try
    //    {


    //        if (!CookiesEnabled())
    //        {
    //            throw COOKIE_EXCEPTION;
    //        }
    //        else if (strUsername == "" || strPassword == "")
    //        {
    //            throw BLANK_EXCEPTION;
    //        }
    //        else if (!IsValidCaptcha())
    //        {
    //            throw CAPTCHA_EXCEPTION;
    //        }
    //        else
    //        {
    //            validToStart = true;
    //        }
    //    }
    //    catch (Exception myException)
    //    {
    //       if (myException == COOKIE_EXCEPTION)
    //        {
    //            txtError = "Please enable browser cookies!";
    //        }else if (myException == BLANK_EXCEPTION)
    //        {
    //            txtError = "Please enter a Username and Password";
    //        }else if (myException == CAPTCHA_EXCEPTION)
    //        {
    //            txtError = "Invalid Captcha";
    //        }
    //        lblError.Text = txtError;
    //        lblError.CssClass = "lblErrorVis";
    //    }

    //    if (validToStart == true)
    //    {
    //        //call login service
    //        int loginResponse = Login.ValidateLogin(strUsername, strPassword, Request.Cookies[DbUse.LOGIN_COOKIE]);

    //        switch (loginResponse)
    //        {

    //            case 0:
    //                //No cookie or session expired AND not logged AND valid user, or no session AND not logged in AND valid user
    //                login(strUsername);
    //                break;

    //            case 1:
    //                //Login error; could not establish connection to server
    //                lblError.Text = "Cannot connect to the server";
    //                lblError.CssClass = "lblErrorVis";

    //                break;

    //            case 2:
    //                //Not a real user or wrong password
    //                lblError.Text = "Invalid Username or Password";
    //                lblError.CssClass = "lblErrorVis";
    //                break;
    //            case 3:
    //                //User is logged in
    //                //dotheLogin2


    //                break;
    //            case 4:
    //                //Valid Session exists

    //                break;
    //            case 5:
    //                //Error encountered during login process
    //                lblError.Text = "Error!";
    //                lblError.CssClass = "lblErrorVis";
    //                break;
    //            default:
    //                lblError.Text = "Error!";
    //                lblError.CssClass = "lblErrorVis";
    //                break;
    //        }

    //    }

    //    Response.Redirect(PAGENAME, true);
    //}

    //private void login(string strUsername)
    //{
    //    string mySessionID = System.Web.HttpContext.Current.Session.SessionID;
    //    int sessionID = System.Web.HttpContext.Current.Session.LCID;

    //    string cookieid = MyUtilities.clean(DateTime.Now.ToString() + sessionID);
    //    cookieid = MyUtilities.clean(cookieid, ' '); // remove spaces
    //    cookieid = PasswordHash.PasswordHash.CreateHash(cookieid); //encode
    //    cookieid = MyUtilities.clean(cookieid);

    //    HttpCookie newcookie = new HttpCookie(DbUse.LOGIN_COOKIE)
    //    {
    //        Value = cookieid,

    //        Expires = DateTime.Now.AddMinutes(30)
    //    };
    //    DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGIN_COOKIE + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + username + "' ; ");
    //    Response.Cookies.Add(newcookie);

    //    SetCookieId(strUsername, cookieid, mySessionID, newcookie.Expires.Ticks);
    //    try
    //    {
    //        string lastNewLoginTime = GetMysqlDatabaseField("newLogin", cookieid);
    //        WriteLoginTimesToDb(DateTime.Now, lastNewLoginTime, cookieid);
    //        DeleteGraphs(); // delete all the graphs from last session
    //    }
    //    catch (Exception excep)
    //    {
    //        logFiles.ErrorLog(excep);
    //    }
    //}

    string TRIAL_COOKIE = "trialCookie";


    private static bool IsValidCaptcha()
    {
        //THIS IS THE PRODUCTION SECRET KEY
        //var secret = "6LftHzcUAAAAAJD-owyhsZJgY7AWpZXez5IZla5b";

        //THIS IS THE DEVELOPMENT SECRET KEY
        //var secret = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe";


        try
        {
            //using (var onSubmit = (WebRequest.Create("https://www.google.com/recaptcha/api/siteverify" + "?secret=" + secret + "&response=" + HttpContext.Current.Request.Form["g-recaptcha-response"])).GetResponse())
            //{

            //    using (StreamReader readStream = new StreamReader(onSubmit.GetResponseStream()))
            //    {
            //        string responseFromServer = readStream.ReadToEnd();
            //        if (!responseFromServer.Contains("\"success\": false"))
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }

            //    }
            //}
        }
        catch
        {
            //throw new Exception("Cannot connect to the server to validate the Captcha");

        }
        return true;
    }



    protected void Page_Load(object sender, EventArgs e)
    {

        //HttpCookie cookie = new HttpCookie(TRIAL_COOKIE)
        //{
        //    Expires = DateTime.Now.AddDays(1)
        //};
        //Response.Cookies.Add(cookie);

        if (!Page.IsPostBack)
        {

            if (Session["timeout"] != null)
            {
                if (Session["timeout"].Equals("true"))
                {
                    //THIS IS NEVER FIRING
                    ClientScript.RegisterClientScriptBlock(Page.GetType(), "isTimeout", "$(function(){alert('Session has ended');});", true);

                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(Page.GetType(), "isTimeout", "$(function(){alert('You have been logged off successfully');});", true);
                }
                //Session.Remove("timeout");
            }
            else
            {
               
            }

            //if (Request.QueryString["timeout"] != null)
            //{
            //    if (Request.QueryString["timeout"] == "true")
            //    {
            //        ClientScript.RegisterClientScriptBlock(Page.GetType(), "isTimeout", "$(function(){alert('Your Session has Ended');});", true);
            //    }
            //    else
            //    {
            //        ClientScript.RegisterClientScriptBlock(Page.GetType(), "isTimeout", "$(function(){alert('You have been logged off successfully');});", true);
            //    }
            //}
            //string script = "<SCRIPT language='javascript' type='text/javascript'>checkCookiesEnabled();</SCRIPT>";
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
            //replaced with javascript event

        }
        else
        {
            RunLogin(); //slight hack, submit button or refresh will be postback; had trouble due to aspscript manager, is removed now, may be able to create new onsubmit

        }

    }

    public static bool CookiesEnabled()
    {
        bool cookiesEnabled = false;
        string trialcookie = "trial_cookie";
        HttpCookie cookie = new HttpCookie(trialcookie);
        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        if (System.Web.HttpContext.Current.Request.Cookies[trialcookie] != null)
        {
            cookiesEnabled = true;
            //need to remove cookie instead of expiring?
            //System.Web.HttpContext.Current.Request.Cookies[trialcookie].Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Remove(trialcookie);
        }

        return cookiesEnabled;
    }


    protected void RunLogin()
    {
        string strUsername = Request.Form["txtUsername"];//should use Server.Htmlencode?
        strUsername = MyUtilities.clean(strUsername);
        string strPassword = Request.Form["txtPassword"];//should use Server.Htmlencode?




        try
        {



            if (!DbUse.CookiesEnabled())
            {
                throw new Exception(ERROR_COOKIES);
            }

            if (strUsername == "" | strPassword == "")
            {
                throw new Exception("Please Enter a Username or Password");
            }

            if (!IsValidCaptcha())
            {
                throw new Exception("Captcha Rejected!");
            }

            RecognizeUser(strUsername, strPassword);
            //IF IT DOES NOT THROW ERROR THEN USERNAME AND PASSWORD PAIR IS VALID

            bool adoOpened = DbUse.OpenAdoMysql(conn);
            string commandString = "SELECT * FROM userlist WHERE username = '" + strUsername + "';";
            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);


            string dbUserDir = rec.Fields["usersub"].Value.ToString();
            string dbCurrentModel = rec.Fields["currentModel"].Value.ToString();

            DbUse.CloseAdo(conn);
            DbUse.CloseAdoRec(rec);

            //Session["timeout"] = "false";
            //Session["username"] = strUsername;
            //Session["user-directory"] = dbUserDir;
            //Session["Basecase-model"] = dbCurrentModel;
            //this.Master.passCurrentModelName(currentModel);
            //IDEALLY SOFTWARE WILL SAVE MODEL ON TIMEOUT/LOGOUT, AND SET MODEL TO NONE

            bool modelModified = GetModelModified();

            if (currentModel.Equals("none"))
            {
                if (modelModified)
                {
                    modelModified = false;
                    SetModelModified(modelModified);

                }
            }

            //Session["isModified"] = modelModified;
            //this.Master.PassModelModified(modelModified);

            bool isWhatif = IsWhatifMode();
            //Session["isScenario"] = isWhatif;

            if (isWhatif)
            {
                //this.Master.PassCurrentWhatifName(GetCurrentWhatif());
                //Session["Scenario-model"] = GetCurrentWhatif();
            }

            bool isAnalysis = IsAnalysisMode();
            //Session["isAnalysis"] = isAnalysis;

            if (isAnalysis)
            {
                //this.Master.PassCurrentAnalysisName(GetCurrentAnalysis());
                //Session["Analysis-model"] = GetCurrentAnalysis();
            }

            Sessionable.Session myNewSession = new Sessionable.Session
            {
                USERNAME = strUsername,
                TIMEOUT = "false",
                USERDIR = dbUserDir,
                BASECASE = dbCurrentModel,
                isMODIFIED = modelModified,
                SCENARIO = GetCurrentWhatif(),
                ANALYSIS = GetCurrentAnalysis(),
                needsRECALC = true
            };

           

            Sessionable.doSessionLogin(myNewSession);
            if (myNewSession.USERNAME != "admingla")
            {
                Response.Redirect("/models.aspx", true);
            }
            else
            {
                Response.Redirect("/mpx_admin.aspx", true);
            }
            
        }
        catch (Exception ex)
        {

            //loginCount++;
            //hidden1.Value = loginCount + "";

            if (ex.Message.ToLower().IndexOf("object reference") == 0)
            {
                txtError += " " + ex.StackTrace;
            }
            else
            {
                txtError = ex.Message;
            }

        }
        lblError.Text = txtError;
        lblError.CssClass = "lblErrorVis";


    }

    //ORIGINAL
    //protected void RunLogin()
    //{
    //    string strUsername = Request.Form["txtUsername"];//should use Server.Htmlencode?
    //    strUsername = MyUtilities.clean(strUsername);
    //    string strPassword = Request.Form["txtPassword"];//should use Server.Htmlencode?

    //    //try
    //    //{
    //    //    loginCount = int.Parse(Request.Form["hidden1"]);
    //    //}
    //    //catch
    //    //{
    //    //    loginCount = 0;
    //    //}


    //    try
    //    {

    //        //if (loginCount > MAX_TRIES)
    //        //{

    //        //    throw new Exception("Too many Attempts!");

    //        //}


    //        if (!DbUse.CookiesEnabled())
    //        {
    //            throw new Exception(ERROR_COOKIES);
    //        }

    //        if (strUsername == "" | strPassword == "")
    //        {
    //            throw new Exception("Please Enter a Username or Password");
    //        }

    //        if (!IsValidCaptcha())
    //        {
    //            throw new Exception("Captcha Rejected!");
    //        }

    //        RecognizeUser(strUsername, strPassword);

    //        if (!IsUserLoggedIn(strUsername))
    //        {
    //            var mySession = HttpContext.Current.Session;
    //            string mySessionID = mySession.SessionID;
    //            int sessionID = mySession.LCID;
    //            //int sessionID = System.Diagnostics.Process.GetCurrentProcess().SessionId;
    //            string cookieid = MyUtilities.clean(DateTime.Now.ToString() + sessionID);
    //            cookieid = MyUtilities.clean(cookieid, ' '); // remove spaces
    //            cookieid = PasswordHash.PasswordHash.CreateHash(cookieid); //encode
    //            cookieid = MyUtilities.clean(cookieid);

    //            HttpCookie newcookie = new HttpCookie(DbUse.LOGIN_COOKIE)
    //            {
    //                Value = cookieid,
    //                //Expires = DateTime.Now.AddSeconds(10)
    //                Expires = DateTime.Now.AddMinutes(20)
    //            };

    //            Response.Cookies.Add(newcookie);

    //            SetCookieId(strUsername, cookieid, mySessionID, DateTime.Now.AddMinutes(20).Ticks);
    //            //Session["username"] = strUsername;


    //            try
    //            {
    //                string lastNewLoginTime = GetMysqlDatabaseField("newLogin", cookieid);
    //                WriteLoginTimesToDb(DateTime.Now, lastNewLoginTime, cookieid);
    //                DeleteGraphs(); // delete all the graphs from last session
    //            }
    //            catch (Exception excep)
    //            {
    //                logFiles.ErrorLog(excep);
    //            }


    //        }
    //        else
    //        {

    //            throw new Exception(ERROR_DUPLICATE_LOGIN);
    //            //HOW TO ASK USER IF WE CAN KICK THIS USER?
    //        }
    //        //Server.Transfer(REDIRECT_PAGE);
    //        //Server.Transfer(PAGENAME);
    //        Response.Redirect(PAGENAME, true);
    //    }
    //    catch (Exception ex)
    //    {

    //        //loginCount++;
    //        //hidden1.Value = loginCount + "";

    //        if (ex.Message.ToLower().IndexOf("object reference") == 0)
    //        {
    //            txtError += " " + ex.StackTrace;
    //        }
    //        else
    //        {
    //            txtError = ex.Message;
    //        }

    //    }
    //    lblError.Text = txtError;
    //    lblError.CssClass = "lblErrorVis";


    //}

    protected void RecognizeUser(string username, string psswd)
    {
        string sqlString = "SELECT id, usercode FROM userlog WHERE username = '" + username + "';";
        int id = -1;
        conn = new Connection();
        rec = new Recordset();
        bool openedCon = DbUse.OpenAdoMysql(conn);
        bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

        try
        {  //An error has occured. Try logging in again.
            if (!openedCon || !openedRec)
            {
                throw new Exception("An error has occured. Try logging in again.");
            }
            else if (rec.EOF)
            {
                throw new Exception("Invalid username or password.");
            }
            else
            {
                string correctHash = rec.Fields["usercode"].Value.ToString();
                if (!PasswordHash.PasswordHash.ValidatePassword(psswd, correctHash))
                {
                    throw new Exception("Invalid username or password.");
                }
                else
                {
                    try
                    {
                        id = int.Parse(rec.Fields["id"].Value.ToString());
                    }
                    catch (Exception exp)
                    {
                        logFiles.ErrorLog(exp);
                    }
                }
            }
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            if (id != -1)
            {
                // clean up previous calculation progress
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc WHERE id = " + id);
            }
        }
        catch (Exception ex)
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw ex;
        }

    }

    //protected void SetCookieId(string username, string cookieid, string sessionid, long sessionExpires)
    //{
    //    DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + cookieid + "', userlist.sessionid = '" + sessionid + "', userlist.sessionexpires = '" + sessionExpires + "' WHERE (((userlist.username)= '" + username + "' ));");

    //    LastPostbackDbUpdate(cookieid, DateTime.Now.Ticks);
    //}


    private bool IsUserLoggedIn(string username)
    {
        bool someoneIn = false;
        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT * FROM userlist WHERE username = '" + username + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        //if (!rec.EOF) {
        try
        {
            string cookieid = rec.Fields["userid"].Value.ToString();
            string oldSessionID = rec.Fields["sessionid"].Value.ToString();
            var mySessionID = HttpContext.Current.Session.SessionID;
            
            if (!cookieid.Equals(DbUse.LOGOUT_USERID) && (!oldSessionID.Equals(mySessionID)))
            {
                long lastUpdate = long.Parse(rec.Fields["lastUpdate"].Value.ToString());
                long sessionExpires = long.Parse(rec.Fields["sessionexpires"].Value.ToString());
                long currentTime = DateTime.Now.Ticks;
                //NEED TO GET DB COOKIE EXPIRE DATE; IF NOT EXPIRED, SOMEONEIN; IF EXPIRED, CLEAR DB AND RETURN FALSE
                //if (currentTime - lastUpdate < NANOSEC_100_IN_MINUTE * TIMEOUT_IN_MINUTES)
                //{
                //    someoneIn = true;
                //}

                if (DateTime.Now.Ticks < sessionExpires)
                {
                    someoneIn = true;
                    //if User passed login, prompt to kick this person out? if yes, replace cookie/session/expire
                    DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + username + "' ; ");
                    someoneIn = false;
                }
                else
                {
                    DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + username + "' ; ");
                }
            }
        }
        catch (Exception) { }
        return someoneIn;
    }


    private bool WaitForTrialCookie(string cookieid, int maxTries, int milliseconds)
    {
        bool cookieFound = false;
        int count = 0;
        HttpCookie cookie = new HttpCookie(TRIAL_COOKIE);
        while (true)
        {
            count++;
            HttpCookie trialCookie = Request.Cookies[TRIAL_COOKIE];
            if (trialCookie != null)
            {
                cookieFound = true;
                HttpContext.Current.Request.Cookies.Remove(TRIAL_COOKIE);
                break;
            }
            else if (count >= maxTries)
            {
                break;
            }
            System.Threading.Thread.Sleep(milliseconds);
        }
        return cookieFound;

    }


}


//protected void LoginProcess() {
//    //string dir;

//    string username = txtUsername.Text;
//    string password = txtPassword.Text;
//    //string userid = "";

//    try {
//        loginCount = int.Parse(hidden1.Value);
//    } catch (Exception) {
//        loginCount = 0;
//    }


//    username = MyUtilities.clean(username);


//    //}old comment out

//    LogFiles logFiles = new LogFiles(username);

//    //logFiles.LoginLog("login process start", DateTime.Now); old comment out

//    try {
//        //helpDiv.Attributes.Add("style", "display:none;");
//        //if (!CaptchaValid()) {
//        //    helpDiv.Attributes.Add("style", "display:inline;");
//        //    throw new Exception("Incorrect captcha text");
//        //}

//        //logFiles.LoginLog("login process after captcha", DateTime.Now);
//        //dir = GetDatabaseLog(username, password, userid);
//        //AddDatabaseCookieId(dir, password, userid);

//        // checks if the user is in the users database and throws exception if not
//        RecognizeUser(username, password);

//        //logFiles.LoginLog("login process after username-pswd recognition", DateTime.Now);

//        // sets the user cookieid in the database and the lastUpdate field with current time
//        if (IsSomeoneLoggedIn(username)) {
//            logFiles.DuplicateLoginLog();
//            //txtCaptcha.Text = "";
//            lblError.Text = "";
//            loginCount = 0;
//            hidden1.Value = loginCount + "";
//            lblError.Text = ERROR_DUPLICATE_LOGIN;
//            //SetFocusOnOkError();
//            logFiles.LoginLog("login process - after set focus on ok button in error message", DateTime.Now);
//            modalError.Show();
//            logFiles.LoginLog("login process - after showing error pop up", DateTime.Now);
//        } else {
//            int sessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;
//            string cookieid = MyUtilities.clean(DateTime.Now.ToString() + sessionId);
//            cookieid = MyUtilities.clean(cookieid, ' '); // remove spaces
//            // encode cookie
//            cookieid = PasswordHash.PasswordHash.CreateHash(cookieid);
//            cookieid = MyUtilities.clean(cookieid);
//            //if (cookiesEnabled) {

//            lblError.Text = "";

//            HttpCookie cookie = new HttpCookie(DbUse.LOGIN_COOKIE);
//            cookie.Value = cookieid;
//            cookie.Expires = DateTime.Now.AddDays(1);
//            Response.Cookies.Add(cookie);
//            //logFiles.LoginLog("login process - cookie starts saving on user machine", DateTime.Now);


//            // workaround - redirect again to login page, but in init of the page (session valid this time), the user gets redirected to the 
//            // last page visited in the current model
//            if (WaitForTrialCookie(cookieid, 1, 1000)) {
//                logFiles.LoginLog("login process - cookie saved", DateTime.Now);
//                SetUserCookieId(username, cookieid);
//                logFiles.LoginLog();
//                try {
//                    string lastNewLoginTime = GetMysqlDatabaseField("newLogin", cookieid);
//                    WriteLoginTimesToDb(DateTime.Now, lastNewLoginTime, cookieid);
//                    DeleteGraphs(); // delete all the graphs from last session
//                } catch (Exception excep) {
//                    logFiles.ErrorLog(excep);
//                }

//                if (Response != null) {
//                    //logFiles.LoginLog("login process - response is not null", DateTime.Now);
//                }
//                if (Request.RawUrl != null) {
//                    //logFiles.LoginLog("login process - raw url " + Request.RawUrl, DateTime.Now);
//                }

//                Response.Redirect(PAGENAME, true);

//            } else {
//                logFiles.LoginLog("login process - cookie not saved", DateTime.Now);

//                loginCount = 0;
//                hidden1.Value = loginCount + "";
//                //txtCaptcha.Text = "";

//                lblError.Text = ERROR_COOKIES;
//                //SetFocusOnOkError();
//                modalError.Show();
//            }
//        }
//    } catch (Exception ex) {
//        string error;

//        loginCount++;
//        hidden1.Value = loginCount + "";
//        //txtCaptcha.Text = "";

//        if (loginCount >= MAX_TRIES) {
//            error = "LOCKUP - Too many unsuccessful tries!!!";
//            txtUsername.Text = "";
//            txtUsername.ReadOnly = true;
//            txtPassword.ReadOnly = true;
//            //txtCaptcha.ReadOnly = true;
//            ////btnSubmit.Disabled = true;

//        } else {
//            error = ex.Message;
//            if (ex.Message.ToLower().IndexOf("object reference") == 0) {
//                error += " " + ex.StackTrace;
//            }
//            txtPassword.Focus();
//        }
//        lblError.Text = error;
//        lblError.Visible = true;
//    }
//}

////protected void SetFocusOnOkError() {
////    SetFocusOnButton(btnSubmit.ClientID);
////}

////protected void SetFocusOnOkInfo() {
////    SetFocusOnButton(btnSubmit.ClientID);
////}



//protected string GetDatabaseLog(string usern, string userp, string userid) {
//    string directory;
//    string sqlString;

//    usern = MyUtilities.clean(usern);
//    //userp = MyUtilities.clean(userp);
//    //userid = MyUtilities.clean(userid);

//    sqlString = "SELECT id, username, usercode FROM userlog WHERE username = '" + usern + "';";

//    conn = new ADODB.Connection();
//    rec = new Recordset();
//    bool openedCon = DbUse.OpenAdoMysql(conn);
//    bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

//    if (!openedCon || !openedRec) {
//        throw new Exception("Error in database access.");
//    } else if (rec.EOF) {
//        throw new Exception("Invalid username or password.");
//    } else {
//        string correctHash = rec.Fields["usercode"].Value.ToString();
//        if (!PasswordHash.PasswordHash.ValidatePassword(userp, correctHash)) {
//            throw new Exception("Invalid username or password.");
//        }
//    }

//    try {
//        directory = (string)rec.Fields["usersub"].Value;
//    } catch (Exception) {
//        if (!userp.Equals("admingla")) {
//            throw new Exception("Field not found in the database.");
//        } else {
//            directory = "";
//        }

//    }

//    DbUse.CloseAdoRec(rec);
//    DbUse.CloseAdo(conn);

//    return directory;
//}


//protected void btnResetPassword_Click(object sender, EventArgs e) {
//    DateTime time = DateTime.Now;
//    string link = "rstpwd" + time.Ticks;
//    string hashedLink = PasswordHash.PasswordHash.CreateHash(link).Substring(4).Replace(':', 's').Replace('/', 'l').Replace('+', 'r');
//    string url = DbUse.DomainPath + "pw_reset.aspx?" + hashedLink;
//    //string username = MyUtilities.clean(txtForgotPwUsername.Text);
//    string body = "You have received this email because you requested a password reset for Value Stream Modeling website. Click the following link to reset your password \n\n " + url + "\n\nIf you did not request password reset please ignore this email.";

//    LogFiles logFiles = new LogFiles(username);



//    ADODB.Connection conn;
//    ADODB.Recordset rec;

//    bool success = false;

//    conn = new ADODB.Connection();
//    rec = new ADODB.Recordset();
//    bool openedCon = DbUse.OpenAdoMysql(conn);
//    bool openedRec = DbUse.OpenAdoRec(conn, rec, "SELECT mail FROM usercred WHERE username = '" + username + "';");

//    string mail = null;
//    try {

//        if (!openedCon || !openedRec) {
//            throw new Exception("Cannot open mysql database.");
//        } else if (rec.EOF) {
//            throw new Exception("Invalid user password reset request.");
//        } else {
//            mail = rec.Fields["mail"].Value.ToString();
//            if (String.IsNullOrEmpty(mail)) {
//                throw new Exception("Cannot provide password reset link because email is not set.");
//            }
//            success = true;
//        }
//    } catch (Exception ex) {
//        logFiles.ErrorLog(ex);

//    } finally {
//        DbUse.CloseAdoRec(rec);
//        DbUse.CloseAdo(conn);
//    }



//    if (success) {
//        success = DbUse.RunMysql("UPDATE usercred SET tempHash='" + hashedLink + "', expiry = " + time.Ticks + " WHERE username = '" + username + "';");
//        try {
//            MailInfo.SendMail(body, MailInfo.PASSWORD_RESET_REQUEST, mail);
//        } catch (Exception ex) {
//            logFiles.ErrorLog(ex);
//        }
//    }
//    logFiles.PasswordResetRequest(username, success);

//    try {
//        string logBody = "Password Reset Request\n";
//        logBody += "Username: " + username + "\n";
//        logBody += "User IP: " + System.Web.HttpContext.Current.Request.UserHostAddress + "\n";
//        logBody += "Temporary link saved in database: " + (success ? "YES" : "NO");
//        MailInfo.SendMail(logBody, MailInfo.PASSWORD_RESET_REQUEST);
//    } catch (Exception ex) {
//        logFiles.ErrorLog(ex);
//    }
//    //txtCaptcha.Text = "";
//    //SetFocusOnOkInfo();
//    //modalInfo.Show();

//}

//protected void SetFocusOnButton()
//{
//    string script = "<SCRIPT language='javascript' type='text/javascript'>function fnFocus() { eval(\"document.getElementById(txtUserame.ClientId).focus()\") } setTimeout(\"fnFocus()\",200);</SCRIPT>";
//    Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
//}


//public void kickUser()
//{
//    DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGIN_COOKIE + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + username + "' ; ");
//    RunLogin();
//}
//public void ClickOnEnter(string btnClientId, TextBox outerControl) {
//    string script = "doFocus('" + btnClientId + "',event)";
//    outerControl.Attributes.Add("onkeydown", script);
//}

//public void ClickOnEnter(string btnLogin, string btnReset, TextBox outerControl) {
//    string script = "chooseFocus('" + btnLogin + "', '" + btnReset + "', event)";
//    outerControl.Attributes.Add("onkeydown", script);
//}


//protected void BtnSubmit_Click(object sender, EventArgs e)
//{
//    LoginProcess();
//}




//protected bool CaptchaValid() {
//    MSCaptcha.CaptchaControl captcha = (MSCaptcha.CaptchaControl)pnlCaptcha.FindControl("loginCaptcha");
//    if (captcha == null) {
//        throw new Exception("Internal captcha problem.");
//    } else {
//        try {
//            captcha.ValidateCaptcha(txtCaptcha.Text.Trim());
//        } catch (Exception ex) {
//            throw new Exception("The captcha expired or was filled in too fast.");
//        }
//    }
//    return captcha.UserValidated;

//}

//protected void Refresh(object sender, EventArgs e) {
//    pnlCaptcha.Update();
//    txtCaptcha.Focus();
//}