using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib.UnitTests.Mocks
{
	public class MockedSenderModule : ISenderModule
	{
		private Action<SessionMessage> sendAction;
		public MockedSenderModule(Action<SessionMessage> SendAction) 
		{ 
			this.sendAction = SendAction;
		}

		public void Enqueue(SessionMessage Message)
		{
			sendAction(Message);
		}
	}
}
