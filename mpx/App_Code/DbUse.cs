using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Configuration;

/// <summary>
/// Summary description for DbUse
/// </summary>
public static class DbUse {
    public const string LOGIN_COOKIE = "loginCookie";

    public const string LASTPAGE_COOKIE = "lastPageCookie";

    public const string DEFAULT_COOKIE_ID = "c73492";

    public const string RUN_COOKIE = "runCookie";

    public const string LOGOUT_PAGE = "login.aspx";

    public const string USERNAME_DATABASE = "Database22.mdb";

    public const string DEMO_MODEL = "Gthubs.mdb";

    public const string LOGOUT_USERID = "c73492";

    public const string UPLOAD_TEMP_DIR = "UploadStorage";

    public const string GRAPHS_DIR = "Graphs";

    public const string INSERT_DATA_ERROR_MSG = "Some of the data entered are incorrect. This can be caused by duplicates, missing or invalid values. Please correct the data.";

    public const string UPDATE_DATA_ERROR_MSG = "Data could not get updated. Some of the data entered are incorrect.";

    public const string OPER_NAME_ERROR_MSG = "An operation with the same operation name and a different operation number already exists. Multiple operations with the same name may appear in the model but they all need the same operation number and the total % assigned must be equal to 100%.";

    public const string OPER_SECONDARY_MSG = "There are other operation records with the same operation name and same operation number for this product. This is correct if you (a) want the same operation done in 2 or more ways and (b) the sum of the Percent assigned to these operations is equal to 100%.";

    public const string OPER_SAME_NAME_DIFF_NUM = "There are other operation records with the same operation name but different operation numbers for this product.";

    public const string OPER_DIFF_NAME_SAME_NUM = "There are other operation records with a different operation name and same operation number for this product.";

    public const string DEFAULT_ROUTING_WARNING = "If you click on the Add Default Routing button you may not get the routing you expect. Please review the routing records if you click on the Add Default Routing button.";

    public const string DUPLICATE_OPER_WARNING = "Note to User: <b>Routings are based upon operation names not operation numbers.</b> So, <b>for each product</b>, if there are multiple operations with the same name and different numbers the operation numbers are ignored and the operations are assumed to be the same and have the same numbers. Likewise if two or more operations have the same number but different names the operations are assumed to have different names and the numbers are ignored.";

    private static int SIZE1 = 20;

    private static int SIZE2 = 10;

    public const string CURRENT_PAGE_COOKIE = "current_page";

    public const string COMMON_RUN_FILE = "dllInProcess.run";

    public const string MESSAGE_WHATIF_DISPLAY = "Note to User: Visit/Use <a href=\"whatif_display.aspx\">What-If Display</a> page to turn on/off Scenario results to be displayed.";

    public static bool CookiesEnabled() {
        bool cookiesEnabled = false;
        string trialcookie = "trial_cookie";
        HttpCookie cookie = new HttpCookie(trialcookie);
        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        if (System.Web.HttpContext.Current.Request.Cookies[trialcookie] != null) {
            cookiesEnabled = true;
            //need to remove cookie instead of expiring?
            //System.Web.HttpContext.Current.Request.Cookies[trialcookie].Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Remove(trialcookie);
        }

        
        return cookiesEnabled;
    }

    public static bool OpenAdo(ADODB.Connection conn, string dbpath) {
        bool opened;

        // close the connection if it already exists
        CloseAdo(conn);
        try {
            conn.ConnectionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source  = " + dbpath + "; Persist Security Info = False";
            conn.Mode = (ADODB.ConnectModeEnum)ADODB.CursorLocationEnum.adUseServer;
            conn.Open();
            opened = true;
        } catch (Exception ex) {
            // MyUtilities.MsgBox("Error in opening database connection: " + conn.ConnectionString + " (" + e.Message + ")");
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
            opened = false;
            //throw new Exception(e.Message);
        }
        return opened;
    }

