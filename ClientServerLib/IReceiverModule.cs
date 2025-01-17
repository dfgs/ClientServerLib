using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public delegate void MessageReceivedEventHandler(string Content);

	public interface IReceiverModule
	{
		event MessageReceivedEventHandler? MessageReceived;


	}
}
