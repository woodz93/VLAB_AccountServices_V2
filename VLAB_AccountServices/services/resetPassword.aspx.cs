using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services.assets.svr;
using VLAB_AccountServices.services.assets.sys;

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
		protected string Username=null;
		protected string Password=null;
		private string Mode="";

		// Processes the password.
		public void processPassword(Object sender, EventArgs e) {
			string u=username.Text;
			string p=password.Text;
			string pc=password_confirm.Text;
			status.Text+="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + resetPassword.ending;
			submit_btn.Enabled=false;
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
			console.ini(this);
			this.SetConnectionString();
			this.SetElements();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.ini();
			this.obj=new User();
			if (this.post_isset("data") || CasAuthentication.CurrentPrincipal!=null) {
				/*
				ICasPrincipal sp=CasAuthentication.CurrentPrincipal;
				user=System.Web.HttpContext.Current.User.Identity.Name;
				username.Text=user;
				*/
				this.casp=CasAuthentication.CurrentPrincipal;
				this.Username=System.Web.HttpContext.Current.User.Identity.Name;
				username.Text=this.Username;
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
				if (Session.Count>0) {
					console.Log("Number of session variables that exist are ("+Session.Count.ToString()+")");
					this.GetSessionData();
					if (this.pass) {
						if (this.ValidateUsername()) {
							this.ProcessSessionData();				// Sets all elements from session data (Used before submitting the form).
							if (IsPostBack) {
								this.ProcessPostBack();				// Processes the submitted form data.
							}
						}
					} else {
						console.Error("Failed to pass previous check (CHECK CONDUCTED BEFORE USERNAME CHECKING)");
					}
				} else {
					console.Warn("Attempting to perform a redirect...");
					this.redirect();
				}
			} else {
				console.Error("Could not discover parameter data... POST or CAS not initialized.");
				this.redirect();
			}
			console.Log("END OF LINE");
		}

		// Performs poast-back action.
		private void ProcessPostBack() {
			bool p=true;
			string data="";
			if (debug.Checked) {
				p=false;
				console.Info("Debug mode has been enabled.");
			}
			this.Username="PASS";
			if (p) {
				console.Info("Performing normal tasks...");
				if (AD.isset(this.obj,"username")) {
					if (AD.isset(this.obj,"cmd")) {
						if (this.obj.cmd=="set-password") {
							this.Mode="set-password";
						} else if (this.obj.cmd=="new-user") {
							this.Mode="new-user";
						}
						//status.Text+="<br>&quot;"+this.obj.cmd+"&quot;<br>";
						if (!(this.Mode.Length>2)) {
							if (submit_btn.Text=="Reset Password") {
								this.Mode="set-password";
								console.Info("Mode has been set to \"set-password\"");
							} else if (submit_btn.Text=="Create Account") {
								this.Mode="new-user";
								console.Info("Mode has been set to \"new-user\"");
							}
						}
					} else {
						//status.Text+="<br>ERROR: MISSING CMD PROPERTY FROM USER OBJECT.<br>";
						console.Error("Missing \"cmd\" property from \"User\" object.");
					}
					this.Password=Request.Form.GetValues("password")[0];
					if (this.validate(this.Password)) {
						data="{\"cmd\":\"" + mode + "\",\"username\":\"" + this.Username + "\",\"password\":\"" + this.Password + "\"}";
						console.Info("Preparing to send regulated command.");
						this.queryRequest(data);
						status.Text+="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br>ALPHA<br>"+data+"<br><br>" + resetPassword.ending;
					} else {
						password.Text=this.sqlParse(this.Password);
						password_confirm.Text=this.sqlParse(this.Password);
						console.Info("Password has been modified.");
						status.Text+="Your password has been modified for validation, please review the changed password and re-submit this form.";
					}
				} else {
					this.Username="NULL";
					string m="[UNKNOWN]";
					if (this.Mode=="new-user") {
						m="create a new user account";
					} else if (this.Mode=="set-password") {
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
				console.Info("Preparing to send debug command.");
				this.Debug(this.obj);
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
									this.Username=this.obj.username;
								} else {
									sys.error("Username property does not exist or is not set.");
									this.redirect();
								}
							} else if (CasAuthentication.CurrentPrincipal!=null) {
								username.Text=this.Username;
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
							submit_btn.Text="Create Account";
							this.Mode="new-user";
						} else if (this.obj.cmd=="set-password") {
							submit_btn.Text="Reset Password";
							this.Mode="set-password";
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
			if (!(this.Username.Length>0) && !(this.obj.username.Length>0)) {
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
		// Prepares all HTML elements for use...
		private void SetElements() {
			try{
				resetPassword.StatusElm=status;
				this.StatElm=status;
			}catch(Exception ex){
				console.Error("Failed to set status element.\n\t\t"+ex.Message);
			}
		}
		// Performs a debugging operation.
		private void Debug(User obj) {
			string str="{\"cmd\":\"add-group\",\"username\":\""+obj.username+"\",\"groups\":[\"VD-VLAB4\"]}";
			DataBase ins=new DataBase();
			ins.SetAction(DataBasePrincipal.UpdatePrincipal);
			ins.AddColumn("id",this.id);
			ins.AddColumn("data",str);
			ins.Send();
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
				string sql="INSERT INTO " + resetPassword.tb + " (\"id\",\"data\") VALUES ('"+id+"', @DATA );";
				//status.Text+=sql;
				try{
					using (SqlConnection con=new SqlConnection(resetPassword.constr)) {
						SqlCommand cmd=new SqlCommand(sql,con);
						//cmd.Parameters.AddWithValue("@ID",id);
						cmd.Parameters.AddWithValue("@DATA",q);
						con.Open();
						cmd.ExecuteNonQuery();
						con.Close();
					}
					// Wait for response...
					this.ResponseWait(id);
				}catch{
					CaseLog cl=new CaseLog();
					cl.code="0x0001";
					cl.status="fatal";
					cl.title="SQL Submission failed";
					cl.msg="An error occurred while attempting to process the form submission.\nPerhaps there is a syntax error in the SQL query.\n\nSQL Query:\t\t" + sql + "\n\nConnection string:\t\t" + resetPassword.constr + "\n\nEND OF LINE";
					cl.data=sql;
					string _obj_=JsonSerializer.Serialize(cl);
					string cref=Case.createCase(_obj_);
					status.Text="An error occurred while attempting to process your request.<br>The issue has been reported to the developer.<br>Your case reference number is <font class=\"case\">" + cref + "</font>" + resetPassword.ending;
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
						if (this.CheckRecordResponse(id)==null) {
							Thread.Sleep(50);
							this.ResponseWait(id);
						}
					}
				}
			}
			return res;
		}
		// Returns the record response on success, null otherwise.
		protected string CheckRecordResponse(string id=null) {
			string res=null;
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					List<Dictionary<string,string>> list=this.GetMatchingRecords(id);
					int i=0;
					uint tmp=0x0000;
					string msg;
					string file;
					string line;
					string path;
					string str="";
					Status ins=new Status();
					while(i<list.Count){
						if (list[i]["id"]==id) {
							if (!String.IsNullOrEmpty(list[i]["data"])) {
								if (!String.IsNullOrWhiteSpace(list[i]["data"])) {
									ins.id=list[i]["id"];
									ins.data=list[i]["data"];
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
											console.Error("Failed to process your request...\n"+str);
											res="Failed";
										} else {
											console.Log("Process completed successfully.");
											res="Success";
										}
									}
								}
							}
						}
						i++;
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
						string sql="SELECT * AS TOTAL FROM " + resetPassword.tb + " WHERE id= @ID ;";
						int i=0;
						int o=0;
						List<string> cols=this.GetColumns();
						Dictionary<string,string> tmp=new Dictionary<string,string>();
						using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
							SqlCommand cmd=new SqlCommand(sql,con);
							cmd.Parameters.AddWithValue("@ID",id);
							con.Open();
							SqlDataReader r=cmd.ExecuteReader();
							while(i<lim){
								o=0;
								if (tmp.Count>0) {
									tmp.Clear();
								}
								while(o<cols.Count){
									tmp.Add(cols[o],r.GetString(o));
									o++;
								}
								res.Add(tmp);
								i++;
							}
							con.Close();
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
				string sql="";
				int i=0;
				int lim=0;
				res=new List<string>();
				using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					con.Open();
					SqlDataReader r=cmd.ExecuteReader();
					lim=r.FieldCount;
					while(i<lim){
						res.Add(r.GetName(i));
						i++;
					}
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
					using(SqlConnection con=new SqlConnection(resetPassword.constr)) {
						SqlCommand cmd=new SqlCommand(sql,con);
						cmd.Parameters.AddWithValue("@ID",id);
						con.Open();
						SqlDataReader r=cmd.ExecuteReader();
						if (r.HasRows) {
							while(r.Read()){
								res++;
							}
						}
						con.Close();
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