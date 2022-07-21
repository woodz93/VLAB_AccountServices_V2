using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices
{
	public class Client
	{
		public static HttpBrowserCapabilities Data=HttpContext.Current.Request.Browser;
		/// <summary>
		/// Gets the client browser name.
		/// </summary>
		/// <returns>the <see cref="string">name</see> of the client browser.</returns>
		public static string GetBrowserName()
		{
			return Client.Data.Browser;
		}
		/// <summary>
		/// Gets the client browser type.
		/// </summary>
		/// <returns>the <see cref="string">type</see> of browser the client is using.</returns>
		public static string GetBrowserType()
		{
			return Client.Data.Type;
		}
		/// <summary>
		/// Gets the client's browser version.
		/// </summary>
		/// <returns>the <see cref="string">version</see> of the client's browser.</returns>
		public static string GetBrowserVersion()
		{
			return Client.Data.Version;
		}
		/// <summary>
		/// Gets the client's platform/operating system.
		/// </summary>
		/// <returns>the client's <see cref="string">platform</see>.</returns>
		public static string GetPlatform()
		{
			return Client.Data.Platform;
		}
		/// <summary>
		/// Gets the client's IP address.
		/// </summary>
		/// <returns>the WAN <see cref="string">IP address</see> of the client network/computer.</returns>
		public static string GetIP()
		{
			return HttpContext.Current.Request.UserHostAddress;
		}
		/// <summary>
		/// Gets the client browser's user agent information.
		/// </summary>
		/// <returns>the <see cref="string">user agent</see> of the client's browser.</returns>
		public static string GetUserAgent()
		{
			return HttpContext.Current.Request.UserAgent;
		}


	}
}