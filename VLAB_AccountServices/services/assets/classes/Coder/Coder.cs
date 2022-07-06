using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices {
	public class Coder {

		private Dictionary<int,string> Decode_Table=new Dictionary<int,string>();
		private Dictionary<string,int> Encode_Table=new Dictionary<string,int>();

		private string Value=null;
		private bool ini_complete=false;


		public Coder() {
			this.ini();
		}

		public Coder(string str=null) {
			this.SetValue(str);
			this.ini();
		}

		private void ini() {
			this.SetupCodeTable();
		}

		private void SetupCodeTable() {
			if (!this.ini_complete) {
				string[] list={
					"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
					"0","1","2","3","4","5","6","7","8","9","'","`","-"," ","_","+","(",")","[","]","{","}","|"
				};
				int i=0;
				int num=0;
				while(i<list.Length){
					num=Coder.GetCode(i,list[i]);
					this.Decode_Table.Add(num,list[i]);
					this.Encode_Table.Add(list[i],num);
					i++;
				}
				this.ini_complete=true;
			}
		}


		// Sets the value.
		public void SetValue(string str=null) {
			if (this.CheckString(str)) {
				this.Value=str;
			}
		}

		private static int GetCode(int i=0,string v=null) {
			return (Convert.ToInt32(v)-97)+((i+1)/2);
		}

		// Returns a string-character decoded from an encoded character value.
		public string DecodeString() {
			string res=null;
			if (this.CheckString(this.Value)) {
				int i=0;
				res="";
				while(i<this.Value.Length){
					res+=this.DecodeChar(this.Value[i]);
					i++;
				}
			}
			return res;
		}
		// Returns a string-character decoded from an encoded character value.
		public string DecodeString(string q=null) {
			string res=null;
			if (this.CheckString(q)) {
				int i=0;
				res="";
				while(i<q.Length){
					res+=this.DecodeChar(q[i]);
					i++;
				}
			}
			return res;
		}


		// Returns an encoded string value.
		public string EncodeString() {
			string res=null;
			if (this.CheckString(this.Value)) {
				int i=0;
				res="";
				while(i<this.Value.Length){
					res+=this.EncodeChar(this.Value[i]);
					i++;
				}
			}
			return res;
		}
		// Returns an encoded string value.
		public string EncodeString(string q=null) {
			string res=null;
			if (this.CheckString(q)) {
				int i=0;
				res="";
				while(i<q.Length){
					res+=this.EncodeChar(q[i]);
					i++;
				}
			}
			return res;
		}


		// Returns a single character (string), decoded from the character provided on success, false otherwise.
		private string DecodeChar(char c) {
			string res=null;
			//string sel=Convert.ToChar(c).ToString();
			int sel=Convert.ToInt32(c);
			if (this.Decode_Table.ContainsKey(sel)) {
				res=this.Decode_Table[sel];
			}
			return res;
		}
		// Returns the encoded character from a given character provided.
		private char EncodeChar(char c) {
			char res='\0';
			string sel=c.ToString();
			if (this.Encode_Table.ContainsKey(sel)) {
				res=Convert.ToChar(this.Encode_Table[sel]);
			}
			return res;
		}

		private bool CheckString(string q=null) {
			bool res=false;
			if (q!=null) {
				if (!String.IsNullOrEmpty(q)) {
					if (!String.IsNullOrWhiteSpace(q)) {
						if (q.Length>0) {
							res=true;
						}
					}
				}
			}
			return res;
		}

		// Returns a string-character decoded from an encoded character value.
		public static string EncodeString(char q) {
			string res="";
			int num=Convert.ToInt32(q);
			switch(num){
				case 0:
					res="a";
					break;
				case 1:
					res="b";
					break;
				case 2:
					res="c";
					break;
				case 3:
					res="d";
					break;
				case 4:
					res="e";
					break;
				case 5:
					res="f";
					break;
				case 6:
					res="g";
					break;
				case 7:
					res="h";
					break;
				case 8:
					res="i";
					break;
				case 9:
					res="j";
					break;
				case 10:
					res="k";
					break;
				case 11:
					res="l";
					break;
				case 12:
					res="m";
					break;
				case 13:
					res="n";
					break;
				case 14:
					res="o";
					break;
				case 15:
					res="p";
					break;
				case 16:
					res="q";
					break;
				case 17:
					res="r";
					break;
				case 18:
					res="s";
					break;
				case 19:
					res="t";
					break;
				case 20:
					res="u";
					break;
				case 21:
					res="v";
					break;
				case 22:
					res="w";
					break;
				case 23:
					res="x";
					break;
				case 24:
					res="y";
					break;
				case 25:
					res="z";
					break;
				case 26:
					res="0";
					break;
				case 27:
					res="1";
					break;
				case 28:
					res="2";
					break;
				case 29:
					res="3";
					break;
				case 30:
					res="4";
					break;
				case 31:
					res="5";
					break;
				case 32:
					res="6";
					break;
				case 33:
					res="7";
					break;
				case 34:
					res="8";
					break;
				case 35:
					res="9";
					break;
				case 36:
					res="'";
					break;
				case 37:
					res="`";
					break;
				case 38:
					res="-";
					break;
				default:
					res="";
					break;
			}
			return res;
		}

	}
}