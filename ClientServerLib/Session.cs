using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public class Session
	{
		private static int sid = 0;

		public int SessionID
		{
			get; 
			private set;
		}

		public required TcpClient TcpClient 
		{ 
			get;
			set; 
		}	

		public required int PingPeriod 
		{
			get;
			set;
		}

		public TimeSpan Delay
		{
			get;
			set;
		}

		public DateTime LastCommunication
		{
			get;
			set;
		}

		public Session()
		{
			sid++;SessionID = sid;
		}
		
	}
}
