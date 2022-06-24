namespace VLAB_AccountServices.services.assets.classes.Database {
	public class DatabasePrincipal {
		// Used to indicate null/failure.
		public static uint NullPrincipal{get {return 0x00000000; } }
		// Used to select a record matching a specified condition.
		public static uint SelectPrincipal{get {return 0x00000001; } }
		// Used to update a record matching a specified ID.
		public static uint UpdatePrincipal{get {return 0x00000010; } }
		// Used to delete a record matching a specified ID.
		public static uint DeletePrincipal{get {return 0x00000011; } }
		// Used to insert a new record.
		public static uint InsertPrincipal{get {return 0x00000100; } }
		// Used to count the number of records total, or that match a specified condition.
		public static uint CountPrincipal{get {return 0x00000101; } }
		// Used to check if the record matching an ID exists.
		public static uint ExistsPrincipal{get {return 0x00000110; } }
		// Used to get the record's reference ID.
		public static uint GetIDPrincipal{get {return 0x00000111; } }
		// Used to automatically generate a unique reference ID for the record.
		public static uint GenerateUniqueIDPrincipal{get {return 0x00001000; } }
		// Used to indicate a where condition.
		public static uint WherePrincipal{get {return 0x00001001; } }
		// Used to indicate a like condition.
		public static uint LikePrincipal{get {return 0x00001010; } }
		// Used to indicate an AND statement within a condition.
		public static uint AndPrincipal{get {return 0x00001011; } }
		// Used to indicate an OR statement within a condition.
		public static uint OrPrincipal{get {return 0x00001100; } }
		// Used to indicate an IN statement within a condition.
		public static uint InPrincipal{get {return 0x00001101; } }
		// Used to indicate a BETWEEN statement within a condition.
		public static uint BetweenPrincipal{get {return 0x00001110; } }
		// Used to specify a search command/process to conduct.
		public static uint SearchPrincipal{get {return 0x00001111; } }
		// Used to remove a specific record from the database that matches the ID.
		public static uint RemoveRecordPrincipal{get {return 0x00010000; } }

	}
}