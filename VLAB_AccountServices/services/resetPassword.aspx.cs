using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Data.SqlClient;
using DotNetCasClient;
using DotNetCasClient.Security;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices.services
{

    public partial class resetPassword : System.Web.UI.Page
    {
        
        //protected System.Web.UI.WebControls.TextBox username;
        //protected System.Web.UI.WebControls.TextBox password;
        //protected System.Web.UI.WebControls.TextBox password_confirm;
        //protected System.Web.UI.WebControls.Content;
        //protected System.Web.UI.WebControls.

        protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
        protected static string ending="<br><br>You may contact <a href=\"tel:+18089843283\" target=\"_blank\">(808) 984-3283</a>, email <a href=\"mailto:uhmchelp@hawaii.edu\" target=\"_blank\">uhmchelp@hawaii.edu</a>, or submit a ticket at <a href=\"https://maui.hawaii.edu/helpdesk/#gform_7\" target=\"_blank\">https://maui.hawaii.edu/helpdesk/#gform_7</a> for further assistance.";

        public void processPassword(Object sender, EventArgs e) {
            string u=username.Text;
            string p=password.Text;
            string pc=password_confirm.Text;
            status.Text="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + resetPassword.ending;
            submit_btn.Enabled=false;
            password.Enabled=false;
            password_confirm.Enabled=false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["data"]="{\"cmd\":\"new-user\",\"username\":\"dvalente\",\"id\":\"asdfj8o93y8\"}";
            string data="";
            string user="";
            string pass="";
            string mode="";
            User m_obj=new User();
            if (this.post_isset("data") || CasAuthentication.CurrentPrincipal!=null) {
                ICasPrincipal sp=CasAuthentication.CurrentPrincipal;
                user=System.Web.HttpContext.Current.User.Identity.Name;
                string d="";
                User obj=new User();
                //obj.username="test";
                try{
                    d=Session["data"].ToString();
                    obj=JsonSerializer.Deserialize<User>(d);
                    m_obj=obj;
                }catch{}
                obj.username="dvalente";
                //username.Text="FAILED";
                //username.Text="\""+sys.getCWD()+"\"";
                if (this.post_isset("data")) {
                    if (AD.isset(obj,"username")) {
                        username.Text=obj.username;
                        username.Text="FAILED";
                        username.Enabled=false;
                        user=obj.username;
                        //username.Text="\""+sys.getCWD()+"\"";
                    } else if (CasAuthentication.CurrentPrincipal!=null) {
                        username.Text=user;
                        username.Enabled=false;
                        //username.Text="\""+sys.getCWD()+"\"";
                    } else {
                        username.Text="";
                        status.Text="Failed to get username request.";
                        //username.Text="\""+sys.getCWD()+"\"";
                    }
                } else {
                    username.Text=user;
                    username.Enabled=false;
                }
                //username.Text="\""+sys.getCWD()+"\"";
                if (AD.isset(obj,"cmd")) {
                    if (obj.cmd=="new-user") {
                        submit_btn.Text="Create Account";
                        mode="new-user";
                    } else if (obj.cmd=="set-password") {
                        submit_btn.Text="Reset Password";
                        mode="set-password";
                    } else {
                        submit_btn.Text="[DISABLED]";
                        submit_btn.Enabled=false;
                    }
                }
            } else {
                status.Text="Could not discover parameter data.";
            }
            //username.Text=sys.getCWD();
            if (IsPostBack) {
                //data=Request.Form.GetValues("username")[0];
                //File.WriteAllText(path,data);
                //user=Request.Form.GetValues("username")[0];
                if (AD.isset(m_obj,"username")) {
                    //user=m_obj.username;
                    pass=Request.Form.GetValues("password")[0];
                    if (this.validate(pass)) {
                        data="{\"cmd\":\"" + mode + "\",\"username\":\"" + user + "\",\"password\":\"" + pass + "\"}";
                        this.queryRequest(data);
                        status.Text="Your request has been submitted and is currently being processed.<br>If you are unable to access your VDI account, please contact us via the options provided below...<br><br>" + resetPassword.ending;
                    } else {
                        password.Text=this.sqlParse(pass);
                        password_confirm.Text=this.sqlParse(pass);
                        status.Text="Your password has been modified for validation, please review the changed password and re-submit this form.";
                    }
                } else {
                    user="NULL";
                    string m="[UNKNOWN]";
                    if (mode=="new-user") {
                        m="create a new user account";
                    } else if (mode=="set-password") {
                        m="reset your account password";
                    }
                    CaseLog cl=new CaseLog();
                    cl.code="0x0000";
                    cl.status="fatal";
                    cl.title="Malformed data";
                    cl.msg="Username was not included in the session variables.\nPossible attempt to access page without authorization.";
                    cl.data=JsonSerializer.Serialize(m_obj);
                    string _obj_=JsonSerializer.Serialize(cl);
                    string cref=Case.createCase(_obj_);
                    status.Text="Failed to query your request to/for " + m + ".<br>This issue has been reported to the developer.<br><br>Case reference number: <font class=\"case\">" + cref + "</font>" + resetPassword.ending;
                }
                
            }
        }

        protected bool post_isset(string key) {
            bool res=false;
            if (System.Web.HttpContext.Current.Session[key]!=null) {
                res=true;
            }
            return res;
        }

        protected void queryRequest(string q="") {
            if (q.Length > 0) {
                string id=this.genID();
                q=this.sqlParse(q);
                string values="'"+id+"','"+q+"'";
                string sql="INSERT INTO " + resetPassword.tb + " (\"id\",\"data\") VALUES (" + values + ");";
                string constr=@"Data Source=" + resetPassword.db_ip + ";Initial Catalog=" + resetPassword.db + ";Persist Security Info=True;User ID=" + resetPassword.db_username + ";Password=" + resetPassword.db_password + ";";
                try{
                    using (SqlConnection con=new SqlConnection(constr)) {
                        SqlCommand cmd=new SqlCommand(sql,con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }catch{
                    CaseLog cl=new CaseLog();
                    cl.code="0x0001";
                    cl.status="fatal";
                    cl.title="SQL Submission failed";
                    cl.msg="An error occurred while attempting to process the form submission.\nPerhaps there is a syntax error in the SQL query.\n\nSQL Query:\t\t" + sql + "\n\nConnection string:\t\t" + constr + "\n\nEND OF LINE";
                    cl.data=sql;
                    string _obj_=JsonSerializer.Serialize(cl);
                    string cref=Case.createCase(_obj_);
                    status.Text="An error occurred while attempting to process your request.<br>The issue has been reported to the developer.<br>Your case reference number is <font class=\"case\">" + cref + "</font>" + resetPassword.ending;
                }
            }
        }

        protected bool validate(string q="") {
            bool res=true;
            string rs="[^\\u0020-\\u007e]";
            if (Regex.IsMatch(q,rs)) {
                res=false;
            }
            rs="[\"\'\\/\\\\]";
            if (Regex.IsMatch(q,rs)) {
                res=false;
            }
            return res;
        }

        protected string sqlParse(string q="") {
            string rs="[^\\u0020-\\u007e]";
            if (Regex.IsMatch(q,rs)) {
                q=Regex.Replace(q,rs,"");
            }
            rs="[\"\'\\/\\\\]";
            if (Regex.IsMatch(q,rs)) {
                q=Regex.Replace(q,rs,"");
            }
            return q;
        }

        protected string sqlEncode(string q=""){
            //List <string> invalid_chars=new List<string>();
            /*
            string[] invalid_chars={
                "\"",
                "`",
                "\\"
            };
            int i=0;
            while(i<invalid_chars.Length){
                if (q.Contains(invalid_chars[i])) {
                    q=q.Replace(invalid_chars[i],"");
                }
            }
            */
            //string regexp_str="([^A-z0-9_!@#\\$%\\^&\\*\\(\\)\\-\\{\\}\\[\\]\\.\\,\\`\\~`\n\t:\\?\\<\\>\\|\\/]+|[^\\u200b]+)";
            string regexp_str="[^\\u0020-\\u007e]";
            if (!Regex.IsMatch(q,regexp_str)) {
                q=Regex.Replace(q,regexp_str,"");
            }
            return q;
        }

        // ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
        protected string genID() {
            string res="";
            string id=this.genRandID();
            string sql="SELECT COUNT(id) FROM " + resetPassword.tb + " WHERE id=\"" + id + "\";";
            string constr=@"Data Source=" + resetPassword.db_ip + ";Initial Catalog=" + resetPassword.db + ";Persist Security Info=True;User ID=" + resetPassword.db_username + ";Password=" + resetPassword.db_password + ";";
            int len=0;
            try{
                using(SqlConnection con=new SqlConnection(constr)) {
                    SqlCommand cmd=new SqlCommand(sql,con);
                    con.Open();
                    cmd.ExecuteNonQuery();
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
                res=this.genRandID();
            }
            return res;
        }
        // Generates a randomized length of random characters to compose the record's ID on the database.
        protected string genRandID() {
            Random r=new Random();
            int lim=r.Next(5,255);
            int i=0;
            char c;
            string res="";
            int sel=r.Next(0,100);
            int st=48;
            int en=90;
            while(i<lim){
                sel=r.Next(0,100);
                if (sel<=25) {
                    st=48;
                    en=57;
                } else if (sel>25 && sel<=50) {
                    st=65;
                    en=90;
                } else if (sel>50 && sel<75) {
                    st=97;
                    en=122;
                } else {
                    st=95;
                    en=96;
                }
                c=(char)r.Next(st,en);
                res+=c;
                i++;
            }
            return res;
        }

    }
}