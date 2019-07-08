using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class LogoutPage : DbPage
{

    public LogoutPage()
    {
        PAGENAME = "/logout.aspx";
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        string logoutMessage = "logout";
        DbUse.WriteLogoutMessageToDb(logoutMessage);

        logoutUser();

        Response.Redirect(LOGOUT_PAGE);

    }

    private void logoutUser()
    {
        var mySession = HttpContext.Current.Session;

        DbUse.RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
        DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + Session["username"] + "' ; ");
        mySession.Clear();
        mySession["timeout"] = "false";
    }

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    LogoutProcess();

    //}

    //protected void LogoutProcess()
    //{
    //    string logoutMessage = "Normal logout.";
    //    DbUse.WriteLogoutMessageToDb(logoutMessage);
    //    LogoutUser();
    //    DeleteBrowserGraphs();
    //    Session["timeout"] = "false";
    //    Response.Redirect(LOGOUT_PAGE);
    //}


    //protected void LogoutUser()
    //{
    //    LogFiles logFiles = new LogFiles(this.username);
    //    logFiles.LogoutLog();
    //    DbUse.LogoutUser();
    //}

    //public void DeleteBrowserGraphs()
    //{
    //    if (userDir != null)
    //    {
    //        string graphsDirectoryPath = DbUse.GetMainDirectory() + DbPage.BROWSER_DIR + "\\" + userDir + "Graphs";
    //        if (Directory.Exists(graphsDirectoryPath))
    //        {
    //            string[] files = Directory.GetFiles(graphsDirectoryPath);
    //            try
    //            {
    //                foreach (string file in files)
    //                {
    //                    File.SetAttributes(file, FileAttributes.Normal);
    //                    File.Delete(file);
    //                }
    //            }
    //            catch (Exception) { }

    //        }
    //        else
    //        {
    //            try
    //            {
    //                // creates the graphs directory again
    //                Directory.CreateDirectory(graphsDirectoryPath);
    //            }
    //            catch (Exception) { }
    //        }
    //    }

    //}

}