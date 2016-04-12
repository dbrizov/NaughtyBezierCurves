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
        private int sampling = 25;

        [SerializeField]
        [HideInInspector]
        private List<BezierPoint3D> keyPoints = new List<BezierPoint3D>();

        [SerializeField]
        [Range(0f, 1f)]
        float normalizedTime = 0.5f;

        // Properties        
        public int Sampling
        {
            get
            {
                return this.sampling;
            }
            set
            {
                this.sampling = value;
            }
        }
        
        public List<BezierPoint3D> KeyPoints
        {
            get
            {
                return this.keyPoints;
            }
        }
        
        public int KeyPointsCount
        {
            get
            {
                return this.KeyPoints.Count;
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
            BezierPoint3D newPoint = new GameObject("Point " + this.KeyPoints.Count, typeof(BezierPoint3D)).GetComponent<BezierPoint3D>();
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
                    newPoint.Position = (this.KeyPoints[0].Position - this.KeyPoints[1].Position).normalized + this.KeyPoints[0].Position;
                }
                else if (index == this.KeyPointsCount)
                {
                    newPoint.Position = (this.KeyPoints[index - 1].Position - this.KeyPoints[index - 2].Position).normalized + this.KeyPoints[index - 1].Position;
                }
                else
                {
                    newPoint.Position = BezierCurve3D.GetPositionOfCubicCurve(0.5f, this.KeyPoints[index - 1], this.KeyPoints[index]);
                }
            }

            this.KeyPoints.Insert(index, newPoint);

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

            var point = this.KeyPoints[index];
            this.KeyPoints.RemoveAt(index);

            Destroy(point.gameObject);

            return true;
        }

        /// <summary>
        /// Evaluates a position along the curve at a specified normalized time [0, 1]
        /// </summary>
        /// <param name="time">The normalized length at which we want to get a position [0, 1]</param>
        /// <returns>The evaluated Vector3 position</returns>
        public Vector3 GetPosition(float time)
        {
            // The evaluated points is between these two points
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.GetPositionOfCubicCurve(timeRelativeToSegment, startPoint, endPoint);
        }

        public Quaternion GetRotation(float time, Vector3 up)
        {
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.GetRotationOfCubicCurve(timeRelativeToSegment, up, startPoint, endPoint);
        }

        public Vector3 GetTangent(float time)
        {
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.GetTangentOfCubicCurve(timeRelativeToSegment, startPoint, endPoint);
        }

        public Vector3 GetBinormal(float time, Vector3 up)
        {
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.GetBinormalOfCubicCurve(timeRelativeToSegment, up, startPoint, endPoint);
        }

        public Vector3 GetNormal(float time, Vector3 up)
        {
            BezierPoint3D startPoint;
            BezierPoint3D endPoint;
            float timeRelativeToSegment;

            this.GetCubicSegment(time, out startPoint, out endPoint, out timeRelativeToSegment);

            return BezierCurve3D.GetNormalOfCubicCurve(timeRelativeToSegment, up, startPoint, endPoint);
        }

        public float GetApproximateLength()
        {
            float length = 0;
            for (int i = 0; i < this.KeyPointsCount - 1; i++)
            {
                length += BezierCurve3D.GetApproximateLengthOfCubicCurve(this.KeyPoints[i], this.KeyPoints[i + 1], this.Sampling + 1);
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
            //int subCurvesSampling = (this.Sampling / (this.KeyPointsCount - 1)) + 1;

            for (int i = 0; i < this.KeyPointsCount - 1; i++)
            {
                subCurvePercent = BezierCurve3D.GetApproximateLengthOfCubicCurve(this.KeyPoints[i], this.KeyPoints[i + 1], this.Sampling) / approximateLength;
                if (subCurvePercent + totalPercent > time)
                {
                    startPoint = this.KeyPoints[i];
                    endPoint = this.KeyPoints[i + 1];

                    break;
                }

                totalPercent += subCurvePercent;
            }

            if (endPoint == null)
            {
                // If the evaluated point is very near to the end of the curve
                startPoint = this.KeyPoints[this.KeyPointsCount - 2];
                endPoint = this.KeyPoints[this.KeyPointsCount - 1];

                timeRelativeToSegment = time;
            }
            else
            {
                timeRelativeToSegment = (time - totalPercent) / subCurvePercent;
            }
        }
        
        public static Vector3 GetPositionOfCubicCurve(float time, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return GetPositionOfCubicCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }
        
        public static Vector3 GetPositionOfCubicCurve(float time, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            float t = time;
            float u = 1f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;

            Vector3 result =
                (u3) * startPosition +
                (3f * u2 * t) * startTangent +
                (3f * u * t2) * endTangent +
                (t3) * endPosition;

            return result;
        }

        public static Quaternion GetRotationOfCubicCurve(float time, Vector3 up, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return GetRotationOfCubicCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        public static Quaternion GetRotationOfCubicCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            Vector3 tangent = GetTangentOfCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
            Vector3 normal = GetNormalOfCubicCurve(time, up, startPosition, endPosition, startTangent, endTangent);

            return Quaternion.LookRotation(tangent, normal);
        }

        public static Vector3 GetTangentOfCubicCurve(float time, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return GetTangentOfCubicCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        public static Vector3 GetTangentOfCubicCurve(float time, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            float t = time;
            float u = 1f - t;
            float u2 = u * u;
            float t2 = t * t;

            Vector3 tangent =
                (-u2) * startPosition +
                (u * (u - 2f * t)) * startTangent -
                (t * (t - 2f * u)) * endTangent +
                (t2) * endPosition;

            return tangent.normalized;
        }

        public static Vector3 GetBinormalOfCubicCurve(float time, Vector3 up, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return GetBinormalOfCubicCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        public static Vector3 GetBinormalOfCubicCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            Vector3 tangent = GetTangentOfCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
            Vector3 binormal = Vector3.Cross(up, tangent);

            return binormal.normalized;
        }

        public static Vector3 GetNormalOfCubicCurve(float time, Vector3 up, BezierPoint3D startPoint, BezierPoint3D endPoint)
        {
            return GetNormalOfCubicCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition);
        }

        public static Vector3 GetNormalOfCubicCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
        {
            Vector3 tangent = GetTangentOfCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
            Vector3 binormal = GetBinormalOfCubicCurve(time, up, startPosition, endPosition, startTangent, endTangent);
            Vector3 normal = Vector3.Cross(tangent, binormal);

            return normal.normalized;
        }

        public static float GetApproximateLengthOfCubicCurve(BezierPoint3D startPoint, BezierPoint3D endPoint, int sampling)
        {
            return GetApproximateLengthOfCubicCurve(startPoint.Position, endPoint.Position, startPoint.RightHandlePosition, endPoint.LeftHandlePosition, sampling);
        }
        
        public static float GetApproximateLengthOfCubicCurve(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int sampling)
        {
            float length = 0f;
            Vector3 fromPoint = GetPositionOfCubicCurve(0f, startPosition, endPosition, startTangent, endTangent);

            for (int i = 0; i < sampling; i++)
            {
                float time = (i + 1) / (float)sampling;
                Vector3 toPoint = GetPositionOfCubicCurve(time, startPosition, endPosition, startTangent, endTangent);
                length += Vector3.Distance(fromPoint, toPoint);
                fromPoint = toPoint;
            }

            return length;
        }

        protected virtual void OnDrawGizmos()
        {
            if (this.KeyPointsCount > 1)
            {
                // Draw the curve
                Vector3 fromPoint = this.GetPosition(0f);

                for (int i = 0; i < this.Sampling; i++)
                {
                    float time = (i + 1) / (float)this.Sampling;
                    Vector3 toPoint = this.GetPosition(time);

                    // Draw segment
                    Gizmos.color = this.curveColor;
                    Gizmos.DrawLine(fromPoint, toPoint);

                    fromPoint = toPoint;
                }

                // Draw the start and the end of the curve indicators
                Gizmos.color = this.startPointColor;
                Gizmos.DrawSphere(this.KeyPoints[0].Position, 0.05f);

                Gizmos.color = this.endPointColor;
                Gizmos.DrawSphere(this.KeyPoints[this.KeyPointsCount - 1].Position, 0.05f);

                // Draw the point at the normalized time
                Vector3 point = this.GetPosition(this.normalizedTime);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(point, 0.025f);

                Vector3 tangent = this.GetTangent(this.normalizedTime);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(point, point + tangent / 2f);

                Vector3 binormal = this.GetBinormal(this.normalizedTime, Vector3.up);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(point, point + binormal / 2f);

                Vector3 normal = this.GetNormal(this.normalizedTime, Vector3.up);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(point, point + normal / 2f);
            }
        }
    }
}
