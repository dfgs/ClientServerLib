using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LogLib;
using ModuleLib;

namespace ClientServerLib
{
	public class MessageManagerModule : SessionModule, IMessageManagerModule
	{
		private IReceiverModule receiverModule;
		private ISessionMessageDeserializer sessionMessageDeserializer;

		public event SessionMessageReceivedEventHandler? PingReceived;

		public MessageManagerModule(ILogger Logger, ISession Session, IReceiverModule ReceiverModule, ISessionMessageDeserializer SessionMessageDeserializer) : base(Logger,Session)
		{
			this.receiverModule = ReceiverModule;
			this.sessionMessageDeserializer = SessionMessageDeserializer;
			this.receiverModule.MessageReceived += ReceiverModule_MessageReceived;
		}

		private void ReceiverModule_MessageReceived(string Content)
		{
			SessionMessage? message=null;

			Log(Message.Debug($"Processing new message: {Content}"));
			sessionMessageDeserializer.Deserialize(Content).Match((m) => message = m,(ex)=> Log(ex));
			if (message == null) return;

			if ((message.Method == "Ping") && (PingReceived != null)) PingReceived(message);
		}

		protected override void ThreadLoop()
		{
			WaitHandle? result;
			while (State == ModuleStates.Started)
			{
				Log(Message.Debug($"Waiting quit event"));
				result = WaitHandles(-1, QuitEvent);
			}
		}




	}
}
