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

public abstract class GridPage : DbPage {

    protected string TABLE_NAME;

    protected Panel panel;

    protected DataTable dt;

    protected GridView grid;
    protected Label lblRow;

    protected OleDbConnection connec = new OleDbConnection();

    protected string GRID_NAME = "grid";
    protected string GRID_CONTAINER = "gridPanel";
    protected string[] STANDARD_FIELDS;
    protected string[] ADVANCED_FIELDS;
    protected string[] FIELDS;

    protected bool[] STANDARD_COMBOS;
    protected bool[] ADVANCED_COMBOS;
    protected bool[] COMBOS;

    protected bool[] STANDARD_CHECKBOXES;
    protected bool[] ADVANCED_CHECKBOXES;
    protected bool[] CHECKBOXES;

    protected const String TABLE_MODE_STANDARD = "Standard";
    protected const String TABLE_MODE_ADVANCED = "Advanced";
    protected String tableMode = TABLE_MODE_STANDARD;

    protected string[] TEXT_BOX_IDS;
    protected string[] LABEL_IDS;
    protected string[] HEADERS;

    protected string[] TEXT_BOX_IDS2;
    protected string[] LABEL_IDS2;

    protected string[] STANDARD_HEADERS;
    protected string[] ADVANCED_HEADERS;

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
    HtmlGenericControl txtClipboard;

    protected AjaxControlToolkit.ComboBox[] comboSorts;
    protected RadioButton[] rdbtnSortAsc;
    protected RadioButton[] rdbtnSortDesc;

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

    protected string RDTNS_CSS_CLASS = "rdtnsInline";
    protected Panel rdbtnsChoiceModePanel;


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

    void Page_PreInit(Object sender, EventArgs e) {
        this.MasterPageFile = "~/MasterPage.master";
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
        grid = panel.FindControl(GRID_NAME) as GridView;
        if (!Page.IsPostBack || isModeSwitch()) {
            this.SetData();
        }


    }

    private GridView GenerateGridControl() {
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
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
        //newGrid.Sorting += new GridViewSortEventHandler(this.OnSorting);
        newGrid.PageIndexChanging += new GridViewPageEventHandler(this.PageIndexChanging);
        newGrid.RowDataBound += new GridViewRowEventHandler(this.Grid_RowDataBound);
        newGrid.RowCommand += new GridViewCommandEventHandler(this.grid_RowCommand);
        newGrid.RowUpdating += new GridViewUpdateEventHandler(this.grid_RowUpdating);
        newGrid.RowDeleting += this.grid_RowDeleting;
        newGrid.EmptyDataText = "There are no data records to display.";
        newGrid.EmptyDataTemplate = new GridViewTemplate(GridViewTemplate.EMPTY_TABLE);

        TemplateField templ = new TemplateField();
        templ.ItemTemplate = new GridViewTemplate(ListItemType.Item, GridViewTemplate.BUTTONS, FIELDS[0], btnCancel.ClientID);
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


            if (COMBOS[i]) {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList());
            } else if (CHECKBOXES[i]) {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.CHECKBOX_DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            } else {
                editTemplate = new GridViewTemplate(ListItemType.EditItem, GridViewTemplate.DATA, FIELDS[i], TEXT_BOX_IDS[i]);
            }

            template.EditItemTemplate = new CompiledBindableTemplateBuilder(delegate(Control container) {
                editTemplate.InstantiateIn(container);
            },
                delegate(Control container) {

                    OrderedDictionary dict = new OrderedDictionary();
                    for (int j = 1; j < FIELDS.Length; j++) {
                        if (COMBOS[j]) {
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
                template.FooterTemplate = new GridViewTemplate(ListItemType.Footer, GridViewTemplate.COMBODATA, FIELDS[i], TEXT_BOX_IDS[i], GetDropList());
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

    private void FillDefaultInsertRow() {
        GridViewRow gridRow = grid.FooterRow;
        FillDefaultLine(gridRow, TEXT_BOX_IDS);
    }

    private List<string> GetDropList() {
        List<string> dropList = new List<string>();
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + "\\" + MAIN_USER_DATABASE + ";");
        string comm = "SELECT LaborDesc FROM tbllabor";
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

            } catch { }
        }
        return dropList;
    }

    protected void SetData() {
        grid = panel.FindControl(GRID_NAME) as GridView;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + "\\" + MAIN_USER_DATABASE + ";");
        string comm = GetCommandString(Command.SELECT);
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.SELECT), connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                dt = new DataTable();
                adapter.Fill(dt);
                DataView dv = dt.DefaultView;
                dv.Sort = this.Sort;

