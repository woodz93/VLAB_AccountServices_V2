using DotNetCasClient;
using DotNetCasClient.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices {
	public class UserCheck : System.Web.UI.Page {
		private string id=null;
		private bool _IsChecked=false;
		private ICasPrincipal CAS_Principal=null;
		public UserCheck() {
			this.ini();
		}
		// Performs a database cleanup.
		private void CleanUp() {
			Database ins=new Database();
			ins.AsyncRemoveAllRecords();
		}
		// Returns true if the client checks are good to go, false otherwise.
		public bool IsChecked() {
			return this._IsChecked;
		}
		// Performs a username check with the AD server.
		private void ini() {
			if (!this.CheckSession("data")) {
				if (this.CheckCas()) {
					string un=this.GetUsername();
					if (!String.IsNullOrEmpty(un)) {
						this.CAS_Principal=CasAuthentication.CurrentPrincipal;
						var c=this.CheckUsername(un);
						string cmd=null;
						if (c==0x01) {
							cmd="set-password";
						} else if (c==0x10) {
							cmd="new-user";
						}
						if (!String.IsNullOrEmpty(cmd)) {
							console.Log(c.ToString());
							string json="{\"id\":\""+this.id+"\",\"cmd\":\""+cmd+"\",\"username\":\""+un+"\"}";
							this.ClearSession();
							this.SetSession("data",json);
							this._IsChecked=true;
						}
					}
				}
			} else {
				this._IsChecked=true;
			}
		}
		// Clears the session.
		public void ClearSession() {
			Session.Clear();
		}
		// Sets the session variable.
		public void SetSession(string key=null,string value=null) {
			if (!String.IsNullOrEmpty(key)) {
				if (!String.IsNullOrEmpty(value)) {
					try{
						Session.Add(key,value);
					}catch(Exception e){
						Session[key]=value;
					}
				}
			}
		}
		// Returns the user's first name.
		public string GetFirstName() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("givenName")[0];
			}
			return res;
		}
		// Returns the user's last name.
		public string GetLastName() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("sn")[0];
			}
			return res;
		}
		// Returns the user's full name.
		public string GetFullName() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("cn")[0];
			}
			return res;
		}
		// Returns the user's email address.
		public string GetEmail() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("uhEmail")[0];
			}
			return res;
		}
		// Returns the department name of the user.
		public string GetDepartment() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("ou")[0];
			}
			return res;
		}
		// Returns the user's telephone number.
		public string GetPhone() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("telephoneNumber")[0];
			}
			return res;
		}
		// Returns the user's job title.
		public string GetJobTitle() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("title")[0];
			}
			return res;
		}
		// Returns the user's affiliations.
		public List<string> GetAffiliations() {
			List<string> res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("eduPersonAffiliation");
			}
			return res;
		}
		// Returns the user's organization.
		public List<string> GetOrganizations() {
			List<string> res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("eduPersonOrgDN");
			}
			return res;
		}
		// Returns the user's UH ID number.
		public string GetUHID() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("uhUuid")[0];
			}
			return res;
		}
		// Returns the user's office location.
		public string GetOffice() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("physicalDeliveryOfficeName")[0];
			}
			return res;
		}
		// Returns the user's personal webpage/website URL.
		public string GetURL() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("labeledURI")[0];
			}
			return res;
		}
		// Returns the user's display name.
		public string GetDisplayName() {
			string res=null;
			if (this.CheckCas()) {
				res=this.GetAttribute("displayName")[0];
			}
			return res;
		}
		// Returns the campus the user belongs to.
		public string GetCampus() {
			string res=null;
			if (this.CheckCas()) {
				var tmp0=this.GetAttribute("uhScopedHomeOrg");
				console.Info(tmp0[0]);
				if (tmp0!=null) {
					var tmp=tmp0[0];
					if (tmp.Contains("org=")) {
						int st=tmp.IndexOf("org=");
						res=tmp.Substring(st,(tmp.Length-st));
					}
				}
			}
			return res;
		}
		// Returns the attribute located within the CAS principal, null is returned if the property key was not found.
		private List<string> GetAttribute(string key=null) {
			List<string> value=null;
			try{
				if (UserCheck.CheckValue(key)) {
					if (this.CAS_Principal!=null) {
						IAssertion sessionAssertion=this.CAS_Principal.Assertion;
						if (sessionAssertion!=null) {
							if (sessionAssertion.Attributes.ContainsKey(key)) {
								var array=sessionAssertion.Attributes[key].ToList<string>();
								if (array.Count()>0) {
									//string fname = GetAttribute("givenName");
									//string lname = GetAttribute("sn");
									value=array;
								} else {
									console.Warn("Array does not contain anything...");
								}
							} else {
								console.Warn("Session does not contain the key\n\t\t"+key);
							}
						} else {
							console.Warn("Session assertion is null");
						}
					} else {
						//console.Warn("CAS Principal is null");
						value=this.GetAttribute(key);
					}
				} else {
					console.Warn("Key failed validation");
				}
			}catch(Exception e){
				console.Error("Failed to get user information from CAS principal...\n\t\t"+e.Message);
			}
			return value;
		}
		// Returns true if the property key name exists within the CAS principal object, false otherwise.
		private bool HasAttribute(string key=null) {
			bool res=false;
			try{
				if (UserCheck.CheckValue(key)) {
					if (this.CAS_Principal!=null) {
						IAssertion sessionAssertion=this.CAS_Principal.Assertion;
						if (sessionAssertion!=null) {
							if (sessionAssertion.Attributes.ContainsKey(key)) {
								string[] array=sessionAssertion.Attributes[key].ToArray();
								if (array.Count()>0) {
									//res=array[0];
									res=true;
								}
							}
						}
					}
				}
			}catch(Exception e){
				console.Error("Failed to get user information from CAS principal...\n\t\t"+e.Message);
			}
			return res;
		}
		// Returns true if the string value passes validation checks, false otherwise.
		private static bool CheckValue(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (q.Trim().Length>0) {
					res=true;
				}
			}
			return res;
		}
		// Returns the username provided by the CAS system.
		public string GetUsername() {
			string res=null;
			if (this.CheckCas()) {
				var casp=CasAuthentication.CurrentPrincipal;
				res=HttpContext.Current.User.Identity.Name;
			}
			return res;
		}
		// Returns 0x01 or 0x10 upon success, 0x00 otherwise.
		private byte CheckUsername(string username=null) {
			byte res=0x00;
			if (!String.IsNullOrEmpty(username)) {
				string data="{\"cmd\":\"check-user\",\"username\":\""+username+"\"}";
				console.Log("Creating a new database class instance...");
				Database ins=new Database();
				string id=ins.GetUniqueID();
				this.id=id;
				ins.SetAction(DatabasePrincipal.InsertPrincipal);
				ins.AddColumn("id",id);
				ins.AddColumn("data",data);
				ins.Send();
				ins.InvokeApplication();
				ins.ResponseWait();
				ins.CheckResponse();
				string tmp=ins.output[0].Values.ElementAt(0);
				ins.Clear();
				Database ins0=new Database();
				//ins.AddWhere("id",id);
				ins0.RemoveRecord(id);
				if (tmp.Contains("status\":true")) {
					res=0x01;
				} else {
					res=0x10;
				}
			} else {
				console.Error("The username provided is invalid.");
			}
			return res;
		}
		// Returns true if the user completed the CAS authentication.
		public bool CheckCas() {
			bool res=false;
			if (CasAuthentication.CurrentPrincipal!=null) {
				res=true;
			}
			return res;
		}
		// Returns true if the session variable exists.
		public bool CheckSession(string key=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(key)) {
				if (System.Web.HttpContext.Current.Session[key]!=null) {
					res=true;
				}
			}
			return res;
		}
	}
}