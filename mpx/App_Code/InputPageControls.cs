using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using AjaxControlToolkit;

/// <summary>
/// Summary description for InputPageControls
/// </summary>
public class InputPageControls
{
	public InputPageControls()
	{
		
	}

    // control IDs
    public const string BTN_OK = "btnOk";
    public const string BTN_OK_LINE = "btnOkLine";
    public const string BTN_CANCEL = "btnCancel";
    public const string BTN_CANCEL_LINE = "btnCancelLine";
    public const string RDBTN_COPY = "rdbtnCopy";
    public const string RDBTN_EDIT = "rdbtnEdit";
    public const string RDBTN_DELETE = "rdbtnDelete";

    public const string RDBTN_COPY_LINE = "rdbtnCopyLine";
    public const string RDBTN_EDIT_LINE = "rdbtnEditLine";
    public const string RDBTN_DELETE_LINE = "rdbtnDeleteLine";
    public const string RDBTN_NEW_LINE = "rdbtnNewLine";
    public const string BTN_ADD = "btnAdd";
    public const string LBL_ACTION = "lblAction";

    public const string RDBTN_SHOW_LINE = "rdbtnShowLine";
    public const string POPUP_PANEL = "pnlModal";
    public const string BTN_DUMMY = "btnDummy";
    public const string BTN_DUMMY2 = "btnDummy2";
    public const string BTN_DUMMY3 = "btnDummy3";
    public const string PNL_MODAL = "pnlModal";
    
    public const string POPUP_BEHAVIOR = "popupBehavior";
    public const string POPUP_LINE_BEHAVIOR = "popupLineBehavior";
    public const string PNL_SORT = "pnlSort";
    public const string PNL_SORT2 = "pnlSort2";
    public const string EXTENDER_LINE_ID = "MPEline";
    public const string PNL_MODAL_LINE = "pnlModalLine";

    public const string BEHAVIOR_SORT = "sortBehavior";
    public const string BEHAVIOR_SORT2 = "sortBehavior2";

    public const string PNL_NEW_NAME = "pnlNewName";
    public const string TXT_NEW_NAME = "txtNewName";
    public const string HIDDEN_MODE = "hdnMode";
    public const string EXTENDER_NEW_NAME = "extenderNewName";
    public const string BTN_CANCEL_NEW_NAME = "btnCancelNewName";
    public const string BTN_OK_NEW_NAME = "btnOkNewName";

    public const string BTN_OK_SORT = "btnOkSort";
    public const string BTN_OK_SORT2 = "btnOkSort2";
    public const string BTN_CANCEL_SORT = "btnCancelSort";
    public const string BTN_CANCEL_SORT2 = "btnCancelSort2";
    public const string EXTENDER_SORT_ID = "MPEsort";
    public const string EXTENDER_SORT_ID2 = "MPEsort2";
    public const string BTN_SORT = "btnSort";  
    public const string BTN_SORT2 = "btnSort2";
    public static string[] COMBO_SORT_IDS = GetComboIds();
    public static string[] COMBO_SORT_IDS2 = GetComboIds2();
    public static string[] SORT_RADIO_BTN_ASC_IDS = GetSortRdbtnIds("Asc");
    public static string[] SORT_RADIO_BTN_DESC_IDS = GetSortRdbtnIds("Desc");
    public static string[] SORT_RADIO_BTN_ASC2_IDS = GetSortRdbtnIds("Asc2");
    public static string[] SORT_RADIO_BTN_DESC2_IDS = GetSortRdbtnIds("Desc2");

    public const string LBL_NEW_NAME = "lblNewName";

    public const int NUM_SORT_EXPRESSIONS = 3;

    public static string[] GetComboIds() {
        string[] comboIds = new string[NUM_SORT_EXPRESSIONS];
        for (int i=0; i<NUM_SORT_EXPRESSIONS; i++) {
            comboIds[i] = "comboSort" + i;
        }
        return comboIds;
    }

