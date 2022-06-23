using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services;
using VLAB_AccountServices.services.assets.sys;
using VLAB_AccountServices.services.assets.svr;

namespace VLAB_AccountServices {
	public partial class Default : System.Web.UI.Page
	{
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
		// Performs checks to see if the 
		protected void Page_Load(object sender, EventArgs e)
		{
			console.ini(this);
			Default.StatusElm=status;
			Default.st=status;
			this.StatElm=status;
			console.Log("Page loaded successfully!");
			
			Default.constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
			//Default.constr=@"Data Source=" + Default.db_ip_alt + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username_alt + ";Password=" + Default.db_password_alt + ";";

			Session.Clear();
			Session.Add("data","");
			this.obj=new User();															// Creates a new instance of the User class object.
			sys.errored=false;																// Resets the error status.
			sys.clear();																	// Clears the output buffer.
			console.Log("Primary initialization completed successfully!");
			console.Error("PASSER");
			this.ini();
		}
		private void ini() {
			console.Log("Checking CAS principal...");
			//sys.Write("Checking CAS principal...");
			if (CasAuthentication.CurrentPrincipal!=null) {									// Checks if the user has gone through the CAS system.
				//sys.Write("CAS principal is not null, proceeding with process...");
				ICasPrincipal sp=CasAuthentication.CurrentPrincipal;						// Creates a new instance of the CAS principal.
				string username=System.Web.HttpContext.Current.User.Identity.Name;			// Gets the UH username from the CAS principal.
				//sys.Write("Username has been collected from the CAS system with it's value as &quot;"+username+"&quot;.");
				this.obj.username=username;													// Stores the username within the User object.
				//this.obj=obj;																// Stores the User object into the class 
				//sys.Write("Objects have been populated... Processing username check request.");
				console.Log("Checking username...");
				this.checkUser(username);													// Sends the request to the db and waits for the response.
				//sys.Write("Attempting to redirect to the form.");
				this.Redirect();															// Redirects the user to the form page.
			} else {
				sys.error("");
				console.Error("Unauthorized access detected.");
			}
		}
		// Asynchronously invokes the store procedure that invokes the script.
		protected void InvokeApplication() {
			using(var con=new SqlConnection(Default.constr)) {
				SqlCommand cmd=new SqlCommand("AccountServicesInvokeADApplication",con);	// Command that invokes the SQL stored procedure that invokes the AD script.
				cmd.CommandType=CommandType.StoredProcedure;								// Specifies the type of SQL command being used.
				con.Open();																	// Opens the database connection.
				cmd.ExecuteNonQuery();														// Executes the SQL command.
				con.Close();																// Closes the database connection.
			}
		}
		// Sends a request to check if the username exists on the AD server.
		protected bool checkUser(string username) {
			console.Log("Attempting to check user...");
			bool res=false;
			string id=this.genID();															// Gets a unique randomized string of characters for the record id.
			string data="{\"cmd\":\"check-user\",\"username\":\"" + username + "\"}";		// The data value of the request that indicates the request to check if the user exists...
			string sql="INSERT INTO " + Default.tb + " (\"id\",\"data\") VALUES ( @ID , @DATA );";
			try{
				using (SqlConnection con=new SqlConnection(Default.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					cmd.Parameters.AddWithValue("@ID",id);									// Sanitizes the id string.
					cmd.Parameters.AddWithValue("@DATA",data);								// Sanitizes the data string.
					con.Open();																// Opens a connection to the database.
					cmd.ExecuteNonQuery();													// Executes the SQL query.
					con.Close();															// Closes the database connection.
					console.Log("SQL query has been processed/sent to the database.");
				}
				console.Log("Invoking AD program...");
				this.InvokeApplication();													// Invokes the AD program.
				console.Log("AD Program hsa been invoked.");
				Default.id=id;																// Sets the record id into the class property.
				console.Log("Attempting to check database results.");
				this.dbCheck();																// Proceeds to check the response from the database record.
			}catch(Exception ex){
				sys.error("Insertion Error:\t"+ex.Message+"\n\n"+sql);						// Sets the error that involves an SQL issue.
				console.Error("SQL failed to complete/execute...\n"+ex.Message+"\n\n"+sql);
			}
			return res;
		}
		// Asynchronously sets the session data after the database has returned with the proper data.
		public void dbCheck() {
			console.Log("Attempting to check the resulting records...");
			this.db_check(Default.id);														// Repeats 50 times or until the record has a response.
			this.removeRecord(Default.id);													// Asynchronously removes the record from the database (This works, and does not need to be synchronous) (Removes the record to free up space in the db).
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
			console.Warn(Session["data"].ToString());
			Response.Redirect("services/resetPassword.aspx");								// Redirects the user to the form page.
		}
		// Asynchronously removes the record that matches the record id specified.
		protected void removeRecord(string id) {
			console.Log("Attempting to delete record...");
			string sql="DELETE FROM " + Default.tb + " WHERE id= @ID ;";
			try{
				using(SqlConnection con=new SqlConnection(Default.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);									// Prepares the SQL string query.
					cmd.Parameters.AddWithValue("@ID",id);									// Sanitizes the ID string for SQL processing.
					con.Open();																// Opens a database connection.
					cmd.ExecuteNonQuery();													// Deletes the record from the database.
					con.Close();															// Closes the database connection.
					console.Log("Record has been removed from the database.");
				}
			}catch(Exception e){
				sys.error("Failed to delete record with id of \""+this.parse(id)+"\"\n"+e.Message+"\n\n"+sql);
				console.Error("Failed to delete record with id of \""+this.parse(id)+"\"\n"+e.Message+"\n\n"+sql);
			}
		}
		// Returns the sanitized string.
		public string parse(string q) {
			string exp="[^\\u0020-\\u007e]+";												// Matches all characters that are beyond the scope of the ASCII keyboard characters.
			if (Regex.IsMatch(q,exp)) {
				q=Regex.Replace(q,exp,"");													// Replaces all characters that are not alpha-numeric characters (Including whitespace characters except for space).
			}
			exp="[\\u0027\\u005c]+";														// Matches characters that would be able to escape the SQL string.
			if (Regex.IsMatch(q,exp)) {
				q=Regex.Replace(q,exp,"");													// Replaces the double and single quotes within the string.
			}
			return q;
		}
		// Asynchronously checks if the database for the results.
		public int db_check(string id) {
			int res=0;																		// Determines how the form page should process the form data.
			string sql="SELECT * FROM " + Default.tb + " WHERE id= @ID ;";
			try{
				using(SqlConnection con=new SqlConnection(Default.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);									// Prepares the SQL string query.
					cmd.Parameters.AddWithValue("@ID",id);									// Sanitizes the ID string for SQL use.
					con.Open();																// Opens a database connection.
					SqlDataReader r=cmd.ExecuteReader();									// Gets the results from the SQL query.
					string tmp="";															// Temporary string data variable (Mult-purpose usage).
					bool pass=false;														// Determines if the process should continue or repeat.
					int i=0;																// Record counter variable.
					if (r.HasRows) {
						while(r.Read()){													// Iterates through all records containing the same record id (In the event there are multiple requests which should NOT happen).
							tmp=r.GetString(1);												// Gets the record data.
							if (tmp.IndexOf("status")!=-1) {								// Checks if the record was changed.
								console.Error(tmp);
								pass=true;													// Sets the continuation variable to true once complete.
								break;														// Breaks out of the loop since there is no need to continue.
							}
							i++;
						}
						if (i>1) {															// Checks if the number of records that exist is invalid.
							sys.error("There were multiple records found matching the id \""+this.parse(id)+"\".<br>Please reload the page and try again.");
							sys.flush();													// Pushes the output to the client.
							sys.clear();													// Clears the output.
							console.Error("There were multiple records fiound matching the id.");
							pass=true;														// Sets the continuation variable to continue with the process.
						}
					} else {
						console.Error("No records found.");
						pass=true;
					}
					con.Close();															// Closes the database connection.
					if (!pass) {															// Checks if the continuation variable is false...
						if (Default.ct<50) {												// If the counter is less than 10...
							Thread.Sleep(100);												// Wait for 1 second...
							Default.ct++;
							this.db_check(id);												// Repeats the check again.
						} else {
							sys.error("Request timmed out.<br>Please reload the page and try again.");
							sys.flush();
							this.removeRecord(id);											// Removes the record from the database to clear up space.
						}
					} else {
						console.Warn("RECORD FOUND!");
						if (tmp.IndexOf("status\":true")!=-1) {								// Checks if the response returned true (Indicates that the user was found on the AD).
							this.pt=1;														// Sets the status variable to indicate that the user was found.
						} else {
							this.pt=2;														// Sets the status variable to indicate that the user was not found.
						}
					}
					res=1;
				}
			}catch(Exception e){
				sys.error("An error occurred while asynchronously checking the database...<br><br>"+e.Message);
				console.Error("An error has occurred while attempting to check the database...\n"+e.Message);
			}
			return res;
		}
		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected string genID() {
			string res="";
			string id=this.genRandID();										 // Gets a randomly generated string of characters.
			string sql="SELECT COUNT(id) FROM " + Default.tb + " WHERE id= @ID ;";
			int len=0;														// Will store the number of records that matched the query.
			try{
				using(SqlConnection con=new SqlConnection(Default.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);					// Prepares the SQL query.
					cmd.Parameters.AddWithValue("@ID",id);					// Sanitizes the ID for the SQL query.
					con.Open();												// Opens a database connection.
					SqlDataReader dr=cmd.ExecuteReader();					// Prepares to read the count results.
					if (dr.HasRows) {										// Checks if there are any matching records (Everything within this scope simply checks the ID).
						while(dr.Read()){									// Iterates through the records.
							len=dr.GetInt32(0);								// Gets the number of records that matched the ID.
							break;											// Breaks out of the loop.
						}
						if (len>0) {										// Checks if the number of matching records is more than 0.
							if (len>1) {									// Checks if the number of matching records exceeds more than one (In which case, delete them, since there shouldnt be more than one).
								this.removeRecord(id);						// Removes the record from the database.
								//res=this.genID();							// Attempts to generate a new record ID.
								res=id;
							} else {
								res=this.genID();							// Generates a new record.
							}
						} else {
							res=id;
						}
					}
					con.Close();											// Closes the database connection.
				}
			}catch(Exception ex){
				sys.error("Failed to generate ID...\n"+ex.Message);
			}
			return res;
		}
		// Generates a randomized length of random characters to compose the record's ID on the database.
		protected string genRandID() {
			Random r=new Random();					// Creates a new instance of a random number object.
			int lim=r.Next(10,128);					// Sets the number of characters within the string.
			int i=0;								// Counter variable that helps iterate to reach the character length specified above.
			char c;									// Will contain the randomly selected character.
			string res="";							// The resulting string.
			int sel=0;								// Randomly selects which set of characters to choose from.
			while(i<lim){							// Repeats the scope within until the character length has been attained.
				sel=r.Next(0,30);					// Randomly selects which set of characters to choose from.
				if (sel<10) {						// Specifies that the character set to use is [NUMBERS].
					c=(char)r.Next(48,57);			// Selects a random numerical character.
				} else if (sel<20) {				// Specifies that the character set to use is [UPPERCASE].
					c=(char)r.Next(65,90);			// Selects a random uppercase character.
				} else {							// Otherwise, specifies the character set to use is [LOWERCASE].
					c=(char)r.Next(97,122);			// Selects a random lowercase character.
				}
				res+=c;								// Appends the character to the string.
				i++;								// Increases the counter variable by 1.
			}
			return res;								// Returns the final/completed/generated string of characters.
		}

	}
}