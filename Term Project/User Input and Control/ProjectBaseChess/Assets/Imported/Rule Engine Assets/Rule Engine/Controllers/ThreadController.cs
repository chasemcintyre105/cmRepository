using System;
using System.Threading;
using System.Collections.Generic;

namespace RuleEngine {

    public class ThreadController : IController {
        
		private Queue<ThreadExecutor> executors;
		private Thread MainThread;
        
        public override void Preinit() {
            executors = new Queue<ThreadExecutor>();
			MainThread = Thread.CurrentThread;
        }

        public override void Init() {
        }
        
		void Update () {
		
			// Check if the next thread in the queue has finished
			if (executors.Count > 0 && executors.Peek().HasThreadedJobFinished()) {
				ThreadExecutor finishedThread = executors.Dequeue();
				if (finishedThread.Callback != null) {
					finishedThread.Callback.Invoke();
				}
			}

		}

		// Note that this is not thread safe and therefore should only be accessed from the main thread
		public void ExecuteThreadedJob(ThreadedJob job, Action callback = null) {
			Assert.True("ExecuteThreadedJob is being run from the main thread", MainThread == null || MainThread.Equals(Thread.CurrentThread));

			ThreadExecutor newExecutor = new ThreadExecutor();
			newExecutor.SetThreadedJob(job, callback);
			newExecutor.StartThreadedJob();
			executors.Enqueue(newExecutor);
		}

	}

}