using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace VLAB_AccountServices.services.assets.sys {
	public class console {

		private static bool enable_output=true;
		private static uint mode=0x00;

		// Writes output to the client.
		public static void Log(string message=null) {
			console.Output("[LOG]:\t\t<font style='color:red;font-weight:bolder;'>"+message+"</font>");
		}
		// Writes output to the client.
		public static void Warn(string message=null) {
			console.Output("[WARN]:\t\t<font style='color:red;font-weight:bolder;'>"+message+"</font>");
		}
		// Writes output to the client.
		public static void Error(string message=null) {
			console.Output("[ERROR]:\t\t<font style='color:red;font-weight:bolder;'>"+message+"</font>");
		}

		private static void Output(string q=null) {
			if (console.enable_output) {
				if (console.CheckValue(q)) {
					string str="["+console.getTime()+"] ["+console.GetCallingFunctionPath()+"/"+console.GetCallingFunction()+" (line: "+console.GetCallingFunctionLineNumber()+")] "+q+"\n";
					str=console.HTMLEncode(str);
					if (console.mode==0x00) {
						try{
							Default.StatusElm.Text+=str;
						}catch(Exception e){
							console.mode=0x01;
							console.Output(q);
						}
					} else if (console.mode==0x01) {
						try{
							resetPassword.StatusElm.Text+=str;
						}catch(Exception e){
							console.mode=0x10;
							console.Output(q);
						}
					} else if (console.mode==0x10) {
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
						list.Add("\t","&nbsp;&nbsp;&nbsp;&nbsp;");
						list.Add("\n","<br>");
						list.Add("\"","&quot;");
						foreach(var item in list){
							if (q.IndexOf(item.Key)!=-1) {
								q.Replace(item.Key,item.Value);
							}
						}
					}
				}
			}
			return q;
		}
		// Returns a string representing the calling function.
		private static string GetCallingFunction() {
			StackTrace s=new StackTrace();
			return s.GetFrame(3).GetMethod().Name;
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

		private static bool CheckValue(string value=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(value)) {
				if (!String.IsNullOrWhiteSpace(value)) {
					res=true;
				}
			}
			return res;
		}







		// Returns a string representing the current date and time.
		private static string getTime() {
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