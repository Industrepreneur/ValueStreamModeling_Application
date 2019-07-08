using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Configuration;

//  xxx  if indexer  no groups ..  error message ... 

public partial class group_models : DbPage {

   //  adding groupmanagement object not class1 or mailinfo 
    GroupManagement groupManagement;

    protected Panel pnlNewGroupPopup;
    protected Label lblNewGroupName;

    private const string OPEN_MODEL_LISTBOX_ARGUMENT = "open";

    protected string SortExpression {
        get {
            if (ViewState["SortExpression"] == null) {
                ViewState["SortExpression"] = "";
            }
            return (string)ViewState["SortExpression"];
        }
        set { ViewState["SortExpression"] = value; }
    }

    protected string CurrentGroupName
    {
        get
        {
            if (ViewState["CurrentGroupName"] == null)
            {
                ViewState["CurrentGroupName"] = "";
            }
            return (string)ViewState["CurrentGroupName"];
        }
        set { ViewState["CurrentGroupName"] = value; }
    }

    protected string CurrentFileid
    {
        get
        {
            if (ViewState["CurrentFileid"] == null)
            {
                ViewState["CurrentFileid"] = "";
            }
            return (string)ViewState["CurrentFileid"];
        }
        set { ViewState["CurrentFileid"] = value; }
    }
    public group_models() {
        PAGENAME = "group_models.aspx";
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
      
        groupManagement = new GroupManagement(GetDirectory(), username);
    }

  

    private int ShowFilesIn(string dir, ListBox lstBox) {
        int count = 0;
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        lstBox.Items.Clear();
        foreach (FileInfo fileItem in dirInfo.GetFiles()) {
            count++;
            lstBox.Items.Add(fileItem.Name);
        }
        return count;
    }

    public const string LASTPAGE_COOKIE = "lastPageCookie";

    protected void Page_Load(object sender, EventArgs e) {


        if (!this.IsPostBack)
        {

            DropDownList2Bind();

            userfilesBind();
            SetGroupModelsData(0);  //   set group models lisat at load! 0=all 

            HttpCookie lastPageCookie = Request.Cookies[LASTPAGE_COOKIE];  // was DbUse.LASTPAGE_COOKIE

            userfilesBind();
            /*
            string modelsDir = GetDirectory() + userDir + "\\" + MODELS;
            try
            {
                ShowFilesIn(modelsDir, lstPersonModels);
            }
            catch (Exception)
            {
                ShowErrorMessage("An error has occured. Missing user data.");
            }
             * */
            //string btnJavascript = ClientScript.GetPostBackClientHyperlink(
            //            btnOpen, "");
            //lstModels.Attributes["ondblclick"] = btnJavascript;
            //ShowFilesIn(outputsDir, lstOutputs);
        }
        else
        {  //  is post back so erase error message ...
            errormessage.Text = "";
        }
    }

    public void ShowErrorMessage(string message)
    {
        logFiles.ErrorMessageLog(message);

        lblGeneralError.Text = message;
        modalExtenderError.Show();
        SetFocus(btnOkError.ClientID);
    }

    /*   protected void btnAddGroup_Click(object sender, EventArgs e)
       {
           pnlNewGroupPopup = InputPageControls.GenerateNewNamePanel("Create new Model:");
           lblNewGroupName = pnlNewGroupPopup.FindControl(InputPageControls.LBL_NEW_NAME) as Label;
           String groupName = "";
           TextBox txtNewGroupName = pnlNewGroupPopup.FindControl(InputPageControls.TXT_NEW_NAME) as TextBox;
           String message;
           txtNewGroupName.Text = groupName;
           lblNewGroupName.Text = "Enter a name for the new group: ";
           int successCreate = cls_cd1.add_new_group(groupName);
           if (successCreate == 1)
           {
               message = "Gruop name is already in use. Please choose a new group name.";
               ShowErrorMessage(message);
           }
       }
       */

