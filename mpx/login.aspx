<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="LoginPage" %>

<%--TODO:
-Popup for too many login attempts or blank form; move error-label to invisible div
-forgot password page (reset password and email to registered email, or send link for them to create new password? Always better to send password?)
    -Forgot Username; email has been sent to registered email with your username.
    -Forgot Password; n=password has been reset and new password has been sent to registered email
-figure out why 'required' and 'pattern' input attributes do not work (see validity checks)
    -problem is the ASP.Net script manager already has postBack function
-add back logFiles
-use .validity checks (patternMismatch, valueMissing, valid
-find better way to limit attempts that does not rely on hidden container
    -should use cookie or sessionID to validate attempts on server side; use database and add column for attempts and expiry
    -or investigate how to link var to SessionID and autocleanse
--%>

<!DOCTYPE HTML>
<%--PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Value Stream Modeling</title>
    <link href="login.css" rel="stylesheet" type="text/css" />
    <link id="Link2" rel="shortcut icon" type="image/x-icon" href="~/favicon.ico" />
    <link id="Link3" runat="server" rel="icon" href="~/favicon.ico" type="image/ico" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script type="text/javascript">

        //replaces the registering of client script 
        addEventListener("pageshow", function () {
            //checkCookiesEnabled();
            ValidateForm();

        });

        function doFocus(buttonName, e) {
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                var btn = document.getElementById(buttonName);
                if (btn != null && !btn.disabled) {
                    btn.focus();
                }
            }
        }

        function chooseFocus(buttonLogin, buttonReset, e) {
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                var btn = document.getElementById(buttonLogin);
                divstyle = document.getElementById("divForgotPw").style.display;
                if (divstyle == "block" || divstyle == "") {
                    btn = document.getElementById(buttonReset);
                }

                if (btn != null && !btn.disabled) {
                    btn.focus();
                }
            }
        }

        function doClick(buttonName, e) {
            //the purpose of this function is to allow the enter key to 
            //point to the correct button to click.
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                //Get the button the user wants to have clicked
                var btn = document.getElementById(buttonName);
                if (btn != null) { //If we find the button click it
                    btn.click();
                    if (window.event) {
                        window.event.keyCode = 0;
                    } else {
                        e.which = 0;
                    }

                }
            }
        }

        //function checkCookiesEnabled() {
        //    try {
        //        document.cookie = "test";
        //        cookieEnabled = (document.cookie.indexOf("test") != -1) ? true : false;


        //        if (!cookieEnabled) {
        //            document.getElementById("lblError").innerHTML = "This website requires cookies to run, please enable cookies in your browser";
        //        }
        //        return cookieEnabled;
        //    } catch (e) {
        //        return true;
        //    }

        //}

        //This section of script is to perform client-side validation before allowing submit
        function ValidateName(x) {

            if (x.value == "" || x.value.match(/\s/)) {
                document.getElementById("lblUsername").style.color = "red"; //add validation for UserName
                document.getElementById("txtUsername").style.borderColor = "red";

            }
            else {

                //document.getElementById("lblUsername").style.color = ""; 
                //document.getElementById("txtUsername").style.borderColor = "";

            }

            ValidateForm();

        }
        function ValidatePassword(e) {
            if (e.value == "") {
                document.getElementById("lblPassword").style.color = "red"; //add validation for UserName
                document.getElementById("txtPassword").style.borderColor = "red";

            }
            else {
                //document.getElementById("lblPassword").style.color = ""; 
                //document.getElementById("txtPassword").style.borderColor = "";

            }

            ValidateForm();

        }

        function ValidateForm() {
            //var thisIsATest = document.getElementById("txtUsername").innerHTML;

            if (document.getElementById("txtUsername").value == "" || document.getElementById("txtUsername").value.match(/\s/) || document.getElementById("txtPassword").value == "") {
                document.getElementById("btnSubmit").disabled = "disabled";


                //if (document.getElementById("hidden1").value > 3) {
                //    document.getElementById("btnSubmit").disabled = "disabled";
                //    document.getElementById("btnSubmit").innerHTML = "LOCKED";
                //    document.getElementById("txtUsername").readOnly = "readOnly";
                //    document.getElementById("txtPassword").readOnly = "readOnly";
                //}
            }
            else {
                document.getElementById("btnSubmit").disabled = "";
            }
        }

    </script>



    <%--this is the google recaptcha submission--%>
<%--    <script src="https://www.google.com/recaptcha/api.js" async="async" defer="defer"></script>
    <script>
            function onSubmit(token) {

                document.getElementById("loginForm").submit(); //this seems to be working 
            }
    </script>--%>
    <script>
        $(document).ready(function () { $("input").attr("autocomplete", "off"); });
    </script>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>


