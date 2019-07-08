using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for IbomResultPage
/// </summary>
public abstract class IbomResultPage: MultiViewPage
{

    public IbomResultPage() {
        addView("IBOM Trees", "results_iTrees_table.aspx");
        addView("IBOM Poles", "results_iPoles_table.aspx");
        
    }

    
}