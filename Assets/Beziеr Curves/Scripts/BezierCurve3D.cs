using UnityEngine;
using System.Collections.Generic;

namespace BezierCurves
{
    public class BezierCurve3D : MonoBehaviour
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
        [Tooltip("The number of segments that the curve has. Affects calculations and performance")]
        private int segmentsCount = 25;

        [SerializeField]
        [HideInInspector]
        private List<BezierPoint3D> keyPoints = new List<BezierPoint3D>();

        // Properties

        /// <summary>
        /// Gets or sets the number of segments.
        /// The segments controls how accurate are the calculation regarding the curve.
        /// A good value is 25, where the calculations are great and there is not much impact on performance.
        /// </summary>
        public int SegmentsCount
        {
            get
            {
                return this.segmentsCount;
            }
            set
            {
                this.segmentsCount = value;
            }
        }

        /// <summary>
        /// Gets the key points of the curve
        /// </summary>
        public List<BezierPoint3D> KeyPoints
        {
            get
            {
                return this.keyPoints;
            }
        }

        /// <summary>
        /// Gets the number of key points along the curve
        /// </summary>
        public int KeyPointsCount
        {
            get
            {
                return this.keyPoints.Count;
            }
        }        

        // Unity Methods
        protected virtual void OnDrawGizmos()
        {
            if (this.KeyPointsCount > 1)
            {
                // Draw the curve
                Gizmos.color = this.curveColor;
                Vector3 fromPoint = this.Evaluate(0f);

                for (int i = 0; i < this.SegmentsCount; i++)
                {
                    float time = (i + 1) / (float)this.SegmentsCount;
                    Vector3 toPoint = this.Evaluate(time);
                    Gizmos.DrawLine(fromPoint, toPoint);
                    fromPoint = toPoint;
                }

                // Draw the start and the end of the curve indicators
                Gizmos.color = this.startPointColor;
                Gizmos.DrawSphere(this.keyPoints[0].Position, 0.05f);

                Gizmos.color = this.endPointColor;
                Gizmos.DrawSphere(this.keyPoints[this.KeyPointsCount - 1].Position, 0.05f);
            }
        }

        // Public Methods

        /// <summary>
        /// Adds a key point at the end of the curve
        /// </summary>
        /// <returns>The new key point</returns>
        public BezierPoint3D AddKeyPoint()
        {
            return this.AddKeyPointAt(this.KeyPointsCount);
        }

        /// <summary>
        /// Add a key point at a specified index
        /// </summary>
        /// <param name="index">The index at which the key point will be added</param>
        /// <returns>The new key point</returns>
        public BezierPoint3D AddKeyPointAt(int index)
        {
            BezierPoint3D newPoint = new GameObject("Point " + this.keyPoints.Count, typeof(BezierPoint3D)).GetComponent<BezierPoint3D>();
            newPoint.Curve = this;
            newPoint.transform.parent = this.transform;
            newPoint.transform.localRotation = Quaternion.identity;

            if (this.KeyPointsCount == 0 || this.KeyPointsCount == 1)
            {
                newPoint.LocalPosition = Vector3.zero;
            }
            else
            {
                if (index == 0)
                {
                    newPoint.Position = (this.keyPoints[0].Position - this.keyPoints[1].Position).normalized + this.keyPoints[0].Position;
                }
                else if (index == this.KeyPointsCount)
                {
                    newPoint.Position = (this.keyPoints[index - 1].Position - this.keyPoints[index - 2].Position).normalized + this.keyPoints[index - 1].Position;
                }
                else
                {
                    newPoint.Position = BezierCurve3D.EvaluateCubicCurve(0.5f, this.keyPoints[index - 1], this.keyPoints[index]);
                }
            }

            this.keyPoints.Insert(index, newPoint);

            return newPoint;
        }

