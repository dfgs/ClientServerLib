using LogLib;
using ModuleLib;
using System.Net;
using System.Net.Sockets;

namespace ClientServerLib
{
	public class ClientModule : ThreadModule
	{
		private TcpClient tcpClient;
		private int pingPeriod;
		private IPAddress address;
		int port;

		private SenderModule? senderModule=null;
		private ReceiverModule? receiverModule=null;
		private MessageManagerModule? messageManagerModule=null;
		private PingModule? pingModule=null;

		public ClientModule(ILogger Logger,IPAddress Address, int Port, int PingPeriod):base(Logger)
		{
			tcpClient = new TcpClient();
			this.pingPeriod = PingPeriod;
			this.address = Address;
			this.port = Port;

		}

		protected override IResult<bool> OnStopping()
		{
			if (pingModule != null)
			{
				Log(Message.Information("Stopping ping module"));
				pingModule.Stop().Match(
					(_) => Log(Message.Debug("Ping module stop successfully")),
					(ex) => Log(ex)
				);
			}
			if (messageManagerModule != null)
			{
				Log(Message.Information("Stopping message manager module"));
				messageManagerModule.Stop().Match(
					(_) => Log(Message.Debug("message manager module stop successfully")),
					(ex) => Log(ex)
				);
			}

			Log(Message.Debug("Stopping tcp client"));
			try
			{
				tcpClient.Close();
			}
			catch { }

			if (senderModule != null)
			{
				Log(Message.Information("Stopping sender module"));
				senderModule.Stop().Match(
					(_) => Log(Message.Debug("Sender module stop successfully")),
					(ex) => Log(ex)
				);
			}
			if (receiverModule != null)
			{
				Log(Message.Information("Stopping receiver module"));
				receiverModule.Stop().Match(
					(_) => Log(Message.Debug("Receiver module stop successfully")),
					(ex) => Log(ex)
				);
			}


			return Result.Success(true);

		}

		private void ReceiverModule_MessageReceived(string Content)
		{
			Log(Message.Debug($"Message received: {Content}"));
		}


		protected override void ThreadLoop()
		{
			bool success;
			WaitHandle? result;
			Session session;

			LogEnter();


			while (State == ModuleStates.Started)
			{

				success = Try(Message.Debug($"Connecting to {address}:{port}"), () => tcpClient.Connect(address, port)).Match(
					(Client) => Log(Message.Debug("TCP client connected successfully")),
					(ex) => Log(ex)
				);
				if (State != ModuleStates.Started) break;
					
				if (success)
				{
					session=new Session() { PingPeriod=pingPeriod, TcpClient=tcpClient };
					senderModule=new SenderModule(Logger, session, new SessionMessageSerializer(Logger));
					receiverModule=new ReceiverModule(Logger, session);
					messageManagerModule=new MessageManagerModule(Logger, session,receiverModule, new SessionMessageDeserializer(Logger) );
					pingModule = new PingModule(Logger, session,senderModule,messageManagerModule);
					
					Log(Message.Debug("Client connected, starting modules"));
					senderModule.Start().Then(receiverModule.Start()).Then(messageManagerModule.Start()).Then(pingModule.Start()).Match(
						(_) => Log(Message.Debug("Modules started successfully")),
						(ex) => Log(ex)
					);

					Log(Message.Debug("Waiting quit event"));
					result = WaitHandles(-1, QuitEvent);
				}
				else
				{
					Log(Message.Debug("Waiting 10s before reconnecting"));
					result = WaitHandles(10000,  QuitEvent);
				}
			}

			
			
			


		}



	}

}
