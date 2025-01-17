using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public delegate void SessionMessageReceivedEventHandler(SessionMessage Message);

	public interface IMessageManagerModule
	{
		event SessionMessageReceivedEventHandler? PingReceived;


	}
}
