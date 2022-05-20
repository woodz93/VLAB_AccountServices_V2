using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLAB_AccountServices.services {
	internal class User {
		public string cmd { get; set; }				// The action/command that will dictate what to do with the data. (See read me for documentation on what to send).
		public string username { get; set; }		// The username to set within the active directory.
		public string password { get; set; }		// The password to set within the active directory.
		public string uid { get; set; }				// The user id of the user.
		public string[] groups { get; set; }		// The associative groups the user will or already does belong to.
		public bool ad_exists { get; set; }			// Does the user exist in the active directory?
		public string algo { get; set; }			// The password hashing algorithm to use within the active directory.
		public string db_id { get; set; }			// The database reference id to/for the user that is being processed.
		public string new_username{get;set; }

	}
}
