<%@ WebHandler Language="C#" Class="Progress" %>

using System;
using System.Web;

public class Progress : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        string type = context.Request.QueryString["type"];
        context.Response.Write(GetProgressReport(type).ToString());
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

    protected string GetDirectory() {
        string dir = HttpContext.Current.Server.MapPath("~");
        dir = dir + "\\";
        return dir;
    }

    public ProgressReport GetProgressReport(string type) {
        long secondsLeft = 0;

        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();

        int timePerCalc = -1;
        int total = -1;
        int calc = -1;
        string myName = "";

     
           
           
                bool adoOpened = DbUse.OpenAdoMysql(conn);
                string commandString = "SELECT usercalc.id, usercalc.total, usercalc.calc, usercalc.timePerCalc, userCalc.lastCheck, userCalc.curName, userlist.lastUpdate, userlist.userid FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';";
                bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
                //if (!rec.EOF) {
                try {
                    long lastCheck = long.Parse(rec.Fields["lastCheck"].Value.ToString());
                    long lastUpdate = long.Parse(rec.Fields["lastUpdate"].Value.ToString());

                    timePerCalc = int.Parse(rec.Fields["timePerCalc"].Value.ToString());
                    total = int.Parse(rec.Fields["total"].Value.ToString());
                    calc = int.Parse(rec.Fields["calc"].Value.ToString());
                    myName = Convert.ToString(rec.Fields["curName"].Value);
                    long currentTime = DateTime.Now.Ticks;
                    secondsLeft = DbPage.TIMEOUT_IN_MINUTES * DbPage.NANOSEC_100_IN_MINUTE - (currentTime - lastUpdate);
                    secondsLeft = secondsLeft * 60 / DbPage.NANOSEC_100_IN_MINUTE;


                } catch (Exception) { }
                finally {
                    DbUse.CloseAdo(conn);
                    DbUse.CloseAdoRec(rec);
                }
                //}
            
        

        return new ProgressReport(calc, total, timePerCalc, type, myName);
    }



    public class ProgressReport {
        public int currentCalc;
        public int totalCalc;
        public int timePerCalc;
        public string type;
        public string modelName;

        public ProgressReport(int currentCalc, int totalCalc, int timePerCalc, string type, string modelName) {
            this.currentCalc = currentCalc;
            this.totalCalc = totalCalc;
            this.timePerCalc = timePerCalc;
            this.type = type;
            this.modelName = modelName;
        }

        public override string ToString() {
            string report = "";
            if (!(currentCalc == -1 && totalCalc == -1 && timePerCalc == -1))
            {


                if (currentCalc == -1 && totalCalc == 0)
                {
                    report = "Verifying data...";
                }
                if (currentCalc > 0 && totalCalc > 0) {
                    report = "Calculating " + modelName + " (" + currentCalc + " of " + totalCalc + ")";
                    //report += "Calculating " + currentCalc + " out of " + totalCalc + " ...";
                }
                if (timePerCalc != -1) {
                    //report += "<br />" + "About " + GetTimeLeft(timePerCalc * (totalCalc - currentCalc + 1)) + " remaining";
                }
                if (currentCalc == -1 && totalCalc == -1 && timePerCalc == -1) {
                    if (type == null || (!type.Equals("a") && !type.Equals("A"))) {
                        report = "Verifying data...";
                    } else {
                        report = "Saving Vision Analysis...";
                    }
                }
            }
            else
            {
                report = null;
            }

            return report;
        }

        public string GetTimeLeft(int ticks) {
            TimeSpan timeSpan = new TimeSpan(ticks);
            string timeLeft = "";
            int hours = (int) timeSpan.TotalHours;
            int mins = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            if (hours > 0) {
                timeLeft += hours + " hour";
                if (hours > 1) {
                    timeLeft += "s";
                }
            } else if (mins > 0) {
                timeLeft += mins + " minute";
                if (mins > 1) {
                    timeLeft += "s";
                }
            } else {
                timeLeft += seconds + " second";
                if (seconds > 1) {
                    timeLeft += "s";
                }
            }

            return timeLeft;
        }

    }

}