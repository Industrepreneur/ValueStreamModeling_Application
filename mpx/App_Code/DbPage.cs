using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using ADODB;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using DAO;

/// <summary>
/// Summary description for DbPage
/// </summary>
public partial class DbPage : System.Web.UI.Page
{

    protected string PAGENAME;

    protected string userDir;

    private string CURRENT_VERSION = "Version vsm 1.09";

    protected const string USERNAME_DATABASE = "Database22.mdb";

    protected const string CURRENT_DATABASE = "curr_mpx.mdb";

    public const string MAIN_USER_DATABASE = "mpxmdb.mdb";

    protected const string NEW_MODEL_DATABASE = "initial.mdb";

    protected const string DEMONSTRATION_DATABASE = "Gthubs.mdb";

    protected const string MPX_DLL = "mpx95i.dll";

    protected const string TABLE_NAMES = "zstblTableNames";
    protected const string FIELD_NAMES = "zstblFieldNames";

    protected const string CURRENT_PAGE_COOKIE = "current_page";

    protected const string LOGOUT_PAGE = "/login.aspx";

    protected const string LOGOUT_USERID = "none";

    protected const string DEFAULT_MPX_PAGE = "/models.aspx";

    protected string oldNewLogin = "";

    public const string BROWSER_DIR = "Charts";

    public const long NANOSEC_100_IN_MINUTE = 10 * 1000 * 1000 * 60;

    public const long TIMEOUT_IN_MINUTES = 30;

    protected ADODB.Connection conn = new ADODB.Connection();

    protected ADODB.Recordset rec = new ADODB.Recordset();

    protected OleDbConnection oleConn = new OleDbConnection();

    protected TableSynchronization tableSync;

    protected const string MODELS = "Models";

    protected const string OUTPUTS = "Outputs";

    protected bool cookiesEnabled;

    protected const string TBL_GENERAL = "tblgeneral";

    protected string username = "";
    protected string lastLogin = "";
    protected string logoutMessage = "";
    protected string currentModel = "none";

