using System;
using System.Text.RegularExpressions;

namespace VLAB_AccountServices.services.assets.sys {
	public class sys {

		public static string debug_buffer="";
		protected static bool output_flush=false;
		protected static bool logging=true;
		public static bool errored=false;
		public static string buffer="";

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
			if (!sys.errored) {
				sys.errored=true;
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
					try{
						Default.st.Text=sys.getBuffer();
					}catch{
						sys.buffer=sys.getBuffer();
					}
				}
			}
		}

		// Returns the current directory of this program.
		public static string getCWD() {
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