<body>


    <header>
        <div class="headerLeft">
            <img src="Images/white-single-row.png" alt="logo" />
        </div>


        <div class="headerRight">
            <form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top">
                <input type="hidden" name="cmd" value="_s-xclick">
                <input type="hidden" name="hosted_button_id" value="GNUZAVPQ824TU">
                <button id="btnDonate" type="submit" class="" name="submit" tabindex="4">Donate</button>

            </form>

        </div>
    </header>

    <main>

        <div class="login-box">

            <div id="textbox1" class="login-header">
                <span class="login-header-text">LOGIN</span>
            </div>

            <div class="login-body">
                <form id="loginForm" name="loginForm" method="post" action="login.aspx" runat="server">

                    <div id="login-text">
                        <div id="login-user">
                            <table id="user-table">
                                <tr>
                                    <td id="login-username">
                                        <label id="lblUsername" for="txtUsername">Username</label>
                                    </td>
                                </tr>
                                <tr>

                                    <td>
                                        <input maxlength="30" autocomplete="off" tabindex="1" type="text" id="txtUsername" name="txtUsername" required="required" autofocus="autofocus" onblur="return ValidateName(txtUsername)" />
                                        <script>

                                            document.getElementById("txtUsername").addEventListener("blur", function () {
                                                document.getElementById("lblUsername").style.color = "rgba(0, 0, 0, 0.54)";
                                                document.getElementById("txtUsername").style.borderColor = "rgba(0, 0, 0, 0.54)";
                                                ValidateName(document.getElementById("txtUsername"));
                                                this.placeholder = "Username";
                                                if (this.value == "") {
                                                    document.getElementById("lblUsername").style.opacity = "0";
                                                }
                                                else {
                                                    document.getElementById("lblUsername").style.opacity = "1";
                                                }

                                            }, true);
                                            document.getElementById("txtUsername").addEventListener("focus", function () {
                                                document.getElementById("lblUsername").style.color = "navy";
                                                document.getElementById("txtUsername").style.borderColor = "navy";
                                                ValidateName(document.getElementById("txtUsername"));
                                                this.placeholder = "";
                                                document.getElementById("lblUsername").style.opacity = "1";
                                            });


                                            document.getElementById("txtUsername").addEventListener("input", function () {
                                                document.getElementById("lblUsername").style.color = "navy";
                                                document.getElementById("txtUsername").style.borderColor = "navy";
                                                ValidateName(document.getElementById("txtUsername"));
                                                this.placeholder = "";

                                                document.getElementById("lblUsername").style.opacity = "1";
                                            }, true);
                                        </script>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="login-password">
                            <table id="password-table">
                                <tr>
                                    <td id="login-password">
                                        <label id="lblPassword" for="txtPassword">Password</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <input type="password" autocomplete="off" id="txtPassword" name="txtPassword" tabindex="2" required="required" onblur="return ValidatePassword(txtPassword)" onfocus="return ValidatePassword(txtPassword)" />
                                        <script>
                                            document.getElementById("txtPassword").addEventListener("blur", function () {
                                                document.getElementById("lblPassword").style.color = "rgba(0, 0, 0, 0.54)";
                                                document.getElementById("txtPassword").style.borderColor = "rgba(0, 0, 0, 0.54)";
                                                ValidatePassword(document.getElementById("txtPassword"));

                                                this.placeholder = "Password";
                                                if (this.value == "") {
                                                    document.getElementById("lblPassword").style.opacity = "0";
                                                }
                                                else {
                                                    document.getElementById("lblPassword").style.opacity = "1";
                                                }

                                            }, true);
                                            document.getElementById("txtPassword").addEventListener("focus", function () {
                                                document.getElementById("lblPassword").style.color = "navy";
                                                document.getElementById("txtPassword").style.borderColor = "navy";
                                                ValidatePassword(document.getElementById("txtPassword"));

                                                this.placeholder = "";
                                                document.getElementById("lblPassword").style.opacity = "1";
                                            });


                                            document.getElementById("txtPassword").addEventListener("input", function () {
                                                document.getElementById("lblPassword").style.color = "navy";
                                                document.getElementById("txtPassword").style.borderColor = "navy";
                                                ValidatePassword(document.getElementById("txtPassword"));

                                                this.placeholder = "";
                                                document.getElementById("lblPassword").style.opacity = "1";
                                            }, true);
                                        </script>
                                    </td>
                                </tr>
                            </table>

                            <a id="btnForgotPw" onclick="showHideForgotPw();">Forgot Password?</a>

                        </div>
                    </div>

                    <%--container for any errors--%>
                    <div id="login-error">
                        <asp:Label
                            ID="lblError"
                            CssClass="lblErrorHidden"
                            runat="server"
                            Text="no error">
                        </asp:Label>
                    </div>

                    <%--container for submit button--%>
                    <div class="login-submit">
                 
                        <%--THIS IS THE PRODUCTION BUTTON--%>
                        <%--<button id="btnSubmit" type="submit" class="g-recaptcha" data-sitekey="6LftHzcUAAAAAEW0eN35pwFOsi1D235NNZ2BPh2B" data-callback="onSubmit" name="btnSubmit" tabindex="3" disabled="disabled">Submit</button>--%>

                        <%--THIS IS THE DEVELOPMENT BUTTON--%>
                        <%--<button id="btnSubmit" type="submit" class="g-recaptcha" data-sitekey="6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI" data-callback="onSubmit" name="btnSubmit" tabindex="3" disabled="disabled">Submit</button>--%>

                        <%--THIS IS THE BORING SUBMIT BUTTON--%>
                        <button id="btnSubmit" type="submit" class="" name="btnSubmit" tabindex="3" disabled="disabled">Submit</button>

                    </div>
                    <%-- <div style="display: none;">
                        <asp:HiddenField ID="hidden1" runat="server" Value="0"></asp:HiddenField>
                    </div>--%>
                </form>


            </div>
            <%--end div for login body--%>
        </div>
        <%--end of login box--%>

        <%--        <div id="error-label">

        </div>--%>

        <%--KO-FI LINK--%>
        <%--        <div>
            <script type='text/javascript' src='https://ko-fi.com/widgets/widget_2.js'></script><script type='text/javascript'>kofiwidget2.init('Donate', '#4646b8', 'B0B55WV9'); kofiwidget2.draw();</script> 
        </div>--%>
    </main>
    <%--end of content--%>

    <footer>

        <%--<div id="footer-copyright"><span>Copyright &copy; 2017 FACTORY CONCEPTS, LLC.</span></div>--%>
        <div id="footer-copyright"><span>Creative Commons NC 2018</span></div>

    </footer>

</body>

</html>

