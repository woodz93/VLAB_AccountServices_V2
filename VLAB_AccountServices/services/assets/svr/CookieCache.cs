using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;


namespace VLAB_AccountServices.services.assets.svr {
	public class CookieCache : resetPassword {

		public static void Set(string key, string value) {
			try{
				HttpCookie ins=new HttpCookie(key);
				ins.Name=key;
				ins.Value=value;
				ins.Expires=DateTime.Now.AddMinutes(5);
				HttpContext.Current.Response.Cookies.Add(ins);
			}catch{

			}
		}

		public bool Exists(string key) {
			bool res= false;
			//HttpCookie c;
			try{
				string[] list=Response.Cookies.AllKeys;
				if (list.Contains(key)) {
					res=true;
				}
			}catch(Exception e){

			}
			return res;
		}

	}
}