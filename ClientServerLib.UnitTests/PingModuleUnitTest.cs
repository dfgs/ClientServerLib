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

			Assert.AreEqual(0, logger.FatalCount+logger.ErrorCount+logger.WarningCount);
			Assert.IsTrue(session.Delay.TotalMilliseconds >= 200);
			Assert.IsTrue(session.Delay.TotalMilliseconds < 300);
			Assert.IsTrue(session.LastCommunication >= startTime.AddSeconds(1));
			Assert.IsTrue(session.LastCommunication < startTime.AddSeconds(2));
		}

		[TestMethod]
		public void ShouldRejectInvalidAnswer()
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
				SessionMessage answer;
				answer = m.ToResponse();
				answer.MessageID= m.MessageID+3;	// create invalid ID
				messageManagerModule.FirePingReceived( answer );
			});


			module = new PingModule(logger, session, senderModule, messageManagerModule);
			startTime = DateTime.Now;
			module.Start();
			Thread.Sleep(1500);
			module.Stop();

			Assert.AreEqual(1, logger.WarningCount);
			Assert.IsTrue(logger.LogsContainKeyWords(LogLevels.Warning,"Invalid message ID"));
			Assert.IsTrue(session.Delay.TotalMilliseconds == 0);
			Assert.IsTrue(session.LastCommunication == new DateTime());
		}



	}
}