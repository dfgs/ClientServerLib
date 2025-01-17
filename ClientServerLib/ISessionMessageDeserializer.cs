using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public interface ISessionMessageDeserializer
	{
		IResult<SessionMessage> Deserialize(string Message);
	}
}
