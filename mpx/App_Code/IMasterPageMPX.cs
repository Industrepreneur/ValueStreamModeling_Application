using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for IMasterPageMPX
/// </summary>
public interface IMasterPageMPX
{
	void setUser(string username);

    string getUser();

    void setUserdir(string userdir);

    void setModel(string modelDb);

    void ResetModel();

    void passCurrentModelName(string currentModel);

    void setLastLogin(string lastLogin);

    void setLogoutMessage(string logoutMessage);

    string GetCurrentModel();

    void ShowErrorMessage();

    void ShowErrorMessage(string message);

    void ShowErrorMessageAndFocus(string message, string clientIdFocus);

    void ShowInfoMessage(string message);

    void ShowInfoMessage_Post(string message);

    void ShowErrorMessage_Post(string message);

    void HideMPXpopups();

    void SetFocus(string clientId);

    void ClickOnEnter(string btnClientId);

    void ClickOnEnter(string btnClientId, TextBox outerControl);

    void ClickOnEnter(string btnClientId, AjaxControlToolkit.ComboBox outerControl);

    void ClickOnEnterF(string btnClientId, TextBox outerControl) ;

    void SetFocus2(string clientId);

    string GetLastLogin();

    void SetModelModified(bool modified);

    void PassModelModified(bool modified);

    void MarkSavedModel();

    string GetFullCurrentModelLabel();

    string GetFullCurrentAnalysisLabel();

    string GetCurrentWhatif();

    string GetFullCurrentWhatif();

    string GetFullCurrentWhatifLabel();

    void PassCurrentWhatifName(string currentWhatif);

    void SetCurrentWhatifLabel();

    void SetCurrentAnalysisLabel();

    void PassCurrentAnalysisName(string currentAnalysis);

    void HideLoadingPopup();

    void SetHelpSheet(string helpSheetFile, string helpSheetTitle);
}