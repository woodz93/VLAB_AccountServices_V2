<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.resetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>UHMC AD Password Reset</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<link rel="stylesheet" href="assets/css/main.css" defer="true" />
	<script src="assets/js/server.js" defer></script>
	<script src="assets/js/main.js" defer></script>
	<script src="assets/css/Bootstrap/Bootstrap.js" defer></script>
</head>
<body>

	

	<div id="bg" class="bg"></div>
	<div class="container center-h center-v beval">
		<form id="form_main" class="container" runat="server" method="post">
			<div class="container content">
				<h1 class="title pos-rel center-h">Account Services</h1>
				<br>
				<div id="accordion" class="container large">

					<div class="card">
						<button data-bs-toggle="collapse" data-bs-target="#password_mgr_container" class="btn btn-primary center-h card-header collapsed" type="button">Password Management</button>
						<div id="password_mgr_container" class="collapse" data-bs-parent="#accordion">
							<table class="container">
								<tr>
									<td><label for="username">Username: </label></td>
									<td>
										<asp:textbox id="username" Text="" runat="server" name="username" type="text" value="" placeholder="UH Username..." CausesValidation="false"></asp:textbox>
										<button type="button" class="btn btn-info info" data-bs-toggle="modal" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-content="Your UH username/email.\nThis is determined from your UH login and cannot be changed in this form."></button>
									</td>
								</tr>
								<tr>
									<td><label for="password">Password: </label></td>
									<td>
										<asp:textbox id="password" Text="" runat="server" name="password" type="password" value="" placeholder="UH Password..." CausesValidation="false"></asp:textbox>
										<button type="button" class="btn btn-info peak" onclick="peak(this)" data-ref="password"></button>
										<button type="button" class="btn btn-info info" data-bs-toggle="modal" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-content="Enter a password that you'll use to login to your desktop."></button>
									</td>
								</tr>
								<tr>
									<td><label for="password-confirm">Confirm Password: </label></td>
									<td>
										<asp:textbox id="password_confirm" Text="" runat="server" name="password_confirm" type="password" value="" placeholder="Confirm password..." CausesValidation="false"></asp:textbox>
										<button type="button" class="btn btn-info peak" onclick="peak(this)" data-ref="password_confirm"></button>
										<button type="button" class="btn btn-info info" data-bs-toggle="modal" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-content="Confirm your password."></button>
									</td>
								</tr>
							</table>
							<br>
							<button class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)">Save Password</button>
							<br>
							<asp:label id="status" Text="" runat="server" class="status alert alert-dark center-h" CausesValidation="false">Response messages will display here.</asp:label>
						</div>
					</div>
					<div class="card">
						<button data-bs-toggle="collapse" data-bs-target="#vdi_container" class="btn btn-primary center-h card-header collapsed" CausesValidation="false" type="button">Available Desktops</button>
						<div id="vdi_container" class="collapse" data-bs-parent="#accordion">
							<asp:Panel ID="group_container" CssClass="groups container" Visible="false" runat="server" CausesValidation="false"></asp:Panel>
							<asp:Label ID="groups_label" CssClass="sub_title center-h" for="groups" runat="server" Visible="false" CausesValidation="false">Available Groups:</asp:Label>
							<br />
							<table class="table table-striped table-bordered">
								<tr>
									<th>Available Desktops <button type="button" class="btn btn-info info" data-bs-toggle="modal" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-content="Here, you can select the virtual desktops that you want access to.\nOnce you've submitted the form, you'll gain immediate access to the virtual desktops you have selected."></button></th>
									<th>My Desktops <button type="button" class="btn btn-info info" data-bs-toggle="modal" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-content="These are the virtual desktops that you already have access to."></button></th>
								</tr>
								<tr>
									<td>
										<asp:CheckBoxList ID="GroupsElement" CssClass="list groups pos-rel" name="groups" runat="server" CausesValidation="false"></asp:CheckBoxList>
									</td>
									<td>
										<asp:CheckBoxList ID="UserGroupsElement" CssClass="list groups pos-rel" name="groups" runat="server" CausesValidation="false"></asp:CheckBoxList>
									</td>
								</tr>
							</table>
							<br>
							<button class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)">Add Groups</button>
						</div>
					</div>

					<asp:Panel ID="dev" Visible="false" runat="server">
						<label for="debug">Debug:</label>
						<asp:CheckBox ID="debug" name="debug" runat="server" />
					</asp:Panel>
				</div>
			</div>
			<br>
			<asp:button id="submit_btn" CssClass="submit_btn btn btn-outline-primary btn-lg center-h submit_button hidden" name="submit_btn" Text="[ERROR]" Onclick="processPassword" runat="server" type="submit" CausesValidation="true"></asp:button>
		</form>
	</div>

	<button type="button" data-bs-toggle="modal" data-bs-target="#modal_panel" class="btn btn-primary" hidden>Open</button>

	<div id="modal_panel" class="modal fade">
		<div class="modal-dialog">
			<div class="modal-content">
				
				<div class="modal-header">
					<h4 id="modal_title" class="modal-title">Response</h4>
					<button type="button" class="btn-close" data-bs-dismiss="modal"></button>
				</div>
				<div id="modal_content" class="modal-body">
					Content here...
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
				</div>

			</div>
		</div>
	</div>

</body>
</html>