    public static bool OpenAdoMysql(ADODB.Connection conn) {
        bool opened;

        // close the connection if it already exists
        CloseAdo(conn);

        try {
           //  gwwd - nont here at start ... conn.DefaultDatabase = "";
            //conn.ConnectionString = "Provider=MySQLProv;Data Source=webmpx; Persist Security Info = False";
            //conn.ConnectionString = "Provider=MySQLProv;" + GetUserDatabaseConnection().ToString();

            /* using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
             * */
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString;
            conn.Mode = (ADODB.ConnectModeEnum)ADODB.CursorLocationEnum.adUseServer;
            conn.Open();
            opened = true;
        } catch (Exception e) {
            // MyUtilities.MsgBox("Error in opening database connection: " + conn.ConnectionString + " (" + e.Message + ")");
            opened = false;
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(e);
            //throw new Exception(e.Message);
        }
        return opened;
    }

    public static void CloseAdo(ADODB.Connection db) {
        // close the connection if it exists
        if (db != null && db.State != 0) {
            try {
                db.Close();
            } catch (Exception) { }
        }
    }


    public static bool OpenAdoRec_R(ADODB.Connection globaldb, ADODB.Recordset rec, string connectionString, ADODB.CursorTypeEnum typelink)
    {
        bool opened;

        CloseAdoRec(rec);
        try
        {
           
            // Open table.
            rec.CursorType = typelink; // ADODB.CursorTypeEnum.adOpenDynamic; // all types of movement through Recordset are allowed
            rec.LockType = ADODB.LockTypeEnum.adLockPessimistic;
            rec.Open(connectionString, globaldb);
            opened = true;
        }
        catch (Exception ex)
        {
            // MyUtilities.MsgBox("Error in opening recordset.");
            opened = false;
            //throw new Exception(ex.Message);
        }
        // Response.Write(opened); - for debugging purposes
        return opened;
    }


    public static bool OpenAdoRec(ADODB.Connection globaldb, ADODB.Recordset rec, string connectionString) {
        bool opened;

        CloseAdoRec(rec);
        try {
            // Open table.
            
            rec.CursorType = ADODB.CursorTypeEnum.adOpenDynamic; // all types of movement through Recordset are allowed
            rec.LockType = ADODB.LockTypeEnum.adLockPessimistic;
            rec.Open(connectionString, globaldb);
            opened = true;
        } catch (Exception ex) {
            // MyUtilities.MsgBox("Error in opening recordset.");
            opened = false;
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
            //throw new Exception(ex.Message);
        }
        // Response.Write(opened); - for debugging purposes
        return opened;
    }

    public static void CloseAdoRec(ADODB.Recordset rec) {
        if (rec != null && rec.State != 0) {
            try {
                rec.Close();
            } catch (Exception) { }
        }
    }

    public static bool open_ado(ref ADODB.Connection conn, string dbpath) {
        if (conn == null) {
            conn = new ADODB.Connection();
        }
        return OpenAdo(conn, dbpath);
    }

    public static bool open_ado_rec(ADODB.Connection conn, ref ADODB.Recordset rec, string connectionString, int contype) {
        return open_ado_rec(conn, ref rec, connectionString);
    }

    public static bool open_ado_rec(ADODB.Connection conn, ref ADODB.Recordset rec, string connectionString) {
        if (rec == null) {
            rec = new ADODB.Recordset();
        }
        return OpenAdoRec(conn, rec, connectionString);
    }

    public static bool RunSql(string commandString, string connectionString) {
        bool updated;
        try {
            using (OleDbConnection oleConn = new OleDbConnection(connectionString)) {
                oleConn.Open();
                using (OleDbCommand comm = new OleDbCommand(commandString, oleConn)) {
                    try {
                        comm.ExecuteNonQuery();
                    } catch (Exception ex) {
                        Exception excep = new Exception("Error in executing oledb command. Command string: " + comm.CommandText + ". Exception message: " + ex.Message, ex);
                        throw excep;
                    }
                    oleConn.Close();
                    updated = true;
                }
            }
        } catch (Exception ex) {
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
            updated = false;
        }
        return updated;
    }

