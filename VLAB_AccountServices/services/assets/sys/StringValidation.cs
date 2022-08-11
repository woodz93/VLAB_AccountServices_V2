using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices
{
	public static class StringValidation
	{
		/// <summary>
		/// Gets the number of characters within the string value.
		/// </summary>
		/// <param name="str"></param>
		/// <returns>A <see cref="uint">unsigned-integer</see> representing the number of characters within the string value.</returns>
		public static uint Count(this string str)
		{
			return (uint)str.Length;
		}
		/// <summary>
		/// Checks if the string value is practical for use.
		/// </summary>
		/// <param name="str"></param>
		/// <returns>A <see cref="bool">boolean</see> value determing if the string's value is practical for use.</returns>
		public static bool CheckValue(this string str)
		{
			bool res=false;
			if(!String.IsNullOrEmpty(str))
				res=str.Trim().Length>0 ? true : false;
			return res;
		}

	}
}