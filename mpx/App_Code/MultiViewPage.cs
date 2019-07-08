using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for MultiViewPage
/// </summary>
public abstract class MultiViewPage: TableGraph
{
	protected Menu productMenu;
    List<ViewItem> views;

    protected int value;

    public MultiViewPage() {
        views = new List<ViewItem>();
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        createMenu();
    }

    protected void createMenu() {
        productMenu = new Menu();
        productMenu.ID = "productMenu";
        productMenu.Orientation = Orientation.Horizontal;
        productMenu.MenuItemClick += productMenu_MenuItemClick;
        productMenu.StaticMenuItemStyle.CssClass = "productMenuItem";
        productMenu.StaticSelectedStyle.CssClass = "productMenuItemActive";

        
        for (int i=0; i< views.Count; i++) {
            ViewItem view = views[i];
            MenuItem item = new MenuItem(view.pagename, i + "");
            productMenu.Items.Add(item);
        }

        productMenu.Items[value].Selected = true; // select the correct tab according to the page

        Control menuContainer = GetMenuContainer();
        menuContainer.Controls.Add(productMenu);
    }

    protected abstract Control GetMenuContainer();

    protected abstract Control GetTabsDiv();

    protected void productMenu_MenuItemClick(object sender, MenuEventArgs e) {
        int itemNum = int.Parse(e.Item.Value);
        try {
            ViewItem view = views[itemNum];
            Response.Redirect(view.pageUrl);
        } catch (Exception) { }
        
    }

    protected void addView(string pagename, string pageUrl) {
        ViewItem view = new ViewItem(pagename, pageUrl);
        views.Add(view);
    }

    private class ViewItem {

        public string pagename;
        public string pageUrl;

        public ViewItem(string pagename, string pageUrl) {
            this.pagename = pagename;
            this.pageUrl = pageUrl;
        }
    }
}