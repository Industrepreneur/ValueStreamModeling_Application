using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class API : System.Web.UI.Page
{ 

    [WebMethod(EnableSession = true)]
    public static bool NeedsReCalc()
    {
        
        //var session = HttpContext.Current.Session;
        //string username = session["username"] as string;
        string myUsername = Sessionable.GetSessionUserName();
        string myDirectory = Sessionable.GetSessionUserDirectoryPath();
        
        return Calculate.isReCalcNecessary(myUsername, myDirectory);

    }

    [WebMethod(EnableSession = true)]
    public static string ExecuteNecessary()
    {
     
        string myUsername = Sessionable.GetSessionUserName();
        string myDirectory = Sessionable.GetSessionUserDirectoryPath();
        string myCookie = Sessionable.GetSessionID();

        return Calculate.RunNecessary(myUsername, myDirectory, myCookie);

    }

    [WebMethod(EnableSession = true)]
    public static string ExecuteAll()
    {

        string myUsername = Sessionable.GetSessionUserName();
        string myDirectory = Sessionable.GetSessionUserDirectoryPath();
        string myCookie = Sessionable.GetSessionID();
        string returnable = "";
       return Calculate.RunAll(myUsername, myDirectory, myCookie);

      
        
    }

    [WebMethod(EnableSession = true)]
    public static string GetCalcProgress()
    {

        string myCookie = Sessionable.GetSessionID();

        return Calculate.Progress(myCookie);

    }

    [WebMethod(EnableSession = true)]
    public static bool CancelRun()
    {
        string myCookie = Sessionable.GetSessionID();

        return Calculate.Cancel(myCookie);

    }

}