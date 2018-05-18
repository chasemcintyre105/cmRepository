using System;
using System.Threading;
using UnityEngine;

namespace RuleEngine {

	public abstract class ThreadedJob {

		private bool _IsDone = false;
		private object _doneLock = new object();
		private Thread thread = null;
		
		protected abstract void OnStart(); // Thread function
		protected abstract void OnFinish(); // Executed by the main thread once finished

		public string Name;

		public ThreadedJob(string Name) {
			this.Name = Name;
		}

		public bool IsDone {
			get {
				bool tmp;
				lock (_doneLock) {
					tmp = _IsDone;
				}
				return tmp;
			}
			set {
				lock (_doneLock) {
					_IsDone = value;
				}
			}
		}
		
		public void Start() {
			thread = new Thread(Run);
			thread.Start();
		}

		public void Abort() {
			thread.Abort();
		}

		public bool Check() {
			if (IsDone) {
				OnFinish();
				return true;
			}
			return false;
		}

		private void Run() {
			try {
				OnStart();
			} catch (Exception e) { // Catch exceptions from the new thread
				Debug.LogError(e.Message + " (secondary thread)\n" + e.StackTrace);
			}
			IsDone = true;
		}

		public void RunJobInCurrentThread() {
			OnStart();
			OnFinish();
		}

	}
}