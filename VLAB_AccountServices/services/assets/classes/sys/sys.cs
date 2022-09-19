using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices.services.assets.classes.sys {
	public class sys {

		public static string getCwd() {
			string res=null;
			try{
				res=Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\";
			}catch(Exception e){
				ConsoleOutput.Error("Unable to determine the current working directory of this program.");
			}
			return res;
		}
		


	}
}