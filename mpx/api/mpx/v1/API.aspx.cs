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
    public static void refreshSession()
    {
        
        Sessionable.refreshUserSession();

    }


}