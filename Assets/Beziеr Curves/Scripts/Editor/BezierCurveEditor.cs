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
            this.points = new ReorderableList(this.serializedObject, serializedObject.FindProperty("points"), true, true, false, false);

            this.points.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Points: " + this.points.serializedProperty.arraySize);
                };

            this.points.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = this.points.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    // Draw "Add Before" button
                    if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
                    {
                        Debug.Log("Add Before");
                    }

                    // Draw point name
                    EditorGUI.PropertyField(
                        new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

                    // Draw "Add After" button
                    if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
                    {
                        Debug.Log("Add After");
                    }

                    // Draw remove button
                    if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
                    {
                        Debug.Log("Remove");
                    }
                };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();

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
