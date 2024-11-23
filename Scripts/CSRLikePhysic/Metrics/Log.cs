using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Metrics
{
	public class Log
	{
		public static void AnEvent(Events theEvent, bool isAsync = true)
		{
			var data = new Dictionary<Parameters, string>();
			AnEvent(theEvent, data, isAsync);
		}

		public static void AnEvent(Events theEvent, Dictionary<Parameters, string> data, bool isAsync = true)
		{
			// if (isAsync)
			// 	new Thread(() => MetricsIntegration.Instance.LogAnEvent_Internal(theEvent, data)).Start();
			// else
			{
				//Debug.Log("Log Events/ " + theEvent.ToString());
				MetricsIntegration.Instance.LogAnEvent_Internal(theEvent, data);
			}
		}
	}
}
