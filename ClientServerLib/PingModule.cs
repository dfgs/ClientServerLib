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
	public class PingModule : SessionModule
	{
		private ISenderModule senderModule;
		private IMessageManagerModule messageManagerModule;
		public PingModule(ILogger Logger, Session Session, ISenderModule SenderModule, IMessageManagerModule MessageManagerModule) : base(Logger,Session)
		{
			this.senderModule = SenderModule;
			this.messageManagerModule = MessageManagerModule;
		}


		protected override void ThreadLoop()
		{
			WaitHandle? result;
			while (State == ModuleStates.Started)
			{
				Log(Message.Debug($"Waiting {Session.PingPeriod}s before ping"));
				result = WaitHandles(Session.PingPeriod*1000, QuitEvent);
				if (result == QuitEvent) break;
				Log(Message.Information($"Sending ping to remote"));
				senderModule.Enqueue("ping");
			}
		}




	}
}
