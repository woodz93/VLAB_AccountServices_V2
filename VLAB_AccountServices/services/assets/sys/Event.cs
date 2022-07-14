using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace VLAB_AccountServices.services.assets.sys {
	public class Event {

		private bool EventReady=false;
		private string Source=null;
		private static int EventID=-1;
		public static readonly string ProjectName="AccountServicesWWW";

		private static string LogPrepend=null;
		private static string LogAppend=null;

		public Event() {
			
		}

		public Event(string src=null) {
			this.SetSource(src);
		}

		private void SetLogString() {
			if (!Event.CheckValue(Event.LogPrepend)) {
				Event.LogPrepend="\n------------\n"+Event.ProjectName+"\n------------\n";
				Event.LogAppend="\n------------\n";
			}
		}

		// Sets the source name of the event.
		public void SetSource(string src=null) {
			if (Event.CheckValue(src)) {
				this.Source=src;
				this.ini();
			}
		}
		// Initializes/Prepares the event logger.
		private void ini() {
			this.EventReady=true;
		}
		// Writes a message to the event log on the IIS server.
		public void Log(string msg=null) {
			if (Event.CheckValue(this.Source)) {
				if (Event.CheckValue(msg)) {
					this.Output(EventLogEntryType.Information,msg);
				}
			}
		}
		// Writes a message to the event log on the IIS server.
		public void Warn(string msg=null) {
			if (Event.CheckValue(this.Source)) {
				if (Event.CheckValue(msg)) {
					this.Output(EventLogEntryType.Warning,msg);
				}
			}
		}
		// Writes a message to the event log on the IIS server.
		public void Error(string msg=null) {
			if (Event.CheckValue(this.Source)) {
				if (Event.CheckValue(msg)) {
					this.Output(EventLogEntryType.Error,msg);
				}
			}
		}
		// Writes a message to the event log on the IIS server.
		public void Success(string msg=null) {
			if (Event.CheckValue(this.Source)) {
				if (Event.CheckValue(msg)) {
					this.Output(EventLogEntryType.SuccessAudit,msg);
				}
			}
		}

		// Outputs to the event log.
		private void Output(EventLogEntryType type=EventLogEntryType.Information, string msg=null, string name=null, string src=null, string machine=null) {
			
			bool p=true;
			if (!Event.CheckValue(msg)) {
				p=false;
			}
			if (!Event.CheckValue(name)) {
				if (!Event.CheckValue(Event.ProjectName)) {
					name="Application";
				} else {
					name=Event.ProjectName;
				}
			}
			if (!Event.CheckValue(src)) {
				if (!Event.CheckValue(this.Source)) {
					src="Application";
				} else {
					src=this.Source;
				}
			}
			if (!Event.CheckValue(machine)) {
				machine=".";
			}
			if (p) {
				msg=Event.LogPrepend+msg+Event.LogAppend;
				if (!EventLog.Exists(src)) {
					using(EventLog con=new EventLog("Application",machine,"Application")){
						int id=this.GetID();
						//con.Log=name;
						con.WriteEntry(msg,type,id);
						con.Close();
					}
				}
			}
		}

		// Returns an available event id (integer), null otherwise.
		private int GetID() {
			int res=0;
			if (!(Event.EventID>-1)) {
				res=this.GenID();
			} else {
				res=Event.EventID;
			}
			return res;
		}
		// Returns a randomly generated numerical id representing this event's ID.
		private int GenID() {
			Random r=new Random();
			return r.Next(0,9999);
		}

		// Returns true if the parameter value is a valid string, false otherwise.
		private static bool CheckValue(string q=null) {
			bool res=false;
			if (!String.IsNullOrEmpty(q)) {
				if (q.Trim().Length>0) {
					res=true;
				}
			}
			return res;
		}
	}
}