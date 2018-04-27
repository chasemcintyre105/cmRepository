using UnityEngine;

namespace RuleEngineExamples.Chess {

    // Follows an ellipse in the XY-plane
	public class ParametricEllipse : ParametricCurve {

        private float radiusX;
        private float radiusY;

        public ParametricEllipse(Vector3 Start, float parametricEnd, float radiusX, float radiusY) : base(Start, parametricEnd) {
            this.radiusX = radiusX;
            this.radiusY = radiusY;
        }

        public override Vector3 CalculatePointOnCurveFromParameter(float t) {
            return new Vector3(radiusX * Mathf.Cos(t) + Start.x,
                               radiusY * Mathf.Cos(t) + Start.y,
                               Start.z);
        }

    }

}