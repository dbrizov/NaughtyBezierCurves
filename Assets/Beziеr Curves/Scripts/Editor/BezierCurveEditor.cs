using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BezierCurves
{
    [CustomEditor(typeof(BezierCurve))]
    [CanEditMultipleObjects]
    public class BezierCurveEditor : Editor
    {
        private const float AddButtonWidth = 80f;
        private const float RemoveButtonWidth = 19f;

        private BezierCurve curve;
        private ReorderableList points;
        private bool showPoints = true;

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

            BezierCurveEditor.AddDefaultPoints(curve);

            Selection.activeGameObject = curve.gameObject;
        }

        private static void AddDefaultPoints(BezierCurve curve)
        {
            BezierPoint startPoint = curve.AddPoint();
            startPoint.LocalPosition = new Vector3(-1f, 0f, 0f);
            startPoint.LeftHandleLocalPosition = new Vector3(-0.25f, -0.25f, 0f);

            BezierPoint endPoint = curve.AddPoint();
            endPoint.LocalPosition = new Vector3(1f, 0f, 0f);
            endPoint.LeftHandleLocalPosition = new Vector3(-0.25f, 0.25f, 0f);
        }

        protected virtual void OnEnable()
        {
            this.curve = (BezierCurve)this.target;
            if (curve.PointsCount < 2)
            {
                while (curve.PointsCount != 0)
                {
                    curve.RemovePointAt(this.curve.PointsCount - 1);
                }

                BezierCurveEditor.AddDefaultPoints(this.curve);
            }

            this.points = new ReorderableList(this.serializedObject, serializedObject.FindProperty("points"), true, true, false, false);
            this.points.drawElementCallback = this.DrawElementCallback;
            this.points.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, string.Format("Reorderable List | Points: {0}", this.points.serializedProperty.arraySize));
                };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();

            this.showPoints = EditorGUILayout.Foldout(this.showPoints, "Points");
            if (this.showPoints)
            {
                this.points.DoLayoutList();

                if (GUILayout.Button("Add Point"))
                {
                    this.curve.AddPoint();
                }

                if (GUILayout.Button("Add Point and Select"))
                {
                    var point = this.curve.AddPoint();
                    Selection.activeGameObject = point.gameObject;
                }

                if (GUILayout.Button("Fix names of points"))
                {
                    this.RenamePoints();
                }
            }

            if (GUILayout.Button("Log Length"))
            {
                Debug.Log(this.curve.ApproximateLength);
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

                BezierPointEditor.handleCapSize = BezierPointEditor.CircleCapSize;
                BezierPointEditor.DrawPointSceneGUI(curve.GetPoint(i));
            }
        }

        private void RenamePoints()
        {
            for (int i = 0; i < this.curve.PointsCount; i++)
            {
                this.curve.GetPoint(i).name = "Point " + i;
            }
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = this.points.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // Draw "Add Before" button
            if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
            {
                this.curve.AddPointAt(index);
            }

            // Draw point name
            EditorGUI.PropertyField(
                new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

            // Draw "Add After" button
            if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
            {
                this.curve.AddPointAt(index + 1);
            }

            // Draw remove button
            if (this.curve.PointsCount > 2)
            {
                if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
                {
                    this.curve.RemovePointAt(index);
                }
            }
        }
    }
}
