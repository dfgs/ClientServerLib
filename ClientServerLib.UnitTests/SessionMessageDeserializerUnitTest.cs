using ClientServerLib.UnitTests.Mocks;
using LogLib;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using ModuleLib;

namespace ClientServerLib.UnitTests
{
	[TestClass]
	public class SessionMessageDeserializerUnitTest
	{
		[TestMethod]
		public void ShouldDeserializeRequest()
		{
			DebugLogger logger;
			SessionMessageDeserializer module;
			string message;
			IResult<SessionMessage> result;
			
			logger = new DebugLogger();
			module = new SessionMessageDeserializer(logger);

			message = 
			$$"""
			<?xml version="1.0" encoding="utf-16"?>
			<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="1" Method="Ping" MessageType="Request" />
			""";

			result = module.Deserialize(message);
			result.Match(
				(message) =>
				{
					Assert.AreEqual(1, message.MessageID);
					Assert.AreEqual("Ping", message.Method);
					Assert.AreEqual(SessionMessageTypes.Request, message.MessageType);

				},
				(ex) => Assert.Fail());
		}

		[TestMethod]
		public void ShouldDeserializeResponse()
		{
			DebugLogger logger;
			SessionMessageDeserializer module;
			string message;
			IResult<SessionMessage> result;

			logger = new DebugLogger();
			module = new SessionMessageDeserializer(logger);

			message =
			$$"""
			<?xml version="1.0" encoding="utf-16"?>
			<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="1" Method="Ping" MessageType="Response" />
			""";

			result = module.Deserialize(message);
			result.Match(
				(message) =>
				{
					Assert.AreEqual(1, message.MessageID);
					Assert.AreEqual("Ping", message.Method);
					Assert.AreEqual(SessionMessageTypes.Response, message.MessageType);

				},
				(ex) => Assert.Fail());
		}

		[TestMethod]
		public void ShouldNotDeserializeMessageWithInvalidMessageType()
		{
			DebugLogger logger;
			SessionMessageDeserializer module;
			string message;
			IResult<SessionMessage> result;

			logger = new DebugLogger();
			module = new SessionMessageDeserializer(logger);

			message =
			$$"""
			<?xml version="1.0" encoding="utf-16"?>
			<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="1" Method="Ping" MessageType="Invalid" />
			""";

			result = module.Deserialize(message);
			result.Match(
				(message) => Assert.Fail(),
				(ex) => { }	);
		}

		[TestMethod]
		public void ShouldNotDeserializeMessageWithInvalidMessageID()
		{
			DebugLogger logger;
			SessionMessageDeserializer module;
			string message;
			IResult<SessionMessage> result;

			logger = new DebugLogger();
			module = new SessionMessageDeserializer(logger);

			message =
			$$"""
			<?xml version="1.0" encoding="utf-16"?>
			<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="Invalid" Method="Ping" MessageType="Response" />
			""";

			result = module.Deserialize(message);
			result.Match(
				(message) => Assert.Fail(),
				(ex) => { });
		}

	}
}