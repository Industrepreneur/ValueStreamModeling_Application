using System;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;

public class GridViewTemplate : System.Web.UI.Page, ITemplate
{
    ListItemType templateType;
    int columnType;
    string bindExpression;
    string controlID;
    short columnNum;
    string headerTooltip;
    string headerText;

    public const int BUTTONS = 0;
    public const int DATA = 1;
    public const int COMBODATA = 2;
    public const int EMPTY_TABLE = 3;
    public const int CHECKBOX_DATA = 4;
    public const int NONEDITABLE_DATA = 5;
    public const int COMBO_NONEDITABLE_DATA = 6;

    public const string BTN_SELECT = "btnSelect";
    public const string BTN_UPDATE = "btnUpdate";
    public const string BTN_CANC = "btnCanc";
    public const string BTN_INSERT = "btnInsert";
    public const string BTN_ADD_NEW = "btnAddNew";
    public const string BTN_COPY = "btnCopyGridLine";
    public const string BTN_EDIT = "btnEditGridLine";
    public const string BTN_DELETE = "btnDeleteGridLine";
    public const string BTN_EDIT_ROUTING = "btnEdit";

    public OleDbConnection oleConn;

    protected List<string> dropList;
    protected ListItemCollection dropdownList;

    public bool newer = false;
    public bool whatif = false;
    public bool wantCopyButton = true;
    public bool analysisGrid = false;

    public GridViewTemplate(ListItemType type, string bindExpression)
    {
        templateType = type;
        this.bindExpression = bindExpression;
    }

    public GridViewTemplate(int columnType)
    {
        this.columnType = columnType;
        this.templateType = ListItemType.Item;
    }

    public GridViewTemplate(ListItemType type, int columnType, string bindExpression, string controlID, List<string> dropList)
        : this(type, columnType, bindExpression, controlID)
    {
        this.dropList = dropList;
    }

    public GridViewTemplate(ListItemType type, int columnType, string bindExpression, string controlID, ListItemCollection dropList)
        : this(type, columnType, bindExpression, controlID)
    {
        this.dropdownList = dropList;
    }

    public GridViewTemplate(ListItemType type, int columnType)
    {
        this.templateType = type;
        this.columnType = columnType;
    }

    public GridViewTemplate(string headerText, string headerTooltip, string controlID)
    {
        this.templateType = ListItemType.Header;
        this.controlID = controlID;
        this.headerText = headerText;
        this.headerTooltip = headerTooltip;
    }



