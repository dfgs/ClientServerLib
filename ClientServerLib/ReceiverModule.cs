using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{


	public class ReceiverModule : SessionModule,IReceiverModule
	{
		private StreamReader? reader=null;
		

		public event MessageReceivedEventHandler? MessageReceived;

		public ReceiverModule(ILogger Logger, Session Session) : base(Logger, Session)
		{
			reader = null;
			
		}

		protected override IResult<bool> OnStarting()
		{
			return Try(() => Session.TcpClient.GetStream()).Select(
				(s) => { reader = new StreamReader(s, Encoding.UTF8); return true; },
				(ex) => ex
			);
		}
		

		protected override void ThreadLoop()
		{
			string? message;

			if (reader == null)
			{
				Log(Message.Error("Reader is not initialized"));
				return;
			}
			while (State == ModuleStates.Started)
			{
				Log(Message.Debug("Waiting data input"));
				try
				{
					message=reader.ReadLine();
					if (message == null)
					{
						Log(Message.Warning("Empty message received, TCP client may be closed"));
						WaitHandles(1000, QuitEvent);
					}
					else if (MessageReceived != null) MessageReceived(message);
				}
				catch (IOException)
				{
					Log(Message.Warning("TCP client has been closed"));
					WaitHandles(-1, QuitEvent);
				}
				catch (Exception ex)
				{
					Log(CreateException("An error occured in data reception, waiting stop event for module", ex));
					WaitHandles(-1, QuitEvent);
				}
				
			}

		}

	}
}
