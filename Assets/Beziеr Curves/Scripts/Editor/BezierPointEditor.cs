using UnityEngine;
using UnityEditor;

namespace BezierCurves
{
    [CustomEditor(typeof(BezierPoint), true)]
    [CanEditMultipleObjects]
    public class BezierPointEditor : Editor
    {
        private const float CircleCapSize = 0.075f;
        private const float RectangeCapSize = 0.1f;
        private const float SphereCapSize = 0.15f;

        public static float pointCapSize = RectangeCapSize;
        public static float handleCapSize = CircleCapSize;

        private BezierPoint point;
        private SerializedProperty handleType;
        private SerializedProperty leftHandleLocalPosition;
        private SerializedProperty rightHandleLocalPosition;

        protected virtual void OnEnable()
        {
            this.point = (BezierPoint)this.target;
            this.handleType = this.serializedObject.FindProperty("handleType");
            this.leftHandleLocalPosition = this.serializedObject.FindProperty("leftHandleLocalPosition");
            this.rightHandleLocalPosition = this.serializedObject.FindProperty("rightHandleLocalPosition");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.handleType);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.leftHandleLocalPosition);
            if (EditorGUI.EndChangeCheck())
            {
                this.rightHandleLocalPosition.vector3Value = -this.leftHandleLocalPosition.vector3Value;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.rightHandleLocalPosition);
            if (EditorGUI.EndChangeCheck())
            {
                this.leftHandleLocalPosition.vector3Value = -this.rightHandleLocalPosition.vector3Value;
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            BezierPointEditor.handleCapSize = BezierPointEditor.CircleCapSize;
            BezierCurveEditor.DrawPointsSceneGUI(this.point.Curve, this.point);

            BezierPointEditor.handleCapSize = BezierPointEditor.SphereCapSize;
            BezierPointEditor.DrawPointSceneGUI(this.point, Handles.DotCap, Handles.SphereCap);
        }

        public static void DrawPointSceneGUI(BezierPoint point)
        {
            BezierPointEditor.handleCapSize = BezierPointEditor.CircleCapSize;
            DrawPointSceneGUI(point, Handles.RectangleCap, Handles.CircleCap);
        }

        public static void DrawPointSceneGUI(BezierPoint point, Handles.DrawCapFunction drawPointFunc, Handles.DrawCapFunction drawHandleFunc)
        {
            // Draw a label for the point
            Handles.color = Color.black;
            Handles.Label(point.Position + new Vector3(0f, HandleUtility.GetHandleSize(point.Position) * 0.4f, 0f), point.gameObject.name);

            // Draw the center of the control point
            Handles.color = Color.yellow;
            Vector3 newPointPosition = Handles.FreeMoveHandle(point.Position, point.transform.rotation,
                HandleUtility.GetHandleSize(point.Position) * BezierPointEditor.pointCapSize, Vector3.one * 0.5f, drawPointFunc);

            if (point.Position != newPointPosition)
            {
                point.Position = newPointPosition;
            }

            // Draw the left and right handles
            Handles.color = Color.white;
            Handles.DrawLine(point.Position, point.LeftHandlePosition);
            Handles.DrawLine(point.Position, point.RightHandlePosition);

            Handles.color = Color.cyan;
            Vector3 newLeftHandlePosition = Handles.FreeMoveHandle(point.LeftHandlePosition, point.transform.rotation,
                HandleUtility.GetHandleSize(point.LeftHandlePosition) * BezierPointEditor.handleCapSize, Vector3.zero, drawHandleFunc);

            if (point.LeftHandlePosition != newLeftHandlePosition)
            {
                point.LeftHandlePosition = newLeftHandlePosition;
            }
            
            Vector3 newRightHandlePosition = Handles.FreeMoveHandle(point.RightHandlePosition, point.transform.rotation,
                HandleUtility.GetHandleSize(point.RightHandlePosition) * BezierPointEditor.handleCapSize, Vector3.zero, drawHandleFunc);

            if (point.RightHandlePosition != newRightHandlePosition)
            {
                point.RightHandlePosition = newRightHandlePosition;
            }
        }
    }
}