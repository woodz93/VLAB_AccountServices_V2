using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json;

namespace VLAB_AccountServices.services.assets.sys {
	public class Status {
		public string id{get;set; }
		public string data{get;set; }
		private Status obj{get;set; }
		private bool status{get;set; }
		private string msg{get;set; }
		private string code{get;set; }
		private string file{get;set; }
		private string source{get;set; }
		private int timestamp{get;set; }
		private string line{get;set; }
		public static Status ToObject(string q) {
			Status res=null;
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					try{
						res=JsonSerializer.Deserialize<Status>(q);
					}catch(Exception e){

					}
				}
			}
			return res;
		}
		// Populates this object's properties.
		public void ToObject() {
			if (!String.IsNullOrEmpty(this.data)) {
				if (this.obj == null) {
					try{
						this.obj=JsonSerializer.Deserialize<Status>(this.data);
					}catch(Exception e){

					}
				}
			}
		}
		// Returns a uint value representing the status from the object.
		public uint GetStatus() {
			uint res=0x00;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.status)) {
					//res=this.obj.status;
					if (this.obj.status==false) {
						res=0x01;
					} else if (this.obj.status==true) {
						res=0x10;
					}
				}
			}
			return res;
		}
		// Returns a string representing the line number of the fault.
		public string GetLine() {
			string res=null;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.line)) {
					res=this.obj.line;
				}
			}
			return res;
		}
		// Returns a string representing the status message.
		public string GetMessage() {
			string res=null;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.msg)) {
					res=this.obj.msg;
				}
			}
			return res;
		}
		// Returns the response/status code.
		public string GetResponseCode() {
			string res=null;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.code)) {
					res=this.obj.code;
				}
			}
			return res;
		}
		// Returns the file name of the fault.
		public string GetFileName() {
			string res=null;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.file)) {
					res=this.obj.file;
				}
			}
			return res;
		}
		// Returns the source of the fault.
		public string GetSource() {
			string res=null;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.source)) {
					res=this.obj.source;
				}
			}
			return res;
		}
		// Returns the timestamp of the response/status message, -1 indicates failure.
		public int GetTimestamp() {
			int res=-1;
			if (this.obj!=null) {
				if (this.CheckValue(this.obj.timestamp)) {
					res=this.obj.timestamp;
				}
			}
			return res;
		}
		// Returns true if the integer passes verification, false otherwise.
		private bool CheckValue(int val) {
			bool res=false;
			if (val!=null) {
				res=true;
			}
			return res;
		}
		// Returns true if the value passes verification, false otherwise.
		private bool CheckValue(uint val) {
			bool res=false;
			if (val!=null) {
				res=true;
			}
			return res;
		}
		// Returns true if the value passes verification, false otherwise.
		private bool CheckValue(bool val) {
			bool res=false;
			if (val==true || val==false) {
				res=true;
			}
			return res;
		}
		// Returns true if the string passes verification, false otherwise.
		private bool CheckValue(string str) {
			bool res=false;
			if (!String.IsNullOrEmpty(str)) {
				if (!String.IsNullOrWhiteSpace(str)) {
					if (str.Length>0) {
						res=true;
					}
				}
			}
			return res;
		}
	}
}