    protected void btnSort_Click(object sender, EventArgs e) {
        StringBuilder sb = new StringBuilder();
        string value = dropListSorting.SelectedValue;
        if (!SortExpression.Equals(String.Empty)) {
            string[] sortExpressions = SortExpression.Split(',');

            int index = -1;
            for (int i = 0; i < sortExpressions.Length; i++) {
                if (sortExpressions[i].IndexOf(value) >= 0) {
                    sortExpressions[i] = value + " " + lstRdbtnOrder.SelectedValue;
                    index = i;
                }
            }

          
            if (index >= 0) {

                for (int i = 0; i < sortExpressions.Length; i++) {
                    if (i == index) {
                        continue;
                    }
                    if (i != 0) {
                        sb.Append(",");
                    }
                    sb.Append(sortExpressions[i]);
                }
            } else {
                sb.Append(SortExpression);
            }
            if (sortExpressions.Length > 0 && index != 0) {
                sb.Append(",");
            }
        }
        sb.Append(value);
        sb.Append(" ");
        sb.Append(lstRdbtnOrder.SelectedValue);
        SortExpression = sb.ToString();

        string str1;    
        str1 = DropDownList2.SelectedValue;
        
        SetGroupModelsData(Convert.ToInt32(str1));


    }
    protected void btnResetSort_Click(object sender, EventArgs e) {
        SortExpression = "";
        dropListSorting.SelectedIndex = 0;
        lstRdbtnOrder.SelectedIndex = 0;

        string str1;
        str1 = DropDownList2.SelectedValue;
      
      
        SetGroupModelsData(Convert.ToInt32(str1));
    }

 
    public void setLeadeermsg(string username, string groupname) {

  int leads = groupManagement.has_group_leader(groupname, username);

  if (leads != 0)
  {
      leadermsg.Text = "You have Leader Status in this group so you can delete any model in the group.";
  }
  else
  {
      leadermsg.Text = "You DO NOT have Leader Status in this group so you can delete only models you added to the group.";
  } 
        return;
    }






    protected int get_modelid_row( GridViewRow row )
    {
        //  get model id from group models for a copy to user models 
        Label lblFileid = row.FindControl("lblFileid") as Label;

        //logFiles.ErrorLog(" getting model id  from row info so to get file from database");
        
        if (lblFileid == null )
        {
            logFiles.ErrorMessageLog(" getting model id  from row info so to get file from database no label fileid ...");
            return -1;
        }

        string fileid = lblFileid.Text;

        logFiles.ErrorMessageLog(" getting model id  from row info so to get file from database ..." + fileid);
        int ret = Convert.ToInt32(fileid);    

            return ret;
        }

    protected void btnLeft_Click(GridViewRow row) {
        //  get model from groups to user !!!
        //copy file to user

        //logFiles.ErrorLog(" left click - move to user list ");
           
        int fileid = get_modelid_row(row);  // get_modelid line from groups!
        if (fileid <0) return;
        string i = DropDownList2.SelectedValue;
        int groupid = Convert.ToInt32(i);
        string usr_dir = GetDirectory() + userDir + MODELS;
     

        int nfilen = groupManagement.get_file(fileid, groupid, usr_dir, username);//  get file from groups!!!
        if (nfilen == 0) {
            errormessage.Text = "An error has occured when copying model from group models to user directory.";
        }
        userfilesBind();

    }
   /* protected void btnLeft_Click_old(object sender, ImageClickEventArgs e)
    {
        //copy file  FROM GROUPS  to user\\
    }*/

    protected void btnRight_Click(object sender, ImageClickEventArgs e)
    {
        //copy file  FROM user to groups

      
        string i = DropDownList2.SelectedValue;
        int groupid = Convert.ToInt32(i);

        if (groupid ==0) { 
            errormessage.Text =" Please choose a specific group in the drop down list at the top before copying file into Group files.";
            return;
        }

        string usr_dir = GetDirectory() + userDir + "\\" + MODELS;
        string filename = "no name yet ";
        //logFiles.ErrorLog("  right click - move to group " + filename);
        try
        {

            filename = lstPersonModels.SelectedItem.Text;// get_modelid line from personmodels!!!
            //logFiles.ErrorLog("  right click - move to group got name:  " + filename);
      
        } catch (Exception) {

            errormessage.Text = "Cannot Copy to Group Models. No user model selected. ";
            return;
        }

        int i1 = groupManagement.add_model_file(usr_dir, filename, username, groupid); // put file to groups!!!
        if (i1 == 0)
        {
            errormessage.Text = "An error has occured when copying model from user directory to group models.";
        }

        SetGroupModelsData(groupid);
        //xx  update List of FileS in grid !!!

    }

    protected void dropList2_SelectedIndexChange(object sender, EventArgs e) {
        //Give list of groups to drop down list
        string i;

       i = DropDownList2.SelectedValue;

       
       int i1 = Convert.ToInt32(i);

       if (i1 == 0)
       {
           CurrentGroupName = "ALL groups";
           SetGroupModelsData(i1); 
           Label2.Text = "All files from All groups in which you are a member.";
           return;
       }

       string gname = groupManagement.find_group_name(i1);
       Label2.Text = "Name of Chosen Group: " + gname;


       //logFiles.ErrorLog(" drop list reset group models groupid, name " + i + gname);
       CurrentGroupName = gname; 
       SetGroupModelsData(i1);  

       
    }

