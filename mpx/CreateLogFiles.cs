using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CreateLogFiles
/// </summary>
public class LogFiles
{

    private string sLogDay;
    private string sPathToLogDir;
    private string errFileName;
    private string logFileName;
    private string serverErrFileName;
    private string pwResetFileName;

    private string userIp;
    private string userName;

    private const string LOG_FILES_DIR = "LogASP";
    private const string ERR_DIR = "Errors\\";
    private const string INFO_DIR = "MarketingInfo\\";
    private const string SERVER_ERR_DIR = "ServerErrors\\";
    private const string PASSWORD_RESET = "PasswordReset\\";

    public const string LOGIN = "LOGIN";
    public const string LOGOUT = "LOGOUT";
    public const string DUPLICATE_LOGIN = "DUPLICATE LOGIN";
    public const string RUN = "RUN";
    public const string RUN_END = "END OF RUN";
    public const string DUPLICATE_RUN_END = "END OF DUPLICATE RUN (NO CALC)";
    public const string LOAD_MODEL = "LOAD MODEL";
    public const string LOAD_DIAGRAM = "LOAD DIAGRAM";
    public const string USER_CREATED = "USER CREATED";
    public const string USER_DELETED = "USER DELETED";

    public LogFiles(): this("Unknown") {
        
    }
    

	public LogFiles(string userName)
	{
        if (userName == null) {
            userName = "Unknown";
        }
        sPathToLogDir = DbUse.GetRootDirectory() + LOG_FILES_DIR + "\\";
        userIp = "Not Applicable";
        try {
            userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
        } catch (HttpException) {
            // request not available
        }

        string errDir = sPathToLogDir + ERR_DIR;
        string infoDir = sPathToLogDir + INFO_DIR;
        string serverErrDir = sPathToLogDir + SERVER_ERR_DIR;
        string pwResetDir = sPathToLogDir + PASSWORD_RESET;

        this.userName = userName;

        DirectoryInfo logDir = new DirectoryInfo(infoDir);
        if (!logDir.Exists) {
            try {
            Directory.CreateDirectory(infoDir);
            } catch (Exception) { }
        }
        logDir = new DirectoryInfo(errDir);
        if (!logDir.Exists) {
            try {
                Directory.CreateDirectory(errDir);
            } catch (Exception) { }
        }
        logDir = new DirectoryInfo(serverErrDir);
        if (!logDir.Exists) {
            try {
                Directory.CreateDirectory(serverErrDir);
            } catch (Exception) { }

        }

        logDir = new DirectoryInfo(pwResetDir);
        if (!logDir.Exists) {
            try {
                Directory.CreateDirectory(pwResetDir);
            } catch (Exception) { }

        }

        //this variable used to create log filename format "
        //for example filename : ErrorLogYYYYMMDD
        string sYear = DateTime.Now.Year.ToString();
        string sMonth = DateTime.Now.Month.ToString();
        string sDay = DateTime.Now.Day.ToString();
        sLogDay = sYear + "-" + sMonth + "-" + sDay;
        errFileName = errDir +  "ErrLog_" + sLogDay;
        logFileName = infoDir + "InfoLog_" + sLogDay;
        serverErrFileName = serverErrDir + "ServerErrLog_" + sLogDay;
        pwResetFileName = pwResetDir + "PwReset_" + sLogDay;
	}

    public void SetUsername(string username) {
        this.userName = username;
    }

    public void ErrorLog(Exception ex) {
        ErrorLog(ex, errFileName);
    }

    public void ErrorLog(Exception ex, string errorFileName) {
        string sErrorTime = DateTime.Now.ToString();
        string sErrMsg = "";
        
        
        sErrMsg += "Exception message: " + ex.Message + "\n";
        string exceptionType = "Exception type: ";
        exceptionType += ex.GetType().ToString() + "\n";
        sErrMsg += exceptionType;
        sErrMsg += "Stack Trace: " + ex.StackTrace + "\n";
        sErrMsg += "User: " + userName + "\n";
        sErrMsg += "IP Address: " + userIp + "\n";
        try {
            using (StreamWriter sw = new StreamWriter(errorFileName + ".log", true)) {
                //sLogFormat used to create log files format :
                // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            
                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                sw.WriteLine(sLogFormat + sErrMsg);
                sw.Flush();
                sw.Close();
            }
        } catch (Exception) { }
    }

    public void ServerErrorLog(Exception ex) {
        ErrorLog(ex, serverErrFileName);
    }

    public void LoginLog() {
        InfoLog(LOGIN);
    }
    
    public void RunLog() {
        InfoLog(RUN);
    }

    public void RunEndLog() {
        InfoLog(RUN_END);
    }

    public void RunLogi(string info) {
        InfoLog(RUN + " " + info);
    }

    public void LoginLog(string info, DateTime dateTime) {
        InfoLog("LOGIN " + info, dateTime);
    }

    public void DuplicateRunEndLog() {
        InfoLog(DUPLICATE_RUN_END);
    }

    public void LoadModelLog() {
        InfoLog(LOAD_MODEL);
    }

    public void DiagramsLog() {
        InfoLog(LOAD_DIAGRAM);
    }

    public void DuplicateLoginLog() {
        InfoLog(DUPLICATE_LOGIN);
    }

    public void LogoutLog() {
        InfoLog(LOGOUT);
    }

    public void LogoutLog(DateTime dateTime) {
        InfoLog(LOGOUT, dateTime);
    }

    public void ErrorMessageLog(string message) {
        string sErrorTime = DateTime.Now.ToString();
        string sErrMsg = "";


        sErrMsg += "Error message: " + message + "\n";
        sErrMsg += "User: " + userName + "\n";
        try {
            using (StreamWriter sw = new StreamWriter(errFileName + ".log", true)) {
                //sLogFormat used to create log files format :
                // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message

                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                sw.WriteLine(sLogFormat + sErrMsg);
                sw.Flush();
                sw.Close();
            }
        } catch (Exception) { }
    }

    public void PasswordResetRequest(string username, bool success) {
        string sErrorTime = DateTime.Now.ToString();
        string sErrMsg = "";

        sErrMsg += "User: " + userName + "\n";
        sErrMsg += "IP Address: " + userIp + "\n";
        sErrMsg += "Status: " + (success ? "successful" : "unsuccessful") + "\n";
        try {
            using (StreamWriter sw = new StreamWriter(pwResetFileName + ".log", true)) {
                //sLogFormat used to create log files format :
                // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message

                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                sw.WriteLine(sLogFormat + sErrMsg);
                sw.Flush();
                sw.Close();
            }
        } catch (Exception) { }
    }

    public void InfoLog(string logType, DateTime dateTime, string username) {
        try {
            using (StreamWriter sw = new StreamWriter(logFileName + ".log", true)) {
                //sLogFormat used to create log files format :
                // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
                string sLogTime = dateTime.ToShortDateString().ToString() + " " + dateTime.ToLongTimeString().ToString() + " " + dateTime.Millisecond + " ms";
                string logMessage = logType + ", " + sLogTime + ", ";
                logMessage += "User: " + (username == null ? userName : username) + ", ";
                logMessage += "IP Address: " + userIp;
                sw.WriteLine(logMessage);
                sw.Flush();
                sw.Close();
            }
        } catch (Exception) { }
    }

    public void InfoLog(string logType, DateTime dateTime) {
        InfoLog(logType, DateTime.Now, null);
    }

    public void InfoLog(string logType) {
        InfoLog(logType, null);
    }

    public void InfoLog(string logType, string username) {
        InfoLog(logType, DateTime.Now, username);
    }
     
}