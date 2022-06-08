﻿using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices {
	public partial class Default : System.Web.UI.Page
    {
        protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
        protected int cur_count=0;
        public static Label st;
        public static string id="";
        public static byte mode=0x00;
        protected static int ct=0;
        private User obj;
        private string username;
        private int pt=0;
        // Performs checks to see if the 
        protected void Page_Load(object sender, EventArgs e)
        {
            User obj=new User();
            Default.st=status;
            sys.errored=false;
            sys.clear();
            if (CasAuthentication.CurrentPrincipal!=null) {
                ICasPrincipal sp=CasAuthentication.CurrentPrincipal;
                string username=System.Web.HttpContext.Current.User.Identity.Name;
                //sys.warn(username);
                //sys.flush();
                obj.username=username;
                this.obj=obj;
                bool tmp=this.checkUser(username);
                /*
                obj.username=username;
                obj.id=this.genID();
                if (tmp==true) {
                    obj.cmd="set-password";
                } else {
                    obj.cmd="new-user";
                }
                */
                //string data=JsonSerializer.Serialize(obj);
                //string data="{\"cmd\":\""+obj.cmd+"\",\"username\":\""+obj.username+"\"}";
                // REDIRECT TO PASSWORD RESET PAGE (Send json object to determine if an account should be made or just a password reset should be conducted).
                //Session["data"]=data;
                //sys.warn(data);
                //sys.flush();
                /*
                if (sys.errored) {
                    sys.error("System errored out.");
                    status.Text=sys.getBuffer();
                    sys.clear();
                    sys.errored=false;
                } else {
                    sys.clear();
                    status.Text+="<br>REDIRECTING...";
                    Response.Redirect("services/resetPassword.aspx");
                }
                */
            } else {
                /*
                string username="";
                bool tmp=this.checkUser(username);
                obj.username=username;
                obj.id=this.genID();
                if (tmp==true) {
                    obj.cmd="set-password";
                } else {
                    obj.cmd="new-user";
                }
                string data=JsonSerializer.Serialize(obj);
                // REDIRECT TO PASSWORD RESET PAGE (Send json object to determine if an account should be made or just a password reset should be conducted).
                Session["data"]=data;
                if (sys.errored) {
                    status.Text=sys.getBuffer();
                } else {
                    sys.clear();
                    Response.Redirect("services/resetPassword.aspx");
                }
                */
                sys.error("Unauthorized access detected.<br>This has been reported to server administrators.");
                sys.flush();
                sys.clear();
            }
        }

        public async Task<int>Test() {
            await Task.Delay(3000);
            status.Text+="HELLO WORLD";
            return 1;
        }

        protected bool checkUser(string username) {
            bool res=false;
            string id=this.genID();
            string data="{\\\"cmd\\\":\\\"check-user\\\",\\\"username\\\":\\\"" + username + "\\\"}";
            string sql="INSERT INTO " + Default.tb + " (\"id\",\"data\") VALUES ('" + id + "','" + data + "');";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            try{
                using (SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                Default.id=id;
                RegisterAsyncTask(new PageAsyncTask(dbCheck));
            }catch(Exception ex){
                sys.error("Insertion Error:\t"+ex.Message+"\n\n"+sql);
            }
            return res;
        }
		public async Task<int> dbCheck() {
            await this.db_check(Default.id);
            await this.removeRecord(Default.id);
            if (sys.errored) {
                sys.error("System errored out.");
                status.Text=sys.getBuffer();
                sys.clear();
                sys.errored=false;
			} else {
                sys.clear();
                if(this.pt==1) {
					this.obj.cmd="set-password";
				} else if(this.pt==2) {
					this.obj.cmd="new-user";
				}
                string data="{\"cmd\":\""+this.obj.cmd+"\",\"username\":\""+this.obj.username+"\"}";
                Session["data"]=data;
                status.Text+="<br>REDIRECTING...";
                status.Text+="<br><br>"+data+"<br><br>";
                status.Text+="<br><br>"+Session["data"]+"<br><br>";
                Response.Redirect("services/resetPassword.aspx");
            }
            return 1;
        }
        protected async Task<int> removeRecord(string id) {
            string sql="DELETE FROM " + Default.tb + " WHERE id='"+id+"';";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            try{
                using(SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    cmd.ExecuteNonQuery();                      // Deletes the record from the database.
                    con.Close();
                }
            }catch(Exception e){
                sys.error("Failed to delete record with id of \""+id+"\"\n"+e.Message+"\n\n"+sql);
            }
            return 1;
        }
        public async Task<int> db_check(string id) {
            int res=0;
            string sql="SELECT * FROM " + Default.tb + " WHERE id='"+id+"';";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            try{
                using(SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    SqlDataReader r=cmd.ExecuteReader();
                    string tmp="";
                    bool pass=false;
                    int i=0;
                    if (r.HasRows) {
                        while(r.Read()){                                    // Iterates through all records containing the same record id (In the event there are multiple requests which should NOT happen).
                            tmp=r.GetString(1);                             // Gets the record data.
                            if (tmp.IndexOf("status")!=-1) {                // Checks if the record was changed.
                                pass=true;
                                break;
                            }
                            i++;
                        }
                        if (i>1) {                                          // Checks if the number of records that exist is invalid.
                            sys.error("There were multiple records found matching the id \""+id+"\".<br>Please reload the page and try again.");
                            sys.flush();                                    // Pushes the output to the client.
                            sys.clear();                                    // Clears the output.
                            //this.removeRecord(id);                          // Removes all records matching the ID.
                            pass=true;
                        }
                    } else {
                        pass=true;
                    }
                    con.Close();
                    if (!pass) {
                        if (Default.ct<10) {
                            await Task.Delay(1000);
                            sys.warn("No records found.<br>Attempting to check for record update...");
                            sys.flush();
                            await this.db_check(id);
                        } else {
                            sys.error("Request timmed out.<br>Please reload the page and try again.");
                            sys.flush();
                            sys.clear();
                            this.removeRecord(id);                      // Removes the record from the database to clear up space.
                        }
                    } else {
                        sys.warn("FOUND RECORD!");
                        sys.warn(tmp);
                        sys.flush();
                        if (tmp.IndexOf("status\":true")!=-1) {
                            this.pt=1;
                        } else {
                            this.pt=2;
                        }
                    }
                    res=1;
                }
            }catch(Exception e){
                sys.error("An error occurred while asynchronously checking the database...<br><br>"+e.Message);
            }
            return res;
        }
        // Waits for the database record matching the request matches...
        protected bool checkUserResponse(string id) {
            bool res=false;
            string sql="SELECT COUNT(*) AS TOTAL FROM " + Default.tb + " WHERE id='" + id + "';";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            try{
                using (SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    SqlDataReader r=cmd.ExecuteReader();
                    if (r.HasRows) {
                        int co=0;
                        while(r.Read()){
                            co=r.GetInt32(0);
                            break;
                        }
                        if (co>0) {
                            res=true;
                        } else {
                            res=this.proc(id);
                        }
                    } else if (this.cur_count<10) {
                        res=this.proc(id);
                    } else {
                        res=false;
                    }
                    con.Close();
                }
            }catch(Exception ex){
                sys.error("SELECTION ERROR:\t"+ex.Message);
                status.Text="Error occurred while checking for AD user.";
            }
            return res;
        }
        protected bool proc(string id) {
            Thread.Sleep(500);                          // Replace with something less harmfull for processing.
            this.cur_count++;
            bool res=this.checkUserResponse(id);
            return res;
        }
        // ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
        protected string genID() {
            string res="";
            string id=this.genRandID();
            string sql="SELECT COUNT(id) FROM " + Default.tb + " WHERE id='" + id + "';";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            int len=0;
            try{
                using(SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
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