using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VLAB_AccountServices
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //status.Text=Request.Params.GetKey(0);
            int i=0;
            int lim=Request.Params.AllKeys.Length;
            string data="";
            while(i<lim){
                data=data+"<br>"+Request.Params.GetKey(i) + " : " + Request.Params.GetValues(i)[0];
                i++;
            }
            status.Text=data+"<br><br>END OF LINE";
        }
    }
}