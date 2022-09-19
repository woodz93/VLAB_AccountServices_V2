using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VLAB_AccountServices.services.assets.classes.Network;
namespace VLAB_AccountServices.services.assets.sys {
	public class ConsoleOutput {
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
			if (!ConsoleOutput.ini_complete) {
				ConsoleOutput.Default_Instance = instance;
				ConsoleOutput.mode=0x00;
				ConsoleOutput.ini_complete=true;
				ConsoleOutput.errored=false;
				ConsoleOutput.PlacedStyle=false;
				//instance.StatElm.Text+="Begun on Default<br>";
			}
		}
		// Initializes the console class for use with the resetPassword class.
		public static void ini(resetPassword instance) {
			if (!ConsoleOutput.ini_complete) {
				ConsoleOutput.resetPassword_Instance = instance;
				ConsoleOutput.mode=0x01;
				ConsoleOutput.ini_complete=true;
				ConsoleOutput.errored=false;
				ConsoleOutput.PlacedStyle=false;
			}
		}
		// Initializes the console class for use with the Network class.
		public static void ini(Network instance) {
			if (!ConsoleOutput.ini_complete) {
				ConsoleOutput.Network_Instance = instance;
				ConsoleOutput.mode=0x0F;
				ConsoleOutput.ini_complete=true;
				ConsoleOutput.errored=false;
				ConsoleOutput.PlacedStyle=false;
			}
		}
		// Initializes the console class for use with the sys class.
		public static void ini(sys instance) {
			if (!ConsoleOutput.ini_complete) {
				ConsoleOutput.sys_Instance = instance;
				ConsoleOutput.mode=0x0F;
				ConsoleOutput.ini_complete=true;
				ConsoleOutput.errored=false;
				ConsoleOutput.PlacedStyle=false;
			}
		}
		// Writes output to the client.
		public static void Log(string message) {
			if (ConsoleOutput.debugging_mode>=0x111) {
				ConsoleOutput.Output("<font style='color:rgb(255,255,255);' class='debug-title'>[LOG]</font>:<font style='color:rgb(255,255,255);' class='debug-text'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Warn(string message) {
			if (ConsoleOutput.debugging_mode>=0x100) {
				ConsoleOutput.Output("<font style='color:rgb(255,200,50);' class='debug-title'>[WARN]</font>:<font style='color:rgb(255,200,50);' class='debug-text'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Error(string message) {
			if (ConsoleOutput.debugging_mode>=0x011) {
				ConsoleOutput.Output("<font style='color:red;' class='debug-title'>[ERROR]</font>:<font style='color:red;' class='debug-text'>\t\t"+message+"</font>");
				if (!ConsoleOutput.errored) {
					ConsoleOutput.errored=true;
				}
			}
		}
		// Writes output to the client.
		public static void Info(string message) {
			if (ConsoleOutput.debugging_mode>=0x110) {
				ConsoleOutput.Output("<font style='color:cyan;' class='debug-title'>[INFO]</font>:<font style='color:cyan;' class='debug-text'>\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Success(string message) {
			if (ConsoleOutput.debugging_mode>=0x101) {
				ConsoleOutput.Output("<font style='color:rgb(100,255,100);' class='debug-title'>[SUCCESS]</font>:<font style='color:rgb(100,255,100);' class='debug-text'>\t"+message+"</font>");
			}
		}
		private static void Output(string q=null) {
			if (ConsoleOutput.enable_output) {
				if (ConsoleOutput.CheckValue(q)) {
					string str="["+ConsoleOutput.getTime()+"] "+q+"\n\t\t[<font style='color:rgb(255,50,150);'>"+ConsoleOutput.GetCallingFunctionPath()+"</font>|"+ConsoleOutput.GetCallingFunction()+" (line: <font style='color:rgb(200,150,0);'>"+ConsoleOutput.GetCallingFunctionLineNumber()+"</font>)]\n";
					str=ConsoleOutput.HTMLEncode(str);
					str="<div class=\"debug\">"+str+"</div>";
					if (!ConsoleOutput.PlacedStyle) {
						str="<style>.debug{font-family:monospace !important;font-size:0.85em !important;background-color:rgba(0,0,0,0.85);color:#FFF;}.debug-title{font-weight:bolder;}.debug-text{font-weight:normal;}</style>"+str;
						ConsoleOutput.PlacedStyle=true;
					}
					if (ConsoleOutput.mode==0x00) {
						try{
							//Default.StatusElm.Text+=str;
							//console.Default_Instance.StatElm.Text+=str;
						}catch{
							try{
								ConsoleOutput.Default_Instance.StatElm.Text+=str;
							}catch{
								ConsoleOutput.mode=0x01;
								ConsoleOutput.Output(q);
							}
						}
					} else if (ConsoleOutput.mode==0x01) {
						try{
							//resetPassword.StatusElm.Text+=str;
							ConsoleOutput.resetPassword_Instance.StatElm.Text+=str;
						}catch{
							try{
								ConsoleOutput.resetPassword_Instance.StatElm.Text+=str;
							}catch{
								ConsoleOutput.mode=0x10;
								ConsoleOutput.Output(q);
							}
						}
					} else if (ConsoleOutput.mode==0x10) {
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
				if (q.Trim().Length > 0) {					
						Dictionary<string,string>list=new Dictionary<string,string>();
						list.Add("[\t]+","&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
						list.Add("(\n\r\f|\r\n\f|\f\r\n|\f\n\r|\n\r|\r\n)","<br>");
						foreach(var item in list){
							if (Regex.IsMatch(q,item.Key)) {
								q=Regex.Replace(q,item.Key,item.Value);
							}
						}					
				}
			}
			return q;
		}
		// Clears the console output.
		public static void Clear() {
			ConsoleOutput.PlacedStyle=false;
			if (ConsoleOutput.mode==0x00) {
				try{
					Default.StatusElm.Text="";
				}catch(Exception e){
					try{
						ConsoleOutput.Default_Instance.StatElm.Text="";
					}catch{
						ConsoleOutput.mode=0x01;
					}
				}
			} else if (ConsoleOutput.mode==0x01) {
				try{
					resetPassword.StatusElm.Text="";
				}catch(Exception e){
					try{
						ConsoleOutput.resetPassword_Instance.StatElm.Text="";
					}catch{
						ConsoleOutput.mode=0x10;
					}
				}
			} else if (ConsoleOutput.mode==0x10) {
				ConsoleOutput.resetPassword_Instance.StatElm.Text="";
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