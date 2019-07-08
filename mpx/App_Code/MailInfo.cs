using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

/// <summary>
/// Summary description for MailInfo
/// </summary>
public class MailInfo {
    public const int EXEC_DOWNLOAD = 1;
    public const int UNHANDLED_EXCEPTION = 0;
    public const int PAGE_NOT_FOUND = 2;
    public const int PASSWORD_RESET_REQUEST = 3;
    public const int PASSWORD_RESET_ACTION = 4;
    public const int USER_DELETED = 5;
    public const int USER_CREATED = 6;
    public const int GROUP_CREATED = 7;
    public const int GROUP_DELETED = 8;
    public const int GROUP_USER_ADDED = 9;
    public const int GROUP_USER_DELETED = 10;
    public const int NEW_DOWNLOAD_INFO = 11;

    public MailInfo() {
        //
        // TODO: Add constructor logic here
        //
    }

    private const string EXEC_DOWNLOAD_EMAIL = "execDownloadEmail";
    private const string DEVELOPER_EMAIL = "developerEmail";
    private const string PASSWORD_RESET_EMAIL = "passwordResetEmail";

    private static string[] GetToEmailAddresses(int infoType) {
        string emailKey = DEVELOPER_EMAIL;
        if (infoType == EXEC_DOWNLOAD) {
            emailKey = EXEC_DOWNLOAD_EMAIL;
        } else if (infoType == PASSWORD_RESET_REQUEST || infoType == PASSWORD_RESET_ACTION || infoType == USER_DELETED || infoType == USER_CREATED || infoType == GROUP_CREATED || infoType == GROUP_DELETED) {
            emailKey = PASSWORD_RESET_EMAIL;
        }
        string mails = ConfigurationManager.AppSettings[emailKey];
        string[] emailAddresses = new string[0];
        if (!string.IsNullOrEmpty(mails)) {
            emailAddresses = mails.Split(';');
        }
        return emailAddresses;
    }

    public static string GetFromEmailAddress() {
        return "btdmpx@gmail.com";
    }

    public static void SendMailMessage(MailMessage message) {
        SmtpClient client = new SmtpClient();
        client.EnableSsl = true;
        client.Send(message);
    }

    public static void SendMail(string body, int infoType) {
        SendMail(body, infoType, (string[]) null, null);
    }

    public static void SendMail(string body, int infoType, string mail) {
        SendMailx(body, infoType, mail, null);
    }

    public static void SendMailx(string body, int infoType, string mail, Attachment attachment) {
        SendMail(body, infoType, new string[] { mail }, attachment);
    }

    public static void SendMail(string body, int infoType, string[] emails) {
        SendMail(body, infoType, emails, null);
    }

    public static void SendMail(string body, int infoType, string[] emails, Attachment attachment) {
        MailMessage message = new MailMessage();
        message.From = new MailAddress("btdmpx@gmail.com");

        string subject = "";

        switch (infoType) {
            case UNHANDLED_EXCEPTION:
                subject = "Unhandled Exception in Value Stream Modeling website";
                break;
            case PAGE_NOT_FOUND:
                subject = "Page not found in Value Stream Modeling website";
                break;
            case EXEC_DOWNLOAD:
                subject = "MPX Executable Download";
                break;
            case PASSWORD_RESET_ACTION:
                subject = "Value Stream Modeling Password Reset";
                break;
            case PASSWORD_RESET_REQUEST:
                subject = "Value Stream Modeling Password Reset Request";
                break;
            case USER_DELETED:
                subject = "Value Stream Modeling User Was Deleted";
                break;
            case USER_CREATED:
                subject = "Value Stream Modeling User Was Created";
                break;
            case GROUP_DELETED:
                subject = "Value Stream Modeling Group Was Deleted";
                break;
            case GROUP_CREATED:
                subject = "Value Stream Modeling Group Was Created";
                break;
            case GROUP_USER_ADDED:
                subject = "Value Stream Modeling Group User Invitation";
                break;
            case GROUP_USER_DELETED:
                subject = "Value Stream Modeling Group User Removal";
                break;
            case NEW_DOWNLOAD_INFO:
                subject = "MPX DEMO DOWNLOAD INFO";
                break;
            default:
                return;

        }
        string[] emailAddresses;
        if (emails != null) {
            emailAddresses = emails;
        } else {
            emailAddresses = GetToEmailAddresses(infoType);
        }


        foreach (string email in emailAddresses) {
            if (!string.IsNullOrEmpty(email)) {
                message.To.Add(new MailAddress(email));
            }
        }
        message.Subject = subject;
        message.Body = body;

        if (attachment != null) {
            message.Attachments.Add(attachment);
        }

        SendMailMessage(message);
    }
}