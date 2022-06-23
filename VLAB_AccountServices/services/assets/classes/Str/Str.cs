using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices.services.assets.classes.Str {
	public class Str {

		// Returns true if the string value is valid/usable.
		public static bool CheckStr(string str=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(str)) {
				if (!String.IsNullOrWhiteSpace(str)) {
					if (str.Length>0) {
						res=true;
					}
				}
			}
			return res;
		}

	}
}