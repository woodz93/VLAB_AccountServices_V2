using System;
using System.IO;

namespace VLAB_AccountServices.services {
	internal class Case {

		protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";

		// Creates a new case and returns the case reference id.
		public static string createCase(string q) {
			string res="";
			string id=Case.genID();
			string dir="C:\\Users\\sitesupport\\source\\repos\\VLAB_AccountServices\\VLAB_AccountServices\\services\\logs\\";
			//File.Create(dir+id+".json");
			File.WriteAllText(dir+id+".json",q);
			res=id;
			return res;
		}


		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected static string genID() {
			string res="";
			string id=Case.genRandID();
			int i=0;
			int lim=50;
			string file_name=id;
			while((!File.Exists("C:/Users/sitesupport/source/repos/VLAB_AccountServices/VLAB_AccountServices/services/logs/log_"+file_name+".json")) && (i<lim)){
				id=Case.genRandID();
				file_name=id;
				i++;
			}
			res=file_name;
			return res;
		}
		// Generates a randomized length of random characters to compose the record's ID on the database.
		protected static string genRandID() {
			Random r=new Random();
			int lim=r.Next(5,12);
			int i=0;
			char c;
			string res="";
			int sel=r.Next(0,100);
			int st=48;
			int en=90;
			while(i<lim){
				sel=r.Next(0,100);
				if (sel<=25) {
					st=48;
					en=57;
				} else if (sel>25 && sel<=50) {
					st=65;
					en=90;
				} else if (sel>50 && sel<75) {
					st=97;
					en=122;
				} else {
					st=95;
					en=96;
				}
				c=(char)r.Next(st,en);
				res+=c;
				i++;
			}
			return res;
		}



	}
}

