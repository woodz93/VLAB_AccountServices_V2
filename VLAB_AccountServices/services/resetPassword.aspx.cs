using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
namespace VLAB_AccountServices.services
{

    public partial class resetPassword : System.Web.UI.Page
    {
        
        //protected System.Web.UI.WebControls.TextBox username;
        //protected System.Web.UI.WebControls.TextBox password;
        //protected System.Web.UI.WebControls.TextBox password_confirm;
        //protected System.Web.UI.WebControls.Content;
        //protected System.Web.UI.WebControls.

        public void processPassword(Object sender, EventArgs e) {
            string u=username.Text;
            string p=password.Text;
            string pc=password_confirm.Text;
            status.Text=u;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string path="C:/Users/sitesupport/Desktop/debugging/debug.txt";
            string data="";
            string username="";
            string password="";
            if (IsPostBack) {
                //data=Request.Form.GetValues("username")[0];
                //File.WriteAllText(path,data);
                username=Request.Form.GetValues("username")[0];
                password=Request.Form.GetValues("password")[0];
                data="{\"cmd\":\"set-password\",\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
                File.WriteAllText(path,data);
                this.queryRequest(data);
            }
        }

        protected void queryRequest(string q="") {
            if (q.Length > 0) {
                string db="UHMC_VLab";
		        string tb="vlab_pendingusers";
		        string db_ip="172.20.0.142";
		        string db_port="";
		        string db_username="uhmcad_user";
		        string db_password="MauiC0LLegeAD2252!";
                string id=this.genID();
                string values=this.sqlEncode(id + "," + q);
                string sql="INSERT INTO " + tb + " (\"id\",\"data\") VALUES (" + values + ");";
            }
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

        protected string genID() {
            string res="";

            return res;
        }

    }
}