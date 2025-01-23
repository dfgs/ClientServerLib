using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClientServerLib
{
	public class SessionMessage
	{
		[XmlAttribute]
		public required int MessageID
		{
			get;
			set;
		}

		[XmlAttribute]
		public required string Method
		{
			get;
			set;
		}

		[XmlAttribute]
		public required SessionMessageTypes MessageType
		{
			get;
			set;
		}


		public string? Body 
		{ 
			get; 
			set; 
		}

		public SessionMessage ToResponse()
		{
			return new SessionMessage() { MessageID = MessageID, MessageType = SessionMessageTypes.Response, Method = Method };
		}

	}
}