    public GridViewTemplate(ListItemType type, int columnType, string bindExpression, string controlID)
    {
        templateType = type;
        this.columnType = columnType;
        this.bindExpression = bindExpression;
        this.controlID = controlID;
        if (controlID != null)
        {
            int numIndex = controlID.IndexOf(GridPage.BASE_CONTROL_NAME) + GridPage.BASE_CONTROL_NAME.Length;
            try
            {
                columnNum = short.Parse(controlID.Substring(numIndex));
            }
            catch { };
        }

    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        switch (templateType)
        {
            case ListItemType.Header:
                Label lbl = new Label();
                lbl.ID = controlID;
                lbl.Text = headerText;
                lbl.ToolTip = headerTooltip;
                container.Controls.Add(lbl);
                break;
            case ListItemType.Item:
                PlaceHolder holderI1 = new PlaceHolder();
                holderI1.ID = "holderI1";
                PlaceHolder holderI2 = new PlaceHolder();
                holderI2.ID = "holderI2";
                holderI1.ID += controlID;
                holderI2.ID += controlID;
                if (columnType == DATA)
                {
                    Label lblItem = new Label();
                    lblItem.ID = controlID;
                    //lblItem.Width = 100;
                    //if (BTN_UPDATE !=null && bindExpression.Equals("tonum") || bindExpression.Equals("fromnum"))
                    //{
                    //    lblItem.Enabled = false;
                    //    lblItem.Attributes["style"] += "cursor:default;";

                    //}


                    lblItem.DataBinding += new EventHandler(this.lblItem_DataBinding);

                    container.Controls.Add(holderI1);
                    container.Controls.Add(holderI2);
                    holderI1.Controls.Add(lblItem);
                }
                else if (columnType == BUTTONS)
                {

                    HtmlGenericControl btnButton = new HtmlGenericControl();
                    btnButton.TagName = "input";
                    btnButton.Attributes.Add("id", "btnInput");
                    btnButton.Attributes.Add("value", "Select");
                    btnButton.Attributes.Add("type", "button");
                    btnButton.DataBinding += new EventHandler(btnButton_DataBinding);

                    container.Controls.Add(btnButton);
                    btnButton.Visible = false;

                    Label lblEdit = new Label();
                    lblEdit.Text = "<i class='fas fa-edit row-icon'></i>";
                    lblEdit.CssClass = "icon-button";
                    lblEdit.AssociatedControlID = BTN_EDIT;
                    container.Controls.Add(lblEdit);

                    Button btnEdit = new Button();
                    btnEdit.CssClass = "row-button";
                    btnEdit.ID = BTN_EDIT;
                    btnEdit.Text = "Edit";
                    btnEdit.CommandName = "Edit";
                    container.Controls.Add(btnEdit);

                    if (!whatif)
                    {
                        //if (wantCopyButton)
                        //{
                        //    Label lblCopy = new Label();
                        //    lblCopy.Text = "<i class='fa fa-clone row-icon'></i>";
                        //    lblCopy.CssClass = "icon-button";
                        //    lblCopy.AssociatedControlID = BTN_COPY;
                        //    container.Controls.Add(lblCopy);

                        //    Button btnCopy = new Button();
                        //    btnCopy.CssClass = "row-button";
                        //    btnCopy.ID = BTN_COPY;
                        //    btnCopy.Text = "Copy";
                        //    btnCopy.CommandName = "Copy";
                        //    container.Controls.Add(btnCopy);
                        //}

                        //HtmlGenericControl btnDelete = new HtmlGenericControl();
                        //btnDelete.TagName = "button";
                        //btnDelete.InnerHtml = "<i class='fa fa-cloud'></i>";
                        //btnDelete.ID = BTN_DELETE;
                        //btnDelete.CommandName = "Delete";

                        //THIS WORKS BUT NEED TO FIGURE OUT HOW TO ASSIGN SAME DYNAMIC ID FROM BTN
                        //Try assigning JS event to button?
                        //HtmlGenericControl lblDelete = new HtmlGenericControl();
                        //lblDelete.TagName = "label";
                        //lblDelete.Attributes.Add("for", controlID);
                        //lblDelete.InnerHtml = "<i class='fa fa-cloud'></i>";
                        //container.Controls.Add(lblDelete);

                        Label lblDelete = new Label();
                        lblDelete.Text = "<i class='fas fa-trash row-icon'></i>";
                        lblDelete.CssClass = "icon-delete";
                        lblDelete.AssociatedControlID = BTN_DELETE;
                        container.Controls.Add(lblDelete);

                        Button btnDelete = new Button();
                        btnDelete.CssClass = "row-button";
                        btnDelete.ID = BTN_DELETE;
                        btnDelete.Text = analysisGrid ? "Remove from Vision" : "Delete";
                        btnDelete.CommandName = "Delete";
                        if (!analysisGrid)
                        {
                            btnDelete.OnClientClick = "return confirm('Are you sure you want to delete the selected record?');";
                        }
                        container.Controls.Add(btnDelete);
                    }


                }
                else if (columnType == EMPTY_TABLE)
                {
                    Button btnAddNew = new Button();
                    btnAddNew.CssClass = "row-button";
                    btnAddNew.Text = "Add New";
                    btnAddNew.CommandName = "insertInEmpty";
                    container.Controls.Add(btnAddNew);

                }
                else if (columnType == CHECKBOX_DATA)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = controlID;
                    //checkBox.Enabled = false;
                    checkBox.Attributes.Add("onclick", "return false;");
                    checkBox.DataBinding += new EventHandler(checkboxItem_DataBinding);
                    container.Controls.Add(holderI1);
                    container.Controls.Add(holderI2);
                    holderI2.Controls.Add(checkBox);
                }
                break;
            case ListItemType.EditItem:
                PlaceHolder holder1 = new PlaceHolder();
                holder1.ID = "holder1";
                PlaceHolder holder2 = new PlaceHolder();
                holder2.ID = "holder2";
                if (columnType == DATA)
                {
                    TextBox txtItem = new TextBox();
                    txtItem.ID = controlID;
                    //txtItem.Attributes.Add("onchange", "setCursorToEnd(this)");
                    //txtItem.Attributes.Add("width", "100%");
                    //txtItem.Width = 100;
                    txtItem.DataBinding += new EventHandler(this.txtItem_DataBinding);
                    if (bindExpression.Equals("GrpSiz") && controlID.Equals("txtEdit4"))
                    {
                        //txtItem.AutoPostBack = true;
                        txtItem.TextChanged += GrpSiz_Changed;
                    }
                    else if (bindExpression.Equals("tonum") || bindExpression.Equals("fromnum") || bindExpression.Equals("UPF") || bindExpression.Equals("level"))
                    {

                        txtItem.Enabled = false;
                        txtItem.Attributes["style"] += "cursor:default;";
                        txtItem.Attributes["style"] += "background-color: #d4d4d4 !important;";
                        txtItem.Attributes["style"] += "text-align:center;";


                    }

                    //else
                    //{
                    //    txtItem.Attributes.Add("onkeydown", "return (event.keyCode=!13);");
                    //}
                    //if (bindExpression.Equals("OpNum")) {
                    //    txtItem.Width = 75;
                    //} else if (bindExpression.Equals("PercentAssign")) {
                    //    txtItem.Width = 90;
                    //} else if (bindExpression.Equals("MainVarFrom") || bindExpression.Equals("MainVarTo") || bindExpression.Equals("MainVarStep")) {
                    //    txtItem.Width = 70;
                    //}

                    holder1.ID += controlID;
                    holder2.ID += controlID;
                    container.Controls.Add(holder1);
                    container.Controls.Add(holder2);
                    holder1.Controls.Add(txtItem);
                }
                else if (columnType == BUTTONS)
                {
                    Label lblUpdate = new Label();
                    lblUpdate.Text = "<i class='far fa-save row-icon'></i>";
                    lblUpdate.CssClass = "icon-update";
                    lblUpdate.AssociatedControlID = BTN_UPDATE;
                    container.Controls.Add(lblUpdate);

                    Button btnUpdate = new Button();
                    //btnUpdate.CssClass = "updateButton";
                    btnUpdate.CssClass = "row-button";
                    btnUpdate.ID = BTN_UPDATE;
                    btnUpdate.Text = "Update";
                    btnUpdate.CommandName = "Update";
                    container.Controls.Add(btnUpdate);

                    Label lblCanc = new Label();
                    lblCanc.Text = "<i class='fas fa-ban row-icon'></i>";
                    lblCanc.CssClass = "icon-delete";
                    lblCanc.AssociatedControlID = BTN_CANC;
                    container.Controls.Add(lblCanc);

                    Button btnCanc = new Button();
                    //btnCanc.CssClass = "otherButton";
                    btnCanc.CssClass = "row-button";
                    btnCanc.ID = BTN_CANC;
                    btnCanc.Text = "Cancel";
                    btnCanc.CommandName = "CancelUpdate";
                    container.Controls.Add(btnCanc);

                }
                else if (columnType == COMBODATA)
                {
                    //if (bindExpression.Equals("compName"))
                    //{
                    //    Label myLabel = new Label();
                    //    myLabel.ID = controlID;
                    //    myLabel.DataBinding += new EventHandler(this.lblItem_DataBinding);
                    //    holder2.Controls.Add(myLabel);
                    //}
                    //else

                    AjaxControlToolkit.ComboBox combo = new AjaxControlToolkit.ComboBox();
                    combo.ID = controlID;
                    combo.CaseSensitive = false;
                    if (bindExpression.Equals("EquipTypeName"))
                    {
                        combo.AutoPostBack = true;
                        combo.TextChanged += EquipType_Changed;

                    }
                    else
                    {
                        combo.AutoPostBack = false;
                    }
                    if (bindExpression.Equals("OpNam") && controlID.Equals("comboEdit3"))
                    {
                        combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDown;
                    }
                    else
                    {
                        combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
                    }
                    combo.AutoCompleteMode = AjaxControlToolkit.ComboBoxAutoCompleteMode.SuggestAppend;
                    combo.RenderMode = AjaxControlToolkit.ComboBoxRenderMode.Block;
                    combo.CssClass = "ajaxCombo";
                    if (newer)
                    {
                        combo.DataValueField = dropList[0];
                        if (dropList.Count > 1)
                        {
                            combo.DataTextField = dropList[1];
                        }
                    }
                    else if (dropdownList != null)
                    {
                        if (dropdownList.Count > 0)
                        {
                            combo.DataValueField = dropdownList[0].Value;
                            combo.DataTextField = dropdownList[1].Text;
                            for (int i = 1; i < dropdownList.Count; i++)
                            {
                                combo.Items.Add(dropdownList[i]);
                            }
                        }
                    }
                    else
                    {
                        combo.Items.Clear();
                        foreach (string item in dropList)
                        {
                            combo.Items.Add(item);
                        }
                    }
                    combo.DataBinding += new EventHandler(combo_DataBinding);
                    holder2.Controls.Add(combo);


                    holder1.ID += controlID;
                    holder2.ID += controlID;
                    container.Controls.Add(holder1);
                    container.Controls.Add(holder2);



                }
                else if (columnType == CHECKBOX_DATA)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = controlID;
                    checkBox.Enabled = true;
                    checkBox.DataBinding += new EventHandler(checkboxItem_DataBinding);
                    holder1.ID += controlID;
                    holder2.ID += controlID;
                    container.Controls.Add(holder1);
                    container.Controls.Add(holder2);
                    holder2.Controls.Add(checkBox);

                }
                //else if (columnType == NONEDITABLE_DATA)
                //{
                //    holder1.ID += controlID;
                //    holder2.ID += controlID;
                //    Label lblItem = new Label();
                //    lblItem.ID = controlID;

