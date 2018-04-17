#define DEBUG

using System; 
using System.Diagnostics;

namespace RuleEngine {

	public class Assert { 

	    [Conditional("DEBUG")] 
	    public static void True(string message, bool condition) { 
	        if (!condition) 
	            throw new Exception("Assert failed: " + message); 
	    }
		
		[Conditional("DEBUG")] 
		public static void False(string message, bool condition) { 
			if (condition) 
				throw new Exception("Assert failed: " + message); 
		}

	    [Conditional("DEBUG")]
	    public static void Null(string message, object obj) { 
	        if (obj != null) 
	            throw new Exception("Assert failed: " + message);  
	    }

	    [Conditional("DEBUG")]
	    public static void NotNull(string message, object obj) { 
	        if (obj == null) 
	            throw new Exception("Assert failed: " + message); 
	    }
		
		[Conditional("DEBUG")]
		public static void Is<T>(string message, object obj) { 
			if (obj == null || typeof(T) != obj.GetType()) 
				throw new Exception("Assert failed: " + message + ((obj == null) ? " (Object is null)" : "")); 
		}
		
		[Conditional("DEBUG")]
		public static void Never(string message) { 
			throw new Exception("Assert.Never: " + message); 
		}
		
		[Conditional("DEBUG")]
		public static void Same(string message, System.Object obj1, System.Object obj2) { 
			if (!obj1.Equals(obj2)) 
				throw new Exception("Assert failed: " + message); 
		}
		
		[Conditional("DEBUG")]
		public static void Different(string message, System.Object obj1, System.Object obj2) { 
			if (obj1.Equals(obj2)) 
				throw new Exception("Assert failed: " + message); 
		}

	}

}