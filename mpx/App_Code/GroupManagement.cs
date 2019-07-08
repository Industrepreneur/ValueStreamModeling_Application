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



/*delet
//--------------------------------------------------------------------
  //   API LIST 
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
 * 
 * runmysql(string)  to run sql again a database ...
//--------------------------------------------------------------------------


//  NOT DONE YET !!!
//--------------------------------------------------------------------------
//  add model to group
//  copy file to user
// comments from files ? 
// comments to files 

//------------------------------------------------
 needed
int run_group_sql(str1)
val = classb1.testread(str1);  see if file exists ...
 * 
 * 

//  done ---------------------------------------------------------------- 
 * 
 *   unique asks   is this name is use ? 
 *    e.g. group name, filename in group,  user name in group (already),  etc. 
 *    
 public    int unique_group_name( string newname) 
public int unique_user_name(int groupid, string newname) 
public int unique_group_filename( int groupid, string newname) 
public int has_group_leader( int groupid, string username) 
public string unique_user_filename( string userdir, string newname)   //  null if name is ok else return new name 
public int is_file_owner( int groupid, string username, int model_id) 
public int add_new_group( string groupname) 
public int add_user_to_group( int groupid, string username, string newuser, int leader) 
public int delete_user_from_group ( int groupid, string username, string del_name) 
public int delete_model_from_group( int model_name_id, int groupid, string user_name) 
public int delete_group(int groupid, string username) 
public int add_model_file(string filedir, string filename, string username, int groupid) 
 * 

 * 
 * 
//------------------------------------------------

 *   sql for binding 
 *   
//  get list of groups for user    
//  get list of users in group + leader status...
//  get list of models in group
//  query to get all groups a username in   + "all" 


//------------------------------------------------

*/


/// <summary>
/// Summary description for Class1
/// </summary>
/// : DbPage {
public class GroupManagement {
    //  public class Class1
    private string _appDataDirPath;
    LogFiles logFiles;

