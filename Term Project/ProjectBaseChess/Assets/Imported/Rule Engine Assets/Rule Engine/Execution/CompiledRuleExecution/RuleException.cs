using System;

namespace RuleEngine {

    public class RuleException : Exception {

        public enum Severity {
            Error,
            Skipped
        }

        private Severity severity;

        public RuleException(string message, Severity severity) : base(message) {
            this.severity = severity;
        }
        
        public Severity GetSeverity() {
            return severity;
        }

    }

}
