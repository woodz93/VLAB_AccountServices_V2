using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices.services.assets.classes.Database {
	public class Database {
		private string db="UHMC_VLab";
		private string tb="vlab_pendingusers";
		private string db_ip="172.20.0.142";
		private string db_username="uhmcad_user";
		private string db_password="MauiC0LLegeAD2252!";
		private string constr=null;
		public List<string> cols=new List<string>();
		//private List<string> vals=new List<string>();
		private Dictionary<string,string> pairs=new Dictionary<string,string>();
		private Dictionary<string,string> where_pairs=new Dictionary<string,string>();
		private List<uint> action_list=new List<uint>();

		private uint action=0x0000;
		public List<Dictionary<string,string>> output=new List<Dictionary<string,string>>();
		private List<Dictionary<string,string>> error_buffer=new List<Dictionary<string,string>>();
		public bool response_received=false;
		public string ResponseMessage=null;
		public List<string> ApplicationDebugOutput=new List<string>();
		public Records Results=null;
		// Data...
		public static List<string> ExistingRecords=new List<string>();
		private long LastTime=0;

		//
		// Summary:
		//			Creates an instance of the Database object.
		//
		// Returns:
		//			Nothing.
		public Database() {
			this.constr=@"Data Source=" + this.db_ip + ";Initial Catalog=" + this.db + ";Persist Security Info=True;User ID=" + this.db_username + ";Password=" + this.db_password + ";";
			this.IniPopulateActionList();
		}
		// Sets the database server IP.
		public bool SetServer(string ip=null) {
			bool res=false;
			if (Database.CheckValue(ip)) {
				if (Network.Network.IsConnected()) {
					if (Network.Network.IsReachable(ip)) {
						this.db_ip=ip;
					}
				}
			}
			return res;
		}
		// Sets the database server username.
		public void SetUsername(string username=null) {
			if (Database.CheckValue(username)) {
				this.db_username=username;
			}
		}
		// Sets the database server password.
		public void SetPassword(string password=null) {
			if (Database.CheckValue(password)) {
				this.db_password=password;
			}
		}
		// Sets the database name.
		public void SetDatabase(string db=null) {
			if (Database.CheckValue(db)) {
				this.db=db;
			}
		}
		// Sets the database table name.
		public void SetTable(string tb=null) {
			if (Database.CheckValue(tb)) {
				this.tb=tb;
			}
		}
		// Returns the list of existing records created (IDs).
		public static List<string>GetExistingRecords() {
			return Database.ExistingRecords;
		}
		// Returns true if a record matching a given ID was created, false otherwise.
		public static bool InRecords(string id=null) {
			bool res=false;
			if (Database.CheckValue(id)) {
				if (Database.ExistingRecords.Contains(id)) {
					res=true;
				}
			}
			return res;
		}
		// Asynchronously removes all records that currently exist and were not yet removed from the database...
		public async Task<int> AsyncRemoveAllRecords() {
			int i=0;
			while(i<Database.ExistingRecords.Count){									// Iterates through all of the currently existing records...
				this.AsyncRemoveRecordFromId(3,Database.ExistingRecords[i]);			// Shouldn't need to return anything since it will not be used later on (No await).
				i++;
			}
			return i;
		}
		// Asynchronously attempts to remove the record with a given id from the database for specified duration.
		public async Task<int> AsyncRemoveRecordFromId(int duration=1, string id=null) {
			int res=0;
			bool p=true;
			if (duration>-1 && duration<60) {
				if (!Database.CheckValue(id)) {
					if (!this.CheckColumnID()) {
						p=false;
						console.Error("Column ID is missing...");
					} else {
						id=this.pairs["id"];
					}
				}
				if (p) {
					this.LastTime=Database.GetCurrentTimestamp();
					res=await this.AsyncRecordRemoval(duration,id);
				} else {
					console.Error("Cannot remove record... Checks failed...");
				}
			}
			return res;
		}
		// Returns the timestamp.
		private static long GetCurrentTimestamp() {
			long res=DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
			return res;
		}
		// Returns true if the time differences exceed the given number of seconds.
		private static bool IsOverTime(int seconds=-1,long last_time=-1) {
			bool res=false;
			if (seconds>-1) {
				if (last_time>-1) {
					long cur=Database.GetCurrentTimestamp();
					if ((cur-last_time)>seconds) {
						res=true;
					}
				}
			}
			return res;
		}
		// An asynchronous underlying task to remove the record from the database.
		private async Task<int> AsyncRecordRemoval(int dur,string id) {
			int res=0;
			int len=-1;
			len=await this.AsyncCheckRecordExists(id);
			if (len>0) {
				this.AsyncDeleteRecord(id);
				res=1;
				if (Database.InRecords(id)) {
					Database.ExistingRecords.Remove(id);
				}
			} else {
				if (!Database.IsOverTime(dur,this.LastTime)) {
					await Task.Delay(100);
					res=await this.AsyncRecordRemoval(dur,id);
				}
			}
			return res;
		}
		// Asynchronously removes the record from the database.
		private async Task<int> AsyncDeleteRecord(string id=null) {
			int res=0;
			if (Database.CheckValue(id)) {
				if (this.RemoveRecord(id)) {
					res=1;
				}
				/*
				string sql="DELETE FROM "+this.tb+" WHERE id= @ID ;";
				string sql="DELETE FROM "+this.tb+" WHERE id= @ID ;";
				try{
					using(SqlConnection con = new SqlConnection(sql)) {
						SqlCommand cmd=new SqlCommand(sql, con);
						try{
							cmd.Parameters.AddWithValue("@ID",id);
							try{
								con.Open();
								try{
									res=await cmd.ExecuteNonQueryAsync();
								}catch(Exception e){
									console.Error("Failed to asynchronously remove the record from the database...\n\t\t"+e.Message);
								}
								con.Close();
							}catch(Exception ex){
								console.Error("Failed to asynchronously open a database connection for record removal...\n\t\t"+ex.Message);
							}
						}catch(Exception e){
							console.Error("Failed to initialize a new SqlCommand type...\n\t\t"+e.Message);
						}
					}
				}catch(Exception e){
					console.Error("Failed to delete record asynchronously...\n\t\t"+e.Message);
				}
				*/
			} else {
				console.Error("ID cannot be null for record deletion...\n\t\tID:\t\t\""+id+"\"");
			}
			return res;
		}
		// Performs a check to determine if a database record with the given id exists within the database.
		private async Task<int> AsyncCheckRecordExists(string id) {
			int res=0;
			string sql="SELECT COUNT(*) AS TOTAL FROM "+this.tb+" WHERE id= @ID ;";
			try{
				using(SqlConnection con=new SqlConnection(this.constr)) {
					try{
						SqlCommand cmd=new SqlCommand(sql,con);
						cmd.Parameters.AddWithValue("@ID",id);
						try{
							con.Open();
							try{
								//res=await cmd.ExecuteNonQueryAsync();
								SqlDataReader dr=await cmd.ExecuteReaderAsync();
								if (dr.HasRows) {
									while(dr.Read()){
										res=dr.GetInt32(0);
										break;
									}
								}
							}catch(Exception e){
								console.Error("Failed to execute SQL query...\n\t\t"+e.Message);
							}
							con.Close();
						}catch(Exception e){
							console.Error("Failed to open a connection to the database...\n\t\t"+e.Message);
						}
					}catch(Exception ex){
						console.Error("Failed to complete variable sanitation...\n\t\t"+ex.Message);
					}
				}
			}catch(Exception e){
				console.Error("Failed to initialize a database connection...\n\t\t"+e.Message);
			}
			return res;
		}
		// Returns the value of a specified key.
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
		// Returns a string consisting of id-value pairs (PRE-SANITIZED).
		public string GenerateSQL(string cmd="normal", string condition_override=",") {
			string res=null;
			//string condition_override=",";
			condition_override=this.parse(condition_override.ToUpper());
			/*
			if (cmd=="and") {
				//condition_override=this.parse(",");
				condition_override="AND";
			} else if (cmd=="or") {
				condition_override="OR";
			} else if (cmd=="like") {
				condition_override="LIKE";
			}
			*/
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
		// Returns a string consisting of the column names.
		private string GetCols() {
			string res="";
			if (this.cols.Count>0) {
				int i=0;
				while(i<this.cols.Count){
					if (i>0) {
						res+=" ,\""+this.cols[i]+"\"";
					} else {
						res+=" \""+this.cols[i]+"\"";
					}
					i++;
				}
			}
			return res;
		}
		// Returns a string consisting of the column values.
		private string GetValues() {
			string res="";
			if (this.cols.Count>0) {
				int i=0;
				while(i<this.pairs.Count){
					if (i>0) {
						res+=", @"+this.cols[i].ToUpper()+" ";
					} else {
						res+=" @"+this.cols[i].ToUpper()+" ";
					}
					i++;
				}
			}
			return res;
		}
		// Returns a string consisting of the key value pairs formatted for update sql command.
		private string PrepUpdate() {
			string res="";
			if (this.cols.Count>0) {
				int i=0;
				res="";
				while(i<this.cols.Count){
					if (i>0) {
						res+=", "+this.cols[i]+"= @"+this.cols[i].ToUpper()+" ";
					} else {
						res+=""+this.cols[i]+"= @"+this.cols[i].ToUpper()+" ";
					}
					i++;
				}
			}
			return res;
		}
		// Returns a string consisting of the where clause.
		private string GetWhere(string cond="and") {
			string res="";
			if (this.where_pairs.Count>0) {
				int i=0;
				List<string> keys=new List<string>();
				keys.AddRange(this.where_pairs.Keys);
				string ovr="= ";
				if (cond.Contains("like-")) {
					ovr="LIKE ";
					cond=cond.Replace("like-","");
				}
				string col="";
				while(i<this.where_pairs.Count){
					col=keys[i].Replace("WHERE_","");
					if (i>0) {
						res+=""+cond+" "+col+ovr+"@"+keys[i].ToUpper()+" ";
					} else {
						res+=" "+col+ovr+"@"+keys[i].ToUpper()+" ";
					}
					i++;
				}
				res=" WHERE"+res;
			}
			return res;
		}
		// Adds a where condition.
		public void AddWhere(string col=null,string val=null) {
			if (Database.CheckValue(col) && Database.CheckValue(val)) {
				col="WHERE_"+this.parse(col);
				val=this.parse(val);
				if (!this.where_pairs.ContainsKey(col)) {
					this.where_pairs.Add(col,val);
				} else {
					this.where_pairs[col]=val;
				}
			}
		}
		// Clears the where conditions.
		public void ClearWhere() {
			if (this.where_pairs.Count>0) {
				this.where_pairs.Clear();
			}
		}
		// Clears all pairs and columns.
		public void Clear() {
			this.pairs.Clear();
			this.ClearWhere();
			this.cols.Clear();
			this.action=0x0000;
			if (this.output!=null) {
				this.output.Clear();
			}
			if (this.Results!=null) {
				this.Results.Clear();
			}
		}
		// Returns true if the record matching the specified id is found.
		public bool RecordExists(string id=null) {
			bool res=false;
			bool pass=false;
			string sql="";
			if (Database.CheckValue(id)) {
				if (!(this.pairs.ContainsKey("id")) || !(this.cols.Contains("id"))) {
					this.AddColumn("id",id);
				}
			}
			if (this.cols.Count>0) {
				string p=this.GeneratePairs();
				sql="SELECT COUNT(*) AS TOTAL FROM "+this.tb+" WHERE "+p+" ;";
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
										console.Error("Failed to process SQL query... The database connection will be closed...\n"+e.Message+"\n\n"+sql);
									}
									con.Close();
								}catch(Exception e){
									console.Error("Failed to open database connection...\n\t\t"+e.Message);
								}
							}catch(Exception e){
								console.Error("Failed to sanitize the id for SQL processing...\n"+e.Message);
							}
						}catch(Exception e){
							console.Error("Failed to process sql query...\n"+e.Message);
						}
					}
				}catch(Exception e){
					console.Error("Failed to establish a connection to the database...\n"+e.Message);
				}
			}
			return res;
		}
		// Processes the query. Any and all output can be retrieved with the "GetOutput" method.
		public bool Send() {
			bool res=false;
			if (this.CheckQuery()) {
				if (!(this.error_buffer.Count>0)) {
					if (this.action==DatabasePrincipal.InsertPrincipal) {
						res=this.InsertRecord();
					} else if (this.action==DatabasePrincipal.SelectPrincipal) {
						res=this.SelectRecord();
					} else if (this.action==DatabasePrincipal.UpdatePrincipal) {
						res=this.UpdateRecord();
						/*
						if (this.CheckColumnID()) {
							console.Error(this.pairs["id"]);
						}
						*/
					} else if (this.action==DatabasePrincipal.ExistsPrincipal) {
						res=this.RecordExists(this.pairs["id"]);
					} else if (this.action==DatabasePrincipal.RemoveRecordPrincipal) {
						res=this.RemoveRecord(this.pairs["id"]);
					// Add more query conditions here...
					} else {
						console.Error("The provided database action to conduct is invalid, does not exist, or is missing required columns to be specified.");
					}
				} else {
					console.Error("An error is currently present that would prevent further database execution.");
				}
			} else {
				console.Error("Provided query is invalid.");
			}
			return res;
		}
		// Removes a record that matches the ID.
		public bool RemoveRecord(string id=null) {
			bool res=false;
			if (Database.CheckValue(id)) {
				if (this.RecordExists(id)) {
					string sql="DELETE FROM "+this.tb+" WHERE id= @ID ;";
					int len=0;
					try{
						using(var con=new SqlConnection(this.constr)){
							try{
								SqlCommand cmd=new SqlCommand(sql, con);
								try{
									cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
									try{
										con.Open();
										try{
											len=cmd.ExecuteNonQuery();
										}catch(Exception ex){
											console.Error("Failed to process SQL query...\n\t\t"+ex.Message);
										}
										con.Close();
									}catch(Exception ex){
										console.Error("Failed to open a connection to the database.\n\t\t"+ex.Message);
									}
								}catch(Exception ex){
									console.Error("Failed to prepare and sanitize input values.\n\t\t"+ex.Message);
								}
							}catch(Exception e){
								console.Error("Failed to prepare SQL command.\n\t\t"+e.Message);
							}
						}
					}catch(Exception e){
						console.Error("Failed to establish a connection to the database.\n\t\t"+e.Message);
					}
					if (len>0) {
						res=true;
						if (id==null) {
							if (this.CheckColumnID()) {
								if (this.pairs.ContainsKey("id")) {
									id=this.pairs["id"];
								}
							}
						}
						if (id!=null) {
							if (Database.ExistingRecords.Contains(id)) {
								Database.ExistingRecords.Remove(id);
							}
						}
					}
				} else {
					console.Warn("Record does not exist.");
				}
			} else {
				console.Error("The ID provided is invalid and cannot be used!");
			}
			return res;
		}
		// Waits until the records is updated as a response.
		public int ResponseWait() {
			int res=this.CheckResponse();
			int max=10000;
			int i=0;
			while(res==0 && i<max){
				res=this.CheckResponse();
				Thread.Sleep(10);
			}
			return res;
		}
		// Checks if the record is a response or a query/request.
		public int CheckResponse() {
			int res=0;
			string sql="SELECT * FROM "+this.tb+" WHERE id= @ID ;";
			Dictionary<string,string> tmp=new Dictionary<string, string>();
			using(SqlConnection con=new SqlConnection(this.constr)) {
				SqlCommand cmd=new SqlCommand(sql,con);
				cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
				con.Open();
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
				con.Close();
			}
			return res;
		}
		// Returns true if one or more records were updated, false otherwise.
		private bool UpdateRecord() {
			bool res=false;
			try{
				int len=0;
				//string sql="UPDATE "+this.tb+" SET data= @DATA WHERE id= @ID ;";
				string sql="UPDATE "+this.tb+" SET "+this.PrepUpdate()+this.GetWhere()+";";
				//console.Warn(sql);
				using(SqlConnection con=new SqlConnection(this.constr)) {
					SqlCommand cmd=new SqlCommand(sql,con);
					//cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
					//cmd.Parameters.AddWithValue("@DATA",this.pairs["data"]);
					int i=0;
					List<string> keys=new List<string>();
					keys.AddRange(this.where_pairs.Keys);
					while(i<this.where_pairs.Count){
						cmd.Parameters.AddWithValue("@"+keys[i].ToUpper(),this.where_pairs[keys[i]]);
						i++;
					}
					keys.Clear();
					keys.AddRange(this.pairs.Keys);
					i=0;
					while(i<this.pairs.Count){
						cmd.Parameters.AddWithValue("@"+keys[i].ToUpper(),this.pairs[keys[i]]);
						i++;
					}
					con.Open();
					len=cmd.ExecuteNonQuery();
					con.Close();
				}
				if (len>0) {
					res=true;
				}
			}catch(Exception e){
				console.Error("Failed to update record...\n"+e.Message);
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
					try{
						cmd.ExecuteNonQuery();
					}catch(Exception ex){
						console.Error("Failed to invoke AD application...\n\t\t"+ex.Message);
					}
					con.Close();																// Closes the database connection.
				}
				console.Success("Application successfully invoked.");
			}catch(Exception e){
				console.Error("Failed to invoke application...\n\t\t"+e.Message);
			}
		}
		// Clears the application debug output buffer.
		public void ClearAppDebug() {
			if (this.ApplicationDebugOutput.Count>0) {
				this.ApplicationDebugOutput.Clear();
			}
		}
		// Selects a record.
		private bool SelectRecord() {
			bool res=false;
			//string sql="SELECT * FROM "+this.tb+" WHERE id= @ID ;";
			string sql="SELECT * FROM "+this.tb+this.GetWhere()+";";
			//console.Log(sql);
			
			try{
				using(var con=new SqlConnection(this.constr)){
					SqlCommand cmd=new SqlCommand(sql,con);
					try{
						//cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
						int i=0;
						List<string> keys0=new List<string>();
						keys0.AddRange(this.where_pairs.Keys);
						string col="";
						while(i<this.where_pairs.Count){
							col=keys0[i].Replace("WHERE_","");
							cmd.Parameters.AddWithValue("@"+keys0[i].ToUpper(),this.where_pairs[keys0[i]]);
							i++;
						}
						keys0.Clear();
						//console.Log(cmd.ToString());
						try{
							con.Open();
							try{
								SqlDataReader dr=cmd.ExecuteReader();
								if (dr.HasRows) {
									if (this.pairs.Count>0) {
										i=0;
										List<string> keys=new List<string>();
										int lim=0;
										/*
										while(dr.Read()){
											lim=dr.FieldCount;
											while(i<lim){
												keys.Add(dr.GetName(i));
												i++;
											}
											break;
										}
										i=0;
										*/
										//keys.AddRange(this.pairs.Keys);
										//var columns=dr.GetSchemaTable().Columns;
										//var tmp0=dr["id"];
										//console.Log(tmp0.ToString());
										//this.output.Clear();
										List<Dictionary<string,string>>tmp0=new List<Dictionary<string,string>>();
										while(dr.Read()){
											Dictionary<string,string> tmp=new Dictionary<string, string>();
											i=0;
											while(i<this.cols.Count){
												try{
													tmp.Add(this.cols[i],dr[this.cols[i]].ToString());
												}catch{}
												i++;
											}
											//this.output.Add(tmp);
											tmp0.Add(tmp);
										}
										this.output=tmp0;
										//console.Log(tmp0.ToString());
										//console.Log(this.output.ToString());
										//bool cp=true;
										//while(dr.Read()){
										//	tmp.Clear();
										//	i=0;
										//	lim=dr.FieldCount;
										//	if (cp) {
										//		while(i<lim){
										//			keys.Add(dr.GetName(i));
										//			i++;
										//		}
										//		cp=false;
										//	}
										//	i=0;
										//	while(i<lim){
										//		tmp[keys[i]]=dr.GetString(i);
										//		i++;
										//	}
										//	this.output.Add(tmp);
										//}
										//console.Log(this.output.ToString());
									} else {
										//console.Info(dr.ToString());
										while(dr.Read()){
											Dictionary<string,string> tmp=new Dictionary<string, string>();
											//tmp["id"]=dr.GetString(0);
											//tmp["data"]=dr.GetString(1);
											tmp["id"]=dr["id"].ToString();
											tmp["data"]=dr["data"].ToString();
											this.output.Add(tmp);
										}
									}
								}
								con.Close();
								this.Results=new Records(this.output);
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
				//string sql="INSERT INTO "+this.tb+" (\"id\",\"data\") VALUES('"+id+"', @DATA );";
				string sql="INSERT INTO "+this.tb+" ("+this.GetCols()+") VALUES("+this.GetValues()+");";
				int len=0;
				try{
					using(var con=new SqlConnection(this.constr)){
						SqlCommand cmd=new SqlCommand(sql,con);
						//cmd.Parameters.AddWithValue("@ID",this.pairs["id"]);
						//cmd.Parameters.AddWithValue("@DATA",this.pairs["data"]);

						int i=0;
						List<string> keys=new List<string>();
						/*
						keys.AddRange(this.where_pairs.Keys);
						while(i<this.where_pairs.Count){
							cmd.Parameters.AddWithValue("@"+keys[i],this.where_pairs[keys[i]]);
							i++;
						}
						keys.Clear();
						*/
						keys.AddRange(this.pairs.Keys);
						//i=0;
						while(i<this.pairs.Count){
							cmd.Parameters.AddWithValue("@"+keys[i].ToUpper(),this.pairs[keys[i]]);
							i++;
						}
						con.Open();
						len=cmd.ExecuteNonQuery();
						con.Close();
						//console.Warn(sql);
					}
				}catch(Exception e){
					console.Error("Failed to insert record into database...\n\t\t"+e.Message);
				}
				if (len>0) {
					res=true;
					if (id!=null) {
						if (!Database.ExistingRecords.Contains(id)) {
							Database.ExistingRecords.Add(id);
						}
					}
				}
			}
			return res;
		}
		// Returns a unique ID string.
		public string GetUniqueID() {
			string res=null;
			if (!this.pairs.ContainsKey("id")) {
				this.AddColumn("id",Database.GenerateRandomString());
			}
			string str=this.pairs["id"];
			string sql="SELECT COUNT(*) AS TOTAL FROM "+this.tb+" WHERE id= @ID ;";
			int i=0;
			int len=0;
			try{
				using(var con=new SqlConnection(this.constr)){
					SqlCommand cmd=new SqlCommand(sql,con);
					cmd.Parameters.AddWithValue("@ID",str);
					con.Open();
					SqlDataReader dr=cmd.ExecuteReader();
					if (dr.HasRows) {
						while(dr.Read()){
							len=dr.GetInt32(0);
							break;
						}
					}
					con.Close();
					if (len>0) {
						this.pairs["id"]=Database.GenerateRandomString();
						res=this.GetUniqueID();
					} else {
						res=str;
					}
				}
			}catch(Exception e){
				console.Error("Failed to check if ID exists in database.\n\t\t"+e.Message);
			}
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
		// Outputs columns.
		public void ShowColumns() {
			int i=0;
			int lim=this.cols.Count;
			console.Success("START OF COLUMNS");
			while(i<lim){
				console.Warn(this.cols[i]);
				i++;
			}
			console.Success("END OF COLUMNS");
			console.Success("START OF COLUMN VALUES");
			i=0;
			foreach(var item in this.pairs) {
				console.Warn("\""+item.Key+"\": \""+item.Value+"\"");
			}
			console.Success("END OF COLUMN VALUES");
		}
		// Returns true if everything is needed in order to process the query, false otherwise.
		private bool CheckQuery() {
			bool res=false;
			if (this.action!=DatabasePrincipal.NullPrincipal) {
				if (this.action==DatabasePrincipal.SelectPrincipal) {
					res=true;
					/*
					if (this.cols.Count>0) {
						res=true;
					} else {
						console.Error("No columns specified while attempting to select records.",0x000A);
						console.Warn("Number of columns present: \""+this.cols.Count+"\"");
						this.ShowColumns();
					}
					*/
				} else if (this.action==DatabasePrincipal.InsertPrincipal) {
					if (this.cols.Count>0) {
						if (this.CheckColumnID()) {
							res=true;
						}
					} else {
						console.Error("No columns specified while attempting to insert new record.");
						console.Warn("Number of columns present: \""+this.cols.Count+"\"");
						this.ShowColumns();
					}
				} else if (this.action==DatabasePrincipal.ExistsPrincipal) {
					if (this.cols.Count>0) {
						res=true;
					} else {
						console.Error("No columns specified while attempting to check record existence.");
						console.Warn("Number of columns present: \""+this.cols.Count+"\"");
						this.ShowColumns();
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
				} else if (this.action==DatabasePrincipal.RemoveRecordPrincipal) {
					if (this.CheckColumnID()) {
						res=true;
					}
				} // END OF ACTION CHECKS.
			} else {
				console.Error("No action was specified.");
			}
			return res;
		}
		// Returns true if the column ID was populated, false otherwise.
		private bool CheckColumnID() {
			bool res=false;
			if (this.cols.Count>0) {
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
									console.Error("The column \""+this.cols[sel]+"\" value is null or consists of only whitespace.");
								}
							} else {
								console.Error("The column \""+this.cols[sel]+"\" value is null or empty.");
							}
						} else {
							console.Error("The column \""+this.cols[sel]+"\" was not prepared properly.");
						}
					} else {
						console.Error("No column was found...");
					}
				} else {
					console.Error("Specified column(s) are invalid or do not specify the record identification.");
				}
			} else {
				console.Error("No columns specified.");
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
						try{
							if (!this.cols.Contains(column_name)) {
								this.cols.Add(column_name);
							}
							if (!this.pairs.ContainsKey(column_name)) {
								this.pairs.Add(column_name,null);
							} else {
								this.pairs[column_name]=null;
							}
						}catch(Exception ex){
							console.Warn("Failed to add column...\n\t\t"+ex.Message);
						}
						res=true;
					} else {
						//console.Error("Column name already exists.\n\t\tColumn Name:\t\t\""+column_name+"\"");
					}
				} else {
					console.Error("Column name is invalid.\n\t\tColumn Name:\t\t\""+column_name+"\"");
				}
			} else {
				console.Error("Column name is invalid.\n\t\tColumn Name:\t\t\""+column_name+"\"");
			}
			return res;
		}
		// Adds a column with its corresponding value.
		public bool AddColumn(string column_name=null, string value=null) {
			bool res=false;
			if (Database.CheckValue(column_name)) {
				try{
					res=this.AddColumn(column_name);
				}catch(Exception e){
					console.Warn("Failed to add column from double param...\n\t\t"+e.Message);
				}
				if (Database.CheckValue(value)&&res) {
					try{
						res=this.SetValue(column_name, value);
					}catch(Exception e){
						console.Warn("Failed to set column value...\n\t\t"+e.Message);
					}
					//console.Success("Column added...");
				} else {
					//console.Error("Failed to create column... Column name failed to pass validation...\n\t\tColumn Name:\t\t\""+column_name+"\"");
				}
			} else {
				console.Error("Specified column name is invalid.\n\t\tColumn Name:\t\t\""+column_name+"\"");
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
		


	}
}