    public static string[] GetComboIds2() {
        string[] comboIds = new string[NUM_SORT_EXPRESSIONS];
        for (int i = 0; i < NUM_SORT_EXPRESSIONS; i++) {
            comboIds[i] = "comboSort2" + i;
        }
        return comboIds;
    }

    public static string[] GetSortRdbtnIds(string direction) {
        string[] sortRdbtns = new string[NUM_SORT_EXPRESSIONS];
        for (int i=0; i< NUM_SORT_EXPRESSIONS; i++) {
            sortRdbtns[i] = "sortRdbtn" + direction + i;
        }
        return sortRdbtns;
    }

    public static Panel GeneratePopupPanel() {
        Panel popPanel = new Panel();
        popPanel.ID = POPUP_PANEL;
        popPanel.CssClass = "popPanel";
        

        Label label = new Label();
        label.ID = "myLabel";
        label.Text = "Choose what you want to do with the row:";
        popPanel.Controls.Add(label);

        popPanel.Controls.Add(new LiteralControl("<br />"));
        popPanel.Controls.Add(new LiteralControl("<br />"));

        RadioButton rdbtnEdit = new RadioButton();
        rdbtnEdit.ID = RDBTN_EDIT;
        rdbtnEdit.Text = "Edit";
        rdbtnEdit.GroupName = "modalGroup";
        popPanel.Controls.Add(rdbtnEdit);
        popPanel.Controls.Add(new LiteralControl("<br />"));

        RadioButton rdbtnCopy = new RadioButton();
        rdbtnCopy.ID = RDBTN_COPY;
        rdbtnCopy.Text = "Copy";
        rdbtnCopy.GroupName = rdbtnEdit.GroupName;
        popPanel.Controls.Add(rdbtnCopy);
        popPanel.Controls.Add(new LiteralControl("<br />"));

        RadioButton rdbtnDelete = new RadioButton();
        rdbtnDelete.ID = RDBTN_DELETE;
        rdbtnDelete.Text = "Delete";
        rdbtnDelete.GroupName = rdbtnEdit.GroupName;
        popPanel.Controls.Add(rdbtnDelete);
        popPanel.Controls.Add(new LiteralControl("<br />"));

        RadioButton rdbtnShowLine = new RadioButton();
        rdbtnShowLine.ID = RDBTN_SHOW_LINE;
        rdbtnShowLine.Text = "Show Line";
        rdbtnShowLine.GroupName = rdbtnEdit.GroupName;
        popPanel.Controls.Add(rdbtnShowLine);
        popPanel.Controls.Add(new LiteralControl("<br />"));
        popPanel.Controls.Add(new LiteralControl("<br />"));

        Button btnOk = new Button();
        btnOk.ID = BTN_OK;
        btnOk.Text = "Ok";
        
        popPanel.Controls.Add(btnOk);
        

        Button btnCancel = new Button();
        btnCancel.ID = BTN_CANCEL;
        btnCancel.Text = "Cancel";
        popPanel.Controls.Add(btnCancel);
        popPanel.Attributes.Add("style", "display:none;");
        return popPanel;
    }

    public static AjaxControlToolkit.ModalPopupExtender GeneratePopupExtender() {
        AjaxControlToolkit.ModalPopupExtender popupExtender = new AjaxControlToolkit.ModalPopupExtender();
        popupExtender.ID = "MPE";
        popupExtender.TargetControlID = BTN_DUMMY;
        popupExtender.PopupControlID = PNL_MODAL;
        popupExtender.BehaviorID = POPUP_BEHAVIOR;
        popupExtender.BackgroundCssClass = "modalBackground";
        popupExtender.DropShadow = true;
        //popupExtender.OkControlID = BTN_OK;
        popupExtender.CancelControlID = BTN_CANCEL;
        
        return popupExtender;
    }


