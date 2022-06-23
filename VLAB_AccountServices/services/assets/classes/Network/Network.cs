using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices.services.assets.classes.Network {
	public class Network {
		public static byte status=0x00;
		public static List<string> profiles=new List<string>();
		// Returns true if the current device is connected to the network, false otherwise.
		public static bool IsConnected() {
			return NetworkInterface.GetIsNetworkAvailable();
		}
		// Returns true if the target IP/host is reachable, false otherwise.
		public static bool IsReachable(string ip) {
			bool res=false;
			try{
				Ping p=new Ping();
				try{
					PingReply r=p.Send(ip);
					try{
						string s=r.Status.ToString().ToLower();
						if (s=="success") {
							res=true;
						}
					}catch(Exception e){
						console.Error("Failed to get the status of the ping test...\n\t\t"+e.Message);
					}
				}catch(Exception e){
					console.Error("Failed to send a ping request to the target IP/host...\n\t\t"+e.Message);
				}
			}catch(Exception e){
				//sys.error("Failed to reach \""+ip+"\"...\n"+e.Message);
				console.Error("Failed to create a new instance of the Ping class...\n\t\t"+e.Message);
				res=false;
			}
			return res;
		}
		// Asynchronously checks if the target IP/host is reachable.
		public static void IsReachableAsync(string ip) {
			//bool res=false;
			try{
				Network.status=0x00;
				Network.check_reach(ip);
			}catch(Exception e){
				//sys.error("Failed to reach \""+ip+"\"...\n"+e.Message);
				//res=false;
				console.Error("Failed to determine if the target IP/host is reachable (Async)...\n\t\t"+e.Message);
			}
		}
		// Asynchronously pings a target IP/host to check if it can be reached.
		private static async Task<bool> check_reach(string ip) {
			bool res=false;
			Ping p=new Ping();
			Task<PingReply> r=p.SendPingAsync(ip);
			Task<bool> c=Network.cr0(r);
			if (c.Equals(true)) {
				res=true;
			}
			return res;
		}
		// Returns true if the the target could be reached, false otherwise. Performs an asynchronous call redirection to the check ping method.
		private static async Task<bool> cr0(Task<PingReply> q) {
			bool res=false;
			await Network.CheckPing(q);
			if (Network.status==0x10) {
				res=true;
			}
			return res;
		}
		// Asynchronously sends a ping to the target IP/host. Sets the Network.status value. Sets 0x10 on success, 0x01 on failure.
		private static async Task<int> CheckPing(Task<PingReply> q) {
			int i=0;
			int lim=100;
			while((q.Status.ToString().ToLower()!="success") && i<lim){
				await Task.Delay(100);
				i++;
			}
			if (i>=lim) {
				Network.status=0x01;
			} else {
				Network.status=0x10;
			}
			return 1;
		}
		// Returns an object consisting of the networking properties.
		public static List<string> getNetProps() {
			List<string> res=new List<string>();
			NetworkInterface[] list=NetworkInterface.GetAllNetworkInterfaces();
			string ip="";
			foreach(NetworkInterface item in list){
				if (item.OperationalStatus.ToString().ToLower()=="up") {
					//IPInterfaceProperties sel=item.GetIPProperties();
					res.Add(item.Name.ToString());
					res.Add(item.Description);
					res.Add(item.OperationalStatus.ToString().ToLower());
					res.Add(item.Speed.ToString());
				}
				break;
			}
			return res;
		}
		// Returns the active network name.
		public static string getActiveNetworkName() {
			return Network.getNetProps()[0];
		}

	}
}