using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web;

public abstract class Calculate
{
    private const string MAIN_USER_DATABASE = "mpxmdb.mdb";

    public static bool isReCalcNecessary(string username, string path)
    {
        Boolean resultMessage;


        if (isCalculating(path))
        {
            resultMessage = false;
        }
        else
        {



            ClassF classE1_1 = new ClassF(path);
            classE1_1.Open();

            try
            {

                int totalCalc = 0;

                classE1_1.username = username;
                classE1_1.SetBasicModelInfo();
                classE1_1.calc_return = 0;     //0 - none, 1 labor, 2 eq over util, 4 warnings 8 errors 
                ADODB.Recordset recNeedCalc = new ADODB.Recordset();
                //check if basecase needs recalc and add to totalCalc
                DbUse.open_ado_rec(classE1_1.globaldb, ref recNeedCalc, "SELECT * FROM zs0tblWhatIf WHERE zs0tblWhatIf.WID=0;");
                int basecaserecalc = Convert.ToInt32(recNeedCalc.Fields["recalc"].Value);
                if (basecaserecalc != 0)
                {
                    totalCalc++;

                }


                bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE FamilyID = 0 AND recalc = true AND display = -1;");

                while (!recNeedCalc.EOF)
                {
                    totalCalc++;
                    recNeedCalc.MoveNext();
                }
                DbUse.CloseAdoRec(recNeedCalc);


                if (totalCalc > 0)
                {


                    resultMessage = true;



                }
                else
                {
                    resultMessage = false;

                }


            }
            catch (Exception)
            {
                resultMessage = false;
            }
            classE1_1.Close();

        }
        return resultMessage;

    }

