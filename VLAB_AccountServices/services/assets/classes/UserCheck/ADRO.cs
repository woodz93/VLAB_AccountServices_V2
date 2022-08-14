using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace VLAB_AccountServices.services.assets.classes.UserCheck
{
	public class ADRO
	{
		private string DataString;
		private ADROD DataObject;

		public ADROD Get()
		{
			return DataObject;
		}

		public static string Encode(ADRO obj)
		{
			string res=null;
			if(obj.IsValid())
			{
				try
				{
					res=JsonSerializer.Serialize(obj.DataObject);
				}
				catch(Exception e)
				{
					// Error output here...
				}
			}
			return res;
		}

		public string Encode()
		{
			string res=null;
			if(IsValid())
			{
				try
				{
					res=JsonSerializer.Serialize(DataObject);
				}
				catch(Exception e)
				{
					res=null;
				}
			}
			return res;
		}

		public static ADRO Decode(string str=null)
		{
			ADRO res=new ADRO();
			if(str.CheckValue())
			{
				try
				{
					res=JsonSerializer.Deserialize<ADRO>(str);
				}
				catch(Exception e)
				{
					res=null;
				}
			}
			return res;
		}

		public ADRO Decode()
		{
			ADRO res=new ADRO();
			if(DataString!=null)
			{
				try
				{
					res=JsonSerializer.Deserialize<ADRO>(DataString);
				}
				catch(Exception e)
				{
					res=null;
				}
			}
			return res;
		}

		public bool IsValid()
		{
			bool res=true;
			if(DataObject==null)
				res=false;
			return res;
		}

	}
}