using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;

/// <summary>
/// Summary description for WhatifGridPage
/// </summary>
public abstract class WhatifGridPage: CommonGridPage
{

    public WhatifGridPage() {
        nonEdits = true;
        isWhatif = true;
    }

    protected override GridView GenerateGridControl() {
        GridView gridBase = base.GenerateGridControl();
        gridBase.ShowFooter = false;
        return gridBase;

    }

}