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

public partial class api_v1_products_operations : System.Web.UI.Page
{
    public const string TableName = "tbloper";
    // private const string TableNameRouting = "tblOperFrTo";
    public const string IdColumn = "opid";

    private static string getSource()
    {
        return ApiUtil.GetSessionUserModelDirectory();
    }

    [WebMethod(EnableSession = true)]
    public static string updateRow(string id, string columnName, string newValue)
    {
        // Check rules
        var rules = new RulesEngine(newValue, columnName);
        rules.checkColumn("opnam")
          .exclude("DOCK")
          .exclude("SCRAP")
          .exclude("STOCK");

        rules.checkColumn("opnum")
         .exclude("0")
         .exclude("10000")
         .exclude("9999")
         .integer()
         .greaterThan(0);

        rules.checkColumn("percentassign")
         .integer()
         .inRange(1, 100);

        rules.checkColumn("eqsetuptime")
         .isSpecialMpxMath();
        rules.checkColumn("eqruntime")
         .isSpecialMpxMath();
        rules.checkColumn("labsetuptime")
         .isSpecialMpxMath();
        rules.checkColumn("labruntime")
         .isSpecialMpxMath();
        
        if (rules.HasError)
        {
            return MpxTableUtil.CreateError(rules.Error);
        }

        //newValue = newValue.Trim();
        //columnName = columnName.ToLower();
        //if (columnName == "opnam")
        //{
        //    newValue.ToUpper();
        //    if (newValue == "DOCK" || newValue == "SCRAP" || newValue == "STOCK")
        //    {
        //        return MpxTableUtil.CreateError("Name cannot be DOCK, SCRAP, or STOCK");
        //    }
        //}
        //if (columnName == "opnum")
        //{
        //    if (newValue == "0" || newValue == "10000" || newValue == "9999")
        //    {
        //        return MpxTableUtil.CreateError("Number cannot be 0, 10000, or 9999");
        //    }
        //    int newValueInt = 0;
        //    if (!int.TryParse(newValue, out newValueInt))
        //    {
        //        return MpxTableUtil.CreateError("Must be an integer");
        //    }
        //    if (!(newValueInt > 0))
        //    {
        //        return MpxTableUtil.CreateError("Must be greater than 0");
        //    }
        //}
        //if (columnName == "percentassign")
        //{
        //    int newValueInt = 0;
        //    if (!int.TryParse(newValue, out newValueInt))
        //    {
        //        return MpxTableUtil.CreateError("Must be an integer");
        //    }
        //    if (!(newValueInt >= 1 && newValueInt <= 100))
        //    {
        //        return MpxTableUtil.CreateError("Must be between 1 and 100");
        //    }
        //}
        //if (columnName == "eqsetuptime")
        //{
        //    // TODO: other mathy columns
        //    // TODO: allow mathy ops
        //    int newValueInt = 0;
        //    if (!int.TryParse(newValue, out newValueInt))
        //    {
        //        return MpxTableUtil.CreateError("Must be an integer");
        //    }
        //    if (!(newValueInt >= 0))
        //    {
        //        return MpxTableUtil.CreateError("Must be 0 or more");
        //    }
        //}

        return MpxTableUtil.UpdateRow(getSource(), TableName, IdColumn, id, columnName, newValue);
    }

    [WebMethod(EnableSession = true)]
    public static string addRow(string param1)
    {
        // TODO: add any defaults
        int nextId;
        var defaultValues = new List<Tuple<string, string>>();
        defaultValues.Add(new Tuple<string, string>("proddesc", param1));
        defaultValues.Add(new Tuple<string, string>("opnam", "NEW"));
        defaultValues.Add(new Tuple<string, string>("equipdesc", "NONE"));
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