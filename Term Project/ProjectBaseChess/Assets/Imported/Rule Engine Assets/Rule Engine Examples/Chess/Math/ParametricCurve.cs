using UnityEngine;

namespace RuleEngineExamples.Chess {

	public abstract class ParametricCurve {

        public abstract Vector3 CalculatePointOnCurveFromParameter(float t);

        public Vector3 Start { get; set; }
        public Vector3 End { get; private set; }
        private float parametricEnd;

        public ParametricCurve(Vector3 Start, float parametricEnd) {
            this.Start = Start;
            this.parametricEnd = parametricEnd;
        }

        public void Initialise() {
            this.End = CalculatePointOnCurveFromParameter(parametricEnd);
        }

    }

}