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
public class ProductDelegatePage {
    protected Menu productMenu;

    protected int value;
    protected Control menuContainer;

    public ProductDelegatePage(int value) {
        this.value = value;
    }

    public void SetMenuContainer(Control menuContainer) {
        this.menuContainer = menuContainer;
    }

    public void OnInit(EventArgs e) {
        
    }
    
    
}