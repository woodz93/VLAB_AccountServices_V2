using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
namespace VLAB_AccountServices.services.assets.sys {
	public class Element : resetPassword {
		public static Dictionary<string,string> groupList=new Dictionary<string,string>();
		public static CheckBoxList group_element=null;
		// Sets the group to modify.
		public static void SetGroupElement(CheckBoxList elm) {
			if (elm!=null) {
				Element.group_element=elm;
			}
		}
		// Adds a group to the collection.
		public static void AddGroup(string name=null) {
			if (Element.CheckString(name)) {
				if (!Element.groupList.ContainsKey(name)) {
					Element.groupList.Add(name,name);
				}
			}
		}
		// Adds a group to the collection.
		public static void AddGroup(string name=null,string display_name=null) {
			if (Element.CheckString(name)) {
				if (!Element.groupList.ContainsKey(display_name)) {
					if (Element.CheckString(display_name)) {
						Element.groupList.Add(display_name,name);
					} else {
						console.Error("Display name was not specified.");
					}
				}
			}
		}
		// Adds the groups to the group list.
		public static void SetGroups() {
			if (Element.group_element!=null) {
				if (Element.group_element.Items.Count>0) {
					Element.group_element.Items.Clear();
				}
				//console.Log(Element.groupList.ToString());
				foreach(var item in Element.groupList){
					Element.group_element.Items.Add(item.Key);
				}
			} else {
				console.Error("HTML group list element was not specified in the ini call to this class.");
			}
		}
		// Returns true if the string value is valid, false otherwise.
		private static bool CheckString(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					if (q.Length>3) {
						res=true;
					}
				}
			}
			return res;
		}
		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected static string genID() {
			string res=null;
			string id=Element.genRandID();
			try{
				int i=0;
				int lim=100;
				while(Element.groupList.ContainsKey(id) && i<lim) {
					id=Element.genRandID();
					i++;
				}
				if (!(i>=lim)) {
					res=id;
				}
			}catch(Exception ex){
				sys.error("Failed to generate ID...\n"+ex.Message);
			}
			return res;
		}
		// Generates a randomized length of random characters to compose the record's ID on the database.
		protected static string genRandID() {
			Random r=new Random();
			int lim=r.Next(10,128);
			int i=0;
			char c;
			string res="";
			int sel=r.Next(0,30);
			while(i<lim){
				sel=r.Next(0,30);
				if (sel<10) {
					c=(char)r.Next(48,57);
				} else if (sel<20) {
					c=(char)r.Next(65,90);
				} else {
					c=(char)r.Next(97,122);
				}
				res+=c;
				i++;
			}
			return res;
		}
	}
}