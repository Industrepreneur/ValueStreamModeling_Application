using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CreateLogFiles
/// </summary>
public class LogFilesDemo
{

    private string sLogDay;
    private string sPathToLogDir;
    private string errFileName;
    private string logFileName;

    private string userIp;
    private string userName;
    private string userCompany;
    private string serialNumber;

    private string rootDirectory;

    private const string LOG_FILES_DIR = "LogASPdemo";
    private const string ERR_DIR = "Errors\\";
    private const string INFO_DIR = "MarketingInfo\\";

    public const string DEMO_DOWNLOAD = "DEMO DOWNLOAD";
    

    public LogFilesDemo(string rootdirectory): this(rootdirectory, "none", "none", "none") {
        
    }
    

	public LogFilesDemo(string rootDirectory, string companyName, string userName, string serialNumber) {
        this.rootDirectory = rootDirectory;
        this.userName = userName;
        this.userCompany = companyName;
        this.serialNumber = serialNumber;
        
        sPathToLogDir = rootDirectory + LOG_FILES_DIR + "\\";
        userIp = System.Web.HttpContext.Current.Request.UserHostAddress;

        string errDir = sPathToLogDir + ERR_DIR;
        string infoDir = sPathToLogDir + INFO_DIR;

        

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

        //this variable used to create log filename format "
        //for example filename : ErrorLogYYYYMMDD
        string sYear = DateTime.Now.Year.ToString();
        string sMonth = DateTime.Now.Month.ToString();
        string sDay = DateTime.Now.Day.ToString();
        sLogDay = sYear + "-" + sMonth + "-" + sDay;
        errFileName = errDir +  "ErrLog_" + sLogDay;
        logFileName = infoDir + "InfoLog_" + sLogDay;
	}

    public void SetUserDetails(string companyName, string username, string serialNumber) {
        this.userName = username;
        this.userCompany = companyName;
        this.serialNumber = serialNumber;
    }

    public void ErrorLog(Exception ex) {
        string sErrorTime = DateTime.Now.ToString();
        string sErrMsg = "";
        
        
        sErrMsg += "Exception message: " + ex.Message + "\n";
        string exceptionType = "Exception type: ";
        exceptionType += ex.GetType().ToString() + "\n";
        sErrMsg += exceptionType;
        sErrMsg += "Stack Trace: " + ex.StackTrace + "\n";
        sErrMsg += "Company: " + userCompany + "\n";
        sErrMsg += "User: " + userName + "\n";
        sErrMsg += "Serial Number: " + serialNumber + "\n";
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

    public void DownloadDemoLog() {
        InfoLog(DEMO_DOWNLOAD);
    }

    public void ErrorMessageLog(string message) {
        string sErrorTime = DateTime.Now.ToString();
        string sErrMsg = "";


        sErrMsg += "Error message: " + message + "\n";
        sErrMsg += "Company: " + userCompany + "\n";
        sErrMsg += "User: " + userName + "\n";
        sErrMsg += "Serial Number: " + serialNumber + "\n";
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

    public void InfoLog(string logType, DateTime dateTime) {
        try {
            using (StreamWriter sw = new StreamWriter(logFileName + ".log", true)) {
                //sLogFormat used to create log files format :
                // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
                string sLogTime = dateTime.ToShortDateString().ToString() + " " + dateTime.ToLongTimeString().ToString() + " " + dateTime.Millisecond + " ms";
                string logMessage = logType + ", " + sLogTime + ", ";
                logMessage += "Company: " + userCompany + ", ";
                logMessage += "User: " + userName + ", ";
                logMessage += "Serial Number: " + serialNumber + ", ";
                logMessage += "IP Address: " + userIp;
                sw.WriteLine(logMessage);
                sw.Flush();
                sw.Close();
            }
        } catch (Exception) { }
    }

    public void InfoLog(string logType) {
        InfoLog(logType, DateTime.Now);
    }
     
}