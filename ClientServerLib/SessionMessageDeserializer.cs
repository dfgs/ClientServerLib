using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClientServerLib
{
	public class SessionMessageDeserializer : Module, ISessionMessageDeserializer
	{
		public SessionMessageDeserializer(ILogger Logger) : base(Logger)
		{
		}

		public IResult<SessionMessage> Deserialize(string Message)
		{
			SessionMessage? sessionMessage;

			XmlSerializer serializer = new XmlSerializer(typeof(SessionMessage));
			return Try(() =>
			{
				using (StringReader textReader = new StringReader(Message))
				{
					sessionMessage = serializer.Deserialize(textReader) as SessionMessage;
					if (sessionMessage == null) throw CreateException("Invalid session message received");
					return sessionMessage;
				}
			});
		}


	}
}
