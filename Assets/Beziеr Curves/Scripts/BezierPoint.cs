using UnityEngine;

namespace BezierCurves
{
    public class BezierPoint : MonoBehaviour
    {
        public enum HandleType
        {
            Connected,
            Broken
        }

        // Serialized fields
        [SerializeField]
        [Tooltip("The curve the point belongs to")]
        private BezierCurve curve = null;

        [SerializeField]
        private HandleType handleType = HandleType.Connected;

        [SerializeField]
        private Vector3 leftHandleLocalPosition = new Vector3(-0.2f, 0f, 0f);

        [SerializeField]
        private Vector3 rightHandleLocalPosition = new Vector3(0.2f, 0f, 0f);

        // Properties
        public BezierCurve Curve
        {
            get
            {
                return this.curve;
            }
            set
            {
                this.curve = value;
            }
        }

        public HandleType HandleStyle
        {
            get
            {
                return this.handleType;
            }
            set
            {
                this.handleType = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.transform.position;
            }
            set
            {
                this.transform.position = value;
            }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return this.transform.localPosition;
            }
            set
            {
                this.transform.localPosition = value;
            }
        }

        public Vector3 LeftHandleLocalPosition
        {
            get
            {
                return this.leftHandleLocalPosition;
            }
            set
            {
                this.leftHandleLocalPosition = value;
                if (this.handleType == HandleType.Connected)
                {
                    this.rightHandleLocalPosition = -value;
                }
            }
        }

        public Vector3 RightHandleLocalPosition
        {
            get
            {
                return this.rightHandleLocalPosition;
            }
            set
            {
                this.rightHandleLocalPosition = value;
                if (this.handleType == HandleType.Connected)
                {
                    this.leftHandleLocalPosition = -value;
                }
            }
        }

        public Vector3 LeftHandlePosition
        {
            get
            {
                return this.transform.TransformPoint(this.LeftHandleLocalPosition);
            }
            set
            {
                this.LeftHandleLocalPosition = this.transform.InverseTransformPoint(value);
            }
        }

        public Vector3 RightHandlePosition
        {
            get
            {
                return this.transform.TransformPoint(this.RightHandleLocalPosition);
            }
            set
            {
                this.RightHandleLocalPosition = this.transform.InverseTransformPoint(value);
            }
        }
    }
}
