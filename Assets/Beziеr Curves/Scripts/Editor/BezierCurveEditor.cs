using UnityEngine;
using UnityEditor;

namespace BezierCurves
{
    [CustomEditor(typeof(BezierCurve))]
    [CanEditMultipleObjects]
    public class BezierCurveEditor : Editor
    {
        private BezierCurve curve;

        [MenuItem("GameObject/Create Other/Bezier Curve")]
        private static void CreateBezeirCurve()
        {
            BezierCurve curve = new GameObject("Bezier Curve", typeof(BezierCurve)).GetComponent<BezierCurve>();
            Vector3 position = Vector3.zero;
            if (Camera.current != null)
            {
                position = Camera.current.transform.position + Camera.current.transform.forward * 10f;
            }

            curve.transform.position = position;

            BezierPoint startPoint = curve.AddPoint();
            startPoint.LocalPosition = new Vector3(-1f, 0f, 0f);
            startPoint.LeftHandleLocalPosition = new Vector3(-0.25f, -0.25f, 0f);

            BezierPoint endPoint = curve.AddPoint();
            endPoint.LocalPosition = new Vector3(1f, 0f, 0f);
            endPoint.LeftHandleLocalPosition = new Vector3(-0.25f, 0.25f, 0f);

            Selection.activeGameObject = curve.gameObject;
        }

        protected virtual void OnEnable()
        {
            this.curve = (BezierCurve)this.target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();

            if (GUILayout.Button("Add Point"))
            {
                this.curve.AddPoint();
            }

            if (GUILayout.Button("Add Point and Select"))
            {
                var point = this.curve.AddPoint();
                Selection.activeGameObject = point.gameObject;
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            BezierCurveEditor.DrawPointsSceneGUI(this.curve);
        }

        public static void DrawPointsSceneGUI(BezierCurve curve, BezierPoint exclude = null)
        {
            for (int i = 0; i < curve.PointsCount; i++)
            {
                if (curve.GetPoint(i) == exclude)
                {
                    continue;
                }

                BezierPointEditor.DrawPointSceneGUI(curve.GetPoint(i));
            }
        }
    }
}
