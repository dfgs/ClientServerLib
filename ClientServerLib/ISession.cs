using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{

	
	public interface ISession
	{

		int SessionID
		{
			get; 
			
		}
		
		int PingPeriod 
		{
			get;
			set;
		}

		TimeSpan Delay
		{
			get;
			set;
		}

		DateTime LastCommunication
		{
			get;
			set;
		}

		bool IsConnected
		{
			get;
		}

		Stream GetStream();
		void Close();

		
	}
}
