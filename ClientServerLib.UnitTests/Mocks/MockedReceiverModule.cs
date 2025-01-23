using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib.UnitTests.Mocks
{
	public class MockedReceiverModule : IReceiverModule
	{
		public event MessageReceivedEventHandler? MessageReceived;

		public MockedReceiverModule() 
		{ 
		}

		public void FireMessageReceived(string Content)
		{
			if (MessageReceived != null) MessageReceived(Content);
		}

	}
}
