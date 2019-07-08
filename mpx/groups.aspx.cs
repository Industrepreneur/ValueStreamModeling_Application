using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data.Odbc;
using System.Data.OleDb;

// xxx   no groups ..  error message ... 

public partial class Groups : DbPage 
{
    //  adding groupmanagement object not class1 or mailinfo 
    GroupManagement groupManagement;

    protected Panel pnlNewGroupPopup;
    protected Label lblNewGroupName;

   
    public Groups () {
    PAGENAME = "groups.aspx";
    } 


    LogFiles logFiles = new LogFiles("admingla");

    protected string SortExpression
    {
        get
        {
            if (ViewState["SortExpression"] == null)
            {
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
                ViewState["CurrentGroupName"] = "None";
            }
            return (string)ViewState["CurrentGroupName"];
        }
        set { ViewState["CurrentGroupName"] = value; }
    }

    
    protected override void OnInit(EventArgs e) {
        
        base.OnInit(e);
        groupManagement = new GroupManagement(GetDirectory(), username);

       

      }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            int ret = DropDownList2Bind();
            if (ret !=0) SetUserGridData();

        }
    }

    public void ShowErrorMessage(string message)
    {
        lblGeneralError.Text = message;
        modalExtenderError.Show();
        SetFocus(btnOkError.ClientID);
        logFiles.ErrorMessageLog("show error message text " + message);
    }

    protected void dropSelectGroup_SelectedIndexChange(object sender, EventArgs e)
    {

        int groupid = Convert.ToInt32( dropSelectGroup.SelectedValue);
        string groupname = groupManagement.find_group_name(groupid);

        CurrentGroupName = groupname;

        //logFiles.ErrorLog("INTO drop select change " + groupname);

        curgrpname.Text = groupname;
        CurrentGroupName = groupname;  

        SetUserGridData();


        chosengroupname.Text = " Chosen Group :" + groupname;

    }


    public void setLeadeermsg(string username, string groupname)
    {
        int leads;
        if ((username == "") || (groupname == ""))
            {  leads = 0; }
        else
            {  leads = groupManagement.has_group_leader(groupname, username); }
    

        if (leads != 0)
        {
            leadermsg.Text = "You have Leader Status in this group so you can delete the group, delete users, add users and change leader status on users.";
            btnCreateNewGroup.Enabled = true;
            btnDeleteGroup.Enabled = true;
            btnAddUser.Enabled = true;
        }
        else
        {
            leadermsg.Text = "You DO NOT have Leader Status in this group so you can change nothing. You can create a new group.";
            btnCreateNewGroup.Enabled = true;
            btnDeleteGroup.Enabled = false;
            btnAddUser.Enabled = false;
         }
        return;
    }

    protected string get_row_name(GridViewRow row)
    {
        //  get  user  name  
        Label lblUsername = row.FindControl("lblUsername") as Label;
        Label txtUsername = row.FindControl("txtUsername") as Label;

        //logFiles.ErrorLog(" getting model id  from row info so to get file from database");

        if ((lblUsername == null)  & (txtUsername == null) )
        {
            logFiles.ErrorMessageLog(" getting username   from row info so to get file from database no label fileid ...");
            return "";
        }

          string username;
          if (lblUsername == null)      
            username = txtUsername.Text;
          else
            username = lblUsername.Text;
     

        return username;
    }
    protected void groupmembers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        Button btn = e.CommandSource as Button;
        if (btn == null)
        {
            return;
        }
        if (groupManagement.has_group_leader(CurrentGroupName, username) ==0) return;

        //logFiles.ErrorLog("INTO row command " + e.CommandName);

        GridViewRow row = btn.NamingContainer as GridViewRow;
        if (e.CommandName.Equals("Delete"))
        {
            ConfirmUserDelete(row);
        }
        else if (e.CommandName.Equals("Flip"))
        {
            groupmembers.EditIndex = row.RowIndex;
            string leadername = get_row_name(row);
            int ret = groupManagement.flipLeader(CurrentGroupName, username, leadername);
            if (ret == 0) ShowErrorMessage("  Did not flip Lead status");

        }
       

        SetUserGridData();
    }

  

    protected void SetUserGridData()
    {


         

        //logFiles.ErrorLog("INTO SET USER GRID: curr group " + CurrentGroupName);

        try
        {
            string groupname = CurrentGroupName;
          
            curgrpname.Text = " Current Group: " + groupname;
            if (CurrentGroupName == "None") return;

            
            setLeadeermsg(username, groupname);


            int groupid = groupManagement.find_group_id(groupname);
            if (groupid < 1)
            {
                ShowErrorMessage(" did not find group " + groupname);
                return;
            }

            string selectQuery = " SELECT distinctrow webmpx.group_members.*  FROM webmpx.group_members WHERE (((Group_members.Groupname)='" + groupname + "')) order by webmpx.group_members.Username asc;";

            using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(selectQuery, connection))
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.Fill(dt);
                        dt.DefaultView.Sort = SortExpression;
                        groupmembers.DataSource = dt.DefaultView;
                        groupmembers.DataBind();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in getting data using sql query: " + selectQuery + ". " + ex.Message, ex);
                    }
                }
                connection.Close();
            }

        }
        catch (Exception exp)
        {
            logFiles.ErrorLog(exp);
            ShowErrorMessage("An error has occured when reading user data.");

        }
    }

    
    protected int DropDownList2Bind()  //  list of groups that user belong to ..> drop down list ....
    {
       // xxx  need ToolboxBitmapAttribute reset ToolboxBitmapAttribute first groupManagement in ListBindableAttribute !!! always.
       // execpt if just AddedControl new GroupManagement 

        //logFiles.ErrorLog("into drop list bind ");   

        try {
            if (username.Length < 1) { username = "none"; }  //  error message here !!!  gwwd lucie xxx

            //  query to get all groups a username is in   not ALL (group list pill down in modelspage)
            //string selectQuery = "SELECT group_list.Group_id, group_list.Group_name FROM group_members INNER JOIN group_list ON group_list.Group_id = group_members.GroupID WHERE group_members.Username = @username;";
            string selectQuery = "SELECT distinctrow webmpx.group_list.Group_id, webmpx.group_list.Group_name FROM webmpx.group_members INNER JOIN webmpx.group_list ON webmpx.group_members.Groupname = webmpx.group_list.Group_name   where webmpx.group_members.Username = '" + username + "' order by webmpx.group_list.Group_name ;";

            using (System.Data.Odbc.OdbcConnection connection = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(selectQuery, connection)) {
                    
                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(command)) {
                        DataTable dt = new DataTable();
                        try {
                            adapter.Fill(dt);
                            dt.DefaultView.Sort = SortExpression;
                            dropSelectGroup.DataSource = dt.DefaultView;
                            dropSelectGroup.DataBind();
                            if (dt.DefaultView.Count == 0) { ShowErrorMessage("You are not a member of any group.  Please create a new group or have a Leader in an existing group add you to that group");
                                  setLeadeermsg("", "");  return 0;
                            };
                             if (CurrentGroupName == "None") { dropSelectGroup.SelectedIndex=0; CurrentGroupName = groupManagement.find_group_name(Convert.ToInt32(dropSelectGroup.SelectedValue)); }                      
                                
                        } catch (Exception ex) {
                            throw new Exception("Error in getting data using sql query: " + selectQuery + ". " + ex.Message, ex);
                        }
                    }
                    connection.Close();
                }
            }

        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
            Master.ShowErrorMessage("An error has occured when reading group list data.");

        }

        return -1;
    }



    protected void ConfirmUserDelete(GridViewRow row)
    {

        string delName =   get_row_name(row);

        string groupname = CurrentGroupName;

        lblConfirmDelete.Text = "Are you sure you want to remove permanently user '" + delName + "' from group " + groupname;
        hdnUserId.Value = delName; //  saving name to be deleted.

        int groupid = groupManagement.find_group_id(groupname);

        int ret = groupManagement.delete_user_from_group(groupid, username, delName); //  groupid, username is name of user DOING - MUST HAVE LEADER STATUS - CHECKED IN CODE

        if (ret == 0)
        {
            ShowErrorMessage("User not removed from group.  Current client (i.e. you) may not have Leader status...");
        }

        SetUserGridData();
        return; 

        //  lucie greg   no confirm ??

       
      //  modalExtenderConfirmDelete.Show();
      //  SetFocus(btnCancelDeleteUser.ClientID);

    }


    protected void ConfirmGroupDelete(object sender, EventArgs e)
    {

        string groupname = CurrentGroupName;
        //logFiles.ErrorLog("into ask confirm group " + groupname + CurrentGroupName);


        lblConfirmGroup.Text = "Are you sure you want to delete group " + groupname + " permanently ?  ALL FILES IN THE GROUP WILL BE LOST."; 
      
        modalExtenderConfirmDelete2.Show();
        SetFocus(btnCancelDeleteGroup.ClientID);

    }

    protected void btnDeleteGroup_Click(object sender, EventArgs e)
    {

       //confirmed to Delete
        // string str1 = curgrpname.Text;

       // logFiles.ErrorLog("into Do delete group " + str1 + CurrentGroupName);

       
        string groupname = CurrentGroupName;

     
        int groupid = groupManagement.find_group_id(groupname);
        if (groupid == 0)
        {
            ShowErrorMessage("Group not deleted.  Group name not found ?" + groupname);
            logFiles.ErrorMessageLog("Do delete group " + "Group not deleted.  Group name not found ?" + groupname);
        }

        int ret = groupManagement.delete_group(groupname, username);

        if (ret == 0)
        {
            ShowErrorMessage("Group not deleted.  Current client (i.e. you) may not have Leader status...");
            logFiles.ErrorMessageLog("Do delete group " + "Group not deleted.  Current client (i.e. you) may not have Leader status...");
        }

        CurrentGroupName = "None";

        //Response.Redirect("groups.aspx");
        //DropDownList2Bind();
        //SetUserGridData();
        ret = DropDownList2Bind();
        if (ret != 0) SetUserGridData();
        
        string message = "User group " + groupname + " deleted.";

        MailInfo.SendMail(message, MailInfo.GROUP_DELETED);


    }



    protected void btnCancelDeleteGroup_Click(object sender, EventArgs e)
    {
        hdnUserId.Value = "";
    }



    protected void btnDeleteUser_Click(object sender, EventArgs e)
    {
       

        string del_name = hdnUserId.Value; //  name of user to delete ... get name from hiddden field
        //ShowErrorMessage("User name  ? " + del_name);

      

        //  string groupname  = curgrpname.Text;
        string groupname = CurrentGroupName;
        int groupid = groupManagement.find_group_id(groupname);

        //logFiles.ErrorLog("into ask delete user" + del_name + groupname);

        //ShowErrorMessage("Group ? " + groupname);

        if (groupid == 0)
        {
            ShowErrorMessage("Group not found ? " + groupname );
            return;
        }

        int ret = groupManagement.delete_user_from_group ( groupid, username, del_name); //  groupid, username is name of user DOING - MUST HAVE LEADER STATUS - CHECKED IN CODE

        if (ret == 0)
        {
            ShowErrorMessage("User not removed from group.  Current client (i.e. you) may not have Leader status...");
        }
        else
        {
            string message = "User group " + groupname + " User " + del_name + " deleted  by user " + username;

            MailInfo.SendMail(message, MailInfo.GROUP_USER_DELETED);
        }

        SetUserGridData();

    }
    protected void btnCancelDeleteUser_Click(object sender, EventArgs e)
    {
        hdnUserId.Value = "";

        //logFiles.ErrorLog("into cancel delete user");
    }



    protected void groupmembers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        groupmembers.PageIndex = e.NewPageIndex;
        SetUserGridData();
    }
    protected void groupmembers_RowEditing(object sender, GridViewEditEventArgs e)
    {

    }
    protected void groupmembers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {

    }
    protected void groupmembers_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

    }
    protected void groupmembers_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void groupmembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }




