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
		public ISession Session
		{
			get;
			private set;
		}

		protected SessionModule(ILogger Logger, ISession Session ) : base(Logger)
		{
			this.Session = Session;
		}

		
		


	}
}
