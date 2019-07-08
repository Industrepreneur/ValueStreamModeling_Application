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

public partial class api_v1_products : System.Web.UI.Page
{
    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public static String ping()
    {
        return "Pong";
    }
}