        /// <summary>
        /// Removes a key point at a specified index
        /// </summary>
        /// <param name="index">The index of the key point that will be removed</param>
        /// <returns>true - if the point was removed, false - otherwise</returns>
        public bool RemoveKeyPointAt(int index)
        {
            if (this.KeyPointsCount < 2)
            {
                return false;
            }

            var point = this.keyPoints[index];
            this.keyPoints.RemoveAt(index);

            Destroy(point.gameObject);

            return true;
        }

        /// <summary>
        /// Evaluates a position along the curve at a specified normalized time [0, 1]
        /// </summary>
        /// <param name="time">The normalized length at which we want to get a position [0, 1]</param>
        /// <returns>The evaluated Vector3 position</returns>
        public Vector3 Evaluate(float time)
        {
            if (time < 0.01f)
            {
                return this.keyPoints[0].Position;
            }
            else if (Mathf.Abs(time - 1f) < 0.01f)
            {
                return this.keyPoints[this.KeyPointsCount - 1].Position;
            }

            // The evaluated points is between these two points
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.EvaluateCubicCurve(timeRelativeToSegment, startPoint, endPoint);
        }

        /// <summary>
        /// Gets the length of the curve.
        /// Depends on the segments of the curve.
        /// </summary>
        public float GetApproximateLength()
        {
            float length = 0;
            for (int i = 0; i < this.KeyPointsCount - 1; i++)
            {
                length += BezierCurve3D.GetApproximateLengthOfCubicCurve(this.keyPoints[i], this.keyPoints[i + 1], this.SegmentsCount);
            }

            return length;
        }

        public void GetCubicSegment(float time, out BezierPoint3D startPoint, out BezierPoint3D endPoint, out float timeRelativeToSegment)
        {
            startPoint = null;
            endPoint = null;
            timeRelativeToSegment = 0f;

            float subCurvePercent = 0f;
            float totalPercent = 0f;
            float approximateLength = this.GetApproximateLength();
            int subCurvesSampling = (this.SegmentsCount / (this.KeyPointsCount - 1)) + 1;

            for (int i = 0; i < this.KeyPointsCount - 1; i++)
            {
                subCurvePercent = BezierCurve3D.GetApproximateLengthOfCubicCurve(this.keyPoints[i], this.keyPoints[i + 1], subCurvesSampling) / approximateLength;
                if (subCurvePercent + totalPercent > time)
                {
                    startPoint = this.keyPoints[i];
                    endPoint = this.keyPoints[i + 1];

                    break;
                }

                totalPercent += subCurvePercent;
            }

            if (endPoint == null)
            {
                // If the evaluated point is very near to the end of the curve
                endPoint = this.keyPoints[this.KeyPointsCount - 1];
                timeRelativeToSegment = time;
            }
            else
            {
                timeRelativeToSegment = (time - totalPercent) / subCurvePercent;
            }
        }

        /// <summary>
        /// Evaluates a cubic bezier curve
        /// </summary>
        public static Vector3 EvaluateCubicCurve(float time, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return EvaluateCubicCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        /// <summary>
        /// Evaluates a cubic bezier curve
        /// </summary>
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

        /// <summary>
        /// Gets the length of a cubic bezier curve
        /// </summary>
        public static float GetApproximateLengthOfCubicCurve(BezierPoint3D startPoint, BezierPoint3D endPoint, int sampling = 10)
        {
            return GetApproximateLengthOfCubicCurve(startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition, sampling);
        }

        /// <summary>
        /// Gets the length of a cubic bezier curve
        /// </summary>
        public static float GetApproximateLengthOfCubicCurve(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int sampling = 10)
        {
            float length = 0f;
            Vector3 fromPoint = EvaluateCubicCurve(0f, startPosition, endPosition, startTangent, endTangent);

            for (int i = 0; i < sampling; i++)
            {
                float time = (i + 1) / (float)sampling;
                Vector3 toPoint = EvaluateCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
                length += Vector3.Distance(fromPoint, toPoint);
                fromPoint = toPoint;
            }

            return length;
        }
    }
}
