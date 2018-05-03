using UnityEngine;

namespace RuleEngineExamples.Chess {

    // Follows a circle in the YZ-plane
	public class ParametricCircleYZPlane : ParametricCurve {

        private float radius;
        private readonly float RadianConversionMultiplier = Mathf.PI / 180;

        public ParametricCircleYZPlane(Vector3 Start, float parametricEnd, float radius) : base(Start, parametricEnd) {
            this.radius = radius;
        }

        public override Vector3 CalculatePointOnCurveFromParameter(float t) {
            return new Vector3(Start.x,
                               radius * Mathf.Cos(t * RadianConversionMultiplier) + Start.y,
                               radius * Mathf.Sin(t * RadianConversionMultiplier) + Start.z);
        }

    }

}