
//using System.Threading;
using System.Timers;

namespace VLAB_AccountServices.services {
	public class timeout {
		public static async void set(int miliseconds=0) {
			Timer t=new Timer(miliseconds);
			t.AutoReset=false;
			//t.Elapsed+=new ElapsedEventHandler(
		}
	}
}


