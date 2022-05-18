<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>UHMC AD Password Reset</title>
    <link rel="stylesheet" defer />
</head>
<body>
    <form id="form1" runat="server" method="post" action="resetPassword.aspx.cs">
        <div>
            <label for="username">Username: </label><input id="username" name="username" type="text" value="" placeholder="UH Username..." />
            <br />
            <label for="password">Password: </label><input id="password" name="password" type="password" value="" placeholder="UH Password..." />
        </div>
    </form>
</body>
</html>
