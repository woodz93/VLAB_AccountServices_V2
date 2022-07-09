﻿using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
//using VLAB_AccountServices.services.assets.svr;
using VLAB_AccountServices.services.assets.sys;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.classes.Network;
using VLAB_AccountServices.services.assets.classes.Str;
using VLAB_AccountServices.services.assets.classes.Groups;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace VLAB_AccountServices.services {

	public partial class resetPassword : System.Web.UI.Page
	{
		protected byte mode=0x00;
		protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
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

		// Processes the password.
		public void processPassword(Object sender, EventArgs e) {
			string u=username.Text;
			string p=password.Text;
			string pc=password_confirm.Text;
			status.Text="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + resetPassword.ending;
			submit_btn.Enabled=false;
			password.Enabled=false;
			password_confirm.Enabled=false;
		}
		public void EndingSuccess() {
			status.Text="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + resetPassword.ending;
			//submit_btn.Enabled=false;
			this.DisableSubmitButton();
			password.Enabled=false;
			password_confirm.Enabled=false;
		}
		// Performs a redirect action.
		protected void redirect() {
			if (this.mode==0x00) {
				Response.Redirect("../Default.aspx");
			} else if (this.mode==0x01) {
				//sys.flush();
				//sys.clear();
				console.Warn("Mode is set to debugging.");
				status.Text+=sys.buffer;
			}
		}

		// Performs a first-time initialization of all variables and data.
		protected void ini() {
			//this.SetConnectionTimeout();
			this.InitialChecks();
			console.ini_complete=false;
			console.ini(this);
			console.errored=false;
			this.SetConnectionString();
			this.SetElements();
			console.Clear();
		}

		protected void SetConnectionTimeout() {
			GlobalHost.Configuration.ConnectionTimeout=TimeSpan.FromSeconds(110);
			GlobalHost.Configuration.DisconnectTimeout=TimeSpan.FromSeconds(30);
			GlobalHost.Configuration.KeepAlive=TimeSpan.FromSeconds(10);
		}

		protected void InitialChecks() {
			this.UC=new UserCheck();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.ini();
			this.obj=new User();
			//if (this.post_isset("data") && CasAuthentication.CurrentPrincipal!=null) {
			if (this.UC.IsChecked()) {
				bool pcheck=true;
				try{
					//this.casp=CasAuthentication.CurrentPrincipal;
					//this.UsernameString=System.Web.HttpContext.Current.User.Identity.Name;
					this.UsernameString=this.UC.GetUsername();										// Gets and stores the CAS/UH username.
					username.Text=this.UsernameString;												// Sets the username input element value to the UH username collected from the CAS system.
				}catch(Exception ex){
					console.Error("Failed to collect CAS client information.\n\t\t"+ex.Message);
					pcheck=false;
				}
				/*
				string campus="";
				try{
					//campus=sp.Assertion.Attributes["campusKey"].ToString();
					//campus=this.getAttribute(sp,"cn");
					//status.Text+="<br>\""+user+"\"<br><br>";
				}catch(Exception ec){
					//status.Text+="<br>ERROR: "+ec.Message+"<br><br>";
					console.Error(ec.Message);
				}
				*/
				if (Session.Count>0 && pcheck) {
					//console.Log("Number of session variables that exist are ("+Session.Count.ToString()+")");
					this.GetSessionData();
					if (this.pass) {
						if (this.ValidateUsername()) {
							this.ProcessSessionData();				// Sets all elements from session data (Used before submitting the form).
							if (IsPostBack) {
								console.Log(GroupsElement.Items.Count.ToString());
								if (GroupsElement.Items.Count>0) {
									this.AddUserGroupsElement();
								}
								this.ProcessPostBack();				// Processes the submitted form data.
								//this.AsyncGetUGroups();
							}
						}
					} else {
						console.Error("Failed to pass previous check (CHECK CONDUCTED BEFORE USERNAME CHECKING)");
					}
				} else {
					console.Error("Session failed checks...");
					console.Info("Attempting to perform a redirect...");
					this.redirect();
				}
			} else {
				console.Error("Could not discover parameter data... POST or CAS not initialized.");
				Response.Redirect("../Default.aspx");
				this.redirect();
			}
			//console.Log("END OF LINE");
			if (Database.ExistingRecords.Count>0) {
				console.Log("Attempting to remove unused records...");
				Thread.Sleep(1000);
				Database dins=new Database();
				dins.AsyncRemoveAllRecords();
				this.ShowRecords();
			}
		}

		private void ShowRecords() {
			int i=0;
			var list=Database.GetExistingRecords();
			while(i<list.Count){
				console.Info(list[i]);
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
				//console.Log(list.ToString());
				//console.Log(Request.ToString());
				while(i<GroupsElement.Items.Count){
					//console.Info(GroupsElement.Items[i].Text);
					//console.Log(Element.groupList.ToString());
					//console.Log(GroupsElement.ToString());
					if (Element.groupList.ContainsKey(GroupsElement.Items[i].Text)) {
						if (list.Contains(GroupsElement.Items[i].Text)) {
							//if (GroupsElement.Items[i].Selected) {
								if (GroupsElement.Items[i].Enabled) {
									//console.Success(GroupsElement.Items[i].Text);
									if (gpstr.Length>0) {
										gpstr+=",\""+Element.groupList[GroupsElement.Items[i].Text]+"\"";
									} else {
										gpstr+="\""+Element.groupList[GroupsElement.Items[i].Text]+"\"";
									}
								}
							//}
						}
					}
					i++;
				}
				/*
				i=0;
				while(i<GroupsElement.Items.Count){
					if (GroupsElement.Items[i].Selected) {
						grps.Add(GroupsElement.Items[i].Value);
						//console.Log(GroupsElement.Items[i].Value);
						if (i>0) {
							gpstr+=",\""+GroupsElement.Items[i].Value+"\"";
						} else {
							gpstr+="\""+GroupsElement.Items[i].Value+"\"";
						}
					}
					i++;
				}
				*/
				gpstr="["+gpstr+"]";
				console.Info(gpstr);
				string id=this.genID();
				string objstr="{\"cmd\":\"add-group\",\"username\":\""+this.obj.username+"\",\"groups\":"+gpstr+"}";
				console.Info("ID for add group query: \""+id+"\"");
				Database sins=new Database();
				sins.SetAction(DatabasePrincipal.InsertPrincipal);
				sins.AddColumn("id",id);
				sins.AddColumn("data",objstr);
				sins.Send();
				sins.InvokeApplication();
				console.Log("Request to add group was queried.");
				console.Info(objstr);
				sins.ResponseWait();
				Thread.Sleep(1000);
				sins.Clear();
				/*
				ins.ResponseWait();
				ins.AddWhere("id",id);
				ins.RemoveRecord(id);
				console.Info(id);
				*/
				Database dbins=new Database();
				dbins.SetAction(DatabasePrincipal.SelectPrincipal);
				dbins.AddColumn("id",id);
				dbins.AddWhere("id",id);
				dbins.AsyncRemoveRecordFromId(3,id);
			}
		}

		// Performs poast-back action.
		private void ProcessPostBack() {
			bool p=true;
			string data="";
			if (debug.Checked) {
				p=false;
				console.Info("Debug mode has been enabled.");
			}
			if (!(password.Text.Length>0 && password_confirm.Text.Length>0)) {
				p=false;
			}
			//this.UsernameString="PASS";
			if (p) {
				console.Info("Performing normal tasks...");
				if (AD.isset(this.obj,"username")) {
					if (AD.isset(this.obj,"cmd")) {
						if (this.obj.cmd=="set-password") {
							this.ModeString="set-password";
						} else if (this.obj.cmd=="new-user") {
							this.ModeString="new-user";
						}
						
					} else {
						//status.Text+="<br>ERROR: MISSING CMD PROPERTY FROM USER OBJECT.<br>";
						console.Error("Missing \"cmd\" property from \"User\" object.");
					}
					this.PasswordString=Request.Form.GetValues("password")[0];
					if (this.PasswordString.Length>0) {
						if (this.validate(this.PasswordString)) {
							data="{\"cmd\":\"" + this.ModeString + "\",\"username\":\"" + this.UsernameString + "\",\"password\":\"" + this.PasswordString + "\"}";
							console.Info("Preparing to send regulated command.");
							this.queryRequest(data);
							//status.Text+="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br>ALPHA<br>"+data+"<br><br>" + resetPassword.ending;
							this.EndingSuccess();
							//form_main.Action="";
							//this.AsyncGetUGroups();
						} else {
							password.Text=this.sqlParse(this.PasswordString);
							password_confirm.Text=this.sqlParse(this.PasswordString);
							console.Info("Password has been modified.");
							status.Text+="Your password has been modified for validation, please review the changed password and re-submit this form.";
						}
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
					console.Error("Failed to query your request to/for " + m + ".<br>This issue has been reported to the developer.<br><br>Case reference number: <font class=\"case\">" + cref + "</font>" + resetPassword.ending);
				}
			} else {
				//console.Info("Preparing to send debug command.");
				//this.Debug(this.obj);
			}
		}

		
		// Checks object for valid username.
		private bool CheckString(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					if (q.Length>2) {
						res=true;
					}
				}
			}
			return res;
		}
		// Processes session data.
		private void ProcessSessionData() {
			if (this.pass) {
				if (this.post_isset("data")) {
					if (AD.isset(this.obj,"cmd")) {
						if (this.CheckString(this.obj.cmd)) {
							if (AD.isset(this.obj,"username")) {
								if (this.CheckString(this.obj.username)) {
									username.Text=this.obj.username;
									username.Enabled=false;
									this.UsernameString=this.obj.username;
								} else {
									sys.error("Username property does not exist or is not set.");
									this.redirect();
								}
							} else if (CasAuthentication.CurrentPrincipal!=null) {
								username.Text=this.UsernameString;
								username.Enabled=false;
								console.Error("Failed to authenticate with the CAS client.");
							} else {
								username.Text="[FAILED]";
								username.Enabled=false;
								console.Error("Failed to get username request.");
							}
						} else {
							sys.error("Command was not specified.");
							this.redirect();
						}
					} else {
						console.Error("Command property does not exist within the object.");
						this.redirect();
					}
				} else {
					console.Error("POST argument does not contain data.");
					this.redirect();
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
							console.Error("Command does not exist.");
							this.redirect();
						}
					} else {
						console.Error("Command property does not exist or is not set.");
						this.redirect();
					}
				} else {
					this.DisableSubmitButton();
					console.Error("Failed to get data.");
					this.redirect();
				}
			} else {
				console.Error("Failed to pass checks.");
				this.redirect();
			}
		}

		// Disables the submit button.
		private void DisableSubmitButton() {
			submit_btn.Text="[DISABLED]";
			submit_btn.Enabled=false;
		}

		// Prepares and validates the username.
		private bool ValidateUsername() {
			bool res=true;
			if (!(this.UsernameString.Length>0) && !(this.obj.username.Length>0)) {
				sys.error("No username found.");
				console.Error("Username is missing.");
				this.pass=false;
				res=false;
				this.redirect();
			}
			return res;
		}

		// Gets the session data.
		private void GetSessionData() {
			string d="{}";
			try{
				try{
					d=Session["data"].ToString();
				}catch(Exception exc){
					console.Error("Failed to collect data property value... Only ("+Session.Count.ToString()+") properties exist in the session variable.");
				}
				try{
					this.obj=JsonSerializer.Deserialize<User>(d);
				}catch(Exception er){
					console.Error("Failed to deserialize User JSON object.\n\t\t"+er.Message);
				}
				try{
					this.id=this.obj.id;
				}catch(Exception et){
					console.Warn("Failed to set id from object reference.\n\t\t"+et.Message);
				}
				this.pass=true;
			}catch(Exception ex){
				sys.error(ex.Message);
				console.Error(ex.Message);
				if (!String.IsNullOrEmpty(this.obj.cmd) && !String.IsNullOrEmpty(this.obj.username)) {
					this.pass=true;
				} else {
					if (String.IsNullOrEmpty(this.obj.cmd)) {
						sys.error("User object is missing the command specification.");
						console.Error("User object is missing the command specification.");
					} else if (!String.IsNullOrEmpty(this.obj.username)) {
						sys.error("User object is missing the username specification.");
						console.Error("User object is missing the username specification.");
					}
					this.redirect();
				}
			}
		}

		// Sets the connection string for the SQL database.
		private void SetConnectionString() {
			try{
				resetPassword.constr=@"Data Source=" + resetPassword.db_ip + ";Initial Catalog=" + resetPassword.db + ";Persist Security Info=True;User ID=" + resetPassword.db_username + ";Password=" + resetPassword.db_password + ";";
			}catch(Exception e){
				console.Error("Failed to establish connection string.\n\t\t"+e.Message);
			}
		}
		// Asynchronously attempts to get the user groups...
		public async Task<int> AsyncGetUGroups() {
			await this.AsyncGCBuffer();
			return 1;
		}
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
				resetPassword.StatusElm=status;
				this.StatElm=status;
				// ToDo: Implement group element event listener to occur when a group item is selected.
				//GroupsElement.Items.Add("VD-VLAB3");
				//group_container.Visible=false;
				try{
					Element.groupList.Clear();
					Element.SetGroupElement(GroupsElement);
					Element.AddGroup("VD-ADOBECC","Adobe");
					Element.AddGroup("BUSINESS VIRTUAL LAB","Business Virtual Lab");
					Element.AddGroup("BUSINESS VIRTUAL LAB 2","Business Virtual Lab 2");
					Element.AddGroup("MATH VIRTUAL LAB","Math Virtual Lab");
					Element.AddGroup("VD-VLAB3","VLAB-3");
					Element.SetGroups();
				}catch(Exception e){
					console.Warn("Failure at...\n\t\t"+e.Message);
				}
				Groups gp=new Groups(this);
				gp.ProcessUserGroups();					// Takes about 2 seconds.
				int i=0;
				// Iterate through the group names...
				while(i<gp.User_Groups.Count){
					gp.SelectGroup(gp.User_Groups[i]);
					i++;
				}
				this.SetSubmitText("Save Changes");
			}catch(Exception ex){
				console.Error("Failed to set status element.\n\t\t"+ex.Message);
			}
		}

		public void EnableForm() {
			submit_btn.Enabled=true;
		}
		
		// Collects grouping information.
		private void GetGroupings() {
			Element.SetGroupElement(GroupsElement);
		}

		// Performs a debugging operation.
		private void Debug(User obj) {
			//string str="{\"cmd\":\"add-group\",\"username\":\""+obj.username+"\",\"GroupsElement\":[\"VD-VLAB4\"]}";
			//this.queryRequest(str);
			/*
			Database ins=new Database();
			ins.SetAction(DatabasePrincipal.InsertPrincipal);
			ins.AddColumn("id",this.id);
			ins.AddColumn("data",str);
			ins.Send();
			*/
			console.Warn("Sending debug request...");
		}
		// Returns true if the session variable exists.
		protected bool post_isset(string key) {
			bool res=false;
			if (System.Web.HttpContext.Current.Session[key]!=null) {
				res=true;
			}
			return res;
		}
		// Sends a request query to the database and waits for a response.
		protected void queryRequest(string q="") {
			if (q.Length > 0) {
				string id=this.genID();
				//q=this.sqlParse(q);
				//string values="'"+id+"','"+q+"'";
				//status.Text+="<br><br>DATA<br>"+id+"<br><br>";
				//string values=" @DATA ";
				string sql="INSERT INTO " + resetPassword.tb + " (\"id\",\"data\") VALUES ( @ID, @DATA );";
				//status.Text+=sql;
				try{
					/*
					using (SqlConnection con=new SqlConnection(resetPassword.constr)) {
						SqlCommand cmd=new SqlCommand(sql,con);
						cmd.Parameters.AddWithValue("@ID",id);
						cmd.Parameters.AddWithValue("@DATA",q);
						con.Open();
						cmd.ExecuteNonQuery();
						con.Close();
					}
					*/
					Database ins=new Database();
					ins.SetAction(DatabasePrincipal.InsertPrincipal);
					ins.AddColumn("id",id);
					ins.AddColumn("data",q);
					try{
						if (ins.Send()) {
							try {
								ins.InvokeApplication();
							}catch(Exception e){
								console.Error("Failed to invoke application from queryRequest...\n\t\t"+e.Message);
							}
							try{
								this.ResponseWait(id);
							}catch(Exception ee){
								console.Error("Failed to wait for response...\n\t\t"+ee.Message);
							}
						} else {
							console.Error("An error occurred that prevented the query request from being executed...");
						}
					}catch(Exception e){
						console.Error("Failed to process database query...\n\t\t"+e.Message);
					}
					// Wait for response...
					//this.ResponseWait(id);
				}catch(Exception e){
					CaseLog cl=new CaseLog();
					cl.code="0x0001";
					cl.status="fatal";
					cl.title="SQL Submission failed";
					cl.msg="An error occurred while attempting to process the form submission.\nPerhaps there is a syntax error in the SQL query.\n\nSQL Query:\t\t" + sql + "\n\nEND OF LINE";
					cl.data=sql;
					string _obj_=JsonSerializer.Serialize(cl);
					string cref=Case.createCase(_obj_);
					status.Text="An SQL error occurred while attempting to process your request.<br>The issue has been reported to the developer.<br>Your case reference number is <font class=\"case\">" + cref + "</font>" + resetPassword.ending;
					console.Error("An error has occurred while attempting to query the request...\n\t\t"+e.Message);
				}
			}
			
		}
		// Returns true on success, false otherwise. Repeats until a response was issued by the AD program.
		protected bool ResponseWait(string id=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					if (this.counter<50) {
						Thread.Sleep(100);
						string resp=null;
						try{
							resp=this.CheckRecordResponse(id);
						}catch(Exception e){
							console.Error("Failed to check for record response...\n\t\t"+e.Message);
							resp="failed";
						}
						if (resp==null) {
							Thread.Sleep(50);
							this.ResponseWait(id);
						} else {
							if (resp=="success") {
								res=true;
							} else if (resp=="failed") {
								res=false;
							}
							Database dbins=new Database();
							dbins.RemoveRecord(id);
						}
					} else {
						console.Error("Response check timed out.");
					}
				} else {
					console.Error("The provided ID was invalid.");
				}
			} else {
				console.Error("The provided ID was invalid.");
			}
			return res;
		}
		// Returns the record response on success, null otherwise.
		protected string CheckRecordResponse(string id=null) {
			string res=null;
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					Database dbins=new Database();
					if (dbins.RecordExists(id)) {
						List<Dictionary<string,string>> list=this.GetMatchingRecords(id);
						console.Info(list.Count.ToString());
						int i=0;
						uint tmp;
						string msg;
						string file;
						string line;
						string path;
						string str;
						Status ins=new Status();
						while(i<list.Count){
							if (list[i]["id"]==id) {
								if (!String.IsNullOrEmpty(list[i]["data"])) {
									if (!String.IsNullOrWhiteSpace(list[i]["data"])) {
										ins.id=list[i]["id"];
										ins.data=list[i]["data"];
										ins.ToObject();
										tmp=ins.GetStatus();
										msg=ins.GetMessage();
										file=ins.GetFileName();
										line=ins.GetLine();
										path=ins.GetSource();
										if (msg==null) {
											msg="[NULL]";
										}
										if (msg==null) {
											msg="[NULL]";
										}
										if (file==null) {
											file="[NULL]";
										}
										if (path==null) {
											path="[NULL]";
										}
										if (line==null) {
											line="[NULL]";
										}
										str=msg+"\n\tFrom \""+path+"/"+file+"\" ("+line+")";
										if (tmp!=0x00) {
											if (tmp==0x01) {
												console.Error("Failed to process your request...\n\t\t"+str);
												res="failed";
											} else {
												console.Log("Process completed successfully.");
												res="success";
											}
										}
									}
								}
							}
							i++;
						}
					} else {
						console.Error("Record does not exist!");
						res="failed";
					}

				}
			}
			return res;
		}
		// Returns a list consisting of all the records that match the ID.
		protected List<Dictionary<string,string>> GetMatchingRecords(string id=null) {
			List<Dictionary<string,string>> res=new List<Dictionary<string,string>>();
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					int lim=this.GetMatchingRecordsCount(id);
					if (lim>0) {
						string sql="SELECT * FROM " + resetPassword.tb + " WHERE id= @ID ;";
						//int i=0;
						int o=0;
						List<string> cols=this.GetColumns();
						Dictionary<string,string> tmp=new Dictionary<string,string>();
						try{
							using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
								SqlCommand cmd=new SqlCommand(sql,con);
								cmd.Parameters.AddWithValue("@ID",id);
								con.Open();
								SqlDataReader r=cmd.ExecuteReader();
								while(r.Read()){
									o=0;
									if (tmp.Count>0) {
										tmp.Clear();
									}
									while(o<cols.Count){
										tmp.Add(cols[o],r.GetString(o));
										o++;
									}
									res.Add(tmp);
								}
								con.Close();
							}
						}catch(Exception ex){
							console.Error("Failed to get matching records...\n\t\t"+ex.Message);
						}
					}
				}
			}
			return res;
		}
		// Returns a List consisting of all the columns in the database.
		protected List<string> GetColumns() {
			List<string> res;
			if (this.cols!=null) {
				res=this.cols;
			} else {
				string sql="SELECT * FROM "+resetPassword.tb+";";
				int i=0;
				int lim=0;
				res=new List<string>();
				using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					con.Open();
					try{
						SqlDataReader r=cmd.ExecuteReader();
						lim=r.FieldCount;
						while(i<lim){
							res.Add(r.GetName(i));
							i++;
						}
					}catch(Exception e){
						console.Error("Failed to get column names...\n\t\t"+e.Message);
					}
					/*
					lim=r.FieldCount;
					while(i<lim){
						//res.Add(r.GetName(i));
						res.Add(r.GetString(i));
						i++;
					}
					*/
					con.Close();
				}
			}
			return res;
		}
		// Returns the number of records that match the ID.
		protected int GetMatchingRecordsCount(string id=null) {
			int res=0;
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					string sql="SELECT COUNT(*) AS TOTAL FROM " + resetPassword.tb + " WHERE id= @ID ;";
					try{
						Database ins=new Database();
						ins.SetAction(DatabasePrincipal.SelectPrincipal);
						ins.AddColumn("id",id);
						ins.AddWhere("id",id);
						ins.Send();
						res=ins.output.Count;
						/*
						using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
							SqlCommand cmd=new SqlCommand(sql,con);
							cmd.Parameters.AddWithValue("@ID",id);
							con.Open();
							SqlDataReader r=cmd.ExecuteReader();
							if (r.HasRows) {
								
								while(r.Read()){
									res=r.GetInt32(0);
									break;
								}
								
								//res=r["TOTAL"].;
							}
							con.Close();
						}
						*/
					}catch(Exception ex){
						console.Error("Failed to get number of matching records...\n\t\t"+ex.Message);
					}
				}
			}
			return res;
		}
		// Validates the string based on regular expressions.
		protected bool validate(string q="") {
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
			string sql="SELECT COUNT(id) FROM " + resetPassword.tb + " WHERE id= @ID ;";
			
			int len=0;
			try{
				using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
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