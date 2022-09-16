<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="resetPassword.aspx.cs" Inherits="VLAB_AccountServices.services.resetPassword" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title id="TITLE">Account Services</title>
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
	<script src="assets/js/Loader.js" defer></script>
	<!--<script src="assets/js/server.js" defer></script>-->
	<!--
	<script src="assets/js/main.js" defer></script>
	<script src="assets/css/Bootstrap/Bootstrap.js" defer></script>
	-->
</head>
<body>

	
	<button type="button" data-bs-toggle="modal" data-bs-target="#modal_support_panel" class="btn btn-primary help">Help</button>

	<div id="bg" class="bg"></div>
	<div id="global" class="container center-h center-v global">
		<div id="form-container" class="container beval">
			<form id="form_main" class="container" runat="server" method="post">
				<button class="logout btn btn-primary" runat="server" id="LogoutBtn" type="button" onclick="Logout()">Logout</button>
				<div class="container content">
					<h1 class="title pos-rel center-h">Account Services</h1>
					<br>
					<div id="accordion" class="container large">
						<!-- Local user information/data -->
						<div class="hidden">
							<!--
								This section is used immediately when the user/client completely loads the page.
								The data in these fields are stored in JavaScript variables that will be referenced by the server for user account reference information.
							-->
							<asp:TextBox ID="info_fname" value="" runat="server" Visible="true"></asp:TextBox>
							<asp:TextBox ID="info_lname" value="" runat="server" Visible="true"></asp:TextBox>
							<asp:TextBox ID="info_uhid" value="" runat="server" Visible="true"></asp:TextBox>
							<asp:TextBox ID="info_email" value="" runat="server" Visible="true"></asp:TextBox>
							<asp:TextBox ID="user_type_element" value="" runat="server" Visible="true"></asp:TextBox>
							<asp:TextBox ID="server_data_element" value="" runat="server" Visible="true"></asp:TextBox>
						</div>
						<!-- PASSWORD MANAGEMENT SECTION -->
						<div class="card">
							<button data-bs-toggle="collapse" data-bs-target="#password_mgr_container" class="btn btn-primary center-h card-header collapsed" type="button">Password Management</button>
							<div id="password_mgr_container" class="collapse" data-bs-parent="#accordion">
								<table class="container pwd-mgr">
									<!-- Username field elements -->
									<tr>
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
									<!-- Password field elements -->
									<tr>
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
									<!-- Password confirmation field elements -->
									<tr>
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
								<button id="form-pwd-btn" class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)" data-type="submit">Save Password</button>
								<br>
								<asp:label id="status" Text="" runat="server" class="status alert alert-dark center-h" CausesValidation="false"></asp:label>
							</div>
						</div>
						<!-- VIRTUAL DESKTOP MANAGEMENT SECTION -->
						<div class="card" id="vdi-mgr-container">
							<button data-bs-toggle="collapse" data-bs-target="#vdi_container" class="btn btn-primary center-h card-header collapsed" CausesValidation="false" type="button">Available Desktops</button>
							<div id="vdi_container" class="collapse" data-bs-parent="#accordion">
								<asp:Panel ID="group_container" CssClass="groups container" Visible="false" runat="server" CausesValidation="false"></asp:Panel>
								<asp:Label ID="groups_label" CssClass="sub_title center-h" for="groups" runat="server" Visible="false" CausesValidation="false">Available Groups:</asp:Label>
								<br />
								<!-- The following table contains the user's available and already joined virtual desktops -->
								<table class="table table-striped table-bordered">
									<tr>
										<th>Available Desktops <button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="Here, you can select the virtual desktops that you want access to.\nOnce you've submitted the form, you'll gain immediate access to the virtual desktops you have selected."></button></th>
										<th>My Desktops <button type="button" class="btn btn-info info t-500" data-bs-toggle="popover" data-bs-target="#modal_panel" onclick="SetInfo(this)" data-bs-content="These are the virtual desktops that you already have access to."></button></th>
									</tr>
									<tr>
										<!-- The available virtual desktops that the user can request -->
										<td>
											<asp:CheckBoxList ID="GroupsElement" CssClass="list groups pos-rel" name="groups" runat="server" CausesValidation="false"></asp:CheckBoxList>
										</td>
										<!-- The virtual desktops that the user is already has access to. -->
										<td>
											<asp:CheckBoxList ID="UserGroupsElement" CssClass="list groups pos-rel" name="groups" runat="server" CausesValidation="false"></asp:CheckBoxList>
										</td>
									</tr>
								</table>
								<br>
								<!-- Virtual Desktop form submission element -->
								<button id="form-vd-btn" class="btn btn-outline-primary pos-rel center-h submit_button" type="button" onclick="SubmitForm(this)" data-type="submit">Add</button>
							</div>
						</div>
						<!-- Developer element (Used for testing specific functionalities and features) -->
						<asp:Panel ID="dev" Visible="false" runat="server">
							<label for="debug">Debug:</label>
							<asp:CheckBox ID="debug" name="debug" runat="server" />
						</asp:Panel>
					</div>
				</div>
				<br>
				<!-- Form submission button -->
				<asp:button id="submit_btn" CssClass="submit_btn btn btn-outline-primary btn-lg center-h submit_button hidden" name="submit_btn" Text="[ERROR]" Onclick="processPassword" runat="server" type="submit" CausesValidation="true"></asp:button>
				<br>
				<!-- Below contains the element used for the post-submission response message (The message that informs the user where to go afterwards to access their virtual desktop) -->
				<asp:Panel ID="SMCElement" CssClass="container footer" runat="server" Visible="true">
					<asp:Panel CssClass="alert alert-info" ID="SMCAlert" runat="server">
						<strong>Info:</strong> You can access your virtual desktops <a href="https://vlab.maui.hawaii.edu/" target="_blank">here</a>.
					</asp:Panel>
				</asp:Panel>
			</form>
		</div>
	</div>

	
	<!--
	<div class="dev container">
		<div id="ctest" class="carousel slide" data-bs-ride="carousel">
			<div class="carousel-indicators">
				<button type="button" data-bs-target="#ctest" data-bs-slide-to="0" class="active"></button>
				<button type="button" data-bs-target="#ctest" data-bs-slide-to="1"></button>
				<button type="button" data-bs-target="#ctest" data-bs-slide-to="2"></button>
			</div>
			<div class="carousel-inner">
				<div class="carousel-item active">
					<div class="d-block citem">
						Hello World!
					</div>
				</div>
				<div class="carousel-item">
					<div class="d-block citem">
						<b>Page 2</b>
					</div>
				</div>
				<div class="carousel-item">
					<div class="d-block citem">
						<font color="#FF0000">Page 3</font>
					</div>
				</div>
			</div>
			<div class="c-control-container">
				<button type="button" class="carousel-control-prev" data-bs-target="#ctest" data-bs-slide="prev">
					<span class="carousel-control-prev-icon"></span>
				</button>
				<button type="button" class="carousel-control-next" data-bs-target="#ctest" data-bs-slide="next">
					<span class="carousel-control-next-icon"></span>
				</button>
			</div>
		</div>
	</div>
	-->

	<div id="fsc" class="ds-section" hidden>
		<div class="ds-indicators">
			<button type="button" data-ds-target="#fsc" data-ds-slide-to="0" class="active"></button>
			<button type="button" data-ds-target="#fsc" data-ds-slide-to="1"></button>
			<button type="button" data-ds-target="#fsc" data-ds-slide-to="2"></button>
		</div>
		<div class="contents">
			<div class="item active">
				Hello World!
			</div>
			<div class="item">
				<b>Page 2</b>
			</div>
			<div class="item">
				<font style="color:orange;">Page 3</font>
			</div>
		</div>
		<div class="ds-controls">
			<button type="button" class="btn btn-outline-primary prev" data-ds-target="#fsc"></button>
			<button type="button" class="btn btn-outline-primary next" data-ds-target="#fsc"></button>
		</div>
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
