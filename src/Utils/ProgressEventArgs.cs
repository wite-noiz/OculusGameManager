using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Utils
{
	public class ProgressEventArgs : EventArgs
	{
		public long Total { get; set; }
		public long Progress { get; set; }
		public bool IsComplete { get; set; }

		public int ToPercent()
		{
			int pcnt;
			if (IsComplete)
			{
				pcnt = 100;
			}
			else if (Progress >= Total && !IsComplete)
			{
				pcnt = 99;
			}
			else
			{
				pcnt = (int)((100.0 / Total) * Progress);
				if (Progress > 0 && pcnt < 1)
				{
					pcnt = 1;
				}
			}
			return pcnt;
		}
	}
}
