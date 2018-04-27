using System;

namespace RuleEngine {

    public class ThreadExecutor {

		private ThreadedJob Job;
		public Action Callback { get; private set; } // Used by the ThreadController

		public void SetThreadedJob(ThreadedJob Job, Action callback) {
			this.Job = Job;
			this.Callback = callback;
		}

		public void StartThreadedJob() {

			SelectiveDebug.LogThread("Starting threaded job: " + Job.Name);
			Job.Start();

			// The job should no longer be touched until HasJobFinished() returns true

		}

		public bool HasThreadedJobFinished() {

			if (Job != null) {
				if (Job.Check()) {
					SelectiveDebug.LogThread("ThreadedJob finished: " + Job.Name);
					Job = null;
					return true;
				}
			}

			return false;
		}

	}
}
