using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_labor : WhatifGridPage
{

    public whatif_labor()
	{
        PAGENAME = "/scenarios/whatif_labor.aspx";        
        featureHelper = new LaborDelegate();
        
        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;

	}

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
    }

    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        return featureHelper.GetCommandString(commandType, commandString);
    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();        
        base.OnInit(e);
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        tableSync = new TableSyncLabor(userDir);
      
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

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