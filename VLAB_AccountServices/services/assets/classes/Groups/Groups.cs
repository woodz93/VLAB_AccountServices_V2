﻿using DotNetCasClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

		public List<string> User_Groups=new List<string>();
		public List<ListItem> Refs=new List<ListItem>();

		public Groups() {
			this.ini();
		}
		public Groups(resetPassword obj) {
			this.RPObj=obj;
			this.ini();
		}
		public Groups(Default obj) {
			this.DObj=obj;
			this.ini();
		}

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
			console.Log(list.ToString());
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
				// Sends a request to the AD program...
				ins.SetAction(Database.DatabasePrincipal.InsertPrincipal);
				ins.AddColumn("id",id);
				ins.AddColumn("data",json);
				ins.Send();
				ins.InvokeApplication();
				ins.ResponseWait();
				ins.Clear();
				// Attempts to get the results from the record...
				Database.Database ins0=new Database.Database();
				ins0.SetAction(Database.DatabasePrincipal.SelectPrincipal);
				ins0.AddWhere("id",id);
				ins0.Send();
				res=ins0.Results;
				Database.Database ins1=new Database.Database();
				ins1.RemoveRecord(id);
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
		// Selects the groups...
		public void SelectGroup(string name=null) {
			if (Str.Str.CheckStr(name)) {
				int i=0;
				var con=(CheckBoxList)RPObj.FindControl("GroupsElement");
				//console.Warn(con.Items.Count.ToString());
				//console.Warn(name);
				//resetPassword.StatusElm.Text+="<br>"+name+"<br>";
				while(i<con.Items.Count){
					if (Element.groupList.ContainsKey(name)) {
						resetPassword.StatusElm.Text+="<br>&nbsp;&nbsp;&nbsp;&nbsp;"+con.Items[i]+" === "+name+"<br>";
						if (con.Items[i].Value==Element.groupList[name]) {
							con.Items[i].Selected=true;
							con.Items[i].Enabled=false;
							break;
						}
					}
					i++;
				}
				/*
				while(i<this.Refs.Count){
					item=this.Refs[i];
					if (item.Value==name) {
						var con=(CheckBoxList)RPObj.FindControl("groups");
						con.ClearSelection();
						//con.SelectedValue="group_item_"+name;
						int o=0;
						while(o<con.Items.Count){
							if (con.Items[o].Value==name) {

							}
							o++;
						}
					}
					i++;
				}
				*/
			}
		}
		// Loads the groups into an HTML element.
		public bool LoadGroups(CheckBoxList elm=null) {
			bool res=false;
			if (elm!=null) {
				if (this.Data.Count>0) {
					int i=0;
					ListItem item=null;
					this.Refs.Clear();
					while(i<this.Data.Count){
						item=new ListItem();
						item.Text=this.Data[i].Name;
						item.Value=this.Data[i].Reference;
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
			Database.Database ins=new Database.Database();
			ins.SetTable("vlab_groups");
			ins.SetAction(Database.DatabasePrincipal.SelectPrincipal);
			ins.AddColumn("id","*");
			ins.Send();
			//List<Dictionary<string,string>>
			Records list=new Records();
			int i=0;
			//Dictionary<string,string>sel=null;
			GroupData gd=null;
			while(list.Read()){
				//sel=list.GetRow(i);
				gd=new GroupData(list.GetRow(i));
				res.Add(gd);
				i++;
			}
			this.Data=res;
		}
		




	}
}