    public static bool RunMySqlParams(string commandString, string[] paramNames, object[] parameters) {
        bool updated;
        string conn_string = GetConnectionString();
        try {
            using (MySqlConnection oleConn = new MySqlConnection(conn_string)) {
                oleConn.Open();
                using (MySqlCommand comm = new MySqlCommand(commandString, oleConn)) {
                    try {
                        for (int i = 0; i < parameters.Length; i++) {
                            comm.Parameters.AddWithValue(paramNames[i], parameters[i]);
                        }
                        comm.ExecuteNonQuery();
                    } catch (Exception ex) {
                        Exception excep = new Exception("Error in executing mysql command. Command string: " + comm.CommandText + ". Exception message: " + ex.Message, ex);
                        throw excep;
                    }
                    oleConn.Close();
                    updated = true;
                }
            }
        } catch (Exception ex) {
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
            updated = false;
        }
        return updated;
    }

    public static void LogoutUser(string myUsername)
    {
        RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });

        //RunMysql("UPDATE userlist SET userlist.userid = '" + LOGOUT_USERID + "' WHERE userlist.username = '" + myUsername + "' ; ");
        DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + myUsername + "' ; ");
      
        
    }

    //public static void LogoutUser() {
    //    RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
    //    string cookieid = "0";

    //    // get the value of login cookie and delete it by modifying expiration date
    //    HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[DbUse.LOGIN_COOKIE];


    //    if (cookie != null)
    //    {
    //        cookieid = MyUtilities.clean(cookie.Value);
    //        System.Web.HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE].Expires = DateTime.Now.AddDays(-1);
    //        RunMysql("UPDATE userlist SET userlist.userid = '" + LOGOUT_USERID + "' WHERE userlist.userid = '" + cookieid + "' ; ");
    //        HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE].Value = null;
    //    }

    //}

    public static void xxx_LogoutUser()
    {
        HttpContext.Current.Session.Abandon();
    }

    public static void LogoutUser()
    {

        if (HttpContext.Current.Session.IsNewSession)
        {
            HttpContext.Current.Session["timeout"] = "true";
            //All logout actions are done in global.asax
        }
        else
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session["timeout"] = "false";
            string mySessionID = HttpContext.Current.Session.SessionID;
            DbUse.RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
            //DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + mySessionID + "', userlist.sessionexpires = '" + HttpContext.Current.Session.Timeout + "' WHERE userlist.sessionid = '" + mySessionID + "' ; ");
            DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.sessionid = '" + mySessionID + "' ; ");

        }
        
        HttpContext.Current.Response.Redirect("/login.aspx");
       
        // get the value of login cookie and delete it by modifying expiration date
        //RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
        //HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[DbUse.LOGIN_COOKIE];
        //string mySessionID = HttpContext.Current.Session.SessionID;

        //    System.Web.HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE].Expires = DateTime.Now.AddDays(-1);
        //    HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE].Value = null;
       
        //    RunMysql("UPDATE userlist SET userlist.userid = '" + LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.sessionid = '" + mySessionID + "' ; ");
        
        //MUST CHECK COOKIE EXPIRATION ON ANY DB RELATED EVENT, OTHERWISE WILL CAUSE DESYNC!!!!!
        //isSessionValid and isUserLoggedIn

        //need to create html unload event
        //need to handle case of closing app after cookie expired?
        //need to clear sessionID in DB when app closes

        //need to check on login if cookie expired, or is user lost cookies but username has cookie that is expired; need to store expire time on session end
        //if cookies match AND has not expired, set new session ID
        //if cookies do not match, but username has cookie and cookie is valid????
        //if cookies do not match, but username has cookie and cookie is invalid????
        //

    }

    public static void TestMySql() {
        RunMySqlParams("UPDATE usercred SET mail=@mail WHERE id = @id;", new string[] { "@mail", "@id" }, new object[] { "luc.hanakova@gmail.com", 10 });
    }

    public static void WriteLogoutMessageToDb(string logoutMessage) {


        RunMysql("UPDATE userlist SET logoutMessage = '" + logoutMessage + "' WHERE sessionid = '" + HttpContext.Current.Session.SessionID + "' ; ");


    }


    public static bool LastPostbackDbUpdate(string cookieid, long updateTime) {
        return RunMysql("UPDATE userlist SET lastUpdate = " + updateTime + " WHERE sessionid = '" + HttpContext.Current.Session.SessionID + "';");

    }

    public static bool RunMysql(string commandString) {
        bool updated = false;
        string conn_string = GetConnectionString();

        using (MySqlConnection conn = new MySqlConnection(conn_string))
        using (MySqlCommand cmd = conn.CreateCommand()) {    //watch out for this SQL injection vulnerability below
            cmd.CommandText = commandString;
            try {
                conn.Open();
                try {
                    cmd.ExecuteNonQuery();

                } catch (Exception exp) {
                    throw new Exception("Error in executing MySql command. Command string: " + commandString + ". Exception message: " + exp.Message, exp);
                }
                updated = true;
            } catch (Exception ex) {
                LogFiles logFiles = new LogFiles();
                logFiles.ErrorLog(ex);
            } finally {
                conn.Close();
            }

        }
        return updated;
    }

    public static string GetMysqlDatabaseField(string field, string key, string keyValue, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();

        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT " + key + ", " + field + " FROM " + table + " WHERE " + key + " = '" + keyValue + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public static string GetMysqlDatabaseField(string field, string key, int keyValue, string table) {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();

        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT " + key + ", " + field + " FROM " + table + " WHERE " + key + " = " + keyValue + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try {
            entry = rec.Fields[field].Value.ToString();
        } catch (Exception) {
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }



    public static string GetMainDirectory() {
        string helperPage = DbUse.LOGOUT_PAGE;
        string dir = HttpContext.Current.Request.MapPath(helperPage);
        dir = dir.Substring(0, dir.IndexOf(helperPage));
        return dir;
    }

    public static string GetRootDirectory() {
        string root = HttpContext.Current.Server.MapPath("~");
        
        if (!root.EndsWith("\\")) {
            root += "\\";
        }
        return root;
    }

    public static bool RunSqlAdo(ADODB.Connection openConn, string queryString) {
        object ret;
        bool success = false;
        try {
            try {
                openConn.Execute(queryString, out ret, 0);
            } catch (Exception ex) {
                throw new Exception("Error in executing ado command. Command string: " + queryString + ". Exception message: " + ex.Message, ex);
            }
            success = true;
        } catch (Exception ex) {
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
        }
        return success;
    }

    public static void CreateDllInProcessFile() {
        CreateDllInProcessFile("Unknown");
    }

    public static void CreateDllInProcessFile(string username) {
        LogFiles logFiles = new LogFiles();
        string fullPath = GetMainDirectory() + "Bin\\" + COMMON_RUN_FILE;
        try {
            using (StreamWriter runFile = new StreamWriter(fullPath, false)) {
                runFile.WriteLine(DateTime.Now.Ticks);
                runFile.Close();
            }
            logFiles.ErrorMessageLog("Created common fun file '" + fullPath + "'.");
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            throw ex;
        }
    }

    public static bool IsDllRunning() {
        bool running = false;
        string pathToDllProcessFile = GetMainDirectory() + "Bin\\" + COMMON_RUN_FILE;
        if (File.Exists(pathToDllProcessFile)) {

            try {
                using (StreamReader runFile = new StreamReader(pathToDllProcessFile)) {
                    string runTime = runFile.ReadLine();
                    long timeElapsedMins = (DateTime.Now.Ticks - long.Parse(runTime)) / DbPage.NANOSEC_100_IN_MINUTE;
                    if (timeElapsedMins < 5) {
                        running = true;
                    }
                    runFile.Close();
                }
            } catch (Exception ex) {

            }
        }
        return running;
    }

    public static bool WaitForDllFinish() {
        string pathToRunFile = GetMainDirectory() + "Bin\\" + COMMON_RUN_FILE;
        return FileOperations.WaitForFile(pathToRunFile, 1000, 30);
    }

    public static void DeleteCommonRunFile() {
        string pathToRunFile = GetMainDirectory() + "Bin\\" + COMMON_RUN_FILE;
        if (FileOperations.WaitForFile(pathToRunFile, 50, 100)) {
            try {
                File.Delete(pathToRunFile);
            } catch (Exception ex) {
                LogFiles logFiles = new LogFiles();
                logFiles.ErrorLog(ex);
            }
        }
    }

    public static bool InRunProcess(string userdir, bool fullpath) {
        bool inRun = false;
        string pathToRunFile = fullpath ? "" : GetMainDirectory() + "App_Data\\";
        pathToRunFile += userdir + CalcClass.RUN_FILE;
        if (File.Exists(pathToRunFile)) {

            try {
                using (StreamReader runFile = new StreamReader(pathToRunFile)) {
                    string runTime = runFile.ReadLine();
                    long timeElapsedMins = (DateTime.Now.Ticks - long.Parse(runTime)) / DbPage.NANOSEC_100_IN_MINUTE;
                    if (timeElapsedMins < 5) {
                        inRun = true;
                    }
                }
            } catch (Exception ex) {

            }
        }
        //string cookieName = DbUse.RUN_COOKIE + username;
        //HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[cookieName];
        //if (cookie != null) {
        //    inRun = true;
        //}
        return inRun;
    }

    public static bool InRunProcess(string userdir) {
        return InRunProcess(userdir, false);
    }

    public static void CreateRunFile(string userdirPath, string username) {
        try {
            using (StreamWriter runFile = new StreamWriter(userdirPath + CalcClass.RUN_FILE, false)) {
                runFile.WriteLine(DateTime.Now.Ticks);
                runFile.Close();
            }

        } catch (Exception ex) {
            LogFiles logFiles = new LogFiles(username);
            logFiles.ErrorLog(ex);
        }
    }

    public static string toRedColor(string message) {
        return "<span style=\"color:#FF4500\" >" + message + "</span>";
    }

    public static string removeRedColor(string message) {
        return message.Replace("<span style=\"color:#FF4500\" >", "|RED").Replace("</span>", "RED|");
    }

    public static string removeRedColorFlash(string message) {
        return message.Replace("<span style=\"color:#FF4500\" >", "").Replace("</span>", "");
    }

    public static string reproduceRedColor(string message) {
        return message.Replace("|RED", "<span style=\"color:#FF4500\" >").Replace("RED|", "</span>");
    }

    public static void DeleteRunFile(string userdirPath, string username) {
        DeleteRunFile(userdirPath, username, false);
    }

    public static void DeleteRunFile(string userdirPath, string username, bool logSuccess) {
        string pathToRunFile = userdirPath + CalcClass.RUN_FILE;
        LogFiles logFiles = new LogFiles(username);
        if (FileOperations.WaitForFile(pathToRunFile, 500, 3)) {
            try {
                File.Delete(pathToRunFile);
                if (logSuccess) {
                    logFiles.ErrorMessageLog("Run file deleted successfully.");
                }
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
            }
        } else if (logSuccess) {
            logFiles.ErrorMessageLog("Run file '" + pathToRunFile + "' not found for user " + username + ".");
        }
    }

    public static string DomainPath {
        get {
            //Return variable declaration
            var appPath = string.Empty;

            //Getting the current context of HTTP request
            var context = HttpContext.Current;

            //Checking the current context content
            if (context != null) {
                //Formatting the fully qualified website url/name
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80 ?
                                             string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }

    public static string GetEmailAddress(string username) {
        using (MySqlConnection conn = new MySqlConnection(DbUse.GetConnectionString())) {
            using (MySqlCommand cmd = new MySqlCommand("SELECT userlog.mail FROM userlog INNER JOIN userlist ON userlog.id = userlist.id WHERE username = @username;", conn)) {
                try {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@username", username);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {
                        string mail = reader["mail"].ToString();
                        conn.Close();
                        return mail;
                    }
                    return null;

                } catch (Exception ex) {
                    LogFiles logFiles = new LogFiles(username);
                    logFiles.ErrorLog(ex);
                    return null;
                }
            }
        }
    }

    public static string GetConnectionString() {
        return ConfigurationManager.ConnectionStrings["MySQLConnStrNoDriver"].ConnectionString;
    }






}