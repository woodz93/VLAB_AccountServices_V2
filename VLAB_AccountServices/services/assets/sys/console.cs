using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VLAB_AccountServices.services.assets.classes.Network;
using VLAB_AccountServices.services.assets.classes.sys;

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

		private static uint debugging_mode=0x111;								// Full debugging enabled
		//private static uint debugging_mode=0x011;								// All debugging disabled


		// Initializes the console class for use with the Default class.
		public static void ini(Default instance) {
			if (!console.ini_complete) {
				console.Default_Instance = instance;
				console.mode=0x00;
				console.ini_complete=true;
				console.errored=false;
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
			}
		}
		// Initializes the console class for use with the Network class.
		public static void ini(Network instance) {
			if (!console.ini_complete) {
				console.Network_Instance = instance;
				console.mode=0x0F;
				console.ini_complete=true;
				console.errored=false;
			}
		}
		// Initializes the console class for use with the sys class.
		public static void ini(sys instance) {
			if (!console.ini_complete) {
				console.sys_Instance = instance;
				console.mode=0x0F;
				console.ini_complete=true;
				console.errored=false;
			}
		}

		// Writes output to the client.
		public static void Log(string message) {
			if (console.debugging_mode>=0x111) {
				console.Output("<font style='color:rgb(255,255,255);font-weight:bolder;'>[LOG]</font>:<font style='color:rgb(255,255,255);font-weight:bolder;'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Warn(string message) {
			if (console.debugging_mode>=0x100) {
				console.Output("<font style='color:rgb(255,200,50);font-weight:bolder;'>[WARN]</font>:<font style='color:rgb(255,200,50);font-weight:bolder;'>\t\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Error(string message) {
			if (console.debugging_mode>=0x011) {
				console.Output("<font style='color:red;font-weight:bolder;'>[ERROR]</font>:<font style='color:red;font-weight:bolder;'>\t\t"+message+"</font>");
				if (!console.errored) {
					console.errored=true;
				}
			}
		}
		// Writes output to the client.
		public static void Info(string message) {
			if (console.debugging_mode>=0x110) {
				console.Output("<font style='color:cyan;font-weight:bolder;'>[INFO]</font>:<font style='color:cyan;font-weight:bolder;'>\t"+message+"</font>");
			}
		}
		// Writes output to the client.
		public static void Success(string message) {
			if (console.debugging_mode>=0x101) {
				console.Output("<font style='color:rgb(100,255,100);font-weight:bolder;'>[SUCCESS]</font>:<font style='color:rgb(100,255,100);font-weight:bolder;'>\t"+message+"</font>");
			}
		}

		private static void Output(string q=null) {
			if (console.enable_output) {
				if (console.CheckValue(q)) {
					string str="["+console.getTime()+"] ["+console.GetCallingFunctionPath()+"/"+console.GetCallingFunction()+" (line: "+console.GetCallingFunctionLineNumber()+")] "+q+"\n";
					str=console.HTMLEncode(str);
					str="<style>.debug{font-family:monospace;font-size:1.0em;background-color:rgba(0,0,0,0.85);color:#FFF;}</style><div class=\"debug\">"+str+"</div>";
					if (console.mode==0x00) {
						try{
							Default.StatusElm.Text+=str;
						}catch(Exception e){
							try{
								console.Default_Instance.StatElm.Text+=str;
							}catch{
								console.mode=0x01;
								console.Output(q);
							}
						}
					} else if (console.mode==0x01) {
						try{
							resetPassword.StatusElm.Text+=str;
						}catch(Exception e){
							try{
								console.resetPassword_Instance.StatElm.Text+=str;
							}catch{
								console.mode=0x10;
								console.Output(q);
							}
						}
					} else if (console.mode==0x10) {
						console.resetPassword_Instance.StatElm.Text+="FAILED";
						try{
							// Write to IIS event logs.
						}catch(Exception e){
							
						}
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
						list.Add("[\t]+","&nbsp;&nbsp;&nbsp;&nbsp;");
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
		// Returns a string representing the calling function.
		private static string GetCallingFunction() {
			string res="";
			StackTrace s=new StackTrace();
			//res+=s.GetFrame(1).GetMethod().Name+"()";
			int i=(s.FrameCount-1);
			if (i>5) {
				i=5;
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
			StackTrace s=new StackTrace();
			return s.GetFrame(3).GetFileName();
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
			string res;
			DateTime dt=new DateTime(DateTime.Now.Ticks);
			string hour;
			string minute;
			string second;
			string month;
			string day;
			string year=dt.Year.ToString();
			string sym="AM";
			if (dt.Month<10) {
				month="0"+dt.Month;
			} else {
				month=dt.Month.ToString();
			}
			if (dt.Day<10) {
				day="0"+dt.Day;
			} else {
				day=dt.Day.ToString();
			}
			if (dt.Hour<10) {
				hour="0"+dt.Hour;
			} else {
				if (dt.Hour>12) {
					int temp=((dt.Hour)-12);
					if (temp<10) {
						hour="0"+temp.ToString();
					} else {
						hour=temp.ToString();
					}
					sym="PM";
				} else {
					hour=dt.Hour.ToString();
				}
			}
			if (dt.Minute<10) {
				minute="0"+dt.Minute;
			} else {
				minute=dt.Minute.ToString();
			}
			if (dt.Second<10) {
				second="0"+dt.Second;
			} else {
				second=dt.Second.ToString();
			}
			res=month+"-"+day+"-"+year+" | "+hour+":"+minute+":"+second+" "+sym;
			return res;
		}




	}
}