    public static AjaxControlToolkit.ModalPopupExtender GeneratePopupLineExtender() {
        AjaxControlToolkit.ModalPopupExtender popupExtender = new AjaxControlToolkit.ModalPopupExtender();
        popupExtender.ID = EXTENDER_LINE_ID;
        popupExtender.TargetControlID = BTN_DUMMY2;
        popupExtender.PopupControlID = PNL_MODAL_LINE;
        //popupExtender.BehaviorID = POPUP_LINE_BEHAVIOR;
        popupExtender.BackgroundCssClass = "modalBackground";
        //popupExtender.CancelControlID = BTN_CANCEL_LINE;
        //popupExtender.OkControlID = BTN_OK_LINE;
        popupExtender.DropShadow = true;

        return popupExtender;
    }
    
    public static Panel GenerateSortPanel() {
        return PrepareSortPanel(PNL_SORT, COMBO_SORT_IDS, SORT_RADIO_BTN_ASC_IDS, SORT_RADIO_BTN_DESC_IDS, "groupName", BTN_OK_SORT, BTN_CANCEL_SORT);
    }

    public static Panel GenerateSortPanel2() {
        return PrepareSortPanel(PNL_SORT2, COMBO_SORT_IDS2, SORT_RADIO_BTN_ASC2_IDS, SORT_RADIO_BTN_DESC2_IDS, "groupName2", BTN_OK_SORT2, BTN_CANCEL_SORT2);        
    }

    public static Panel PrepareSortPanel(string panelId, string[] comboSortIds, string[] radioAsc, string[] radioDesc, string groupNameBase, string btnOkSortId, string btnCancelSortId) {
        Panel pnlSort = new Panel();
        pnlSort.ID = panelId;
        pnlSort.CssClass = "popPanel";
        //pnlSort.Attributes.Add("style", "display: block;");

        pnlSort.Controls.Add(new LiteralControl("<h3>Sort by " + NUM_SORT_EXPRESSIONS + " chosen fields:</h3>"));

        AjaxControlToolkit.ComboBox[] comboSort = new AjaxControlToolkit.ComboBox[NUM_SORT_EXPRESSIONS];
        for (int i = 0; i < NUM_SORT_EXPRESSIONS; i++) {
            pnlSort.Controls.Add(new LiteralControl("<div>"));
            Label lblSort = new Label();
            lblSort.CssClass = "lblSort";
            if (i == 0) {
                lblSort.Text = "Sort first by: ";
            } else if (i == NUM_SORT_EXPRESSIONS - 1) {
                lblSort.Text = "Sort finally by: ";
            } else {
                lblSort.Text = "Sort then by: ";
            }
            pnlSort.Controls.Add(lblSort);

            pnlSort.Controls.Add(new LiteralControl("<div>"));
            comboSort[i] = new AjaxControlToolkit.ComboBox();
            comboSort[i].ID = comboSortIds[i];
            comboSort[i].CssClass = "comboBoxInsideModalPopup";
            comboSort[i].AutoPostBack = false;
            comboSort[i].DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
            comboSort[i].AutoCompleteMode = AjaxControlToolkit.ComboBoxAutoCompleteMode.SuggestAppend;
            pnlSort.Controls.Add(comboSort[i]);
            pnlSort.Controls.Add(new LiteralControl("</div>"));


            RadioButton rdbtnAsc = new RadioButton();
            rdbtnAsc.ID = radioAsc[i];
            rdbtnAsc.Text = "Ascending";
            rdbtnAsc.GroupName = groupNameBase + i;
            rdbtnAsc.Checked = true;
            rdbtnAsc.CssClass = "popLineRadioBtns";
            pnlSort.Controls.Add(rdbtnAsc);

            RadioButton rdbtnDesc = new RadioButton();
            rdbtnDesc.ID = radioDesc[i];
            rdbtnDesc.Text = "Descending";
            rdbtnDesc.GroupName = rdbtnAsc.GroupName;
            rdbtnDesc.CssClass = rdbtnAsc.CssClass;
            pnlSort.Controls.Add(rdbtnDesc);
            pnlSort.Controls.Add(new LiteralControl("</div>"));
            //pnlSort.Controls.Add(new LiteralControl("<br /> <br />"));
        }

        pnlSort.Controls.Add(new LiteralControl("<br />"));

        Button btnOkSort = new Button();
        btnOkSort.ID = btnOkSortId;
        btnOkSort.Text = "Sort";  //   size  ??
        btnOkSort.Width = 60;
        btnOkSort.Height = 25;
        pnlSort.Controls.Add(btnOkSort);
        Button btnCancelSort = new Button();
        btnCancelSort.ID = btnCancelSortId;
        btnCancelSort.Text = "Cancel";
        pnlSort.Controls.Add(btnCancelSort);

        return pnlSort;
    }
   
