﻿using System;
using System.Text.RegularExpressions;

namespace VLAB_AccountServices.services.assets.sys {
	public class sys {

		public static string debug_buffer="";
		protected static bool output_flush=false;
		protected static bool logging=true;
		public static bool errored=false;
		public static string buffer="";
		// Returns the converted output buffer string into an HTML-formatted string.
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
		// Adds an error to the output buffer.
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
		// Adds a warning message to the output buffer.
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
		// Clears the output buffer.
		public static void clear() {
			sys.debug_buffer="";
			if (sys.output_flush) {
				sys.output_flush=false;
			}
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
		// Returns the converted output buffer into HTML-formatted string.
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
		// Writes a string to the status element (Works only on the Default page).
		public static void Write(string q) {
			if (sys.logging) {
				if (!String.IsNullOrEmpty(q)) {
					if (!String.IsNullOrWhiteSpace(q)) {
						Default.StatusElm.Text+=q+"<br>";
					}
				}
			}
		}
		// Returns a sanitized string.
		public static string sanitize(string q) {
			string exp="[^\\u0020-\\u007e]+";					// Matches all characters that are beyond the scope of the ASCII keyboard characters.
			if (Regex.IsMatch(q,exp)) {
				q=Regex.Replace(q,exp,"");
			}
			exp="[\\u0027\\u005c]+";							// Matches characters that would be able to escape the SQL string.
			if (Regex.IsMatch(q,exp)) {
				q=Regex.Replace(q,exp,"");
			}
			return q;
		}
	}
}