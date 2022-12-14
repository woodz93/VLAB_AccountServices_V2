using DotNetCasClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web.UI.WebControls;
using VLAB_AccountServices.services.assets.classes.Database;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices.services.assets.classes.Groups {
	public class Groups {
		public List<Dictionary<string,string>>Group_List=new List<Dictionary<string,string>>();			// Contains all of the groups collected from the database.
		// Flow control properties...
		// Class object management...
		private string Mode				=null;
		private resetPassword RPObj		=null;
		private Default DObj			=null;
		// Element references...
		private GroupMeta Elm			=null;
		public List<GroupData> Data		=null;
		private List<string> CheckedIn	=new List<string>();
		public List<string> User_Groups	=new List<string>();
		public List<ListItem> Refs		=new List<ListItem>();
		// Groups class initialization.
		public Groups() {
			this.ini();
		}
		// Groups class initialization under the resetPassword class object.
		public Groups(resetPassword obj) {
			this.RPObj=obj;
			this.ini();
		}
		// Groups class initialization for Default page.
		public Groups(Default obj) {
			this.DObj=obj;
			this.ini();
		}
		// General initialization method.
		private void ini() {
			this.Elm=new GroupMeta();
			this.Data=this.GetGroups();
		}
		// Returns a list of group data objects.
		public List<GroupData> GetGroups() {
			List<GroupData> res=new List<GroupData>();
			if (this.Data==null) {
				this.UpdateGroups();
			}
			res=this.Data;
			return res;
		}
		// Adds an item to include in the list of pre-checked/pre-selected items.
		public void AddCheckedItem(string reference=null) {
			if (Str.Str.CheckStr(reference)) {
				this.CheckedIn.Add(reference);
			}
		}
		// Removes all checked in items.
		public void ClearItems() {
			this.CheckedIn.Clear();
		}
		// Processes the user's groups.
		public void ProcessUserGroups() {
			Records list=this.GetUserGroups();
			int i=0;
			var tmp=JsonSerializer.Deserialize<GroupInfo>(list.GetRow(0)["data"]);
			this.User_Groups=tmp.data;
		}
		// Sends a query request to the AD program to get the groups associated with the current user.
		public Records GetUserGroups() {
			Records res=null;
			string username=this.GetUsername();
			if (username!=null) {
				string json="{\"cmd\":\"get-groups\",\"username\":\""+username+"\"}";
				Database.Database ins=new Database.Database();
				string id=ins.GetUniqueID();
				try{
					// Sends a request to the AD program...
					ins.SetAction(Database.DatabasePrincipal.InsertPrincipal);
					ins.AddColumn("id",id);
					ins.AddColumn("data",json);
					ins.Send();
					ins.InvokeApplication();		// Takes about 2 seconds to complete.
					ins.ResponseWait();
					try{
						// Attempts to get the results from the record...
						Database.Database ins0=new Database.Database();
						ins0.SetAction(Database.DatabasePrincipal.SelectPrincipal);
						ins0.AddWhere("id",id);
						ins0.Send();
						res=ins0.Results;
					}catch(Exception ex){
						ConsoleOutput.Warn("Failed to query selection of group collection...\n\t\t"+ex.Message);
					}
					try{
						// Removes the record from the database...
						Database.Database ins1=new Database.Database();
						ins1.AddColumn("id",id);
						ins1.AddWhere("id",id);
						ins1.RemoveRecord(id);
					}catch(Exception exc){
						ConsoleOutput.Warn("Failed to remove record with id of \""+id+"\"\n\t\t"+exc.Message);
					}
				}catch(Exception e){
					ConsoleOutput.Warn("Failed to query request for group collection...\n\t\t"+e.Message);
				}
			}
			return res;
		}
		// Returns the current user's username.
		private string GetUsername() {
			string res=null;
			if (CasAuthentication.CurrentPrincipal!=null) {
				var cis=CasAuthentication.CurrentPrincipal;
				try{
					res=System.Web.HttpContext.Current.User.Identity.Name;
				}catch(Exception ex){
					res=null;
				}
			}
			return res;
		}
		// Returns the display name from the AD name.
		private string GetDisplayName(string name=null) {
			string res=null;
			if (Str.Str.CheckStr(name)) {
				if (Element.groupList.ContainsValue(name)) {
					int i=0;
					var list=Element.groupList.Values;
					var keys=Element.groupList.Keys;
					while(i<Element.groupList.Count){
						if (list.ElementAt(i)==name) {
							res=keys.ElementAt(i);
							break;
						}
						i++;
					}
				}
			}
			return res;
		}
		// Selects the groups...
		public void SelectGroup(string name=null) {
			// NOTE: The parameter "name" is the actual AD group name, NOT the display name.
			if (Str.Str.CheckStr(name)) {
				int i=0;
				var con=(CheckBoxList)RPObj.FindControl("GroupsElement");
				var con0=(CheckBoxList)RPObj.FindControl("UserGroupsElement");
				string dn;										// Display name.
				// NOTE: The values within the "GroupElement" only contain the "display_name" value, NOT the actual AD group name(s).
				if (Element.groupList.ContainsValue(name)) {
					dn=this.GetDisplayName(name);				// Gets the display name from the given AD reference name.
					while(i<con.Items.Count){					// Iterates through the group elements to check if the values match the display name.
						if (con.Items[i].Text==dn) {
							con.Items[i].Selected=true;
							con.Items[i].Enabled=false;
							con0.Items.Add(con.Items[i]);
							break;								// Does not need to continue iteration because the group was found.
						}
						i++;
					}
				}
			}
		}
		// Returns the key based on the value.
		public string GetKey(string val=null) {
			string res=null;
			if (val!=null) {
				if (Element.groupList.ContainsValue(val)) {
					int i=0;
					foreach(var item in Element.groupList){
						if (item.Value==val) {
							res=item.Key;
							break;
						}
						i++;
					}
				}
			}
			return res;
		}
		// Loads the groups into an HTML element.
		public bool LoadGroups(CheckBoxList elm=null) {
			bool res=false;
			if (elm!=null) {
				if (this.Data.Count>0) {
					int i=0;
					ListItem item;
					this.Refs.Clear();
					while(i<this.Data.Count){
						item=new ListItem {
							Text=this.Data[i].Name,
							Value=this.Data[i].Reference
						};
						item.Attributes.Add("id","group_item_"+this.Data[i].Reference);
						this.Refs.Add(item);
						elm.Items.Add(item);
						i++;
					}
					res=true;
				}
			}
			return res;
		}
		// Updates the group data.
		public void UpdateGroups() {
			List<GroupData> res=new List<GroupData>();
			Database.Database ddins=new Database.Database();
			ddins.SetTable("vlab_groups");
			ddins.SetAction(Database.DatabasePrincipal.SelectPrincipal);
			ddins.AddWhere("id","*");
			ddins.Send();
			Records list=ddins.Results;
			ddins.Clear();
			int i=0;
			GroupData gd=null;
			while(list.Read()){
				gd=new GroupData(list.GetRow(i));
				res.Add(gd);
				i++;
			}
			this.Data=res;
		}
	}
}