    protected LogFiles logFiles = new LogFiles();

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Master.GetCurrentModel().Equals("none") && !PAGENAME.Equals("/models.aspx"))
        {

            Response.Redirect(DEFAULT_MPX_PAGE, true);

        }

        SetCurrentPageInDb();

    }

    private string VERSION_FILE = "version.vsm";

    protected bool IsLastVsmVersion()
    {

        bool lastVersion = false;
        string pathToVersionFile = GetDirectory() + VERSION_FILE;
        string pathToUserVersionFile = GetDirectory() + userDir + VERSION_FILE;

        if (File.Exists(pathToVersionFile) && File.Exists(pathToUserVersionFile))
        {

            try
            {

                string userVersion = "0";
                string currentVersion = "-1";

                using (StreamReader versionFile = new StreamReader(pathToVersionFile))
                {
                    currentVersion = versionFile.ReadLine();
                }

                using (StreamReader userVersionFile = new StreamReader(pathToUserVersionFile))
                {
                    userVersion = userVersionFile.ReadLine();
                }

                lastVersion = (currentVersion.Equals(userVersion));

            }
            catch (Exception ex)
            {

            }
        }

        return lastVersion;

    }

    protected bool OpenModel(string userDb)
    {

        bool val = OpenModel(userDb, true);

        return val;

    }

    protected bool OpenModel(string userDb, bool showMessage)
    {
        string message;
        string newFileDir = GetDirectory() + userDir;
        string oldFileDir = newFileDir + MODELS + "\\";
        string oldPath = oldFileDir + userDb;
        string newPath = newFileDir + userDb;
        CloseAnalysis(); // close analysis if it is open

        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        Master.PassCurrentAnalysisName("");
        Master.SetCurrentAnalysisLabel();

        // SetCurrentModelInDb analyss, whatif in zstblstate = 0 !!!!
        UpdateSql("UPDATE zstblstate SET AnalysisID = 0;");
        UpdateSql("UPDATE zstblstate SET WID = 0;");

        if (!FileOperations.Copy_File(oldPath, newPath))
        {
            Master.ShowErrorMessage("Error in copying files.");
            return false;
            // make a copy of the file in the user directory
        }


        FileInfo currDbInfo = new FileInfo(newFileDir + CURRENT_DATABASE);
        if (currDbInfo.Exists)
        {
            try
            {
                File.Delete(currDbInfo.FullName); // delete the old CURRENT_DATABASE if it exists
            }
            catch (Exception ex)
            {
                logFiles.ErrorLog(ex);
                message = "Model '" + userDb + "' could not open because current model '" + Master.GetCurrentModel() + "' is in use by another process.";
                if (showMessage)
                {
                    Master.ShowErrorMessage(message);
                }
                return false;
            }
        }


        File.Move(newPath, currDbInfo.FullName); // rename the copy of the file to CURRENT_DATABASE
        if (!DbUse.InRunProcess(userDir) && !IsLastVsmVersion())
        {
            UpdateVersion();
        }



        int link = LinkTables();
        bool opened = (link == -1);
        if (link == -2) { return false; }; //xxx - Not a good MODEL!!! 
        bool cleanup = true;
        bool unitProblem = false;
        bool namesProblem = false;
        int dockStockDefRoutingFlag = 0;
        message = "Model '" + userDb + "' ";
        {
            ClassF classF = new ClassF(GetDirectory() + userDir);
            classF.setGlobalVar();
            try
            {
                dockStockDefRoutingFlag = classF.check_dock_stock();
            }
            catch (Exception ex)
            {
                logFiles.ErrorLog(ex);
            }
            try
            {
                unitProblem = classF.clean_model();    //  null in field ???
            }
            catch (Exception ex)
            {
                cleanup = false;
                logFiles.ErrorLog(ex);
            }
            namesProblem = !classF.set_names2ids();
            classF.Close();
            CalcClass.save_errors_results(classF, "", ""); // clear errors/result calculation messages
        }
        if (opened)
        {
            message += "opened successfully";
            if (!cleanup)
            {
                message += " but there are errors in the access database and the model might not work properly";
                if (namesProblem)
                {
                    message += ". Moreover, some labor, equipment, operation or product names are not set correctly";
                }
            }
            else if (unitProblem)
            {
                message += " but there were incorrect units in general table. The units have been replaced by defaults. Please go to general page if you wish to change the units";
                if (namesProblem)
                {
                    message += ". Moreover, some labor, equipment, operation or product names are not set correctly";
                }
            }
            else if (namesProblem)
            {
                message += " but some labor, equipment, operation or product names are not set correctly";
            }
            message += ". ";
            if ((dockStockDefRoutingFlag & CalcClass.DOCK_STOCK_MISSING) > 0)
            {
                message += "Default operations 'DOCK', 'STOCK' and 'SCRAP' were added for products missing these operations. ";
            }
            if ((dockStockDefRoutingFlag & CalcClass.NO_ROUTING) > 0)
            {
                message += "Default routing was added for products without any routings.";
            }

            SetCurrentModelInDb(userDb);
            this.Master.setModel(userDb);
            if (showMessage)
            {
                Master.ShowInfoMessage_Post(message);
            }
            return true;

        }
        else
        {
            message += " could not be opened.";
            ResetCurrentModelInDb();
            Master.ResetModel();
            if (showMessage)
            {
                Master.ShowErrorMessage_Post(message);
            }
            return false;
        }
    }

    protected void CopyVersionFile()
    {
        string pathToVersionFile = GetDirectory() + VERSION_FILE;
        string pathToUserVersionFile = GetDirectory() + userDir + VERSION_FILE;

        try
        {
            File.Copy(pathToVersionFile, pathToUserVersionFile, true);
        }
        catch (Exception ex)
        {
            logFiles.ErrorLog(ex);
        }
    }

    protected void UpdateVersion()
    {

        string modelFilePath = GetDirectory() + DbPage.MAIN_USER_DATABASE;
        string userModelFilePath = GetDirectory() + userDir + DbPage.MAIN_USER_DATABASE;
        try
        {
            File.Copy(modelFilePath, userModelFilePath, true);
        }
        catch (Exception ex2)
        {
            logFiles.ErrorLog(ex2);
        }
        userModelFilePath = GetDirectory() + userDir + DbPage.NEW_MODEL_DATABASE;
        modelFilePath = GetDirectory() + DbPage.NEW_MODEL_DATABASE;
        try
        {
            File.Copy(modelFilePath, userModelFilePath, true);
        }
        catch (Exception)
        {
        }
        CopyVersionFile();

    }

    protected void CloseAnalysis()
    {
        Master.PassCurrentAnalysisName("");
        Master.SetCurrentAnalysisLabel();
        UpdateSql("UPDATE zstblstate SET AnalysisID = 0;");
    }



    protected void CloseWhatif()
    {
        Master.PassCurrentWhatifName("");
        Master.SetCurrentWhatifLabel();
        UpdateSql("UPDATE zstblstate SET WID = 0;");
    }


    private int in_link_now = 0;

    protected int LinkTables()
    {
        int linked = -1;



        string databaseDir = GetDirectory() + userDir;
        string dbpath = databaseDir + MAIN_USER_DATABASE;
        string tablepath = databaseDir + CURRENT_DATABASE;

        string tblpath = ";DATABASE=" + tablepath + ";";
        DAO.Database dat = null;
        DAO.Recordset rec = null;
        DAO.TableDef table;
        int didnotlink = 0;

        try
        {
            DAO.DBEngine daoEngine = new DAO.DBEngine();
            dat = daoEngine.OpenDatabase(dbpath, false, false, "");
            rec = dat.OpenRecordset(TABLE_NAMES, DAO.RecordsetTypeEnum.dbOpenDynaset);


            rec.MoveFirst();
            while (!rec.EOF)
            {
                try
                {
                    string tblname = rec.Fields["tablename"].Value.ToString();
                    try
                    {
                        dat.Execute("DROP TABLE " + tblname);
                    }
                    catch (Exception ex)
                    {
                        string str2 = ex.Message;
                        if (str2.Contains("Unrecognized database format") == true)
                        {
                            try { rec.Close(); }
                            catch (Exception) { }  //  error  double run to catch !!!
                            try { dat.Close(); }
                            catch (Exception) { }
                            return -2;
                        }
                    }
                    table = dat.CreateTableDef(tblname);
                    table.Connect = tblpath;
                    table.SourceTableName = tblname;
                    dat.TableDefs.Append(table);
                    didnotlink = -1;
                }
                catch (Exception ex)
                {
                    logFiles.ErrorLog(ex);
                    string str2 = ex.Message;
                    if (str2.Contains("Unrecognized database format") == true)
                    {
                        try { rec.Close(); } catch (Exception) { }  //  error  double run to catch !!!
                        try { dat.Close(); } catch (Exception) { }
                        return -2;
                    }

                    //linked = false;
                }

                rec.MoveNext();
            }


        }
        catch (Exception ex)
        {
            logFiles.ErrorLog(ex);
            if (didnotlink == 0) linked = -1; else linked = 0;
        }
        finally
        {
            try { rec.Close(); } catch (Exception) { }
            try { dat.Close(); } catch (Exception) { }
        }

        if (in_link_now == 0)
        {
            string str_version = GetDatabaseField("version", "tblgeneral");

            if (str_version == "-1")
            {
                str_version = "FILE IS NOT READABLE ACCESS DATABASE i.e. NOT MPX or VSmodelling file.";
                return 0;
            }

            /* can not read tblgeneral ...    if (str_version == null)
            {
                str_version = "FILE IS NOT READABLE ACCESS DATABASE i.e. NOT MPX or VSmodelling file.";
                return false;
            } */

            if (str_version != CURRENT_VERSION)
            {
                buildall();
                update_version();
                in_link_now = -1;
                linked = LinkTables();


                //  add records ifnone ...
                add_new_rec();
                in_link_now = 0;
            }
        }

        return linked;
    }



    private void update_version()
    {

        CalcClass calcClass = new CalcClass(GetDirectory() + userDir + "mpxmdb.mdb");
        string str1 = " UPDATE tblgeneral SET tblgeneral.Version = '" + CURRENT_VERSION + "';";
        calcClass.runsql(str1);
        return;
    }


    public DAO.DataTypeEnum string2Enum(string fieldtype)
    {
        fieldtype = fieldtype.ToLower();

        if (fieldtype == "long")
        {
            return DAO.DataTypeEnum.dbLong;
        }
        if (fieldtype == "single")
        {
            return DAO.DataTypeEnum.dbSingle;
        }
        if (fieldtype == "memo")
        {
            return DAO.DataTypeEnum.dbMemo;
        }
        if (fieldtype == "counter")
        {
            return DAO.DataTypeEnum.dbLong;
        }
        if (fieldtype == "date")
        {
            return DAO.DataTypeEnum.dbDate;
        }
        if (fieldtype == "integer")
        {
            return DAO.DataTypeEnum.dbInteger;
        }
        if (fieldtype == "text")
        {
            return DAO.DataTypeEnum.dbText;
        }
        if (fieldtype == "yesno")
        {
            return DAO.DataTypeEnum.dbBoolean; // Boolean;
        }

        return DAO.DataTypeEnum.dbLong;

    }


    public void add_table_field(DAO.Database datCurr, DAO.TableDef tableA, DAO.Recordset reccust)
    {

        DAO.Field fieldA;
        string strfieldA;
        DAO.DataTypeEnum typeA;
        string str1;
        string str2;

        // tableA = datCurr.CreateTableDef(tablename);  done
        //  add field then append table next!
        try
        {
            strfieldA = reccust.Fields["fieldname"].Value.ToString();
            str1 = reccust.Fields["fieldtype"].Value.ToString();
            typeA = string2Enum(str1);
            fieldA = tableA.CreateField(strfieldA, typeA);
            //  if field is then exception/return
        }
        catch (Exception ex)
        {
            return;
        }

        try
        {
            //  add other attributes here!!!!
            if (str1 == "counter")
                fieldA.Attributes = (int)DAO.FieldAttributeEnum.dbAutoIncrField;
            //  gwwd not available ? 
            /* 
               str2 = reccust.Fields["Allownonull"].Value.ToString();
               if (str2 == "true")
                   fieldA.Attributes = (int)DAO.FieldAttributeEnum.
            */
            tableA.Fields.Append(fieldA);
            //  if field is there  exception/return
        }
        catch (Exception ex)
        {
            return;
        }

        return;
    }





    public const short Labor = 0;
    public const short equip = 1;
    public const short product = 2;
    public const short oper = 3;
    public const short route = 4;
    public const short ibom = 5;
    public const short Whatif = 6;
    public const short Eq_type = 7;
    public const short General = 8;


    public void add_new_rec()
    {
        CalcClass calcClass = new CalcClass(GetDirectory() + userDir + "mpxmdb.mdb");
        ClassA classA = new ClassA(GetDirectory() + userDir);

        string str1;

        int id = classA.find_nameItem("none", 0, General, 0);

        if (id == 0)
        {

            str1 = "INSERT INTO tblgeneral ( Athr, Coef_v_Equip, Coef_v_Labor, Coef_v_Parts, RTU1b, RTU1c, Version, TUFor, TULT, TUProd,utlimit, Title ) " +
              " SELECT 'user' AS Expr1, 30 AS Expr2, 30 AS Expr3, 30 AS Expr4, 250 AS Expr9, 8 AS Expr10, '" + CURRENT_VERSION + "' AS Expr11, 'YEAR' AS Expr5, 'CALENDAR DAY' " +
              " AS Expr6, 'HOUR' AS Expr7, " +
              " 95 AS Expr8, 'New model ... ' AS Expr12;";

            calcClass.runsql(str1);
        }

        //  add new labor rec??
        int num1;


        num1 = classA.find_nameItem("none", 0, Labor, 0);

        if (num1 == 0)
        {
            str1 = "INSERT INTO tbllabor ( LaborDesc, LaborDept, LabComment,  GrpSiz, Setup, Run, Varbility, Abst, OT, PriorityShare ) " +
                   " SELECT  'none' AS Expr1, 'none' AS Expr10,' ' AS Expr12, -1 AS Expr2, 1 AS Expr3, 1 AS Expr4, 1 AS Expr5, 0 AS Expr6, 0 AS Expr7, True AS Expr8;";

            calcClass.runsql(str1);
        }

        int labid;

        num1 = classA.find_nameItem("none", 0, Labor, 0);
        labid = num1;

        num1 = classA.find_nameItem("none", 0, equip, 0);
        if (num1 == 0)
        {
            str1 = "INSERT INTO tblequip ( EquipDesc, EquipDept, eqcomment,  GrpSiz, OT, MTF, MTR, Setup, Run, Varbility, Labor, LaborDesc, EquipType, equiptypename ) " +
                 " SELECT 'none' AS Expr1, 'none' AS Expr10,' ' AS Expr11, -1 AS Expr2, 0 AS Expr3, 1 AS Expr4, 0 AS Expr5, 1 AS Expr6, 1 AS Expr7, 1 AS Expr8, " + labid + " AS Expr9, 'none' AS Expr10, " +
                 " 1 AS Expr11, 'Delay' AS Expr12 ;";

            calcClass.runsql(str1);
        }

        return;
    }

    public int buildall()
    {

        DBEngine daoEngine2 = new DAO.DBEngine();

        DAO.Database datCurr = daoEngine2.OpenDatabase(GetDirectory() + userDir + CURRENT_DATABASE, false, false, "");

        string str1 = "SELECT zstblfieldnames.tableName, zstblfieldnames.FieldName, zstblfieldnames.FieldType," +
           " zstblfieldnames.Allow_NO_null, zstblfieldnames.index, zstblfieldnames.NO_Duplicate, zstblfieldnames.defaultvalue, " +
           " zstblfieldnames.validaterule FROM zstblfieldnames ORDER BY zstblfieldnames.tableName, zstblfieldnames.FieldName;";
        string oldtable = "";
        string tablename = "";
        DAO.TableDef tableA;
        DAO.TableDef tableThere;
        DAO.Recordset reccust = null;

        DAO.Database dat;
        DAO.DBEngine daoEngine = new DAO.DBEngine();
        string dbpath = GetDirectory() + userDir + MAIN_USER_DATABASE;

        dat = daoEngine.OpenDatabase(dbpath, false, false, "");
        reccust = dat.OpenRecordset(str1, DAO.RecordsetTypeEnum.dbOpenDynaset);
        reccust.MoveFirst();
        while (!reccust.EOF)
        {



            tablename = reccust.Fields["tablename"].Value.ToString();
            if (oldtable != tablename)
            {

                //add tblgeneral add data 
                // add_table_field tblabor add none
                //  add tblequip add none 


                try
                {
                    tableThere = datCurr.TableDefs[tablename];
                    add_table_field(datCurr, tableThere, reccust);
                }
                catch (Exception ex)
                {
                    tableA = datCurr.CreateTableDef(tablename);
                    add_table_field(datCurr, tableA, reccust);
                    datCurr.TableDefs.Append(tableA);
                }

            }
            else
            {
                tableThere = datCurr.TableDefs[tablename];
                add_table_field(datCurr, tableThere, reccust);
            }

            oldtable = tablename;
            reccust.MoveNext();
        }
        return -1;
    }


    protected bool UnlinkTables()
    {
        bool unlinked = true;
        string dbpath = GetDirectory() + userDir + MAIN_USER_DATABASE;
        DAO.Database dat;
        DAO.Recordset rec;

        try
        {
            DAO.DBEngine daoEngine = new DAO.DBEngine();
            dat = daoEngine.OpenDatabase(dbpath, false, false, "");
            rec = dat.OpenRecordset(TABLE_NAMES, DAO.RecordsetTypeEnum.dbOpenDynaset);

            rec.MoveFirst();
            while (!rec.EOF)
            {
                try
                {
                    string tblname = rec.Fields["tablename"].Value.ToString();
                    try
                    {
                        dat.Execute("DROP TABLE " + tblname);
                    }
                    catch (Exception) { }

                }
                catch (Exception)
                {
                    unlinked = false;
                }
                rec.MoveNext();
            }
            rec.Close();
            dat.Close();
        }
        catch (Exception ex)
        {
            logFiles.ErrorLog(ex);
            unlinked = false;
        }
        return unlinked;

    }

    protected bool UpdateSql(string commandString, string cookieid)
    {
        return UpdateSql(commandString);
    }

    protected bool UpdateSql(string commandString)
    {
        bool updated;
        string connectionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + GetDirectory() + userDir + MAIN_USER_DATABASE;
        try
        {
            oleConn = new OleDbConnection(connectionString);
            oleConn.Open();
            OleDbCommand comm = new OleDbCommand(commandString, oleConn);
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw new Exception("Error in executing sql command. Command string: " + commandString + ". Exception message: " + exp.Message, exp);
            }
            updated = true;
        }
        catch (Exception ex)
        {
            logFiles.ErrorLog(ex);
            updated = false;
        }
        finally
        {
            try
            {
                oleConn.Close();
            }
            catch (Exception) { }
        }
        return updated;
    }

    protected string GetDirectory()
    {
        string dir = GetMainDirectory() + "App_Data\\";
        return dir;
    }

    protected string GetMainDirectory()
    {
        string dir = Server.MapPath("~");
        dir += "\\";
        return dir;
    }

    protected string GetBrowserDirectory()
    {
        return GetMainDirectory() + BROWSER_DIR;
    }

    protected bool WriteToDatabase(string newEntry, string field, string table, string cookieid)
    {
        string commandString = "UPDATE " + table + " SET " + field + " = '" + newEntry + "'; ";
        return UpdateSql(commandString, cookieid);
    }

    protected string GetMysqlDatabaseField(string field, string cookieid)
    {
        string entry;
        conn = new ADODB.Connection();
        rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdoMysql(conn);
        var mySession = HttpContext.Current.Session;
        string commandString = "SELECT " + field + " FROM userlist WHERE sessionid = '" + mySession.SessionID + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try
        {
            entry = rec.Fields[field].Value.ToString();
        }
        catch (Exception)
        {
            throw new Exception("Field '" + field + "' not found in the table '" + "userlist" + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    protected string GetMysqlDatabaseField(string field, int id)
    {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT " + field + " FROM userlist WHERE id = " + id + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
            throw new Exception("Error in opening database/dataset.");

        try
        {
            entry = rec.Fields[field].Value.ToString();
        }
        catch (Exception)
        {
            throw new Exception("Field '" + field + "' not found in the table '" + "userlist" + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string table)
    {
        string entry;

        ADODB.Connection conn = new ADODB.Connection();


        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened;
        try
        {
            adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        }
        catch (Exception ex)
        {
            return "-1";
        }
        string commandString = "SELECT " + field + " FROM " + table + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        try
        {
            if (!adoOpened || !adoRecOpened)
            {
                DbUse.CloseAdoRec(rec);
                DbUse.CloseAdo(conn);
                throw new Exception("Error in opening database/dataset.");

            }
        }
        catch (Exception ex)
        {

            return null;
        }

        try
        {
            entry = rec.Fields[field].Value.ToString();
        }
        catch (Exception ex)
        {
            logFiles.ErrorLog(ex);
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string key, string keyValue, string table)
    {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT [" + key + "], [" + field + "] FROM " + table + " WHERE [" + key + "] = '" + keyValue + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw new Exception("Error in opening database/dataset.");
        }

        try
        {
            entry = rec.Fields[field].Value.ToString();
        }
        catch (Exception)
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    public string GetDatabaseField(string field, string key, int keyValue, string table)
    {
        string entry;
        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT [" + key + "], [" + field + "] FROM " + table + " WHERE [" + key + "] = " + keyValue + ";";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
        if (!adoOpened || !adoRecOpened)
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw new Exception("Error in opening database/dataset.");
        }

        try
        {
            entry = rec.Fields[field].Value.ToString();
        }
        catch (Exception)
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
            throw new Exception("Field '" + field + "' not found in the table '" + table + "'.");
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);
        return entry;
    }

    protected bool OperExists(int prodId, string operName)
    {
        bool exists = false;
        try
        {
            string entry;
            bool adoOpened = DbUse.OpenAdo(conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
            string commandString = "SELECT OpNam FROM tbloper WHERE ProdFore = " + prodId + " AND OpNam = '" + operName + "';";
            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
            if (!adoOpened || !adoRecOpened)
            {
                DbUse.CloseAdoRec(rec);
                DbUse.CloseAdo(conn);
                throw new Exception("Error in opening database/dataset.");
            }
            entry = rec.Fields["OpNam"].Value.ToString();
            exists = true;
        }
        catch (Exception) { }
        finally
        {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
        }
        return exists;
    }

    //protected bool isValid()
    //{
    //    bool sessionValid = false;
    //    string mySessionID = HttpContext.Current.Session.SessionID;
    //    //HttpCookie cookie = Request.Cookies[DbUse.LOGIN_COOKIE];
    //    if (!HttpContext.Current.Session.IsNewSession)
    //    {
    //        if (this.username != "")
    //        {

    //            //string clientCookieID = MyUtilities.clean(cookie.Value);
    //            bool adoOpened = DbUse.OpenAdoMysql(conn);
    //            string commandString = "SELECT * FROM userlist WHERE username = '" + this.username + "';";
    //            bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);
    //            string oldSessionID = rec.Fields["sessionid"].Value.ToString();

    //            string oldCookieID = rec.Fields["userid"].Value.ToString();
    //            long expiresOn = long.Parse(rec.Fields["sessionexpires"].Value.ToString());

    //            if (mySessionID.Equals(oldSessionID) && DateTime.Now.Ticks < expiresOn)
    //            {
    //                sessionValid = true;
    //                //cookie.Expires = DateTime.Now.AddMinutes(20);
    //                DbUse.RunMysql("UPDATE userlist SET userlist.sessionexpires = '" + DateTime.Now.AddMinutes(20).Ticks + "' WHERE userlist.sessionid = '" + mySessionID + "' ; ");
    //                //NEED TO SET PAGE VARIABLES
    //            }

    //        }

    //        if (!PAGENAME.Equals("login.aspx") && sessionValid == false)
    //        {
    //            DbUse.LogoutUser();
    //        }

    //    }

    //    return sessionValid;
    //}

    //protected bool IsUserSessionValid()
    //{
    //    bool sessionValid = false;
    //    HttpCookie cookie = Request.Cookies[DbUse.LOGIN_COOKIE];

    //    if (cookie != null && !cookie.Value.Equals(DbUse.DEFAULT_COOKIE_ID))
    //    {

    //        string cookieid = MyUtilities.clean(cookie.Value);
    //        if (cookieid.Equals(String.Empty))
    //        {
    //            return false;
    //        }

    //        bool adoOpened = DbUse.OpenAdoMysql(conn);
    //        string commandString = "SELECT * FROM userlist WHERE userid = '" + cookieid + "';";
    //        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);

    //        try
    //        {
    //            long lastUpdate = long.Parse(rec.Fields["lastupdate"].Value.ToString());
    //            string username = rec.Fields["username"].Value.ToString();
    //            long sessionExpires = long.Parse(rec.Fields["sessionexpires"].Value.ToString());
    //            logFiles.SetUsername(username);

    //            //think this needs to change to check cookie expire date
    //            if (DateTime.Now.Ticks < sessionExpires)
    //            {

    //                sessionValid = true;
    //                lastUpdate = DateTime.Now.Ticks;
    //                string userDir = rec.Fields["usersub"].Value.ToString();
    //                string currentModel = rec.Fields["currentModel"].Value.ToString();
    //                string lastLogin = rec.Fields["lastLogin"].Value.ToString();
    //                string logoutMessage = rec.Fields["logoutMessage"].Value.ToString();
    //                oldNewLogin = rec.Fields["newLogin"].Value.ToString(); // keeping the newLogin in the class
    //                DbUse.CloseAdo(conn);
    //                DbUse.CloseAdoRec(rec);


    //                if (sessionValid)
    //                {
    //                    // session is still valid - set the user subdirectory, username and the name of the current model
    //                    string mySessionID = System.Web.HttpContext.Current.Session.SessionID;
    //                    cookie.Expires = DateTime.Now.AddMinutes(20);
    //                    DbUse.RunMysql("UPDATE userlist SET userlist.sessionid = '" + mySessionID + "', userlist.sessionexpires = '" + DateTime.Now.AddMinutes(20).Ticks + "' WHERE userlist.userid = '" + cookieid + "' ; ");
    //                    this.userDir = userDir;
    //                    this.username = username;
    //                    Session["username"] = this.username;
    //                    Session["user-directory"] = this.userDir;
    //                    this.logoutMessage = logoutMessage;
    //                    this.lastLogin = lastLogin;
    //                    this.currentModel = currentModel;

    //                    if (!PAGENAME.Equals("flash_diagrams.aspx") && !PAGENAME.Equals("login.aspx") && !PAGENAME.Equals("speciallog.aspx") && !PAGENAME.Equals("mpx_admin.aspx"))
    //                    {
    //                        this.Master.setUser(username);
    //                        this.Master.setUserdir(userDir);
    //                        this.Master.passCurrentModelName(currentModel);
    //                        this.Master.setLastLogin(lastLogin);
    //                        this.Master.setLogoutMessage(logoutMessage);
    //                        bool modelModified = GetModelModified();
    //                        if (currentModel.Equals("none"))
    //                        {
    //                            if (modelModified)
    //                            {
    //                                modelModified = false;
    //                                SetModelModified(modelModified);

    //                            }
    //                        }

    //                        this.Master.PassModelModified(modelModified);

    //                        bool isWhatif = IsWhatifMode();
    //                        if (isWhatif)
    //                        {
    //                            this.Master.PassCurrentWhatifName(GetCurrentWhatif());
    //                        }

    //                        bool isAnalysis = IsAnalysisMode();
    //                        if (isAnalysis)
    //                        {
    //                            this.Master.PassCurrentAnalysisName(GetCurrentAnalysis());
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {

    //                logoutMessage = "User session timeout.";
    //                WriteLogoutMessageToDb(logoutMessage);

    //                DbUse.LogoutUser();
    //                if (lastUpdate != 0)
    //                {
    //                    DateTime dateTime = new DateTime(lastUpdate);
    //                    logFiles.LogoutLog(dateTime);
    //                }
    //                Session["timeout"] = "true";
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logFiles.ErrorLog(ex);
    //        }

    //    }
    //    else
    //    {


    //    }
    //    return sessionValid;
    //}



    //protected bool LastLoginUpdate(string cookieid, string loginTime)
    //{
    //    return DbUse.RunMysql("UPDATE userlist SET newLogin = " + loginTime + " WHERE userid = '" + cookieid + "';");



    //}

    //protected void WriteLogoutMessageToDb(string logoutMessage)
    //{
    //    if (logoutMessage != null & !logoutMessage.Equals(String.Empty))
    //    {
    //        DbUse.WriteLogoutMessageToDb(logoutMessage);

    //    }
    //}

    //protected void WriteLoginTimesToDb(DateTime dtNewLogin, string oldLoginTime, string cookieid)
    //{
    //    DbUse.RunMysql("UPDATE userlist SET newLogin = '" + dtNewLogin.ToLocalTime() + "', lastLogin = '" + oldLoginTime + "' WHERE userid = '" + cookieid + "';");

    //}

    //protected string GetCookieId()
    //{
    //    string cookieid = DbUse.DEFAULT_COOKIE_ID;
    //    if (DbUse.CookiesEnabled())
    //    {
    //        HttpCookie cookie = Request.Cookies[DbUse.LOGIN_COOKIE];
    //        if (cookie != null)
    //        {
    //            cookieid = MyUtilities.clean(cookie.Value);
    //        }
    //    }
    //    return cookieid;
    //}

    protected bool TestReadFile(string path)
    {
        bool read;
        try
        {
            FileStream fs = File.Open(path, FileMode.Open);
            fs.Close();
            read = true;
        }
        catch (Exception)
        {
            read = false;
        }
        return read;
    }


    public new IMasterPageMPX Master
    {
        get { return base.Master as IMasterPageMPX; }
    }

    protected override void OnInit(EventArgs e)
    {
        string redirectPage = "";
        string dbSessionID = "";
        var mySession = HttpContext.Current.Session;
        string currentSessionID = mySession.SessionID;
        string myUsername = mySession["username"] as string;

        if (!PAGENAME.Equals("/timeout.aspx") && !PAGENAME.Equals("/logout.aspx"))
        {
            if (mySession.IsNewSession || String.IsNullOrEmpty(myUsername))
            {
                if (!PAGENAME.Equals("/login.aspx"))
                {
                    Response.Redirect("/login.aspx", true);
                }
            }
            else
            {               
                if (Sessionable.ValidateSession())
                {

                    string currentPage = HttpContext.Current.Request.Path.ToString();

                    if (currentPage.Equals("/login.aspx"))
                    {
                        Response.Redirect(DEFAULT_MPX_PAGE, true);
                    }
                    else if (currentPage.Equals(PAGENAME) && !currentPage.Equals("/mpx_admin.aspx"))
                    {

                        LoadDllPath();

                        //THIS SHOULD NOT BE NECESSARY, EVENTUALLY
                        this.userDir = Session["user-directory"].ToString();
                        this.Master.setUserdir(Session["user-directory"].ToString());

                        this.username = Session["username"].ToString();
                        this.Master.setUser(Session["username"].ToString());

                        this.currentModel = Session["basecase"].ToString();
                        this.Master.passCurrentModelName(Session["basecase"].ToString());

                    }
                    else
                    {
                        Response.Redirect(PAGENAME, true);
                    }
                }
                else
                {
                    if (!PAGENAME.Equals("/login.aspx"))
                    {
                        Response.Redirect("/login.aspx", true);
                    }

                }
            }
        }
        else
        {

        }

        base.OnInit(e);

    }

    //ORIGINAL
    //protected override void OnInit(EventArgs e)
    //{
    //    logFiles = new LogFiles();

    //    if (!IsUserSessionValid())
    //    {
    //        // user session is not valid - we only let the user get to the login or logout page
    //        if (!PAGENAME.Equals("login.aspx") && !PAGENAME.Equals("speciallog.aspx"))
    //        {

    //            DbUse.LogoutUser();

    //            //Response.Redirect(LOGOUT_PAGE, true);
    //            //Response.Redirect(LOGOUT_PAGE + "?timeout=true", true);
    //            //Session.Abandon();
    //            //Response.Redirect(LOGOUT_PAGE, true);


    //        }
    //        else
    //        {
    //            //logFiles.LoginLog("in login page - session not valid", DateTime.Now);
    //        }
    //    }
    //    else
    //    {
    //        // user session is valid
    //        logFiles.SetUsername(this.username);

    //        if (PAGENAME.Equals("login.aspx") || PAGENAME.Equals("speciallog.aspx"))
    //        {
    //            //logFiles.LoginLog("in login page - user session valid", DateTime.Now);
    //            // user is logged in - no need to continue with login page, redirect to mpx content immediately

    //            // delete from calc stuff

    //            string redirectPage = DEFAULT_MPX_PAGE;


    //            try
    //            {
    //                if (username.Equals("admingla"))
    //                {
    //                    redirectPage = "mpx_admin.aspx";
    //                }
    //                else
    //                {
    //                    HttpCookie lastPageCookie = Request.Cookies[DbUse.LASTPAGE_COOKIE];
    //                    if (lastPageCookie != null)
    //                    {

    //                        redirectPage = GetCurrentPage();

    //                    }
    //                }

    //            }
    //            catch (Exception) { }


    //            Response.Redirect(redirectPage, true);


    //        }
    //        else if (!PAGENAME.Equals("mpx_admin.aspx") && !PAGENAME.Equals("404.aspx") && !PAGENAME.Equals("error.aspx") && !PAGENAME.Equals("404_2.aspx") && !PAGENAME.Equals("error2.aspx"))
    //        {
    //            CheckForMainVsmFiles();
    //        }

    //        LoadDllPath();
    //    }


    //    base.OnInit(e);


    //}



    protected void SetCurrentModelInDb(string modelName)
    {
        DbUse.RunMysql("UPDATE userlist SET currentModel = '" + modelName + "' WHERE sessionid = '" + HttpContext.Current.Session.SessionID + "';");

    }

    protected void ResetCurrentModelInDb()
    {  //  gwwd  reread !!!!
        SetCurrentModelInDb("none");
        if (!UpdateSql("UPDATE zstblstate SET ModelModified = 0, AnalysisID = 0, currentProdId = -1, currentDiag = 0, verifyNeeded = 0 WHERE WID >= 0;"))
        {
            UpdateVersion();
        }


    }

    protected void ResetCurrentModel()
    {
        SetCurrentModelInDb("none");
        Master.setModel("none");
    }

    protected string GetCurrentPage()
    {
        return GetDatabaseField("currentPage", "zstblstate");

    }

    protected void SetCurrentPageInDb()
    {
        UpdateSql("UPDATE zstblstate SET currentPage = '" + PAGENAME + "';");
    }

    public static Control GetPostBackControl(Page page)
    {
        Control control = null;

        string ctrlname = page.Request.Params.Get("__EVENTTARGET");
        if (ctrlname != null && ctrlname != string.Empty)
        {
            control = page.FindControl(ctrlname);
        }
        else
        {
            foreach (string ctl in page.Request.Form)
            {
                Control c = page.FindControl(ctl);
                if (c is System.Web.UI.WebControls.Button)
                {
                    control = c;
                    break;
                }
            }
        }
        return control;
    }

    public static string GetAsyncPostBackControlID(Page page)
    {
        string smUniqueId = ScriptManager.GetCurrent(page).UniqueID;
        string smFieldValue = page.Request.Form[smUniqueId];

        if (!String.IsNullOrEmpty(smFieldValue) && smFieldValue.Contains("|"))
        {
            return smFieldValue.Split('|')[1];
        }

        return String.Empty;
    }

    protected void DeleteGraphs()
    {
        string graphsDirectoryPath = GetDirectory() + userDir + DbUse.GRAPHS_DIR;
        if (Directory.Exists(graphsDirectoryPath))
        {
            string[] files = Directory.GetFiles(graphsDirectoryPath);
            try
            {
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
            catch (Exception) { }

        }
        else
        {
            try
            {
                // creates the graphs directory again
                Directory.CreateDirectory(graphsDirectoryPath);
            }
            catch (Exception) { }
        }
        graphsDirectoryPath = GetMainDirectory() + BROWSER_DIR + "\\" + userDir + "Graphs";
        if (Directory.Exists(graphsDirectoryPath))
        {
            string[] files = Directory.GetFiles(graphsDirectoryPath);
            try
            {
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
            catch (Exception) { }

        }
        else
        {
            try
            {
                // creates the graphs directory again
                Directory.CreateDirectory(graphsDirectoryPath);
            }
            catch (Exception) { }
        }

    }

    private void LoadDllPath()
    {
        /*SetDllDirectory(GetDirectory());
        IntPtr dllHandle = LoadLibrary("webmpx95i.dll");
        if (dllHandle != IntPtr.Zero) {

        }*/
        string pathNew = GetMainDirectory() + "Bin\\";
        string currPaths = System.Environment.GetEnvironmentVariable("Path");
        if (!System.Environment.GetEnvironmentVariable("Path").Contains(pathNew))
        {
            try
            {
                System.Environment.SetEnvironmentVariable("Path", pathNew + ";" + System.Environment.GetEnvironmentVariable("Path"));
            }
            catch (Exception ex)
            {
                logFiles.ErrorLog(ex);
            }
        }


    }

    protected void SetModelModified(bool modified, bool allModified)
    {
        string model_modified = (modified) ? "-1" : "0";
        string queryString = "UPDATE zstblstate SET model_modified = " + model_modified + ";";
        UpdateSql(queryString);
        Master.PassModelModified(modified);
        Master.MarkSavedModel();

        int wid = Convert.ToInt32(GetDatabaseField("WID", "zstblstate"));

        if (modified == true && allModified == true)
        {

            if (wid == 0)
            {


                queryString = ("UPDATE tblWhatIf SET tblWhatIf.recalc = -1");
                UpdateSql(queryString);
                queryString = " UPDATE zs0tblWhatIf SET zs0tblWhatIf.recalc = -1  WHERE (((zs0tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);
            }
            else
            {

                queryString = " UPDATE tblWhatIf SET tblWhatIf.recalc = -1  WHERE (((tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);
            }

        }
        else if (modified == true && allModified == false)
        {
            if (wid == 0)
            {

                queryString = " UPDATE zs0tblWhatIf SET zs0tblWhatIf.recalc = -1  WHERE (((zs0tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);

            }
            else
            {

                queryString = " UPDATE tblWhatIf SET tblWhatIf.recalc = -1  WHERE (((tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);
            }
        }
        else
        {

        }




    }

    protected void SetModelModified(bool modified)
    {
        string model_modified = (modified) ? "-1" : "0";
        string queryString = "UPDATE zstblstate SET model_modified = " + model_modified + ";";
        UpdateSql(queryString);
        Master.PassModelModified(modified);
        Master.MarkSavedModel();


        if (modified == true)
        {

            int wid = Convert.ToInt32(GetDatabaseField("WID", "zstblstate"));

            if (wid == 0)
            {


                queryString = ("UPDATE tblWhatIf SET tblWhatIf.recalc = -1");
                UpdateSql(queryString);
                queryString = " UPDATE zs0tblWhatIf SET zs0tblWhatIf.recalc = -1  WHERE (((zs0tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);
            }
            else
            {

                queryString = " UPDATE tblWhatIf SET tblWhatIf.recalc = -1  WHERE (((tblWhatIf.WID)=" + wid + "));";
                UpdateSql(queryString);
            }
        }




    }

    protected bool GetModelModified()
    {
        bool modelModified = false;

        try
        {
            string modified = GetDatabaseField("model_modified", "zstblstate");
            int mod = int.Parse(modified);
            modelModified = mod != 0;
        }
        catch (Exception) { }
        return modelModified;
    }

    protected bool IsWhatifMode()
    {
        return GetWhatifMode() != 0;
    }

    protected bool IsAnalysisMode()
    {
        return GetAnalysisMode() != 0;
    }

    protected int GetAnalysisMode()
    {
        int mod = 0;
        try
        {
            string mode = GetDatabaseField("AnalysisID", "zstblstate");
            mod = int.Parse(mode);
        }
        catch (Exception) { }


        return mod;
    }

    protected int GetWhatifMode()
    {
        int mod = 0;
        try
        {
            string mode = GetDatabaseField("WID", "zstblstate");
            mod = int.Parse(mode);
        }
        catch (Exception) { }
        return mod;
    }

    protected string GetCurrentWhatif()
    {
        string whatifName = "";

        int mod;
        ClassE e1 = null;
        try
        {
            string mode = GetDatabaseField("WID", "zstblstate");
            mod = int.Parse(mode);
            e1 = new ClassE(GetDirectory() + userDir);
            whatifName = e1.get_widname(mod);
        }
        catch (Exception) { }
        if (e1 != null)
        {
            e1.Close();
        }
        return whatifName;
    }

    protected string GetCurrentAnalysis()
    {
        string analysisName = "";
        ClassE e1 = null;
        try
        {
            int mod = GetAnalysisMode();
            e1 = new ClassE(GetDirectory() + userDir);
            analysisName = e1.GetAnalysisName(mod);
        }
        catch (Exception)
        {
            if (e1 != null)
            {
                e1.Close();
            }
        }

        return analysisName;
    }

    protected bool LastPostbackDbUpdate(string cookieid, long updateTime)
    {
        return DbUse.LastPostbackDbUpdate(cookieid, updateTime);
    }

    protected string GetUniqueName(string namefield, string idfield, string tblname, string origName)
    {
        string uniqueName = origName;
        bool done = false;
        do
        {
            try
            {
                string id = GetDatabaseField(idfield, namefield, uniqueName, tblname);
                uniqueName += "_COPY";
            }
            catch (Exception)
            {
                done = true;
            }
        } while (!done);
        return uniqueName;
    }

    protected void CheckForMainVsmFiles()
    {
        string mainMpxFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        string linkedMpxFile = GetDirectory() + userDir + CURRENT_DATABASE;
        string initialMpxFile = GetDirectory() + userDir + NEW_MODEL_DATABASE;
        string defInitialMpxFile = GetDirectory() + NEW_MODEL_DATABASE;

        try
        {
            if (!File.Exists(NEW_MODEL_DATABASE))
            {
                File.Copy(defInitialMpxFile, initialMpxFile, true);
            }
        }
        catch (Exception) { }
        try
        {
            //ONE OF THESE IS CAUSING THE 404
            if (!File.Exists(mainMpxFile))
            {

                UpdateVersion();
                ResetCurrentModel();

                if (!PAGENAME.Equals("models.aspx"))
                {

                    Response.Redirect("/models.aspx", true);

                }
            }

            else if (!currentModel.ToLower().Equals("none") && (!File.Exists(linkedMpxFile)))
            {

                ResetCurrentModel();

                if (!PAGENAME.Equals("models.aspx"))
                {

                    Response.Redirect("/models.aspx", true);

                }
            }
        }

        catch (Exception) { }
    }


    protected bool TablesLinked()
    {
        if (!UpdateSql("SELECT LaborID FROM tblLabor;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT EquipID FROM tblEquip;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT ProdID FROM tblProdfore;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT GeneralID FROM tblGeneral;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT OpID FROM tblOper;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT RecID FROM tblOperFrTo;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT IbomID FROM tblibom;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT ProdID FROM tblPictures;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblWhatif;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT oldshow FROM tblWhatifAudit;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblRsLabor;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblRsEquip;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblRsSummary;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblRsOper;"))
        {
            return false;
        }
        if (!UpdateSql("SELECT WID FROM tblRsProd;"))
        {
            return false;
        }
        return true;
    }

    protected void ResetModelGoToModels()
    {
        //Master.ResetModel();
        Response.Redirect("models.aspx", true);
    }

    protected virtual bool LocalTablesLinked()
    {
        return true;
    }



}