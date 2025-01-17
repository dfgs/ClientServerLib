using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public abstract class SessionModule : ThreadModule, ISessionModule
	{
		public Session Session
		{
			get;
			private set;
		}

		protected SessionModule(ILogger Logger, Session Session ) : base(Logger)
		{
			this.Session = Session;
		}

		
		


	}
}
