using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class equipment : DbPage
{

    public equipment() {
        PAGENAME = "/input/equipment/table2.aspx";
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }

}