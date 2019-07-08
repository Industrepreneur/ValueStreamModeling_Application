using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class account : DbPage {

    public account() {
        PAGENAME = "/account.aspx";
    }

    

    protected void btnSetEmail_Click(object sender, EventArgs e) {

        var mySessionID = HttpContext.Current.Session.SessionID;
            if (CheckCurrPassword(mySessionID, txtEmailPwd.Text) && ChangeEmail(mySessionID, txtEmail.Text)) {
                Master.ShowInfoMessage("Your email was saved successfully.");
            }
        
    }

    protected void btnChangePwd_Click(object sender, EventArgs e) {

        
        var mySessionID = HttpContext.Current.Session.SessionID;
        if (CheckCurrPassword(mySessionID, txtPwdCurr.Text) && ChangePassword(mySessionID, txtPwdNew.Text)) {//this is attempting to set the password as email?
                Master.ShowInfoMessage("Your password was changed successfully.");
            }
        
    }

    public bool ChangePassword(string cookieid, string pswdNew) {
        bool updated = false;
        string sqlString = "SELECT id FROM userlist WHERE sessionid = '" + cookieid + "';";
        int id = -1;

        conn = new ADODB.Connection();
        rec = new ADODB.Recordset();
        bool openedCon = DbUse.OpenAdoMysql(conn);
        bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

        try {
            if (!openedCon || !openedRec) {
                throw new Exception("An error has occured. Please try again.");
            } else if (rec.EOF) {
                throw new Exception("An internal error has occured. Missing data.");
            } else {
                id = int.Parse(rec.Fields["id"].Value.ToString());
                
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage(ex.Message);
            return false;
        } finally {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
        }
        string hash = PasswordHash.PasswordHash.CreateHash(pswdNew);
        updated = DbUse.RunMysql("UPDATE userlog SET usercode = '" + hash + "' WHERE id = " + id + ";");
        if (!updated) {
            Master.ShowErrorMessage("Password was not changed. Please try again.");
        }

        return updated;
    }

    public bool CheckCurrPassword(string cookieid, string psswd) {
        bool correct = false;
        string sqlString = "SELECT userlog.id, userlog.usercode, userlist.userid FROM userlog INNER JOIN userlist ON userlog.id = userlist.id WHERE userlist.sessionid = '" + cookieid + "';";

        conn = new ADODB.Connection();
        rec = new ADODB.Recordset();
        bool openedCon = DbUse.OpenAdoMysql(conn);
        bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

        try {
            if (!openedCon || !openedRec) {
                throw new Exception("An error has occured. Please try again.");
            } else if (rec.EOF) {
                DbUse.CloseAdoRec(rec);
                DbUse.CloseAdo(conn);
                Response.Redirect(LOGOUT_PAGE, true);
            } else {
                string correctHash = rec.Fields["usercode"].Value.ToString();
                if (!PasswordHash.PasswordHash.ValidatePassword(psswd, correctHash)) {
                    throw new Exception("Current password is incorrect.");
                } else {
                    correct = true;
                }
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage(ex.Message);
        } finally {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
        }

        return correct;
    }

    public bool ChangeEmail(string cookieid, string email) {
        bool updated = false;
        string sqlString = "SELECT usercred.id, usercred.mail FROM usercred INNER JOIN userlist ON usercred.id = userlist.id WHERE userlist.sessionid = '" + cookieid + "';";

        conn = new ADODB.Connection();
        rec = new ADODB.Recordset();
        bool openedCon = DbUse.OpenAdoMysql(conn);
        bool openedRec = DbUse.OpenAdoRec(conn, rec, sqlString);

        try {
            if (!openedCon || !openedRec) {
                throw new Exception("An error has occured. Please try again.");
            } else if (rec.EOF) {
                throw new Exception("An internal error has occured. Missing data.");
            } else {
                rec.Fields["mail"].Value = email;
                rec.Update();
                updated = true;
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage(ex.Message);
        } finally {
            DbUse.CloseAdoRec(rec);
            DbUse.CloseAdo(conn);
        }

        return updated;
    }
}