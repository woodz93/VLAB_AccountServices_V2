<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.resetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>UHMC AD Password Reset</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<link rel="stylesheet" href="assets/css/main.css" defer="true" />
	<script src="assets/js/server.js" defer></script>
	<script src="assets/js/main.js" defer></script>
</head>
<body>
	<div id="bg" class="bg"></div>
	<form id="form_main" class="container" runat="server" method="post">
		<h1 class="title">Account Services</h1>
		<br>
		<div>
			<table class="container">
				<tr>
					<td><label for="username">Username: </label></td>
					<td><asp:textbox id="username" Text="" runat="server" name="username" type="text" value="" placeholder="UH Username..."></asp:textbox></td>
				</tr>
				<tr>
					<td><label for="password">Password: </label></td>
					<td><asp:textbox id="password" Text="" runat="server" name="password" type="password" value="" placeholder="UH Password..."></asp:textbox><button type="button" class="peak" onclick="peak(this)" data-ref="password"></button></td>
				</tr>
				<tr>
					<td><label for="password-confirm">Confirm Password: </label></td>
					<td><asp:textbox id="password_confirm" Text="" runat="server" name="password_confirm" type="password" value="" placeholder="Confirm password..."></asp:textbox><button type="button" class="peak" onclick="peak(this)" data-ref="password_confirm"></button></td>
				</tr>
			</table>
			<br />
			<asp:Panel ID="group_container" Visible="true" runat="server">
				<asp:Label ID="groups_label" for="groups" runat="server">Available Groups:</asp:Label>
				<br />
				<asp:CheckBoxList ID="groups" CssClass="list groups" name="groups" runat="server"></asp:CheckBoxList>
			</asp:Panel>
			<asp:Panel ID="dev" Visible="false" runat="server">
				<label for="debug">Debug:</label>
				<asp:CheckBox ID="debug" name="debug" runat="server" />
			</asp:Panel>
			<asp:button id="submit_btn" CssClass="submit_btn" name="submit_btn" Text="[ERROR]" Onclick="processPassword" runat="server"></asp:button>
			<asp:label id="status" Text="" runat="server" class="status">Response messages will display here.</asp:label>
		</div>
	</form>
</body>
</html>
