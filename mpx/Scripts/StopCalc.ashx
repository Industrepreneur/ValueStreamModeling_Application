<%@ WebHandler Language="C#" Class="StopCalc" %>

using System;
using System.Web;

public class StopCalc : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.Write(StopCalculations());
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    public string StopCalculations() {
       
            DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET cancel = 1 WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
            
        
        return "Cancelling calculations...";
    }

}