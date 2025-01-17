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

		public event MessageReceivedEventHandler? PingReceived;

		public MessageManagerModule(ILogger Logger, Session Session, IReceiverModule ReceiverModule ) : base(Logger,Session)
		{
			this.receiverModule = ReceiverModule;
			this.receiverModule.MessageReceived += ReceiverModule_MessageReceived;
		}

		private void ReceiverModule_MessageReceived(string Content)
		{
			Log(Message.Debug($"Processing new message: {Content}"));
			if ((Content == "Ping") && (PingReceived != null)) PingReceived(Content);
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
