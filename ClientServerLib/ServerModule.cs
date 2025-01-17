using LogLib;
using ModuleLib;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace ClientServerLib
{
	public class ServerModule : ThreadModule
	{
		private TcpListener listener;
		private int pingPeriod;
		private List<PingModule> pingModules;
		private List<SenderModule> senderModules;
		private List<ReceiverModule> receiverModules;
		private List<MessageManagerModule> messageManagerModules;

		public ServerModule(ILogger Logger,IPAddress Address, int Port, int PingPeriod):base(Logger)
		{
			listener = new TcpListener(Address, Port);
			this.pingPeriod = PingPeriod;
			pingModules = new List<PingModule>();
			senderModules = new List<SenderModule>();
			receiverModules = new List<ReceiverModule>();	
			messageManagerModules = new List<MessageManagerModule>();
		}

		protected override IResult<bool> OnStopping()
		{
			Log(Message.Information("Stopping ping modules"));
			foreach (PingModule module in pingModules)
			{
				module.Stop().Match(
					(_) => Log(Message.Information("Ping module stop successfully")),
					(ex) => Log(ex)
				);
			}
			Log(Message.Information("Stopping message manager modules"));
			foreach (MessageManagerModule module in messageManagerModules)
			{
				module.Stop().Match(
					(_) => Log(Message.Information("Message manager module stop successfully")),
					(ex) => Log(ex)
				);
			}

			Log(Message.Debug("Stopping tcp clients"));
			foreach (PingModule module in pingModules)
			{
				try
				{
					module.Session.TcpClient.Close();
				}
				catch { }
			}


			Log(Message.Information("Stopping listener"));
			try
			{
				listener.Stop();
			}
			catch { }

			Log(Message.Information("Stopping sender modules"));
			foreach (SenderModule module in senderModules)
			{
				module.Stop().Match(
					(_) => Log(Message.Information("Sender module stop successfully")),
					(ex) => Log(ex)
				);
			}

			Log(Message.Information("Stopping receiver modules"));
			foreach (ReceiverModule module in receiverModules)
			{
				module.Stop().Match(
					(_) => Log(Message.Information("Receiver module stop successfully")),
					(ex) => Log(ex)
				);
			}

			return Result.Success(true);

		}

		private IResult<TcpClient> WaitForConnection()
		{
			return Try(Message.Information("Waiting for new connection"), () => listener.AcceptTcpClient());
		}

		protected override void ThreadLoop()
		{
			bool success;
			Session session;
			SenderModule senderModule;
			ReceiverModule receiverModule;
			MessageManagerModule messageManagerModule;
			PingModule pingModule;

			LogEnter();

			success = Try(Message.Debug("Starting listener"), () => listener.Start()).Match(
				(_) => Log(Message.Debug("Listener started successfully")),
				(ex) => Log(ex)
			);
			if (!success) return;

			while (State == ModuleStates.Started)
			{
				success = WaitForConnection().Match(
					(tcpClient) =>
					{
						session = new Session() { PingPeriod = pingPeriod, TcpClient = tcpClient };
						senderModule = new SenderModule(Logger, session);
						receiverModule = new ReceiverModule(Logger, session);
						messageManagerModule = new MessageManagerModule(Logger, session, receiverModule);
						pingModule = new PingModule(Logger, session, senderModule, messageManagerModule);

						Log(Message.Debug("Client connected, starting modules"));
						senderModule.Start().Then(receiverModule.Start()).Then(messageManagerModule.Start()).Then(pingModule.Start()).Match(
							(_) => Log(Message.Debug("Modules started successfully")),
							(ex) => Log(ex)
						);

						if (success)
						{
							lock (pingModules)
							{
								pingModules.Add(pingModule);
							}
							lock(senderModules)
							{
								senderModules.Add(senderModule);
							}
							lock (receiverModules)
							{
								receiverModules.Add(receiverModule);
							}
							lock (messageManagerModules)
							{
								messageManagerModules.Add(messageManagerModule);
							}
						}
					},
					(ex) => { if (State == ModuleStates.Started) Log(ex); }
				);

			}



			

			


		}



	}

}
