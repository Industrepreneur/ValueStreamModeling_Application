using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

/// <summary>
/// Summary description for PageControls
/// </summary>
public class PageControls
{
	public const string INFO_POPUP_ID = "pnlInfo";
    public const string COPY_TO_CLIPBOARD_POPUP_ID = "pnlCopyToClipboard";
    public const string COPY_TO_CLIPBOARD_EXTENDER = "extenderCopyToClipboard";
    public const string COPY_BEHAVIOR = "behaviorCopy";
    public const string INFO_LABEL_ID = "lblInfo";
    public const string INFO_EXTENDER = "extenderInfo";
    public const string BTN_DUMMY_INFO = "btnDummyInfo";
    public const string INPUT_COPY_TABLE = "inputCopyTable";
    public const string BTN_COPY_TABLE = "btnCopyTablePopup";

    public const string RDBTN_WITH_HEADER = "rdbtnTableWithHeader";
    public const string RDBTN_WITHOUT_HEADER = "rdbtnTableWithoutHeader";
    public const string CHECK_SELECT_ALL = "checkSelectAll";
    public const string BTN_DONE = "btnCopyDone";

    public const string BTN_INSERT_MULTIPLE_WARNING_OK = "btnInsertMultipleWarningOk";
    public const string BEHAVIOR_MULTIPLE_INSERT = "multipleInsertWarningBehavior";

    

    public static Panel generateInfoPanel() {
        Panel panel = new Panel();
        panel.ID = PageControls.INFO_POPUP_ID;
        panel.CssClass = "warningPanel";
        panel.Controls.Add(new LiteralControl("<h3>MPX Multiple Insert Warning</h3>"));
        panel.Controls.Add(new LiteralControl("One or more lines in the pasted table could not be inserted. This may be caused by missing/extra/invalid fields or duplicate name. Please, correct the leftover lines and try to insert them again."));
        panel.Controls.Add(new LiteralControl("<br /> <br />"));
        Button btnOk = new Button();
        btnOk.Text = "Ok";
        btnOk.ID = BTN_INSERT_MULTIPLE_WARNING_OK;
        btnOk.OnClientClick = "HidePopup('" + BEHAVIOR_MULTIPLE_INSERT + "');return false;";
        btnOk.Width = 60;
        panel.Controls.Add(btnOk);
        panel.Attributes.Add("style", "display:none;");
        return panel;
    }

    public static AjaxControlToolkit.ModalPopupExtender generateInfoExtender() {
        AjaxControlToolkit.ModalPopupExtender extender = new AjaxControlToolkit.ModalPopupExtender();
        extender.ID = INFO_EXTENDER;
        extender.PopupControlID = INFO_POPUP_ID;
        extender.TargetControlID = BTN_DUMMY_INFO;
        extender.BackgroundCssClass = "modalBackground";
        extender.BehaviorID = BEHAVIOR_MULTIPLE_INSERT;
        extender.DropShadow = true;
        return extender;
    }

    public static Panel generateCopyPanel() {
        Panel panel = new Panel();
        panel.ID = PageControls.COPY_TO_CLIPBOARD_POPUP_ID;
        panel.CssClass = "copyPanel";
        panel.Controls.Add(new LiteralControl("<h3>Copy Table</h3>"));
        panel.Controls.Add(new LiteralControl("Please select lines and press Ctrl+C to copy the table to clipboard:"));
        panel.Controls.Add(new LiteralControl("<br /> <br />"));

        RadioButton rdbtnWithHeader = new RadioButton();
        rdbtnWithHeader.ID = RDBTN_WITH_HEADER;
        rdbtnWithHeader.Text = "Table with headers";
        rdbtnWithHeader.GroupName = "rdbtnsCopyTable";
        rdbtnWithHeader.Checked = true;
        rdbtnWithHeader.AutoPostBack = true;
        rdbtnWithHeader.CssClass = "popLineRadioBtns";
        panel.Controls.Add(rdbtnWithHeader);

        RadioButton rdbtnWithoutHeader = new RadioButton();
        rdbtnWithoutHeader.ID = RDBTN_WITHOUT_HEADER;
        rdbtnWithoutHeader.Text = "Table without headers";
        rdbtnWithoutHeader.GroupName = rdbtnWithHeader.GroupName;
        rdbtnWithoutHeader.AutoPostBack = true;
        rdbtnWithoutHeader.CssClass = rdbtnWithHeader.CssClass;
        panel.Controls.Add(rdbtnWithoutHeader);

        panel.Controls.Add(new LiteralControl("<br /> <br />"));

        CheckBox checkSelectAll = new CheckBox();
        checkSelectAll.ID = CHECK_SELECT_ALL;
        checkSelectAll.Text = "Select All";
        checkSelectAll.Checked = false;
        panel.Controls.Add(checkSelectAll);

        panel.Controls.Add(new LiteralControl("<br /> <br />"));

        TextBox txtCopy = new TextBox();
        txtCopy.ID = INPUT_COPY_TABLE;
        txtCopy.TextMode = TextBoxMode.MultiLine;
        txtCopy.Width = 990;
        txtCopy.Height = 200;
        panel.Controls.Add(txtCopy);
        
        panel.Controls.Add(new LiteralControl("<br /> <br />"));
        Button btnDone = new Button();
        btnDone.ID = BTN_DONE;
        btnDone.Text = "Done";
        btnDone.Width = 60;
        btnDone.OnClientClick = "HidePopup('" + COPY_BEHAVIOR + "'); return true;";
        panel.Controls.Add(btnDone);
        panel.Attributes.Add("style", "display:none");
        return panel;
    }

    public static AjaxControlToolkit.ModalPopupExtender generateCopyExtender() {
        AjaxControlToolkit.ModalPopupExtender extender = new AjaxControlToolkit.ModalPopupExtender();
        extender.ID = COPY_TO_CLIPBOARD_EXTENDER;
        extender.BehaviorID = COPY_BEHAVIOR;
        extender.TargetControlID = BTN_COPY_TABLE;
        extender.PopupControlID = COPY_TO_CLIPBOARD_POPUP_ID;
        extender.BackgroundCssClass = "modalBackground";
        extender.DropShadow = true;
        return extender;
    }
}