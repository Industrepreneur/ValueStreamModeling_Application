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
/// Summary description for InputGridPage
/// </summary>
public abstract class InputGridPage : CommonGridPage
{

    protected TextBox txtInsertTable;
    protected Button btnInsertTable;

    protected object[] savedInsertValues;

    protected void FillDefaultInsertRow()
    {
        GridViewRow gridRow = grid.FooterRow;
        if (savedInsertValues != null)
        {
            FillInsertRowWithSavedValues(gridRow, TEXT_BOX_IDS);
        }
        else
        {
            FillDefaultLine(gridRow, TEXT_BOX_IDS);
        }

    }


    protected void FillInsertRowWithSavedValues(Control container, string[] ids)
    {
        Control[] txtNewRecord = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++)
        {
            txtNewRecord[i] = container.FindControl(ids[i]);
            if (txtNewRecord[i] != null)
            {
                if (COMBOS[i])
                {
                    ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text = (string)savedInsertValues[i];
                }
                else if (CHECKBOXES[i])
                {
                    ((CheckBox)txtNewRecord[i]).Checked = (bool)savedInsertValues[i];
                }
                else
                {
                    ((TextBox)txtNewRecord[i]).Text = (string)savedInsertValues[i];
                }

            }

        }

    }

    protected void SaveInsertValues(Control container, string[] ids)
    {
        Control[] txtNewRecord = new Control[FIELDS.Length];
        savedInsertValues = new object[ids.Length];
        for (int i = 1; i < ids.Length; i++)
        {
            txtNewRecord[i] = container.FindControl(ids[i]);
            if (txtNewRecord[i] != null)
            {
                if (COMBOS[i])
                {
                    savedInsertValues[i] = ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text;
                }
                else if (CHECKBOXES[i])
                {
                    savedInsertValues[i] = ((CheckBox)txtNewRecord[i]).Checked;
                }
                else
                {
                    savedInsertValues[i] = ((TextBox)txtNewRecord[i]).Text;
                }

            }
        }
    }

    private void FillDefaultLine(Control container, string[] ids)
    {
        string dbpath = GetDirectory() + userDir + MAIN_USER_DATABASE;
        DAO.Database dat = null;

        Control[] txtNewRecord = new Control[FIELDS.Length];

        try
        {
            DAO.DBEngine daoEngine = new DAO.DBEngine();
            dat = daoEngine.OpenDatabase(dbpath, false, false, "");
            DAO.TableDef tableDef = dat.TableDefs[TABLE_NAME];
            for (int i = 1; i < FIELDS.Length; i++)
            {
                if (!FIELDS[i].Equals("opnam1") && !FIELDS[i].Equals("opnam2"))
                { // aliases which are not in the database
                    string defaultValue = (string)tableDef.Fields[FIELDS[i]].Properties["DefaultValue"].Value;
                    if (defaultValue != null && !defaultValue.Equals("\" \"") && !defaultValue.Equals(""))
                    {
                        txtNewRecord[i] = container.FindControl(ids[i]);
                        if (txtNewRecord[i] != null)
                        {
                            defaultValue = MyUtilities.clean(defaultValue, '"');
                            if (COMBOS[i])
                            {
                                ((AjaxControlToolkit.ComboBox)txtNewRecord[i]).Text = defaultValue;
                            }
                            else if (CHECKBOXES[i])
                            {
                                ((CheckBox)txtNewRecord[i]).Checked = defaultValue.ToString().Equals("-1");
                            }
                            else if (!((TextBox)txtNewRecord[i]).Enabled){
                                defaultValue = null;
            }
                            else
                            {
                                ((TextBox)txtNewRecord[i]).Text = defaultValue;
                            }

                        }
                    }
                }
            }


            dat.Close();
        }
        catch (Exception)
        {
            try
            {
                dat.Close();
            }
            catch (Exception) { }
        }
    }

    protected void GoToEditMode(int id)
    {
        bool found = false;
        int index = 0;
        int pageIndex = 0;
        grid.PageIndex = pageIndex;
        this.SetData();
        for (int i = 0; i < grid.PageCount; i++)
        {
            for (index = 0; index < grid.DataKeys.Count; index++)
            {
                if (Convert.ToInt32(grid.DataKeys[index].Value.ToString()) == id)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                break;
            }

            pageIndex++;
            grid.PageIndex = pageIndex;
            this.SetData();
        }

        if (found)
        {
            grid.PageIndex = pageIndex;
            grid.EditIndex = index;

        }
        else
        {
            grid.EditIndex = -1;
            grid.PageIndex = 0;
        }
        this.SetData();
        index = grid.EditIndex;
        if (index >= 0)
        {
            try
            {
                Control control = grid.Rows[index].FindControl(TEXT_BOX_IDS[1]);
                control.Focus();
            }
            catch (Exception) { }
        }
    }

    protected void ShowMultipleInsertWarning()
    {
        Master.SetFocus(btnInsertMultipleWarningOk.ClientID);
        extenderInfo.Show();
    }

    protected virtual string GetId(string desc)
    {
        return GetDatabaseField(FIELDS[0], FIELDS[1], desc, TABLE_NAME);
    }

    protected virtual void Copy(int rowIndex)
    {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);

        GridViewRow insertRow = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++)
        {
            txtControls[i] = insertRow.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
        }
        int j = 1;
        string origName = newValues[FIELDS[j]].ToString();
        string copyName = origName + "_COPY";
        if (FIELDS[j].ToLower().Equals("labordesc"))
        {
            copyName = GetUniqueName("LaborDesc", "LaborID", "tbllabor", copyName);
        }
        else if (FIELDS[j].ToLower().Equals("equipdesc"))
        {
            copyName = GetUniqueName("EquipDesc", "EquipID", "tblequip", copyName);
        }
        else if (FIELDS[j].ToLower().Equals("proddesc"))
        {
            copyName = GetUniqueName("ProdDesc", "ProdID", "tblprodfore", copyName);
        }
        else if (FIELDS[j].ToLower().Equals("opnam"))
        {
            copyName = GetUniqueName("Opnam", "OpID", "tbloper", copyName);  // gwwd
        }
        if (COMBOS[j])
        {
            ((AjaxControlToolkit.ComboBox)txtControls[j]).Text = copyName;
        }
        else
        {
            ((TextBox)txtControls[j]).Text = copyName;
        }

        for (int i = 2; i < FIELDS.Length; i++)
        {

            if (COMBOS[i])
            {
                ((AjaxControlToolkit.ComboBox)txtControls[i]).Text = newValues[FIELDS[i]].ToString();
            }
            else if (CHECKBOXES[i])
            {
                ((CheckBox)txtControls[i]).Checked = (bool)newValues[FIELDS[i]];
            }
            else
            {
                string value = newValues[FIELDS[i]].ToString();
                ((TextBox)txtControls[i]).Text = newValues[FIELDS[i]].ToString();
            }
        }
        txtControls[1].Focus();
    }

    protected override void SetData()
    {
        base.SetData();
        //if (!Page.IsPostBack) {
        FillDefaultInsertRow();
        //}
    }

    protected override void grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        Button btn = e.CommandSource as Button;
        if (btn == null)
        {
            return;
        }
        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        try
        {
            Master.HideLoadingPopup();
        }
        catch (Exception) { }
        if (e.CommandName.Equals("Update"))
        {
            RowUpdate(rowIndex);
            SyncTables();
            SetModelModified(true, false);
        }//NOTE SURE IF THIS IS OK, OVERRIDES COMMONGRIDPAGE
        else if (e.CommandName.Equals("Insert"))
        {
            InsertRowSetData();
            SyncTables();
            SetModelModified(true, true);
        }
        else if (e.CommandName.Equals("Delete"))
        {
            DeleteRow(rowIndex);
            SetModelModified(true, true);
        }
        else if (e.CommandName.Equals("Copy"))
        {
            Copy(rowIndex);
            InsertRow(true);
            SetModelModified(true, true);
        }
        else
        {
            base.grid_RowCommand(sender, e);
        }
    }

    protected override void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        base.Grid_RowDataBound(sender, e);
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            //if (!PAGENAME.Equals("products_ibom.aspx") && !PAGENAME.Equals("whatif_products_ibom.aspx"))
            if (true)
            {
                Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_INSERT) as Button;
                string tooltip = "Double-click to add ";
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
                        // Add the column index as the event argument parameter
                        string js = btnJavascript.Insert(btnJavascript.Length - 2,
                            columnIndex.ToString());
                        // Add this javascript to the onclick Attribute of the cell
                        //e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                        // Add a cursor style to the cells
                        e.Row.Cells[columnIndex].Attributes["style"] +=
                            "cursor:pointer;";
                        ////e.Row.Cells[columnIndex].ToolTip = tooltip;

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

                }
            }
        }

    }

    protected virtual void InsertRowSetData()
    {
        InsertRow();
        SetData();

    }

    protected virtual void InsertRow(bool goToEdit)
    {
        string desc = "";
        GridViewRow row = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        for (int i = 1; i < FIELDS.Length; i++)
        {
            txtControls[i] = row.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        //string command = GetCommandString(Command.INSERT, selectedFields.ToArray());
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            for (int i = 1; i < txtControls.Length; i++)
            {
                string value;
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i])
                {
                    if (COMBOS[i])
                    {
                        if (((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedItem != null)
                        {
                            value = ((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedValue;
                        }
                        else
                        {
                            value = MyUtilities.clean(((AjaxControlToolkit.ComboBox)txtControls[i]).Text);
                        }
                    }
                    else if (CHECKBOXES[i])
                    {
                        value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                    }
                    else
                    {
                        value = MyUtilities.clean(((TextBox)txtControls[i]).Text);
                        if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam"))
                        {
                            value = value.ToUpper();
                        }
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    if (i == 1)
                    {
                        desc = value;
                    }
                }
            }
            try
            {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                connec.Close();
                if (goToEdit)
                {
                    try
                    {
                        SetData();
                        int id = int.Parse(GetId(desc));
                        GoToEditMode(id);
                    }
                    catch (Exception) { }
                }

            }
            catch (Exception ex)
            {
                //logFiles.ErrorLog(ex);
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch (Exception) { }
                SaveInsertValues(grid.FooterRow, TEXT_BOX_IDS);
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    protected void InsertRow()
    {
        InsertRow(false);

    }

    protected override void InitializeComponent()
    {
        base.InitializeComponent();
        txtInsertTable = new TextBox();
        //txtInsertTable.Attributes.Add("style", "display:none");
        txtInsertTable.TextMode = TextBoxMode.MultiLine;
        txtInsertTable.ID = TXT_INSERT_TABLE;
        thirdPanel.Controls.Add(txtInsertTable);


        btnInsertTable = new Button();
        btnInsertTable.Text = "Insert";
        btnInsertTable.CssClass = "otherButton";
        btnInsertTable.Click += new EventHandler(btnInsertTable_Click);
        thirdPanel.Controls.Add(btnInsertTable);

        // panel insert multiple warning
        Panel fourthPanel = GetFourthPanel();
        Panel pnlInfo = PageControls.generateInfoPanel();
        fourthPanel.Controls.Add(pnlInfo);
        btnInsertMultipleWarningOk = pnlInfo.FindControl(PageControls.BTN_INSERT_MULTIPLE_WARNING_OK) as Button;
        extenderInfo = PageControls.generateInfoExtender();
        fourthPanel.Controls.Add(extenderInfo);
        LinkButton btnDummy3 = new LinkButton();
        btnDummy3.ID = PageControls.BTN_DUMMY_INFO;
        fourthPanel.Controls.Add(btnDummy3);
    }

    protected virtual void btnInsertTable_Click(object sender, EventArgs e)
    {
        string fields = txtInsertTable.Text;
        txtInsertTable.Text = "";
        string[] lines = fields.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] entries = lines[i].Split(new Char[] { '\t', ';', ',' });
            if (!InsertRecord(entries))
            {
                txtInsertTable.Text += lines[i] + "\n";
            }
            else
            {
                SetModelModified(true, true);
            }
        }
        SyncTables();
        if (!txtInsertTable.Text.Equals(""))
        {
            ShowMultipleInsertWarning();
        }
    }

    protected virtual bool InsertRecord(string[] entries)
    {
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            int i = 1;
            for (int j = 0; j < entries.Length && i < FIELDS.Length; j++)
            {
                if (mode.Equals("Standard"))
                {
                    while (i < FIELDS.Length && ADVANCED_FIELDS[i])
                    {
                        i++;
                    }
                }
                if (i < FIELDS.Length)
                {
                    string value = MyUtilities.clean(entries[j]);
                    if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam"))
                    {
                        value = value.ToUpper();
                    }
                    if (CHECKBOXES[i])
                    {
                        value = value.ToLower().Equals("true") ? "1" : "0";
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    i++;
                }
            }
            try
            {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                this.SetData();
                wasInserted = true;
                connec.Close();
            }
            catch
            {
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch { }
                FillDefaultInsertRow();
            }
        }
        return wasInserted;
    }

    protected void DeleteRow(int rowIndex)
    {
        GridViewRow row = grid.Rows[rowIndex];
        var newValues = this.GetValues(row);
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(GetCommandString(Command.DELETE), connec);

        cmd.Parameters.AddWithValue(FIELDS[0], grid.DataKeys[row.RowIndex][FIELDS[0]]);

        int key = int.Parse(grid.DataKeys[row.RowIndex][FIELDS[0]].ToString());

        try
        {

            int prodid = -1;
            string opname = "";
            bool issueRoutingWarning = false;
            ClassF classA = null;
            if (TABLE_NAME.Equals("tbloper"))
            {
                try
                {
                    prodid = int.Parse(GetDatabaseField("Prodfore", "OpID", key, "tblOper"));
                    opname = GetDatabaseField("OpNam", "OpID", key, "tblOper");
                    try
                    {
                        classA = new ClassF(GetDirectory() + userDir);
                        issueRoutingWarning = classA.repair_routings(key, opname, prodid);
                        classA.Close();
                    }
                    catch (Exception) { }
                }
                catch (Exception) { }
            }
            connec.Open();
            try
            {
                int result = cmd.ExecuteNonQuery();
            }
            catch (Exception excep)
            {
                Exception excepNew = new Exception("Error in executing delete query. Command string: " + cmd.CommandText + ". Exception message: " + excep.Message, excep);
                throw excepNew;
            }
            connec.Close();

            grid.EditIndex = -1;
            this.SetData();

            if (classA == null)
            {
                classA = new ClassF(GetDirectory() + userDir);
            }
            else
            {
                classA.Open();
            }

            if (TABLE_NAME.Equals("tbllabor"))
            {
                classA.del_labor_res(key);
                classA.del_lab_ref(key);
            }
            else if (TABLE_NAME.Equals("tblequip"))
            {
                classA.del_eq_res(key);
                classA.del_eq_ref(key);
            }
            else if (TABLE_NAME.Equals("tblprodfore"))
            {
                classA.del_pt_res(key);
            }
            else if (TABLE_NAME.Equals("tbloper"))
            {
                // delete oper results ??? ask Greg
                classA.del_op_res(key);

            }
            classA.Close();
            if (TABLE_NAME.Equals("tbloper"))
            {
                tableSync.UpdateOpNumbers();
            }

            if (issueRoutingWarning)
            {
                Master.ShowInfoMessage("Routings for the operation were deleted. Please check out '" + opname + "' routing deleted does not leave a hole in the routings.");
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
            Master.ShowErrorMessage("An error has occured and the record could not get deleted.");
        }
    }


}