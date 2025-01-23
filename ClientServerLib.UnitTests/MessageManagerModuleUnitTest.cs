using ClientServerLib.UnitTests.Mocks;
using LogLib;

namespace ClientServerLib.UnitTests
{
	[TestClass]
	public class MessageManagerModuleUnitTest
	{
		[TestMethod]
		public void ShouldTriggerPingReceived()
		{
			DebugLogger logger;

			MessageManagerModule module;
			MockedReceiverModule receiverModule;
			ISessionMessageDeserializer sessionMessageDeserializer;

			SessionMessage? sessionMessage = null;

			logger = new DebugLogger();
			
			receiverModule = new MockedReceiverModule();
			sessionMessageDeserializer = new SessionMessageDeserializer(logger);

			module = new MessageManagerModule(logger, new MockedSession(), receiverModule,sessionMessageDeserializer );
			module.PingReceived += (message) => { sessionMessage = message; };

			receiverModule.FireMessageReceived($$"""<?xml version="1.0" encoding="utf-16"?><SessionMessage MessageID="1" Method="Ping" MessageType="Request" />""");

			Assert.IsNotNull(sessionMessage);
			Assert.AreEqual("Ping", sessionMessage.Method);
			Assert.AreEqual(0, logger.ErrorCount+logger.FatalCount+logger.WarningCount);
		}

		[TestMethod]
		public void ShouldNotHandleInvalidMessage()
		{
			DebugLogger logger;

			MessageManagerModule module;
			MockedReceiverModule receiverModule;
			ISessionMessageDeserializer sessionMessageDeserializer;

			SessionMessage? sessionMessage = null;

			logger = new DebugLogger();

			receiverModule = new MockedReceiverModule();
			sessionMessageDeserializer = new SessionMessageDeserializer(logger);

			module = new MessageManagerModule(logger, new MockedSession(), receiverModule, sessionMessageDeserializer);
			module.PingReceived += (message) => { sessionMessage = message; };

			receiverModule.FireMessageReceived($$"""<?xml version="1.0" encoding="utf-16"?><SessionMessage MessageID="Invalid" Method="Ping" MessageType="Request" />""");

			Assert.IsNull(sessionMessage);
			Assert.AreEqual(2, logger.ErrorCount );
			Assert.IsTrue(logger.LogsContainKeyWords(LogLevels.Error,"There is an error in XML document"));

		}


	}
}