    public static bool isCalculating(string userDirectoryPath)
    {
        bool inRun = false;

        string pathToRunFile = userDirectoryPath + CalcClass.RUN_FILE;
        if (System.IO.File.Exists(pathToRunFile))
        {

            try
            {
                using (System.IO.StreamReader runFile = new System.IO.StreamReader(pathToRunFile))
                {
                    string runTime = runFile.ReadLine();
                    long timeElapsedMins = (DateTime.Now.Ticks - long.Parse(runTime)) / DbPage.NANOSEC_100_IN_MINUTE;
                    if (timeElapsedMins < 5)
                    {
                        inRun = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //string cookieName = DbUse.RUN_COOKIE + username;
        //HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[cookieName];
        //if (cookie != null) {
        //    inRun = true;
        //}
        return inRun;
    }

    public static string RunNecessary(string username, string path, string sessionID)
    {
        string resultMessage;
        var resultArray = new List<string[]>();

        if (isCalculating(path))
        {
            resultMessage = "Calculations are still in process from the previous run, please wait";
        }
        else
        {
            int ret;

            DbUse.CreateRunFile(path, username);
            ClassF classE1_1 = new ClassF(path);
            classE1_1.Open();

            try
            {

                int totalCalc = 0;
                int currentCalc = -1;
                classE1_1.username = username;
                classE1_1.SetBasicModelInfo();
                classE1_1.calc_return = 0;     //0 - none, 1 labor, 2 eq over util, 4 warnings 8 errors 
                ADODB.Recordset recNeedCalc = new ADODB.Recordset();                
                DbUse.open_ado_rec(classE1_1.globaldb, ref recNeedCalc, "SELECT * FROM zs0tblWhatIf WHERE zs0tblWhatIf.WID=0;");

                int basecaserecalc = Convert.ToInt32(recNeedCalc.Fields["recalc"].Value);
                if (basecaserecalc != 0)
                {
                    totalCalc++;

                }
                bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE FamilyID = 0 AND recalc = true AND display = -1;");
                //bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE recalc = true;");


                while (!recNeedCalc.EOF)
                {
                    totalCalc++;
                    recNeedCalc.MoveNext();
                }
                DbUse.CloseAdoRec(recNeedCalc);

                DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.sessionid = '" + sessionID + "';");
                DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = " + totalCalc + ", calc = " + currentCalc + ", lastCheck = " + DateTime.Now.Ticks + ", cancel = 0, timePerCalc = 100000000 WHERE userlist.sessionid = '" + sessionID + "';");

                classE1_1.global_runalldone = false;
                classE1_1.global_initwid = classE1_1.glngwid;
                classE1_1.errorMessageGlobal = "";
                while (classE1_1.global_runalldone == false)
                {
                    classE1_1.calc_return = 0;
                    ////classE1_1.Run_All_ReCalc(0);
                    resultArray = classE1_1.CalculateResults(1, sessionID);
                }

                if (classE1_1.global_initwid != classE1_1.glngwid)
                {
                    if (classE1_1.global_initwid != 0)
                    {
                        classE1_1.LoadBaseCase();
                        ret = classE1_1.LoadWhatIf(classE1_1.global_initwid);
                    }
                    else
                    {
                        classE1_1.LoadBaseCase();
                    }
                };

                resultMessage = CalcClass.GetErrorMessage(classE1_1);

                if (resultMessage.Trim().Equals(String.Empty))
                {
                    foreach (var result in resultArray)
                    {
                        if (result[4].Equals("true"))
                        {
                            resultMessage += "<span style='color:#27ae60;'>";
                        }
                        else
                        {
                            resultMessage += "<span style='color:#c0392b;'>";
                        }                        
                        resultMessage += result[0] + ": ";
                        if (!String.IsNullOrEmpty(result[1]))
                        {
                            resultMessage += result[1];
                        }
                        if (!String.IsNullOrEmpty(result[2]))
                        {
                            resultMessage += result[2];
                        }
                        if (!String.IsNullOrEmpty(result[3]))
                        {
                            resultMessage += result[3];
                        }

                        resultMessage += "</span>";
                    }
                }

                classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + classE1_1.glngwid + ";");

            }
            catch (Exception)
            {
                resultMessage = "MPX internal error has occured";
            }
            classE1_1.Close();
            DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + sessionID + "';");
            DbUse.DeleteRunFile(path, username);
        }
        return resultMessage;
    }

    public static string RunAll(string username, string path, string sessionID)
    {
        string resultMessage = "";
        var resultArray = new List<string[]>();

        if (isCalculating(path))
        {
            resultMessage = "Calculations are still in process from the previous run, please wait";
        }
        else
        {
            int ret;

            DbUse.CreateRunFile(path, username);
            ClassF classE1_1 = new ClassF(path);
            classE1_1.Open();

            try
            {

                int totalCalc = 0;
                int currentCalc = -1;
                classE1_1.username = username;
                classE1_1.SetBasicModelInfo();
                classE1_1.calc_return = 0;     //0 - none, 1 labor, 2 eq over util, 4 warnings 8 errors 
                ADODB.Recordset recNeedCalc = new ADODB.Recordset();
                //check if basecase needs recalc and add to totalCalc
                DbUse.open_ado_rec(classE1_1.globaldb, ref recNeedCalc, "SELECT * FROM zs0tblWhatIf WHERE zs0tblWhatIf.WID=0;");

                totalCalc++;
                bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE FamilyID = 0 AND display = -1;");
                //bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf;");

                while (!recNeedCalc.EOF)
                {
                    totalCalc++;
                    recNeedCalc.MoveNext();
                }
                DbUse.CloseAdoRec(recNeedCalc);



                DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.sessionid = '" + sessionID + "';");
                DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = " + totalCalc + ", calc = " + currentCalc + ", lastCheck = " + DateTime.Now.Ticks + ", cancel = 0, timePerCalc = 100000000 WHERE userlist.sessionid = '" + sessionID + "';");


                classE1_1.global_runalldone = false;
                classE1_1.global_initwid = classE1_1.glngwid;
                classE1_1.errorMessageGlobal = "";
                while (classE1_1.global_runalldone == false)
                {
                    classE1_1.calc_return = 0;
                    //classE1_1.Run_All_ReCalc(1);
                    resultArray = classE1_1.CalculateResults(1, sessionID);
                }

                if (classE1_1.global_initwid != classE1_1.glngwid)
                {
                    if (classE1_1.global_initwid != 0)
                    {
                        classE1_1.LoadBaseCase();
                        ret = classE1_1.LoadWhatIf(classE1_1.global_initwid);
                    }
                    else
                    {
                        classE1_1.LoadBaseCase();
                    }
                };

                resultMessage = CalcClass.GetErrorMessage(classE1_1);

                if (resultMessage.Trim().Equals(String.Empty))
                {
                    foreach (var result in resultArray)
                    {
                        if (result[4].Equals("true"))
                        {
                            resultMessage += "<span style='color:#27ae60;'>";
                        }
                        else
                        {
                            resultMessage += "<span style='color:#c0392b;'>";
                        }
                        resultMessage += result[0] + ": ";
                        if (!String.IsNullOrEmpty(result[1]))
                        {
                            resultMessage += result[1];
                        }
                        if (!String.IsNullOrEmpty(result[2]))
                        {
                            resultMessage += result[2];
                        }
                        if (!String.IsNullOrEmpty(result[3]))
                        {
                            resultMessage += result[3];
                        }

                        resultMessage +=  "</span>";
                    }
                }


                //if (resultMessage.Trim().Equals(String.Empty))
                //{
                //    if ((classE1_1.calc_return & CalcClass.ERR_FLAG) > 0)
                //    {
                //        resultMessage = CalcClass.do_calc_msg(classE1_1.calc_return, 0);
                //    }
                //    else
                //    {
                //        resultMessage = CalcClass.do_calc_msg(classE1_1.calc_return, 1);
                //    }
                //}

                classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + classE1_1.glngwid + ";");




            }
            catch (Exception)
            {
                resultMessage = "MPX internal error has occured";
            }
            classE1_1.Close();
            DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + sessionID + "';");
            DbUse.DeleteRunFile(path, username);
        }



        return resultMessage;
    }

