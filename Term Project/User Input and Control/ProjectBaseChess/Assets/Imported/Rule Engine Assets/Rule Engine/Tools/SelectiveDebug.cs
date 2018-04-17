//#define INIT
//#define THREADS
//#define COLLISIONS
//#define EFFECTS
//#define STATE_MACHINES
//#define RULEEXECUTORS
//#define RULEEXECUTOR_VARIABLES
//#define DRAG_AND_DROP
//#define DRAG_AND_DROP_PERMITTED
//#define EXECUTION
//#define RULESET
//#define RULE_JUDGE
//#define ACTION_STACK

//#define TIMERS

using System.Diagnostics;

namespace RuleEngine {

	public class SelectiveDebug {

		[Conditional("TIMERS")] 
		public static void StartTimer(string id) {
			Timer.StartTimer(id);
		} 
		 
		[Conditional("TIMERS")] 
		public static void StopTimer(string id) { 
			Timer.StopTimer(id, true);
		}

        [Conditional("INIT")]
        public static void LogInit(string message) {
            UnityEngine.Debug.Log("INIT: " + message);
        }
        
        [Conditional("RULE_JUDGE")]
        public static void LogJudge(string message) {
            UnityEngine.Debug.Log("RULE_JUDGE: " + message);
        }

        [Conditional("ACTION_STACK")]
        public static void LogActionStack(string message) {
            UnityEngine.Debug.Log("ACTION_STACK: " + message);
        }
        
		[Conditional("THREADS")] 
		public static void LogThread(string message) { 
			UnityEngine.Debug.Log("THREADS: " + message);
		}

		[Conditional("COLLISIONS")] 
		public static void LogCollision(string message) { 
			UnityEngine.Debug.Log("COLLISIONS: " + message);
		}

		[Conditional("EFFECTS")] 
		public static void LogEffect(string message) { 
			UnityEngine.Debug.Log("EFFECTS: " + message);
		}

		[Conditional("STATE_MACHINES")] 
		public static void LogStateMachine(string message) { 
			UnityEngine.Debug.Log("STATE_MACHINES: " + message);
		}

		[Conditional("RULEEXECUTORS")] 
	    public static void LogRuleExecutor(string message) { 
			UnityEngine.Debug.Log("RULEEXECUTORS: " + message);
	    }
		
		[Conditional("RULEEXECUTOR_VARIABLES")] 
		public static void LogVariable(string message) { 
			UnityEngine.Debug.Log("RULEEXECUTOR_VARIABLES: " + message);
		}

		[Conditional("DRAG_AND_DROP")] 
		public static void LogDragDrop(string message) { 
			UnityEngine.Debug.Log("DRAG_AND_DROP: " + message);
		}

        [Conditional("DRAG_AND_DROP_PERMITTED")]
        public static void LogDragDropPermitted(string message) {
            UnityEngine.Debug.Log("DRAG_AND_DROP_PERMITTED: " + message);
        }

        [Conditional("RULESET")]
        public static void LogRuleSet(string message) {
            UnityEngine.Debug.Log("RULESET: " + message);
        }

        private static readonly string indent = "  ";
        private static string ExecutionIndent = "";

        [Conditional("EXECUTION")]
        public static void LogExecution(string message) {
            UnityEngine.Debug.Log("EXECUTION: " + ExecutionIndent + message);
        }

        [Conditional("EXECUTION")]
        public static void LogExecution(string message, RuleComponent obj) {
            UnityEngine.Debug.Log("EXECUTION: " + ExecutionIndent + message + " " + obj);
        }

        [Conditional("EXECUTION")]
        public static void ExecutionIndentAdd(int add) {
            if (add > 0) {
                for (int i = 0; i < add; i++) {
                    ExecutionIndent += indent;
                }
            } else if (add < 0) {
                if (indent.Length * -add <= ExecutionIndent.Length)
                    ExecutionIndent = ExecutionIndent.Remove(0, indent.Length * -add);
            }
        }

        [Conditional("EXECUTION")]
        public static void ResetExecutionIndent() {
            ExecutionIndent = "";
        }
        
    }

}