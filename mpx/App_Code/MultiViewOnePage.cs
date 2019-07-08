using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for MultiViewOnePage
/// </summary>
public abstract class MultiViewOnePage:TableGraph
{
	protected Menu productMenu;
    protected MultiView multiView;
    List<string> views;

    protected int value;

    public MultiViewOnePage() {
        views = new List<string>();

        wantSort = false;
        wantSort2 = false;
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        createMenu();
        multiView = GetMultiView();
    }

    protected void createMenu() {
        productMenu = new Menu();
        productMenu.ID = "productMenu";
        productMenu.Orientation = Orientation.Horizontal;
        productMenu.MenuItemClick += productMenu_MenuItemClick;
        productMenu.StaticMenuItemStyle.CssClass = "productMenuItem";
        productMenu.StaticSelectedStyle.CssClass = "productMenuItemActive";
        
        for (int i=0; i< views.Count; i++) {
            MenuItem item = new MenuItem(views[i], i + "");
            productMenu.Items.Add(item);
        }

        productMenu.Items[value].Selected = true; // select the correct tab according to the page

        Control menuContainer = GetMenuContainer();
        menuContainer.Controls.Add(productMenu);
    }

    protected abstract Control GetMenuContainer();

    protected abstract Control GetTabsDiv();

    protected virtual void productMenu_MenuItemClick(object sender, MenuEventArgs e) {
        int itemNum = int.Parse(e.Item.Value);
        try {
            productMenu.Items[itemNum].Selected = true;
            multiView.ActiveViewIndex = itemNum;
        } catch (Exception) { }
        
    }

    protected virtual void SetActiveView(int itemNum) {
        productMenu.Items[itemNum].Selected = true;
        multiView.ActiveViewIndex = itemNum;
    }

    protected void addView(string pagename) {
        views.Add(pagename);
    }

    protected abstract MultiView GetMultiView();

    // TableGraph things - return null if not used
    protected override Panel GetCopyTableContainer() {
        return null;
    }

    public override Panel GetSortPanelContainer() {
        return null;
    }

    public override Control GetSortButtonContainer() {
        return null;
    }

    protected override GridView getGridView() {
        return null;
    }

    protected override Chart getChart() {
        return null;
    }


    protected override void RefreshData() {
        
    }

    protected override void AddWhatifDisplayLabel() {
        return;
    }
}