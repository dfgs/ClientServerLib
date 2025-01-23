using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib.UnitTests.Mocks
{
	public class MockedMessageManagerModule : IMessageManagerModule
	{
		public event SessionMessageReceivedEventHandler? PingReceived;

		public void FirePingReceived(SessionMessage SessionMessage)
		{
			if (PingReceived != null) PingReceived(SessionMessage);
		}

	}
}
