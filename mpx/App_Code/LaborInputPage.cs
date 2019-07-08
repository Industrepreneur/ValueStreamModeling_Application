using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LaborInputPage
/// </summary>
public abstract class LaborInputPage: InputGridPage
{
	public LaborInputPage()
	{
        FIELDS = new string[] { "LaborID", "LaborDesc", "LaborDept", "GrpSiz", "OT", "Abst", "PriorityShare", "Setup", "Run", "Varbility", "LabComment" };
        ADVANCED_FIELDS = new bool[] { false, false, false, false, false, false, true, true, true, true, false };
        HEADERS = new string[] { null, "Labor Name", "Group / Dept / Area", "No. Working Simultaneously", "Overtime %", "Absenteeism %", "Prioritize Use", "Setup Time Multiplier", "Run Time Multiplier", "Variability Multiplier", "Comment" };

        InitializeCombos();
        InitializeCheckboxes();
        CHECKBOXES[6] = true;

        TEXT_BOX_IDS = GetIDs(IDs.TEXT_BOX);
        LABEL_IDS = GetIDs(IDs.LABEL);

        TABLE_NAME = "tbllabor";
        sortedTableName = TABLE_NAME;
        defaultSortString = "ORDER BY tbllabor.LaborDesc";
        SORT_COMMAND = "SELECT zstblsort.* FROM zstblsort WHERE (((zstblsort.tableName)= '" + sortedTableName + "') AND (afieldname <> 'L1') AND (afieldname <> 'L2') AND (afieldname <> 'L3') AND (afieldname <> 'L4'));";
	}

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
    }

    protected override string GetCommandString(int commandType, string[] selectedFields) {
        string commandString = base.GetCommandString(commandType, selectedFields);
        if (commandType == Command.SELECT) {
            int index = commandString.ToLower().IndexOf("order by");
            string orderCommand = commandString.Substring(index);
            string selectCommand = commandString.Substring(0, index);
            commandString = selectCommand + " WHERE LaborDesc <> 'none' " + orderCommand;
        }
        return commandString;
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        
    }
}