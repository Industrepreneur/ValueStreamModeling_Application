using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;

/// <summary>
/// ASP page class which implements basic feature of displaying and editing grid data
/// </summary>
public abstract class CommonGridPage : SortablePage
{
    protected string TABLE_NAME
    {
        get
        {
            if (ViewState["TABLE_NAME"] == null)
            {
                ViewState["TABLE_NAME"] = "";
            }
            return (string)ViewState["TABLE_NAME"];
        }
        set { ViewState["TABLE_NAME"] = value; }
    }

    protected bool wantTwoHeaders = false;

    protected virtual void SetupSecondHeaders()
    {
        featureHelper.SetupSecondHeaders();
        info = featureHelper.info;
    }

    //property for storing of information about merged columns
    protected MergedColumnsInfo info;

    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        //call the method for custom rendering the columns headers 
        if (e.Row.RowType == DataControlRowType.Header)
            e.Row.SetRenderMethodDelegate(RenderHeader);
    }

    private bool ShouldMergeColsInStandardMode()
    {
        bool shouldMerge = false;
        for (int i = 1; i < info.MergedColumns.Count; i++)
        {
            int colNext = info.MergedColumns[i];
            int colPrev = info.MergedColumns[i - 1];
            if (colNext - colPrev == 1 && !ADVANCED_FIELDS[colNext] && !ADVANCED_FIELDS[colPrev]
                    && !info.StartColumns.Contains(colNext))
            {
                shouldMerge = true;
                break;
            }
        }
        return shouldMerge;
    }

    //method for rendering the columns headers 
    protected void RenderHeader(HtmlTextWriter output, Control container)
    {

        bool shouldMerge = true;
        int currentMergedCol = 0;
        if (mode.Equals("Standard"))
        {
            shouldMerge = ShouldMergeColsInStandardMode();
        }
        List<int> mergedCols = new List<int>();
        for (int i = 0; i < info.MergedColumns.Count; i++)
        {
            mergedCols.Add(info.MergedColumns[i]);
        }

        for (int i = 0; i < container.Controls.Count; i++)
        {
            TableCell cell = (TableCell)container.Controls[i];

            //stretch non merged columns for two rows
            if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i])
            {
                if (!info.MergedColumns.Contains(i))
                {
                    if (shouldMerge)
                    {
                        cell.Attributes["rowspan"] = "2";
                    }
                    cell.RenderControl(output);
                }
                else
                {
                    int k = i;
                    while (!info.StartColumns.Contains(k))
                    {
                        k--;
                    }
                    string title = "";
                    if (HEADER_TOOLTIPS != null)
                    {
                        title = " title=\"" + HEADER_TOOLTIPS[i] + "\"";
                    }
                    if (!shouldMerge)
                    {
                        output.Write(string.Format("<th{0}>{1}</th>", title,
                                    info.Titles[k] + " (" + HEADERS[i] + ")"));
                    }
                    else
                    {
                        int length = int.Parse(info.StartColumns[k].ToString());
                        int hiddenCols = 0;
                        if (mode.Equals("Standard"))
                        {
                            for (int j = k; j < k + length; j++)
                            {
                                if (ADVANCED_FIELDS[j])
                                {
                                    hiddenCols++;
                                }
                            }
                        }
                        int numMergedCols = length - hiddenCols;
                        if (numMergedCols > 1 && currentMergedCol < k)
                        {
                            output.Write(string.Format("<th align='center' colspan='{0}'>{1}</th>",
                                        length - hiddenCols, info.Titles[k]));
                        }
                        else if (numMergedCols == 1)
                        {
                            mergedCols.Remove(i);
                            output.Write(string.Format("<th{0} rowspan='2'>{1}</th>", title,
                                    info.Titles[k] + " (" + HEADERS[i] + ")"));

                        }
                    }
                    if (k > currentMergedCol)
                    {
                        currentMergedCol = k;

                    }

                }
            }

        }

        if (shouldMerge)
        {
            //close the first row 
            output.Write("</tr>");
            //set attributes for the second row

            grid.HeaderStyle.AddAttributesToRender(output);
            //start the second row
            output.RenderBeginTag("tr");

            //render the second row (only the merged columns)
            for (int i = 0; i < mergedCols.Count; i++)
            {
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[mergedCols[i]])
                {
                    TableCell cell = (TableCell)container.Controls[mergedCols[i]];
                    cell.RenderControl(output);
                }

            }
        }

    }

    protected FeatureDelegate featureHelper;


    protected string tableQueryString;

    protected Panel pnlMainGrid;
    protected DataTable dt;
    protected GridView grid;

    public bool nonEdits = false;
    protected bool[] fieldsNonEditable;

    protected bool hasAdvanced = true;

    protected OleDbConnection connec = null;

    protected static string GRID_NAME = "grid";
    protected string GRID_CONTAINER = "gridPanel";

    protected string[] FIELDS;
    protected bool[] ADVANCED_FIELDS;
    protected string[] HEADERS;
    protected string[] HEADER_TOOLTIPS;

    protected bool[] COMBOS;
    protected bool[] CHECKBOXES;

    protected string[] TEXT_BOX_IDS;
    protected string[] LABEL_IDS;

    public const string BASE_CONTROL_NAME = "Edit";
    public const string LBL_ROW = "lblRow"; // might not need!!!!
    public const string BUTTON_DIV = "buttondiv";

    protected Panel secondPanel; // some might be not needed!!!
    protected Panel pnlPopupLine;
    protected Panel thirdPanel;

    protected AjaxControlToolkit.ModalPopupExtender extenderInfo;

    protected bool isWhatif = false;
    protected bool wantCopyButton = true;


    /********************************************************/
    // Copy table stuff
    protected RadioButton rdbtnTableWithHeaders;
    protected RadioButton rdbtnTableWithoutHeaders;
    protected CheckBox boxCheckAll;
    protected AjaxControlToolkit.ModalPopupExtender extenderCopy;
    protected TextBox txtCopyTable;
    protected Panel pnlCopyTable;
    /*******************************************************/

    public const string TXT_INSERT_TABLE = "txtInsertTable";

    protected Button btnInsertMultipleWarningOk;


    public static class Command
    {
        public const int SELECT = 0;
        public const int INSERT = 1;
        public const int UPDATE = 2;
        public const int DELETE = 3;
    }

    protected static class IDs
    {
        public const int TEXT_BOX = 0;
        public const int LABEL = 1;
    }

    protected string mode
    {
        get
        {
            if (ViewState["mode"] == null)
            {
                ViewState["mode"] = "Standard";
            }
            return (string)ViewState["mode"];
        }
        set { ViewState["mode"] = value; }

    }

    protected Button btnAdvanced;
    protected Label lblAdvanced;

    void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = "~/MasterPage.master";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Page.IsPostBack)
        {
            this.SetData();

        }

    }

    /****************************************************/
    // Methods for generating and event handling for the GridView
    protected virtual GridView GenerateGridControl()
    {
        GridView newGrid = new GridView();
        SetGridProperties(newGrid);
        GenerateColumns(newGrid);
        return newGrid;
    }

    //DEFINE PROPERTIES OF THE GRID HERE
    public static void SetGridProperties(GridView newGrid, string keyField)
    {

        newGrid.DataKeyNames = new string[] { keyField };
        newGrid.AllowPaging = false;
        newGrid.AllowSorting = false;

        newGrid.AutoGenerateColumns = false;
        //newGrid.HeaderStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        //newGrid.FooterStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        newGrid.RowStyle.CssClass = "datatable-rowstyle";
        //newGrid.AlternatingRowStyle.BackColor = ColorTranslator.FromHtml("#f3f3f3");
        newGrid.BorderWidth = 0;
        newGrid.Style.Add("min-width", "100%");
        newGrid.ShowHeaderWhenEmpty = true;
        newGrid.PagerStyle.CssClass = "pager";

        newGrid.EmptyDataText = "There are no data records to display.";

        //newGrid.BorderColor = Color.Black;
        //newGrid.BorderWidth = 3;
        //newGrid.BorderStyle = BorderStyle.Solid;
    }

    private void SetGridProperties(GridView newGrid)
    {
        SetGridProperties(newGrid, FIELDS[0]);
        newGrid.ID = GRID_NAME;
        newGrid.ShowFooter = isWhatif ? false : true;

        newGrid.PageIndexChanging += new GridViewPageEventHandler(this.PageIndexChanging);
        newGrid.RowDataBound += new GridViewRowEventHandler(this.Grid_RowDataBound);
        newGrid.RowCommand += new GridViewCommandEventHandler(this.grid_RowCommand);
        newGrid.RowUpdating += new GridViewUpdateEventHandler(this.grid_RowUpdating);
        newGrid.RowDeleting += new GridViewDeleteEventHandler(this.grid_RowDeleting);
        newGrid.RowEditing += new GridViewEditEventHandler(this.grid_RowEditing);
        if (wantTwoHeaders)
        {
            newGrid.RowCreated += GridView_RowCreated;
        }


    }

    private void GenerateColumns(GridView newGrid)
    {
        // the buttons column
        TemplateField templ = new TemplateField();
        GridViewTemplate btnTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.BUTTONS, FIELDS[0], BASE_CONTROL_NAME + 0);
        btnTemplate.whatif = isWhatif;
        btnTemplate.wantCopyButton = wantCopyButton;
        templ.ItemTemplate = btnTemplate;
        templ.EditItemTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.BUTTONS, FIELDS[0], null);
        if (!isWhatif)
        {
            templ.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.BUTTONS, FIELDS[0], null);
        }
        newGrid.Columns.Add(templ);

        // data columns - standard and edit mode; footer

        for (int i = 1; i < FIELDS.Length; i++)
        {

            TemplateField template = new TemplateField();

            if (HEADER_TOOLTIPS != null)
            {
                template.HeaderTemplate = new GridViewTemplate(HEADERS[i], HEADER_TOOLTIPS[i], LABEL_IDS[i]);
            }
            else
            {
                template.HeaderText = HEADERS[i];

            }



            // standard mode
            GridViewTemplate itemTemplate;
            if (CHECKBOXES[i])
            {
                itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], LABEL_IDS[i]);
            }
            else
            {
                itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS[i], LABEL_IDS[i]);
            }
            template.ItemTemplate = new CompiledBindableTemplateBuilder(delegate (Control container)
            {

                itemTemplate.InstantiateIn(container);
            },
                delegate (Control container)
                {
                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS.Length; j++)
                    {
                        if (CHECKBOXES[j])
                        {
                            dict[FIELDS[j]] = ((CheckBox)container.FindControl(LABEL_IDS[j])).Checked;
                        }
                        else
                        {
                            dict[FIELDS[j]] = ((Label)container.FindControl(LABEL_IDS[j])).Text;
                        }
                    }
                    return dict;
                });

            // edit mode
            GridViewTemplate editTemplate;
            if (COMBOS[i] && (!nonEdits || !fieldsNonEditable[i]))
            {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList(FIELDS[i]));
            }
            else if (CHECKBOXES[i])
            {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            }
            else
            {
                if (nonEdits && fieldsNonEditable[i])
                {
                    editTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                }
                else
                {
                    editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                }

            }

            template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate (Control container)
            {
                editTemplate.InstantiateIn(container);
            },
                delegate (Control container)
                {

                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS.Length; j++)
                    {
                        if (nonEdits && fieldsNonEditable[j])
                        {
                            dict[FIELDS[j]] = ((Label)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        }
                        else if (COMBOS[j])
                        {
                            dict[FIELDS[j]] = ((AjaxControlToolkit.ComboBox)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        }
                        else if (CHECKBOXES[j])
                        {
                            dict[FIELDS[j]] = ((CheckBox)container.FindControl(TEXT_BOX_IDS[j])).Checked;
                        }
                        else
                        {

                            dict[FIELDS[j]] = ((TextBox)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        }
                    }
                    return dict;
                });

            // footer
            if (!isWhatif)
            {
                if (COMBOS[i])
                {
                    template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList(FIELDS[i]));
                }
                else if (CHECKBOXES[i])
                {
                    template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                }
                else
                {
                    template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                }

            }

            // add new column

            newGrid.Columns.Add(template);
        }
    }

    protected void grid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //grid.EditIndex = e.NewEditIndex;
        //this.SetData();
    }

    protected void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }

    protected void grid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (PAGENAME.Equals("labor.aspx") || PAGENAME.Equals("whatif_labor.aspx"))
        {
            RowUpdate(e.RowIndex);
            SyncTables();
            SetModelModified(true, false);
        }

    }

    protected void RowUpdateCancel()
    {
        grid.EditIndex = -1;
        this.SetData();

    }



    protected virtual void grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        Button btn = e.CommandSource as Button;
        if (btn == null)
        {
            return;
        }

        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        if (e.CommandName.Equals("Update"))
        {
            RowUpdate(rowIndex);
            SyncTables();
            SetModelModified(true, false);
            //need to pass function what type of command it is; if update, only set THIS model as true; if insert or delete AND BC, set all true
        }
        else if (e.CommandName.Equals("CancelUpdate"))
        {
            RowUpdateCancel();
        }
        else if (e.CommandName.Equals("Edit"))
        {
            grid.EditIndex = rowIndex;
            this.SetData();
            try
            {
                //int _rowIndex = int.Parse(e.CommandArgument.ToString());
                int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                Control editControl = grid.Rows[rowIndex].FindControl(TEXT_BOX_IDS[_columnIndex]) as Control;
                if (editControl != null && (editControl is TextBox || editControl is AjaxControlToolkit.ComboBox || editControl is CheckBox))
                {
                    editControl.Focus();

                }

            }
            catch (Exception)
            {

            }
        }
        try
        {
            Master.HideLoadingPopup();
        }
        catch (Exception) { }

    }

    /******************************************************************/

    protected void btnAdvanced_Click(object sender, EventArgs e)
    {
        
        if (btnAdvanced.Text.Equals("Expand"))
        {
            mode = "Advanced";
            btnAdvanced.Text = "Hide Optional Parameters";
            lblAdvanced = pnlMainGrid.FindControl("lblAdvancedMode") as Label;
            lblAdvanced.ToolTip = "compress additional columns";
            lblAdvanced.Text = "<i class='fas fa-eye-slash fa-fw row-icon'></i><span>SHOW/HIDE</span>";


        }
        else
        {
            mode = "Standard";
            btnAdvanced.Text = "Expand";
            lblAdvanced = pnlMainGrid.FindControl("lblAdvancedMode") as Label;
            lblAdvanced.ToolTip = "expand additional columns";
            lblAdvanced.Text = "<i class='fas fa-eye-slash fa-fw-slash row-icon'></i><span>SHOW/HIDE</span>";

        }

        for (int i = 0; i < FIELDS.Length; i++)
        {
            if (ADVANCED_FIELDS[i])
            {
                grid.Columns[i].Visible = !grid.Columns[i].Visible;
            }
           
        }

        if (PAGENAME.Equals("/input/products/bom.aspx"))
        {
            CheckBox myCheck = pnlMainGrid.FindControl("checkAllSubComponents") as CheckBox;
            myCheck.Checked = !myCheck.Checked;
        }
        this.SetData();
    }

    protected void HideAdvancedColumns()
    {
        for (int i = 0; i < FIELDS.Length; i++)
        {
            if (ADVANCED_FIELDS[i])
            {
                grid.Columns[i].Visible = false;
            }
        }
    }

    public override Panel GetSortPanelContainer()
    {
        secondPanel = GetSecondPanel();
        return secondPanel;
    }

    public override Control GetSortButtonContainer()
    {
        Control buttondiv = getButtonDiv();
        return buttondiv;

    }

    protected abstract Panel GetThirdPanel();

    protected override void RefreshData()
    {
        this.SetData();
    }

    protected virtual List<string> GetDropList(string name)
    {
        return new List<string>();
    }

    protected virtual void SetData()
    {
        grid = pnlMainGrid.FindControl(GRID_NAME) as GridView;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string comm = GetCommandString(Command.SELECT);
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.SELECT), connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try
            {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                bool empty = false;
                if (dt.Rows.Count == 0)
                {
                    empty = true;
                    DataRow dr = dt.NewRow();
                    dt.Rows.Add(dr);
                }
                DataView dv = dt.DefaultView;


                grid.DataSource = dv;
                grid.DataBind();
                string copiedTable = "";
                TableString = "";
                TableStringHeaders = "";
                if (empty)
                {
                    // hide the buttons
                    GridViewRow row = grid.Rows[0];
                    Button button = row.FindControl(GridViewTemplate.BTN_COPY) as Button;
                    if (button != null)
                    {
                        button.Visible = false;
                    }
                    button = row.FindControl(GridViewTemplate.BTN_EDIT) as Button;
                    if (button != null)
                    {
                        button.Visible = false;
                    }
                    button = row.FindControl(GridViewTemplate.BTN_DELETE) as Button;
                    if (button != null)
                    {
                        button.Visible = false;
                    }

                }
                int lastIndex = FIELDS.Length - 1;
                if (mode.Equals("Standard"))
                {
                    for (int i = lastIndex; i > 0 && ADVANCED_FIELDS[i]; i--)
                    {
                        lastIndex--;
                    }
                }

                for (int i = 1; i < HEADERS.Length; i++)
                {
                    if ((mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) && HEADERS[i] != null)
                    {
                        string header = HEADERS[i];
                        if (wantTwoHeaders && info.MergedColumns.Contains(i))
                        {
                            if (info.StartColumns.ContainsKey(i))
                            {
                                header = info.Titles[i] + " (" + header + ")";
                            }
                            else
                            {
                                int j = i;
                                while (!info.StartColumns.ContainsKey(j) && j >= 0)
                                {
                                    j--;
                                }
                                header = info.Titles[j] + "(" + header + ")";

                            }
                        }
                        copiedTable += header;
                        TableStringHeaders += header;
                        if (i != lastIndex)
                        {
                            copiedTable += "\t"; // tab for next cell
                            TableStringHeaders += "\t";
                        }
                        else
                        {
                            copiedTable += "\r\n"; // end of table line
                            TableStringHeaders += "\r\n";
                        }
                    }
                }
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 1; i < FIELDS.Length; i++)
                    {
                        if ((mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) && HEADERS[i] != null)
                        {
                            copiedTable += row[FIELDS[i]].ToString();
                            TableString += row[FIELDS[i]].ToString();
                            if (i != lastIndex)
                            {
                                copiedTable += "\t"; // tab for next cell
                                TableString += "\t";
                            }
                            else
                            {
                                copiedTable += "\r\n"; // end of table line
                                TableString += "\r\n";
                            }
                        }

                    }
                }
                //txtClipboard.Attributes.Remove("value");
                //txtClipboard.Attributes.Add("value", copiedTable);
                txtCopyTable.Text = copiedTable;
                connec.Close();

                if (mode.Equals("Standard"))
                {
                    HideAdvancedColumns();
                }
            }
            catch (Exception ex)
            {
                logFiles.ErrorLog(ex);
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch { }
                if (!TablesLinked())
                {
                    Master.ShowErrorMessage("An error has occured. Current model '" + Master.GetCurrentModel() + "' is not loaded properly because some tables are missing. Please go to the models page and load the model again.");
                }
                else
                {
                    Master.ShowErrorMessage();
                }
            }
        }
    }

    protected virtual void PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid.PageIndex = e.NewPageIndex;
        this.SetData();
    }



    protected virtual void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //if (!PAGENAME.Equals("products_ibom.aspx") && !PAGENAME.Equals("whatif_products_ibom.aspx"))
                if (true)
            {
                Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT) as Button;
                string tooltip = "Double-click to edit ";
                if (((e.Row.RowState & DataControlRowState.Edit) > 0))
                {
                    btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                    //tooltip = "Double-click to update ";
                    /*e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Update$" + e.Row.RowIndex);
                    e.Row.Attributes["style"] = "cursor:normal;";
                    e.Row.ToolTip = "Double-click to update row";*/
                }
                if (PAGENAME.Contains("labor"))
                {
                    tooltip += "labor";
                }
                else if (PAGENAME.Contains("equipment"))
                {
                    tooltip += "equipment";
                }
                else if (PAGENAME.Contains("oper"))
                {
                    tooltip += "operation";
                }
                else if (PAGENAME.Contains("routing"))
                {
                    tooltip += "routing";
                }
                else if (PAGENAME.Contains("product"))
                {
                    tooltip += "product";
                }
                else
                {
                    tooltip += "row";
                }
                if (btnToClick != null)
                {
                    string btnJavascript = ClientScript.GetPostBackClientHyperlink(
                    btnToClick, "");


                    for (int columnIndex = 1; columnIndex <
                e.Row.Cells.Count; columnIndex++)
                    {
                        if (!((e.Row.RowState & DataControlRowState.Edit) > 0))
                        {
       // Add the column index as the event argument parameter
                            string js = btnJavascript.Insert(btnJavascript.Length - 2,
                            columnIndex.ToString());
                        // Add this javascript to the onclick Attribute of the cell
                        e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                        // Add a cursor style to the cells
                        e.Row.Cells[columnIndex].Attributes["style"] +=
                            "cursor:pointer;";
                        e.Row.Cells[columnIndex].ToolTip = tooltip;
                        }
                     

                        
                            try
                            {
                                Control control = e.Row.Cells[columnIndex].FindControl(TEXT_BOX_IDS[columnIndex]);
                                if (control is TextBox)
                                {
                                    (control as TextBox).Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                                }
                                else if (control is AjaxControlToolkit.ComboBox)
                                {
                                    TextBox textBox = (control as AjaxControlToolkit.ComboBox).FindControl(control.ID + "_TextBox") as TextBox;
                                    if (textBox != null)
                                    {
                                        textBox.Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                                    }

                                }
                            }
                            catch (Exception) { }
                        

                    }

                    //e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Edit$" + e.Row.RowIndex);
                    //e.Row.Attributes["style"] = "cursor:pointer";
                    //e.Row.ToolTip = "Double-click to edit row";
                }
            }
            //e.Row.Cells[1].ForeColor = ColorTranslator.FromHtml("black");
            //e.Row.Cells[1].Font.Bold = false;


            foreach (TableCell cell in e.Row.Cells)
            {
                cell.DataBind();


                try
                {

                    foreach (Control control in cell.Controls)
                    {
                        foreach (Control control2 in control.Controls)
                        {
                            if (control2 is Label)
                            {


                                Label lbl = control2 as Label;
                                lbl.CssClass = "padding";
                                double num = Double.Parse(lbl.Text);

                                cell.HorizontalAlign = HorizontalAlign.Center;
                                if (((e.Row.RowState & DataControlRowState.Edit) > 0))
                                {
                                    lbl.Enabled = false;
                                    lbl.Attributes["style"] += "cursor:default;";
                                }

                                    break;
                            }
                            else if (control2 is TextBox || control2 is AjaxControlToolkit.ComboBox || control2 is CheckBox)
                            {
                                cell.HorizontalAlign = HorizontalAlign.Center;
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }
            }
        }
    }

    protected string TableStringHeaders
    {
        get
        {
            if (ViewState["TableStringHeaders"] == null)
            {
                ViewState["TableStringHeaders"] = "";
            }
            return (string)ViewState["TableStringHeaders"];
        }
        set { ViewState["TableStringHeaders"] = value; }
    }

    protected string TableString
    {
        get
        {
            if (ViewState["TableString"] == null)
            {
                ViewState["TableString"] = "";
            }
            return (string)ViewState["TableString"];
        }
        set { ViewState["TableString"] = value; }
    }

    protected IDictionary<string, object> GetValues(GridViewRow row)
    {
        IOrderedDictionary dictionary = new OrderedDictionary();
        foreach (Control control in row.Controls)
        {
            DataControlFieldCell cell = control as DataControlFieldCell;

            if ((cell != null && cell.Visible))
            {
                cell.ContainingField.ExtractValuesFromCell(dictionary, cell, row.RowState, true);

            }
        }
        IDictionary<string, object> values = new Dictionary<string, object>();
        foreach (DictionaryEntry de in dictionary)
        {
            object value = de.Value;
            if (value == null)
            {
                value = "";
            }
            if (value is string)
            {
                string key = de.Key.ToString();
                if (key.Equals("LaborDesc") || key.Equals("ProdDesc") || key.Equals("EquipDesc") || key.Equals("OpNam"))
                {
                    value = (value as string).ToUpper();
                }
                value = (value as string).Replace("&nbsp;", " ").Replace("&NBSP;", " ");
                value = MyUtilities.clean(value as string);
            }
            values[de.Key.ToString()] = value;
        }
        return values;
    }

    protected string GetOrderByString()
    {
        string order;
        try
        {
            order = GetOrderBy(sortedTableName);
            if (order == null || order.Trim().Equals(""))
            {
                order = defaultSortString;
            }
        }
        catch (Exception)
        {
            order = defaultSortString;
        }
        return order;
    }

    protected virtual string GetCommandString(int commandType, string[] selectedFields)
    {
        // string of all the fields separated by ','
        string fieldEnum = "";
        string insertQuestionMarks = "";
        if (commandType == Command.SELECT)
        {
            fieldEnum += selectedFields[0] + ", ";
        }
        for (int i = 1; i < selectedFields.Length - 1; i++)
        {
            fieldEnum += "[" + selectedFields[i] + "] ";
            if (commandType == Command.UPDATE)
            {
                fieldEnum += " = ?";
            }
            fieldEnum += ", ";
            insertQuestionMarks += "?,";
        }
        fieldEnum += "[" + selectedFields[selectedFields.Length - 1] + "]";
        if (commandType == Command.UPDATE)
        {
            fieldEnum += " = ?";
        }
        insertQuestionMarks += "?";

        string command = "";
        switch (commandType)
        {
            case Command.SELECT:
                string order = GetOrderByString();
                if (tableQueryString != null)
                {
                    command = tableQueryString;
                }
                else
                {
                    command = "SELECT " + fieldEnum + " FROM " + "[" + TABLE_NAME + "] ";
                }
                command += order + ";";
                break;
            case Command.INSERT:
                command = "INSERT INTO [" + TABLE_NAME + "] (" + fieldEnum + ") VALUES (" + insertQuestionMarks + ");";
                break;
            case Command.UPDATE:
                command = "UPDATE [" + TABLE_NAME + "] SET " + fieldEnum + " WHERE [" + selectedFields[0] + "] = ?";
                break;
            case Command.DELETE:
                command = "DELETE FROM [" + TABLE_NAME + "] WHERE (" + selectedFields[0] + " = @" + selectedFields[0] + ")";
                break;
            default:
                break;
        }
        return command;
    }

    protected virtual string GetCommandString(int commandType)
    {
        if ((commandType == Command.UPDATE || commandType == Command.INSERT) && mode.Equals("Standard"))
        {
            List<string> selectedFields = new List<string>();
            for (int i = 0; i < FIELDS.Length; i++)
            {
                if (!ADVANCED_FIELDS[i])
                {
                    selectedFields.Add(FIELDS[i]);
                }
            }
            return GetCommandString(commandType, selectedFields.ToArray<string>());

        }
        return GetCommandString(commandType, FIELDS);
    }

    protected string[] GetIDs(int type)
    {
        string[] idArray = new string[FIELDS.Length];
        idArray[0] = null;
        for (int i = 1; i < idArray.Length; i++)
        {
            switch (type)
            {
                case IDs.TEXT_BOX:
                    if (COMBOS[i])
                    {
                        idArray[i] = "combo";
                    }
                    else if (CHECKBOXES[i])
                    {
                        idArray[i] = "check";
                    }
                    else
                    {
                        idArray[i] = "txt";
                    }
                    break;
                case IDs.LABEL:
                    idArray[i] = "lbl";
                    break;
            }
            idArray[i] += BASE_CONTROL_NAME + i;
        }
        return idArray;
    }

    protected virtual void RowUpdate(int rowIndex)
    {
        GridViewRow row = grid.Rows[rowIndex];

        var newValues = this.GetValues(row);
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++)
            {
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i])
                {
                    if (CHECKBOXES[i])
                    {
                        object bla = newValues[FIELDS[i]];
                        cmd.Parameters.AddWithValue(FIELDS[i], ((bool)newValues[FIELDS[i]]) ? "1" : "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(newValues[FIELDS[i]].ToString()));
                    }
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try
            {
                connec.Open();
                int result = cmd.ExecuteNonQuery();

                grid.EditIndex = -1;

                this.SetData();
                connec.Close();
                if (mode.Equals("Standard"))
                {
                    HideAdvancedColumns();
                }
            }
            catch (Exception ex)
            {
                logFiles.ErrorLog(ex);
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch { }
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    protected virtual void SyncTables()
    {
        if (tableSync != null)
        {
            tableSync.SyncTables();
        }
    }

    protected virtual void InitializeComponent()
    {
        TEXT_BOX_IDS = GetIDs(IDs.TEXT_BOX);
        LABEL_IDS = GetIDs(IDs.LABEL);


        secondPanel = GetSecondPanel();

        grid = GenerateGridControl();
        pnlMainGrid.Controls.Add(grid);

        // advanced mode stuff
        Control buttondiv = getButtonDiv();
        if (hasAdvanced)
        {
            Label lblAdvanced = new Label();

            lblAdvanced.ID = "lblAdvancedMode";
            lblAdvanced.CssClass = "icon-menu";
            lblAdvanced.AssociatedControlID = "btnAdvancedMode";

            lblAdvanced.ToolTip = "expand additional columns";
            lblAdvanced.Text = "<i class='fas fa-eye-slash fa-fw-slash row-icon'></i><span>SHOW/HIDE</span>";
            
            buttondiv.Controls.Add(lblAdvanced);

            btnAdvanced = new Button();
            btnAdvanced.ID = "btnAdvancedMode";
            btnAdvanced.Text = "Expand";
            btnAdvanced.CssClass = "menu-button";
            btnAdvanced.Click += btnAdvanced_Click;
            btnAdvanced.UseSubmitBehavior = false;
            buttondiv.Controls.Add(btnAdvanced);
            
        }

        //THIS FEATURE CAN BE ACCOMPLISHED BY HIGHLIGHTING DATA AND PASTING INTO EXCEL EXCEPT IN SOME OUTPUT CASES
        Label lblCopyTable = new Label();
        lblCopyTable.Text = "<i class='fas fa-copy fa-fw row-icon'></i>";
        lblCopyTable.CssClass = "hidden icon-menu";
        lblCopyTable.AssociatedControlID = PageControls.BTN_COPY_TABLE;
        buttondiv.Controls.Add(lblCopyTable);

        Button btnCopyTable = new Button();  //  size ? 
        btnCopyTable.ID = PageControls.BTN_COPY_TABLE;
        btnCopyTable.Text = "Copy";
        btnCopyTable.CssClass = "menu-button";
        buttondiv.Controls.Add(btnCopyTable);


        secondPanel.CssClass = "gridPanel";

        thirdPanel = GetThirdPanel();
        // copy table pop up stuff
        Panel fifthPanel = GetFifthPanel();
        pnlCopyTable = PageControls.generateCopyPanel();

        fifthPanel.Controls.Add(pnlCopyTable);
        rdbtnTableWithHeaders = pnlCopyTable.FindControl(PageControls.RDBTN_WITH_HEADER) as RadioButton;
        rdbtnTableWithoutHeaders = pnlCopyTable.FindControl(PageControls.RDBTN_WITHOUT_HEADER) as RadioButton;
        rdbtnTableWithHeaders.CheckedChanged += new EventHandler(rdbtnTable_CheckedChanged);
        rdbtnTableWithoutHeaders.CheckedChanged += new EventHandler(rdbtnTable_CheckedChanged);
        boxCheckAll = pnlCopyTable.FindControl(PageControls.CHECK_SELECT_ALL) as CheckBox;
        txtCopyTable = pnlCopyTable.FindControl(PageControls.INPUT_COPY_TABLE) as TextBox;
        boxCheckAll.Attributes.Add("onclick", "selectTable('" + txtCopyTable.ClientID + "', '" + boxCheckAll.ClientID + "')");

        Button btnCopyDone = pnlCopyTable.FindControl(PageControls.BTN_DONE) as Button;
        btnCopyDone.Click += new EventHandler(btnCopyDone_Click);
        Master.ClickOnEnter(btnCopyDone.ClientID, txtCopyTable);
        extenderCopy = PageControls.generateCopyExtender();
        fifthPanel.Controls.Add(extenderCopy);
    }

    protected void btnCopyDone_Click(object sender, EventArgs e)
    {
        boxCheckAll.Checked = false;
    }

    protected virtual void rdbtnTable_CheckedChanged(object sender, EventArgs e)
    {
        if (rdbtnTableWithHeaders.Checked)
        {
            txtCopyTable.Text = TableStringHeaders + TableString;
        }
        else
        {
            txtCopyTable.Text = TableString;
        }
        boxCheckAll.Checked = false;
        extenderCopy.Show();
    }

    protected override void OnInit(EventArgs e)
    {
        SetFeaturesFromHelper();
        base.OnInit(e);
        InitializeComponent();
        if (wantTwoHeaders)
        {
            SetupSecondHeaders();
        }


    }

    protected void InitializeCombos()
    {
        COMBOS = new bool[FIELDS.Length];
        for (int i = 0; i < FIELDS.Length; i++)
        {
            COMBOS[i] = false;
        }
    }

    protected void InitializeCheckboxes()
    {
        CHECKBOXES = new bool[FIELDS.Length];
        for (int i = 0; i < FIELDS.Length; i++)
        {
            CHECKBOXES[i] = false;
        }
    }

    protected abstract Control getButtonDiv();

    protected abstract Panel GetSecondPanel();

    protected abstract Panel GetFourthPanel();

    protected abstract Panel GetFifthPanel();

    private void SetFeaturesFromHelper()
    {
        TABLE_NAME = featureHelper.TABLE_NAME;
        tableQueryString = featureHelper.tableQueryString;
        defaultSortString = featureHelper.defaultSortString;
        SORT_COMMAND = featureHelper.SORT_COMMAND;

        FIELDS = featureHelper.FIELDS;
        ADVANCED_FIELDS = featureHelper.ADVANCED_FIELDS;
        HEADERS = featureHelper.HEADERS;
        HEADER_TOOLTIPS = featureHelper.HEADER_TOOLTIPS;

        if (!isWhatif)
        {
            nonEdits = featureHelper.nonEdits;
            fieldsNonEditable = featureHelper.fieldsNonEditable;
        }
        if (sortedTableName == null)
        {
            sortedTableName = featureHelper.sortedTableName;
        }
        if (featureHelper.wantSort2)
        {
            wantSort2 = true;
            sortedTableName2 = featureHelper.sortedTableName2;
        }

        hasAdvanced = featureHelper.hasAdvanced;
        wantSort = featureHelper.wantSort;
        wantTwoHeaders = featureHelper.wantTwoHeaders;
        info = featureHelper.info;

        COMBOS = featureHelper.COMBOS;
        CHECKBOXES = featureHelper.CHECKBOXES;
    }
}