using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices.Controllers.assets.data
{
	public class FData
	{
		public Dictionary<string,string>Data=new Dictionary<string,string>();

		public void Add(string key,string value)
		{
			if(!Data.ContainsKey(key))
				Data.Add(key, value);
			else
				Data[key] = value;
		}
	}
}