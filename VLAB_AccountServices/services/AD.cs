using System.DirectoryServices.AccountManagement;

namespace VLAB_AccountServices.services {
	internal class AD {
		public static string domain_name="maui.hawaii.local";
		public static string org_unit="OU=maui.hawaii";
		public static string domain_component="DC=maui.hawaii";
		// Returns true if the username exists within the active directory, false otherwise.
		public static bool userExists(User obj) {
			bool res=false;
			if (AD.isset(obj,"username")) {
				res=AD.user_exists(obj.username);
			}
			return res;
		}
		// Returns true if the username exists within the active directory, false otherwise.
		private static bool user_exists(string username) {
			bool res=false;
			using (var ctx=new PrincipalContext(ContextType.Domain,"maui.hawaii.local")) {
				try {
					using(var fu=UserPrincipal.FindByIdentity(ctx,IdentityType.SamAccountName,username)) {
						res=(fu!=null);
					}
				}catch{
					res=false;
				}
			}
			return res;
		}
		// Returns true if the property name exists within the object.
		private static bool isset(User obj,string key) {
			bool res=false;
			if (obj.GetType().GetProperty(key)!=null) {
				res=true;
			}
			return res;
		}

	}
}
