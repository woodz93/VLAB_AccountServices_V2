using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace VLAB_AccountServices.services.assets.sys {
	public class sys {

		public static string debug_buffer="";
		protected static bool output_flush=false;
		protected static bool logging=true;

		public static string getBuffer() {
			string res=sys.debug_buffer;
			if (res.IndexOf("\n")!=-1) {
				res=res.Replace("\n","<br>");
			}
			if (res.IndexOf("\t")!=-1) {
				res=res.Replace("\t","&nbsp;&nbsp;&nbsp;&nbsp;");
			}
			return res;
		}

		public static void error(string q) {
			q=sys.sanitize(q);
			string time=sys.getTime();
			Console.Write("\n\n["+time+"] AD ERROR:\t");
			Console.ForegroundColor=ConsoleColor.Red;
			Console.Write(q+"\n\n");
			Console.ForegroundColor=ConsoleColor.White;
			q="["+time+"] AD ERROR:\t"+q;
			sys.debug_buffer+=q+"\n";
			if (!sys.output_flush) {
				sys.output_flush=true;
			}
		}
		public static void warn(string q) {
			q=sys.sanitize(q);
			string time=sys.getTime();
			Console.Write("\n\n["+time+"] AD WARN:\t");
			Console.ForegroundColor=ConsoleColor.Yellow;
			Console.Write(q+"\n\n");
			Console.ForegroundColor=ConsoleColor.White;
			q="["+time+"] AD WARN:\t"+q;
			sys.debug_buffer+=q+"\n";
			if (!sys.output_flush) {
				sys.output_flush=true;
			}
		}

		public static void clear() {
			sys.debug_buffer="";
			if (sys.output_flush) {
				sys.output_flush=false;
			}
		}

		public static string getTime() {
			string res="";
			DateTime dt=new DateTime(DateTime.Now.Ticks);
			string hour="";
			string minute="";
			string second="";
			string month="";
			string day="";
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

		public static void flush() {
			if (sys.logging) {
				if (sys.output_flush) {
					/*
					string path=sys.getCWD()+"logs\\";
					if (!Directory.Exists(path)) {
						Directory.CreateDirectory(path);
					}
					DateTime dt=new DateTime(DateTime.Now.Ticks);
					string tmp="";
					string month="";
					string day="";
					string year=dt.Year.ToString();
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
					path+=year+"\\";
					if (!Directory.Exists(path)) {
						Directory.CreateDirectory (path);
					}
					path+=month+"\\";
					if (!Directory.Exists(path)) {
						Directory.CreateDirectory(path);
					}
					tmp=month+"-"+day+"-"+year;
					path+=tmp+".log";
					string buffer="";
					if (!File.Exists(path)) {
						//File.Create(path);
						File.WriteAllText(path,"");
						buffer="-LOG_START-\n";
					} else {
						buffer=File.ReadAllText(path);
					}
					File.WriteAllText(path,buffer+sys.debug_buffer);
					if (sys.output_flush) {
						sys.output_flush=false;
					}
					Thread.Sleep(3000);
					*/
					Default.st.Text=sys.getBuffer();
				}
			}
		}

		// Returns the current directory of this program.
		public static string getCWD() {
			//return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\";
			return "\\\\172.20.0.204\\www\\wwwvlabaccountservices\\";
		}

		// Returns a sanitized string.
		public static string sanitize(string q) {
			string patt=@"[^A-z0-9_\-~`!@#$%\^&\*\(\)\+=\{\[\}\]|\\'\:;\,\.<>\?\/ \t"+"\""+"]+";
			if (Regex.Match(q,patt).Success) {
				q=Regex.Replace(q,patt,"");
			}
			return q;
		}

	}
}