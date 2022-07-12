using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.classes.sys;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices.services.assets.classes.UserInfo {
	public class UserInfo {
		private string raw=null;
		private bool ini_complete=false;
		private UserData ud=null;
		public UserInfo() {

		}
		public UserInfo(string data=null) {
			if (this.CheckValue(data)) {
				this.SetData(data);
			}
		}
		public UserInfo(UserData obj=null) {
			if (obj!=null) {
				try{
					this.SetData(JsonSerializer.Serialize(obj));
				}catch(Exception e){
					console.Error("Failed to deserialize the data object.\n\t\t"+e.Message);
				}
			}
		}
		// Sets the data string.
		public void SetData(string data=null) {
			if (this.CheckValue(data)) {
				this.raw=data;
				this.ParseDataString();
			}
		}
		// Attempts to parse the raw data string.
		private void ParseDataString() {
			if (this.CheckValue(this.raw)) {
				try{
					this.ud=JsonSerializer.Deserialize<UserData>(this.raw);
				}catch(Exception e){
					console.Error("Unable to parse data object string...\n\t\t"+e.Message);
				}
			}
		}
		// Returns the user's authentication key provided to them in order to complete the form.
		public string GetKey() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Key;
			}
			return res;
		}
		// Returns the account type of the user.
		public string GetAccountType() {
			string res=null;
			if (this.Check()) {
				res=this.ud.AccountType;
			}
			return res;
		}
		// Returns the building of the user.
		public string GetBuilding() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Building;
			}
			return res;
		}
		// Returns the department of the user.
		public string GetDepartment() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Department;
			}
			return res;
		}
		// Returns the phone number of the user.
		public string GetPhone() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Phone;
			}
			return res;
		}
		// Returns the last name of the user.
		public string GetLastName() {
			string res=null;
			if (this.Check()) {
				res=this.ud.LastName;
			}
			return res;
		}
		// Returns the first name of the user.
		public string GetFirstName() {
			string res=null;
			if (this.Check()) {
				res=this.ud.FirstName;
			}
			return res;
		}
		// Returns the email of the user.
		public string GetEmail() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Email;
			}
			return res;
		}
		// Returns the username of the user.
		public string GetUsername() {
			string res=null;
			if (this.Check()) {
				res=this.ud.Username;
			}
			return res;
		}
		// Returns true if the class object contains the required data.
		private bool Check() {
			bool res=false;
			if (this.ini_complete) {
				if (this.ud!=null) {
					res=true;
				}
			}
			return res;
		}
		// Returns true if the parameter is valid, false otherwise.
		private bool CheckValue(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (q.Trim().Length>0) {
					res=true;
				}
			}
			return res;
		}
	}
}