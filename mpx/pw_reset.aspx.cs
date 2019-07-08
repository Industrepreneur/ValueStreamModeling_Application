using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pw_reset : System.Web.UI.Page {

    private int LINK_EXPIRATION_MINS = 10;

    LogFiles logFiles;

    protected void Page_Load(object sender, EventArgs e) {
        if (!Page.IsPostBack) {
            string link = Request.QueryString.ToString();
            if (GetPasswordResetLinkId(link) == -1) {
                Response.Redirect(DbUse.LOGOUT_PAGE, true);
            } else {
                hdnLink.Value = link;
                ClickOnEnter(btnChangePwd.ClientID, txtPwdNew);
                ClickOnEnter(btnChangePwd.ClientID, txtPwdNewConf);
                btnChangePwd.Attributes.Add("onclick", "this.disabled = true;");
                pnlInfo.Attributes.Add("style", "display:none;");
                txtPwdNew.Focus();
            }
            
        }
    }

    private void ClickOnEnter(string btnClientId, TextBox outerControl) {
        string script = "doFocus('" + btnClientId + "',event)";
        outerControl.Attributes.Add("onkeydown", script);
    }

    protected override void OnInit(EventArgs e) {
        logFiles = new LogFiles();
        base.OnInit(e);
    }

    protected void btnChangePwd_Click(object sender, EventArgs e) {
        string link = hdnLink.Value;
        if (ChangePassword(link, txtPwdNew.Text)) {
            SetFocus(btnOk.ClientID);
            modalInfo.Show();
        }
        
    }

    public bool ChangePassword(string link, string pswdNew) {
        bool updated = false;
        int id = GetPasswordResetLinkId(link);
        if (id != -1) {
            string hash = PasswordHash.PasswordHash.CreateHash(pswdNew);
            updated = DbUse.RunMysql("UPDATE userlog SET usercode = '" + hash + "' WHERE id = " + id + ";");
            if (updated) {
                try {
                    string logBody = "Password was changed for\n";
                    logBody += "Username: " + DbUse.GetMysqlDatabaseField("username", "id", id, "userlist") + "\n";
                    logBody += "User IP: " + System.Web.HttpContext.Current.Request.UserHostAddress + "\n";
                    MailInfo.SendMail(logBody, MailInfo.PASSWORD_RESET_ACTION);
                } catch (Exception ex) {
                    logFiles.ErrorLog(ex);
                }
            }
        }
        return updated;
    }

    public void SetFocus(string clientId) {
        string script = "<SCRIPT language='javascript' type='text/javascript'>function fnFocus() { eval(\"document.getElementById('" + clientId + "').focus()\") } setTimeout(\"fnFocus()\",200);</SCRIPT>";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "print_script", script);
    }

    public int GetPasswordResetLinkId(string link) {
        string sqlString = "SELECT id, tempHash, mail, expiry FROM usercred WHERE tempHash = '" + link + "';";
        int id = -1;

        ADODB.Connection conn = new ADODB.Connection();
        ADODB.Recordset rec = new ADODB.Recordset();
        bool openedCon = DbUse.OpenAdoMysql(conn);
        bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

        try {
            if (!openedCon || !openedRec) {
                throw new Exception("Error in opening database/dataset.");
            } else if (rec.EOF) {
                throw new Exception("Password reset link does not exist.");
            } else {
                long linkTicks = long.Parse(rec.Fields["expiry"].Value.ToString());
                if (!IsLinkValid(linkTicks)) {
                    throw new Exception("Password reset link has expired.");
                }
                id = int.Parse(rec.Fields["id"].Value.ToString());
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
        } finally {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
        }

        return id;
    }

    public bool IsLinkValid(long linkTicks) {
        return ((DateTime.Now.Ticks - linkTicks)/DbPage.NANOSEC_100_IN_MINUTE) < LINK_EXPIRATION_MINS;
    }

    
    protected void btnOk_Click(object sender, EventArgs e) {
        Response.Redirect(DbUse.LOGOUT_PAGE, true);
    }
}