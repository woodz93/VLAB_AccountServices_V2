using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices
{
	public class UserCheck:System.Web.UI.Page
	{
		private string ID = null;
		private bool _IsChecked = false;
		public ICasPrincipal CAS_Principal = null;
		public bool Ready = false;
		private FormsAuthenticationTicket AT;
		private string ServiceTicketName;
		private static bool _Active=false;
		private static bool _Exists=false;
		public static bool Active { get { return _Active; } }
		public UserCheck()
		{
			Ini();
			Ready=true;
			if(!UserCheck._Active)
				UserCheck._Active=true;
		}
		/// <summary>
		/// Performs a database cleanup.
		/// </summary>
		private void CleanUp()
		{
			Database ins = new Database();
			ins.AsyncRemoveAllRecords();
		}
		// Returns true if the client checks are good to go, false otherwise.
		public bool IsChecked()
		{
			return _IsChecked;
		}
		// Performs a username check with the AD server.
		private void Ini()
		{
			CAS_Principal=CasAuthentication.CurrentPrincipal;
			if(!CheckSession("data"))
			{
				if(CheckCas())
				{
					string un = GetUsername();
					ServiceTicketName="AccountServices_Cas_Service_Ticket_"+GetUsername();
					AT=CasAuthentication.CreateFormsAuthenticationTicket(un,"/Auth/"+un,ServiceTicketName,DateTime.Now,DateTime.Now.AddHours(1));
					if(!String.IsNullOrEmpty(un))
					{
						var c = CheckUsername(un);
						string cmd = null;
						if(c==0x01)
						{
							cmd="set-password";
							_Exists=true;
						}
						else if(c==0x10)
							cmd="new-user";
						if(!String.IsNullOrEmpty(cmd))
						{
							//console.Log(c.ToString());
							string json = "{\"id\":\""+ID+"\",\"cmd\":\""+cmd+"\",\"username\":\""+un+"\"}";
							Session.Clear();
							SetSession("data",json);
							_IsChecked=true;
						}
					}
				}
			}
			else
				_IsChecked=true;
		}

		public bool Exists()
		{
			return _Exists;
		}

		public void Logout()
		{
			UserCheck._Active=false;
			Session.Clear();
			//if(CasAuthentication.ServiceTicketManager.ContainsTicket(ServiceTicketName))
			//{
			//	CasAuthentication.ServiceTicketManager.RevokeTicket(ServiceTicketName);
			//	AT=null;
			//}
			//Session.Abandon();
			//CAS_Principal
			//Response.Redirect("https://vlab.accountservices.maui.hawaii.edu");
			CasAuthentication.SingleSignOut();
			//CasAuthentication.ClearAuthCookie();
			//Response.Redirect("https://cas-test.its.hawaii.edu/cas/logout");
			//Response.Redirect("https://authn.hawaii.edu/cas/logout?https://vlab.accountservices.maui.hawaii.edu");
			//Session.Abandon();
		}
		/// <summary>
		/// Checks if the current CAS session is still valid.
		/// </summary>
		/// <returns>A <see cref="bool">boolean</see> value representing the validity of the current CAS session.</returns>
		public bool IsSessionValid()
		{
			bool res=false;
			if(AT!=null)
				if(!AT.Expired)
					if(CasAuthentication.ServiceTicketManager.ContainsTicket(ServiceTicketName))
						if(CasAuthentication.ServiceTicketManager.GetTicket(ServiceTicketName)!=null)
							res=true;
			return res;
		}
						
		// Sets the session variable.
		public void SetSession(string key = null,string value = null)
		{
			if(!String.IsNullOrEmpty(key))
				if(!String.IsNullOrEmpty(value))
					try
					{
						Session.Add(key,value);
					}
					catch(Exception e)
					{
						Session[key]=value;
					}
		}


		private static string GetAttribute1(ICasPrincipal sessionPrincipal, string key)
        {
            string value = "";
            if (sessionPrincipal != null)
            {
                IAssertion sessionAssertion = sessionPrincipal.Assertion;
                if (sessionAssertion != null)
                {
                    if (sessionAssertion.Attributes.ContainsKey(key))
                    {
                        string[] array = sessionAssertion.Attributes[key].ToArray();
                        if (array.Count() > 0)
                        {
                            value = array[0];
                        }
                    }
                }
            }
            return value;
        }

        private static List<string> GetAttributeList1(ICasPrincipal sessionPrincipal, string key)
        {
			List<string> list = null;
			if (sessionPrincipal != null)
		    {
				IAssertion sessionAssertion = sessionPrincipal.Assertion;
		        if (sessionAssertion != null)
				{
					if (sessionAssertion.Attributes.ContainsKey(key))
				    {
						list = sessionAssertion.Attributes[key].ToList();
					}
			    }
		    }
			return list;
	    }



		/// <summary>
		/// Gets the current user's first name.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetFirstName()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("givenName");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's last name.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetLastName()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("sn");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's full name.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetFullName()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("cn");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's email address.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetEmail()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("uhEmail");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the department name of the current user.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetDepartment()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("ou");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's telephone number.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetPhone()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("telephoneNumber");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's job title.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetJobTitle()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("title");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's affiliations.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public List<string> GetAffiliations()
		{
			List<string> res = null;
			if(CheckCas())
			{
				var tmp = GetAttribute("eduPersonAffiliation");
				if(tmp!=null)
					res=tmp;
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's organization.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public List<string> GetOrganizations()
		{
			List<string> res = null;
			if(CheckCas())
			{
				var tmp = GetAttribute("eduPersonOrgDN");
				if(tmp!=null)
					res=tmp;
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's UH ID number.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetUHID()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("uhUuid");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's office location.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetOffice()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("physicalDeliveryOfficeName");
				if(tmp!=null)
					res=tmp[0].ToLower();
			}
			return res;
		}
		/// <summary>
		/// Gets the user's FAX number.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetFax()
		{
			string res=string.Empty;
			if(CheckCas())
			{
				var tmp=GetAttribute("facsimileTelephoneNumber");
				if(tmp!=null)
					res=tmp.Count>0 ? tmp[0] : string.Empty;
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's campus affiliation.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetAffiliation()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp=GetAttribute("eduPersonAffiliation");
				if(tmp!=null)
					res=tmp.Count>0 ? tmp[0] : string.Empty;
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's campus affiliation.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetRole()
		{
			return GetAffiliation();
		}
		/// <summary>
		/// Gets the current user's personal webpage/website URL.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetURL()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("labeledURI");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the current user's display name.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetDisplayName()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp = GetAttribute("displayName");
				if(tmp!=null)
					res=tmp[0];
			}
			return res;
		}
		/// <summary>
		/// Gets the campus abbreviation that the user belongs to.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value representing the campus abbreviation.</returns>
		public string GetCampus()
		{
			string res = string.Empty;
			if(CheckCas())
			{
				var tmp0 = GetAttribute("uhScopedHomeOrg");
				if(tmp0!=null)
				{
					var tmp = tmp0.Count>0 ? tmp0[0] : string.Empty;
					if(tmp.Contains("org="))
					{
						int st = tmp.IndexOf("org=");
						res=tmp.Substring(st+4,(tmp.Length-(st+4)));
					}
				}
			}
			return res;
		}
		/// <summary>
		/// Gets the user's departmental email address.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value.</returns>
		public string GetMail()
		{
			string res=string.Empty;
			if(CheckCas())
			{
				var tmp=GetAttribute("mail");
				if(tmp!=null)
					res=tmp.Count>0 ? tmp[0] : string.Empty;
			}
			return res;
		}



		public string GetCampusName()
		{
			string res = string.Empty;
			string tmp=GetCampus();
			if(tmp!=null)
				res=CampusData.GetCampusName(tmp)??string.Empty;
			return res;
		}
		/// <summary>
		/// Gets the attribute located within the CAS principal, null is returned if the property key was not found.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <see cref="List{string}">List</see> containing all of the records stored within the specified attribute/property within the CAS.</returns>
		public List<string> GetAttribute(string key = null)
		{
			List<string> value = null;
			try
			{
				if(UserCheck.CheckValue(key))
					if(CAS_Principal!=null)
					{
						IAssertion sessionAssertion = CAS_Principal.Assertion;
						if(sessionAssertion!=null)
							if(sessionAssertion.Attributes.ContainsKey(key))
							{
								var array = sessionAssertion.Attributes[key].ToList<string>();
								if(array.Count()>0)
									value=array;
								else
									ConsoleOutput.Warn("Array does not contain anything...");
							}
							else
								ConsoleOutput.Warn("Session does not contain the key\n\t\t"+key);
						else
							ConsoleOutput.Warn("Session assertion is null");
					}
					else
						value=GetAttribute(key);
				else
					ConsoleOutput.Warn("Key failed validation");
			}
			catch(Exception e)
			{
				ConsoleOutput.Error("Failed to get user information from CAS principal...\n\t\t"+e.Message);
			}
			return value;
		}
		/// <summary>
		/// Checks if the CAS contains the requested attribute/property.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <see cref="bool">boolean</see> value determining if the CAS contains the attribute/property name or not.</returns>
		private bool HasAttribute(string key = null)
		{
			bool res = false;
			try
			{
				if(UserCheck.CheckValue(key))
					if(CAS_Principal!=null)
					{
						IAssertion sessionAssertion = CAS_Principal.Assertion;
						if(sessionAssertion!=null)
							if(sessionAssertion.Attributes.ContainsKey(key))
							{
								string[] array = sessionAssertion.Attributes[key].ToArray();
								if(array.Count()>0)
									res=true;
							}
					}
			}
			catch(Exception e)
			{
				ConsoleOutput.Error("Failed to get user information from CAS principal...\n\t\t"+e.Message);
			}
			return res;
		}
		/// <summary>
		/// Checks if the CAS contains the requested attribute/property.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <see cref="bool">boolean</see> value determining if the CAS contains the attribute/property name or not.</returns>
		public bool Contains(string key = null)
		{
			return HasAttribute(key);
		}
		/// <summary>
		/// Checks the string value for usability.
		/// </summary>
		/// <param name="q"></param>
		/// <returns>A <see cref="bool">boolean</see> value determining if the string value is practical to be used.</returns>
		private static bool CheckValue(string q = null)
		{
			bool res = false;
			if(!String.IsNullOrEmpty(q))
				if(q.Trim().Length>0)
					res=true;
			return res;
		}
		/// <summary>
		/// Gets the current user's username from the CAS.
		/// </summary>
		/// <returns>A <see cref="string">string</see> value representing the current user's username.</returns>
		public string GetUsername()
		{
			string res = string.Empty;
			if(CheckCas())
				res=HttpContext.Current.User.Identity.Name;
			return res;
		}
		/// <summary>
		/// Returns 0x01 or 0x10 upon success, 0x00 otherwise.
		/// </summary>
		/// <param name="username"></param>
		/// <returns>A <see cref="byte">byte</see> value where <see cref="byte">0x01</see> represents that the current username exists in the domain, <see cref="byte">0x10</see> represents that the current user does not exist within the domain, and <see cref="byte">0x00</see> represents that there was an error that occurred within the accountservices.exe program.</returns>
		private byte CheckUsername(string username = null)
		{
			byte res = 0x00;
			if(!String.IsNullOrEmpty(username))
			{
				string data = "{\"cmd\":\"check-user\",\"username\":\""+username+"\"}";
				ConsoleOutput.Log("Creating a new database class instance...");
				Database ins = new Database();
				string id = ins.GetUniqueID();
				ID=id;
				ins.SetAction(DatabasePrincipal.InsertPrincipal);
				ins.AddColumn("id",id);
				ins.AddColumn("data",data);
				ins.Send();
				ins.InvokeApplication();
				ins.ResponseWait();
				ins.CheckResponse();
				string tmp = ins.output[0].Values.ElementAt(0);
				ins.Clear();
				Database ins0 = new Database();
				ins0.RemoveRecord(id);
				if(tmp.Contains("status\":true"))
					res=0x01;
				else if(tmp.Contains("status\":false"))
					res=0x10;
				else
					ConsoleOutput.Error("An error has occurred while attempting to check if the username exists on the system...");
			}
			else
				ConsoleOutput.Error("The username provided is invalid.");
			return res;
		}
		/// <summary>
		/// Checks if the <see cref="ICasPrincipal">CAS</see> session principal is valid.
		/// </summary>
		/// <returns>A <see cref="true">boolean</see> value where <see cref="bool">true</see> represents that the CAS session principal is valid, and <see cref="bool">false</see> otherwise.</returns>
		public bool CheckCas()
		{
			bool res = false;
			if(CasAuthentication.CurrentPrincipal!=null)
				res=true;
			return res;
		}
		/// <summary>
		/// Verifies the session.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <see cref="bool">boolean</see> value where <see cref="bool">true</see> represents that the current session is valid, and <see cref="bool">false</see> otherwise.</returns>
		public bool CheckSession(string key = null)
		{
			bool res = false;
			if(!String.IsNullOrEmpty(key))
				if(System.Web.HttpContext.Current.Session[key]!=null)
					res=true;
			return res;
		}
	}
}