    public GroupManagement(string appDataDirPath, string userName) {
        _appDataDirPath = appDataDirPath;
        logFiles = new LogFiles(userName);
    }



    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int find_group_id(string groupname) {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        string id1;
        id1 = "0";

        if (groupname == "") return 0;

        try {
            id1 = DbUse.GetMysqlDatabaseField("Group_id", "Group_name", groupname, "group_list");
        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return 0;  //  not found 
            }
        }
        int i_id;
        i_id = Convert.ToInt32(id1);
        return i_id;
    }


    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public string find_group_name(int groupid)
    {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        string name1;
        name1 = "";

        try
        {
            name1 = DbUse.GetMysqlDatabaseField("Group_name", "Group_id", groupid, "group_list");
        }
        catch (Exception ex)
        {
            if (ex.Message.IndexOf("Group ID not found") >= 0)
            {
                return "";  //  not found 
            }
        }
        
        return name1;
    }


    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public string get_groupname_fileid(int fileid)
    {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        string name1;
        name1 = "";

        try
        {
            name1 = DbUse.GetMysqlDatabaseField("Groupname", "file_id", fileid, "group_files");
        }
        catch (Exception ex)
        {
            if (ex.Message.IndexOf("Group nameD not found") >= 0)
            {
                return "";  //  not found 
            }
        }

        return name1;
    }/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int unique_user_name(string groupname, string newname) {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        int ret = 0;
      //  try
      //  {

            string entry;
            ADODB.Connection conn = new ADODB.Connection();
            ADODB.Recordset rec = new ADODB.Recordset();

            bool adoOpened = DbUse.OpenAdoMysql(conn);


            string commandString = "SELECT `webmpx`.Group_members.Username, `webmpx`.Group_members.Groupname FROM `webmpx`.Group_members WHERE (((`webmpx`.Group_members.Username)='" + newname + "') AND ((Group_members.Groupname)='" + groupname + "')) ;";

            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
            if (!adoOpened || !adoRecOpened)
                throw new Exception("Error in opening database/dataset.");
            ret = -1;  //  unique name ok 

            try {
                entry = rec.Fields["Username"].Value.ToString();
                ret = 0;  //  not unique  name is there...
              } catch (Exception) {
                  //  is unique and ret is set 
                  ret = -1;
           }  //  end try catch
            DbUse.CloseAdo(conn);
            DbUse.CloseAdoRec(rec);
            return ret;

 /*       } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return 0;
            }
        }  //  end try-catch
  * */
       // return 0;
    }  //  end routine


    
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  
    public int  flipLeader(string GroupName, string username, string leadername)
    {
        if (has_group_leader(GroupName, username) ==0) return 0;

        int curval = has_group_leader(GroupName, leadername);
        
       
        string newval;
     
        if ( curval ==0 ) newval = "yes"; else newval = "no" ;

       
        if (!DbUse.RunMySqlParams("update `webmpx`.group_members SET `webmpx`.group_members.`leader` = '" + newval +"'  WHERE  `webmpx`.group_members.`Username` = '" + leadername + "'  and  `webmpx`.group_members.`Groupname` = '" + GroupName +"'  ;",        
                new string[] {  }, new object[] {  }))
        {
            return 0;
        }

        return -1;
    }
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  

   

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////       
    public int has_group_leader(string groupname, string username)
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    {


        try {

            string entry;
            ADODB.Connection conn = new ADODB.Connection();
            ADODB.Recordset rec = new ADODB.Recordset();

            bool adoOpened = DbUse.OpenAdoMysql(conn);



            string commandString = "SELECT `webmpx`.group_members.* FROM   `webmpx`.group_members  WHERE (((`webmpx`.group_members.Groupname)='" + groupname + "') AND ((`webmpx`.group_members.Username)='" + username + "')  and (`webmpx`.Group_members.Leader = 'yes')  ); ";

            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
            if (!adoOpened || !adoRecOpened)
                throw new Exception("Error in opening database/dataset.");

            try {
                entry = rec.Fields["leader"].Value.ToString();
            } catch (Exception) {
                throw new Exception("Field  username  leader  not found in the table  group_members.");
            }  //  end try
            DbUse.CloseAdo(conn);
            DbUse.CloseAdoRec(rec);

        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return 0;
            }
        }  //  end try
        return -1;  //  group leader set ='yes'
    }  //  end routine

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public string unique_user_filename(string newname, string userDir)
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    {

        string str1;
        bool done;
        int ok;
       // ClassB classB = new ClassB(userDir);

        str1 = userDir + "\\" + newname;
        done = false;

        while (done == false) {
            ok = testread_dup(str1);  //classB.testread(str1);  // to do  add new new for filwe ...

            if (ok == 0)
                done = true;
            else {
                newname =  create_new_name(newname);
                str1 = userDir + "\\" + newname;
            }
        }
        return str1;
    }

    ///////////////////////////////////////////
    public short testread_dup(string str1)
    ///////////////////////////////////////////
    {
        //on error GoTo err1;
        short retval;

        retval = 0;

        if (!File.Exists(str1))
        {
            return 0;  //..  no File exists 
        }

        retval = -1;
        //exit1:
        return -1; //exit  Function;
        

    } // end function;


    ///////////////////////////////////////////
    public string create_new_name(string str1)
        ///////////////////////////////////////////
    {
        string str2;
        str2 = "a_" + str1;
        return str2;
    }

    ////// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int is_file_owner(int groupid, string username, int model_id)
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
{


        string name1;
        name1 = DbUse.GetMysqlDatabaseField("File_owner", "file_id", model_id, "group_files"); 
         if (name1 == username) 
            {
             return -1;
         }

        return 0;  // is file owner
    }  //  end routine





    // Add group
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int add_new_group(string groupname, string username ) {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        int val = 0;
        //  test if name in use 
        if (find_group_id(groupname) != 0)
        { return 0; }  //  group name in use !!!
        // add new group 

        //////////////////////////////////////////////////////////////////

        if (!DbUse.RunMySqlParams("INSERT INTO webmpx.group_list (Group_name) VALUES (@groupname)",
                new string[] { "@groupname" }, new object[] { groupname })) {
            return 0;
        }

        //  add 1st user to group 
        //  add test if user in is user list !!!!
        if (UsernameExists(username) == false)
        { return -1; }

      

        if (!DbUse.RunMySqlParams("INSERT INTO webmpx.group_members (Username, Groupname, leader) VALUES ( @username, @groupname, @leader)",
               new string[] { "@username", "@groupname", "@leader" }, new object[] { username, groupname, "yes" }))
        {
            return -2;
        }


        return 1;
    }

    ///////////////////////////////////////////
    public string get_filename(int groupid, int fileid) {
        ///////////////////////////////////////////

        string fname;

        fname = "";
        try {
            fname = DbUse.GetMysqlDatabaseField("File_name", "file_id", fileid, "group_files");
        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return "";  //  not found 
            }
        }

        return fname;
    }

    
            ///////////////////////////////////////////
