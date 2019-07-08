using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Drawing;

/// <summary>
/// Summary description for ProductPage
/// </summary>
public abstract class ProductPage : SimpleGridPage {
    protected Menu productMenu;

    protected int value;

    public ProductPage() {

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

        MenuItem item = new MenuItem("Input Table", "0");

        productMenu.Items.Add(item);
        item = new MenuItem("Operations/Routing Table", "1");
        productMenu.Items.Add(item);
        item = new MenuItem("All Operations Table", "2");                                                                                                                                                                                                                                                                                                                                                                                                           
        productMenu.Items.Add(item);
        item = new MenuItem("All Routing Table", "3");
        productMenu.Items.Add(item);
        item = new MenuItem("IBOM Data Table", "4");
        productMenu.Items.Add(item);
        productMenu.Items[value].Selected = true; // select the correct tab according to the page

        Control menuContainer = GetMenuContainer();
        menuContainer.Controls.Add(productMenu);
    }

    protected abstract Control GetMenuContainer();

    protected abstract Control GetTabsDiv();

    protected void productMenu_MenuItemClick(object sender, MenuEventArgs e) {
        int itemNum = int.Parse(e.Item.Value);

        switch (itemNum) {
            case 0:
                Response.Redirect("products_table.aspx");
                break;
            case 1:
                Response.Redirect("products_oper_routing.aspx");
                break;
            case 2:
                Response.Redirect("products_oper.aspx");
                break;
            case 3:
                Response.Redirect("products_routing.aspx");
                break;
            case 4:
                Response.Redirect("products_ibom.aspx");
                break;
            default:
                break;
        }
    }
}