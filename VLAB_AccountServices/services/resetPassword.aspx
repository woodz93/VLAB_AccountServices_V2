<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>UHMC AD Password Reset</title>
    <link rel="stylesheet" href="assets/css/main.css" defer="true" />
    <script src="assets/js/main.js" defer></script>
</head>
<body>
    <div id="bg" class="bg"></div>
    <div id="form_main" class="container" runat="server" method="post" action="assets/css/main.css">
        <div>
            <label for="username">Username: </label><input id="username" name="username" type="text" value="" placeholder="UH Username..." />
            <br />
            <label for="password">Password: </label><input id="password" name="password" type="password" value="" placeholder="UH Password..." />
            <br />
            <label for="password-confirm">Confirm Password: </label><input id="password-confirm" name="password-confirm" type="password" value="" placeholder="Confirm password..." />
            <br />
            <button id="submit" type="submit" onclick="submitPasswordResetApplication()">Reset Password</button>
        </div>
        <div id="status" class="status"></div>
    </div>
</body>
</html>
