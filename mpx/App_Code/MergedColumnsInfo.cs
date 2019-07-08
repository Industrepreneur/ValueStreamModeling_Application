using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MergedColumnsInfo
/// </summary>
[Serializable] public class MergedColumnsInfo
{
	public MergedColumnsInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    // indexes of merged columns
    public List<int> MergedColumns = new List<int>();
    // key-value pairs: key = the first column index, value = number of the merged columns
    public Hashtable StartColumns = new Hashtable();
    // key-value pairs: key = the first column index, value = common title of the merged columns 
    public Hashtable Titles = new Hashtable();

    //parameters: the merged columns indexes, common title of the merged columns 
    public void AddMergedColumns(int[] columnsIndexes, string title) {
        MergedColumns.AddRange(columnsIndexes);
        StartColumns.Add(columnsIndexes[0], columnsIndexes.Length);
        Titles.Add(columnsIndexes[0], title);
    }
}