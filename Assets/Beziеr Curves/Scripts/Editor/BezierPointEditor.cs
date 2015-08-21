using UnityEngine;
using UnityEditor;

namespace BezierCurves
{
    [CustomEditor(typeof(BezierPoint))]
    [CanEditMultipleObjects]
    public class BezierPointEditor : Editor
    {
        private BezierPoint point;

        protected virtual void OnEnable()
        {
            this.point = (BezierPoint)this.target;
        }

        protected virtual void OnSceneGUI()
        {
            BezierCurveEditor.DrawPointsSceneGUI(this.point.Curve);
        }

        public static void DrawPointSceneGUI(BezierPoint point)
        {
            Handles.color = Color.black;
            Handles.Label(point.Position + new Vector3(0f, HandleUtility.GetHandleSize(point.Position) * 0.4f, 0f), point.gameObject.name);

            Handles.color = Color.yellow;
            point.Position = Handles.FreeMoveHandle(point.Position, point.transform.rotation,
                HandleUtility.GetHandleSize(point.Position) * 0.1f, Vector3.zero, Handles.RectangleCap);

            Handles.color = Color.white;
            Handles.DrawLine(point.Position, point.LeftHandlePosition);
            Handles.DrawLine(point.Position, point.RightHandlePosition);

            Handles.color = Color.cyan;
            point.LeftHandlePosition = Handles.FreeMoveHandle(point.LeftHandlePosition, point.transform.rotation,
                HandleUtility.GetHandleSize(point.LeftHandlePosition) * 0.075f, Vector3.zero, Handles.CircleCap);

            point.RightHandlePosition = Handles.FreeMoveHandle(point.RightHandlePosition, point.transform.rotation,
                HandleUtility.GetHandleSize(point.RightHandlePosition) * 0.075f, Vector3.zero, Handles.CircleCap);
        }
    }
}