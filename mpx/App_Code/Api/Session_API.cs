using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web;

public abstract class Sessionable
{
    private const string MAIN_USER_DATABASE = "mpxmdb.mdb";

    protected static ADODB.Connection conn = new ADODB.Connection();
    protected static ADODB.Recordset rec = new ADODB.Recordset();

    public class Session
    {
        public string USERNAME;
        public string USERDIR;
        public string TIMEOUT;
        public string BASECASE;
        public string SCENARIO;
        public string ANALYSIS;
        public bool isMODIFIED;
        public bool needsRECALC;
        public bool isADMIN;
        public bool isDEMO;
    }


    public static string GetSessionUserName()
    {
        var session = HttpContext.Current.Session;

        var returnable = session["username"] as string;
        return returnable;
    }

    public static string GetSessionUserDirectory()
    {
        var session = HttpContext.Current.Session;



        var returnable = session["user-directory"] as string;
        return returnable;
    }

    public static string GetSessionUserDirectoryPath()
    {
        var session = HttpContext.Current.Session;
        var returnable = SetUserDirectoryPath(GetSessionUserDirectory());

        return returnable;
    }

    public static string SetUserDirectoryPath(string userDir)
    {
        string dir = HttpContext.Current.Server.MapPath("~");
        dir += "App_Data\\";
        dir += userDir;

        return dir;
    }

    public static string GetSessionUserModelDirectoryPath()
    {
        var session = HttpContext.Current.Session;
        var returnable = SetUserModelDirectoryPath(GetSessionUserDirectory());

        return returnable;

    }

    public static string SetUserModelDirectoryPath(string userDir)
    {

        string dir = HttpContext.Current.Server.MapPath("~");
        dir += "App_Data\\";
        dir += userDir;
        dir += MAIN_USER_DATABASE;
        return dir;

    }

    public static string GetSessionID()
    {
        var session = HttpContext.Current.Session;

        var returnable = session.SessionID as string;
        return returnable;
    }

    public static string getUserCookie()
    {
        var returnable = HttpContext.Current.Request.Cookies[DbUse.LOGIN_COOKIE].Value;


        return returnable;
    }

    public static void refreshUserSession()
    {

        DbUse.RunMysql("UPDATE userlist SET userlist.sessionexpires = '" + DateTime.Now.AddMinutes(20).Ticks + "' WHERE userlist.sessionid = '" + GetSessionID() + "';");
        //HttpContext.Current.Response.Cookies[DbUse.LOGIN_COOKIE].Expires = DateTime.Now.AddMinutes(20);

    }

    public static void doSessionLogin(Sessionable.Session myNewSession)
    {

        var session = HttpContext.Current.Session;

        session["username"] = myNewSession.USERNAME;
        session["user-directory"] = myNewSession.USERDIR;
        session["timeout"] = myNewSession.TIMEOUT;
        session["basecase"] = myNewSession.BASECASE;
        session["scenario"] = myNewSession.SCENARIO;
        session["analysis"] = myNewSession.ANALYSIS;
        session["isModified"] = myNewSession.isMODIFIED;
        session["needsRecalc"] = true;
        if (myNewSession.USERNAME.Equals("admingla"))
        {
            session["isAdmin"] = false;
        }
        else
        {
            session["isDemo"] = false;
        }



        try
        {
            DbUse.RunMysql("UPDATE userlist SET userlist.userid = '" + String.Empty + "', userlist.sessionid = '" + GetSessionID() + "' WHERE userlist.username = '" + GetSessionUserName() + "' ; ");
        }
        catch
        {

        }


    }

    public static bool ValidateSession()
    {
        bool returnable = false;
        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT * FROM userlist WHERE username = '" + GetSessionUserName() + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        string dbSessionID = rec.Fields["sessionid"].Value.ToString();
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);

        if (!String.IsNullOrEmpty(dbSessionID) && GetSessionID().Equals(dbSessionID))
        {
            returnable = true;
        }
        return returnable;
    }

}