public int get_file(int fileId, int groupid, string filePath, string username)
            ///////////////////////////////////////////
    {
        string nfilename;
        string fuldir;

         //  get file with fileid from groupid  
        nfilename = get_filename(groupid, fileId);
    

        //  check for dupl name (add a_ at front if file exists already)
        fuldir  = unique_user_filename(nfilename, filePath);

        //  place file into userdir
       

        using (MySqlConnection conn = new MySqlConnection(DbUse.GetConnectionString()))
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT File_stuff From Webmpx.group_files WHERE file_id = @fileId;", conn))
            {
                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@fileId", fileId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        byte[] fileBytes = (byte[])reader["File_stuff"];
                        File.WriteAllBytes(fuldir, fileBytes);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    // show error message...
                    LogFiles logFiles = new LogFiles();
                    logFiles.ErrorLog(ex);
                    return 0;
                }
            }
        }

        return -1;

        }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int update_file_comment(int fileid, string cstr1) {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (!DbUse.RunMySqlParams("UPDATE webmpx.group_files SET webmpx.group_files.Comments = '@comment' WHERE (((group_files.[file_id])= @file_id));",
                new string[] { "@comment", "@file_id" }, new object[] { cstr1, fileid })) {
            return 0;
        }

        return (-1);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     public int add_model_file(string filePath, string fileName, string fileOwner, int groupid) 
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    {

        try
        {
            byte[] rawData = File.ReadAllBytes(filePath+ "\\" + fileName);
            int fileSize = rawData.Length;

            // other way to get a file to byte array
            //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //long fileSize2 = fs.Length;
            //rawData = new byte[fileSize];
            //fs.Read(rawData, 0, (int)fileSize2);
            //fs.Close();

            string groupname;
            groupname= find_group_name(groupid);
            DateTime DateC = DateTime.UtcNow;

            DbUse.RunMySqlParams("INSERT INTO webmpx.group_files (File_name, File_stuff, File_size, File_owner, Group_id, Groupname, Date_created) VALUES (@FileName, @File, @FileSize, @File_owner, @Group_id, @Groupname, @DateC);",
                new string[] { "@FileName", "@File", "@FileSize", "@File_owner", "@Group_id", "@Groupname", "@DateC" },
                new object[] { fileName, rawData, fileSize, fileOwner, groupid, groupname, DateC });
        }
        catch (Exception ex)
        {
            // show error message...
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
            return 0;
        }

        return -1;

    }


    /// ///////////////////////////////////////////////////////////
    public bool UsernameExists(string username)
        /// ///////////////////////////////////////////////////////////
    {
        try {
            string id = DbUse.GetMysqlDatabaseField("id", "username", username, "userlist");
        } catch (Exception ex) {
            if (ex.Message.IndexOf("not found") >= 0) {
                return false;
            }
        }
        return true;
    }

    // add user to group 
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int add_user_to_group(string groupname, string username, bool leader, string adding_user) {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // test for new name 
        if (unique_user_name(groupname, username) == 0) { return 0; };

        // test if nadding user has leader authority in group
        if (has_group_leader(groupname, adding_user) == 0)
            return 0;

        //  add test if user in is user list !!!!
        if (UsernameExists(username) == false)
            return 0;

      
        string slead;

        if (leader==false) 
            slead = "no"; 
        else slead = "yes";
        /////////////////////////////////////////

        if (!DbUse.RunMySqlParams("INSERT INTO webmpx.group_members (Username, Groupname, leader) VALUES ( @username, @groupname, @leader)",
                new string[] { "@username", "@groupname", "@leader" }, new object[] { username, groupname, slead })) {
            return 0;
        }

        //  mail to usr 
        try {
            string mail = DbUse.GetEmailAddress(username); // GREG
            MailInfo.SendMail("You were invited to Value Stream Modeling website group. Please visit group models page." , MailInfo.GROUP_USER_ADDED, mail);
        } catch (Exception ex) { }

        return -1;
    }


    //  check file powner ...

    //delete user from group
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int delete_user_from_group(int groupid, string username, string del_name)
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
          {
        int val = -1;

        string groupname = find_group_name(groupid);

        //  check if user has group leader authority
        if (has_group_leader(groupname, username) == 0)
            return 0;

        // check if del_name is in group
        if (unique_user_name(groupname, del_name) != 0)
            return (0);

    
        string deleteQuery = " DELETE webmpx.group_members.* FROM webmpx.group_members WHERE (((Group_members.Username)= '"+ del_name + "' ) AND ((Group_members.Groupname)= '"+ groupname +"'  )); ";
        if (!DbUse.RunMySqlParams(deleteQuery, new string[] {}, new object[] { })) {
            return 0;
        }

         

        return val;
    }


    //get list of groups for user

    //  get list of models in group

    // int delete model from group
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int delete_model_from_group(int model_name_id, int groupid, string user_name)
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         {
        int val = 0;

        string groupname = get_groupname_fileid ( model_name_id);

        //  check user_name for authority 
        if (has_group_leader(groupname, user_name) == 0) {
            // else check if owner 
            if (is_file_owner(groupid, user_name, model_name_id) == 0)
                return 0;
        }

        // delete model from models list
        //xxquery

        string deleteQuery = "DELETE webmpx.group_files.* From Webmpx.group_files WHERE (Group_files.file_id=  @file_id);";
        //string deleteQuery = "DELETE  webmpx.group_files.File_owner, webmpx.group_files.[file_id], webmpx.group_files.* From Webmpx.group_files WHERE (((group_files.Groupname)= '"+ groupname +"' ) AND ((group_files.File_owner)= '" + user_name + "' ) AND ((group_files.file_id)= '"+ model_name_id +"' ));";


        DbUse.RunMySqlParams(deleteQuery, new string[] { "@file_id" }, new object[] { model_name_id });

            
        return -1;
        
    }



    //  delete group
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int delete_group(string groupname, string username) {
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        int val;

        val = -1;

     
        int groupid = find_group_id(groupname);

        //  check check if leader !!!
        //  check if user has group leader authority
        if (has_group_leader(groupname, username) == 0)
            return 0;

        try {
            string message = String.Format("User {0} deleting group '{1}' .\n", username, System.Web.HttpContext.Current.Request.UserHostAddress, DbUse.DomainPath);
            //  mail to usr , all memebers of group and admin
            // TODO add mail addresses 
            MailInfo.SendMail(message, MailInfo.GROUP_DELETED);
        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
        }

        // delele membrs list 
        string deleteQuery;
    
        if (!DbUse.RunMySqlParams("DELETE webmpx.group_members.* FROM webmpx.group_members WHERE (((Group_members.Groupname)= '"+ groupname+ "' ));",
                new string[] { }, new object[] {  })) {
            return 0;
        }
           


     //   xxx  delete members ? 
     //       xx  flipLeader 
     //           delete username from group

        //delete files
        deleteQuery = "DELETE webmpx.group_files.*  From Webmpx.group_files WHERE (((group_files.Groupname)= '" + groupname + "' ));";
        DbUse.RunMySqlParams(deleteQuery, new string[] {  }, new object[] { });
        

        //delete group 
        deleteQuery = "DELETE webmpx.group_list.* FROM webmpx.group_list WHERE (((Group_list.Group_id)= @groupid ));";
        DbUse.RunMySqlParams(deleteQuery, new string[] { "@groupid" }, new object[] { groupid });
        



        try {
            string message = String.Format("User {0} deleted group '{1}' successfully.\n", username, System.Web.HttpContext.Current.Request.UserHostAddress, DbUse.DomainPath);
            MailInfo.SendMail(message, MailInfo.GROUP_DELETED);
        } catch (Exception exp) {
            logFiles.ErrorLog(exp);
        }



        return val;
    }


}  // end of class 
//--------------------------------------------------------------------------
// QUERY FOR USE IN BINDING 

