<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VLAB_AccountServices.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Loading...</title>
    <script src="assets/js/server.js" defer></script>
    <script src="assets/js/default.js" defer></script>
</head>
<body>
    <!--
        -receive UH username here<br />
        -do a test here to see if account exists in AD<br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;yes? redir to /services/resetPassword.aspx<br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;no? redir to /services/createAccount.aspx
    -->
    <div id="js-assets" hidden></div>
    <form id="form-main" runat="server"></form>
</body>
</html>
