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
            GameObject bezierCurve = new GameObject("Bezier Curve", typeof(BezierCurve));

            Vector3 position = Vector3.zero;
            if (Camera.current != null)
            {
                position = Camera.current.transform.position + Camera.current.transform.forward * 10f;
            }

            bezierCurve.transform.position = position;
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
                this.curve.InsertPoint(this.curve.PointsCount);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            BezierCurveEditor.DrawPointsSceneGUI(this.curve);
        }

        public static void DrawPointsSceneGUI(BezierCurve curve)
        {
            for (int i = 0; i < curve.PointsCount; i++)
            {
                BezierPointEditor.DrawPointSceneGUI(curve.GetPoint(i));
            }
        }
    }
}
