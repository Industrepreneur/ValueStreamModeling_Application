using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for SortablePage
/// </summary>
public abstract class SortablePage : DbPage {

    protected string sortedTableName;
    protected string sortedTableName2;

    protected string defaultSortString;

    protected bool wantSort = true;
    protected bool wantSort2 = false;

    protected string SORT_COMMAND;


    public Panel sortPanel;
    public AjaxControlToolkit.ModalPopupExtender sortExtender;
    public Button btnSort;

    protected AjaxControlToolkit.ComboBox[] comboSorts;
    protected RadioButton[] rdbtnSortAsc;
    protected RadioButton[] rdbtnSortDesc;

    AccessDataSource comboSource = new AccessDataSource();

    protected string SORT_COMMAND2;


    public Panel sortPanel2;
    public AjaxControlToolkit.ModalPopupExtender sortExtender2;
    public Button btnSort2;

    protected AjaxControlToolkit.ComboBox[] comboSorts2;
    protected RadioButton[] rdbtnSortAsc2;
    protected RadioButton[] rdbtnSortDesc2;

    AccessDataSource comboSource2 = new AccessDataSource();

    public SortablePage(string sortTableName) {
        this.sortedTableName = sortTableName;
    }

    public SortablePage() {

    }

    public abstract Panel GetSortPanelContainer();
    public abstract Control GetSortButtonContainer();

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        InitializeComponent();
    }

    public void InitializeComponent() {
        if (wantSort) {
            comboSource.ID = "comboSource";
            comboSource.DataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;

         //  get sortstring from zstblsort_table, parse to get items for combo boxes ... put into combo boxes...

            if (SORT_COMMAND != null) {
                comboSource.SelectCommand = SORT_COMMAND;
            } else {
                comboSource.SelectCommand = "SELECT zstblsort.* FROM zstblsort WHERE (((zstblsort.tableName)= '" + sortedTableName + "')) ORDER BY index1;";
            }

            //HIDE SORT UNTIL I ENABLE GRIDVIEW SORT EVENT
            Label lblSort = new Label();
            lblSort.Text = "<i class='fas fa-sort fa-fw row-icon'></i>";
            lblSort.CssClass = "hidden icon-menu";
            lblSort.AssociatedControlID = InputPageControls.BTN_SORT;

            btnSort = new Button();
            btnSort.ID = InputPageControls.BTN_SORT;
            btnSort.Text = "Sort"; //   size  ??
            btnSort.CssClass = "menu-button";

            Control sortButtonContainer = GetSortButtonContainer();
            sortButtonContainer.Controls.Add(lblSort);
            sortButtonContainer.Controls.Add(btnSort);
            sortButtonContainer.Controls.Add(comboSource);


            sortPanel = InputPageControls.GenerateSortPanel();
            sortExtender = InputPageControls.GenerateSortExtender();

            Panel sortPanelContainer = GetSortPanelContainer();
            sortPanelContainer.Controls.Add(sortExtender);
            sortPanelContainer.Controls.Add(sortPanel);


            comboSorts = new AjaxControlToolkit.ComboBox[InputPageControls.NUM_SORT_EXPRESSIONS];
            rdbtnSortAsc = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
            rdbtnSortDesc = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
            for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
                comboSorts[i] = sortPanel.FindControl(InputPageControls.COMBO_SORT_IDS[i]) as AjaxControlToolkit.ComboBox;
                rdbtnSortAsc[i] = sortPanel.FindControl(InputPageControls.SORT_RADIO_BTN_ASC_IDS[i]) as RadioButton;
                rdbtnSortDesc[i] = sortPanel.FindControl(InputPageControls.SORT_RADIO_BTN_DESC_IDS[i]) as RadioButton;
            }

            Button btnOkSort = sortPanel.FindControl(InputPageControls.BTN_OK_SORT) as Button;
            btnOkSort.Click += new EventHandler(btnOkSort_Click);
            btnOkSort.OnClientClick = "HidePopup('" + InputPageControls.BEHAVIOR_SORT + "'); return true;";

            Button btnCancelSort = sortPanel.FindControl(InputPageControls.BTN_CANCEL_SORT) as Button;
            btnCancelSort.OnClientClick = "HidePopup('" + InputPageControls.BEHAVIOR_SORT + "'); return false;";

            for (int j = 0; j < InputPageControls.NUM_SORT_EXPRESSIONS; j++) {
                AjaxControlToolkit.ComboBox combo = comboSorts[j];
                combo.DataSource = comboSource;
                combo.DataTextField = "FieldName";
                combo.DataValueField = "afieldname2";
                //ComboBoxFixer.RegisterComboBox(combo);
                combo.DataBind();
                combo.Attributes.Add("onkeydown", "doFocus('" + btnOkSort.ClientID + "', event);");
                //Master.ClickOnEnter(btnOkSort.ClientID, combo);
            }
            string scriptResizeCombos = "document.getElementById('" + sortPanel.ClientID + "').style.display='block'; ";
            for (int j = 0; j < InputPageControls.NUM_SORT_EXPRESSIONS; j++) {
                AjaxControlToolkit.ComboBox combo = comboSorts[j];
                scriptResizeCombos += "ResetComboBox('" + combo.ClientID + "'); ";
            }
            btnSort.OnClientClick = scriptResizeCombos;
            sortPanel.Attributes.Add("style", "display: none;");

            
            

        }
        //ComboBoxFixer.RegisterModalPopupExtender(sortExtender); // combo fixes
        if (wantSort2) {
            comboSource2.ID = "comboSource2";
            comboSource2.DataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
            if (SORT_COMMAND2 != null) {
                comboSource2.SelectCommand = SORT_COMMAND2;
            } else {
                comboSource2.SelectCommand = "SELECT zstblsort.* FROM zstblsort WHERE (((zstblsort.tableName)= '" + sortedTableName2 + "')) ORDER BY index1;";
            }

            //NEED TO WRAP IN LABEL AND ADD CSS CLASS
            btnSort2 = new Button();
            btnSort2.ID = InputPageControls.BTN_SORT2;
            btnSort2.Text = "Sort"; //   size  ??
            //btnSort2.Width = 60;
            //btnSort2.Height = 25;
            //btnSort2.CssClass = "otherButton";

            Control sortButtonContainer2 = GetSortButtonContainer2();
            sortButtonContainer2.Controls.Add(btnSort2);
            sortButtonContainer2.Controls.Add(comboSource2);

            sortPanel2 = InputPageControls.GenerateSortPanel2();
            sortExtender2 = InputPageControls.GenerateSortExtender2();

            Panel sortPanelContainer = GetSortPanelContainer2();
            sortPanelContainer.Controls.Add(sortExtender2);
            sortPanelContainer.Controls.Add(sortPanel2);



            comboSorts2 = new AjaxControlToolkit.ComboBox[InputPageControls.NUM_SORT_EXPRESSIONS];
            rdbtnSortAsc2 = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
            rdbtnSortDesc2 = new RadioButton[InputPageControls.NUM_SORT_EXPRESSIONS];
            for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
                comboSorts2[i] = sortPanel2.FindControl(InputPageControls.COMBO_SORT_IDS2[i]) as AjaxControlToolkit.ComboBox;
                rdbtnSortAsc2[i] = sortPanel2.FindControl(InputPageControls.SORT_RADIO_BTN_ASC2_IDS[i]) as RadioButton;
                rdbtnSortDesc2[i] = sortPanel2.FindControl(InputPageControls.SORT_RADIO_BTN_DESC2_IDS[i]) as RadioButton;
            }

            Button btnOkSort2 = sortPanel.FindControl(InputPageControls.BTN_OK_SORT2) as Button;
            btnOkSort2.Click += new EventHandler(btnOkSort2_Click);
            btnOkSort2.OnClientClick = "HidePopup('" + InputPageControls.BEHAVIOR_SORT2 + "'); return true;";

            Button btnCancelSort2 = sortPanel.FindControl(InputPageControls.BTN_CANCEL_SORT2) as Button;
            btnCancelSort2.OnClientClick = "HidePopup('" + InputPageControls.BEHAVIOR_SORT2 + "'); return false;";

            for (int j = 0; j < InputPageControls.NUM_SORT_EXPRESSIONS; j++) {
                AjaxControlToolkit.ComboBox combo = comboSorts2[j];
                combo.DataSource = comboSource2;
                combo.DataTextField = "FieldName";
                combo.DataValueField = "afieldname2";
                //ComboBoxFixer.RegisterComboBox(combo);
                combo.DataBind();
                combo.Attributes.Add("onkeydown", "doFocus('" + btnOkSort2.ClientID + "', event);");
                //Master.ClickOnEnter(btnOkSort2.ClientID, combo);
            }
            string scriptResizeCombos = "document.getElementById('" + sortPanel2.ClientID + "').style.display='block'; ";
            for (int j = 0; j < InputPageControls.NUM_SORT_EXPRESSIONS; j++) {
                AjaxControlToolkit.ComboBox combo = comboSorts2[j];
                scriptResizeCombos += "ResetComboBox('" + combo.ClientID + "'); ";
            }
            btnSort2.OnClientClick = scriptResizeCombos;
            sortPanel2.Attributes.Add("style", "display: none;");

            
        }


    }

    protected virtual void btnOkSort_Click(object sender, EventArgs e) {
        SaveSortingOrder(comboSorts, rdbtnSortAsc, sortedTableName);

    }

    protected virtual void btnOkSort2_Click(object sender, EventArgs e) {
        SaveSortingOrder(comboSorts2, rdbtnSortAsc2, sortedTableName2);

    }

    protected virtual void SaveSortingOrder(AjaxControlToolkit.ComboBox[] combos, RadioButton[] radioAsc, string sortTableName) {
        List<string> orderingFields = new List<string>();
        for (int i = 0; i < InputPageControls.NUM_SORT_EXPRESSIONS; i++) {
            if (combos[i].SelectedValue != null && !combos[i].SelectedValue.Trim().Equals("")) {
                string orderingField = combos[i].SelectedValue;
                if (orderingField.ToLower().IndexOf("val(") != 0 && orderingField.ToLower().IndexOf("sum(") != 0) {
                    orderingField = "[" + orderingField + "]";
                }
                orderingField += (radioAsc[i].Checked) ? " " : " desc";
                if (!orderingFields.Contains(orderingField)) {
                    orderingFields.Add(orderingField);
                }
            }
        }

        if (orderingFields.Count > 0) {
//  xx  reading choices
            StringBuilder orderString = new StringBuilder("Order By ");
            for (int i = 0; i < orderingFields.Count; i++) {
                orderString.Append(orderingFields.ElementAt(i));
                if (i != orderingFields.Count - 1) {
                    orderString.Append(", ");
                }
            }
            string commandString = "UPDATE zstblsort_table SET zstblsort_table.sortstring = '" + orderString + "' WHERE (((zstblsort_table.tableName) ='" + sortTableName + "'));";
            UpdateSql(commandString);
            RefreshData();
        }
    }

    protected abstract void RefreshData();

    protected string GetOrderBy(string tableName) {
        return GetDatabaseField("sortstring", "tablename", tableName, "zstblsort_table");
    }

    protected virtual Control GetSortButtonContainer2() {
        return null;
    }

    protected virtual Panel GetSortPanelContainer2() {
        return null;
    }
}