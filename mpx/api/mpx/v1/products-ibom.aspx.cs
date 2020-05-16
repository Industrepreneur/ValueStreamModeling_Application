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

public partial class api_v1_products_ibom : System.Web.UI.Page
{
    public const string TableName = "tblIbom";
    public const string IdColumn = "ibomid";

    private static string getSource()
    {
        return ApiUtil.GetSessionUserModelDirectory();
    }

    [WebMethod(EnableSession = true)]
    public static string updateRow(string id, string columnName, string newValue)
    {
        // Check rules
        var rules = new RulesEngine(newValue, columnName);
        rules.checkColumn("upa")
          .required()
          .number();

        if(columnName == "compname")
        {
            // TODO: need to do additonal query, probably before running model
            // Check compname's ibom settings, make sure it isn't set to this compname

            //Selected Product cannot nest an upper level
            //This would make infinite loop
            //A needs 2xB
            //B cannot nest A
            //A needs 2xC and 2xB, C needs 4xB, this is OK
        }

        if (rules.HasError)
        {
            return MpxTableUtil.CreateError(rules.Error);
        }

        return MpxTableUtil.UpdateRow(getSource(), TableName, IdColumn, id, columnName, newValue);
    }

    [WebMethod(EnableSession = true)]
    public static string addRow(string param1)
    {
        // TODO: add any defaults
        int nextId;
        var defaultValues = new List<Tuple<string, string>>();
        defaultValues.Add(new Tuple<string, string>("proddesc", param1));
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

}