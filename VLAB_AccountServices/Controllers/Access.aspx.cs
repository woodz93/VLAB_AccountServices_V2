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

namespace VLAB_AccountServices.Controllers
{
	public partial class WebForm1:System.Web.UI.Page
	{
		private Dictionary<string,string> FDO=new Dictionary<string, string>();
		private string Command;
		private UserCheck UC;
		private FData Data=new FData();
		protected void Page_Load(object sender,EventArgs e)
		{
			Ini();
		}


		

		private void Ini()
		{
			UC=new UserCheck();
			ProcessInputData();
			if(Request.Form.Count>0)
			{
				Command=GetCommand();
				if(Command.CheckValue())
					ProcAdjustments();
			}
		}

		private void CheckSession()
		{
			string res="";
			OutputElement.Text=res;
		}

		private void Output()
		{
			bool s=true;
			string res="{";
			foreach(var item in Data.Data)
			{
				if(s)
				{
					s=false;
					res+="\""+item.Key+"\":\""+item.Value+"\"";
				}
				else
				{
					res+=",\""+item.Key+"\":\""+item.Value+"\"";
				}
			}
			res+="}";
			OutputElement.Text=res;
		}

		private void ProcAdjustments()
		{
			if(Command.Contains("["))
			{
				string[] list = JsonSerializer.Deserialize<string[]>(Command);
				int i=0;
				while(i<list.Length)
				{
					ProcCmd(list[i].ToLower());
					i++;
				}
			}
		}

		private void ProcCmd(string q)
		{
			FData obj=new FData();
			if(q=="access-groups")
			{
				if(UC.GetCampusName()=="maui college")
				{
					obj.Add(q,"true");
				}
				else
				{
					obj.Add(q,"false");
				}
			}
			else if(q=="check-session")
			{
				if(UC.IsSessionValid())
				{
					obj.Add(q,"true");
				}
				else
				{
					obj.Add(q,"false");
				}
			}
			Data=obj;
		}


		/// <summary>
		/// Processes the POST data submitted to this page.
		/// </summary>
		private void ProcessInputData()
		{
			FDO.Clear();
			if(Request.Form.Count>0)
			{
				int i=0;
				while(i<Request.Form.Count)
				{
					if(!FDO.ContainsKey(Request.Form.GetKey(i)))
						FDO.Add(Request.Form.GetKey(i),Request.Form[i]);
					else
						FDO[Request.Form.GetKey(i)]=Request.Form[i];
					i++;
				}
			}
		}
		/// <summary>
		/// Gets the command specified from the client, which will direct the process flow accordingly.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value representing the command request.</returns>
		private string GetCommand()
		{
			string res=null;
			if(FDO!=null)
				if(FDO.Count>0)
					if(FDO.ContainsKey("cmd"))
						if(FDO["cmd"].CheckValue())
							res=FDO["cmd"];
			return res;
		}

	}
}