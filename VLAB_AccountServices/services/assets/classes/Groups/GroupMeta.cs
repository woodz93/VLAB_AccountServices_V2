using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace VLAB_AccountServices.services.assets.classes.Groups {
	public class GroupMeta {

		public Dictionary<string,Label>				Labels				=new Dictionary<string,Label>();
		public Dictionary<string,CheckBox>			CheckBoxes			=new Dictionary<string,CheckBox>();
		public Dictionary<string,CheckBoxList>		CheckBoxLists		=new Dictionary<string,CheckBoxList>();
		public Dictionary<string,TextBox>			TextBoxes			=new Dictionary<string,TextBox>();
		public Dictionary<string,DropDownList>		DropDownLists		=new Dictionary<string,DropDownList>();


		public GroupMeta() {

		}





		// Adds an element to the group collection.
		public void AddElement(Label q=null) {
			this.AddLabel(q);
		}
		// Adds an element to the group collection.
		public void AddElement(CheckBox q=null) {
			this.AddCheckBox(q);
		}
		// Adds an element to the group collection.
		public void AddElement(CheckBoxList q=null) {
			this.AddCheckBoxes(q);
		}
		// Adds an element to the group collection.
		public void AddElement(TextBox q=null) {
			this.AddTextBox(q);
		}
		// Adds an element to the group collection.
		public void AddElement(DropDownList q=null) {
			this.AddDropDown(q);
		}



		public void AddLabel(Label q=null) {
			this.CheckElm(q);
		}
		public void AddCheckBox(CheckBox q=null) {
			this.CheckElm(q);
		}
		public void AddCheckBoxes(CheckBoxList q=null) {
			this.CheckElm(q);
		}
		public void AddTextBox(TextBox q=null) {
			this.CheckElm(q);
		}
		public void AddDropDown(DropDownList q=null) {
			this.CheckElm(q);
		}

		public void RemoveElement(string id=null) {
			if (this.CheckString(id)) {
				if (this.IDExists(id)) {
					string sel=this.GetElementGroup(id)[0];
					if (sel=="labels") {
						this.Labels.Remove(id);
					} else if (sel=="checkboxes") {
						this.CheckBoxes.Remove(id);
					} else if (sel=="checkboxlists") {
						this.CheckBoxLists.Remove(id);
					} else if (sel=="textboxes") {
						this.TextBoxes.Remove(id);
					} else if (sel=="dropdownlists") {
						this.DropDownLists.Remove(id);
					}
				}
			}
		}

		// Returns the matching label element.
		public Label GetLabelById(string id=null) {
			Label res=null;
			if (this.IDExists(id)) {
				List<string> list=this.GetElementGroup(id);
				if (list.Count>0) {
					if (list[0]=="labels") {
						try{
							res=this.Labels[id];
						}catch(Exception e){}
					}
				}
			}
			return res;
		}
		// Returns the matching checkbox element.
		public CheckBox GetCheckBoxById(string id=null) {
			CheckBox res=null;
			if (this.IDExists(id)) {
				List<string> list=this.GetElementGroup(id);
				if (list.Count>0) {
					if (list[0]=="checkboxes") {
						try{
							res=this.CheckBoxes[id];
						}catch(Exception e){}
					}
				}
			}
			return res;
		}
		// Returns the matching checkbox list element.
		public CheckBoxList GetCheckBoxListById(string id=null) {
			CheckBoxList res=null;
			if (this.IDExists(id)) {
				List<string> list=this.GetElementGroup(id);
				if (list.Count>0) {
					if (list[0]=="checkboxlists") {
						try{
							res=this.CheckBoxLists[id];
						}catch(Exception e){}
					}
				}
			}
			return res;
		}
		// Returns the matching textbox element.
		public TextBox GetTextBoxById(string id=null) {
			TextBox res=null;
			if (this.IDExists(id)) {
				List<string> list=this.GetElementGroup(id);
				if (list.Count>0) {
					if (list[0]=="textboxes") {
						try{
							res=this.TextBoxes[id];
						}catch(Exception e){}
					}
				}
			}
			return res;
		}
		// Returns the matching dropdown element.
		public DropDownList GetDropDownListById(string id=null) {
			DropDownList res=null;
			if (this.IDExists(id)) {
				List<string> list=this.GetElementGroup(id);
				if (list.Count>0) {
					if (list[0]=="dropdownlists") {
						try{
							res=this.DropDownLists[id];
						}catch(Exception e){}
					}
				}
			}
			return res;
		}

		// Returns a list of strings representing which list the id exists in.
		public List<string> GetElementGroup(string id=null) {
			List<string> res=new List<string>();
			if (this.IDExists(id)) {
				if (this.Labels.ContainsKey(id)) {
					res.Add("labels");
				}
				if (this.CheckBoxes.ContainsKey(id)) {
					res.Add("checkboxes");
				}
				if (this.CheckBoxLists.ContainsKey(id)) {
					res.Add("checkboxlists");
				}
				if (this.TextBoxes.ContainsKey(id)) {
					res.Add("textboxes");
				}
				if (this.DropDownLists.ContainsKey(id)) {
					res.Add("dropdownlists");
				}
			}
			return res;
		}

		// Returns true if the element id was found, false otherwise.
		public bool IDExists(string id=null) {
			bool res=false;
			if (this.CheckString(id)) {
				if (this.Labels.ContainsKey(id) || this.CheckBoxes.ContainsKey(id) || this.CheckBoxLists.ContainsKey(id) || this.TextBoxes.ContainsKey(id) || this.DropDownLists.ContainsKey(id)) {
					res=true;
				}
			}
			return res;
		}

		// Returns true if the string value is valid, false otherwise.
		private bool CheckString(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					if (q.Length>0) {
						res=true;
					}
				}
			}
			return res;
		}

		// Returns true if the element was set successfully, false otherwise.
		private bool CheckElm(Label q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ID!=null) {
					if (!this.Labels.ContainsKey(q.ID)) {
						this.Labels.Add(q.ID, q);
					} else {
						this.Labels[q.ID]=q;
					}
					res=true;
				}
			}
			return res;
		}
		// Returns true if the element was set successfully, false otherwise.
		private bool CheckElm(CheckBox q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ID!=null) {
					if (!this.CheckBoxes.ContainsKey(q.ID)) {
						this.CheckBoxes.Add(q.ID, q);
					} else {
						this.CheckBoxes[q.ID]=q;
					}
					res=true;
				}
			}
			return res;
		}
		// Returns true if the element was set successfully, false otherwise.
		private bool CheckElm(CheckBoxList q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ID!=null) {
					if (!this.CheckBoxLists.ContainsKey(q.ID)) {
						this.CheckBoxLists.Add(q.ID, q);
					} else {
						this.CheckBoxLists[q.ID]=q;
					}
					res=true;
				}
			}
			return res;
		}
		// Returns true if the element was set successfully, false otherwise.
		private bool CheckElm(TextBox q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ID!=null) {
					if (!this.TextBoxes.ContainsKey(q.ID)) {
						this.TextBoxes.Add(q.ID, q);
					} else {
						this.TextBoxes[q.ID]=q;
					}
					res=true;
				}
			}
			return res;
		}
		// Returns true if the element was set successfully, false otherwise.
		private bool CheckElm(DropDownList q=null) {
			bool res=false;
			if (q!=null) {
				if (q.ID!=null) {
					if (!this.DropDownLists.ContainsKey(q.ID)) {
						this.DropDownLists.Add(q.ID, q);
					} else {
						this.DropDownLists[q.ID]=q;
					}
					res=true;
				}
			}
			return res;
		}






	}
}