//--------------------------------------------------------------------------
/*
 * 
 * 
 *     protected void SetUserGridData() {}  in mpx_admin.aspx.cs  top bind data & has select query !!!!
 * 
 * 
    //get list of groups for user  (group list pull down in groups page)
str1 = " SELECT Group_list.Group_name, Group_members.Username, Group_list.Group_id FROM Group_members INNER JOIN Group_list ON Group_members.GroupID = Group_list.Group_id WHERE (((Group_members.Username)='"+ user_name + "'));";

 * 
//  query to get all groups a username is in   + "all"  (group list pill down in modelspage)
str1 = " SELECT Group_list.Group_id, Group_list.Group_name FROM Group_members INNER JOIN Group_list ON Group_members.GroupID = Group_list.Group_id WHERE (((Group_members.Username)='" + username +"')) UNION SELECT ggrouo_list_all.Group_id, ggrouo_list_all.Group_name FROM ggrouo_list_all; ";

 * 
//  get list of users in group + leader status...  (table to edit in grouos page)
    str1 = " SELECT Group_members.Username, Group_members.leader, Group_members.GroupID FROM Group_members WHERE (((Group_members.GroupID)=" + groupid + "));";


//  get list of models in group  (list of models from 1 group)
str1 = " SELECT Group_files.GroupID, Group_files.* From Webmpx.Group_files WHERE (((Group_files.GroupID)= " + groupid + "));";


//get list of all models in all user member groups (groupid = 0 <=> all) 
str1 = "SELECT Group_members.Username, Group_files.* FROM Group_members INNER JOIN Group_files ON Group_members.GroupID = Group_files.GroupID WHERE (((Group_members.Username)='" +username + "')); ";


   */