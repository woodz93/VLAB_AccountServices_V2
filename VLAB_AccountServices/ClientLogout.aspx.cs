using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VLAB_AccountServices.Controllers.assets.data;
using VLAB_AccountServices.services.assets.sys;
using VLAB_AccountServices.services.assets.classes.UserCheck;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace VLAB_AccountServices
{
	public partial class WebForm1:System.Web.UI.Page
	{
		private UserCheck UC;
		protected void Page_Load(object sender,EventArgs e)
		{
			if(UserCheck.Active)
			{
				UC=new UserCheck();
				UC.Logout();
			}
			else
			{
				Response.Redirect("https://vlab.accountservices.maui.hawaii.edu");
			}
		}
	}
}