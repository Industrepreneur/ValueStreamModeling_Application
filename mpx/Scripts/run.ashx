<%@ WebHandler Language="C#" Class="Run" %>

using System;
using System.Web;

public class Run : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string username = context.Request.QueryString["username"];
        string mainDir = context.Request.QueryString["mainDir"];
        string userDir = context.Request.QueryString["userDir"];
        string cookie = context.Request.QueryString["cookie"];
        string type = context.Request.QueryString["type"];
        //context.Response.Write(RunCurrent(username, userDir, mainDir));
        context.Response.Write(RunAll(username, userDir, mainDir, cookie, type));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public string RunAll(string username, string userDir, string mainDir, string cookieId, string type)
    {
        string resultMessage;
        int calcType = Int32.Parse(type);

        if (DbUse.InRunProcess(userDir))
        {
            resultMessage = "Calculations are still in process from the previous run, please wait";
        }
        else
        {
            int ret;
            string cookieid = cookieId;
            DbUse.CreateRunFile(mainDir + userDir, username);
            ClassF classE1_1 = new ClassF(mainDir + userDir);
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
                if (calcType == 0)
                {
                    int basecaserecalc = Convert.ToInt32(recNeedCalc.Fields["recalc"].Value);
                    if (basecaserecalc != 0)
                    {
                        totalCalc++;

                    }
                    bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE FamilyID = 0 AND recalc = true AND display = -1;");
                }
                else
                {
                    totalCalc++;
                    bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recNeedCalc, "SELECT * FROM tblWhatIf WHERE FamilyID = 0 AND display = -1;");
                }

                while (!recNeedCalc.EOF)
                {
                    totalCalc++;
                    recNeedCalc.MoveNext();
                }
                DbUse.CloseAdoRec(recNeedCalc);


                //if (totalCalc > 0)
                //{

                DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.sessionid = '" + cookieid + "';");
                DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = " + totalCalc + ", calc = " + currentCalc + ", lastCheck = " + DateTime.Now.Ticks + ", cancel = 0, timePerCalc = 100000000 WHERE userlist.sessionid = '" + cookieid + "';");


                classE1_1.global_runalldone = false;
                classE1_1.global_initwid = classE1_1.glngwid;
                classE1_1.errorMessageGlobal = "";
                while (classE1_1.global_runalldone == false)
                {


                    classE1_1.calc_return = 0;
                    classE1_1.Run_All_ReCalc(calcType);


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
                    if ((classE1_1.calc_return & CalcClass.ERR_FLAG) > 0)
                    {
                        resultMessage = CalcClass.do_calc_msg(classE1_1.calc_return, 0);
                    }
                    else
                    {
                        resultMessage = CalcClass.do_calc_msg(classE1_1.calc_return, 1);
                    }
                }

                classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + classE1_1.glngwid + ";");

                //}
                //else
                //{
                //    resultMessage = "calculations are not necessary";

                //}


            }
            catch (Exception)
            {
                resultMessage = "MPX internal error has occured";
            }
            classE1_1.Close();
            DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + cookieid + "';");
            DbUse.DeleteRunFile(mainDir + userDir, username);
        }
        return resultMessage;
    }//end sub


}