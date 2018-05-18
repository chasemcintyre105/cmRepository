using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class Timer {

		private static Dictionary<string, DateTime> timers = new Dictionary<string, DateTime>();

		public static void ClearTimer(string id) {
			if (timers.ContainsKey(id))
				timers.Remove(id);
		}

		public static void StartTimer(string id) {
			Assert.False("Timer does not already exist: " + id, timers.ContainsKey(id));
			timers.Add(id, DateTime.Now);
		}

		public static double StopTimer(string id, bool print = false) { 
			DateTime startTime;
			Assert.True("Timer already exists: " + id, timers.TryGetValue(id, out startTime));
			timers.Remove(id);

			double duration = Math.Floor((DateTime.Now - startTime).TotalMilliseconds);

			if (print)
				UnityEngine.Debug.Log("TIMERS: " + id + ": " + duration + "ms");

			return duration;
		}

	}

}