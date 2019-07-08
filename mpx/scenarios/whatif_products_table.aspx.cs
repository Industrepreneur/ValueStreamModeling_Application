using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_products_table : WhatifGridPage
{
    ProductDelegatePage helperProduct;
    
    public whatif_products_table() {
        PAGENAME = "whatif_products_table.aspx";
        featureHelper = new ProductTableDelegate();
        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;

        int value = 0;
        helperProduct = new ProductDelegatePage(value);
        
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        pnlMenu.Controls.Add(new LiteralControl("<h2>What-If: Products</h2>"));
        base.OnInit(e);
        if (!IsWhatifMode()) {
            string whatifPart = "whatif_";
            Response.Redirect(PAGENAME.Substring(whatifPart.Length));
        }
        helperProduct.SetMenuContainer(pnlMenu);
        helperProduct.OnInit(e);
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        tableSync = new TableSyncProduct(userDir);
        //srcProductsList.DataFile = dataFile;
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