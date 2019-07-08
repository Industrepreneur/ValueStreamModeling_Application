using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web;

public class ApiUtil
{
    public const string MAIN_USER_DATABASE = "mpxmdb.mdb";
    private static readonly string maxUtilQuery = "SELECT tblgeneral.utlimit FROM tblgeneral";

    //THIS SHOULD BE SetUserModelDirectoryPath
    public static string GetUserDatabasePath(string userDir)
    {
        string dir = HttpContext.Current.Server.MapPath("~");
        dir += "App_Data\\";
        dir += userDir;
        dir += MAIN_USER_DATABASE;
        return dir;
    }

    public static string SetUserDirectoryPath(string userDir)
    {
        string dir = HttpContext.Current.Server.MapPath("~");
        dir += "App_Data\\";
        dir += userDir;
        
        return dir;
    }

    public static void SetSessionInfo(string userDir)
    {
        var session = HttpContext.Current.Session;
        session["user-model-directory"] = GetUserDatabasePath(userDir);
        //THIS SHOULD BE USER-MODEL-DIRECTORY-PATH
        //THIS SHOULD NOT BE A SESSION VALUE? INSTEAD GRAB SESSION USERDIR AND CALL GETUSERDATABASEPATH????
        
        session["user-directory"] = userDir;
    }

    

    public static string GetSessionUserModelDirectory()
    {
        var session = HttpContext.Current.Session;

        // Only in debug mode, load a particular user
        if (HttpContext.Current.IsDebuggingEnabled)
        {
            if (session.Count == 0)
            {
                // use a test account
                return GetUserDatabasePath("stevie3\\");
            }
        }


        var dir = session["user-model-directory"] as string;
        return dir;
    }

    public static string GetSessionUserDirectory()
    {
        var session = HttpContext.Current.Session;



        var dir = session["user-directory"] as string;
        return dir;
    }

    public static string GetSessionUserDirectoryPath()
    {
        var session = HttpContext.Current.Session;
        var dir = SetUserDirectoryPath(GetSessionUserDirectory());

        return dir;
    }

    public static string GetSessionUserName()
    {
        var session = HttpContext.Current.Session;



        var dir = session["username"] as string;
        return dir;

    }
   
}