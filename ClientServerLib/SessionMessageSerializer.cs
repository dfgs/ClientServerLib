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
	public class SessionMessageSerializer:Module,ISessionMessageSerializer
	{
		public SessionMessageSerializer(ILogger Logger) : base(Logger)
		{
		}

		public IResult<string> Serialize(SessionMessage Message)
		{

			XmlSerializer serializer = new XmlSerializer(typeof(SessionMessage));
			return Try(() =>
			{
				using (StringWriter textWriter = new StringWriter())
				{
					serializer.Serialize(textWriter, Message);
					return textWriter.ToString();
				}
			});

		}


	}
}
