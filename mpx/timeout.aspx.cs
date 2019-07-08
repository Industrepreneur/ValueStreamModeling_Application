using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class TimeoutPage : DbPage
{

    public TimeoutPage()
    {
        PAGENAME = "/timeout.aspx";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
      
        string logoutMessage = "timeout";
        DbUse.WriteLogoutMessageToDb(logoutMessage);

        timeoutUser();

        Response.Redirect(LOGOUT_PAGE);

    }

    private void timeoutUser()
    {
        var mySession = HttpContext.Current.Session;
        
        DbUse.RunMySqlParams("SELECT * FROM tblGroups WHERE GroupID = ?", new string[] { "@GroupID" }, new object[] { 3 });
        DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + DbUse.LOGOUT_USERID + "', userlist.sessionid = '" + null + "', userlist.sessionexpires = '" + 0 + "' WHERE userlist.username = '" + Session["username"] + "' ; ");
        mySession.Clear();
        mySession["timeout"] = "true";
    }
  

}