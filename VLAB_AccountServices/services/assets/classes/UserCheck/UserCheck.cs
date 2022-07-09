﻿using DotNetCasClient;
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