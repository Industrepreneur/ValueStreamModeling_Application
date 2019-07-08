<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup


        // check whether any calculations were running before and perform clean up
        // delete run files and remove records from MySql usercalc table
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        try
        {


            DbUse.OpenAdoMysql(conn);
            string commString = "SELECT usercalc.id, userlist.username, userlist.usersub FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id;";
            DbUse.OpenAdoRec(conn, rec, commString);

            string dataDir = Server.MapPath("~");
            if (!dataDir.EndsWith("\\"))
            {
                dataDir += "\\";

            }
            dataDir += "App_Data\\";
            if (!rec.RecordCount.Equals(-1))
            {
                while (!rec.EOF)
                {
                    try
                    {
                        string userDir = rec.Fields["usersub"].Value.ToString();
                        string name = rec.Fields["username"].Value.ToString();
                        DbUse.DeleteRunFile(dataDir + userDir, name, true);


                    }
                    catch (Exception ex)
                    {
                        LogFiles logFiles = new LogFiles();
                        logFiles.ErrorLog(ex);
                    }
                    rec.MoveNext();
                }
            }
            else
            {
                //rec is empty
            }




        }
        catch (Exception exp)
        {
            LogFiles logFiles2 = new LogFiles();
            logFiles2.ErrorLog(exp);
        }
        finally
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            DbUse.RunMysql("DELETE usercalc.* FROM usercalc;");
        }


    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown


    }

    void Application_Error(object sender, EventArgs e)
    {
        // Get the error details
        HttpException lastErrorWrapper = Server.GetLastError() as HttpException;

        Exception lastError = lastErrorWrapper;
        if (lastErrorWrapper.InnerException != null)
        {
            lastError = lastErrorWrapper.InnerException;
        }

        string lastErrorTypeName = lastError.GetType().ToString();
        string lastErrorMessage = lastError.Message;
        string lastErrorStackTrace = lastError.StackTrace;

        // get the user associated with the error

        string username = null;
        if (Session != null)
        {

            ADODB.Connection conn = new ADODB.Connection();
            ADODB.Recordset rec = new ADODB.Recordset();
            bool adoOpened = DbUse.OpenAdoMysql(conn);
            string commandString = "SELECT * FROM userlist WHERE sessionid = '" + Session.SessionID + "';";
            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);

            try
            {
                long lastUpdate = long.Parse(rec.Fields["lastUpdate"].Value.ToString());
                long currentTime = DateTime.Now.Ticks;
                if (currentTime - lastUpdate < DbPage.NANOSEC_100_IN_MINUTE * DbPage.TIMEOUT_IN_MINUTES)
                {
                    username = rec.Fields["username"].Value.ToString();
                }
            }
            catch (Exception) { }
            finally
            {
                DbUse.CloseAdoRec(rec);
                DbUse.CloseAdo(conn);
            }

        }

        LogFiles logFiles = new LogFiles(username);
        logFiles.ServerErrorLog(lastError);

        string userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
        string body = string.Format("Url: {0}\n\n User Identity: {1}\n\n Username: {2}\n\n IP Address: {3}\n\n Exception Type: {4}\n\n Message: {5}\n\n Stack Trace: {6}\n\n",
            DbUse.DomainPath + Request.RawUrl,
            User.Identity.Name,
            username != null ? username : "Unknown",
            userIp,
            lastErrorTypeName,
            lastErrorMessage,
            lastErrorStackTrace);


        // Attach the Yellow Screen of Death for this error   
        string YSODmarkup = lastErrorWrapper.GetHtmlErrorMessage();
        System.Net.Mail.Attachment YSOD = null;
        if (!string.IsNullOrEmpty(YSODmarkup))
        {
            YSOD = System.Net.Mail.Attachment.CreateAttachmentFromString(YSODmarkup, "YSOD.htm");
        }



        string errorPage = "";
        if (lastErrorWrapper.GetHttpCode() == 404)
        {
            try
            {
                MailInfo.SendMailx(body, MailInfo.PAGE_NOT_FOUND, null, YSOD);
            }
            catch (Exception) { }
            errorPage += "/ErrorPages/404.aspx";
        }
        else
        {
            try
            {
                MailInfo.SendMailx(body, MailInfo.UNHANDLED_EXCEPTION, null, YSOD);
            }
            catch (Exception) { }
            errorPage += "/ErrorPages/error.aspx";

        }
        Server.Transfer(errorPage);

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        if (Session != null)
        {
            if (Session["timeout"] != null && !Session["timeout"].Equals("true"))
            {
                Session["timeout"] = false;

            }
            else
            {

            }
        }

        Session.Timeout = 60 * 4;


    }

    void Session_End(object sender, EventArgs e)
    {

        Session.Clear();
        // Code that runs when a session ends. 
        //THIS SHOULD SAVE CURRENT MODEL AND UNLOAD IT
        //var mySession = HttpContext.Current.Session;
        //string mySessionID = mySession.SessionID;
        //string myTimeout = mySession["timeout"] as string;
        //DbUse.RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
        //DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + Session["username"] + "' ; ");
        //mySession.Clear();
        //mySession["timeout"] = myTimeout;
        //if (HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE] != null)
        //{
        //    HttpContext.Current.Response.Cookies.Remove(DbUse.LOGIN_COOKIE);
        //}


    }

</script>
