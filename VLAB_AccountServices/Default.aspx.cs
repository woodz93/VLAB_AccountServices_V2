using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetCasClient;
using DotNetCasClient.Security;
using System.Data.SqlClient;
using System.Text.Json;
using VLAB_AccountServices.services;

namespace VLAB_AccountServices
{
    public partial class Default : System.Web.UI.Page
    {
        protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
        // Performs checks to see if the 
        protected void Page_Load(object sender, EventArgs e)
        {
            User obj;
            if (CasAuthentication.CurrentPrincipal!=null) {
                ICasPrincipal sp=CasAuthentication.CurrentPrincipal;
                
            } else {
                status.Text="FAILED";
            }
            //string username=System.Web.HttpContext.Current.User.Identity.Name;
                string username="dvalente";
                //status.Text=username;
                //this.checkUser(username);
                string data="{\"username\":\"" + username + "\"}";
                status.Text=data;
                obj = JsonSerializer.Deserialize<User>(data);
                /*
                
                if (AD.userExists(obj)) {
                    status.Text="User was found in the active directory.";
                } else {
                    status.Text="User was not found in the active directory.";
                }
                */
        }
        protected void checkUser(string username) {
            string id=this.genID();
            string data="{\\\"cmd\\\":\\\"check-user\\\",\\\"username\\\":\\\"" + username + "\\\"}";
            string sql="INSERT INTO " + Default.tb + " (id,data) VALUES (\"" + id + "\",\"" + data + "\");";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            try{
                using (SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    SqlDataReader r=cmd.ExecuteReader();
                    con.Close();
                }
            }catch(Exception ex){

            }
        }
        // ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
        protected string genID() {
            string res="";
            string id=this.genRandID();
            string sql="SELECT COUNT(id) FROM " + Default.tb + " WHERE id=\"" + id + "\";";
            string constr=@"Data Source=" + Default.db_ip + ";Initial Catalog=" + Default.db + ";Persist Security Info=True;User ID=" + Default.db_username + ";Password=" + Default.db_password + ";";
            int len=0;
            try{
                using(SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    SqlDataReader dr=cmd.ExecuteReader();
                    if (dr.HasRows) {
                        while(dr.Read()){
                            len=dr.GetInt16(0);
                            break;
                        }
                        if (len>0) {
                            res=this.genID();
                        }
                    }
                    con.Close();

                }
            }catch(Exception ex){

            }
            return res;
        }
        // Generates a randomized length of random characters to compose the record's ID on the database.
        protected string genRandID() {
            Random r=new Random();
            int lim=r.Next(10,255);
            int i=0;
            char c;
            string res="";
            while(i<lim){
                c=(char)r.Next(48,90);
                res+=c;
                i++;
            }
            return res;
        }

    }
}