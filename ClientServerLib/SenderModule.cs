using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public class SenderModule : SessionModule, ISenderModule
	{
		private StreamWriter? writer;
		private readonly Queue<string> items;

		private AutoResetEvent changedEvent;
		
		public SenderModule(ILogger Logger, Session Session) : base(Logger, Session)
		{
			writer = null;

			Log(Message.Debug("Create changed event"));
			changedEvent = new AutoResetEvent(false);
			Log(Message.Debug("Create events list"));
			items = new Queue<string>();
		}

		protected override IResult<bool> OnStarting()
		{
			return Try(() => Session.TcpClient.GetStream()).Select(
				(s) => { writer=new StreamWriter(s,Encoding.UTF8); return true; },
				(ex) => ex
			);
		}
		public void Enqueue(string Message)
		{

			LogEnter();
			if (State!=ModuleStates.Started)
			{
				Log(LogLib.Message.Warning($"Module is not started, ignoring message"));
				return;
			}

			lock (this.items)
			{
                Log(LogLib.Message.Information($"Enqueue new message"));
				items.Enqueue(Message);
			}
			if (State == ModuleStates.Started) changedEvent.Set();
		}

		private IResult<bool> OnTriggerEvent(string Item)
		{
			if (writer == null) return Result.Fail<bool>(CreateException("Writer is not initialized"));

			if (!Session.TcpClient.Connected)
			{
				Log(Message.Warning("TCP client is not connected, ignoring event"));
				return Result.Success(true);
			}
			try
			{
				writer.WriteLine(Item); writer.Flush();
				return Result.Success(true);
			}
			catch (IOException)
			{
				Log(Message.Warning("TCP client has been closed"));
				return Result.Success(true);
			}
			catch (Exception ex)
			{
				return Result.Fail<bool>(CreateException("An error occured while sending data", ex));
			}



		}

		protected sealed override void ThreadLoop()
		{
			string? item;
			WaitHandle? result;

			LogEnter();

			while (State == ModuleStates.Started)
			{
				Log(Message.Debug($"Waiting for change in message queue"));
				result = WaitHandles(-1, changedEvent, QuitEvent);

				if (result == changedEvent)
				{
					Log(Message.Debug($"Message queue has changed"));
					lock (items)
					{
						if (items.Any())
						{
							item = items.Dequeue();
						}
						else
						{
							Log(Message.Warning($"Message queue is empty"));
							continue;
						}
						Log(Message.Debug($"Triggering event"));
						OnTriggerEvent(item).Match(
							(_) => { },
							(ex) => Log(ex)
						);
					}
				}
				



			}
		}

		
	}
}
