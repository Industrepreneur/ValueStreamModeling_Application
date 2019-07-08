using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web;

public abstract class Query
{
    private const string MAIN_USER_DATABASE = "mpxmdb.mdb";

    public static string GetMaxUtil()
    {
        string maxUtilQuery = "SELECT tblgeneral.utlimit FROM tblgeneral";
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Sessionable.GetSessionUserModelDirectoryPath() + ";");
        OleDbCommand cmd = new OleDbCommand(maxUtilQuery, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

        DataTable dt = new DataTable();
        String myUtil = null;

        try
        {
            connec.Open();

            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                throw new Exception();
            }
            else
            {
                foreach (DataRow dtrow in dt.Rows)
                {

                    myUtil = dtrow[0].ToString().ToUpper();

                }
            }


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

        return myUtil;
    }

    public static List<string[]> databaseQueryArray(string query, int[] queryArray)
    {

        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Sessionable.GetSessionUserModelDirectoryPath() + ";");
        OleDbCommand cmd = new OleDbCommand(query, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

        DataTable dt = new DataTable();
        List<string[]> myResults = new List<string[]>();

        try
        {
            connec.Open();

            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                throw new Exception();
            }
            else
            {
                foreach (DataRow dtrow in dt.Rows)
                {
                    string[] myResult = new string[queryArray.Length];

                    for (int i = 0; i < queryArray.Length; i++)
                    {
                        myResult[i] = dtrow[queryArray[i]].ToString().ToUpper();
                    }

                    myResults.Add(myResult);
                }
            }


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

        return myResults;
    }

}