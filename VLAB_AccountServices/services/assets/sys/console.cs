using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VLAB_AccountServices.services.assets.classes.Network;
namespace VLAB_AccountServices.services.assets.sys {
	public class console {
		private static bool enable_output=true;
		private static uint mode=0x00;
		// Instances:
		private static Default Default_Instance;
		private static resetPassword resetPassword_Instance;
		private static Network Network_Instance;
		private static sys sys_Instance;
		public static bool errored=false;
		public static bool ini_complete=false;
		private static bool PlacedStyle=false;
		//private static uint debugging_mode=0x111;								// Full debugging enabled
		private static uint debugging_mode=0x011;								// All debugging disabled
		

		// Initializes the console class for use with the Default class.
		public static void ini(Default instance) {
			if (!console.ini_complete) {
				console.Default_Instance = instance;
				console.mode=0x00;
				console.ini_complete=true;
				console.errored=false;
				console.PlacedStyle=false;
				instance.StatElm.Text+="Begun on Default<br>";
			}
		}
		// Initializes the console class for use with the resetPassword class.
		public static void ini(resetPassword instance) {
			if (!console.ini_complete) {
				console.resetPassword_Instance = instance;
				console.mode=0x01;
				console.ini_complete=true;
				console.errored=false;
				console.PlacedStyle=false;
			}
		}
		// Initializes the console class for use with the Network class.
		public static void ini(Network instance) {
			if (!console.ini_complete) {
				console.Network_Instance = instance;
				console.mode=0x0F;
				console.ini_complete=true;
				console.errored=false;
				console.PlacedStyle=false;
			}
		}
		// Initializes the console class for use with the sys class.
		public static void ini(sys instance) {
			if (!console.ini_complete) {
				console.sys_Instance = instance;
				console.mode=0x0F;
				console.ini_complete=true;
				console.errored=false;
				console.PlacedStyle=false;
			}
		}
		// Writes output to the client.
		public static void Log(string message) {
			if (console.debugging_mode>=0x111) {
				console.Output("<font style='color:rgb(255,255,255);' class='debug-title'>[LOG]</font>:<font style='color:rgb(255,255,255);' class='debug-text'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Warn(string message) {
			if (console.debugging_mode>=0x100) {
				console.Output("<font style='color:rgb(255,200,50);' class='debug-title'>[WARN]</font>:<font style='color:rgb(255,200,50);' class='debug-text'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Error(string message) {
			if (console.debugging_mode>=0x011) {
				console.Output("<font style='color:red;' class='debug-title'>[ERROR]</font>:<font style='color:red;' class='debug-text'>\t\t"+message+"</font>");
				if (!console.errored) {
					console.errored=true;
				}
			}
		}
		// Writes output to the client.
		public static void Info(string message) {
			if (console.debugging_mode>=0x110) {
				console.Output("<font style='color:cyan;' class='debug-title'>[INFO]</font>:<font style='color:cyan;' class='debug-text'>\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Success(string message) {
			if (console.debugging_mode>=0x101) {
				console.Output("<font style='color:rgb(100,255,100);' class='debug-title'>[SUCCESS]</font>:<font style='color:rgb(100,255,100);' class='debug-text'>\t"+message+"</font>");
			}
		}
		private static void Output(string q=null) {
			if (console.enable_output) {
				if (console.CheckValue(q)) {
					string str="["+console.getTime()+"] "+q+"\n\t\t[<font style='color:rgb(255,50,150);'>"+console.GetCallingFunctionPath()+"</font>|"+console.GetCallingFunction()+" (line: <font style='color:rgb(200,150,0);'>"+console.GetCallingFunctionLineNumber()+"</font>)]\n";
					str=console.HTMLEncode(str);
					str="<div class=\"debug\">"+str+"</div>";
					if (!console.PlacedStyle) {
						str="<style>.debug{font-family:monospace !important;font-size:0.85em !important;background-color:rgba(0,0,0,0.85);color:#FFF;}.debug-title{font-weight:bolder;}.debug-text{font-weight:normal;}</style>"+str;
						console.PlacedStyle=true;
					}
					if (console.mode==0x00) {
						try{
							//Default.StatusElm.Text+=str;
							//console.Default_Instance.StatElm.Text+=str;
						}catch{
							try{
								console.Default_Instance.StatElm.Text+=str;
							}catch{
								console.mode=0x01;
								console.Output(q);
							}
						}
					} else if (console.mode==0x01) {
						try{
							//resetPassword.StatusElm.Text+=str;
							console.resetPassword_Instance.StatElm.Text+=str;
						}catch{
							try{
								console.resetPassword_Instance.StatElm.Text+=str;
							}catch{
								console.mode=0x10;
								console.Output(q);
							}
						}
					} else if (console.mode==0x10) {
						//console.resetPassword_Instance.StatElm.Text+="FAILED";
						Event evt=new Event("AccountServicesWWW");
						evt.Log("Testing log message");
					}
				}
			}
		}
		// Returns an encoded HTML string.
		private static string HTMLEncode(string q=null) {
			if (!String.IsNullOrEmpty(q)) {
				if (!String.IsNullOrWhiteSpace(q)) {
					if (q.Length>0) {
						Dictionary<string,string>list=new Dictionary<string,string>();
						list.Add("[\t]+","&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
						//list.Add("[\t]+","&#9;");
						list.Add("(\n\r\f|\r\n\f|\f\r\n|\f\n\r|\n\r|\r\n)","<br>");
						list.Add("[\n]+","<br>");
						//list.Add("[\"]+","&quot;");
						//list.Add("[<]+","&lt;");
						//list.Add("[>]+","&gt;");
						foreach(var item in list){
							//Regex reg=new Regex(item.Key);
							if (Regex.IsMatch(q,item.Key)) {
								q=Regex.Replace(q,item.Key,item.Value);
							}
							/*
							if (q.IndexOf(item.Key)!=-1) {
								q.Replace(item.Key,item.Value);
							}
							*/
						}
					}
				}
			}
			return q;
		}
		// Clears the console output.
		public static void Clear() {
			console.PlacedStyle=false;
			if (console.mode==0x00) {
				try{
					Default.StatusElm.Text="";
				}catch(Exception e){
					try{
						console.Default_Instance.StatElm.Text="";
					}catch{
						console.mode=0x01;
					}
				}
			} else if (console.mode==0x01) {
				try{
					resetPassword.StatusElm.Text="";
				}catch(Exception e){
					try{
						console.resetPassword_Instance.StatElm.Text="";
					}catch{
						console.mode=0x10;
					}
				}
			} else if (console.mode==0x10) {
				console.resetPassword_Instance.StatElm.Text="";
				try{
					// Write to IIS event logs.
				}catch(Exception e){
							
				}
			}
		}
		// Returns a string representing the calling function.
		private static string GetCallingFunction() {
			string res="";
			StackTrace s=new StackTrace();
			//res+=s.GetFrame(1).GetMethod().Name+"()";
			int i=(s.FrameCount-1);
			if (i>7) {
				i=7;
			}
			int lim=i;
			while(i>2){
				if (i<lim) {
					res+="->"+s.GetFrame(i).GetMethod().Name+"()";
				} else {
					res+=s.GetFrame(i).GetMethod().Name+"()";
				}
				i--;
			}
			return res;
		}
		// Returns a string representing the location of the calling function.
		private static string GetCallingFunctionPath() {
			string res="";
			StackTrace s=new StackTrace();
			var tmp=s.GetFrame(3).GetMethod();
			if (tmp.DeclaringType==null) {
				res=tmp.Name;
			} else {
				res=tmp.DeclaringType.Name;
			}
			//return s.GetFrame(3).GetFileName();
			return res;
		}
		// Returns an integer representing the line number of the calling function.
		private static int GetCallingFunctionLineNumber() {
			StackTrace s=new StackTrace();
			return s.GetFrame(3).GetFileLineNumber();
		}
		// Returns true if the parameter is an acceptable value, false otherwise.
		private static bool CheckValue(string value=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(value)) {
				if (!String.IsNullOrWhiteSpace(value)) {
					res=true;
				}
			}
			return res;
		}
		// Returns a sanitized string.
		public static string sanitize(string q) {
			string patt=@"[^A-z0-9_\-~`!@#$%\^&\*\(\)\+=\{\[\}\]|\\'\:;\,\.<>\?\/ \t"+"\""+"]+";
			if (Regex.Match(q,patt).Success) {
				q=Regex.Replace(q,patt,"");
			}
			return q;
		}
		// Returns a string representing the current date and time.
		public static string getTime() {
			var dt=new DateTime(DateTime.Now.Ticks);
			return dt.ToString("MM-dd-yyyy | hh:mm:ss tt");
		}
	}
}