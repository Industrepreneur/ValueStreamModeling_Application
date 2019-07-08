using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class api_v1_products : System.Web.UI.Page
{
    public static string TableName = "tbllabor";
    public static string IdColumn = "LaborID";

    private static string getSource()
    {
        return ApiUtil.GetSessionUserModelDirectory();
    }

    [WebMethod(EnableSession = true)]
    public static string updateRow(string id, string columnName, string newValue)
    {
        return MpxTableUtil.UpdateRow(getSource(), TableName, IdColumn, id, columnName, newValue);
    }

    [WebMethod(EnableSession = true)]
    public static string addRow()
    {
        // TODO: add any defaults
        int nextId;
        var defaultValues = new List<Tuple<string, string>>();
        defaultValues.Add(new Tuple<string, string>("labordesc", "NEW ITEM"));
        return MpxTableUtil.AddRow(getSource(), TableName, IdColumn, defaultValues, out nextId);
    }

    [WebMethod(EnableSession = true)]
    public static string deleteRow(string id)
    {
        return MpxTableUtil.DeleteRow(getSource(), TableName, IdColumn, id);
    }

    [WebMethod(EnableSession = true)]
    public static TableResults selectAll()
    {
        return MpxTableUtil.SelectAll(getSource(), TableName);
    }

    [WebMethod(EnableSession = true)]
    public static TableResults selectList(string columnName)
    {
        return MpxTableUtil.SelectList(getSource(), TableName, columnName);
    }
    
}