using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class error : DbPage
{
    public error() {
        PAGENAME = "error.aspx";
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected override void OnInit(EventArgs e) {
        //logFiles = new LogFiles();

        //if (IsUserSessionValid()) {
        //    logFiles.SetUsername(this.username);
        //}

    }


}