using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for ProductResultPage
/// </summary>
public abstract class ProductResultPage: TableGraph
{
	protected Menu productMenu;

    protected int value;

    public ProductResultPage() {

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

        MenuItem item = new MenuItem("Production Table and Graph", "0");

        productMenu.Items.Add(item);
        item = new MenuItem("Product Manuf. Critical-path Times Graph", "1");
        productMenu.Items.Add(item);
        item = new MenuItem("Product WIP Graph", "2");
        productMenu.Items.Add(item);

        item = new MenuItem("Product Oper Details", "3");
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
                Response.Redirect("results_prod_table.aspx");
                break;
            case 1:
                Response.Redirect("results_prod_graph2.aspx");
                break;
            case 2:
                Response.Redirect("results_prod_graph3.aspx");
                break;
            case 3:
                Response.Redirect("results_prod_oper.aspx");
                break;
            default:
                break;
        }
    }
}