    protected int DropDownList2Bind()  //  list of groups that user belong to ..> drop down list ....
    {
        int ret = 1;
        try {
            if (username.Length < 1) { username = "none"; }  //  error message here !!!  gwwd lucie xxx

            //  query to get all groups a username is in   + "all"  (group list pill down in modelspage)
             string selectQuery =" SELECT distinctrow webmpx.group_list.Group_id, webmpx.group_list.Group_name FROM webmpx.group_list INNER JOIN webmpx.group_members ON webmpx.group_members.Groupname = webmpx.group_list.Group_name WHERE (((Group_members.Username)= '" + username  +"' )) UNION SELECT webmpx.group_all.Group_id, webmpx.group_all.Group_name FROM webmpx.group_all order by group_id asc; ";

                        
            using (System.Data.Odbc.OdbcConnection connection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(selectQuery, connection)) {
                    command.Parameters.AddWithValue("@username", username);
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(command)) {
                        DataTable dt = new DataTable();
                        try {
                            adapter.Fill(dt);
                            if (dt.DefaultView.Count == 1)
                            {
                                ShowErrorMessage("You are not a member of any group.  Please create a new group or have a Leader in an existing group add you to that group");
                                //connection.Close(); Response.Redirect("groups.aspx"); 
                                ret = 0;
                            };
                             //  do not sort list of groups  sort list of files !!! ???
                            DropDownList2.DataSource = dt.DefaultView;
                            DropDownList2.DataBind();
                        } catch (Exception ex) {
                            throw new Exception("Error in getting data using sql query: " + selectQuery + ". " + ex.Message, ex);
                        }
                    }
                    connection.Close();
                }
            }

        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
            errormessage.Text = "An error has occured when reading group/user data.";

        }
        return ret;
    }
    


    protected void btnGoToManageGroups(object sender, EventArgs e) {
        Response.Redirect("groups.aspx");
    }

    protected void gridGroupModels_RowEditing(object sender, GridViewEditEventArgs e) {

    }
    protected void gridGroupModels_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {

    }
    protected void gridGroupModels_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }
    protected void gridGroupModels_RowDataBound(object sender, GridViewRowEventArgs e) {

    }
    protected void gridGroupModels_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }
    protected void gridGroupModels_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }

      
        int groupid = Convert.ToInt32   (DropDownList2.SelectedValue);
       

        GridViewRow row = btn.NamingContainer as GridViewRow;
        if (e.CommandName.Equals("Update")) {
            UpdateRow(row);
            gridGroupModels.EditIndex = -1;
        } else if (e.CommandName.Equals("Delete")) {
             doDeleteFile(row);  //  no confirm !! lucie gwwd 
        } else if (e.CommandName.Equals("Edit")) {
            gridGroupModels.EditIndex = row.RowIndex;
            //  get fileid info 
            CurrentFileid = Convert.ToString(get_modelid_row(row)); 
        }
        else if (e.CommandName.Equals("CancelUpdate"))
        {
            gridGroupModels.EditIndex = -1;
           
        } 
        else if (e.CommandName.Equals("Copy"))
        {
            btnLeft_Click(row);
        }

       
        SetGroupModelsData(groupManagement.find_group_id(CurrentGroupName));


    }

    protected void xxxConfirmUserDelete(GridViewRow row) {
        

        Label lblFilename = row.FindControl("lblFilename") as Label;
        Label lblFileOwner = row.FindControl("lblFileOwner") as Label;
        Label lblDate = row.FindControl("lblDate") as Label;

     

        if (lblFilename == null || lblFileOwner == null || lblDate == null) {
            return;
        }


        string fileName = lblFilename.Text;
        //logFiles.ErrorLog("INTO row delete model check? file ?   " + fileName);
        string fileOwner = lblFileOwner.Text;
        string date = lblDate.Text;
        //  gwwd  lblConfirmDelete.Text = "Are you sure you want to delete permanently file " + fileName + " created by " + fileOwner + " created on " + date + "?";

        //  NEED TO GET MODELID FIELD NUMBER !!!  
        hdnUserId.Value = Convert.ToString(get_modelid_row(row));  // got_modelid from GROUP MODELS!! 

        modalExtenderConfirmDelete.Show();
        //  gwwd  SetFocus(btnCancelDeleteFile.ClientID);
    }


    protected void btnDeleteFile_Click(object sender, EventArgs e)
    {
        int modelid = Convert.ToInt32(hdnUserId.Value);
        doDeleteFile(modelid);
        return;
    }

    public void btnDeleteModel(object sender, EventArgs e)
    {
        pnlError.Visible = false;
        
        return;

    } 
    
    public  void doDeleteFile(GridViewRow row) {
      
        // btnCancel <asp:Button ID="btnCancelDeleteFile" runat="server" Text="Cancel" Width="70px" OnClientClick="HidePopup('modalPopupConfirmDelete'); return true;" OnClick="btnCancelDeleteFile_Click" />
        //  panel  at end       style="display:none"

        string i = DropDownList2.SelectedValue;
        int groupid = Convert.ToInt32(i);
        int modelid = Convert.ToInt32(get_modelid_row(row)); 

       //  xxx lucie greg confirm delete file ? 
        //logFiles.ErrorLog("INTO DO row delete model" + i);

        RemoveModel(groupid, username, modelid);


        
        SetGroupModelsData(groupid);

    }

    public void doDeleteFile(int modelid)
    {
        string i = DropDownList2.SelectedValue;
        int groupid = Convert.ToInt32(i);
       
        //  ??  gwwd lucie ShowErrorMessage(" into delete file ");
        //logFiles.ErrorLog("INTO DO row delete model" + i);

        RemoveModel(groupid, username, modelid);



        SetGroupModelsData(groupid);

    }

    protected void btnCancelDeleteFile_Click(object sender, EventArgs e) {
        hdnUserId.Value = "";
        gridGroupModels.EditIndex = -1;
       
    }


    protected void RemoveModel(int groupid, string username, int modelid) {
        int ret;
        ret = groupManagement.delete_model_from_group(modelid, groupid, username);
        if (ret == 0) {
            errormessage.Text = "Model not removed.  Model is not group models list or user does have leader or owner status or no such group?";
        }

        string i = DropDownList2.SelectedValue;
        SetGroupModelsData(Convert.ToInt32(i));

    }



    /// /////////////////////////////////////////////////////////////////////////////////
    protected void userfilesBind() {

        int n_models =0;

        string modelsDir = GetDirectory() + userDir + "\\" + MODELS;
        try {
            n_models = ShowFilesIn(modelsDir, lstPersonModels);
        } catch (Exception) {
            errormessage.Text = " No models in user's server directory.";
        }


        //xx lucie check if no models in user list and then turn off left button ...
        // else turn on left button ...
        if (n_models < 1) btnRight.Visible = false;

    }


    protected void SetGroupModelsData(int groupid) {

        string selectQuery;
        try {
            if (groupid == 0) {
                //get list of all models in all user member groups (groupid = 0 <=> all) 
                leadermsg.Text = "You may have Leader Status for some of these groups so you can ";
                leadermsg2.Text = "delete any model (1) you added or (2) from a group for which you have leader status.";
                //leadermsg.Text = "You may have Leader Status for some of these groups ";
                selectQuery = "SELECT distinctrow webmpx.group_members.Username, webmpx.group_files.file_id, webmpx.group_files.* FROM webmpx.group_members INNER JOIN webmpx.group_files ON webmpx.group_members.Groupname = webmpx.group_files.Groupname WHERE (((Group_members.Username)='" + username + "'));";
                gridGroupModels.Columns[2].Visible = true;
            } else {
                string groupname = groupManagement.find_group_name(groupid);
                //  get list of models in group  (list of models from 1 group)
                selectQuery = " SELECT distinctrow webmpx.group_files.Groupname, webmpx.group_files.file_id, webmpx.group_files.* From Webmpx.group_files WHERE (((group_files.Groupname)= '" + groupname + "'));";
                setLeadeermsg(username, groupname);
                leadermsg2.Text = "";
                gridGroupModels.Columns[2].Visible = false;
            }

           //  xxx lucie errors with no models in all ???

            using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(selectQuery, connection))
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(command)) {
                    DataTable dt = new DataTable();
                    try {
                        adapter.Fill(dt);
                        dt.DefaultView.Sort = SortExpression;
                        gridGroupModels.DataSource = dt.DefaultView;
                        gridGroupModels.DataBind();
                    } catch (Exception ex) {
                        throw new Exception("Error in getting data using sql query: " + selectQuery + ". " + ex.Message, ex);
                    }
                }
                connection.Close();
            }

        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
            errormessage.Text = "An error has occured when reading group models data.";

        }
    }

    
  
    protected void UpdateRow(GridViewRow row) {

        TextBox txtComment = row.FindControl("txtComments") as TextBox;    
        string file_id = CurrentFileid;
       
        int file_id_int = Convert.ToInt32(file_id);
        string comment1 = MyUtilities.clean(txtComment.Text);

        string updateQuery = "UPDATE webmpx.group_files  SET webmpx.group_files.Comments = '"+ comment1 +"' WHERE webmpx.group_files.file_id = "+ file_id +";";

        if (!DbUse.RunMySqlParams(updateQuery,
                new string[] { }, new object[] { }))
        {
            return;
        }
        int groupid = groupManagement.find_group_id(CurrentGroupName);
        SetGroupModelsData(groupid);

    }

    protected void gridGroupModels_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        gridGroupModels.PageIndex = e.NewPageIndex;

        errormessage.Text = "page change in grid has occured ? ";
    }
}