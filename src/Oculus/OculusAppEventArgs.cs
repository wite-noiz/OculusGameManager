using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Oculus
{
	public class OculusAppEventArgs : EventArgs
	{
		public string EventName { get; set; }
		public OculusApp App { get; set; }
	}
}
