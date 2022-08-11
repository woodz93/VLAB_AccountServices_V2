using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetCasClient;
using DotNetCasClient.Security;

namespace VLAB_AccountServices
{
	public class UHRole
	{
		private UserCheck UC;
		private ICasPrincipal CAS;
		public readonly string Name;
		public readonly string Code;
		public readonly string Status;
		public readonly string Affiliation;

		public UHRole(UserCheck q) {
			UC=q;
			CAS=q.CAS_Principal;
		}

		private void GetInformation()
		{
			if(Check())
			{

			}
		}


		private void GetName()
		{
			var tmp=UC.GetAttribute("eduPersonAffiliation");
			//if(tmp!=null)
		}


		private bool Check()
		{
			bool res=true;
			if(CAS==null)
				res=false;
			return res;
		}
	}
}