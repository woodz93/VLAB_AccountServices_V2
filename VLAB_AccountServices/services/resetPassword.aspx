<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.resetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>UHMC AD Password Reset</title>
    <link rel="stylesheet" href="assets/css/main.css" defer="true" />
    <script src="assets/js/server.js" defer></script>
    <script src="assets/js/main.js" defer></script>
</head>
<body>
    <div id="bg" class="bg"></div>
    <form id="form_main" class="container" runat="server" method="post">
        <div>
            <label for="username">Username: </label><asp:textbox id="username" Text="" runat="server" name="username" type="text" value="" placeholder="UH Username..."></asp:textbox>
            <br />
            <label for="password">Password: </label><asp:textbox id="password" Text="" runat="server" name="password" type="password" value="" placeholder="UH Password..."></asp:textbox>
            <button type="button" class="peak" onclick="peak(this)" data-ref="password"></button>
            <br />
            <label for="password-confirm">Confirm Password: </label><asp:textbox id="password_confirm" Text="" runat="server" name="password_confirm" type="password" value="" placeholder="Confirm password..."></asp:textbox>
            <button type="button" class="peak" onclick="peak(this)" data-ref="password_confirm"></button>
            <br />
            <asp:button id="submit_btn" CssClass="submit_btn" name="submit_btn" Text="Reset Password" Onclick="processPassword" runat="server"></asp:button>
        </div>
        <asp:label id="status" Text="" runat="server" class="status">Console data will be displayed here.</asp:label>
    </form>
</body>
</html>
