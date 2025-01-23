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
			ILogger logger;

			MessageManagerModule module;
			MockedReceiverModule receiverModule;
			ISessionMessageDeserializer sessionMessageDeserializer;

			SessionMessage? sessionMessage = null;

			logger = new ConsoleLogger(new DefaultLogFormatter());
			
			receiverModule = new MockedReceiverModule();
			sessionMessageDeserializer = new SessionMessageDeserializer(logger);

			module = new MessageManagerModule(logger, new MockedSession(), receiverModule,sessionMessageDeserializer );
			module.PingReceived += (message) => { sessionMessage = message; };

			receiverModule.FireMessageReceived($$"""<?xml version="1.0" encoding="utf-16"?><SessionMessage MessageID="1" Method="Ping" MessageType="Request" />""");

			Assert.IsNotNull(sessionMessage);
			Assert.AreEqual("Ping", sessionMessage.Method);

		}
	}
}