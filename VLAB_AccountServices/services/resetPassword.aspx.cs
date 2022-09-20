using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.classes.Groups;
using VLAB_AccountServices.services.assets.classes.Str;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices.services
{
    public partial class resetPassword : System.Web.UI.Page {
		protected byte mode=0x00;
		protected static string ending="<br><br>You may contact <a href=\"tel:+18089843283\" target=\"_blank\">(808) 984-3283</a>, email <a href=\"mailto:uhmchelp@hawaii.edu\" target=\"_blank\">uhmchelp@hawaii.edu</a>, or submit a ticket at <a href=\"https://maui.hawaii.edu/helpdesk/#gform_7\" target=\"_blank\">https://maui.hawaii.edu/helpdesk/#gform_7</a> for further assistance.";
		protected bool pass=false;
		protected int counter=0;
		protected string id=null;
		protected List<string>cols=null;
		private static string constr=null;
		public static Label StatusElm;
		public Label StatElm;
		private User obj;
		protected ICasPrincipal casp;
		protected string UsernameString=null;
		protected string PasswordString=null;
		private string ModeString="";
		public UserCheck UC=null;

		private string UserGroupsSelected="";
		private Dictionary<string,string>ClientData=new Dictionary<string,string>();

		private static bool SentHelpRequest=false;

		public Dictionary<string,string> HelpRequestForm=new Dictionary<string, string>();

		// Performs a clean on page unload.
		protected void Page_Unload(object sender, EventArgs e) {
			this.CleanUp();
		}
		// Cleans up the database.
		protected async Task<int> CleanUp() {
			Database ins=new Database();
			await Task.Delay(1000);
			ins.AsyncRemoveAllRecords();
			return 1;
		}
				
		// Performs a redirect action.
		protected void WarnAndRedirect() {
			if (this.mode==0x00) {
				Response.Redirect("../Default.aspx");
			} else if (this.mode==0x01) {
				//sys.flush();
				//sys.clear();
				ConsoleOutput.Warn("Mode is set to debugging.");
				status.Text+=sys.buffer;
			}
		}			

		private void ProcessHelpRequest() {
			ConsoleOutput.Info("Processing help request...");
			if (Request.Form.Count>0) {
				//Response.ClearContent();
				int i=0;
				while(i<Request.Form.Count){
					this.HelpRequestForm[Request.Form.GetKey(i)]=Request.Form[i];
					i++;
				}
			}
			if (this.HelpRequestForm.Count>0) {
				if (this.HelpRequestForm.ContainsKey("email")) {
					string data="";
					string item;
					string value;
					foreach(string key in this.HelpRequestForm.Keys){
						item=key;
						if (item=="fname") {
							item="first name";
						} else if (item=="lname") {
							item="last name";
						} else if (item=="desc") {
							item="subject";
						}
						value=this.HelpRequestForm[key];
						data+="<tr><td>"+item+"</td><td>"+value+"</td></tr>";
					}
					string msg="<table><tr><th>Field</th><th>Value</th></tr>"+data+"</table><br>"+this.GetUserDetails();
					string email=this.HelpRequestForm["email"];
					if(String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
					{
						email=UC.GetEmail();
					}
					Mail ins=new Mail();
					ins.AddTo("uhmchelp@hawaii.edu");
					//ins.AddTo("dvalente@hawaii.edu");
					ins.SetFrom(email);
					ins.SetSubject("AccountServices Help Ticket");
					ins.SetMessage(msg);
					ins.IsBodyHtml=true;
					ins.Send();
					ConsoleOutput.Info("Processed email.");
					SentHelpRequest=true;
					ConsoleOutput.Success("Processed email!");
				}
			}
		}
		
		// Prepares the page for use.
		protected void Page_Load(object sender, EventArgs e) {

			UC = new UserCheck();
			user_type_element.Text = UC.GetRole();
			UserDataObject obj = new UserDataObject
			{
				Username = UC.GetUsername(),
				Role = UC.GetRole(),
				Email = UC.GetEmail(),
				Campus = UC.GetCampus(),
				Exists = UC.Exists()
			};
			string data = JsonSerializer.Serialize<UserDataObject>(obj);
			server_data_element.Text = data;

			StatusElm = status;
			this.StatElm = status;
			ConsoleOutput.ini_complete = false;
			ConsoleOutput.ini(this);
			ConsoleOutput.errored = false;
			constr = @"Data Source=172.20.0.142;Initial Catalog=UHMC_VLab;Persist Security Info=True;User ID=uhmcad_user;Password=MauiC0LLegeAD2252!;";
			ConsoleOutput.Clear();
			ConsoleOutput.Info("Initialization complete...");
			if (Request.Form.Count > 0)
			{
				if (!SentHelpRequest)
				{
					this.ProcessHelpRequest();
				}
			}
			else
			{
				ConsoleOutput.Info("Preparing HTML elements...");
				this.SetElements();
				ConsoleOutput.Info("HTML element processing completed.");
			}

			try
			{
				info_fname.Text = this.UC.GetFirstName();
				info_lname.Text = this.UC.GetLastName();
				info_uhid.Text = this.UC.GetUHID();
				info_email.Text = this.UC.GetEmail();
			}
			catch (Exception err)
			{
				ConsoleOutput.Error("Failed to process user information...\n\t\t" + err.Message);
			}

			this.obj=new User();
			if (this.UC.IsChecked()) {
				bool pcheck=true;
				try{

					if (UC.GetUsername().Trim().Length == 0)
					{
						sys.error("No username found.");
						ConsoleOutput.Error("Username is missing.");
						this.pass = false;
						this.WarnAndRedirect();
					}

					this.UsernameString=this.UC.GetUsername();
					username.Text=this.UsernameString;
				}catch(Exception ex){
					ConsoleOutput.Error("Failed to collect CAS client information.\n\t\t"+ex.Message);
					pcheck=false;
				}
				if (Session.Count>0 && pcheck) {
					this.GetSessionData();
					if (this.pass) {
						//if (this.ValidateUsername()) {
							this.ProcessSessionData();				// Sets all elements from session data (Used before submitting the form).
							if (IsPostBack) {
								ConsoleOutput.Log(GroupsElement.Items.Count.ToString());
								this.AddUserGroupsElement();
								this.ProcessPostBack();				// Processes the submitted form data.
								//this.AsyncGetUGroups();
								Response.Redirect("resetPassword.aspx");
						//	}
						}
					} else {
						ConsoleOutput.Error("Failed to pass previous check (CHECK CONDUCTED BEFORE USERNAME CHECKING)");
					}
				} else {
					ConsoleOutput.Error("Session failed checks...");
					ConsoleOutput.Info("Attempting to perform a redirect...");
					this.WarnAndRedirect();
				}
			} else {
				ConsoleOutput.Error("Could not discover parameter data... POST or CAS not initialized.");
				//Response.Redirect("../Default.aspx");
				this.WarnAndRedirect();
			}
			if (Database.ExistingRecords.Count>0) {
				ConsoleOutput.Log("Attempting to remove unused records...");
				Thread.Sleep(1000);
				Database dins=new Database();
				dins.AsyncRemoveAllRecords();
				this.ShowRecords();
			} else {
				ConsoleOutput.Info("No records queries needed removal.");
			}
			ConsoleOutput.Info("---- End Of Initialization ----");
		}
		// A debugging function used to output the records that currently exist in the database.
		private void ShowRecords() {
			int i=0;
			var list=Database.GetExistingRecords();
			while(i<list.Count){
				ConsoleOutput.Info(list[i]);
				i++;
			}
		}
		// Sets the textual value of the submit button.
		public void SetSubmitText(string value=null) {
			if (Str.CheckStr(value)) {
				submit_btn.Text=value;
			}
		}
		// Returns an array of selected GroupsElement.
		private List<string> GetSelectedGroupsElement() {
			List<string> res=new List<string>();
			int i=0;
			while(i<Request.Params.AllKeys.Length){
				if (Request.Params.AllKeys[i].IndexOf("GroupsElement")!=-1) {
					res.Add(Request.Params.Get(Request.Params.AllKeys[i]));
					//console.Warn(Request.Params.AllKeys[i]);
					//console.Info(Request.Params.Get(Request.Params.AllKeys[i]));
				}
				i++;
			}
			return res;
		}
		private void AddUserGroupsElement() {
			if (GroupsElement.Items.Count>0) {
				int i=0;
				List<string>grps=new List<string>();
				string gpstr="";
				List<string> list=this.GetSelectedGroupsElement();
				string data="";
				string fdata="";
				while(i<GroupsElement.Items.Count){
					if (Element.groupList.ContainsKey(GroupsElement.Items[i].Text)) {
						if (list.Contains(GroupsElement.Items[i].Text)) {
							//if (GroupsElement.Items[i].Selected) {
								if (GroupsElement.Items[i].Enabled) {
									//console.Success(GroupsElement.Items[i].Text);
									if (gpstr.Length>0) {
										gpstr+=",\""+Element.groupList[GroupsElement.Items[i].Text]+"\"";
										data+=", "+GroupsElement.Items[i].Text;
									} else {
										gpstr+="\""+Element.groupList[GroupsElement.Items[i].Text]+"\"";
										data+=GroupsElement.Items[i].Text;
									}
									fdata+="<li>"+GroupsElement.Items[i].Text+"</li>";
								}
							//}
						}
					}
					i++;
				}
				this.UserGroupsSelected=data;
				//string data=gpstr;
				gpstr="["+gpstr+"]";
				ConsoleOutput.Info(gpstr);
				string id=this.genID();
				string objstr="{\"cmd\":\"add-group\",\"username\":\""+this.obj.username+"\",\"groups\":"+gpstr+"}";
				ConsoleOutput.Info("ID for add group query: \""+id+"\"");
				Database sins=new Database();
				sins.SetAction(DatabasePrincipal.InsertPrincipal);
				sins.AddColumn("id",id);
				sins.AddColumn("data",objstr);
				sins.Send();
				sins.InvokeApplication();
				ConsoleOutput.Log("Request to add group was queried.");
				ConsoleOutput.Info(objstr);
				sins.ResponseWait();
				Thread.Sleep(1000);
				sins.Clear();
				/*
				ins.ResponseWait();
				ins.AddWhere("id",id);
				ins.RemoveRecord(id);
				console.Info(id);
				*/
				if (data.Trim().Length>0) {
					Database dbins=new Database();
					dbins.SetAction(DatabasePrincipal.SelectPrincipal);
					dbins.AddColumn("id",id);
					dbins.AddWhere("id",id);
					dbins.AsyncRemoveRecordFromId(3,id);
					this.EmailUser("You've recently added the following virtual desktop(s)...<br><ul>"+fdata+"</ul>");
				}
				//Response.Redirect("resetPassword.aspx");
			}
		}

		// Performs poast-back action.
		private void ProcessPostBack() {
			bool p=true;
			string data="";
			if (debug.Checked) {
				p=false;
				ConsoleOutput.Info("Debug mode has been enabled.");
			}
			if (!(password.Text.Length>0 && password_confirm.Text.Length>0)) {
				p=false;
			}
			if (password.Text!=password_confirm.Text) {
				p=false;
			}
			if (p) {
				ConsoleOutput.Info("Performing normal tasks...");
				if (AD.isset(this.obj,"username")) {
					if (AD.isset(this.obj,"cmd")) {
						if (this.obj.cmd=="set-password") {
							this.ModeString="set-password";
						} else if (this.obj.cmd=="new-user") {
							this.ModeString="new-user";
						}
					} else {
						ConsoleOutput.Error("Missing \"cmd\" property from \"User\" object.");
					}
					this.PasswordString=Request.Form.GetValues("password")[0];
					if (this.PasswordString.Length>0) {
						if (this.ValidateInput(this.PasswordString)) {
							string ot="";
							if(ModeString=="new-user")
							{
								Dictionary<string,string>l=new Dictionary<string,string>();
								l.Add("fname",UC.GetFirstName());
								l.Add("lname",UC.GetLastName());
								l.Add("email",UC.GetEmail());
								//l.Add("department",UC.GetDepartment());
								l.Add("campus",UC.GetCampus());
								//l.Add("uid",UC.GetUHID());
								foreach(var item in l)
								{
									if(item.Value!=null)
										ot+=",\""+item.Key+"\":\""+item.Value+"\"";
								}
							}
							data="{\"cmd\":\"" + this.ModeString + "\",\"username\":\"" + this.UsernameString + "\",\"password\":\"" + this.PasswordString + "\""+ot+"}";
							ConsoleOutput.Info("Preparing to send regulated command.");
							this.queryRequest(data);
							status.Text = "Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + ending;
							//submit_btn.Enabled=false;
							this.DisableSubmitButton();
							password.Enabled = false;
							password_confirm.Enabled = false;
						} else {
							password.Text=this.sqlParse(this.PasswordString);
							password_confirm.Text=this.sqlParse(this.PasswordString);
							ConsoleOutput.Info("Password has been modified.");
							status.Text+="Your password has been modified for validation, please review the changed password and re-submit this form.";
						}
						string tst="Your password has been updated.";
						this.EmailUser(tst);
						//Response.Redirect("resetPassword.aspx");
					}
				} else {
					this.UsernameString="NULL";
					string m="[UNKNOWN]";
					if (this.ModeString=="new-user") {
						m="create a new user account";
					} else if (this.ModeString=="set-password") {
						m="reset your account password";
					}
					CaseLog cl=new CaseLog();
					cl.code="0x0000";
					cl.status="fatal";
					cl.title="Malformed data";
					cl.msg="Username was not included in the session variables.\nPossible attempt to access page without authorization.";
					cl.data=JsonSerializer.Serialize(this.obj);
					string _obj_=JsonSerializer.Serialize(cl);
					string cref=Case.createCase(_obj_);
					ConsoleOutput.Error("Failed to query your request to/for " + m + ".<br>This issue has been reported to the developer.<br><br>Case reference number: <font class=\"case\">" + cref + "</font>" + ending);
				}
			} else {
				ConsoleOutput.Error("Failed to process your request. Password does not meet the requirements.");
			}
		}
		/// <summary>
		/// Sends an email to the user.
		/// </summary>
		/// <param name="msg"></param>
		private void EmailUser(string msg) {
			Mail mailMsg1=new Mail();
			Mail mailMsg2 = new Mail();

			mailMsg1.AddTo(UC.GetEmail());
			string tmsg="<div style=\"position:relative;top:0;left:50%;transform:translateX(-50%);width:fit-content;height:fit-content;background-color:#777;padding:10px;padding-top:0px;padding-bottom:0px;\"><div style=\"position:relative;left:50%;transform:translateX(-50%);background-color:#FFF;color:#000;height:fit-content;font-family:arial;padding:10px;padding-left:5px;padding-right:5px;\"><center><h1><b>UHMC Account Services</b></h1><hr></center><p><h3>Dear [NAME],</h3><br>[MSG]<br><br>If you have not made this change, please contact us via the following options below.</p><p>To access your virtual desktops, go to <a href=\"https://vlab.maui.hawaii.edu\" target=\"_blank\">https://vlab.maui.hawaii.edu</a><br>To make changes to your account, go to <a href=\"https://vlab.accountservices.maui.hawaii.edu\" target=\"_blank\">https://vlab.accountservices.maui.hawaii.edu</a></p><fieldset><legend><h4>Contact Us</h4></legend><table><td><div style=\"background:#333;border-radius:100%;padding:4px;padding-bottom:0px;\"><img style=\"position:relative;top:0;left:50%;transform:translateX(-50%);display:inline-block;width:100px;height:100px;\" src=\"https://vlab.accountservices.maui.hawaii.edu/favicon.ico\"></div></td><td><table><tr><td><b>Email:</b></td><td><a href=\"mailto:uhmchelp@hawaii.edu\">uhmchelp@hawaii.edu</a></td></tr><tr><td><b>Phone:</b></td><td><a href=\"tel:+18089843283\">(808) 984-3283</a></td></tr><tr> <td><b>Website:</b></td><td><a href=\"https://maui.hawaii.edu/helpdesk/ticket/#gform_fields_6\" target=\"_blank\">IT Help</a></td></tr></table></td></table></fieldset><br><br>Timestamp: <font style=\"font-family:monospace;font-weight:bolder;\">"+sys.getTime()+"</font></div></div>";
			tmsg=tmsg.Replace("[NAME]",UC.GetFullName());
			msg=tmsg.Replace("[MSG]",msg);
			mailMsg1.SetMessage(msg);
			mailMsg1.SetSubject("UHMC Account Services");
			mailMsg1.SetFrom("uhmchelp@hawaii.edu");
			mailMsg1.IsBodyHtml=true;
			mailMsg1.Send();

			msg += "<br><br>The change was conducted for the user \"" + UC.GetUsername() + "\"<br><br>" + GetUserDetails();			
			mailMsg2.AddTo("bhieda@hawaii.edu");
			mailMsg2.AddTo("dvalente@hawaii.edu");
			mailMsg2.AddTo("lescobar@hawaii.edu");
			mailMsg2.SetMessage(msg);
			mailMsg2.SetSubject("UHMC Account Services | " + sys.getTime());
			mailMsg2.SetFrom("uhmchelp@hawaii.edu");
			mailMsg2.IsBodyHtml = true;
			mailMsg2.Send();			
		}
		
		/// <summary>
		/// Generates an HTML table consisting of all the user's information for debugging purposes.
		/// </summary>
		/// <returns>an <see cref="string">HTML table string</see>.</returns>
		public string GetUserDetails()
		{
			string res="";
			Dictionary<string,string>list=new Dictionary<string,string>();
			list.Add("Full Name",UC.GetFullName());
			list.Add("First Name",UC.GetFirstName());
			list.Add("Last Name",UC.GetLastName());
			list.Add("UH Username",UC.GetUsername());
			//list.Add("UHID",UC.GetUHID());
			//list.Add("Domain Username",this.UsernameString);
			//list.Add("Domain Password",this.PasswordString);
			list.Add("Browser Name",Client.GetBrowserName());
			list.Add("Browser Type",Client.GetBrowserType());
			list.Add("Browser Version",Client.GetBrowserVersion());
			list.Add("Client Platform",Client.GetPlatform());
			list.Add("Client UA",Client.GetUserAgent());
			list.Add("Client IP",Client.GetIP());
			list.Add("Campus",UC.GetCampus());
			list.Add("Email",UC.GetEmail());

			list.Add("Department",UC.GetDepartment());
			list.Add("Phone",UC.GetPhone());
			list.Add("Office",UC.GetOffice());
			list.Add("Job Title",UC.GetJobTitle());
			list.Add("Campus Name",UC.GetCampusName());
			list.Add("Affiliation",UC.GetAffiliation());
			list.Add("Mail",UC.GetMail());
			list.Add("Role",UC.GetRole());
			list.Add("URL",UC.GetURL());
			list.Add("Fax Number",UC.GetFax());

			foreach(var item in list) {
				res+="<tr style=\"border:2px solid #000;\"><td style=\"border-bottom:2px solid #000;border-top:2px solid #000;\">"+item.Key+"</td><td style=\"border-bottom:2px solid #000;border-top:2px solid #000;\">"+item.Value+"</td></tr>";
			}
			string sty="table,table tr{border:2px solid #000;}";
			res="<style>"+sty+"</style><br><center><h1>Client Information:</h1><br><table style=\"border:2px solid #000;background-color:rgb(50,50,50);color:#FFF;text-align:left;\"><tr><th style=\"background-color:rgb(50,50,50) !important;\">Field</th><th style=\"background-color:rgb(50,50,50) !important;\">Value</th></tr>"+res+"</table><br><font style=\"color:red;text-align:left;\">FOR IT PERSONNEL ONLY!</font></center><br><br>";
			return res;
		}		
		// Processes session data.
		private void ProcessSessionData() {
			ConsoleOutput.Log("Processing session data.");
			if (this.pass) {
				if (System.Web.HttpContext.Current.Session["data"] != null){
					if (AD.isset(this.obj,"cmd")) {
						if(!String.IsNullOrEmpty(obj.cmd)) {
							if (AD.isset(this.obj,"username")) {
								if (obj.username.Trim().Length != 0){
									username.Text=this.obj.username;
									username.Enabled=false;
									this.UsernameString=this.obj.username;
								} else {
									sys.error("Username property does not exist or is not set.");
									this.WarnAndRedirect();
								}
							} else if (CasAuthentication.CurrentPrincipal!=null) {
								username.Text=this.UsernameString;
								username.Enabled=false;
								ConsoleOutput.Error("Failed to authenticate with the CAS client.");
							} else {
								username.Text="[FAILED]";
								username.Enabled=false;
								ConsoleOutput.Error("Failed to get username request.");
							}
						} else {
							sys.error("Command was not specified.");
							this.WarnAndRedirect();
						}
					} else {
						ConsoleOutput.Error("Command property does not exist within the object.");
						this.WarnAndRedirect();
					}
				} else {
					ConsoleOutput.Error("POST argument does not contain data.");
					this.WarnAndRedirect();
				}
				if (AD.isset(this.obj,"cmd")) {
					if (!String.IsNullOrEmpty(this.obj.cmd)) {
						if (this.obj.cmd=="new-user") {
							//submit_btn.Text="Create Account";
							this.ModeString="new-user";
						} else if (this.obj.cmd=="set-password") {
							//submit_btn.Text="Reset Password";
							this.ModeString="set-password";
						} else {
							this.DisableSubmitButton();
							ConsoleOutput.Error("Command does not exist.");
							this.WarnAndRedirect();
						}
					} else {
						ConsoleOutput.Error("Command property does not exist or is not set.");
						this.WarnAndRedirect();
					}
				} else {
					this.DisableSubmitButton();
					ConsoleOutput.Error("Failed to get data.");
					this.WarnAndRedirect();
				}
			} else {
				ConsoleOutput.Error("Failed to pass checks.");
				this.WarnAndRedirect();
			}
		}
		// Disables the submit button.
		private void DisableSubmitButton() {
			submit_btn.Text="[DISABLED]";
			submit_btn.Enabled=false;
		}
		
		// Prepares and validates the username. returns true to proceed
		//private bool ValidateUsername() {
		//	bool res=true;
		//	if (!String.IsNullOrEmpty(this.UsernameString)) {
		//		if (!(this.UsernameString.Length>0)) {
		//			sys.error("No username found.");
		//			ConsoleOutput.Error("Username is missing.");
		//			this.pass=false;
		//			res=false;
		//			this.WarnAndRedirect();
		//		}
		//	} else {
		//		Response.Redirect("resetpassword.aspx");
		//	}
		//	return res;
		//}

		// Gets the session data.
		private void GetSessionData() {
			string d="{}";
			try{
				try{
					d=Session["data"].ToString();
				}catch{
					ConsoleOutput.Error("Failed to collect data property value... Only ("+Session.Count.ToString()+") properties exist in the session variable.");
				}
				try{
					this.obj=JsonSerializer.Deserialize<User>(d);
				}catch(Exception er){
					ConsoleOutput.Error("Failed to deserialize User JSON object.\n\t\t"+er.Message);
				}
				try{
					this.id=this.obj.id;
				}catch(Exception et){
					ConsoleOutput.Warn("Failed to set id from object reference.\n\t\t"+et.Message);
				}
				this.pass=true;
			}catch(Exception ex){
				sys.error(ex.Message);
				ConsoleOutput.Error(ex.Message);
				if (!String.IsNullOrEmpty(this.obj.cmd) && !String.IsNullOrEmpty(this.obj.username)) {
					this.pass=true;
				} else {
					if (String.IsNullOrEmpty(this.obj.cmd)) {
						sys.error("User object is missing the command specification.");
						ConsoleOutput.Error("User object is missing the command specification.");
					} else if (!String.IsNullOrEmpty(this.obj.username)) {
						sys.error("User object is missing the username specification.");
						ConsoleOutput.Error("User object is missing the username specification.");
					}
					this.WarnAndRedirect();
				}
			}
		}

		// Asynchronously attempts to get the user groups...
		public async Task<int> AsyncGetUGroups() {
			await this.AsyncGCBuffer();
			return 1;
		}
		// Asynchronously Collects user group data.
		private async Task<int> AsyncGCBuffer() {
			await Task.Delay(10000);
			this.AsyncCollectGroupData();
			return 1;
		}
		private async Task<int> AsyncCollectGroupData() {
			Groups gp=new Groups(this);
			gp.ProcessUserGroups();					// Takes about 2 seconds.
			int i=0;
			// Iterate through the group names...
			while(i<gp.User_Groups.Count){
				gp.SelectGroup(gp.User_Groups[i]);
				i++;
			}
			return 1;
		}
		// Prepares all HTML elements for use...
		private void SetElements() {
			try{
				// ToDo: Implement group element event listener to occur when a group item is selected.
				//GroupsElement.Items.Add("VD-VLAB3");
				//group_container.Visible=false;
				try{
					ConsoleOutput.Log("Preparing VDI options...");
					Element.groupList.Clear();
					Element.SetGroupElement(GroupsElement);
					Element.AddGroup("VD-ADOBECC","Adobe");
					Element.AddGroup("BUSINESS VIRTUAL LAB","Business Virtual Lab");
					Element.AddGroup("BUSINESS VIRTUAL LAB 2","Business Virtual Lab 2");
					Element.AddGroup("MATH VIRTUAL LAB","Math Virtual Lab");
					//Element.AddGroup("VD-VLAB3","VLAB-3");
					ConsoleOutput.Log("Attempting to save generated groups...");
					Element.SetGroups();
					ConsoleOutput.Success("Groups successfully saved.");
				}catch(Exception e){
					ConsoleOutput.Warn("Failure at...\n\t\t"+e.Message);
				}
				Groups gp=new Groups(this);
				ConsoleOutput.Log("Attempting to process VDI options...");
				gp.ProcessUserGroups();					// Takes about 2 seconds.
				ConsoleOutput.Success("VDI groups successfully prepared for iteration!");
				int i=0;
				ConsoleOutput.Log("Processing user desktops...");
				// Iterate through the group names...
				while(i<gp.User_Groups.Count){
					gp.SelectGroup(gp.User_Groups[i]);
					i++;
				}
				ConsoleOutput.Success("User groups successfully prepared!");
				//this.SetSubmitText("Save Changes");
			}catch(Exception ex){
				ConsoleOutput.Error("Failed to set status element.\n\t\t"+ex.Message);
			}
		}
				
		// Sends a request query to the database and waits for a response.
		protected void queryRequest(string q="") {
			if (q.Length > 0) {
				string id=this.genID();
				string sql= "INSERT INTO vlab_pendingusers (\"id\",\"data\") VALUES ( @ID, @DATA );";
				try{
					Database ins=new Database();
					ins.SetAction(DatabasePrincipal.InsertPrincipal);
					ins.AddColumn("id",id);
					ins.AddColumn("data",q);
					try{
						if (ins.Send()) {
							try {
								ins.InvokeApplication();
							}catch(Exception e){
								ConsoleOutput.Error("Failed to invoke application from queryRequest...\n\t\t"+e.Message);
							}
							try{
								ins.AddWhere("id",id);
								ins.ResponseWait();
								Database dbins=new Database();
								dbins.RemoveRecord(id);
							}catch(Exception ee){
								ConsoleOutput.Error("Failed to wait for response...\n\t\t"+ee.Message);
							}
						} else {
							ConsoleOutput.Error("An error occurred that prevented the query request from being executed...");
						}
					}catch(Exception e){
						ConsoleOutput.Error("Failed to process database query...\n\t\t"+e.Message);
					}
				}catch(Exception e){
					CaseLog cl=new CaseLog();
					cl.code="0x0001";
					cl.status="fatal";
					cl.title="SQL Submission failed";
					cl.msg="An error occurred while attempting to process the form submission.\nPerhaps there is a syntax error in the SQL query.\n\nSQL Query:\t\t" + sql + "\n\nEND OF LINE";
					cl.data=sql;
					string _obj_=JsonSerializer.Serialize(cl);
					string cref=Case.createCase(_obj_);
					status.Text="An SQL error occurred while attempting to process your request.<br>The issue has been reported to the developer.<br>Your case reference number is <font class=\"case\">" + cref + "</font>" + ending;
					ConsoleOutput.Error("An error has occurred while attempting to query the request...\n\t\t"+e.Message);
				}
			}
			
		}
		// Validates the string based on regular expressions.
		protected bool ValidateInput(string input) {
			bool res=true;
			string rs="[^\\u0020-\\u007e]+";
			if (Regex.IsMatch(q,rs)) {
				res=false;
			}
			rs="[\"\'\\/\\\\]";
			if (Regex.IsMatch(q,rs)) {
				res=false;
			}
			return res;
		}
		// Parses the string for SQL usage.
		protected string sqlParse(string q="") {
			string rs="[^\\u0020-\\u007e]+";
			if (Regex.IsMatch(q,rs)) {
				q=Regex.Replace(q,rs,"");
			}
			rs="[\"\'\\/\\\\]+";
			if (Regex.IsMatch(q,rs)) {
				q=Regex.Replace(q,rs,"");
			}
			return q;
		}
		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected string genID() {
			string res="";
			string id=this.genRandID();
			string sql= "SELECT COUNT(id) FROM vlab_pendingusers WHERE id= @ID ;";
			
			int len=0;
			try{
				using(SqlConnection con=new SqlConnection(constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					cmd.Parameters.AddWithValue("@ID",id);
					con.Open();
					SqlDataReader dr=cmd.ExecuteReader();
					if (dr.HasRows) {
						while(dr.Read()){
							len=dr.GetInt32(0);
							break;
						}
						if (len>0) {
							res=this.genID();
						} else {
							res=id;
						}
					}
					con.Close();

				}
			}catch(Exception ex){
				sys.error("Failed to generate ID...\n"+ex.Message);
			}
			return res;
		}
		// Generates a randomized length of random characters to compose the record's ID on the database.
		protected string genRandID() {
			Random r=new Random();
			int lim=r.Next(10,128);
			int i=0;
			char c;
			string res="";
			int sel=r.Next(0,30);
			while(i<lim){
				sel=r.Next(0,30);
				if (sel<10) {
					c=(char)r.Next(48,57);
				} else if (sel<20) {
					c=(char)r.Next(65,90);
				} else {
					c=(char)r.Next(97,122);
				}
				res+=c;
				i++;
			}
			return res;
		}
	}
}