public void CreateGroup_Click(object sender, EventArgs e)
   {
      
       ModalPopupExtender3.Show();
       SetFocus(AddGroupName.ClientID);

   }

public void btnCancelCreateGroup_Click(object sender, EventArgs e)
   {
       ModalPopupExtender3.Hide();
       DropDownList2Bind();
   }

   public void btnCreateGroup_Click(object sender, EventArgs e)
   {
       string message;
       string groupName = MyUtilities.clean(AddGroupName.Text);
       groupName = groupName.Trim();
       if (groupName == "")
       {
           //ShowErrorMessage("No group name entered.  Please try again.");  
           return;
       }

      

       int successCreate = groupManagement.add_new_group(groupName, username);
       if (successCreate == 0)
       {
           message = "Group name is already in use. Please choose a new group name.";
           logFiles.ErrorMessageLog(message);
           ShowErrorMessage(message);
           
          ModalPopupExtender3.Show();
           return;
       }
       if (successCreate == -1)
       {
           message = "Couldn't find added Group name TIMING!!!.";
           logFiles.ErrorMessageLog(message);
           ShowErrorMessage(message);
           
           return;
       }

       CurrentGroupName = groupName;
       ModalPopupExtender3.Hide();
       DropDownList2Bind();
       ModalPopupExtender3.Hide();

       //ShowErrorMessage("added group " + groupName);
       message = "User group " + groupName + " created.";
       MailInfo.SendMail(message, MailInfo.GROUP_CREATED);

       SetUserGridData();
  
   }

   public void btnAddUser_Click(object sender, EventArgs e)
    {
        string userNameNew = MyUtilities.clean(txtNewUser.Text);  //   clean user name in case try to crash DB !!!
        userNameNew.Trim();
        string message;
        if (userNameNew == "")
        {
            ShowErrorMessage("No user name entered.  Please try again.");
            return;
        }

       string groupname = CurrentGroupName;


        int groupID = groupManagement.find_group_id(groupname);

        
        //logFiles.ErrorLog(" trying to add user to group " + groupname + " user " + userNameNew);

        int successAdd = groupManagement.unique_user_name(groupname, userNameNew);

        if (successAdd == 0)
        {
            message = "User in group already?";
            logFiles.ErrorMessageLog(message);
            ShowErrorMessage(message);

            return;
        }

        bool leader1 = chkIsLeader.Checked;
        string ld1 = Convert.ToString(leader1);

       
      
        successAdd = groupManagement. add_user_to_group(groupname, userNameNew, leader1, username);
        if (successAdd == 0)
        {
            message = "New user not added.  User '" + userNameNew + "' not found? ";
            logFiles.ErrorMessageLog(message);
            ShowErrorMessage(message);
        }

        message = "User added to group " + groupname + " username " +userNameNew ;

        MailInfo.SendMail(message, MailInfo.GROUP_USER_ADDED);

        SetUserGridData();
        txtNewUser.Text= "";
        chkIsLeader.Checked = false;
    }


}  //  end class 