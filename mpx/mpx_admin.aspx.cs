using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using PasswordHash;

public partial class mpx_admin : DbPage {
    public mpx_admin() {
        PAGENAME = "/mpx_admin.aspx";
    }

    //LogFiles logFiles = new LogFiles("admingla");

    protected string SortExpression {
        get {
            if (ViewState["SortExpression"] == null) {
                ViewState["SortExpression"] = "";
            }
            return (string)ViewState["SortExpression"];
        }
        set { ViewState["SortExpression"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e) {
       
            //SetUserGridData();
       
    }

    protected override void OnInit(EventArgs e) {
        //base.OnInit(e);
        //if (!username.Equals("admingla") )  // && !username.Equals("greg"))  //  test
        //{
        //    Response.Redirect("/login.aspx");
        //}
        SetUserGridData();
    }



    protected void btnAddUser_Click(object sender, EventArgs e){
        // check for username and user directory if it is unique
        // check for valid password and confirm password
        // add username to user database
        // create the user directory
        // copy default mpx files
        try {
            string usernameNew = txtNewUser.Text.Trim();
            if (usernameNew.Equals(String.Empty)) {
                throw new Exception("Username cannot be empty.");
            } else if (!usernameNew.Equals(MyUtilities.clean(usernameNew))) {
                throw new Exception("Invalid characters in username.");
            }
            string userDirNew = txtUserdir.Text.Trim();
            if (String.IsNullOrWhiteSpace(userDirNew)) {
                throw new Exception("User directory cannot consist of whitespace only.");
            }
            if (userDirNew.IndexOf(".") == 0) {
                throw new Exception("User directory name cannot begin with '.'");
            }
            string cleanedUserDir = MyUtilities.clean(userDirNew);
            cleanedUserDir = MyUtilities.clean(cleanedUserDir, '*');
            if (!userDirNew.Equals(cleanedUserDir)) {
                throw new Exception("Invalid characters in user directory name.");
            }

            string password = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            if (!password.Equals(confirmPassword)) {
                throw new Exception("Password and confirm password do not match.");
            }

            string company = MyUtilities.clean(txtCompany.Text);
            if (UsernameExists(usernameNew)) {
                throw new Exception("Username '" + usernameNew + "' already exists. Please select a different username.");
            }
            if (UserdirExists(userDirNew)) {
                throw new Exception("User directory '" + userDirNew + "' already exists. Please select a different user directory name.");
            }
            string email = txtEmail.Text;
            int ret1;
            ret1 = CreateUser(usernameNew, password, userDirNew, email, company);
                if (ret1 != -1) {
                  if (ret1 == 1) throw new Exception("An error has occured.  Problem at section 1. The user might not have been created correctly. Please contact the server administrator.");
                  if (ret1 == 2) throw new Exception("An error has occured.  Problem at section 2. The user might not have been created correctly. Please contact the server administrator.");
                  if (ret1 == 3) throw new Exception("An error has occured.  Problem at section 3. The user might not have been created correctly. Please contact the server administrator.");
                  if (ret1 == 4) throw new Exception("An error has occured.  Problem at section 4. The user might not have been created correctly. Please contact the server administrator.");
                   throw new Exception("An error has occured.  Problem at unknown section. The user might not have been created correctly. Please contact the server administrator.");
                 }

            //lblError.ForeColor = System.Drawing.Color.Black;
            //lblError.Visible = true;
            //lblError.Text = "User '" + usernameNew + "' was created successfully.";
            //logFiles.InfoLog(LogFiles.USER_CREATED, usernameNew);

            try {
                string message = String.Format("User {0} with user subdirectory '{1}' was created successfully.\nIP Address: {2}\nWebsite: {3}", usernameNew, userDirNew, System.Web.HttpContext.Current.Request.UserHostAddress, DbUse.DomainPath);
                //MailInfo.SendMail(message, MailInfo.USER_CREATED);
            } catch (Exception exp) {
                //logFiles.ErrorLog(exp);
            }

            txtNewUser.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtUserdir.Text = "";
            txtEmail.Text = "";
            txtCompany.Text = "";

            ShowInfoMessage(String.Format("User {0} was created successfully.", usernameNew));

            SetUserGridData();



        } catch (Exception ex) {
            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#cc0000");
            lblError.Visible = true;
        }
        //lblErrorEdit.Visible = false;
    }

    protected bool UsernameExists(string username) {
        try {
            string id = DbUse.GetMysqlDatabaseField("id", "username", username, "userlist");
        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return false;
            }
        }
        return true;
    }

    protected bool UserdirExists(string userDirectory) {
        try {
            string id = DbUse.GetMysqlDatabaseField("id", "usersub", userDirectory, "userlist");
        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return false;
            }
        }
        return true;
    }

