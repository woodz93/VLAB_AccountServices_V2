using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.classes.Str;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices {
	public partial class Default : System.Web.UI.Page {
		protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
		protected static string constr=null;
		protected int cur_count=0;
		public static Label st;
		public static string id="";
		public static byte mode=0x00;
		protected static int ct=0;
		private User obj;
		private int pt=0;
		public static Label StatusElm;
		public Label StatElm;
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
		protected void Page_Load(object sender, EventArgs e) {
			Default.StatusElm=status;
			Default.st=status;
			this.StatElm=status;
			console.ini(this);
			console.errored=false;
			console.Clear();
			console.Log("Page loaded successfully!");
			Default.constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
			Session.Clear();
			Session.Add("data","");
			this.obj=new User();															// Creates a new instance of the User class object.
			sys.errored=false;																// Resets the error status.
			sys.clear();																	// Clears the output buffer.
			console.Log("Primary initialization completed successfully!");
			this.ini();
		}
		// Invokes first-time processes.
		private void ini() {
			console.Log("Checking CAS principal...");
			if (CasAuthentication.CurrentPrincipal!=null) {									// Checks if the user has gone through the CAS system.
				ICasPrincipal sp=CasAuthentication.CurrentPrincipal;						// Creates a new instance of the CAS principal.
				string username=System.Web.HttpContext.Current.User.Identity.Name;			// Gets the UH username from the CAS principal.
				//sys.Write("Username has been collected from the CAS system with it's value as &quot;"+username+"&quot;.");
				this.obj.username=username;													// Stores the username within the User object.
				console.Log("Checking username...");
				//this.checkUser(username);													// Sends the request to the db and waits for the response.
				this.CheckUsername(username);

				this.Redirect();															// Redirects the user to the form page.
			} else {
				sys.error("");
				console.Error("Unauthorized access detected.");
			}
		}
		// Returns true if the username was found on the AD server.
		private bool CheckUsername(string u=null) {
			bool res=false;
			if (Str.CheckStr(u)) {
				Default.id=this.genID();
				string data="{\"cmd\":\"check-user\",\"username\":\""+u+"\"}";
				console.Log("Creating a new database class instance...");
				Database ins=new Database();
				ins.SetAction(DatabasePrincipal.InsertPrincipal);
				ins.AddColumn("id",Default.id);
				ins.AddColumn("data",data);
				console.Log("Attempting to submit database query...");
				bool tmp=ins.Send();
				if (tmp) {
					ins.InvokeApplication();
					this.dbCheck();
				} else {
					console.Error("An error occurred while attempting to insert a new record into the database...");
				}
			}
			return res;
		}
		// Asynchronously sets the session data after the database has returned with the proper data.
		public void dbCheck() {
			console.Log("Attempting to check the resulting records...");
			this.db_check(Default.id);														// Repeats 50 times or until the record has a response.
			//this.removeRecord(Default.id);													// Asynchronously removes the record from the database (This works, and does not need to be synchronous) (Removes the record to free up space in the db).
			if (sys.errored) {																// Checks if there are any errors that were thrown.
				console.Error("System errored out.");
			} else {
				sys.clear();																// Clears the output buffer.
				console.Log("Value of pt is \""+pt+"\"");
				if(this.pt==1) {															// Determines what the command/process should be on the form.
					this.obj.cmd="set-password";											// Indicates a password reset operation (Occurs if the user does exist on the AD).
				} else if(this.pt==2) {
					this.obj.cmd="new-user";												// Indicates that a new user should be created (Occurs if the user does not exist on the AD).
				}
				string data="{\"id\":\""+Default.id+"\",\"cmd\":\""+this.obj.cmd+"\",\"username\":\""+this.obj.username+"\"}";
				Session["data"]=data;														// Stores the JSON configuration from the above line.
			}
		}
		// Performs a redirect.
		protected void Redirect() {
			if (!(console.errored) && Default.mode==0x00) {
				Response.Redirect("services/resetPassword.aspx");
			} else {
				console.Info("Unable to redirect... either a debugging or error was thrown...");
			}
		}
		// Asynchronously checks if the database for the results.
		public int db_check(string id) {
			int res=0;																		// Determines how the form page should process the form data.
			try{
				bool pass=false;
				Database ins=new Database();
				ins.SetAction(DatabasePrincipal.SelectPrincipal);
				ins.AddWhere("id",id);
				ins.AddColumn("id",id);
				ins.ResponseWait();
				ins.AddColumn("data");
				ins.Send();
				Dictionary<string,string>tmp=null;
				if (ins.Results.Count()>0) {
					int i=0;
					while(ins.Results.Read()){
						tmp=ins.Results.GetRow(i);
						if (tmp["data"].Length>0) {
							if (tmp["data"].Contains("status")) {
								pass=true;
								break;
							}
						}
						i++;
					}
				}
				if (!pass) {
					this.db_check(id);
				} else {
					if (tmp!=null) {
						if (tmp["data"].Contains("status\":true")) {
							this.pt=1;
						} else if (tmp["data"].Contains("status\":false")) {
							this.pt=2;
							console.Error("An error has occurred.");
						}
						Database ins0=new Database();
						ins0.RemoveRecord(id);
					} else {
						this.db_check(id);
					}
				}
			}catch(Exception e){
				sys.error("An error occurred while asynchronously checking the database...<br><br>"+e.Message);
				console.Error("An error has occurred while attempting to check the database...\n"+e.Message);
			}
			return res;
		}
		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected string genID() {
			Database ins=new Database();
			return ins.GetUniqueID();
		}
	}
}