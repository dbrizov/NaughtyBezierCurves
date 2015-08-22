using UnityEngine;
using System.Collections.Generic;

namespace BezierCurves
{
    public class BezierCurve : MonoBehaviour
    {
        // Serializable Fields
        [SerializeField]
        [Tooltip("The color used to render the curve")]
        private Color curveColor = Color.green;

        [SerializeField]
        [Tooltip("The color used to render the start point of the curve")]
        private Color startPointColor = Color.red;

        [SerializeField]
        [Tooltip("The color used to render the end point of the curve")]
        private Color endPointColor = Color.blue;

        [SerializeField]
        [Tooltip("Used only for scene rendering")]
        private int antiAliasing = 5;

        [SerializeField]
        [Tooltip("How precise are the calculations")]
        private int precision = 25;

        [SerializeField]
        [HideInInspector]
        private List<BezierPoint> points = null;

        // Properties
        public int Precision
        {
            get
            {
                return this.precision;
            }
            set
            {
                this.precision = value;
            }
        }

        public int PointsCount
        {
            get
            {
                int pointsCount = 0;
                if (this.points != null)
                {
                    pointsCount = this.points.Count;
                }

                return pointsCount;
            }
        }

        public float ApproximateLength
        {
            get
            {
                float length = 0;
                for (int i = 0; i < this.PointsCount - 1; i++)
                {
                    length += BezierCurve.GetApproximateLengthOfCubicCurve(this.GetPoint(i), this.GetPoint(i + 1), this.Precision);
                }

                return length;
            }
        }

        // Unity Methods
        protected virtual void OnDrawGizmos()
        {
            if (this.PointsCount > 1)
            {
                // Draw the curve
                Gizmos.color = this.curveColor;
                Vector3 fromPoint = this.Evaluate(0f);
                int drawSegmentsCount = (int)(this.antiAliasing * this.ApproximateLength);

                for (int i = 0; i < drawSegmentsCount; i++)
                {
                    float time = (i + 1) / (float)drawSegmentsCount;
                    Vector3 toPoint = this.Evaluate(time);
                    Gizmos.DrawLine(fromPoint, toPoint);
                    fromPoint = toPoint;
                }

                // Draw the start and the end of the curve indicators
                Gizmos.color = this.startPointColor;
                Gizmos.DrawSphere(this.GetPoint(0).Position, 0.05f);

                Gizmos.color = this.endPointColor;
                Gizmos.DrawSphere(this.GetPoint(this.PointsCount - 1).Position, 0.05f);
            }
        }

        // Public Methods
        public BezierPoint AddPoint()
        {
            return this.InsertPoint(this.PointsCount);
        }

        public BezierPoint InsertPoint(int index)
        {
            if (this.points == null)
            {
                this.points = new List<BezierPoint>();
            }

            BezierPoint newPoint = new GameObject("Point" + index, typeof(BezierPoint)).GetComponent<BezierPoint>();
            newPoint.transform.parent = this.transform;
            newPoint.LocalPosition = Vector3.zero;
            newPoint.Curve = this;

            this.points.Insert(index, newPoint);
            return newPoint;
        }

        public BezierPoint GetPoint(int index)
        {
            return this.points[index];
        }

        public Vector3 Evaluate(float time)
        {
            if (time < 0.01f)
            {
                return this.GetPoint(0).Position;
            }
            else if (Mathf.Abs(time - 1f) < 0.01f)
            {
                return this.GetPoint(this.PointsCount - 1).Position;
            }

            // The evaluated points is between these two points
            BezierPoint startPoint = null;
            BezierPoint endPoint = null;
            float subCurveTime = 0f;
            float totalTime = 0f;
            float approximateLength = this.ApproximateLength;

            for (int i = 0; i < this.PointsCount - 1; i++)
            {
                subCurveTime = BezierCurve.GetApproximateLengthOfCubicCurve(this.GetPoint(i), this.GetPoint(i + 1)) / approximateLength;
                if (subCurveTime + totalTime > time)
                {
                    startPoint = this.GetPoint(i);
                    endPoint = this.GetPoint(i + 1);

                    break;
                }

                totalTime += subCurveTime;
            }

            float timeMappedToSubCurve = ((time - totalTime) / subCurveTime);
            return BezierCurve.EvaluateCubicCurve(timeMappedToSubCurve, startPoint, endPoint);
        }

        public static Vector3 EvaluateCubicCurve(float time, BezierPoint startPoint, BezierPoint endPoint)
        {
            return EvaluateCubicCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        public static Vector3 EvaluateCubicCurve(float time, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            if (time < 0.01f)
            {
                return startPosition;
            }
            else if (Mathf.Abs(time - 1f) < 0.01f)
            {
                return endPosition;
            }

            float u = 1f - time;
            float tt = time * time;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * time;

            Vector3 result = uuu * startPosition; // First term
            result += 3 * uu * time * startTangent; // Second term
            result += 3 * u * tt * endTangent; // Third term
            result += ttt * endPosition; // Fourth term

            return result;
        }

        public static float GetApproximateLengthOfCubicCurve(BezierPoint startPoint, BezierPoint endPoint, int precision = 10)
        {
            return GetApproximateLengthOfCubicCurve(startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition, precision);
        }

        public static float GetApproximateLengthOfCubicCurve(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int precision = 10)
        {
            float length = 0f;
            Vector3 fromPoint = EvaluateCubicCurve(0f, startPosition, endPosition, startTangent, endTangent);

            for (int i = 0; i < precision; i++)
            {
                float time = (i + 1) / (float)precision;
                Vector3 toPoint = EvaluateCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
                length += Vector3.Distance(fromPoint, toPoint);
                fromPoint = toPoint;
            }

            return length;
        }
    }
}
