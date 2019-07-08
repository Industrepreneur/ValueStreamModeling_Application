using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class P_IBOM : InputGridPage
{

    ProductDelegatePage helperProduct;

    public P_IBOM()
    {
        PAGENAME = "/input/products/bom.aspx";
        featureHelper = new IbomDelegate();
        int value = 4;
        helperProduct = new ProductDelegatePage(value);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        //grid.Columns[0].Visible = false;
        grid.Columns[5].Visible = false;
        grid.ShowFooter = true;
        base.Page_Load(sender, e);
        if (!Page.IsPostBack)
        {
          

        }


    }

    private void InitializeComponent()
    {
        pnlMainGrid = gridPanel;


    }

    protected override void SetData()
    {
        {
            ClassE calc = new ClassE(GetDirectory() + userDir);

            calc.setGlobalVar();
            calc.runsql("DELETE * FROM tblbomTree;");
            if (dropListProducts.Items.Count > 0 && dropListProducts.SelectedItem == null)
            {
                dropListProducts.SelectedIndex = 0;
            }
            try
            {

                calc.MakeIbomTree(0, int.Parse(dropListProducts.SelectedValue), 0);
            }
            catch (Exception) { }
            calc.Close();
        }



        if (!checkAllSubComponents.Checked)
        {
            //tableQueryString += " WHERE Show = -1";
            tableQueryString += " WHERE Level = 1";
        }
        else
        {
            tableQueryString += " WHERE Level >= 1";
        }

        base.SetData();
        GridViewRow myRow = grid.FooterRow;
        TextBox myText = (TextBox)myRow.Cells[2].Controls[0].Controls[0];
        myText.Text = "1";
    }



    protected void SetAllowableComponentData()
    {
        {
            ClassE calc = new ClassE(GetDirectory() + userDir);
            calc = new ClassE(GetDirectory() + userDir);
            calc.runsql("DELETE * FROM tblPossibleComp;");
            try
            {
                calc.MakePossibleTable(int.Parse(dropListProducts.SelectedValue));
            }
            catch (Exception) { }
            calc.Close();
        }

        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string commandRouting = "SELECT tblPossibleComp.* from tblPossibleComp ORDER BY ProdDesc;";
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try
            {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataView dv = dt.DefaultView;
                //need to bind this to a droplist

                connec.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch { }
                string message = ex.Message;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        InitializeComponent();

        base.OnInit(e);
        if (!LocalTablesLinked())
        {
            ResetModelGoToModels();
        }
        if (IsWhatifMode())
        {
            Response.Redirect("whatif_" + PAGENAME);
        }

        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;

        srcProductStructure.DataFile = dataFile;
        try
        {
            dropListProducts.DataBind();
        }
        catch (Exception)
        {
            Master.ShowErrorMessage();
        }
        Control buttondiv = getButtonDiv();




        tableSync = new TableSynchronization(userDir);

        //string sheet = "Cheat Sheat IBOM Page";
        //Master.SetHelpSheet(sheet + ".pdf", sheet);

    }

    protected override bool LocalTablesLinked()
    {
        if (!UpdateSql("SELECT ProdID FROM tblProdfore;"))
        {
            return false;
        }
        return true;
    }



    protected override Control getButtonDiv()
    {
        return buttondiv;
    }

    protected override Panel GetSecondPanel()
    {
        return secondPanel;
    }

    protected override Panel GetThirdPanel()
    {
        return thirdPanel;
    }

    protected override Panel GetFourthPanel()
    {
        return fourthPanel;
    }

    protected override Panel GetFifthPanel()
    {
        return fourthPanel;
    }

    protected void dropListProducts_SelectedIndexChanged(object sender, EventArgs e)
    {
        string value = dropListProducts.SelectedValue;
        this.SetData();

    }

    //protected void gridRouting_RowEditing(object sender, GridViewEditEventArgs e)
    //{

    //}
    //protected void gridRouting_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    //{

    //}
    //protected void gridRouting_RowUpdating(object sender, GridViewUpdateEventArgs e)
    //{

    //}

    protected override void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.Footer)
        {

            AjaxControlToolkit.ComboBox ddl = (AjaxControlToolkit.ComboBox)e.Row.Cells[1].Controls[1].Controls[0];

            FillAllowableComponentsDdl(ddl);
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_INSERT) as Button;

            if (btnToClick != null)
            {
              
                for (int columnIndex = 1; columnIndex <
                e.Row.Cells.Count; columnIndex++)
                {

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
        //NEED TO DISABLE IF LEVEL !=1
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Button btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_EDIT) as Button;
            Button btnToDelete = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_DELETE) as Button;
            string tooltip = "Double-click to edit component";
            //IF LEVEL IS >1 DISABLE EDITING AND HIDE BTNS
            Label curLevel = (Label)e.Row.Cells[4].Controls[0].Controls[0];
            Label curParent = (Label)e.Row.Cells[5].Controls[0].Controls[0];

            if (!curLevel.Text.Equals("") & !curParent.Text.Equals(""))
            {
                if (Double.Parse(curLevel.Text) >= 1 & curParent.Text.Equals("True"))
                {
                    e.Row.Cells[1].CssClass += "parent";
                }
                if (Double.Parse(curLevel.Text) > 1)
                {

                    btnToClick.Enabled = false;
                    btnToDelete.Enabled = false;

                    btnToDelete.Parent.Controls[1].Visible = false;
                    btnToClick.Parent.Controls[3].Visible = false;


                }
                else
                {
                    if (((e.Row.RowState & DataControlRowState.Edit) > 0))
                    {
                        btnToClick = e.Row.Cells[0].FindControl(GridViewTemplate.BTN_UPDATE) as Button;
                        tooltip = "Double-click to update component";

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
                            e.Row.Cells[columnIndex].Attributes["ondblclick"] = js;
                            // Add a cursor style to the cells
                            e.Row.Cells[columnIndex].Attributes["style"] +=
                                "cursor:pointer;";
                            e.Row.Cells[columnIndex].ToolTip = tooltip;
                            //NEED TO HANDLE COMBOBOX
                            if (((e.Row.RowState & DataControlRowState.Edit) > 0))
                            {
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
            else
            {


            }

        }

    }

    protected void FillAllowableComponentsDdl(AjaxControlToolkit.ComboBox ddl)
    {
        {
            ClassE calc = new ClassE(GetDirectory() + userDir);
            calc = new ClassE(GetDirectory() + userDir);
            calc.runsql("DELETE tblPossibleComp.* from tblPossibleComp;");
            try
            {
                calc.MakePossibleTable(int.Parse(dropListProducts.SelectedValue));
            }
            catch (Exception) { }
            calc.Close();
        }

        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string commandRouting = "Select tblPossibleComp.* from tblPossibleComp ORDER BY ProdDesc;";
        OleDbCommand cmd = new OleDbCommand(commandRouting, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd); //
        {
            try
            {
                connec.Open();
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                if (ddl != null)
                {
                    ddl.Items.Clear();
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        ListItem item = new ListItem(ds.Rows[i]["ProdDesc"].ToString(), ds.Rows[i]["ProdID"].ToString());
                        ddl.Items.Add(item);
                    }
                }
                connec.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    connec.Close();
                    connec = null;
                }
                catch { }
                string message = ex.Message;
            }
        }
    }

    //protected void gridRouting_RowDeleting(object sender, GridViewDeleteEventArgs e)
    //{

    //}

    protected override void grid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        Button btnSender = e.CommandSource as Button;
        GridViewRow row = btnSender.NamingContainer as GridViewRow;
        //NEED TO HANDLE COMBOBOX?
        if (e.CommandName.Equals("Edit"))
        {

            grid.EditIndex = row.RowIndex;
        }
        //THIS WORKS 02/08/18 SPL (CHECK IF ADVANCED-MODE STOPS THIS FROM WORKING DUE TO ADDED COLUMNS)
        else if (e.CommandName.Equals("Delete"))
        {
            Label curName = (Label)row.Cells[1].Controls[0].Controls[0];
            string dirtyName = curName.Text;
            string cleanName = dirtyName.Remove(0, dirtyName.LastIndexOf(";") + 1);
            OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            //OleDbCommand cmd = new OleDbCommand("DELETE FROM [tblIbom] WHERE tblIbom.compName = " + cleanName + " AND tblIbom.ParentName= " + dropListProducts.SelectedItem.Text, connec);
            OleDbCommand cmd = new OleDbCommand("DELETE FROM [tblIbom] WHERE (compName = @comp) AND (ParentName = @parent)", connec);
            cmd.Parameters.AddWithValue("comp", cleanName);
            cmd.Parameters.AddWithValue("parent", dropListProducts.SelectedItem.Text);

            try
            {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                grid.EditIndex = -1;
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
            }
        }
        //THIS WORKS 02/08/18 SPL (MAKE SURE WORKS WHEN IN ADVANCED MODE)
        else if (e.CommandName.Equals("Insert"))
        {
            TextBox curName = (TextBox)row.Cells[1].Controls[1].Controls[0].Controls[0].Controls[0].Controls[0].Controls[0];
            string cleanName = MyUtilities.clean(curName.Text).Trim();
            TextBox curUPA = (TextBox)row.Cells[2].Controls[0].Controls[0];
            string cleanUPA = MyUtilities.clean(curUPA.Text).Trim();


            try
            {
                connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
                OleDbCommand comm = new OleDbCommand("SELECT ProdID, ProdDesc FROM tblPossibleComp WHERE ProdDesc = '" + cleanName + "';", connec);
                OleDbCommand cmd = new OleDbCommand("INSERT into tblIbom (compName, UPA, ParentName, CompID, flag, ParentID) VALUES ( ?, ?, ?, ?, ?, ?);", connec);
                {
                    try
                    {
                        connec.Open();
                        OleDbDataAdapter adapter = new OleDbDataAdapter(comm);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("compName", cleanName);
                        cmd.Parameters.AddWithValue("UPA", cleanUPA);
                        cmd.Parameters.AddWithValue("ParentName", dropListProducts.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("CompID", dt.Rows[0]["ProdID"]);
                        cmd.Parameters.AddWithValue("flag", 1);
                        cmd.Parameters.AddWithValue("ParentID", dropListProducts.SelectedValue);
                        cmd.Parameters.AddWithValue("ProdDesc", dropListProducts.SelectedItem.Text);

                        int result = cmd.ExecuteNonQuery();
                        grid.EditIndex = -1;
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
                    }
                }
            }
            catch (Exception)
            {
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
        else if (e.CommandName.Equals("CancelUpdate"))
        {
            grid.EditIndex = -1;
        }
        //NEED TO DO THIS
        else if (e.CommandName.Equals("Update"))
        {
            Label curName = (Label)row.Cells[1].Controls[0].Controls[0];
            string dirtyName = curName.Text;
            string cleanName = dirtyName.Remove(0, dirtyName.LastIndexOf(";") + 1);
            var newValues = GetValues(row);
            TextBox curUPA = (TextBox)row.Cells[2].Controls[0].Controls[0];
            string cleanUPA = MyUtilities.clean(curUPA.Text).Trim();
            connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
            //OleDbCommand cmd = new OleDbCommand("UPDATE tblIbom SET UPA = ?  WHERE IbomID = ?;", connec);
            OleDbCommand cmd = new OleDbCommand("UPDATE tblIbom SET UPA = @UPA WHERE (compName = @comp) AND (ParentName = @parent)", connec);


            {
                try
                {
                    connec.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("UPA", cleanUPA);
                    cmd.Parameters.AddWithValue("comp", cleanName);
                    cmd.Parameters.AddWithValue("parent", dropListProducts.SelectedItem.Text);
                    //cmd.Parameters.AddWithValue("IbomID", grid.DataKeys[row.RowIndex]["IbomID"]);

                    int result = cmd.ExecuteNonQuery();
                    grid.EditIndex = -1;
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
                    Master.ShowErrorMessage("An error has occured and the data could not get updated.");
                }
            }
        }

        this.SetData();

    }


    protected void checkAllSubComponents_CheckedChanged(object sender, EventArgs e)
    {
    
        this.SetData();
        
    }

  




}