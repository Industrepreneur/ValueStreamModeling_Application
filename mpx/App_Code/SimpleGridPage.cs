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
/// Summary description for GridPage
/// </summary>
/// 

public abstract class SimpleGridPage : SortablePage {

    protected string TABLE_NAME {
        get {
            if (ViewState["TABLE_NAME"] == null) {
                ViewState["TABLE_NAME"] = "";
            }
            return (string)ViewState["TABLE_NAME"];
        }
        set { ViewState["TABLE_NAME"] = value; }
    }

    //property for storing of information about merged columns
    protected MergedColumnsInfo info {
        get {
            if (ViewState["info"] == null)
                ViewState["info"] = new MergedColumnsInfo();
            return (MergedColumnsInfo)ViewState["info"];
        }
    }

    protected bool wantTwoHeaders = false;

    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e) {
        //call the method for custom rendering the columns headers 
        if (e.Row.RowType == DataControlRowType.Header)
            e.Row.SetRenderMethodDelegate(RenderHeader);
    }

    private bool ShouldMergeColsInStandardMode() {
        bool shouldMerge = false;
        for (int i = 1; i < info.MergedColumns.Count; i++) {
            int colNext = info.MergedColumns[i];
            int colPrev = info.MergedColumns[i - 1];
            if (colNext - colPrev == 1 && !ADVANCED_FIELDS[colNext] && !ADVANCED_FIELDS[colPrev]
                    && !info.StartColumns.Contains(colNext)) {
                shouldMerge = true;
                break;
            }
        }
        return shouldMerge;
    }

    //method for rendering the columns headers 
    protected void RenderHeader(HtmlTextWriter output, Control container) {

        bool shouldMerge = true;
        int currentMergedCol = 0;
        if (mode.Equals("Standard")) {
            shouldMerge = ShouldMergeColsInStandardMode();
        }
        List<int> mergedCols = new List<int>();
        for (int i=0; i< info.MergedColumns.Count; i++) {
            mergedCols.Add(info.MergedColumns[i]);
        }
        
        for (int i = 0; i < container.Controls.Count; i++) {
            TableCell cell = (TableCell)container.Controls[i];
            if (i == 6) {
                string bla = "bla";
            }
            
            //stretch non merged columns for two rows
            if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                if (!info.MergedColumns.Contains(i)) {
                    if (shouldMerge) {
                        cell.Attributes["rowspan"] = "2";
                    }
                    cell.RenderControl(output);
                } else {
                    int k = i;
                    while (!info.StartColumns.Contains(k)) {
                        k--;
                    }
                    if (!shouldMerge) {
                        output.Write(string.Format("<th>{0}</th>",
                                    info.Titles[k] + " (" + HEADERS[i] + ")"));
                    } else {
                        int length = int.Parse(info.StartColumns[k].ToString());
                        int hiddenCols = 0;
                        if (mode.Equals("Standard")) {
                            for (int j = k; j < k + length; j++) {
                                if (ADVANCED_FIELDS[j]) {
                                    hiddenCols++;
                                }
                            }
                        }
                        int numMergedCols = length - hiddenCols;
                        if (numMergedCols > 1 && currentMergedCol < k) {
                            output.Write(string.Format("<th align='center' colspan='{0}'>{1}</th>",
                                        length - hiddenCols, info.Titles[k]));
                        } else if (numMergedCols == 1) {
                            mergedCols.Remove(i);
                            output.Write(string.Format("<th rowspan='2'>{0}</th>",
                                    info.Titles[k] + " (" + HEADERS[i] + ")"));
                            
                        }
                    }
                    if (k>currentMergedCol) {
                        currentMergedCol = k;
                        
                    }
                    
                }
            }
            
        }

        if (shouldMerge) {
            //close the first row 
            output.Write("</tr>");
            //set attributes for the second row

            grid.HeaderStyle.AddAttributesToRender(output);
            //start the second row
            output.RenderBeginTag("tr");

            //render the second row (only the merged columns)
            for (int i = 0; i < mergedCols.Count; i++) {
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[mergedCols[i]]) {
                    TableCell cell = (TableCell)container.Controls[mergedCols[i]];
                    cell.RenderControl(output);
                }

            }
        }

    }
    // values from insert footer row
    protected object[] savedInsertValues;

    protected string tableQueryString;

    protected Panel pnlMainGrid;

    protected DataTable dt;

    protected GridView grid;
    protected Label lblRow;

    protected Table popupTable;

    public bool nonEdits = false;

    protected bool[] fieldsNonEditable;

    protected bool hasAdvanced = true;

    protected bool isWhatif = false;
    protected bool wantCopyButton = true;

    protected OleDbConnection connec = null;

    protected string GRID_NAME = "grid";
    protected string GRID_CONTAINER = "gridPanel";
    protected string[] FIELDS;
    protected bool[] ADVANCED_FIELDS;

    protected bool[] COMBOS;

    protected bool[] CHECKBOXES;

    protected string[] TEXT_BOX_IDS;
    protected string[] LABEL_IDS;
    protected string[] HEADERS;

    protected string[] TEXT_BOX_IDS2;
    protected string[] LABEL_IDS2;

    public const string BASE_CONTROL_NAME = "Edit";
    public const string LBL_ROW = "lblRow";
    public const string BUTTON_DIV = "buttondiv";

    protected Panel secondPanel;
    protected Panel pnlPopupLine;
    protected Panel thirdPanel;

    protected RadioButton rdbtnCopy;
    protected RadioButton rdbtnEdit;
    protected RadioButton rdbtnDelete;
    protected RadioButton rdbtnShowLine;

    protected RadioButton rdbtnCopyLine;
    protected RadioButton rdbtnEditLine;
    protected RadioButton rdbtnDeleteLine;
    protected RadioButton rdbtnNewLine;
    protected Button btnAddLine;
    protected Button btnOkLine;
    protected Label lblAction;

    protected TextBox txtInsertTable;
    protected Button btnInsertTable;

    protected AjaxControlToolkit.ModalPopupExtender extenderLinePopup;

    protected AjaxControlToolkit.ModalPopupExtender extenderNewName;
    protected AjaxControlToolkit.ModalPopupExtender extenderInfo;
    protected Panel pnlNewName;

    protected Button btnCancel;

    protected TextBox txtCopyTable;

    protected RadioButton rdbtnTableWithHeaders;
    protected RadioButton rdbtnTableWithoutHeaders;
    protected CheckBox boxCheckAll;
    protected AjaxControlToolkit.ModalPopupExtender extenderCopy;

    protected Panel pnlCopyTable;

    protected string RDTNS_CSS_CLASS = "rdtnsInline";
    protected Panel rdbtnsChoiceModePanel;

    protected Button btnAdvanced;

    public const string TXT_INSERT_TABLE = "txtInsertTable";

    protected Button btnInsertMultipleWarningOk;


    protected static class Command {
        public const int SELECT = 0;
        public const int INSERT = 1;
        public const int UPDATE = 2;
        public const int DELETE = 3;
    }

    protected static class IDs {
        public const int TEXT_BOX = 0;
        public const int LABEL = 1;
    }

    protected string mode {
        get {
            if (ViewState["mode"] == null) {
                ViewState["mode"] = "Standard";
            }
            return (string)ViewState["mode"];
        }
        set { ViewState["mode"] = value; }

    }

    void Page_PreInit(Object sender, EventArgs e) {
        this.MasterPageFile = "~/MasterPage.master";
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
        grid = pnlMainGrid.FindControl(GRID_NAME) as GridView;
        if (!Page.IsPostBack) {
            this.SetData();

        }


    }

    protected virtual GridView GenerateGridControl() {
        GridView newGrid = new GridView();
        newGrid.ID = GRID_NAME;
        newGrid.DataKeyNames = new string[] { FIELDS[0] };
        newGrid.AllowPaging = true;
        newGrid.AllowSorting = false;
        newGrid.ShowFooter = true;
        newGrid.AutoGenerateColumns = false;
        newGrid.HeaderStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        newGrid.FooterStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
        newGrid.RowStyle.CssClass = "datatable-rowstyle";
        newGrid.AlternatingRowStyle.BackColor = Color.White;
        newGrid.ShowHeaderWhenEmpty = true;
        newGrid.BorderWidth = 3;
        newGrid.BorderStyle = BorderStyle.Solid;
        newGrid.BorderColor = Color.Black;
        //newGrid.Sorting += new GridViewSortEventHandler(this.OnSorting);
        newGrid.PageIndexChanging += new GridViewPageEventHandler(this.PageIndexChanging);
        newGrid.RowDataBound += new GridViewRowEventHandler(this.Grid_RowDataBound);
        //newGrid.DataBound += Grid_DataBound; // for nothing 
        newGrid.RowCommand += new GridViewCommandEventHandler(this.grid_RowCommand);
        newGrid.RowUpdating += new GridViewUpdateEventHandler(this.grid_RowUpdating);
        newGrid.RowDeleting += new GridViewDeleteEventHandler(this.grid_RowDeleting);
        newGrid.RowEditing += new GridViewEditEventHandler(this.grid_RowEditing);

        if (wantTwoHeaders) {
            newGrid.RowCreated += GridView_RowCreated;
        }

        newGrid.EmptyDataText = "There are no data records to display.";
        newGrid.EmptyDataTemplate = new GridViewTemplate(GridViewTemplate.EMPTY_TABLE);
        TemplateField templ = new TemplateField();
        GridViewTemplate btnTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.BUTTONS, FIELDS[0], BASE_CONTROL_NAME + 0);
        btnTemplate.whatif = isWhatif;
        btnTemplate.wantCopyButton = wantCopyButton;
        templ.ItemTemplate = btnTemplate;
        templ.EditItemTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.BUTTONS, FIELDS[0], null);

        templ.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.BUTTONS, FIELDS[0], null);
        newGrid.Columns.Add(templ);
        for (int i = 1; i < FIELDS.Length; i++) {
            TemplateField template = new TemplateField();
            GridViewTemplate itemTemplate;
            if (CHECKBOXES[i]) {
                itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], LABEL_IDS[i]);
            } else {
                itemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS[i], LABEL_IDS[i]);
            }
            template.ItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
                itemTemplate.InstantiateIn(container);
            },
                delegate(Control container) {

                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS.Length; j++) {
                        if (CHECKBOXES[j]) {
                            dict[FIELDS[j]] = ((CheckBox)container.FindControl(LABEL_IDS[j])).Checked;
                        } else {
                            dict[FIELDS[j]] = ((Label)container.FindControl(LABEL_IDS[j])).Text;
                        }
                    }
                    return dict;
                });

            GridViewTemplate editTemplate;


            if (COMBOS[i] && (!nonEdits || !fieldsNonEditable[i])) {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList(FIELDS[i]));
            } else if (CHECKBOXES[i]) {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            } else {
                if (nonEdits && fieldsNonEditable[i]) {
                    editTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                } else {
                    editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
                }

            }

            template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
                editTemplate.InstantiateIn(container);
            },
                delegate(Control container) {

                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS.Length; j++) {
                        if (nonEdits && fieldsNonEditable[j]) {
                            dict[FIELDS[j]] = ((Label)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        } else if (COMBOS[j]) {
                            dict[FIELDS[j]] = ((AjaxControlToolkit.ComboBox)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        } else if (CHECKBOXES[j]) {
                            dict[FIELDS[j]] = ((CheckBox)container.FindControl(TEXT_BOX_IDS[j])).Checked;
                        } else {

                            dict[FIELDS[j]] = ((TextBox)container.FindControl(TEXT_BOX_IDS[j])).Text;
                        }
                    }
                    return dict;
                });
            if (COMBOS[i]) {

                template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList(FIELDS[i]));
            } else if (CHECKBOXES[i]) {
                template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            } else {
                template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            }
            template.HeaderText = HEADERS[i];
            template.SortExpression = FIELDS[i];
            newGrid.Columns.Add(template);
        }
        return newGrid;
    }

    protected virtual void Grid_DataBound(object sender, EventArgs e) {
        if (!Page.IsPostBack) {
        //    FillDefaultInsertRow();
        }
    }

    protected void grid_RowEditing(object sender, GridViewEditEventArgs e) {

    }


    protected List<string> GetDropList(string name, string table) {
        List<string> dropList = new List<string>();
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string comm = "SELECT " + name + " FROM " + table + ";";
        OleDbCommand cmd = new OleDbCommand(comm, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++) {
                    dropList.Add(dt.Rows[i][0].ToString());
                }

                connec.Close();

            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        return dropList;
    }

    protected virtual List<string> GetDropList() {
        return new List<string>();
    }

    protected virtual List<string> GetDropList(string name) {
        return GetDropList();
    }

    protected virtual void SetData() {
        grid = pnlMainGrid.FindControl(GRID_NAME) as GridView;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string comm = GetCommandString(Command.SELECT);
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.SELECT), connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                bool empty = false;
                if (dt.Rows.Count == 0) {
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
                if (empty) {
                    // hide the buttons
                    GridViewRow row = grid.Rows[0];
                    Button button = row.FindControl(GridViewTemplate.BTN_COPY) as Button;
                    if (button != null) {
                        button.Visible = false;
                    }
                    button = row.FindControl(GridViewTemplate.BTN_EDIT) as Button;
                    if (button != null) {
                        button.Visible = false;
                    }
                    button = row.FindControl(GridViewTemplate.BTN_DELETE) as Button;
                    if (button != null) {
                        button.Visible = false;
                    }

                }
                int lastIndex = FIELDS.Length - 1;
                if (mode.Equals("Standard")) {
                    for (int i = lastIndex; i > 0 && ADVANCED_FIELDS[i]; i--) {
                        lastIndex--;
                    }
                }

                for (int i = 1; i < HEADERS.Length; i++) {
                    if ((mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) && HEADERS[i] != null) {
                        string header = HEADERS[i];
                        if (wantTwoHeaders && info.MergedColumns.Contains(i)) {
                            if (info.StartColumns.ContainsKey(i)) {
                                header = info.Titles[i] + " (" + header + ")";
                            } else {
                                int j = i;
                                while (!info.StartColumns.ContainsKey(j) && j >= 0) {
                                    j--;
                                }
                                header = info.Titles[j] + "(" + header + ")";

                            }
                        }
                        copiedTable += header;
                        TableStringHeaders += header;
                        if (i != lastIndex) {
                            copiedTable += "\t"; // tab for next cell
                            TableStringHeaders += "\t";
                        } else {
                            copiedTable += "\r\n"; // end of table line
                            TableStringHeaders += "\r\n";
                        }
                    }
                }
                foreach (DataRow row in dt.Rows) {
                    for (int i = 1; i < FIELDS.Length; i++) {
                        if ((mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) && HEADERS[i] != null) {
                            copiedTable += row[FIELDS[i]].ToString();
                            TableString += row[FIELDS[i]].ToString();
                            if (i != lastIndex) {
                                copiedTable += "\t"; // tab for next cell
                                TableString += "\t";
                            } else {
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
                //if (!Page.IsPostBack) {
                FillDefaultInsertRow();
                //}
                if (mode.Equals("Standard")) {
                    HideAdvancedColumns();
                }
            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage();
            }
        }
    }

    protected virtual void PageIndexChanging(object sender, GridViewPageEventArgs e) {
        grid = pnlMainGrid.FindControl(GRID_NAME) as GridView;
        grid.PageIndex = e.NewPageIndex;
        this.SetData();
    }

    

    protected virtual void Grid_RowDataBound(object sender, GridViewRowEventArgs e) {
        /*if (e.Row.RowType == DataControlRowType.Header) {
            foreach (TableCell cell in e.Row.Cells) {
                foreach (Control ctl in cell.Controls) {
                    if (ctl.GetType().ToString().Contains("DataControlLinkButton")) {
                        cell.Attributes.Add("title", "Sort");
                    }
                }
            }
        } else if (e.Row.RowType == DataControlRowType.DataRow) {
            e.Row.Cells[1].ForeColor = ColorTranslator.FromHtml("#CC5201");
            e.Row.Cells[1].Font.Bold = true;
        }*/
        if (e.Row.RowType == DataControlRowType.DataRow) {
            if (!PAGENAME.Equals("products_ibom.aspx") && !PAGENAME.Equals("whatif_products_ibom.aspx")) {
                Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT) as Button;
                string tooltip = "Double-click to edit row";
                if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                    btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                    tooltip = "Double-click to update row";
                    /*e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Update$" + e.Row.RowIndex);
                    e.Row.Attributes["style"] = "cursor:normal;";
                    e.Row.ToolTip = "Double-click to update row";*/
                }
                if (btnToClick != null) {
                    string btnJavascript = ClientScript.GetPostBackClientHyperlink(
                    btnToClick, "");
                    for (int columnIndex = 1; columnIndex <
                    e.Row.Cells.Count; columnIndex++) {
                        // Add the column index as the event argument parameter
                        string js = btnJavascript.Insert(btnJavascript.Length - 2,
                            columnIndex.ToString());
                        // Add this javascript to the onclick Attribute of the cell
                        e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                        // Add a cursor style to the cells
                        e.Row.Cells[columnIndex].Attributes["style"] +=
                            "cursor:pointer;cursor:hand;";
                        e.Row.Cells[columnIndex].ToolTip = tooltip;
                        if (((e.Row.RowState & DataControlRowState.Edit) > 0)) {
                            try {
                                Control control = e.Row.Cells[columnIndex].FindControl(TEXT_BOX_IDS[columnIndex]);
                                if (control is TextBox) {
                                    (control as TextBox).Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                                } else if (control is AjaxControlToolkit.ComboBox) {
                                    (control as AjaxControlToolkit.ComboBox).Attributes.Add("onkeydown", "doFocus('" + btnToClick.ClientID + "', event);");
                                }
                            } catch (Exception) { }
                        }

                    }

                    //e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grid, "Edit$" + e.Row.RowIndex);
                    //e.Row.Attributes["style"] = "cursor:pointer";
                    //e.Row.ToolTip = "Double-click to edit row";
                }
            }
            e.Row.Cells[1].ForeColor = ColorTranslator.FromHtml("black");
            e.Row.Cells[1].Font.Bold = false;
            foreach (TableCell cell in e.Row.Cells) {
                cell.DataBind();
                try {

                    foreach (Control control in cell.Controls) {
                        foreach (Control control2 in control.Controls) {
                            if (control2 is Label) {
                                Label lbl = control2 as Label;
                                lbl.CssClass = "padding";
                                double num = Double.Parse(lbl.Text.Trim());
                                cell.HorizontalAlign = HorizontalAlign.Center;
                                break;
                            } else if (control2 is TextBox || control2 is AjaxControlToolkit.ComboBox || control2 is CheckBox) {
                                cell.HorizontalAlign = HorizontalAlign.Center;
                                break;
                            }
                        }
                    }

                } catch (Exception) {

                }
            }
        }
    }

    protected string TableStringHeaders {
        get {
            if (ViewState["TableStringHeaders"] == null) {
                ViewState["TableStringHeaders"] = "";
            }
            return (string)ViewState["TableStringHeaders"];
        }
        set { ViewState["TableStringHeaders"] = value; }
    }

    protected string TableString {
        get {
            if (ViewState["TableString"] == null) {
                ViewState["TableString"] = "";
            }
            return (string)ViewState["TableString"];
        }
        set { ViewState["TableString"] = value; }
    }

    protected IDictionary<string, object> GetValues(GridViewRow row) {
        IOrderedDictionary dictionary = new OrderedDictionary();
        foreach (Control control in row.Controls) {
            DataControlFieldCell cell = control as DataControlFieldCell;

            if ((cell != null && cell.Visible)) {
                cell.ContainingField.ExtractValuesFromCell(dictionary, cell, row.RowState, true);

            }
        }
        IDictionary<string, object> values = new Dictionary<string, object>();
        foreach (DictionaryEntry de in dictionary) {
            object value = de.Value;
            if (value == null) {
                value = "";
            }
            if (value is string) {
                string key = de.Key.ToString();
                if (key.Equals("LaborDesc") || key.Equals("ProdDesc") || key.Equals("EquipDesc") || key.Equals("OpNam") ||
                    key.Equals("opnam1") || key.Equals("opnam2")) {
                    value = (value as string).ToUpper();
                }
                value = (value as string).Replace("&nbsp;", " ").Replace("&NBSP;", " ");
            }
            values[de.Key.ToString()] = value;
            
        }
        return values;
    }


    protected string GetOrderByString() {
        string order;
        try {
            order = GetOrderBy(sortedTableName);
            if (order == null || order.Trim().Equals("")) {
                order = defaultSortString;
            }
        } catch (Exception) {
            order = defaultSortString;
        }
        return order;
    }

    protected virtual string GetCommandString(int commandType, string[] selectedFields) {
        // string of all the fields separated by ','
        string fieldEnum = "";
        string insertQuestionMarks = "";
        if (commandType == Command.SELECT) {
            fieldEnum += selectedFields[0] + ", ";
        }
        for (int i = 1; i < selectedFields.Length - 1; i++) {
            fieldEnum += "[" + selectedFields[i] + "] ";
            if (commandType == Command.UPDATE) {
                fieldEnum += " = ?";
            }
            fieldEnum += ", ";
            insertQuestionMarks += "?,";
        }
        fieldEnum += "[" + selectedFields[selectedFields.Length - 1] + "]";
        if (commandType == Command.UPDATE) {
            fieldEnum += " = ?";
        }
        insertQuestionMarks += "?";

        string command = "";
        switch (commandType) {
            case Command.SELECT:
                string order = GetOrderByString();
                if (tableQueryString != null) {
                    command = tableQueryString;
                } else {
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

    protected virtual string GetCommandString(int commandType) {
        if ((commandType == Command.UPDATE || commandType == Command.INSERT) && mode.Equals("Standard")) {
            List<string> selectedFields = new List<string>();
            for (int i = 0; i < FIELDS.Length; i++) {
                if (!ADVANCED_FIELDS[i]) {
                    selectedFields.Add(FIELDS[i]);
                }
            }
            return GetCommandString(commandType, selectedFields.ToArray<string>());

        }
        return GetCommandString(commandType, FIELDS);
    }

    protected string[] GetIDs(int type) {
        string[] idArray = new string[FIELDS.Length];
        idArray[0] = null;
        for (int i = 1; i < idArray.Length; i++) {
            switch (type) {
                case IDs.TEXT_BOX:
                    if (COMBOS[i]) {
                        idArray[i] = "combo";
                    } else if (CHECKBOXES[i]) {
                        idArray[i] = "check";
                    } else {
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

    protected string[] GetIDs2(int type) {
        string[] idArray = new string[FIELDS.Length];
        idArray[0] = null;
        for (int i = 1; i < idArray.Length; i++) {
            switch (type) {
                case IDs.TEXT_BOX:
                    if (COMBOS[i]) {
                        idArray[i] = "combo2";
                    } else if (CHECKBOXES[i]) {
                        idArray[i] = "check2";
                    } else {
                        idArray[i] = "txt2";
                    }
                    break;
                case IDs.LABEL:
                    idArray[i] = "lbl2";
                    break;
            }
            idArray[i] += BASE_CONTROL_NAME + i;
        }
        return idArray;
    }

    protected virtual void Copy(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);

        GridViewRow insertRow = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = insertRow.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
        }
        int j = 1;
        string origName = newValues[FIELDS[j]].ToString();
        string copyName = origName + "_COPY";
        if (FIELDS[j].ToLower().Equals("labordesc")) {
            copyName = GetUniqueName("LaborDesc", "LaborID", "tbllabor", copyName);
        } else if (FIELDS[j].ToLower().Equals("equipdesc")) {
            copyName = GetUniqueName("EquipDesc", "EquipID", "tblequip", copyName);
        } else if (FIELDS[j].ToLower().Equals("proddesc")) {
            copyName = GetUniqueName("ProdDesc", "ProdID", "tblprodfore", copyName);
        } else if (FIELDS[j].ToLower().Equals("equipdesc")) {
            copyName = GetUniqueName("Opnam", "OpID", "tbloper", copyName);
        }
        
        if (COMBOS[j]) {
            ((AjaxControlToolkit.ComboBox)txtControls[j]).Text = copyName;
        } else {
            ((TextBox)txtControls[j]).Text = copyName;
        }

        for (int i = 2; i < FIELDS.Length; i++) {

            if (COMBOS[i]) {
                ((AjaxControlToolkit.ComboBox)txtControls[i]).Text = newValues[FIELDS[i]].ToString();
            } else if (CHECKBOXES[i]) {
                ((CheckBox)txtControls[i]).Checked = (bool)newValues[FIELDS[i]];
            } else {
                string value = newValues[FIELDS[i]].ToString();
                ((TextBox)txtControls[i]).Text = newValues[FIELDS[i]].ToString();
            }
        }
        txtControls[1].Focus();
        /*TextBox[] txtInserts = new TextBox[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtInserts[i] = insertRow.FindControl(TEXT_BOX_IDS[i]) as TextBox;
            if (txtInserts[i] == null) { return; }
        }
        for (int i = 2; i < FIELDS.Length; i++) {
            txtInserts[i].Text = newValues[FIELDS[i]].ToString();
        }
        txtInserts[1].Focus();*/
    }

    protected void Copy() {
        grid = pnlMainGrid.FindControl(GRID_NAME) as GridView;
        int rowIndex = GetCurrentRowIndex();
        Copy(rowIndex);
    }

    protected void grid_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }

    private int GetCurrentRowIndex() {
        int rowIndex = -1;
        int id = int.Parse(lblRow.Text);
        foreach (GridViewRow row in grid.Rows) {
            rowIndex++;
            int key = int.Parse(grid.DataKeys[rowIndex][FIELDS[0]].ToString());
            if (id == key) {
                break;
            }
        }
        return rowIndex;
    }

    private void Edit() {

        int rowIndex = GetCurrentRowIndex();
        grid.EditIndex = rowIndex;
        this.SetData();

    }

    protected void btnOk_Click(object sender, EventArgs e) {
        lblRow = getLblRow();

        lblRow.Text = String.Format("{0}", Request.Form["selectedRowId"]);
        //lblRow.Text = String.Format("{0}", Request.QueryString["selectedRowId"]);

        if (rdbtnCopy.Checked) {
            Copy();
            rdbtnCopy.Checked = false;
        } else if (rdbtnDelete.Checked) {
            DeleteRow();
            rdbtnDelete.Checked = false;
        } else if (rdbtnEdit.Checked) {
            Edit();
            rdbtnEdit.Checked = false;
        } else if (rdbtnShowLine.Checked) {
            FillPopupLine();
            ToChoiceMode();
            rdbtnShowLine.Checked = false;
        }
    }

    protected void grid_RowUpdating(object sender, EventArgs e) {

    }

    protected virtual void RowUpdate(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];

        var newValues = this.GetValues(row);
        string oldField = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++) {
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (CHECKBOXES[i]) {
                        object bla = newValues[FIELDS[i]];
                        cmd.Parameters.AddWithValue(FIELDS[i], ((bool)newValues[FIELDS[i]]) ? "1" : "0");
                    } else {
                        cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(newValues[FIELDS[i]].ToString()));
                    }
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();

                grid.EditIndex = -1;

                this.SetData();
                connec.Close();
                if (mode.Equals("Standard")) {
                    HideAdvancedColumns();
                }
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    protected virtual void SyncTables() {
        if (tableSync != null) {
            tableSync.SyncTables();
        }
    }

    protected void RowUpdate() {

        int rowIndex = GetCurrentRowIndex();
        RowUpdate(rowIndex);
        SyncTables();
    }



    protected void RowUpdateCancel() {
        grid.EditIndex = -1;

        this.SetData();
        if (mode.Equals("Standard")) {
            HideAdvancedColumns();
        }
    }

    protected void DeleteRow() {
        int rowIndex = GetCurrentRowIndex();
        DeleteRow(rowIndex);
    }

    protected void DeleteRow(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.DELETE), connec);

        cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);

        int key = int.Parse(grid.DataKeys[row.RowIndex][FIELDS[0]].ToString());

        try {
            connec.Open();
            int result = cmd.ExecuteNonQuery();
            {
                ClassF classA = new ClassF(GetDirectory() + userDir);
                classA.setGlobalVar();
                if (TABLE_NAME.Equals("tbllabor")) {
                    classA.del_labor_res(key);
                    classA.del_lab_ref(key);
                } else if (TABLE_NAME.Equals("tblequip")) {
                    classA.del_eq_res(key);
                    classA.del_eq_ref(key);
                } else if (TABLE_NAME.Equals("tblprodfore")) {
                    classA.del_pt_res(key);
                } else if (TABLE_NAME.Equals("tbloper")) {
                    classA.del_op_res(key);
                }
                classA.Close();
            }
            grid.EditIndex = -1;
            this.SetData();
            connec.Close();

        } catch {
            try {
                connec.Close();
                connec = null;
            } catch { }
            Master.ShowErrorMessage("An error has occured and the record could not get deleted.");
        }
    }

    protected virtual void grid_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }
        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        if (e.CommandName.Equals("Insert")) {
            InsertRowSetData();
            SyncTables();
            SetModelModified(true, true);
        } else if (e.CommandName.Equals("Update")) {
            RowUpdate(rowIndex);
            SyncTables();
            SetModelModified(true, false);
        } else if (e.CommandName.Equals("CancelUpdate")) {
            RowUpdateCancel();
        } else if (e.CommandName.Equals("insertInEmpty")) {
            insertToEmptyTable();
        } else if (e.CommandName.Equals("Edit")) {
            grid.EditIndex = rowIndex;
            this.SetData();
            try {
                //int _rowIndex = int.Parse(e.CommandArgument.ToString());
                int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                Control editControl = grid.Rows[rowIndex].FindControl(TEXT_BOX_IDS[_columnIndex]) as Control;
                editControl.Focus();

            } catch (Exception) {

            }
        } else if (e.CommandName.Equals("Delete")) {
            DeleteRow(rowIndex);
            SetModelModified(true, true);
        } else if (e.CommandName.Equals("Copy")) {
            Copy(rowIndex);
            InsertRow(true);
            SetModelModified(true, true);
        }
        try {
            Master.HideLoadingPopup();
        } catch (Exception) { }
    }



    protected virtual void InsertRowSetData() {
        InsertRow();
        SetData();

    }

    protected virtual void InsertRow(bool goToEdit) {
        string desc = "";
        GridViewRow row = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        //TextBox[] txtInserts = new TextBox[FIELDS.Length];
        List<String> selectedFields = new List<String>();
        List<TextBox> selectedTextBoxes = new List<TextBox>();
        selectedTextBoxes.Add(null);
        selectedFields.Add(FIELDS[0]);
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = row.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
            //txtInserts[i] = row.FindControl(TEXT_BOX_IDS[i]) as TextBox;
            //if (txtInserts[i] == null) { return; }
            //if (!txtInserts[i].Text.Equals(String.Empty)) {
            //    selectedFields.Add(FIELDS[i]);
            //    selectedTextBoxes.Add(txtInserts[i]);
            //}
        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        //string command = GetCommandString(Command.INSERT, selectedFields.ToArray());
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            /*for (int i = 1; i < selectedFields.Count; i++) {
                string value = selectedTextBoxes.ElementAt(i).Text;
                cmd.Parameters.AddWithValue(selectedFields[i], value);
            }*/

            for (int i = 1; i < txtControls.Length; i++) {
                string value;
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (COMBOS[i]) {
                        if (((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedItem != null) {
                            value = ((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedValue;
                        } else {
                            value = MyUtilities.clean(((AjaxControlToolkit.ComboBox)txtControls[i]).Text);
                        }
                    } else if (CHECKBOXES[i]) {
                        value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                    } else {
                        value = MyUtilities.clean(((TextBox)txtControls[i]).Text);
                        
                    }
                    if (!COMBOS[i] && (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam"))) {
                        value = value.ToUpper();
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    if (i == 1) {
                        desc = value;
                    }
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                connec.Close();
                if (goToEdit) {
                    try {
                        SetData();
                        int id = int.Parse(GetId(desc));
                        GoToEditMode(id);
                    } catch (Exception) { }
                }

            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                SaveInsertValues(grid.FooterRow, TEXT_BOX_IDS);
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    protected void InsertRow() {
        InsertRow(false);

    }

    protected virtual string GetId(string desc) {
        return GetDatabaseField(FIELDS[0], FIELDS[1], desc, TABLE_NAME);
    }

    protected void InitializeComponent() {

        TEXT_BOX_IDS = GetIDs(IDs.TEXT_BOX);
        LABEL_IDS = GetIDs(IDs.LABEL);


        secondPanel = GetSecondPanel();

        // code to generate the yellow pop up box edit/copy/delete/show line
        Panel popupPanel = InputPageControls.GeneratePopupPanel();
        pnlMainGrid.Controls.Add(popupPanel);

        AjaxControlToolkit.ModalPopupExtender extender = InputPageControls.GeneratePopupExtender();
        pnlMainGrid.Controls.Add(extender);

        Button btnOk = popupPanel.FindControl(InputPageControls.BTN_OK) as Button;
        btnOk.Click += new EventHandler(this.btnOk_Click);

        rdbtnCopy = popupPanel.FindControl(InputPageControls.RDBTN_COPY) as RadioButton;
        rdbtnDelete = popupPanel.FindControl(InputPageControls.RDBTN_DELETE) as RadioButton;
        rdbtnEdit = popupPanel.FindControl(InputPageControls.RDBTN_EDIT) as RadioButton;
        rdbtnShowLine = popupPanel.FindControl(InputPageControls.RDBTN_SHOW_LINE) as RadioButton;
        btnCancel = popupPanel.FindControl(InputPageControls.BTN_CANCEL) as Button;

        grid = GenerateGridControl();
        pnlMainGrid.Controls.Add(grid);

        btnAdvanced = new Button();
        btnAdvanced.ID = "btnAdvancedMode";
        btnAdvanced.Text = "Show Optional Parameters";
        btnAdvanced.Click += btnAdvanced_Click;

        Control buttondiv = getButtonDiv();
        if (hasAdvanced) {
            buttondiv.Controls.Add(btnAdvanced);
        }

        Button btnCopyTable = new Button();
        btnCopyTable.ID = PageControls.BTN_COPY_TABLE;
        btnCopyTable.Text = "Copy Table";  //  size  ??  //  returnzz
        btnCopyTable.Height = 26;
        buttondiv.Controls.Add(btnCopyTable);

        Panel anotherPanel = new Panel();
        anotherPanel.ID = "pnlAnother";
        pnlMainGrid.Controls.Add(anotherPanel);

        // code for pop up line panel
        //Panel popupLinePanel = GeneratePopupLine();
        //popupLinePanel.Attributes.Add("style","display: none; position: absolute !important; height:350px; overflow:scroll;");
        //popupLinePanel.Attributes.Add("style", "display: block; position: absolute !important;");
        //anotherPanel.Controls.Add(popupLinePanel);
        //anotherPanel.Attributes.Add("style", "height: 100%");

        //AjaxControlToolkit.ModalPopupExtender extender2 = InputPageControls.GeneratePopupLineExtender();
        //extender2.RepositionMode = AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll;
        //extender2.RepositionMode = AjaxControlToolkit.ModalPopupRepositionMode.None;
        //extender2.PopupDragHandleControlID = "pnlDummy";
        //Panel dummyPanel = new Panel();
        //dummyPanel.ID = extender2.PopupDragHandleControlID;
        //anotherPanel.Controls.Add(dummyPanel);
        //extender2.Y = 120;
        //anotherPanel.Controls.Add(extender2);
        //extenderLinePopup = extender2;

        //pnlPopupLine = popupLinePanel;
        //lblRow = getLblRow();
        //secondPanel.CssClass = "gridPanel";

        //rdbtnCopyLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_COPY_LINE) as RadioButton;
        //rdbtnDeleteLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_DELETE_LINE) as RadioButton;
        //rdbtnEditLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_EDIT_LINE) as RadioButton;
        //rdbtnNewLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_NEW_LINE) as RadioButton;
        //btnAddLine = pnlPopupLine.FindControl(InputPageControls.BTN_ADD) as Button;
        //btnOkLine = pnlPopupLine.FindControl(InputPageControls.BTN_OK_LINE) as Button;
        //lblAction = pnlPopupLine.FindControl(InputPageControls.LBL_ACTION) as Label;

        //btnOkLine.Click += new EventHandler(btnOkLine_Click);



        thirdPanel = GetThirdPanel();

        //Panel pnlNewName = InputPageControls.GenerateNewNamePanel("Enter new description name for the copied record: ");
        //thirdPanel.Controls.Add(pnlNewName);
        //this.pnlNewName = pnlNewName;
        //Button btnOkNewName = pnlNewName.FindControl(InputPageControls.BTN_OK_NEW_NAME) as Button;
        //btnOkNewName.Click += new EventHandler(btnOkNewName_Click);

        //AjaxControlToolkit.ModalPopupExtender newNameExtender = InputPageControls.GenerateNewNameExtender();
        //thirdPanel.Controls.Add(newNameExtender);
        //extenderNewName = newNameExtender;

        txtInsertTable = new TextBox();
        txtInsertTable.Attributes.Add("style", "width: 85%; min-width:600px; height: 150px;");
        txtInsertTable.TextMode = TextBoxMode.MultiLine;
        txtInsertTable.ID = TXT_INSERT_TABLE;
        thirdPanel.Controls.Add(txtInsertTable);
        thirdPanel.Controls.Add(new LiteralControl("<br />"));

        btnInsertTable = new Button();
        btnInsertTable.Text = "Insert";
        btnInsertTable.Click += new EventHandler(btnInsertTable_Click);
        thirdPanel.Controls.Add(btnInsertTable);

        Panel fourthPanel = GetFourthPanel();
        Panel pnlInfo = PageControls.generateInfoPanel();
        fourthPanel.Controls.Add(pnlInfo);

        btnInsertMultipleWarningOk = pnlInfo.FindControl(PageControls.BTN_INSERT_MULTIPLE_WARNING_OK) as Button;


        extenderInfo = PageControls.generateInfoExtender();
        fourthPanel.Controls.Add(extenderInfo);
        LinkButton btnDummy3 = new LinkButton();
        btnDummy3.ID = PageControls.BTN_DUMMY_INFO;
        fourthPanel.Controls.Add(btnDummy3);

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

    protected void btnCopyDone_Click(object sender, EventArgs e) {
        boxCheckAll.Checked = false;
        FillDefaultInsertRow();
    }

    protected virtual void rdbtnTable_CheckedChanged(object sender, EventArgs e) {
        if (rdbtnTableWithHeaders.Checked) {
            txtCopyTable.Text = TableStringHeaders + TableString;
        } else {
            txtCopyTable.Text = TableString;
        }
        boxCheckAll.Checked = false;
        extenderCopy.Show();
    }

    protected virtual void btnInsertTable_Click(object sender, EventArgs e) {
        string fields = txtInsertTable.Text;
        txtInsertTable.Text = "";
        string[] lines = fields.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++) {
            string[] entries = lines[i].Split(new Char[] { '\t', ';', ',' });
            if (!InsertRecord(entries)) {
                txtInsertTable.Text += lines[i] + "\n";
            } else {
                SetModelModified(true);
                
            }
        }
        SyncTables();
        if (!txtInsertTable.Text.Trim().Equals("")) {
            ShowMultipleInsertWarning();
        }
    }

    protected virtual bool InsertRecord(string[] entries) {
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            int i = 1;
            for (int j = 0; j < entries.Length && i < FIELDS.Length; j++) {
                if (mode.Equals("Standard")) {
                    while (i < FIELDS.Length && ADVANCED_FIELDS[i]) {
                        i++;
                    }
                }
                if (i < FIELDS.Length) {
                    string value = MyUtilities.clean(entries[j]);
                    if (value.Trim().Length > 0) {
                        value = value.Trim();
                    }
                    if (CHECKBOXES[i]) {
                        value = value.ToLower().Equals("true") ? "1" : "0";
                    }
                    if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam")) {
                        value = value.ToUpper();
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    i++;
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                wasInserted = true;
                connec.Close();
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        FillDefaultInsertRow();
        return wasInserted;
    }

    protected abstract Label getLblRow();

    protected override void OnInit(EventArgs e) {
        if (PAGENAME.Equals("products_routing.aspx")) {
            wantCopyButton = false;
        }
        base.OnInit(e);
        InitializeComponent();

        if (wantTwoHeaders) {
            SetupSecondHeaders();
        }


    }

    protected virtual void SetupSecondHeaders() {

    }

    protected abstract Control getButtonDiv();

    protected void InitializeCombos() {
        COMBOS = new bool[FIELDS.Length];
        for (int i = 0; i < FIELDS.Length; i++) {
            COMBOS[i] = false;
        }
    }

    protected void InitializeCheckboxes() {
        CHECKBOXES = new bool[FIELDS.Length];
        for (int i = 0; i < FIELDS.Length; i++) {
            CHECKBOXES[i] = false;
        }
    }

    public Panel GeneratePopupLine() {
        Panel panel = new Panel();
        panel.ID = InputPageControls.PNL_MODAL_LINE;
        panel.CssClass = "popPanel";
        //panel.Attributes.Add("style", "display: block;");

        Label lblAction = new Label();
        lblAction.ID = InputPageControls.LBL_ACTION;
        lblAction.CssClass = "lblHeader";
        panel.Controls.Add(lblAction);
        panel.Controls.Add(new LiteralControl("<br />"));


        rdbtnsChoiceModePanel = new Panel();
        rdbtnsChoiceModePanel.Controls.Add(new LiteralControl("<br />"));
        RadioButton rdbtnCopy = new RadioButton();
        rdbtnCopy.ID = InputPageControls.RDBTN_COPY_LINE;
        rdbtnCopy.Text = "Copy";
        rdbtnCopy.CssClass = RDTNS_CSS_CLASS;
        rdbtnCopy.GroupName = "secondGroup";
        rdbtnsChoiceModePanel.Controls.Add(rdbtnCopy);

        RadioButton rdbtnEdit = new RadioButton();
        rdbtnEdit.ID = InputPageControls.RDBTN_EDIT_LINE;
        rdbtnEdit.Text = "Edit";
        rdbtnEdit.CssClass = rdbtnCopy.CssClass;
        rdbtnEdit.GroupName = rdbtnCopy.GroupName;
        rdbtnsChoiceModePanel.Controls.Add(rdbtnEdit);

        RadioButton rdbtnDelete = new RadioButton();
        rdbtnDelete.ID = InputPageControls.RDBTN_DELETE_LINE;
        rdbtnDelete.Text = "Delete";
        rdbtnDelete.CssClass = rdbtnCopy.CssClass;
        rdbtnDelete.GroupName = rdbtnCopy.GroupName;
        rdbtnsChoiceModePanel.Controls.Add(rdbtnDelete);

        RadioButton rdbtnNewLine = new RadioButton();
        rdbtnNewLine.ID = InputPageControls.RDBTN_NEW_LINE;
        rdbtnNewLine.Text = "New";
        rdbtnNewLine.CssClass = rdbtnCopy.CssClass;
        rdbtnNewLine.GroupName = rdbtnCopy.GroupName;
        rdbtnsChoiceModePanel.Controls.Add(rdbtnNewLine);
        rdbtnsChoiceModePanel.Controls.Add(new LiteralControl("<br />"));
        panel.Controls.Add(rdbtnsChoiceModePanel);




        panel.Controls.Add(new LiteralControl("<br />"));
        popupTable = new Table();
        popupTable.ID = "popupTable";

        for (int i = 1; i < TEXT_BOX_IDS2.Length; i++) {
            TableRow row = new TableRow();
            TableCell cell1 = new TableCell();
            cell1.Controls.Add(new LiteralControl("<span class=\"lblBold\">" + HEADERS[i] + "</span>"));

            row.Cells.Add(cell1);
            TableCell cell2 = new TableCell();

            TextBox txtBox;
            AjaxControlToolkit.ComboBox combo;
            Label label;
            CheckBox checkBox;
            if (COMBOS[i]) {
                combo = new AjaxControlToolkit.ComboBox();
                combo.ID = TEXT_BOX_IDS2[i];
                combo.CssClass = "comboBoxInsideModalPopup";
                combo.AutoPostBack = false;
                combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
                combo.AutoCompleteMode = AjaxControlToolkit.ComboBoxAutoCompleteMode.SuggestAppend;
                List<String> dropList = GetDropList(FIELDS[i]);
                foreach (string item in dropList) {
                    combo.Items.Add(item);
                }
                cell2.Controls.Add(combo);
            } else if (CHECKBOXES[i]) {
                checkBox = new CheckBox();
                checkBox.ID = TEXT_BOX_IDS2[i];
                cell2.Controls.Add(checkBox);
            } else if (nonEdits && fieldsNonEditable[i]) {
                /*label = new Label();
                label.ID = TEXT_BOX_IDS2[i];
                cell2.Controls.Add(label);*/
                txtBox = new TextBox();
                txtBox.ID = TEXT_BOX_IDS2[i];
                txtBox.Enabled = false;
                cell2.Controls.Add(txtBox);
            } else {
                txtBox = new TextBox();
                txtBox.ID = TEXT_BOX_IDS2[i];
                cell2.Controls.Add(txtBox);
            }

            row.Controls.Add(cell2);
            popupTable.Rows.Add(row);
        }

        panel.Controls.Add(popupTable);

        panel.Controls.Add(new LiteralControl("<br />"));

        Button btnAdd = new Button();
        btnAdd.ID = InputPageControls.BTN_ADD;
        btnAdd.Text = "Add";
        panel.Controls.Add(btnAdd);
        btnAdd.Click += new EventHandler(btnAdd_Click);

        Button btnOk = new Button();
        btnOk.ID = InputPageControls.BTN_OK_LINE;
        btnOk.Text = "Ok";
        panel.Controls.Add(btnOk);

        Button btnCancel = new Button();
        btnCancel.ID = InputPageControls.BTN_CANCEL_LINE;
        btnCancel.Text = "Cancel";
        panel.Controls.Add(btnCancel);
        panel.Attributes.Add("style", "display:none;");

        return panel;
    }

    public void FillPopupLine() {
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string comm = GetCommandString(Command.SELECT);
        int index = comm.ToLower().IndexOf(" where");
        if (index > -1) {
            comm = comm.Substring(0, index);
        } else {
            index = comm.ToLower().IndexOf(" order");
            if (index > -1) {
                comm = comm.Substring(0, index);
            }
        }

        comm += " WHERE " + FIELDS[0] + "=" + lblRow.Text + ";";
        OleDbCommand cmd = new OleDbCommand(comm, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 1; i < TEXT_BOX_IDS2.Length; i++) {
                    Control control = pnlPopupLine.FindControl(TEXT_BOX_IDS2[i]);

                    if (COMBOS[i]) {
                        AjaxControlToolkit.ComboBox combo = control as AjaxControlToolkit.ComboBox;
                        combo.Text = dt.Rows[0][i].ToString();
                    } else if (CHECKBOXES[i]) {
                        CheckBox checkBox = control as CheckBox;
                        checkBox.Checked = dt.Rows[0][i].ToString().Equals("True");
                    } else {
                        /*Label label = control as Label;
                        label.Text = dt.Rows[0][i].ToString();*/
                        TextBox txtBox = control as TextBox;
                        txtBox.Text = dt.Rows[0][i].ToString();
                    }


                }
                connec.Close();

            } catch { }
        }
    }

    protected abstract Panel GetSecondPanel();

    protected void btnOkLine_Click(object sender, EventArgs e) {
        if (rdbtnCopyLine.Checked) {
            CopyLine();
            rdbtnCopyLine.Checked = false;
        } else if (rdbtnDeleteLine.Checked) {
            DeleteRow();
            rdbtnDeleteLine.Checked = false;
        } else if (rdbtnEditLine.Checked) {
            EditLine();
            rdbtnEditLine.Checked = false;
        } else if (rdbtnNewLine.Checked) {
            CleanFields();
            FillDefaultNewLine();
            ToAddMode();
            Control control = pnlPopupLine.FindControl(TEXT_BOX_IDS2[1]);
            control.Focus();
            rdbtnNewLine.Checked = false;
        }
    }

    protected void btnOkNewName_Click(object sender, EventArgs e) {
        TextBox txtNewName = pnlNewName.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
        TextBox txtName = pnlPopupLine.FindControl(TEXT_BOX_IDS2[1]) as TextBox;
        txtName.Text = txtNewName.Text;
        ToAddMode();
    }

    protected void btnAdd_Click(object sender, EventArgs e) {
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = pnlPopupLine.FindControl(TEXT_BOX_IDS2[i]);
            if (txtControls[i] == null) { return; }

        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            for (int i = 1; i < txtControls.Length; i++) {
                string value;
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (COMBOS[i]) {
                        value = ((AjaxControlToolkit.ComboBox)txtControls[i]).Text;
                    } else if (CHECKBOXES[i]) {
                        value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                    } else {
                        value = ((TextBox)txtControls[i]).Text;
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                connec.Close();
            } catch {
                extenderLinePopup.Show();
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    protected void EditLine() {
        int rowIndex = GetCurrentRowIndex();
        GridViewRow row = grid.Rows[rowIndex];

        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.UPDATE);
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++) {
                Control control = pnlPopupLine.FindControl(TEXT_BOX_IDS2[i]);
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (COMBOS[i]) {
                        cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(((AjaxControlToolkit.ComboBox)control).Text));
                    } else if (CHECKBOXES[i]) {
                        cmd.Parameters.AddWithValue(FIELDS[i], (((CheckBox)control).Checked) ? "1" : "0");
                    } else {
                        cmd.Parameters.AddWithValue(FIELDS[i], MyUtilities.clean(((TextBox)control).Text));
                    }
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();

                this.SetData();
                connec.Close();
            } catch {
                extenderLinePopup.Show();
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }

    }

    protected void CopyLine() {
        TextBox txtNewName = pnlNewName.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
        txtNewName.Text = "";
        extenderNewName.Show();
    }

    protected virtual void ToAddMode() {
        rdbtnsChoiceModePanel.CssClass = "hidden";
        rdbtnsChoiceModePanel.Visible = false;
        btnAddLine.Visible = true;
        btnOkLine.Visible = false;
        lblAction.Text = "Insert a new set";
        extenderLinePopup.Show();
    }

    protected virtual void ToChoiceMode() {
        rdbtnsChoiceModePanel.CssClass = "";
        rdbtnsChoiceModePanel.Visible = true;
        btnOkLine.Visible = true;
        btnAddLine.Visible = false;
        lblAction.Text = "Choose the action: ";
        extenderLinePopup.Show();
    }

    protected void FillDefaultNewLine() {
        FillDefaultLine(pnlPopupLine, TEXT_BOX_IDS2);
    }

    protected void FillInsertRowWithCurrValues(Control container, string[] ids) {
        Control[] txtNewRecord = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtNewRecord[i] = container.FindControl(ids[i]);
            if (txtNewRecord[i] != null) {
                if (COMBOS[i]) {
                    ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text = ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text;
                } else if (CHECKBOXES[i]) {
                    ((CheckBox)txtNewRecord[i]).Checked = ((CheckBox)txtNewRecord[i]).Checked;
                } else {
                    ((TextBox)txtNewRecord[i]).Text = ((TextBox)txtNewRecord[i]).Text;
                }

            }

        }

    }

    private void FillDefaultLine(Control container, string[] ids) {
        string dbpath = GetDirectory() + userDir + MAIN_USER_DATABASE;
        DAO.Database dat;

        Control[] txtNewRecord = new Control[FIELDS.Length];

        try {
            DAO.DBEngine daoEngine = new DAO.DBEngine();
            dat = daoEngine.OpenDatabase(dbpath, false, false, "");
            DAO.TableDef tableDef = dat.TableDefs[TABLE_NAME];
            for (int i = 1; i < FIELDS.Length; i++) {
                if (!FIELDS[i].Equals("opnam1") && !FIELDS[i].Equals("opnam2")) { // aliases which are not in the database
                    string defaultValue = (string)tableDef.Fields[FIELDS[i]].Properties["DefaultValue"].Value;
                    if (defaultValue != null && !defaultValue.Equals("\" \"") && !defaultValue.Equals("")) {
                        txtNewRecord[i] = container.FindControl(ids[i]);
                        if (txtNewRecord[i] != null) {
                            defaultValue = MyUtilities.clean(defaultValue, '"');
                            if (COMBOS[i]) {
                                ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text = defaultValue;
                            } else if (CHECKBOXES[i]) {
                                ((CheckBox)txtNewRecord[i]).Checked = defaultValue.ToString().Equals("-1");
                            } else {
                                ((TextBox)txtNewRecord[i]).Text = defaultValue;
                            }

                        }
                    }
                }
            }


            dat.Close();
        } catch (Exception) { }
    }

    protected void CleanFields() {
        Control[] controls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            controls[i] = pnlPopupLine.FindControl(TEXT_BOX_IDS2[i]);
            if (COMBOS[i]) {
                //((AjaxControlToolkit.ComboBox) controls[i]).Text = "";
            } else if (CHECKBOXES[i]) {
                ((CheckBox)controls[i]).Checked = false;
            } else {
                ((TextBox)controls[i]).Text = "";
            }
        }

    }

    protected void insertToEmptyTable() {
        CleanFields();
        FillDefaultNewLine();
        ToAddMode();
    }

    protected abstract Panel GetFourthPanel();

    protected abstract Panel GetFifthPanel();

    protected Panel generatePopupInfoPanel() {
        Panel panel = new Panel();
        panel.ID = PageControls.INFO_POPUP_ID;
        panel.CssClass = "infoPanel";

        return panel;
    }

    protected void btnAdvanced_Click(object sender, EventArgs e) {
        if (btnAdvanced.Text.Equals("Show Optional Parameters")) {
            mode = "Advanced";
            btnAdvanced.Text = "Hide Optional Parameters";
        } else {
            mode = "Standard";
            btnAdvanced.Text = "Show Optional Parameters";
        }

        for (int i = 0; i < FIELDS.Length; i++) {
            if (ADVANCED_FIELDS[i]) {
                grid.Columns[i].Visible = !grid.Columns[i].Visible;
                // popupTable.Rows[i - 1].Visible = !popupTable.Rows[i - 1].Visible;
            }
        }
        this.SetData();
    }

    protected void HideAdvancedColumns() {
        for (int i = 0; i < FIELDS.Length; i++) {
            if (ADVANCED_FIELDS[i]) {
                grid.Columns[i].Visible = false;
                //popupTable.Rows[i - 1].Visible = false; // for pop up line!!!
            }
        }
    }

    private void ShowAdvancedColumns() {
        for (int i = 0; i < FIELDS.Length; i++) {
            if (ADVANCED_FIELDS[i]) {
                grid.Columns[i].Visible = true;
                popupTable.Rows[i - 1].Visible = true;
            }
        }
    }

    public override Panel GetSortPanelContainer() {
        secondPanel = GetSecondPanel();
        return secondPanel;
    }

    public override Control GetSortButtonContainer() {
        Control buttondiv = getButtonDiv();
        return buttondiv;

    }

    protected abstract Panel GetThirdPanel();

    protected override void RefreshData() {
        this.SetData();
    }

    protected void GoToEditMode(int id) {
        bool found = false;
        int index = 0;
        int pageIndex = 0;
        for (int i = 0; i < grid.PageCount; i++) {
            for (index = 0; index < grid.DataKeys.Count; index++) {
                if (Convert.ToInt32(grid.DataKeys[index].Value.ToString()) == id) {
                    found = true;
                    break;
                }
            }

            if (found) {
                break;
            }

            pageIndex++;
            grid.PageIndex = pageIndex;
            this.SetData();
        }

        if (found) {
            grid.PageIndex = pageIndex;
            grid.EditIndex = index;

        } else {
            grid.EditIndex = -1;
            grid.PageIndex = 0;
        }
        SetData();
        index = grid.EditIndex;
        if (index >= 0) {
            try {
                Control control = grid.Rows[index].FindControl(TEXT_BOX_IDS[1]);
                control.Focus();
            } catch (Exception) { }
        }
    }

    protected void ShowMultipleInsertWarning() {
        Master.SetFocus(btnInsertMultipleWarningOk.ClientID);
        extenderInfo.Show();
    }

    protected void FillDefaultInsertRow() {
        GridViewRow gridRow = grid.FooterRow;
        if (gridRow != null) {
            if (savedInsertValues != null) {
                FillInsertRowWithSavedValues(gridRow, TEXT_BOX_IDS);
            } else {
                FillDefaultLine(gridRow, TEXT_BOX_IDS);
            }
        }
    }

    protected void FillInsertRowWithSavedValues(Control container, string[] ids) {
        Control[] txtNewRecord = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtNewRecord[i] = container.FindControl(ids[i]);
            if (txtNewRecord[i] != null) {
                if (COMBOS[i]) {
                    ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text = (string)savedInsertValues[i];
                } else if (CHECKBOXES[i]) {
                    ((CheckBox)txtNewRecord[i]).Checked = (bool)savedInsertValues[i];
                } else {
                    ((TextBox)txtNewRecord[i]).Text = (string)savedInsertValues[i];
                }

            }

        }

    }

    protected void SaveInsertValues(Control container, string[] ids) {
        Control[] txtNewRecord = new Control[FIELDS.Length];
        savedInsertValues = new object[ids.Length];
        for (int i = 1; i < ids.Length; i++) {
            txtNewRecord[i] = container.FindControl(ids[i]);
            if (txtNewRecord[i] != null) {
                if (COMBOS[i]) {
                    savedInsertValues[i] = ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text;
                } else if (CHECKBOXES[i]) {
                    savedInsertValues[i] = ((CheckBox)txtNewRecord[i]).Checked;
                } else {
                    savedInsertValues[i] = ((TextBox)txtNewRecord[i]).Text;
                }

            }
        }
    }





}