                grid.DataSource = dv;
                grid.DataBind();
                string copiedTable = "";
                TableString = "";
                TableStringHeaders = "";
                for (int i = 1; i < HEADERS.Length; i++) {
                    copiedTable += HEADERS[i];
                    TableStringHeaders += HEADERS[i];
                    if (i != HEADERS.Length - 1) {
                        copiedTable += "\t"; // tab for next cell
                        TableStringHeaders += "\t";
                    } else {
                        copiedTable += "\r\n"; // end of table line
                        TableStringHeaders += "\r\n";
                    }
                }
                foreach (DataRow row in dt.Rows) {
                    for (int i = 1; i < dt.Columns.Count; i++) {
                        copiedTable += row[i].ToString();
                        TableString += row[i].ToString();
                        if (i != dt.Columns.Count - 1) {
                            copiedTable += "\t"; // tab for next cell
                            TableString += "\t";
                        } else {
                            copiedTable += "\r\n"; // end of table line
                            TableString += "\r\n";
                        }
                    }
                }
                //txtClipboard.Attributes.Remove("value");
                //txtClipboard.Attributes.Add("value", copiedTable);
                txtCopyTable.Text = copiedTable;
                connec.Close();
                FillDefaultInsertRow();
            } catch { }
        }
    }

    protected void PageIndexChanging(object sender, GridViewPageEventArgs e) {
        grid = panel.FindControl(GRID_NAME) as GridView;
        grid.PageIndex = e.NewPageIndex;
        this.SetData();
    }

    protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.Header) {
            foreach (TableCell cell in e.Row.Cells) {
                foreach (Control ctl in cell.Controls) {
                    if (ctl.GetType().ToString().Contains("DataControlLinkButton")) {
                        cell.Attributes.Add("title", "Sort");
                    }
                }
            }
        } else if (e.Row.RowType == DataControlRowType.DataRow) {
            e.Row.Cells[1].ForeColor = ColorTranslator.FromHtml("black");
            e.Row.Cells[1].Font.Bold = false;
        }
    }

    private string Sort {
        get {
            if (ViewState["Sort"] == null) {
                ViewState["Sort"] = FIELDS[0] + " ASC";
            }
            return (string)ViewState["Sort"];
        }
        set { ViewState["Sort"] = value; }

    }

    private string TableStringHeaders {
        get {
            if (ViewState["TableStringHeaders"] == null) {
                ViewState["TableStringHeaders"] = "";
            }
            return (string)ViewState["TableStringHeaders"];
        }
        set { ViewState["TableStringHeaders"] = value; }
    }

    private string TableString {
        get {
            if (ViewState["TableString"] == null) {
                ViewState["TableString"] = "";
            }
            return (string)ViewState["TableString"];
        }
        set { ViewState["TableString"] = value; }
    }

    private IDictionary<string, object> GetValues(GridViewRow row) {
        IOrderedDictionary dictionary = new OrderedDictionary();
        foreach (Control control in row.Controls) {
            DataControlFieldCell cell = control as DataControlFieldCell;

            if ((cell != null) && cell.Visible) {
                cell.ContainingField.ExtractValuesFromCell(dictionary, cell, row.RowState, true);
            }
        }
        IDictionary<string, object> values = new Dictionary<string, object>();
        foreach (DictionaryEntry de in dictionary) {
            values[de.Key.ToString()] = de.Value;
        }
        return values;
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
                command = "SELECT " + fieldEnum + " FROM " + "[" + TABLE_NAME + "];";
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

    protected string GetCommandString(int commandType) {
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

    protected void Copy() {
        grid = panel.FindControl(GRID_NAME) as GridView;
        int rowIndex = GetCurrentRowIndex();
        Copy(rowIndex);
    }

    protected void Copy(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);

        GridViewRow insertRow = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = insertRow.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
        }
        for (int i = 2; i < FIELDS.Length; i++) {
            if (COMBOS[i]) {
                ((AjaxControlToolkit.ComboBox)txtControls[i]).Text = newValues[FIELDS[i]].ToString();
            }
            if (CHECKBOXES[i]) {
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

    protected void RowUpdate() {
        int rowIndex = GetCurrentRowIndex();
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);
        string oldField = "";
        if (TABLE_NAME.Equals("tbllabor")) {
            oldField = GetOldField(grid.DataKeys[row.RowIndex][FIELDS[0]].ToString());
        }

        string oldValue = GetOldField(grid.DataKeys[row.RowIndex][FIELDS[0]].ToString());
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++) {
                if (CHECKBOXES[i]) {
                    object bla = newValues[FIELDS[i]];
                    cmd.Parameters.AddWithValue(FIELDS[i], ((bool)newValues[FIELDS[i]]) ? "1" : "0");
                } else {
                    cmd.Parameters.AddWithValue(FIELDS[i], newValues[FIELDS[i]]);
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                grid.EditIndex = -1;

                this.SetData();
                connec.Close();
                if (TABLE_NAME.Equals("tbllabor")) {
                    SyncTables(newValues["LaborDesc"].ToString(), oldField);
                }
            } catch { }
        }
    }

    protected void SyncTables(string newValue, string oldValue) {
        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";";
        string command = "UPDATE tblequip SET LaborDesc = '" + newValue + "' WHERE LaborDesc = '" + oldValue + "';";
        DbUse.RunSql(command, connectionString);

    }

    protected string GetOldField(string id) {
        string oldField = "";
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand("SELECT LaborDesc FROM tbllabor WHERE LaborID = " + id + ";", connec);
        OleDbDataAdapter oleAdapter = new OleDbDataAdapter(cmd);
        try {
            connec.Open();
            DataTable dt = new DataTable();
            oleAdapter.Fill(dt);
            oldField = dt.Rows[0][0].ToString();
            connec.Close();
        } catch (Exception) {
            try {
                connec.Close();
            } catch (Exception) { }
        }
        return oldField;
    }

    protected void RowUpdateCancel() {
        grid.EditIndex = -1;
        this.SetData();
    }

    protected void DeleteRow(int rowIndex) {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.DELETE), connec);
        cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);

        try {
            connec.Open();
            int result = cmd.ExecuteNonQuery();
            grid.EditIndex = -1;
            this.SetData();
            connec.Close();

        } catch { }
    }

    protected void DeleteRow() {
        int rowIndex = GetCurrentRowIndex();
        DeleteRow(rowIndex);
    }

    protected void grid_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }
        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        if (e.CommandName.Equals("Insert")) {
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
            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
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
                    if (COMBOS[i]) {
                        value = ((AjaxControlToolkit.ComboBox)txtControls[i]).Text;
                    } else if (CHECKBOXES[i]) {
                        bool isChecked = ((CheckBox)txtControls[i]).Checked;
                        value = (isChecked) ? "1" : "0";
                    } else {
                        value = ((TextBox)txtControls[i]).Text;
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                }
                try {
                    connec.Open();
                    int result = cmd.ExecuteNonQuery();
                    this.SetData();
                    connec.Close();
                } catch { }
            }
        } else if (e.CommandName.Equals("Update")) {
            RowUpdate();
        } else if (e.CommandName.Equals("CancelUpdate")) {
            RowUpdateCancel();
        } else if (e.CommandName.Equals("insertInEmpty")) {
            insertToEmptyTable();
        } else if (e.CommandName.Equals("Edit")) {
            grid.EditIndex = rowIndex;
        } else if (e.CommandName.Equals("Delete")) {
            DeleteRow(rowIndex);
        } else if (e.CommandName.Equals("Copy")) {
            Copy(rowIndex);
        }
    }

    protected void grid_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }

    protected void saveTableMode(ControlCollection controls) {
        HtmlGenericControl hiddenField = new HtmlGenericControl();
        hiddenField.TagName = "input";
        hiddenField.Attributes.Add("name", "tableType");
        hiddenField.Attributes.Add("type", "hidden");
        hiddenField.Attributes.Add("value", tableMode);

        controls.Add(hiddenField);

    }

    protected String getTableMode() {
        //return Request.QueryString.GetValues("tableType")[0];
        string mode = Request.Form["tableType"];
        return (mode == null) ? "standard" : mode;

    }


    private void modeSwitch() {
        if (tableMode.Equals(TABLE_MODE_ADVANCED)) {
            tableMode = TABLE_MODE_STANDARD;
        } else {
            tableMode = TABLE_MODE_ADVANCED;
        }
    }

    private Boolean isModeSwitch() {
        //return Request.QueryString.GetValues("tableModeSwitch") != null;
        return Request.Form["tableModeSwitch"] != null;
    }


    protected void InitializeComponent() {

        HtmlGenericControl btnTableMode = new HtmlGenericControl();
        btnTableMode.TagName = "input";
        btnTableMode.Attributes.Add("name", "tableModeSwitch");
        btnTableMode.Attributes.Add("type", "submit");

        try {
            tableMode = getTableMode();
        } catch (Exception) {
        }

        if (isModeSwitch()) {
            modeSwitch();
        }

        if (tableMode.Equals(TABLE_MODE_ADVANCED)) {
            FIELDS = ADVANCED_FIELDS;
            HEADERS = ADVANCED_HEADERS;
            COMBOS = ADVANCED_COMBOS;
            CHECKBOXES = ADVANCED_CHECKBOXES;
            btnTableMode.Attributes.Add("value", TABLE_MODE_STANDARD);

        } else {
            FIELDS = STANDARD_FIELDS;
            HEADERS = STANDARD_HEADERS;
            COMBOS = STANDARD_COMBOS;
            CHECKBOXES = STANDARD_CHECKBOXES;
            btnTableMode.Attributes.Add("value", TABLE_MODE_ADVANCED);
        }

        TEXT_BOX_IDS = GetIDs(IDs.TEXT_BOX);
        LABEL_IDS = GetIDs(IDs.LABEL);
        TEXT_BOX_IDS2 = GetIDs2(IDs.TEXT_BOX);
        LABEL_IDS2 = GetIDs2(IDs.LABEL);

        secondPanel = GetSecondPanel();

        Panel popupPanel = InputPageControls.GeneratePopupPanel();
        panel.Controls.Add(popupPanel);

        AjaxControlToolkit.ModalPopupExtender extender = InputPageControls.GeneratePopupExtender();
        panel.Controls.Add(extender);

        Button btnOk = popupPanel.FindControl(InputPageControls.BTN_OK) as Button;
        btnOk.Click += new EventHandler(this.btnOk_Click);

        rdbtnCopy = popupPanel.FindControl(InputPageControls.RDBTN_COPY) as RadioButton;
        rdbtnDelete = popupPanel.FindControl(InputPageControls.RDBTN_DELETE) as RadioButton;
        rdbtnEdit = popupPanel.FindControl(InputPageControls.RDBTN_EDIT) as RadioButton;
        rdbtnShowLine = popupPanel.FindControl(InputPageControls.RDBTN_SHOW_LINE) as RadioButton;
        btnCancel = popupPanel.FindControl(InputPageControls.BTN_CANCEL) as Button;

        grid = GenerateGridControl();
        panel.Controls.Add(grid);

        Control buttondiv = getButtonDiv();
        buttondiv.Controls.Add(btnTableMode);
        saveTableMode(panel.Controls);

        Button btnSort = new Button();
        btnSort.ID = InputPageControls.BTN_SORT;
        btnSort.Text = "Sort";  //   size  ??
        btnSort.Width = 60;
        btnSort.Height = 25;
        buttondiv.Controls.Add(btnSort);

        //HtmlGenericControl btnCopyToClipboard = new HtmlGenericControl();
        //btnCopyToClipboard.TagName = "input";
        //btnCopyToClipboard.Attributes.Add("value", "Copy Table To Clipboard");
        //btnCopyToClipboard.Attributes.Add("name", "btnCopyToClipboard");
        //btnCopyToClipboard.Attributes.Add("id", "btnCopyToClipboard");
        //btnCopyToClipboard.Attributes.Add("type", "submit");
        //btnCopyToClipboard.Attributes.Add("onclick", "copyTableToClipboard()");
        //buttondiv.Controls.Add(btnCopyToClipboard);

        //txtClipboard = new HtmlGenericControl();
        //txtClipboard.TagName = "input";
        //txtClipboard.Attributes.Add("type", "hidden");
        //txtClipboard.Attributes.Add("id", "txtClipboard");
        //txtClipboard.Attributes.Add("name", "txtClipboard");
        //buttondiv.Controls.Add(txtClipboard);

        Button btnCopyTable = new Button();
        btnCopyTable.ID = PageControls.BTN_COPY_TABLE;
        btnCopyTable.Text = "Copy Table";  //  size  ??  //  returnzz

        btnCopyTable.Height = 26;

        buttondiv.Controls.Add(btnCopyTable);

        Panel anotherPanel = new Panel();
        //panel.ScrollBars = ScrollBars.Auto;
        anotherPanel.ID = "pnlAnother";
        panel.Controls.Add(anotherPanel);


        Panel popupLinePanel = GeneratePopupLine();
        //popupLinePanel.Attributes.Add("style","display: none; position: absolute !important; height:350px; overflow:scroll;");
        popupLinePanel.Attributes.Add("style", "display: block; position: absolute !important;");
        anotherPanel.Controls.Add(popupLinePanel);
        //anotherPanel.Attributes.Add("style", "height: 100%");

        AjaxControlToolkit.ModalPopupExtender extender2 = InputPageControls.GeneratePopupLineExtender();
        //extender2.RepositionMode = AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll;
        extender2.RepositionMode = AjaxControlToolkit.ModalPopupRepositionMode.None;
        extender2.PopupDragHandleControlID = "pnlDummy";
        Panel dummyPanel = new Panel();
        dummyPanel.ID = extender2.PopupDragHandleControlID;
        anotherPanel.Controls.Add(dummyPanel);
        extender2.Y = 120;
        anotherPanel.Controls.Add(extender2);
        extenderLinePopup = extender2;
        
        pnlPopupLine = popupLinePanel;
        lblRow = getLblRow();
        secondPanel.CssClass = "gridPanel";
        Panel pnlSort = InputPageControls.GenerateSortPanel();
        secondPanel.Controls.Add(pnlSort);

        comboSorts = new AjaxControlToolkit.ComboBox[InputPageControls.NUM_SORT_EXPRESSIONS];
        rdbtnSortAsc = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
        rdbtnSortDesc = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
        for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
            comboSorts[i] = pnlSort.FindControl(InputPageControls.COMBO_SORT_IDS[i]) as AjaxControlToolkit.ComboBox;
            rdbtnSortAsc[i] = pnlSort.FindControl(InputPageControls.SORT_RADIO_BTN_ASC_IDS[i]) as RadioButton;
            rdbtnSortDesc[i] = pnlSort.FindControl(InputPageControls.SORT_RADIO_BTN_DESC_IDS[i]) as RadioButton;
        }
        
        for (int i = 1; i < HEADERS.Length; i++) {
            for (int j = 0; j < InputPageControls.NUM_SORT_EXPRESSIONS; j++) {
                comboSorts[j].Items.Add(HEADERS[i]);
            }
        }
        Button btnOkSort = pnlSort.FindControl(InputPageControls.BTN_OK_SORT) as Button;
        btnOkSort.Click += new EventHandler(btnOkSort_Click);

        rdbtnCopyLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_COPY_LINE) as RadioButton;
        rdbtnDeleteLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_DELETE_LINE) as RadioButton;
        rdbtnEditLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_EDIT_LINE) as RadioButton;
        rdbtnNewLine = pnlPopupLine.FindControl(InputPageControls.RDBTN_NEW_LINE) as RadioButton;
        btnAddLine = pnlPopupLine.FindControl(InputPageControls.BTN_ADD) as Button;
        btnOkLine = pnlPopupLine.FindControl(InputPageControls.BTN_OK_LINE) as Button;
        lblAction = pnlPopupLine.FindControl(InputPageControls.LBL_ACTION) as Label;

        btnOkLine.Click += new EventHandler(btnOkLine_Click);

        AjaxControlToolkit.ModalPopupExtender sortExtender = InputPageControls.GenerateSortExtender();
        secondPanel.Controls.Add(sortExtender);

        thirdPanel = GetThirdPanel();

        Panel pnlNewName = InputPageControls.GenerateNewNamePanel("Enter new description name for the copied record: ");
        thirdPanel.Controls.Add(pnlNewName);
        this.pnlNewName = pnlNewName;
        Button btnOkNewName = pnlNewName.FindControl(InputPageControls.BTN_OK_NEW_NAME) as Button;
        btnOkNewName.Click += new EventHandler(btnOkNewName_Click);

        AjaxControlToolkit.ModalPopupExtender newNameExtender = InputPageControls.GenerateNewNameExtender();
        thirdPanel.Controls.Add(newNameExtender);
        extenderNewName = newNameExtender;

        txtInsertTable = new TextBox();
        txtInsertTable.Width = 600;
        txtInsertTable.Height = 100;
        txtInsertTable.TextMode = TextBoxMode.MultiLine;
        thirdPanel.Controls.Add(txtInsertTable);
        thirdPanel.Controls.Add(new LiteralControl("<br />"));

        btnInsertTable = new Button();
        btnInsertTable.Text = "Insert";
        btnInsertTable.Click += new EventHandler(btnInsertTable_Click);
        thirdPanel.Controls.Add(btnInsertTable);

        Panel fourthPanel = GetFourthPanel();
        Panel pnlInfo = PageControls.generateInfoPanel();
        fourthPanel.Controls.Add(pnlInfo);
        extenderInfo = PageControls.generateInfoExtender();
        fourthPanel.Controls.Add(extenderInfo);
        LinkButton btnDummy3 = new LinkButton();
        btnDummy3.ID = PageControls.BTN_DUMMY_INFO;
        fourthPanel.Controls.Add(btnDummy3);

        Panel fifthPanel = GetFifthPanel();
        Panel pnlCopyTable = PageControls.generateCopyPanel();

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
        extenderCopy = PageControls.generateCopyExtender();
        fifthPanel.Controls.Add(extenderCopy);


    }

    protected void btnCopyDone_Click(object sender, EventArgs e) {
        boxCheckAll.Checked = false;
    }

    protected void rdbtnTable_CheckedChanged(object sender, EventArgs e) {
        if (rdbtnTableWithHeaders.Checked) {
            txtCopyTable.Text = TableStringHeaders + TableString;
        } else {
            txtCopyTable.Text = TableString;
        }
        boxCheckAll.Checked = false;
        extenderCopy.Show();
    }

    protected void btnInsertTable_Click(object sender, EventArgs e) {
        string fields = txtInsertTable.Text;
        txtInsertTable.Text = "";
        string[] lines = fields.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++) {
            string[] entries = lines[i].Split(new Char[] { '\t', ';', ',' });
            if (!InsertRecord(entries)) {
                txtInsertTable.Text += lines[i] + "\n";
            }
        }
        if (!txtInsertTable.Text.Equals("")) {
            extenderInfo.Show();
        }
    }

    protected bool InsertRecord(string[] entries) {
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            for (int i = 1; i < Math.Min(FIELDS.Length, entries.Length + 1); i++) {
                cmd.Parameters.AddWithValue(FIELDS[i], entries[i - 1]);
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                wasInserted = true;
                connec.Close();
            } catch { }
        }
        return wasInserted;
    }

    protected abstract Label getLblRow();

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        base.OnInit(e);
    }

    protected abstract Control getButtonDiv();

    protected void InitializeCombos() {
        STANDARD_COMBOS = new bool[STANDARD_FIELDS.Length];
        ADVANCED_COMBOS = new bool[ADVANCED_FIELDS.Length];
        for (int i = 0; i < ADVANCED_FIELDS.Length; i++) {
            ADVANCED_COMBOS[i] = false;
        }
        for (int i = 0; i < STANDARD_FIELDS.Length; i++) {
            STANDARD_COMBOS[i] = false;
        }
    }

    protected void InitializeCheckboxes() {
        STANDARD_CHECKBOXES = new bool[STANDARD_FIELDS.Length];
        ADVANCED_CHECKBOXES = new bool[ADVANCED_FIELDS.Length];
        for (int i = 0; i < ADVANCED_FIELDS.Length; i++) {
            ADVANCED_CHECKBOXES[i] = false;
        }
        for (int i = 0; i < STANDARD_FIELDS.Length; i++) {
            STANDARD_CHECKBOXES[i] = false;
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
        panel.Controls.Add(new LiteralControl("<table>"));
        for (int i = 1; i < TEXT_BOX_IDS2.Length; i++) {
            panel.Controls.Add(new LiteralControl("<tr>"));
            panel.Controls.Add(new LiteralControl("<td>"));
            Label label = new Label();
            label.ID = LABEL_IDS2[i];
            label.CssClass = "lblBold";
            label.Text = HEADERS[i];
            panel.Controls.Add(new LiteralControl("<span class=\"lblBold\">" + HEADERS[i] + "</span>"));
            panel.Controls.Add(new LiteralControl("</td>"));
            panel.Controls.Add(new LiteralControl("<td>"));

            PlaceHolder holderA = new PlaceHolder();
            PlaceHolder holderB = new PlaceHolder();

            holderA.ID = "holder1" + TEXT_BOX_IDS2[i];
            holderB.ID = "holder2" + TEXT_BOX_IDS2[i];

            TextBox txtBox;
            AjaxControlToolkit.ComboBox combo;
            CheckBox checkBox;
            if (COMBOS[i]) {
                combo = new AjaxControlToolkit.ComboBox();
                combo.ID = TEXT_BOX_IDS2[i];
                combo.CssClass = "comboBoxInsideModalPopup";
                combo.AutoPostBack = false;
                combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
                combo.AutoCompleteMode = AjaxControlToolkit.ComboBoxAutoCompleteMode.SuggestAppend;
                List<String> dropList = GetDropList();
                foreach (string item in dropList) {
                    combo.Items.Add(item);
                }
                holderB.Controls.Add(combo);
            } else if (CHECKBOXES[i]) {
                checkBox = new CheckBox();
                checkBox.ID = TEXT_BOX_IDS2[i];
                holderB.Controls.Add(checkBox);
            } else {
                txtBox = new TextBox();
                txtBox.ID = TEXT_BOX_IDS2[i];
                holderA.Controls.Add(txtBox);
            }

            panel.Controls.Add(holderA);
            panel.Controls.Add(holderB);
            //panel.Controls.Add(new LiteralControl("<br />"));
            panel.Controls.Add(new LiteralControl("</td>"));
            panel.Controls.Add(new LiteralControl("</tr>"));
        }

        panel.Controls.Add(new LiteralControl("</table>"));

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


        return panel;
    }

    public void FillPopupLine() {
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + "\\" + MAIN_USER_DATABASE + ";");
        string comm = GetCommandString(Command.SELECT);
        comm = comm.Substring(0, comm.Length - 1);
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
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            for (int i = 1; i < txtControls.Length; i++) {
                string value;
                if (COMBOS[i]) {
                    value = ((AjaxControlToolkit.ComboBox)txtControls[i]).Text;
                } else if (CHECKBOXES[i]) {
                    value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                } else {
                    value = ((TextBox)txtControls[i]).Text;
                }
                cmd.Parameters.AddWithValue(FIELDS[i], value);
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                connec.Close();
            } catch { }
        }
    }

    protected void EditLine() {
        int rowIndex = GetCurrentRowIndex();
        GridViewRow row = grid.Rows[rowIndex];
        string oldField = "";
        if (TABLE_NAME.Equals("tbllabor")) {
            oldField = GetOldField(grid.DataKeys[row.RowIndex][FIELDS[0]].ToString());
        }

        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + "aa\\" + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.UPDATE), connec);


        {
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < FIELDS.Length; i++) {
                Control control = pnlPopupLine.FindControl(TEXT_BOX_IDS2[i]);
                if (COMBOS[i]) {
                    cmd.Parameters.AddWithValue(FIELDS[i], ((AjaxControlToolkit.ComboBox)control).Text);
                } else if (CHECKBOXES[i]) {
                    cmd.Parameters.AddWithValue(FIELDS[i], (((CheckBox)control).Checked) ? "1" : "0");
                } else {
                    cmd.Parameters.AddWithValue(FIELDS[i], ((TextBox)control).Text);
                }
            }
            cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();

                this.SetData();
                connec.Close();
                if (TABLE_NAME.Equals("tbllabor")) {
                    TextBox txtLaborDesc = pnlPopupLine.FindControl(TEXT_BOX_IDS2[1]) as TextBox;
                    SyncTables(txtLaborDesc.Text, oldField);
                }
            } catch { }
        }

    }

    protected void CopyLine() {
        extenderNewName.Show();
    }

    protected void ToAddMode() {
        rdbtnsChoiceModePanel.CssClass = "hidden";
        rdbtnsChoiceModePanel.Visible = false;
        btnAddLine.Visible = true;
        btnOkLine.Visible = false;
        lblAction.Text = "Insert a new set";
        extenderLinePopup.Show();
    }

    protected void ToChoiceMode() {
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

    private void FillDefaultLine(Control container, string[] ids) {
        string dbpath = GetDirectory() + userDir + "\\" + MAIN_USER_DATABASE;
        DAO.Database dat;

        Control[] txtNewRecord = new Control[FIELDS.Length];

        try {
            DAO.DBEngine daoEngine = new DAO.DBEngine();
            dat = daoEngine.OpenDatabase(dbpath, false, false, "");
            DAO.TableDef tableDef = dat.TableDefs[TABLE_NAME];
            for (int i = 1; i < FIELDS.Length; i++) {
                string defaultValue = (string)tableDef.Fields[FIELDS[i]].Properties["DefaultValue"].Value;
                if (defaultValue != null && !defaultValue.Equals("\" \"")) {
                    txtNewRecord[i] = container.FindControl(ids[i]);
                    if (txtNewRecord[i] != null) {
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

    protected void btnOkSort_Click(object sender, EventArgs e) {
        OrderedDictionary sortExpressions = new OrderedDictionary();
        for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
            if (!comboSorts[i].Text.Equals("")) {
                string expression = "";
                for (int j = 1; j < HEADERS.Length; j++) {
                    if (comboSorts[i].Text.Equals(HEADERS[j])) {
                        expression = FIELDS[j];
                        break;
                    }
                }
                if (!expression.Equals("") && !sortExpressions.Contains(expression)) {
                    string direction = (rdbtnSortAsc[i].Checked) ? "ASC" : "DESC";
                    sortExpressions.Add(expression, direction);
                }
            }
        }
        if (sortExpressions.Count > 0) {
            StringBuilder sbSortExpression = new StringBuilder();
            if (sortExpressions.Count > 0) {
                string[] myKeys = new string[sortExpressions.Count];
                sortExpressions.Keys.CopyTo(myKeys, 0);
                for (int i = 0; i < sortExpressions.Count; i++) {
                    sbSortExpression.Append(myKeys[i]);
                    sbSortExpression.Append(" ");
                    sbSortExpression.Append(sortExpressions[myKeys[i]]);
                    if (i != sortExpressions.Count - 1) {
                        sbSortExpression.Append(", ");
                    }
                }
            }
            this.Sort = sbSortExpression.ToString();
            this.SetData();
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



    protected abstract Panel GetThirdPanel();

}