                //    lblItem.DataBinding += new EventHandler(this.lblItem_DataBinding);

                //    lblItem.Attributes["style"] += "cursor:default";
                //    lblItem.Enabled = false;

                //    //lblItem.Width = 100;

                //    //if (!bindExpression.Equals("tonum") && !bindExpression.Equals("fromnum")) {
                //    //    lblItem.DataBinding += new EventHandler(this.lblItem_DataBinding);
                //    //    lblItem.Enabled = false;
                //    //}

                //    container.Controls.Add(holder1);
                //    container.Controls.Add(holder2);
                //    holder1.Controls.Add(lblItem);
                //}
                break;
            case ListItemType.Footer:
                if (container is TableCell && columnType != BUTTONS)
                {
                    (container as TableCell).HorizontalAlign = HorizontalAlign.Center;
                }
                PlaceHolder holderA = new PlaceHolder();
                holderA.ID = "holderA";
                PlaceHolder holderB = new PlaceHolder();
                holderB.ID = "holderB";
                if (columnType == DATA)
                {
                    TextBox txtInsert = new TextBox();
                    txtInsert.ID = controlID;
                    txtInsert.Attributes.Add("width", "96%");
                    if (bindExpression.Equals("GrpSiz") && controlID.Equals("txtEdit4"))
                    {
                        txtInsert.AutoPostBack = true;
                        txtInsert.TextChanged += GrpSiz_Changed;
                    }
                    else
                    {
                        //txtInsert.Attributes.Add("onkeydown", "return (event.keyCode=!13);");
                    }
                    if (bindExpression.Equals("OpNum"))
                    {
                        //txtInsert.Width = 75;
                    }
                    else if (bindExpression.Equals("PercentAssign"))
                    {
                        //txtInsert.Width = 90;
                    }
                    //if (bindExpression.Equals("tonum") || bindExpression.Equals("fromnum") || bindExpression.Equals("UPA"))
                    if (bindExpression.Equals("tonum") || bindExpression.Equals("fromnum") || bindExpression.Equals("UPF") || bindExpression.Equals("level"))
                    {

                        txtInsert.Enabled = false;
                        txtInsert.Attributes["style"] += "cursor:default;";
                        txtInsert.Attributes["style"] += "background-color: #d4d4d4 !important;";
                        txtInsert.Attributes["value"] = null;
                    }


                    txtInsert.TabIndex = columnNum;
                    //txtInsert.EnableViewState = false;
                    holderA.ID += controlID;
                    holderB.ID += controlID;
                    container.Controls.Add(holderA);
                    container.Controls.Add(holderB);
                    holderA.Controls.Add(txtInsert);



                }
                else if (columnType == BUTTONS)
                {
                    Label lblAdd = new Label();
                    lblAdd.Text = "<i class='fas fa-plus icon-add'></i>";
                    lblAdd.CssClass = "row-add";
                    lblAdd.ToolTip = "add new row";
                    lblAdd.AssociatedControlID = BTN_INSERT;
                    container.Controls.Add(lblAdd);

                    Button btnInsert = new Button();
                    btnInsert.CssClass = "row-button";
                    btnInsert.ID = BTN_INSERT;
                    btnInsert.Text = "Add";
                    btnInsert.UseSubmitBehavior = true;


                    btnInsert.CommandName = "Insert";
                    container.Controls.Add(btnInsert);
                }
                else if (columnType == COMBODATA)
                {

                    AjaxControlToolkit.ComboBox combo = new AjaxControlToolkit.ComboBox();
                    combo.ID = controlID;
                    combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDown;
                    combo.AutoCompleteMode = AjaxControlToolkit.ComboBoxAutoCompleteMode.SuggestAppend;
                    combo.RenderMode = AjaxControlToolkit.ComboBoxRenderMode.Block;
                    combo.CssClass = "ajaxCombo";
                    //combo.DataBinding += new EventHandler(this.combo_DataBinding);
                    if (bindExpression.Equals("EquipTypeName"))
                    {
                        combo.AutoPostBack = true;
                        combo.SelectedIndexChanged += EquipType_Changed;
                    }
                    else if (bindExpression.Equals("ProdDesc") && controlID.Equals("comboEdit1"))
                    {
                        combo.AutoPostBack = true;
                        combo.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
                        combo.SelectedIndexChanged += SelectedProduct_Changed;

                    }
                    if (newer)
                    {
                        combo.DataValueField = dropList[0];
                        if (dropList.Count > 1)
                        {
                            combo.DataTextField = dropList[1];
                        }
                    }
                    else if (dropdownList != null)
                    {
                        if (dropdownList.Count > 0)
                        {
                            combo.DataValueField = dropdownList[0].Value;
                            combo.DataTextField = dropdownList[1].Text;
                            for (int i = 1; i < dropdownList.Count; i++)
                            {
                                combo.Items.Add(dropdownList[i]);
                            }
                        }
                    }
                    else
                    {
                        foreach (string item in dropList)
                        {
                            combo.Items.Add(item);
                        }
                        if (bindExpression.Equals("ProdDesc") && controlID.Equals("comboEdit1"))
                        {
                            combo.Items.Add("");
                        }
                    }
                    holderA.ID += controlID;
                    holderB.ID += controlID;
                    container.Controls.Add(holderA);
                    container.Controls.Add(holderB);
                    holderB.Controls.Add(combo);
                }
                else if (columnType == CHECKBOX_DATA)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = controlID;
                    checkBox.Enabled = true;
                    holderA.ID += controlID;
                    holderB.ID += controlID;
                    container.Controls.Add(holderA);
                    container.Controls.Add(holderB);
                    holderB.Controls.Add(checkBox);
                }
                else if (columnType == NONEDITABLE_DATA)
                {
                    holderA.ID += controlID;
                    holderB.ID += controlID;
                    Label lblItem = new Label();
                    lblItem.ID = controlID;

                    //lblItem.Attributes["style"] += "cursor:default";

                    //lblItem.Width = 100;

                    //if (!bindExpression.Equals("tonum") && !bindExpression.Equals("fromnum")) {
                    //    lblItem.DataBinding += new EventHandler(this.lblItem_DataBinding);
                    //    lblItem.Enabled = false;
                    //}

                    container.Controls.Add(holderA);
                    container.Controls.Add(holderB);
                    holderA.Controls.Add(lblItem);
                }
                break;

        }
    }

    private void lblItem_DataBinding(Object sender, EventArgs e)
    {
        Label lc = (Label)sender;
        GridViewRow row = (GridViewRow)lc.NamingContainer;
        string propertyValue = DataBinder.Eval(row.DataItem, bindExpression).ToString();
        string text = propertyValue.Replace(" ", "&nbsp;");
        lc.Text = text;


    }



    private void btnButton_DataBinding(Object sender, EventArgs e)
    {
        HtmlGenericControl btnButton = (HtmlGenericControl)sender;
        GridViewRow row = (GridViewRow)btnButton.NamingContainer;
        string property = DataBinder.Eval(row.DataItem, bindExpression).ToString();
        btnButton.Attributes.Add("onclick", "popupClick('" + controlID + "' , '" + property + "');");
    }

    private void checkboxItem_DataBinding(Object sender, EventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        GridViewRow row = (GridViewRow)checkBox.NamingContainer;
        checkBox.Checked = DataBinder.Eval(row.DataItem, bindExpression).ToString().Equals("True");

    }

    private void comboFill(AjaxControlToolkit.ComboBox combo)
    {
        /*string command = "SELECT LaborDesc FROM tbllabor;";
        OleDbCommand oleComm = new OleDbCommand(command, oleConn);
        OleDbDataAdapter oleAdapter = new OleDbDataAdapter();
        DataTable dt = new DataTable();
        oleAdapter.Fill(dt);
        combo.DataSource = dt;
        combo.DataBind();*/
        List<string> list = new List<string>();
        ListItem item = new ListItem("apples");
        ListItem item2 = new ListItem("pears");
        ArrayList myList = new ArrayList();

        myList.Add(new ListItem("apples", "1"));
        myList.Add(new ListItem("pears", "2"));
        myList.Add(new ListItem("bananas", "3"));
        foreach (ListItem it in myList)
        {
            combo.Items.Add(it);
        }
        //string propertyValue = DataBinder.Eval(row.DataItem, bindExpression).ToString();
        //combo.Text = propertyValue;
        ;
    }

    private void combo_DataBinding(object sender, EventArgs e)
    {
        AjaxControlToolkit.ComboBox combo = sender as AjaxControlToolkit.ComboBox;
        GridViewRow row = (GridViewRow)combo.NamingContainer;
        if (row.DataItem != null && combo.Items.Count > 0)
        {
            string propertyValue = DataBinder.Eval(row.DataItem, bindExpression).ToString();
            if (combo.Items.FindByText(propertyValue) == null)
            {
                combo.Items.Add(propertyValue);
            }
            combo.Text = propertyValue;
        }

    }



    private void txtItem_DataBinding(object sender, EventArgs e)
    {
        TextBox lc = (TextBox)sender;
        GridViewRow row = (GridViewRow)lc.NamingContainer;
        string propertyValue = DataBinder.Eval(row.DataItem, bindExpression).ToString();
        lc.Text = propertyValue;
    }

    protected void EquipType_Changed(object sender, EventArgs e)
    {
        AjaxControlToolkit.ComboBox combo = sender as AjaxControlToolkit.ComboBox;
        GridViewRow row = combo.NamingContainer as GridViewRow;
        TextBox txtGrpSiz = row.FindControl("txtEdit4") as TextBox;
        if (combo.Text.Equals("Standard"))
        {
            txtGrpSiz.Text = "1";
        }
        else
        {
            txtGrpSiz.Text = "-1";
        }
    }

    protected void GrpSiz_Changed(object sender, EventArgs e)
    {
        TextBox txtGrpSiz = sender as TextBox;
        GridViewRow row = txtGrpSiz.NamingContainer as GridViewRow;
        AjaxControlToolkit.ComboBox combo = row.FindControl("comboEdit3") as AjaxControlToolkit.ComboBox;

        string postbackControlId = "";
        try
        {
            if (ScriptManager.GetCurrent(txtGrpSiz.Page).IsInAsyncPostBack)
            {
                postbackControlId = DbPage.GetAsyncPostBackControlID(txtGrpSiz.Page);
            }
            else
            {
                postbackControlId = DbPage.GetPostBackControl(txtGrpSiz.Page).ID;
            }
        }
        catch (Exception) { }
        if (!postbackControlId.EndsWith(combo.ID))
        {
            int num = 0;
            try
            {
                num = int.Parse(txtGrpSiz.Text);
                if (num == -1)
                {
                    combo.SelectedValue = "Delay";
                }
                else if (num > 0)
                {
                    combo.SelectedValue = "Standard";
                }
                //txtGrpSiz.Text = 10 + "";
            }
            catch (Exception) { }
        }
    }

    protected void SelectedProduct_Changed(object sender, EventArgs e)
    {
        AjaxControlToolkit.ComboBox comboProduct = sender as AjaxControlToolkit.ComboBox;
        GridViewRow row = comboProduct.NamingContainer as GridViewRow;
        AjaxControlToolkit.ComboBox comboFrom = row.FindControl("comboEdit2") as AjaxControlToolkit.ComboBox;
        AjaxControlToolkit.ComboBox comboTo = row.FindControl("comboEdit3") as AjaxControlToolkit.ComboBox;
        if (!comboProduct.SelectedValue.Equals(""))
        {
            if (comboFrom != null)
            {
                comboFrom.Enabled = true;
            }
            if (comboTo != null)
            {
                comboTo.Enabled = true;
            }
        }

    }



}