    public static AjaxControlToolkit.ModalPopupExtender GenerateSortExtender() {
        AjaxControlToolkit.ModalPopupExtender extender = new AjaxControlToolkit.ModalPopupExtender();
        extender.ID = EXTENDER_SORT_ID;
        extender.BackgroundCssClass = "modalBackground";
        extender.TargetControlID = BTN_SORT;
        extender.PopupControlID = PNL_SORT;
        extender.BehaviorID = BEHAVIOR_SORT;
        extender.DropShadow = true;
        return extender;
    }

    public static AjaxControlToolkit.ModalPopupExtender GenerateSortExtender2() {
        AjaxControlToolkit.ModalPopupExtender extender = GenerateSortExtender();
        extender.ID = EXTENDER_SORT_ID2;
        extender.TargetControlID = BTN_SORT2;
        extender.PopupControlID = PNL_SORT2;
        extender.BehaviorID = BEHAVIOR_SORT2;
        return extender;
    }

    public static Panel GenerateNewNamePanel(string labelText) {
        Panel pnlNewName = new Panel();
        pnlNewName.ID = PNL_NEW_NAME;
        pnlNewName.CssClass = "popPanel";
        pnlNewName.Attributes.Add("style", "display: none;");

        pnlNewName.Controls.Add(new LiteralControl("<div class='popHeader'><span class='popHeaderText'>Name</span></div>"));
        pnlNewName.Controls.Add(new LiteralControl("<div class='popMessage'>"));

        Label label = new Label();
        label.ID = LBL_NEW_NAME;
        label.Text = "Enter new description name for the copied record: ";
        label.Text = labelText;
        pnlNewName.Controls.Add(label);

        TextBox txtBox = new TextBox();
        txtBox.ID = TXT_NEW_NAME;
        txtBox.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
        pnlNewName.Controls.Add(txtBox);

        pnlNewName.Controls.Add(new LiteralControl("</div>"));

        pnlNewName.Controls.Add(new LiteralControl("<div class='popSubmit'>"));

        Button btnOk = new Button();
        btnOk.ID = BTN_OK_NEW_NAME;
        btnOk.Text = "Ok";
        btnOk.Attributes.Add("style", "width:55px; margin-right: 10px;");
        btnOk.OnClientClick = "HidePopup('modalPopupNewNameBehavior'); return true;";
        pnlNewName.Controls.Add(btnOk);

        Button btnCancel = new Button();
        btnCancel.ID = BTN_CANCEL_NEW_NAME;
        btnCancel.Text = "Cancel";
        btnCancel.OnClientClick = "HidePopup('modalPopupNewNameBehavior'); return false;";
        pnlNewName.Controls.Add(btnCancel);

        HiddenField mode = new HiddenField();
        mode.ID = HIDDEN_MODE;
        pnlNewName.Controls.Add(mode);

        pnlNewName.Controls.Add(new LiteralControl("</div>"));

        return pnlNewName;
    }

    

    public static AjaxControlToolkit.ModalPopupExtender GenerateNewNameExtender() {
        AjaxControlToolkit.ModalPopupExtender extender = new AjaxControlToolkit.ModalPopupExtender();
        extender.ID = EXTENDER_NEW_NAME;
        extender.TargetControlID = BTN_DUMMY3;
        extender.PopupControlID = PNL_NEW_NAME;
        extender.BehaviorID = "modalPopupNewNameBehavior";
        extender.BackgroundCssClass = "modalBackground";
        extender.DropShadow = false;
        return extender;
    }
    
     
}