using UnityEngine;

namespace RuleEngineExamples.Chess {

    // Follows a circle in the XY-plane
	public class ParametricCircleXYPlane : ParametricCurve {

        public float Radius;
        private readonly float RadianConversionMultiplier = Mathf.PI / 180;

        public ParametricCircleXYPlane(Vector3 Start, float parametricEnd, float radius) : base(Start, parametricEnd) {
            this.Radius = radius;
        }

        public override Vector3 CalculatePointOnCurveFromParameter(float t) {
            return new Vector3(Radius * Mathf.Cos(t * RadianConversionMultiplier) + Start.x,
                                        Radius * Mathf.Sin(t * RadianConversionMultiplier) + Start.y,
                                        Start.z);
        }

    }

}