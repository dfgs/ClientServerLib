using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib.UnitTests.Mocks
{
	public class MockedSession : ISession
	{
		public int SessionID { get; set; }

		public int PingPeriod { get;set; }
		public TimeSpan Delay { get; set; }
		public DateTime LastCommunication { get; set; }

		public bool IsConnected => true;

		public MockedSession()
		{

		}

		public Stream GetStream()
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
		}
	}
}
