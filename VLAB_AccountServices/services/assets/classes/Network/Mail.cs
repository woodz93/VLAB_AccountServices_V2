using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using VLAB_AccountServices.services.assets.sys;

namespace VLAB_AccountServices {
	public class Mail {

		private static readonly string SMTP_CLIENT="mr1.maui.hawaii.local";
		private static readonly int SMTP_CLIENT_PORT=587;
		private static readonly string USERNAME="";
		private static readonly string PASSWORD="";

		private string From=null;
		private string Destination_Email=null;
		private string Message=null;
		private string Subject=null;
		private bool ini_complete=false;
		private List<Attachment>Attachments=new List<Attachment>();

		public Mail() {

		}
		public Mail(string destination=null) {
			this.SetDestination(destination);
			this.ini();
		}

		private void ini() {
			this.ini_complete=true;
		}
		// Processes uploaded files as attachments.
		public void ProcessFiles(HttpFileCollection files=null) {
			if (files!=null) {
				if (files.Count>0) {
					int i=0;
					while(i<files.Count){
						HttpPostedFile pf=files[i];
						Attachment tmp=new Attachment(files[i].InputStream,files[i].FileName);
						this.AddAttachment(tmp);
						i++;
					}
				}
			}
		}
		// Sets the subject of the message.
		public void SetSubject(string sub=null) {
			if (Mail.CheckValue(sub)) {
				this.Subject=sub;
			}
		}
		// Sets the message content of the message.
		public void SetMessage(string msg=null) {
			if (Mail.CheckValue(msg)) {
				this.Message=msg;
			}
		}
		// Sets the source email.
		public void SetFrom(string src=null) {
			if (Mail.CheckValue(src)) {
				this.From=src;
			}
		}
		// Adds an attachment to be sent with the email.
		public void AddAttachment(Attachment file=null) {
			if (file!=null) {
				this.Attachments.Add(file);
			}
		}
		// Clears all attachments.
		public void ClearAttachments() {
			this.Attachments.Clear();
		}
		// Sends an email.
		public void Send() {
			this.OutputSend();
		}
		// Sends an email.
		public void Send(string msg=null) {
			this.SetMessage(msg);
			this.Send();
		}
		// Sends an email.
		public void Send(string sub=null,string msg=null) {
			this.SetSubject(sub);
			this.SetMessage(msg);
			this.Send();
		}
		// Actually sends out the email.
		private void OutputSend() {
			if (this.ini_complete) {
				if (Mail.CheckValue(this.Destination_Email) && Mail.CheckValue(this.Message)) {
					string sub="";
					if (Mail.CheckValue(this.Subject)) {
						sub=this.Subject;
					}
					try{
						MailMessage mm=new MailMessage(this.Destination_Email,this.From,sub,this.Message);
						if (this.Attachments.Count>0) {
							int i=0;
							while(i<this.Attachments.Count){
								mm.Attachments.Add(this.Attachments[i]);
								i++;
							}
						}
						try{
							//SmtpClient client=new SmtpClient(Mail.SMTP_CLIENT,Mail.SMTP_CLIENT_PORT);
							SmtpClient client=new SmtpClient(Mail.SMTP_CLIENT);
							client.EnableSsl=true;
							client.DeliveryMethod=SmtpDeliveryMethod.Network;
							client.UseDefaultCredentials=false;
							//client.Credentials=new System.Net.NetworkCredential(Mail.USERNAME,Mail.PASSWORD);
							client.Send(mm);
							console.Info("Successfully sent an email to the user/client.");
						}catch(Exception e){
							console.Error("Failed to connect to SMTP client...\n\t\t"+e.Message);
						}
					}catch(Exception e){
						console.Error(e.Message);
					}
				}
			}
		}


		public void SetDestination(string destination=null) {
			if (Mail.CheckValue(destination)) {
				this.Destination_Email=destination;
			}
		}

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