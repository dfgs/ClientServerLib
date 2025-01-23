using ClientServerLib.UnitTests.Mocks;
using LogLib;

namespace ClientServerLib.UnitTests
{
	[TestClass]
	public class PingModuleUnitTest
	{
		[TestMethod]
		public void ShouldPingAndUpdateSession()
		{
			DebugLogger logger;

			PingModule module;
			MockedSenderModule senderModule;
			MockedMessageManagerModule messageManagerModule;
			MockedSession session;
			DateTime startTime;

			logger = new DebugLogger();

			session = new MockedSession() { PingPeriod = 1 };

			messageManagerModule = new MockedMessageManagerModule();

			senderModule = new MockedSenderModule((m) => 
			{
				Thread.Sleep(200);
				messageManagerModule.FirePingReceived(m.ToResponse());
			});


			module = new PingModule(logger,session, senderModule, messageManagerModule);
			startTime= DateTime.Now;
			module.Start();
			Thread.Sleep(1500);
			module.Stop();

			Assert.IsTrue(session.Delay.TotalMilliseconds >= 200);
			Assert.IsTrue(session.Delay.TotalMilliseconds < 300);
			Assert.IsTrue(session.LastCommunication >= startTime.AddSeconds(1));
			Assert.IsTrue(session.LastCommunication < startTime.AddSeconds(2));
		}




	}
}