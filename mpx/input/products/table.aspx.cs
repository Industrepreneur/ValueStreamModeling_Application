using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class products : InputGridPage {

    ProductDelegatePage helperProduct;
    
    public products() {
        PAGENAME = "/input/products/table.aspx";
        featureHelper = new ProductTableDelegate();
        fieldsNonEditable = new bool[featureHelper.FIELDS.Length];
        fieldsNonEditable[1] = true;

        int value = 0;
        helperProduct = new ProductDelegatePage(value);

        
    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);
    }

    private void InitializeComponent() {
        pnlMainGrid = gridPanel;

    }

    protected override void OnInit(EventArgs e) {
        InitializeComponent();
        //pnlMenu.Controls.Add(new LiteralControl("<h2>Products</h2>"));
        base.OnInit(e);
        if (IsWhatifMode()) {
            Response.Redirect("whatif_" + PAGENAME);
        }
      
        helperProduct.OnInit(e);
        string dataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        tableSync = new TableSyncProduct(userDir);

       
        //srcProductsList.DataFile = dataFile;
    }

    protected override Control getButtonDiv() {
        return buttondiv;
    }

    protected override Panel GetSecondPanel() {
        return secondPanel;
    }

    protected override Panel GetThirdPanel() {
        return thirdPanel;
    }

    protected override Panel GetFourthPanel() {
        return fourthPanel;
    }

    protected override Panel GetFifthPanel() {
        return fourthPanel;
    }

    protected override void grid_RowCommand(object sender, GridViewCommandEventArgs e) {
        Button btn = e.CommandSource as Button;
        if (btn == null) {
            return;
        }
        GridViewRow row = btn.NamingContainer as GridViewRow;
        int rowIndex = row.RowIndex;
        if (e.CommandName.Equals("Copy")) {
            var newValues = this.GetValues(row);
            string origName = newValues[FIELDS[1]].ToString().Replace("&nbsp;", " ");
            origName = origName.Replace("&NBSP;", " ");
            string copyName = origName + "_COPY";
            copyName = GetUniqueName("ProdDesc", "ProdID", "tblprodfore", copyName);
            {
                ClassA classA = new ClassA(GetDirectory() + userDir);
                classA.setGlobalVar();
                string uniqueName = "";
                try {
                    uniqueName = classA.CopyProdFore(origName, copyName);

                } catch (Exception exp) {
                    logFiles.ErrorLog(exp); 
                    SetData();
                    try {
                        Master.HideLoadingPopup();
                    } catch (Exception) { }
                    Master.ShowErrorMessage("There is invalid data in the Product file. Some information may not be copied."); 
                    return;
                }
                classA.Close();
                tableSync.SyncTables();
                tableSync.UpdateEquipNames();
                tableSync.UpdateOperNames();
                SetData();
                if (!uniqueName.Equals("")) {
                    try {
                        SetData();
                        int id = int.Parse(GetId(uniqueName));
                        GoToEditMode(id);
                    } catch (Exception) { }
                }

            }
            try {
                Master.HideLoadingPopup();
            } catch (Exception) { }
        } else {
            base.grid_RowCommand(sender,e);
        }
    }

    protected override void InsertRow(bool goToEdit) {
        GridViewRow row = grid.FooterRow;
        Control[] txtControls = new Control[FIELDS.Length];
        //TextBox[] txtInserts = new TextBox[FIELDS.Length];
        List<String> selectedFields = new List<String>();
        List<TextBox> selectedTextBoxes = new List<TextBox>();
        selectedTextBoxes.Add(null);
        selectedFields.Add(FIELDS[0]);
        for (int i = 1; i < FIELDS.Length; i++) {
            txtControls[i] = row.FindControl(TEXT_BOX_IDS[i]);
            if (txtControls[i] == null) { return; }
            
        }
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        //string command = GetCommandString(Command.INSERT, selectedFields.ToArray());
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            /*for (int i = 1; i < selectedFields.Count; i++) {
                string value = selectedTextBoxes.ElementAt(i).Text;
                cmd.Parameters.AddWithValue(selectedFields[i], value);
            }*/

            string prodDesc = "";

            for (int i = 1; i < txtControls.Length; i++) {
                string value;
                if (mode.Equals("Advanced") || !ADVANCED_FIELDS[i]) {
                    if (COMBOS[i]) {
                        if (((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedItem != null) {
                            value = ((AjaxControlToolkit.ComboBox)txtControls[i]).SelectedValue;
                        } else {
                            value = MyUtilities.clean(((AjaxControlToolkit.ComboBox)txtControls[i]).Text);
                        }
                    } else if (CHECKBOXES[i]) {
                        value = (((CheckBox)txtControls[i]).Checked) ? "1" : "0";
                    } else {
                        value = MyUtilities.clean(((TextBox)txtControls[i]).Text);
                    }
                    if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam")) {
                        value = value.ToUpper();
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    if (i == 1) {
                        prodDesc = value;
                    }
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                connec.Close();
                int prodId = int.Parse(GetId(prodDesc));
                {
                    ClassA classA = new ClassA(GetDirectory() + userDir);
                    classA.addoper_1(prodId);
                    classA.Close();
                    tableSync.UpdateEquipNames();
                    tableSync.UpdateOperNames();
                }
                if (goToEdit) {
                    SetData();
                    GoToEditMode(prodId);
                }
                
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                SaveInsertValues(grid.FooterRow, TEXT_BOX_IDS);
                Master.ShowErrorMessage(DbUse.INSERT_DATA_ERROR_MSG);
            }
        }
    }

    

    protected override bool InsertRecord(string[] entries) {
        bool wasInserted = false;
        connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        string command = GetCommandString(Command.INSERT);
        OleDbCommand cmd = new OleDbCommand(command, connec);
        {
            int i = 1;
            
            string prodDesc = "";
            if (entries.Length > 0) {
                prodDesc = MyUtilities.clean(entries[0]).Trim();
            }
            for (int j = 0; j < entries.Length && i < FIELDS.Length; j++) {
                if (mode.Equals("Standard")) {
                    while (i < FIELDS.Length && ADVANCED_FIELDS[i]) {
                        i++;
                    }
                }
                if (i < FIELDS.Length) {
                    string value = MyUtilities.clean(entries[j]);
                    if (value.Trim().Length > 0) {
                        value = value.Trim();
                    }
                    if (CHECKBOXES[i]) {
                        value = value.ToLower().Equals("true") ? "1" : "0";
                    }
                    if (FIELDS[i].Equals("LaborDesc") || FIELDS[i].Equals("EquipDesc") || FIELDS[i].Equals("ProdDesc") || FIELDS[i].Equals("OpNam")) {
                        value = value.ToUpper();
                    }
                    cmd.Parameters.AddWithValue(FIELDS[i], value);
                    i++;
                }
            }
            try {
                connec.Open();
                int result = cmd.ExecuteNonQuery();
                wasInserted = true;
                connec.Close();
                this.SetData();
                int prodId = int.Parse(GetId(prodDesc));
                {
                    ClassA classA = new ClassA(GetDirectory() + userDir);
                    classA.addoper_1(prodId);
                    classA.Close();
                }
                
            } catch {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
            }
        }
        FillDefaultInsertRow();
        return wasInserted;
    }

    


}