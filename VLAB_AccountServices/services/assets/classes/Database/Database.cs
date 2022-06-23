using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VLAB_AccountServices.services.assets.sys;
using VLAB_AccountServices.services.assets.classes.sys;
using VLAB_AccountServices.services.assets.classes.Network;

namespace VLAB_AccountServices.services.assets.classes.Database {
	public class Database {

		private static string db="UHMC_VLab";
		private static string tb="vlab_pendingusers";
		private static string db_ip="172.20.0.142";
		private static string db_port="1433";
		private static string db_username="uhmcad_user";
		private static string db_password="MauiC0LLegeAD2252!";
		private bool use_port=false;
		private string constr=null;
		private bool con_open=false;
		private SqlConnection con=null;
		private List<string> cols=new List<string>();
		//private List<string> vals=new List<string>();
		private Dictionary<string,string> pairs=new Dictionary<string,string>();
		private List<uint> action_list=new List<uint>();

		private uint action=0x0000;
		private List<Dictionary<string,string>> output=new List<Dictionary<string,string>>();
		private List<Dictionary<string,string>> error_buffer=new List<Dictionary<string,string>>();
		public bool response_received=false;
		public string ResponseMessage=null;

		//
		// Summary:
		//			Creates an instance of the Database object.
		//
		// Returns:
		//			Nothing.
		public Database() {
			this.constr=@"Data Source=" + Database.db_ip + ";Initial Catalog=" + Database.db + ";Persist Security Info=True;User ID=" + Database.db_username + ";Password=" + Database.db_password + ";";
			this.IniPopulateActionList();
		}
		

