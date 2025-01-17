using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
	public interface ISessionModule
	{
		Session Session
		{
			get;
		}
	
	}
}
