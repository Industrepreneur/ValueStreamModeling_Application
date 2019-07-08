using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class whatif_display : DbPage {
    public whatif_display() {
        PAGENAME = "whatif_display.aspx";
    }



    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
        if (!Page.IsPostBack) {
            try {
                BindCheckWhatifs();
            } catch (Exception) {
                Master.ShowErrorMessage("MPX internal error has occured.");
            }
        }
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
    }


    protected void btnSaveShowWhatifs_Click(object sender, EventArgs e) {
        try {
            CalcClass calc = new CalcClass(GetDirectory() + userDir + MAIN_USER_DATABASE);
            int numOfWhatifShow = 0;
            for (int i = 0; i < lstCheckWhatifs.Items.Count; i++) {
                ListItem item = lstCheckWhatifs.Items[i];
                string key = item.Value;
                string show = item.Selected ? "-1" : "0";
                if (key.Equals("0")) {
                    if (calc.runsql("UPDATE zs0tblWhatif SET display = " + show + " WHERE WID = " + key + ";") && item.Selected) {
                        numOfWhatifShow++;
                    }
                } else {
                    if (calc.runsql("UPDATE tblWhatif SET display = " + show + " WHERE WID = " + key + ";") && item.Selected) {
                        numOfWhatifShow++;
                    }
                }
            }
            if (numOfWhatifShow == 0) {
                // the user chose no results to display - by default display basecase results
                calc.runsql("UPDATE zs0tblWhatif SET display = -1 WHERE WID = 0;");
                Master.ShowErrorMessage("No results are selected to display. The basecase results will be shown by default.");
            }
            BindCheckWhatifs();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e) {
        for (int i = 0; i < lstCheckWhatifs.Items.Count; i++) {
            lstCheckWhatifs.Items[i].Selected = true;
        }
    }


    protected void btnClearAllWhatifs_Click(object sender, EventArgs e) {
        for (int i = 0; i < lstCheckWhatifs.Items.Count; i++) {
            ListItem item = lstCheckWhatifs.Items[i];
            string key = item.Value;
            // clear all but basecase
            if (key != null && !key.Equals("0")) {
                item.Selected = false;
            }
        }
    }

    protected void btnReset_Click(object sender, EventArgs e) {
        BindCheckWhatifs();

    }

    protected void BindCheckWhatifs() {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string comm = "SELECT WID, Name, Comm, display FROM zstblwhatif;";
        OleDbCommand cmd = new OleDbCommand(comm, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                lstCheckWhatifs.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++) {
                    ListItem item = new ListItem(dt.Rows[i]["Name"].ToString(), dt.Rows[i]["WID"].ToString());
                    item.Selected = !dt.Rows[i]["display"].ToString().Equals("0");
                    item.Attributes.Add("title", dt.Rows[i]["Comm"].ToString());
                    lstCheckWhatifs.Items.Add(item);
                }
            } catch (Exception ex) {
                try {
                    connec.Close();
                } catch (Exception) { }
                logFiles.ErrorLog(ex);
                if (!TablesLinked()) {
                    Master.ShowErrorMessage("An error has occured. Current model '" + Master.GetCurrentModel() + "' is not loaded properly because some tables are missing. Please go to the models page and load the model again.");                    
                } else {
                    Master.ShowErrorMessage("MPX internal error has occured.");
                }
            }
        }
    }


}