using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using S22.Imap;
using Troopy;

namespace Troopy.Mail
{
	public class IMAPClient
	{
		// Credentials
		private string hostname;
		public string Hostname{
			get{return this.hostname;}
			set{this.hostname = value;}
		}
		
		private int port;
		public int Port{
			get{return this.port;}
			set{this.port = value;}
		}		
		
		private string username;
		public string Username{
			get{return this.username;}
			set{this.username = value;}
		}				
		
		private string password;
		public string Password{
			get{return this.password;}
			set{this.password = value;}
		}						

		private bool ssl;
		public bool Ssl{
			get{return this.ssl;}
			set{this.ssl = value;}
		}			
		
		// Client
		protected ImapClient client;
		
		private bool idleMode;
		public bool IdleMode {
			get {
				return this.idleMode;
			}
			set {
				if(this.idleMode && !value) {
				
					client.NewMessage -= new EventHandler<IdleMessageEventArgs>(onNewMessage);
					this.idleMode = false;
				
				} else if(!this.idleMode && value) {
				
					if(client.Supports("IDLE") == false) {
						this.idleMode = false;
					} else {
						client.NewMessage += new EventHandler<IdleMessageEventArgs>(onNewMessage);
						this.idleMode = true;
					}					
				}
			}
		}

		// Messages		
		protected bool receivedFlag;
		
		// Status
		private bool connected;
		public bool Connected {
			get {
				return this.connected;
			}
		}

		// Event on Mail Connection lost
		public event EventHandler onConnectionLost;
		
		//
		// Constructor
		//
		public IMAPClient()
		{
		}
		
		public void connect()
		{
			try {
				this.client = new ImapClient(
					this.Hostname,
					this.Port,
					this.Username,
					this.Password,
					AuthMethod.Login,
					this.Ssl								
				);

				this.client.IdleError += new EventHandler<IdleErrorEventArgs>(connectionLost);
				this.connected = true;
								
			} catch(System.Net.Sockets.SocketException ex) {
				throw new MailException("Server neodpověděl");
			} catch(InvalidCredentialsException ex) {
				throw new MailException("Chybné přihlašovací údaje");
			} catch(System.IO.IOException ex) {
				throw new MailException("Neočekávaná forma dat, zkuste přepnout SSL");
			}
		}

		public void disconnect()
		{
			this.connected = false;
			try {
				this.client.Logout();
			} catch (System.IO.IOException ex) {
				throw new MailException("Odhlášení E-malového klienta se nezdrařilo, připojení bylo možná ztraceno");
			}
		}

		private void connectionLost(object sender, IdleErrorEventArgs e)
		{
			this.connected = false;

			if (this.onConnectionLost != null) {				
				this.onConnectionLost(this, EventArgs.Empty);
			}
		}
										
		private void onNewMessage(object sender, IdleMessageEventArgs e)
		{
			this.receivedFlag = true;						
		}	

		public List<string> getMailboxList()
		{
			try {
				return Conversion.IEnumString2List(this.client.ListMailboxes());
			} catch(System.IO.IOException ex) {
				throw new MailException(ex.Message);
			}
		}

		public List<string> getCapabilities()
		{
			try {
				return Conversion.IEnumString2List(this.client.Capabilities());
			} catch(System.IO.IOException ex) {
				throw new MailException(ex.Message);
			}
		}

		protected List<uint> getUidsAll(string mailbox)
		{
			return Conversion.IEnumUint2List(this.client.Search(SearchCondition.All(), mailbox));
		}
		
		protected List<uint> getUidsUnseen(string mailbox)
		{
			return Conversion.IEnumUint2List(this.client.Search(SearchCondition.Unseen(), mailbox));
		}			
	}
	

	
	public class MailException : TroopyException
	{
		public MailException(string message) : base(message)
   		{
   		}		
	}
}
