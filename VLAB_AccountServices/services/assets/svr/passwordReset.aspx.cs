using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.WebRequestMethods;

namespace VLAB_AccountServices.services
{
    public partial class resetPassword : System.Web.UI.Page
    {
        protected void Main()
        {
            Console.WriteLine(Request.Form);
        }
    }
}