    public static string Progress(string sessionID)
    {
        var returnable = "";


        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();

        int timePerCalc = -1;
        int total = -1;
        int calc = -1;
        string myName = "";



        bool adoOpened = DbUse.OpenAdoMysql(conn);
        string commandString = "SELECT usercalc.id, usercalc.total, usercalc.calc, usercalc.timePerCalc, userCalc.curName, userlist.sessionid FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + sessionID + "';";
        bool adoRecOpened = DbUse.OpenAdoRec(conn, rec, commandString);

        try
        {


            timePerCalc = int.Parse(rec.Fields["timePerCalc"].Value.ToString());
            total = int.Parse(rec.Fields["total"].Value.ToString());
            calc = int.Parse(rec.Fields["calc"].Value.ToString());
            myName = Convert.ToString(rec.Fields["curName"].Value);


            if (!(calc == -1 && total == -1))
            {


                if (calc == -1 && total == 0)
                {
                    returnable = "Verifying data...";
                }
                if (calc > 0 && total > 0)
                {
                    returnable = "Calculating " + myName + " (" + calc + " of " + total + ")";
                    //report += "Calculating " + currentCalc + " out of " + totalCalc + " ...";
                }


            }
            else
            {
                returnable = "Verifying data...";
            }

        }
        catch (Exception)
        {
            returnable = "Error...";
        }
        finally
        {
            DbUse.CloseAdo(conn);
            DbUse.CloseAdoRec(rec);
        }



        return returnable;
    }

    public static bool Cancel(string sessionID)
    {
        bool returnable = false;
        try
        {
            DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET cancel = 1 WHERE userlist.sessionid = '" + sessionID + "';");
            returnable = true;
        }
        catch
        {

        }

        return returnable;
    }
}