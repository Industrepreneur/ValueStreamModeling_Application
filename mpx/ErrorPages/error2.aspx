<%@ Page Language="C#" AutoEventWireup="true" CodeFile="error2.aspx.cs" Inherits="ErrorPages_error2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="<%= ResolveClientUrl("../mpxstyle.css")%>" rel="stylesheet" type="text/css" />
    <link href='https://fonts.googleapis.com/css?family=Roboto:500,400' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Roboto+Condensed:400,300' rel='stylesheet' type='text/css'/>
    <link href='https://fonts.googleapis.com/css?family=Droid+Serif' rel='stylesheet' type='text/css'/>
    <title>Value Stream Modeling</title>
</head>
<body>
    
    <form style="padding-top: 0px; margin-top: 0px;" id="mpxForm" method="post" runat="server">
            
    <div class="page shadow">
        
        
            <div class="header">

                <div style="float:right;text-align:right; margin-top:-5px; font-weight:bold;margin-right:10px;">
                <div style="height: 35px; font-size: 13px; font-family: 'Roboto', Verdana;"><span style="color: #b9e0bb;">Build</span><span style="color:#f7b2b3;">-To-</span><span style="color: #b0c8e2;">Demand</span><span style="color: #d7d7d7;">, Inc.</span></div>
                <div>
                    
                </div>
                    </div>
                
                
                
               
                <img src="<%= ResolveClientUrl("../Images/logo.png")%>" height="40" style="margin-left:38%" class="mpxTitleImage" />
                <h1 class="mpxTitle" style="margin-bottom:5px;">Value Stream Modeling</h1>
                 
                 <div class="line" style="clear: both;"></div>
            </div>
            <div class="content" style="background-color:#FFFFFF;">
                <div style="padding:20px;">
                <h2>Unexpected Error</h2>
            <p>Oops... An unexpected error has occured. The website administrator has been notified. <a href="<%= ResolveClientUrl("../login.aspx")%>">Back to Value Stream Modeling</a></p>
                    
                    </div>
    </div>
    </div>
        

        <div class="page footer">
        
        <a href="http://www.build-to-demand.com"><img src="<%= ResolveClientUrl("../Images/logo.png")%>" style="height:68px;margin-left:auto;margin-right:auto; margin-top:20px;margin-bottom:0px;" /></a>
        <div style="margin-top:1px;">
            E-mail: <a href="mailto:Info@Build-To-Demand.com">Info@Build-To-Demand.com</a>
            <br />
            Copyright &copy; 2013  Build-To-Demand, Inc.
        </div>
            </div>
       
    </form>
</body>
</html>
