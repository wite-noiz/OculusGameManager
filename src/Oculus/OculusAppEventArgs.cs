using System;

namespace OculusGameManager.Oculus
{
    public class OculusAppEventArgs : EventArgs
	{
		public string EventName { get; set; }
		public OculusApp App { get; set; }
	}
}
