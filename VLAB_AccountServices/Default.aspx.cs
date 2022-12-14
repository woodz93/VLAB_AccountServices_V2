using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.classes.Str;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices
{
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
		private UserCheck UD=null;
		protected void Page_Unload(object sender, EventArgs e) {
			//this.CleanUp();
		}
		// Cleans up the database.
		protected async Task<int> CleanUp() {
			Database ins=new Database();
			await Task.Delay(1000);
			ins.AsyncRemoveAllRecords();
			return 1;
		}
		protected void Page_Load(object sender, EventArgs e) {
			CleanUp();
			Default.StatusElm=status;
			Default.st=status;
			StatElm=status;
			Session.Clear();
			UD=new UserCheck();
			obj=new User();
			sys.errored=false;
			ConsoleOutput.ini(this);
			ConsoleOutput.Clear();
			ConsoleOutput.errored=false;
			Thread.Sleep(100);
			Response.Redirect("services/resetPassword.aspx");
			/*
			Default.StatusElm=status;
			Default.st=status;
			this.StatElm=status;
			console.ini(this);
			console.errored=false;
			console.Clear();
			console.Log("Page loaded successfully!");
			//Default.constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
			Session.Clear();
			Session.Add("data","");
			this.obj=new User();															// Creates a new instance of the User class object.
			sys.errored=false;																// Resets the error status.
			sys.clear();																	// Clears the output buffer.
			console.Log("Primary initialization completed successfully!");
			this.ini();
			*/
		}
		// Invokes first-time processes.
		private void ini() {
			ConsoleOutput.Log("Checking CAS principal...");
			UD=new UserCheck();
			if (UD.Ready) {
				obj.username=UD.GetUsername();
				StatElm.Text+="<br><br>- Username: &quot;"+UD.GetUsername()+"&quot;";
				ConsoleOutput.Log("Checking username...");
				if(CheckUsername(obj.username))
					Redirect();
				else
					StatElm.Text+="<br><br>- Username check failed...";
			} else {
				sys.error("Unauthorized access detected.");
				ConsoleOutput.Error("Unauthorized access detected.");
				StatElm.Text+="<br><br>- Unauthorized access detected.";
			}
		}
		// Returns true if the username was found on the AD server.
		private bool CheckUsername(string u=null) {
			bool res=false;
			if(Str.CheckStr(u))
			{
				Default.id=this.genID();
				string data = "{\"cmd\":\"check-user\",\"username\":\""+u+"\"}";
				ConsoleOutput.Log("Creating a new database class instance...");
				Database ins = new Database();
				ins.SetAction(DatabasePrincipal.InsertPrincipal);
				ins.AddColumn("id",Default.id);
				ins.AddColumn("data",data);
				ConsoleOutput.Log("Attempting to submit database query...");
				bool tmp = ins.Send();
				if(tmp)
				{
					StatElm.Text+="<br><br>- Insertion successful!";
					ins.InvokeApplication();
					dbCheck();
					res=true;
				}
				else
				{
					StatElm.Text+="<br><br>- Insertion failed!";
					ConsoleOutput.Error("An error occurred while attempting to insert a new record into the database...");
					sys.error("Failed to query request.");
					StatElm.Text+="<br><br>- Failed to query request.";
				}
			}
			else
			{
				StatElm.Text+="<br><br>- Username string failed to pass validation checks! &quot;"+u+"&quot;";
			}
			return res;
		}
		// Asynchronously sets the session data after the database has returned with the proper data.
		public void dbCheck() {
			ConsoleOutput.Log("Attempting to check the resulting records...");
			this.db_check(Default.id);														// Repeats 50 times or until the record has a response.
			//this.removeRecord(Default.id);													// Asynchronously removes the record from the database (This works, and does not need to be synchronous) (Removes the record to free up space in the db).
			if (sys.errored) {																// Checks if there are any errors that were thrown.
				ConsoleOutput.Error("System errored out.");
				sys.error("An error has occurred...");
				StatElm.Text+="<br><br>- ERROR DETECTED";
			} else {
				sys.clear();																// Clears the output buffer.
				ConsoleOutput.Log("Value of pt is \""+pt+"\"");
				
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
			if (!(ConsoleOutput.errored) && Default.mode==0x00) {
				Response.Redirect("services/resetPassword.aspx");
				//sys.error("PASSED");
				//StatElm.Text+="<br><br>- SUCCESS!";
				//StatElm.Text+="<br><br>- "+Session["data"];
			} else {
				ConsoleOutput.Info("Unable to redirect... either a debugging or error was thrown...");
				sys.error("Unable to redirect.");
				StatElm.Text+="<br><br>- Unable to redirect.";
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
							ConsoleOutput.Error("An error has occurred.");
							sys.error("An error has occurred while attempting to check if the user exists...");
							StatElm.Text+="<br><br>- Unable to check if user exists on the system.";
						}
						Database ins0=new Database();
						ins0.RemoveRecord(id);
					} else {
						this.db_check(id);
					}
				}
			}catch(Exception e){
				sys.error("An error occurred while asynchronously checking the database...<br><br>"+e.Message);
				ConsoleOutput.Error("An error has occurred while attempting to check the database...\n"+e.Message);
				StatElm.Text+="<br><br>- "+e.Message;
			}
			return res;
		}
		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected string genID() {
			Database ins=new Database();
			return ins.GetUniqueID();
		}

		private static string getAttribute(ICasPrincipal sessionPrincipal, string key)
		{
			string value = "";
			if (sessionPrincipal != null)
			{
				IAssertion sessionAssertion = sessionPrincipal.Assertion;
				if (sessionAssertion != null)
				{
					if (sessionAssertion.Attributes.ContainsKey(key))
					{
						string[] array = sessionAssertion.Attributes[key].ToArray();
						if (array.Count() > 0)
						{
							value = array[0];
						}
					}
				}
			}
			return value;
		}
	}
}