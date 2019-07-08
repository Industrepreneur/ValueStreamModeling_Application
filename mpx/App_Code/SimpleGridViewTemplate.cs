using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for SimpleGridViewTemplate
/// </summary>
public class SimpleGridViewTemplate : System.Web.UI.Page, ITemplate {
    ListItemType templateType;
    string bindExpression;

    public SimpleGridViewTemplate(ListItemType templateType, string bindExpression) {
        this.templateType = templateType;
        this.bindExpression = bindExpression;
    }

    public SimpleGridViewTemplate(string bindExpression)
        : this(ListItemType.Item, bindExpression) {

    }

    public void InstantiateIn(System.Web.UI.Control container) {
        switch (templateType) {
            case ListItemType.Item:
                Label lblItem = new Label();
                lblItem.DataBinding += new EventHandler(this.lblItem_DataBinding);
                container.Controls.Add(lblItem);
                break;
            default:
                break;
        }
    }

    private void lblItem_DataBinding(Object sender, EventArgs e) {
        Label lc = (Label)sender;
        GridViewRow row = (GridViewRow)lc.NamingContainer;
        string propertyValue = DataBinder.Eval(row.DataItem, bindExpression).ToString();
        try {
            double num = Double.Parse(propertyValue);
            num = MyUtilities.RoundNum(num, 3);
            lc.Text = num + "";
        } catch (Exception) {
            string text = propertyValue.Replace(" ", "&nbsp;");
            lc.Text = text;
        }
        
    }
}