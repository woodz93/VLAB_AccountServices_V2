using System;
using System.IO;
using VLAB_AccountServices.services.assets.classes.Network;
using VLAB_AccountServices.services.assets.sys;
namespace VLAB_AccountServices.services {
	internal class Case {
		protected static string db="UHMC_VLab";
		protected static string tb="vlab_pendingusers";
		protected static string db_ip="172.20.0.142";
		protected static string db_port="";
		protected static string db_username="uhmcad_user";
		protected static string db_password="MauiC0LLegeAD2252!";
		private static string case_dir="\\\\172.20.0.101\\a\\Inventory\\programs\\accountservices\\cases\\";
		private static string case_svr="172.20.0.101";
		private static string dir="/logs/";
		// Creates a new case and returns the case reference id.
		public static string createCase(string q) {
			string res="";
			string id=null;
			if (!Directory.Exists(Case.dir)) {
				Directory.CreateDirectory(Case.dir);
			}
			//string dir="C:\\Users\\sitesupport\\source\\repos\\VLAB_AccountServices\\VLAB_AccountServices\\services\\logs\\";
			try{
				if (Network.IsReachable(Case.case_svr)) {
					try{
						if (Directory.Exists(Case.case_dir)) {
							try{
								id=Case.genIDSvr();
								File.WriteAllText(Case.case_dir+id+".json",q);
							}catch{
								id=Case.genID();
								File.WriteAllText(dir+id+".json",q);
							}
						} else {
							id=Case.genID();
							File.WriteAllText(dir+id+".json",q);
						}
					}catch{
						id=Case.genID();
						File.WriteAllText(dir+id+".json",q);
					}
				} else {
					id=Case.genID();
					File.WriteAllText(dir+id+".json",q);
				}
				res=id;
			}catch(Exception e){
				ConsoleOutput.Error("Failed to create case file.\n\t\t"+e.Message);
			}
			return res;
		}

		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected static string genIDSvr() {
			string res="";
			string id=Case.genRandID();
			int i=0;
			int lim=50;
			string file_name=id;
			while((!File.Exists(Case.case_dir+"log_"+file_name+".json")) && (i<lim)){
				id=Case.genRandID();
				file_name=id;
				i++;
			}
			res=file_name;
			return res;
		}

		// ID controller method. Checks if the generated ID does not exist on the database. If it does not, then the generated ID will be returned.
		protected static string genID() {
			string res=null;
			string id=Case.genRandID();
			int i=0;
			int lim=50;
			string file_name=id;
			if (Directory.Exists(Case.dir)) {
				while((!File.Exists(Case.dir+"log_"+file_name+".json")) && (i<lim)){
					id=Case.genRandID();
					file_name=id;
					i++;
				}
				res=file_name;
			} else {
				ConsoleOutput.Error("Directory does not exist.");
			}
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

