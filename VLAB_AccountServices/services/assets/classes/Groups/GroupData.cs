using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices.services.assets.classes.Groups {
	public class GroupData {

		public string Name			=null;
		public string ID			=null;
		public string Owner			=null;
		public string Category		=null;
		public string Department	=null;
		public string Raw			=null;
		public string Reference		=null;


		public GroupData(Dictionary<string,string> q=null) {
			this.ini(q);
		}


// Group_ID design:	[unique_50_char_code]-[8_hex_owner_code]-[8_hex_category_code]-[8_hex_department_code]-[ad_reference_name]-[additional_padding_starting_with_4_zero_width_characters_until_maximum_255_characters_long]

		private void ini(Dictionary<string,string> q=null) {
			if (GroupData.CheckValue(q)) {
				this.Raw=q["group_id"];
				this.Name=q["group_name"];
				this.ProcData();
			}
		}


		// Returns a string consisting of the meta data of the group information/data.
		private string ProcData() {
			string res=null;
			if (this.Raw!=null) {
				Coder cins=new Coder();
				this.ID=this.Raw.Substring(0,50);
				//cins.SetValue(this.DecodeString(this.Raw.Substring(51,59)));
				this.Owner=cins.DecodeString(this.Raw.Substring(51,59));
				//cins.SetValue(this.Raw.Substring(60,69));
				this.Category=cins.DecodeString(this.Raw.Substring(60,69));

				this.Department=cins.DecodeString(this.Raw.Substring(70,79));

				string tmp=cins.DecodeString(this.Raw.Substring(80,this.Raw.Length));
				char ch=Convert.ToChar(8203);
				if (tmp.Contains(ch)) {
					this.Reference=cins.DecodeString(tmp.Substring(0,tmp.IndexOf(ch)+1));
				}
			}
			return res;
		}


		// Returns a string representing the decoded value of the character integer (decimal) value.
		private string DecodeString(string q=null) {
			string res=null;
			if (q!=null) {
				if (q.Length>0) {
					int i=0;
					Coder cins=new Coder(q);
					res=cins.DecodeString();
				}
			}
			return res;
		}


		// Returns true if the parameter value is valid, false otherwise.
		private static bool CheckValue(Dictionary<string,string> q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ContainsKey("group_name")) {
					if (q.ContainsKey("group_id")) {
						if (q["group_name"].Length>0) {
							if (q["group_id"].Length>0) {
								res=true;
							}
						}
					}
				}
			}
			return res;
		}




	}
}