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
		
		private IResult<string> ReadMessage()
		{
			string? line;
			string message="";

			if (reader == null) return Result.Fail<string>(CreateException("Reader is not initialized"));

			if (!Session.TcpClient.Connected)
			{
				Log(Message.Warning("TCP client is not connected"));
				return Result.Fail<string>(CreateException("TCP client is not connected"));
			}

			while (true)
			{
				try
				{
					line = reader.ReadLine();
					if (string.IsNullOrEmpty(line)) return Result.Success(message);
					message += line;
				}
				catch (IOException ex)
				{
					Log(Message.Warning("TCP client has been closed"));
					return Result.Fail<string>(ex);
				}
				catch (Exception ex)
				{
					Log(ex);
					return Result.Fail<string>(ex);
				}
			}
		}

		protected override void ThreadLoop()
		{

			if (reader == null)
			{
				Log(Message.Error("Reader is not initialized"));
				return;
			}
			while (State == ModuleStates.Started)
			{
				Log(Message.Debug("Waiting data input"));
				
				ReadMessage().Match(
					(message) =>
					{
						if (string.IsNullOrEmpty(message))
						{
							Log(Message.Warning("Empty message received, TCP client may be closed"));
							WaitHandles(1000, QuitEvent);
						}
						else if (MessageReceived != null) MessageReceived(message);
					},
					(ex) => WaitHandles(-1, QuitEvent)
				);
				
			}

		}

	}
}