		//
		// Summary:
		//	Gets the value from a specified key name from the key-value pairs list.
		//
		// Parameters:
		//	key:
		//		A string representing the key that exists within the key-value pair list.
		//
		// Returns:
		//	The value of the value-pair.
		public string GetValue(string key=null) {
			string res=null;
			if (!String.IsNullOrEmpty(key)) {
				if (!String.IsNullOrWhiteSpace(key)) {
					if (this.cols.Contains(key)) {
						res=this.pairs[key];
					}
				}
			}
			return res;
		}
		// Returns a string consisting of id-value pairs (PRE-SANITIZED).
		public string GeneratePairs(string condition_override="and") {
			string res=null;
			condition_override=this.parse(condition_override.ToUpper());
			if (this.cols.Count>0) {
				int i=0;
				res="";
				while(i<this.cols.Count){
					if (i>0) {
						res+=" "+condition_override+" "+this.cols[i]+"= @"+this.cols[i].ToUpper();
					} else {
						res+=""+this.cols[i]+"= @"+this.cols[i].ToUpper();
					}
					i++;
				}
			}
			return res;
		}
		// Clears all pairs and columns.
		public void Clear() {
			this.pairs.Clear();
			this.cols.Clear();
			this.action=0x0000;
		}
		// Returns true if the record matching the specified id is found.
		public bool RecordExists(string id=null) {
			bool res=false;
			bool pass=false;
			string sql="";
			if (!String.IsNullOrEmpty(id)) {
				if (!String.IsNullOrWhiteSpace(id)) {
					this.AddColumn("id");
					this.SetValue("id",id);
				}
			}
			if (this.cols.Count>0) {
				int i=0;
				string p=this.GeneratePairs();
				sql="SELECT COUNT(*) AS TOTAL FROM "+Database.tb+" WHERE "+p+" ;";
				pass=true;
			}
			if (pass) {
				try{
					int tmp=0;
					using(SqlConnection con=new SqlConnection(this.constr)) {
						try{
							SqlCommand cmd=new SqlCommand(sql, con);
							try{
								int i=0;
								while(i<this.cols.Count){
									cmd.Parameters.AddWithValue("@"+this.cols[i].ToUpper(),this.GetValue(this.cols[i]));
									i++;
								}
								try{
									con.Open();
									try{
										SqlDataReader r=cmd.ExecuteReader();
										if (r.HasRows) {
											while(r.Read()){
												tmp=r.GetInt32(0);
												break;
											}
										}
										if (tmp>0) {
											res=true;
										}
									}catch(Exception e){
										this.Error("Failed to process SQL query... The database connection will be closed...\n"+e.Message+"\n\n"+sql);
									}
									con.Close();
								}catch(Exception e){

								}
							}catch(Exception e){
								this.Error("Failed to sanitize the id for SQL processing...\n"+e.Message);
							}
						}catch(Exception e){
							this.Error("Failed to process sql query...\n"+e.Message);
						}
					}
				}catch(Exception e){
					this.Error("Failed to establish a connection to the database...\n"+e.Message);
				}
			}
			return res;
		}
		// Processes the query. Any and all output can be retrieved with the "GetOutput" method.
		public bool Send() {
			bool res=false;
			if (this.CheckQuery()) {
				if (!(this.error_buffer.Count>0)) {
					if (this.action==DatabasePrincipal.InsertPrincipal && this.pairs.ContainsKey("data")) {
						res=this.InsertRecord();
					} else if (this.action==DatabasePrincipal.SelectPrincipal && this.CheckColumnID()) {
						res=this.SelectRecord();
					} else if (this.action==DatabasePrincipal.UpdatePrincipal && this.pairs.ContainsKey("data")) {
						res=this.UpdateRecord();
					} else if (this.action==DatabasePrincipal.ExistsPrincipal && this.pairs.ContainsKey("id")) {
						res=this.RecordExists(this.pairs["id"]);
					}
				}
			}
			return res;
		}
		// Asynchronously waits for a response from the AD.
		public async Task<int> WaitForResponse() {
			await Task.Delay(500);
			int tmp=await this.AsyncSelectRecord();
			if (tmp==0) {
				await Task.Delay(500);
				this.WaitForResponse();
			} else {
				if (this.response_received==false) {
					this.response_received=true;
				}
			}
			return 1;
		}
		// Asynchronously selects data from the database.
		private async Task<int> AsyncSelectRecord() {
			int res=0;
			string sql="SELECT * FROM "+Database.tb+" WHERE id='@ID';";
			SqlCommand cmd=new SqlCommand(sql,this.con);
			cmd.Parameters.Add("@ID",SqlDbType.VarChar);
			cmd.Parameters["@ID"].Value=this.pairs["id"];
			Dictionary<string,string> tmp=new Dictionary<string, string>();
			this.Open();
			SqlDataReader dr=cmd.ExecuteReader();
			if (dr.HasRows) {
				while(dr.Read()){
					tmp["data"]=dr.GetString(1);
					if (tmp["data"].Contains("status\":")) {
						tmp["id"]=dr.GetString(0);
						this.output.Add(tmp);
						if (res!=1) {
							res=1;
						}
					}
				}
			}
			this.Close();
			return res;
		}
		// Returns true if one or more records were updated, false otherwise.
		private bool UpdateRecord() {
			bool res=false;
			try{
				int len=0;
				string sql="UPDATE "+Database.tb+" SET data= @DATA WHERE id= @ID ;";
				using(SqlConnection con=new SqlConnection(this.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
					cmd.Parameters.AddWithValue("@DATA",this.pairs["data"]);
					con.Open();
					len=cmd.ExecuteNonQuery();
					con.Close();
				}
				if (len>0) {
					res=true;
				}
			}catch(Exception e){
				this.Error("Failed to update record...\n"+e.Message);
			}
			return res;
		}
		// Invokes the store procedure that invokes the script.
		public void InvokeApplication() {
			try{
				using(var con=new SqlConnection(this.constr)) {
					SqlCommand cmd=new SqlCommand("AccountServicesInvokeADApplication",con);	// Command that invokes the SQL stored procedure that invokes the AD script.
					cmd.CommandType=CommandType.StoredProcedure;								// Specifies the type of SQL command being used.
					con.Open();																	// Opens the database connection.
					cmd.ExecuteNonQuery();														// Executes the SQL command.
					con.Close();																// Closes the database connection.
				}
			}catch(Exception e){
				console.Error("Failed to invoke application...\n\t\t"+e.Message);
			}
		}
		// Selects a record.
		private bool SelectRecord() {
			bool res=false;
			string sql="SELECT * FROM "+Database.tb+" WHERE id= @ID ;";
			Dictionary<string,string> tmp=new Dictionary<string, string>();
			try{
				using(var con=new SqlConnection(this.constr)){
					SqlCommand cmd=new SqlCommand(sql,con);
					try{
						cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
						try{
							con.Open();
							try{
							SqlDataReader dr=cmd.ExecuteReader();
							if (dr.HasRows) {
								while(dr.Read()){
									tmp.Clear();
									tmp["id"]=dr.GetString(0);
									tmp["data"]=dr.GetString(1);
									this.output.Add(tmp);
								}
							}
							con.Close();
							res=true;
							}catch(Exception e){
								console.Error("Failed to process SQL query...\n\t\t"+e.Message);
								con.Close();
							}
						}catch(Exception e){
							console.Error("Failed to open database connection.\n\t\t"+e.Message);
						}
					}catch(Exception ex){
						console.Error("Failed to prepare sql command.\n\t\t"+ex.Message);
					}
				}
			}catch(Exception e){
				console.Error("Failed to establish database connection...\n\t\t"+e.Message);
			}
			return res;
		}
		// Inserts a new record.
		private bool InsertRecord() {
			bool res=false;
			string id=this.GetUniqueID();
			if (!String.IsNullOrEmpty(id)) {
				string sql="INSERT INTO "+Database.tb+" (\"id\",\"data\") VALUES('"+id+"', @DATA );";
				int len=0;
				try{
					using(var con=new SqlConnection(this.constr)){
						SqlCommand cmd=new SqlCommand(sql,con);
						//cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
						cmd.Parameters.AddWithValue("@DATA",this.pairs["data"]);
						con.Open();
						len=cmd.ExecuteNonQuery();
						con.Close();
					}
				}catch(Exception e){
					console.Error("Failed to insert record into database...\n\t\t"+e.Message);
				}
				if (len>0) {
					res=true;
				}
			}
			return res;
		}
		// Returns a unique ID string.
		private string GetUniqueID() {
			string res=null;
			string str=Database.GenerateRandomString();
			string sql="SELECT COUNT(*) AS TOTAL WHERE id= @ID ;";
			int i=0;
			SqlCommand cmd=new SqlCommand(sql,this.con);
			cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
			//cmd.Parameters["@ID"].Value=str;
			this.Open();
			SqlDataReader dr=cmd.ExecuteReader();
			int len=0;
			if (dr.HasRows) {
				while(dr.Read()){
					len=dr.GetInt32(0);
					break;
				}
			}
			this.Close();
			if (len>0) {
				while(len>0 && i<500){
					this.Open();
					str=Database.GenerateRandomString();
					cmd.Parameters["@ID"].Value=str;
					dr=cmd.ExecuteReader();
					if (dr.HasRows) {
						while(dr.Read()){
							len=dr.GetInt32(0);
							break;
						}
						this.Close();
					} else {
						this.Close();
						len=0;
						break;
					}
					if (!(len>0)) {
						break;
					}
					i++;
				}
				if (!(len>0)) {
					res=str;
				}
			}
			this.Close();
			return res;
		}
		// Returns a randomly generated string consisting of a random length of characters.
		private static string GenerateRandomString() {
			string res="";
			Random r=new Random();
			int lim=r.Next(10,128);
			int i=0;
			char c;
			int sel=0;
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
		// Returns true if everything is needed in order to process the query, false otherwise.
		private bool CheckQuery() {
			bool res=false;
			if (this.action!=DatabasePrincipal.NullPrincipal) {
				if (this.action==DatabasePrincipal.SelectPrincipal) {
					if (!(this.cols.Count>0)) {
						res=true;
					} else {
						this.Error("No columns specified.",0x000A);
					}
				} else if (this.action==DatabasePrincipal.InsertPrincipal) {
					if (!(this.cols.Count>0)) {
						if (this.CheckColumnID()) {
							res=true;
						}
					} else {
						this.Error("No columns specified.",0x000A);
					}
				} else if (this.action==DatabasePrincipal.ExistsPrincipal) {
					if (!(this.cols.Count>0)) {
						res=true;
					} else {
						this.Error("No columns specified.",0x000A);
					}
				} else if (this.action==DatabasePrincipal.GenerateUniqueIDPrincipal) {
					res=true;
				} else if (this.action==DatabasePrincipal.DeletePrincipal) {
					if (this.CheckColumnID()) {
						res=true;
					}
				} else if (this.action==DatabasePrincipal.UpdatePrincipal) {
					res=true;
					/*
					if (this.CheckColumnID()) {
						res=true;
					}
					*/
				} // END OF ACTION CHECKS.
			} else {
				this.Error("No action was specified.",0x0001);
			}
			return res;
		}

		// Returns true if the column ID was populated, false otherwise.
		private bool CheckColumnID() {
			bool res=false;
			if (!(this.cols.Count>0)) {
				if (this.cols.Contains("id") || this.cols.Contains("ID")) {
					int sel=-1;
					if (this.cols.IndexOf("id")!=-1) {
						sel=this.cols.IndexOf("id");
					} else {
						sel=this.cols.IndexOf("ID");
					}
					if (sel>-1) {
						if (this.pairs.ContainsKey(this.cols[sel])) {
							if (!String.IsNullOrEmpty(this.pairs[this.cols[sel]])) {
								if (!String.IsNullOrWhiteSpace(this.pairs[this.cols[sel]])) {
									res=true;
								} else {
									this.Error("The column \""+this.cols[sel]+"\" value is null or consists of only whitespace.",0x00AA);
								}
							} else {
								this.Error("The column \""+this.cols[sel]+"\" value is null or empty.",0x00AA);
							}
						} else {
							this.Error("The column \""+this.cols[sel]+"\" was not prepared properly.",0x0A00);
						}
					} else {
						this.Error("No column was found...",0x00AA);
					}
				} else {
					this.Error("Specified column(s) are invalid or do not specify the record identification.",0x00A0);
				}
			} else {
				this.Error("No columns specified.",0x000A);
			}
			return res;
		}

		// Adds an error message to the output buffer.
		private void Error(string msg=null,uint code=0x0000) {
			string code_out="0x0000";
			try{
				code_out="0x"+String.Format(code.ToString("x"),"{0:x}");
				if (!String.IsNullOrEmpty(msg)) {
					if (!String.IsNullOrWhiteSpace(msg)) {
						Dictionary<string,string> res=new Dictionary<string,string>();
						string source="UNKNOWN";
						try{
							StackTrace trace=new StackTrace();
							source=trace.GetFrame(2).GetMethod().Name;
						}catch{
							if (source!="UNKNOWN") {
								source="UNKNOWN";
							}
						}
						res.Add("timestamp",Database.GetTimestamp());
						res.Add("code",code_out);
						res.Add("msg",msg);
						res.Add("source",source);
						this.error_buffer.Add(res);
						this.ErrorOut(msg);
						//AD.error(msg);
					}
				}
			}catch(Exception e){

			}
		}
		private void ErrorOut(string msg) {
			//msg=console.sanitize(msg);
			console.Error(msg);
			/*
			string time=console.getTime();
			Console.Write("\n["+time+"] Database ERROR:\t");
			Console.ForegroundColor=ConsoleColor.Red;
			Console.Write(msg+"\n");
			Console.ForegroundColor=ConsoleColor.White;
			*/
		}
		// Returns a string representing the current date and time.
		private static string GetTimestamp() {
			string res="";
			DateTime dt=new DateTime(DateTime.Now.Ticks);
			string hour="";
			string minute="";
			string second="";
			string month="";
			string day="";
			string year=dt.Year.ToString();
			string sym="AM";
			if (dt.Month<10) {
				month="0"+dt.Month;
			} else {
				month=dt.Month.ToString();
			}
			if (dt.Day<10) {
				day="0"+dt.Day;
			} else {
				day=dt.Day.ToString();
			}
			if (dt.Hour<10) {
				hour="0"+dt.Hour;
			} else {
				if (dt.Hour>12) {
					int temp=((dt.Hour)-12);
					if (temp<10) {
						hour="0"+temp.ToString();
					} else {
						hour=temp.ToString();
					}
					sym="PM";
				} else {
					hour=dt.Hour.ToString();
				}
			}
			if (dt.Minute<10) {
				minute="0"+dt.Minute;
			} else {
				minute=dt.Minute.ToString();
			}
			if (dt.Second<10) {
				second="0"+dt.Second;
			} else {
				second=dt.Second.ToString();
			}
			res=month+"-"+day+"-"+year+" | "+hour+":"+minute+":"+second+" "+sym;
			return res;
		}
		// Returns what's in the error buffer.
		public string GetError() {
			string res="";
			int i=0;
			while(i<this.error_buffer.Count){
				if (this.error_buffer[i].ContainsKey("code") && this.error_buffer[i].ContainsKey("msg") && this.error_buffer[i].ContainsKey("timestamp") && this.error_buffer[i].ContainsKey("source")) {
					res+="ERROR (Database) ("+this.error_buffer[i]["source"]+") ["+this.error_buffer[i]["code"]+"] ["+this.error_buffer[i]["timestamp"]+"]:\t"+this.error_buffer[i]["msg"]+"\n";
				}
				i++;
			}
			return res;
		}
		// Sets the action/command process to use for this sql query.
		public bool SetAction(uint action=0x0000) {
			bool res=false;
			if (this.CheckAction(action)) {
				this.action=action;
				res=true;
			}
			return res;
		}
		// Returns true if the specified Principal is valid.
		private bool CheckAction(uint action=0x0000) {
			bool res=false;
			if (action!=0x0000) {
				if (this.action_list.Contains(action)) {
					res=true;
				}
			}
			return res;
		}
		// Populates the action list.
		private void IniPopulateActionList() {
			if (!(this.action_list.Count>0)) {
				uint[] list={
					DatabasePrincipal.SelectPrincipal,
					DatabasePrincipal.UpdatePrincipal,
					DatabasePrincipal.DeletePrincipal,
					DatabasePrincipal.InsertPrincipal,
					DatabasePrincipal.CountPrincipal,
					DatabasePrincipal.ExistsPrincipal,
					DatabasePrincipal.GetIDPrincipal,
					DatabasePrincipal.GenerateUniqueIDPrincipal,
					DatabasePrincipal.SearchPrincipal
				};
				int i=0;
				while(i<list.Length){
					this.action_list.Add(list[i]);
					i++;
				}
			}
		}

		// Clears the value of a key/for a column.
		public bool ClearValue(string key=null) {
			bool res=false;
			key=this.parse(key);
			if (!String.IsNullOrEmpty(key)) {
				if (!String.IsNullOrWhiteSpace(key)) {
					if (this.pairs.ContainsKey(key)) {
						this.pairs[key]=null;
						res=true;
					}
				}
			}
			return res;
		}
		// Sets the value of a key/for a column.
		public bool SetValue(string key=null, string value=null) {
			bool res=false;
			key=this.parse(key);
			value=this.parse(value);
			if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value)) {
				if (!String.IsNullOrWhiteSpace(key) && !String.IsNullOrWhiteSpace(value)) {
					if (this.cols.Contains(key)) {
						if (this.pairs.ContainsKey(key)) {
							this.pairs[key]=value;
						} else {
							this.pairs.Add(key, value);
						}
						res=true;
					}
				}
			}
			return res;
		}
		// Adds a column that will be used.
		public bool AddColumn(string column_name=null) {
			bool res=false;
			column_name=this.parse(column_name);
			if (!String.IsNullOrEmpty(column_name)) {
				if (!String.IsNullOrWhiteSpace(column_name)) {
					if (!this.cols.Contains(column_name)) {
						this.cols.Add(column_name);
						if (!this.pairs.ContainsKey(column_name)) {
							this.pairs.Add(column_name,null);
						} else {
							this.pairs[column_name]=null;
						}
						res=true;
					}
				}
			}
			return res;
		}
		// Adds a column with its corresponding value.
		public bool AddColumn(string column_name=null, string value=null) {
			bool res=false;
			if (Database.CheckValue(column_name)) {
				res=this.AddColumn(column_name);
				if (Database.CheckValue(value)&&res) {
					res=this.SetValue(column_name, value);
				} else {
					console.Error("Failed to create column... Column name failed to pass validation...\n\t\tColumn Name:\t\t\""+column_name+"\"");
				}
			}
			return res;
		}
		// Returns true if the parameter is valid.
		private static bool CheckValue(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					if (q.Length>0) {
						res=true;
					}
				}
			}
			return res;
		}
		// Removes a column from the list.
		public bool RemoveColumn(string column_name=null) {
			bool res=false;
			column_name=this.parse(column_name);
			if (!String.IsNullOrEmpty(column_name)) {
				if (!String.IsNullOrWhiteSpace(column_name)) {
					if (this.cols.Contains(column_name)) {
						this.cols.Remove(column_name);
					}
					if (this.pairs.ContainsKey(column_name)) {
						this.pairs.Remove(column_name);
					}
					res=true;
				}
			}
			return res;
		}
		// Opens a connection to the database.
		private bool Open() {
			bool res=false;
			if (this.CheckCon()) {
				if (!this.con_open) {
					try{
						using(this.con=new SqlConnection(this.constr)) {
							try{
								this.con.Open();
								this.con_open=true;
								res=true;
								//AD.warn("A connection was created!");
								console.Warn("A connection was created!");
							}catch(Exception ex){
								this.Error("Failed to open a connection to the database.\n"+ex.Message);
							}
						}
					}catch(Exception e){
						this.Error("Failed to open a connection to the database.\n"+e.Message);
					}
				} else {
					this.Error("A database connection is already open!");
				}
			} else {
				this.Error("Constring check failed!");
			}
			return res;
		}
		// Closes the connection to the database.
		private bool Close() {
			bool res=false;
			if (this.CheckCon()) {
				if (this.con_open) {
					try{
						this.con.Close();
						this.con_open=false;
						res=true;
					}catch(Exception e){

					}
				}
			}
			return res;
		}
		// Returns a sanitized string safe for SQL injection and C# processing.
		private string parse(string q=null) {
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					string exp="[^\\u0020-\\u007e]+";					// Matches all characters that are beyond the scope of the ASCII keyboard characters.
					if (Regex.IsMatch(q,exp)) {
						q=Regex.Replace(q,exp,"");
					}
					exp="[\\u0027\\u005c]+";							// Matches characters that would be able to escape the SQL string.
					if (Regex.IsMatch(q,exp)) {
						q=Regex.Replace(q,exp,"");
					}
				} else {
					q="";
				}
			} else {
				q="";
			}
			return q;
		}
		// Returns true if the constring was set.
		private bool CheckCon() {
			bool res=false;
			if (!String.IsNullOrEmpty(this.constr)) {
				res=true;
			}
			return res;
		}


	}
}