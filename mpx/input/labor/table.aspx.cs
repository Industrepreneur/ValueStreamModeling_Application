using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class labor : InputGridPage
{
    
    public labor() {

        PAGENAME = "/input/labor/table.aspx";

        featureHelper = new LaborDelegate();
        
    }
    
    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
    }

    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        return featureHelper.GetCommandString(commandType, commandString);
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        base.OnInit(e);
        if (IsWhatifMode()) {
            Response.Redirect("/scenarios/whatif_labor.aspx");
        }
        tableSync = new TableSyncLabor(userDir);
        string sheet = "Cheat Sheat Labor Input Page";
        Master.SetHelpSheet(sheet + ".pdf", sheet);

    }

    protected override Control getButtonDiv() {
        return buttondiv;
    }

    protected override Panel GetSecondPanel() {
        return secondPanel;
    }

    protected override Panel GetThirdPanel() {
        return thirdPanel;
    }

    protected override Panel GetFourthPanel() {
        return fourthPanel;
    }

    protected override Panel GetFifthPanel() {
        return fourthPanel;
    }


}