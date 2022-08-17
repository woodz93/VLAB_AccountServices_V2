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
using System.Data.SqlClient;

namespace VLAB_AccountServices
{
	public class AccessController:System.Web.UI.Page
	{
		private UserCheck UC;

		public AccessController()
		{
			UC=new UserCheck();
		}

		public bool GroupAccess()
		{
			bool res=false;

			return res;
		}

	}
}