using UnityEngine;
using System.Collections.Generic;

namespace BezierCurves
{
    public class BezierCurve : MonoBehaviour
    {
        // Serializable Fields
        [SerializeField]
        private Color color = Color.green;

        [SerializeField]
        [Tooltip("Used only for scene rendering")]
        private int antiAliasing = 50;

        [SerializeField]
        [Tooltip("How precise are the calculations")]
        private int precision = 25;

        [SerializeField]
        private List<BezierPoint> points = null;

        // Properties
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

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
                return this.points.Count;
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
        //protected virtual void Update()
        //{
        //    Debug.Log(this.ApproximateLength);
        //}

        protected virtual void OnDrawGizmos()
        {
            // Draw the curve
            if (this.PointsCount > 1)
            {
                Gizmos.color = this.Color;
                Vector3 fromPoint = this.Evaluate(0f);
                for (int i = 0; i < this.antiAliasing; i++)
                {
                    float time = (i + 1) / (float)this.antiAliasing;
                    Vector3 toPoint = this.Evaluate(time);
                    Gizmos.DrawLine(fromPoint, toPoint);
                    fromPoint = toPoint;
                }
            }
        }

        // Public Methods
        public void InsertPoint(int index)
        {
            BezierPoint newPoint = new GameObject("Point" + index, typeof(BezierPoint)).GetComponent<BezierPoint>();
            newPoint.transform.parent = this.transform;
            newPoint.LocalPosition = Vector3.zero;
            newPoint.Curve = this;

            this.points.Insert(index, newPoint);
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

            float curveTime = 0f;
            float totalTime = 0f;
            float approximateLength = this.ApproximateLength;

            // The evaluated points is between these two points
            BezierPoint startPoint = null;
            BezierPoint endPoint = null;

            for (int i = 0; i < this.PointsCount - 1; i++)
            {
                curveTime = BezierCurve.GetApproximateLengthOfCubicCurve(this.GetPoint(i), this.GetPoint(i + 1)) / approximateLength;
                if (curveTime + totalTime > time)
                {
                    startPoint = this.GetPoint(i);
                    endPoint = this.GetPoint(i + 1);

                    break;
                }

                totalTime += curveTime;
            }

            float timeOfSubCurve = ((time - totalTime) / curveTime);
            return BezierCurve.EvaluateCubicCurve(timeOfSubCurve, startPoint, endPoint);
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
    }
}
