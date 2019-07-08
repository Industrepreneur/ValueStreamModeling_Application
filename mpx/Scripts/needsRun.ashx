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
        //context.Response.Write(RunCurrent(username, userDir, mainDir));
        context.Response.Write(RunAll(username, userDir, mainDir, cookie));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }




    public Boolean RunAll(string username, string userDir, string mainDir, string cookieId)
    {
        Boolean resultMessage;

        if (DbUse.InRunProcess(userDir))
        {
            resultMessage = false;
        }
        else
        {
        
            string cookieid = cookieId;
          
            ClassF classE1_1 = new ClassF(mainDir + userDir);
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
    }//end sub


}