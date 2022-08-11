using System.Collections.Generic;

namespace VLAB_AccountServices.services.assets.classes.UserCheck
{
	public class CampusData
	{
		private static bool Ready=false;
		private static Dictionary<string,string> Names=new Dictionary<string,string>();
		private static Dictionary<string,string> Location=new Dictionary<string,string>();
		private static Dictionary<string,string> CampusKeys=new Dictionary<string,string>();
		public static void Ini()
		{
			if(!Ready)
			{
				Setup();
			}
		}
		/// <summary>
		/// Gets the campus name from the abbreviation/campus code.
		/// </summary>
		/// <param name="code"></param>
		/// <returns>A <see cref="string">string</see> representation of the campus name.</returns>
		public static string GetCampusName(string code)
		{
			Ini();
			string res=null;
			if(Names.ContainsKey(code))
				res=Names[code];
			return res;
		}

		private static void Setup()
		{
			Names.Clear();
			Location.Clear();
			CampusKeys.Clear();
			SetupCK();
			SetupLocations();
			SetupNames();
		}

		private static void SetupCK()
		{
			CampusKeys.Add("cc","community college");
			CampusKeys.Add("ha","hawaii cc");
			CampusKeys.Add("hi","uh hilo");
			CampusKeys.Add("ho","honolulu cc");
			CampusKeys.Add("ka","kapiolani cc");
			CampusKeys.Add("ku","kauai cc");
			CampusKeys.Add("le","leeward cc");
			CampusKeys.Add("ma","uh manoa");
			CampusKeys.Add("mu","maui cc");
			CampusKeys.Add("sw","uh system");
			CampusKeys.Add("wi","windward cc");
			CampusKeys.Add("wo","uh west oahu");
		}

		private static void SetupLocations()
		{
			Location.Add("mcc","maui");
			Location.Add("mauicc","maui");
			Location.Add("uhsystem","unknown");
			Location.Add("uhm","oahu");
			Location.Add("uhh","hawaii");
			Location.Add("uhwo","oahu");
			Location.Add("hcc","oahu");
			Location.Add("kcc","oahu");
			Location.Add("lcc","oahu");
			Location.Add("wcc","oahu");
			Location.Add("kauaicc","kauai");
			Location.Add("hawcc","hawaii");
			Location.Add("uh","unknown");
			Location.Add("rcuh","unknown");
			Location.Add("uhf","unknown");
			Location.Add("uhs","unknown");
			Location.Add("ewc","unknown");
			Location.Add("other","unknown");
		}

		private static void SetupNames()
		{
			Names.Add("mcc","maui");
			Names.Add("mauicc","maui");
			Names.Add("uhsystem","system");
			Names.Add("uhm","manoa");
			Names.Add("uhh","hilo");
			Names.Add("uhwo","west oahu");
			Names.Add("hcc","honolulu");
			Names.Add("kcc","kapiolani");
			Names.Add("lcc","leeward");
			Names.Add("kauaicc","kauai");
			Names.Add("hawcc","hawaii");
			Names.Add("uh","unknown");
			Names.Add("rcuh","uh research corporation");
			Names.Add("uhf","uh foundation");
			Names.Add("uhs","laboratory school");
			Names.Add("ewc","east west center");
			Names.Add("other","other");
		}

	}
}