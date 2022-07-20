<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.resetPassword" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title id="TITLE">UHMC AD Password Reset</title>
	<meta charset="utf-8">
	<meta name="author" content="UHMC Computing Services">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta name="theme-color" content="#FF0000">
	<meta property="og:image" content="https://vlab.accountservices.maui.hawaii.edu/favicon.ico" />
	<meta name="description" content="Manage your UHMC virtual desktops here." />
	<meta name="keywords" content="" />
	<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate">
	<meta http-equiv="Pragma" content="no-cache">
	<meta http-equiv="Expires" content="0">

	<link rel="stylesheet" href="assets/css/main.css" defer="true" />
	<script src="assets/js/server.js" defer></script>
	<script src="assets/js/main.js" defer></script>
	<script src="assets/css/Bootstrap/Bootstrap.js" defer></script>
</head>
<body>

	<button type="button" data-bs-toggle="modal" data-bs-target="#modal_support_panel" class="btn btn-primary help">Help</button>

	<div id="bg" class="bg"></div>
	<div class="container center-h center-v beval">
		<form id="form_main" class="container" runat="server" method="post">
			<div class="container content">
				<h1 class="title pos-rel center-h">Account Services</h1>
				<br>
				<div id="accordion" class="container large">

					<div hidden="true">
						<asp:TextBox ID="info_fname" value="" runat="server"></asp:TextBox>
						<asp:TextBox ID="info_lname" value="" runat="server"></asp:TextBox>
					</div>

					<div class="card">
						<button data-bs-toggle="collapse" data-bs-target="#password_mgr_container" class="btn btn-primary center-h card-header collapsed" type="button">Password Management</button>
						<div id="password_mgr_container" class="collapse" data-bs-parent="#accordion">
							<table class="container pwd-mgr">
								<tr>
									<!--<td><label for="username">Username: </label></td>-->
									<td>
										<div class="form-floating mb-3 mt-3 tal">
											<asp:textbox id="username" Text="" runat="server" name="username" type="text" value="" placeholder="UH Username..." CausesValidation="false" CssClass="form-control"></asp:textbox>
											<label for="password-confirm">Username</label>
										</div>
										<div class="options">
											<button type="button" class="btn btn-info peak hide"></button>
											<button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="Your UH username/email.\nThis is determined from your UH login and cannot be changed in this form."></button>
										</div>
									</td>
								</tr>
								<tr>
									<!--<td><label for="password">Password: </label></td>-->
									<td>
										<div class="form-floating mb-3 mt-3 tal">
											<asp:textbox id="password" Text="" runat="server" name="password" type="password" value="" placeholder="UH Password..." CausesValidation="false" CssClass="form-control"></asp:textbox>
											<label for="password">Password</label>
										</div>
										<div class="options">
											<button type="button" class="btn btn-info peak" onclick="peak(this)" data-ref="password"></button>
											<button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="Enter a password that you'll use to login to your virtual desktop."></button>
										</div>

									</td>
								</tr>
								<tr>
									<!--<td><label for="password-confirm">Confirm Password: </label></td>-->
									<td>
										<div class="form-floating mb-3 mt-3 tal">
											<asp:textbox id="password_confirm" Text="" runat="server" name="password_confirm" type="password" value="" placeholder="Confirm password..." CausesValidation="false" CssClass="form-control"></asp:textbox>
											<label for="password-confirm">Confirm Password</label>
										</div>
										<div class="options">
											<button type="button" class="btn btn-info peak" onclick="peak(this)" data-ref="password_confirm"></button>
											<button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="Confirm your password."></button>
										</div>
									</td>
								</tr>
							</table>
							<br>
							<button class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)" data-type="submit">Save Password</button>
							<br>
							<asp:label id="status" Text="" runat="server" class="status alert alert-dark center-h" CausesValidation="false"></asp:label>
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
									<th>Available Desktops <button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="Here, you can select the virtual desktops that you want access to.\nOnce you've submitted the form, you'll gain immediate access to the virtual desktops you have selected."></button></th>
									<th>My Desktops <button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="These are the virtual desktops that you already have access to."></button></th>
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
							<button class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)" data-type="submit">Add</button>
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


	<div id="modal_support_panel" class="modal fade">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<h4 id="modal_support_title" class="modal-title">Response</h4>
					<button type="button" class="btn-close" data-bs-dismiss="modal"></button>
				</div>
				<div id="modal_support_content" class="modal-body">
					<form method="post" enctype="multipart/form-data" target="gform_ajax_frame_6" id="gform_6">
						<div class="input-group mb-3">
							<span class="input-group-text">Your Name:</span>
							<input type="text" class="form-control" placeholder="First Name" id="input_6_5_3" name="input_5.3">
							<input type="text" class="form-control" placeholder="Last Name" id="input_6_5_6" name="input_5.6">
						</div>
						<div class="input-group mb-3">
							<span class="input-group-text">Email:</span>
							<input id="input_6_2" name="input_2" type="text" class="form-control" placeholder="Email">
						</div>
						<div class="input-group mb-3">
							<label for="input_7" class="input-group-text">Department:</label>
							<select class="form-select" name="input_7" id="input_6_7">
								<option value="Please Select">-- --</option>
								<option value="Admissions">Admissions</option>
								<option value="Counseling">Counseling</option>
								<option value="Careers or Internships">Careers or Internships</option>
								<option value="Scheduling">Scheduling</option>
								<option value="Housing">Housing</option>
								<option value="Financial Aid">Financial Aid</option>
								<option value="IT Help" selected>IT Help</option>
								<option value="Other">Other</option>
							</select>
						</div>
						<div class="input-group mb-3">
							<span class="input-group-text">Description:</span>
							<input id="input_6_8" name="input_8" type="text" class="form-control" placeholder="Brief one-sentence description of the issue..." value="AccountServices">
						</div>
						<div class="form-floating mb-3 mt-3">
							<textarea class="form-control" rows="10" cols="400" name="input_3" id="input_6_3" placeholder="Please provide additional details on the issue here..."></textarea>
							<label for="input_3">Details:</label>
						</div>
						<div hidden>
							<!--
							<iframe title="reCAPTCHA" src="https://www.google.com/recaptcha/api2/anchor?ar=1&k=6LfjJfUSAAAAABb0BxPDtUiUzF0V1CwVm8ZTCyMF&co=aHR0cHM6Ly9tYXVpLmhhd2FpaS5lZHU6NDQz&hl=en&v=4rwLQsl5N_ccppoTAwwwMrEN&theme=light&size=normal&cb=y4b9minbt3gw" width="304" height"78" role="presentation" name="a-rzcaksaeb8yf" frameborder="0" scrolling="no" sandbox="allow-forms allow-popups allow-same-origin allow-scripts allow-top-navigation allow-modals allow-popups-to-escape-sandbox"></iframe>
							-->
						</div>
						<button type="button" class="btn btn-outline-primary" onclick="SubmitHelpRequest()" id="submit_ticket_btn">Submit Ticket</button>
					</form>
				</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-danger" data-bs-dismiss="modal">Close</button>
				</div>

			</div>
		</div>
	</div>
	
	<div id="note" class="t-1000 danger btn-danger note hidden" onclick="CloseNote()">
		Note here...
	</div>

</body>
</html>