    protected int CreateUser(string username, string password, string userDirectory, string email, string company) {
        string hash = PasswordHash.PasswordHash.CreateHash(password);

        string slash = "\\\\";
        string null1 = "";
        char csl = '\\';
        slash = slash.Insert(1, null1);
        //slash = slash.Substring(0, 1);
        int z1 = slash.Length;

        string helperUserdir = userDirectory + "\\\\";
       
        userDirectory = userDirectory + "\\";
        

        string values = "('" + username + "','" + helperUserdir + "', '" + DbUse.DEFAULT_COOKIE_ID + "', " + 0 + ", 'none', '" + company + "')";
        string sqlcmd;
        string cookie_val;
        cookie_val = DbUse.DEFAULT_COOKIE_ID;
        sqlcmd = " INSERT INTO userlist (username, usersub, userid, lastUpdate, currentModel, company) VALUES ( '"+ username + "', '"+ helperUserdir + "' , '" + cookie_val + "', 0 ,'none' , '" + company +"');";
        if (!DbUse.RunMysql(sqlcmd))
            return 1;

         //  if (!DbUse.RunMySqlParams("INSERT INTO userlist (username, usersub, userid, lastUpdate, currentModel, company) VALUES ( ?, ?, ?, ?, ?, ?)",
         //       new string[] { "username", "usersub", "userid", "lastUpdate", "currentModel", "company" }, new object[] { username , helperUserdir, DbUse.DEFAULT_COOKIE_ID, 0, "none", company})) {
         //   return 1;
        // }

        try {
            int id = int.Parse(DbUse.GetMysqlDatabaseField("id", "username", username, "userlist"));
            
            sqlcmd =  "INSERT INTO userlog (id, username, usersub, usercode) VALUES (" + id + ", '" + username + "', '" + helperUserdir +"', '" +hash +"');";
                     
            if (!DbUse.RunMysql(sqlcmd))
                return 2;
            values = "(" + id + ", '" + username + "', '" + helperUserdir + "', '" + hash + "')";
            //    (!DbUse.RunMySqlParams("INSERT INTO userlog (id, username, usersub, usercode) VALUES (?, ?, ?, ?)" + values + ";", new string[] { "id", "username", "usersub", "usercode" }, new object[] { id, username, helperUserdir, hash })) {
            //    return 2;
           // }
            string insertQuery = "INSERT INTO usercred (id, username, mail) VALUES (?, ?, ?);";
            try {
                using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(insertQuery, connection)) {
                        command.Parameters.AddWithValue("id", id);
                        command.Parameters.AddWithValue("username", username);
                        command.Parameters.AddWithValue("mail", email);
                        try {
                            command.ExecuteNonQuery();
                            gridUsers.EditIndex = -1;
                            command.Dispose();
                        } catch (Exception ex) {
                            throw new Exception("Error in getting data using sql query: " + insertQuery + ". " + ex.Message, ex);
                        }
                    }
                    connection.Close();
                }
            } catch (Exception ex) {
                //logFiles.ErrorLog(ex);
                return 3;
            }

