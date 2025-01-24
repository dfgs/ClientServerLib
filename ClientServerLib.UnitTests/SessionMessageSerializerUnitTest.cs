using ClientServerLib.UnitTests.Mocks;
using LogLib;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using ModuleLib;

namespace ClientServerLib.UnitTests
{
	[TestClass]
	public class SessionMessageSerializerUnitTest
	{
		[TestMethod]
		public void ShouldSerializeRequest()
		{
			DebugLogger logger;
			SessionMessageSerializer module;
			SessionMessage message;
			IResult<string> result;
			
			logger = new DebugLogger();
			module = new SessionMessageSerializer(logger);
			
			message=new SessionMessage() { MessageID = 1,MessageType=SessionMessageTypes.Request, Method="Ping" };
			result = module.Serialize(message);
			result.Match(
			(content) => 
				Assert.AreEqual(
					$$"""
					<?xml version="1.0" encoding="utf-16"?>
					<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="1" Method="Ping" MessageType="Request" />
					"""
					,content) ,
				(ex) => Assert.Fail());
		}

		[TestMethod]
		public void ShouldSerializeResponse()
		{
			DebugLogger logger;
			SessionMessageSerializer module;
			SessionMessage message;
			IResult<string> result;

			logger = new DebugLogger();
			module = new SessionMessageSerializer(logger);

			message = new SessionMessage() { MessageID = 1, MessageType = SessionMessageTypes.Response, Method = "Ping" };
			result = module.Serialize(message);
			result.Match(
			(content) =>
				Assert.AreEqual(
					$$"""
					<?xml version="1.0" encoding="utf-16"?>
					<SessionMessage xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" MessageID="1" Method="Ping" MessageType="Response" />
					"""
					, content),
				(ex) => Assert.Fail());
		}



	}
}