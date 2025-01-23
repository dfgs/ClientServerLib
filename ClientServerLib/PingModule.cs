using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LogLib;
using ModuleLib;

namespace ClientServerLib
{
	public class PingModule : SessionModule
	{
		private ISenderModule senderModule;
		private IMessageManagerModule messageManagerModule;
		private int lastMessageID = 0;
		private DateTime lastTimeStamp;

		public PingModule(ILogger Logger, ISession Session, ISenderModule SenderModule, IMessageManagerModule MessageManagerModule) : base(Logger,Session)
		{
			this.senderModule = SenderModule;
			this.messageManagerModule = MessageManagerModule;
			messageManagerModule.PingReceived += MessageManagerModule_PingReceived;
		}

		private void MessageManagerModule_PingReceived(SessionMessage SessionMessage)
		{
			SessionMessage response;

			switch (SessionMessage.MessageType)
			{
				case SessionMessageTypes.Request:
					Log(Message.Information($"Ping request received ({SessionMessage.MessageID})"));
					response = new SessionMessage() { MessageID = SessionMessage.MessageID, Method = SessionMessage.Method, MessageType = SessionMessageTypes.Response };
					response.Body = "OK";
					Log(Message.Information($"Sending ping answer to remote"));
					senderModule.Enqueue(response);
					break;
				case SessionMessageTypes.Response:
					Log(Message.Information($"Ping response received ({SessionMessage.MessageID})"));
					if (SessionMessage.MessageID!=lastMessageID)
					{
						Log(Message.Warning($"Invalid message ID, ignoring answer"));
						return;
					}
					Session.LastCommunication = DateTime.Now;
					Session.Delay = Session.LastCommunication - lastTimeStamp;
					Log(Message.Debug($"Ping delay: {Session.Delay}"));

					break;
			}
		}

		protected override void ThreadLoop()
		{
			WaitHandle? result;
			SessionMessage ping;

			while (State == ModuleStates.Started)
			{
				Log(Message.Debug($"Waiting {Session.PingPeriod}s before ping"));
				result = WaitHandles(Session.PingPeriod*1000, QuitEvent);
				if (result == QuitEvent) break;

				lastMessageID++;
				ping = new SessionMessage() { MessageID = lastMessageID, Method = "Ping" ,MessageType=SessionMessageTypes.Request };
				Log(Message.Information($"Sending ping to remote"));
				lastTimeStamp= DateTime.Now;
				senderModule.Enqueue(ping);
			}
		}




	}
}