            string directory = GetDirectory() + userDirectory;
            if (!Directory.Exists(directory)) {
                // create user directory
                Directory.CreateDirectory(directory);

                // create necessary subdirectories in user directory
                Directory.CreateDirectory(directory + MODELS);
                Directory.CreateDirectory(directory + DbUse.UPLOAD_TEMP_DIR);
                Directory.CreateDirectory(directory + DbUse.GRAPHS_DIR);

                Directory.CreateDirectory(GetMainDirectory() + DbPage.BROWSER_DIR + "//" + userDirectory + DbUse.GRAPHS_DIR);

                string mainDbFile = GetDirectory() + MAIN_USER_DATABASE;
                string initDbFile = GetDirectory() + NEW_MODEL_DATABASE;
                string demoFile = GetDirectory() + DbUse.DEMO_MODEL;
                string helpFile1 = GetDirectory() + "a.xxx";
                string helpFile2 = GetDirectory() + "b.xxx";

                // copy default mpx files
                File.Copy(mainDbFile, GetDirectory() + userDirectory + MAIN_USER_DATABASE);
                File.Copy(initDbFile, GetDirectory() + userDirectory + NEW_MODEL_DATABASE);
                File.Copy(demoFile, GetDirectory() + userDirectory + MODELS + "\\" + DbUse.DEMO_MODEL);
                File.Copy(helpFile1, GetDirectory() + userDirectory + "a.xxx");
                File.Copy(helpFile2, GetDirectory() + userDirectory + "b.xxx");

            }
        } catch (Exception) {
            return 4;
        }
        return -1;
    }
    protected void btnHash_Click(object sender, EventArgs e) {
        //txtHash.Text = PasswordHash.PasswordHash.CreateHash(txtHash.Text);
    }

    protected void btnLogout_Click(object sender, EventArgs e) {
        string logoutMessage = "Normal logout.";
        DbUse.WriteLogoutMessageToDb(logoutMessage);
        DbUse.LogoutUser();
        Response.Redirect(LOGOUT_PAGE);
    }

    protected void btnResetCountdown_Click(object sender, EventArgs e) {

    }

    /*
    protected void btnUpdateUser_Click(object sender, EventArgs e) {
        string username = MyUtilities.clean(txtUsernameEdit.Text);
        string email = txtEmailEdit.Text;
        try {
            if (!username.Equals(txtUsernameEdit.Text)) {
                throw new Exception("Invalid characters in username.");
            }
            if (!UsernameExists(username)) {
                throw new Exception("The username does not exist.");
            }
            if (!DbUse.RunMysql("UPDATE usercred INNER JOIN userlist ON usercred.id = userlist.id SET usercred.mail = '" + email + "' WHERE userlist.username = '" + username + "';")) {
                throw new Exception("Missing data. Please contact the database administrator.");
            }
            lblErrorEdit.ForeColor = System.Drawing.Color.Black;
            lblErrorEdit.Visible = true;
            lblErrorEdit.Text = "User '" + username + "' was updated successfully.";
        } catch (Exception ex) {
            lblErrorEdit.Text = ex.Message;
            lblErrorEdit.ForeColor = System.Drawing.ColorTranslator.FromHtml("#cc0000");
            lblErrorEdit.Visible = true;

        }
        lblError.Visible = false;
    }
     * */
    protected void gridUsers_RowEditing(object sender, GridViewEditEventArgs e) {

    }
    protected void gridUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {

    }
    protected void gridUsers_RowUpdating(object sender, GridViewUpdateEventArgs e) {

    }
    protected void gridUsers_RowDataBound(object sender, GridViewRowEventArgs e) {

    }
    protected void gridUsers_RowDeleting(object sender, GridViewDeleteEventArgs e) {

    }
    protected void gridUsers_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }

        GridViewRow row = btn.NamingContainer as GridViewRow;
        if (e.CommandName.Equals("Update")) {
            UpdateRow(row);
        } else if (e.CommandName.Equals("Delete")) {
            ConfirmUserDelete(row);
        } else if (e.CommandName.Equals("Edit")) {
            gridUsers.EditIndex = row.RowIndex;
        } else if (e.CommandName.Equals("CancelUpdate")) {
            gridUsers.EditIndex = -1;
        }
        SetUserGridData();
    }

    protected void ConfirmUserDelete(GridViewRow row) {
        Label lblUsername = row.FindControl("lblUsername") as Label;
        Label lblCompany = row.FindControl("lblCompany") as Label;
        Label lblEmail = row.FindControl("lblMail") as Label;

        if (lblUsername == null || lblCompany == null || lblEmail == null) {
            return;
        }

        string userName = lblUsername.Text;
        string company = lblCompany.Text;
        string email = lblEmail.Text;
        lblConfirmDelete.Text = "Are you sure you want to delete permanently user " + userName + " with the email " + email + " representing company " + company + "? All his files and directories as well as his credentials will be permanently lost.";
        hdnUserId.Value = gridUsers.DataKeys[row.RowIndex]["id"].ToString();
        modalExtenderConfirmDelete.Show();
        SetFocus(btnCancelDeleteUser.ClientID);
    }

    protected void UpdateRow(GridViewRow row) {
        TextBox txtMail = row.FindControl("txtMail") as TextBox;
        TextBox txtCompany = row.FindControl("txtCompany") as TextBox;

        if (txtMail == null || txtCompany == null) {
            return;
        }
        string email = txtMail.Text;
        string company = MyUtilities.clean(txtCompany.Text);
        string updateQuery = "UPDATE userlist INNER JOIN usercred ON userlist.id = usercred.id SET userlist.company = ?, usercred.mail = ? WHERE userlist.id = ?;";
        try {
            using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(updateQuery, connection)) {
                    command.Parameters.AddWithValue("company", company);
                    command.Parameters.AddWithValue("mail", email);
                    command.Parameters.AddWithValue("id", gridUsers.DataKeys[row.RowIndex]["id"]);
                    try {
                        command.ExecuteNonQuery();
                        gridUsers.EditIndex = -1;
                        command.Dispose();
                    } catch (Exception ex) {
                        throw new Exception("Error in getting data using sql query: " + updateQuery + ". " + ex.Message, ex);
                    }
                }
                connection.Close();
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            ShowErrorMessage("An internal error has occured. Cannot open database connection.");
        }
    }

    private bool DeleteUserdir(string userdir, string username) {
        string appDataPath = GetDirectory();
        DirectoryInfo userDirInfo = new DirectoryInfo(appDataPath + userdir);
        DirectoryInfo appDataDirInfo = new DirectoryInfo(appDataPath);
        var pathUser = Path.Combine(userDirInfo.Parent.FullName, userdir);
        string userGraphsPath = GetMainDirectory() + DbPage.BROWSER_DIR + "//" + userdir + DbUse.GRAPHS_DIR;
        if (userDirInfo.Exists) {
            DirectoryInfo parentDirInfo = userDirInfo.Parent;
            if (parentDirInfo.FullName.TrimEnd('\\').Equals(appDataDirInfo.FullName.TrimEnd('\\'))) {
                return DeleteRecursivelyWithRetries(userDirInfo.FullName, username) && DeleteRecursivelyWithRetries(userGraphsPath, username);
            } else {
                //logFiles.ErrorMessageLog("Invalid user subdirectory '" + userDirInfo.FullName + "' - not a child of App_Data directory.");
                return false;
            }
        }
        return true;
    }

    private bool DeleteRecursivelyWithRetries(string destinationDir, string username) {
        const int maxTries = 10;
        for (var numTries = 1; numTries <= maxTries; numTries++) {
            try {
                Directory.Delete(destinationDir, true);
                //logFiles.ErrorMessageLog("Subdirectory '" + destinationDir + "' of user '" + username + "' deleted.");
                return true;
            } catch (DirectoryNotFoundException) {
                //logFiles.ErrorMessageLog("Subdirectory '" + destinationDir + "' of user '" + username + "' not found.");
                return true;  // good!
            } catch (IOException) { // System.IO.IOException: The directory is not empty or in use (Windows Explorer, website, virus scan,...)
                //logFiles.ErrorMessageLog("Some process prevents deletion of '" + destinationDir + "'. Waiting, attempt #" + numTries + ".");
                System.Threading.Thread.Sleep(50);
            }

        }
        return false;
    }



    protected void DeleteUser() {
        try {
            int id = int.Parse(hdnUserId.Value);

            string userdir = DbUse.GetMysqlDatabaseField("usersub", "id", id, "userlist");
            string userName = DbUse.GetMysqlDatabaseField("username", "id", id, "userlist");
            string deleteQuery = "DELETE userlist.* FROM userlist WHERE userlist.id = ?;";

            if (DeleteUserdir(userdir, userName)) {
                try {
                    using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                        connection.Open();
                        using (OdbcCommand command = new OdbcCommand(deleteQuery, connection)) {
                            command.Parameters.AddWithValue("id", id);
                            try {
                                command.ExecuteNonQuery();
                                command.Dispose();
                            } catch (Exception ex) {
                                throw new Exception("Error in deleting data using sql query: " + deleteQuery + ". " + ex.Message, ex);
                            }
                        }
                        deleteQuery = "DELETE usercred.* FROM usercred WHERE usercred.id = ?;";
                        using (OdbcCommand command = new OdbcCommand(deleteQuery, connection)) {
                            command.Parameters.AddWithValue("id", id);
                            try {
                                command.ExecuteNonQuery();
                                command.Dispose();
                            } catch (Exception ex) {
                                throw new Exception("Error in deleting data using sql query: " + deleteQuery + ". " + ex.Message, ex);
                            }
                        }
                        deleteQuery = "DELETE userlog.* FROM userlog WHERE userlog.id = ?;";
                        using (OdbcCommand command = new OdbcCommand(deleteQuery, connection)) {
                            command.Parameters.AddWithValue("id", id);
                            try {
                                command.ExecuteNonQuery();
                                command.Dispose();
                            } catch (Exception ex) {
                                throw new Exception("Error in deleting data using sql query: " + deleteQuery + ". " + ex.Message, ex);
                            }
                        }
                        deleteQuery = "DELETE usercalc.* FROM usercalc WHERE usercalc.id = ?;";
                        using (OdbcCommand command = new OdbcCommand(deleteQuery, connection)) {
                            command.Parameters.AddWithValue("id", id);
                            try {
                                command.ExecuteNonQuery();
                                command.Dispose();
                            } catch (Exception ex) {
                                throw new Exception("Error in deleting data using sql query: " + deleteQuery + ". " + ex.Message, ex);
                            }
                        }
                        connection.Close();
                    }
                    ShowInfoMessage("User " + userName + " was deleted successfully.");
                    try {
                        string message = String.Format("User {0} with user subdirectory '{1}' was deleted successfully.\nIP Address: {2}\nWebsite: {3}", userName, userdir, System.Web.HttpContext.Current.Request.UserHostAddress, DbUse.DomainPath);
                        //MailInfo.SendMail(message, MailInfo.USER_DELETED);
                    } catch (Exception exp) {
                        //logFiles.ErrorLog(exp);
                    }
                    //logFiles.InfoLog(LogFiles.USER_DELETED, userName);
                } catch (Exception ex) {
                    //logFiles.ErrorLog(ex);
                    ShowErrorMessage("User files were deleted successfully but an error has occured while deleting database data. Please contact the website administrator.");
                }
            } else {
                ShowErrorMessage("Cannot delete all user data files and directories. Please contact the website administrator.");
            }
        } catch (Exception exp) {
            //logFiles.ErrorLog(exp);
            ShowErrorMessage("An internal error has occured. Invalid user data.");
            ShowErrorMessage(exp.Message);
        }
        hdnUserId.Value = "";
    }



    protected void SetUserGridData() {
        try {
            string selectQuery = "SELECT userlist.id, userlist.username, userlist.usersub, usercred.mail, userlist.company FROM userlist INNER JOIN usercred ON userlist.id = usercred.id WHERE userlist.username <> 'admingla';";
            using (OdbcConnection connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString)) {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(selectQuery, connection))
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(command)) {
                    DataTable dt = new DataTable();
                    try {
                        adapter.Fill(dt);
                        dt.DefaultView.Sort = SortExpression;
                        gridUsers.DataSource = dt.DefaultView;
                        gridUsers.DataBind();
                    } catch (Exception ex) {
                        throw new Exception("Error in getting data using sql query: " + selectQuery + ". " + ex.Message, ex);
                    }
                }
                connection.Close();
            }

        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
            ShowErrorMessage("An error has occured when reading user data.");

        }
    }

    public void ShowErrorMessage(string message) {
        lblGeneralError.Text = message;
        modalExtenderError.Show();
        SetFocus(btnOkError.ClientID);
    }

    public void ShowInfoMessage(string message) {
        lblGeneralInfo.Text = message;
        modalExtenderInfo.Show();
        SetFocus(btnInfoOk.ClientID);
    }

    //public void SetFocus(string clientId) {
    //    string script = "<SCRIPT language='javascript' type='text/javascript'>function fnFocus() { eval(\"document.getElementById('" + clientId + "').focus()\") } setTimeout(\"fnFocus()\",200);</SCRIPT>";
    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
    //}

    protected void btnDeleteUser_Click(object sender, EventArgs e) {
        DeleteUser();
        SetUserGridData();
    }
    protected void btnCancelDeleteUser_Click(object sender, EventArgs e) {
        hdnUserId.Value = "";
    }
    protected void gridUsers_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        gridUsers.PageIndex = e.NewPageIndex;
        SetUserGridData();
    }


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
        SetUserGridData();
    }
    protected void btnResetSort_Click(object sender, EventArgs e) {
        SortExpression = "";
        dropListSorting.SelectedIndex = 0;
        lstRdbtnOrder.SelectedIndex = 0;
        SetUserGridData();
    }
}