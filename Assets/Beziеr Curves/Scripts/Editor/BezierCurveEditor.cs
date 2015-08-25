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
        private ReorderableList keyPoints;
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

            Undo.RegisterCreatedObjectUndo(curve.gameObject, "Create curve");

            Selection.activeGameObject = curve.gameObject;
        }

        private static void AddDefaultPoints(BezierCurve curve)
        {
            BezierPoint startPoint = curve.AddKeyPoint();
            startPoint.LocalPosition = new Vector3(-1f, 0f, 0f);
            startPoint.LeftHandleLocalPosition = new Vector3(-0.25f, -0.25f, 0f);

            BezierPoint endPoint = curve.AddKeyPoint();
            endPoint.LocalPosition = new Vector3(1f, 0f, 0f);
            endPoint.LeftHandleLocalPosition = new Vector3(-0.25f, 0.25f, 0f);
        }

        protected virtual void OnEnable()
        {
            this.curve = (BezierCurve)this.target;
            if (curve.KeyPointsCount < 2)
            {
                while (curve.KeyPointsCount != 0)
                {
                    curve.RemoveKeyPointAt(this.curve.KeyPointsCount - 1);
                }

                BezierCurveEditor.AddDefaultPoints(this.curve);
            }

            this.keyPoints = new ReorderableList(this.serializedObject, serializedObject.FindProperty("keyPoints"), true, true, false, false);
            this.keyPoints.drawElementCallback = this.DrawElementCallback;
            this.keyPoints.drawHeaderCallback =
                (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, string.Format("Reorderable List | Points: {0}", this.keyPoints.serializedProperty.arraySize));
                };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();

            this.showPoints = EditorGUILayout.Foldout(this.showPoints, "Key Points");
            if (this.showPoints)
            {
                this.keyPoints.DoLayoutList();

                if (GUILayout.Button("Add Point"))
                {
                    AddKeyPointAt(this.curve, this.curve.KeyPointsCount);
                }

                if (GUILayout.Button("Add Point and Select"))
                {
                    var point = AddKeyPointAt(this.curve, this.curve.KeyPointsCount);
                    Selection.activeGameObject = point.gameObject;
                }

                if (GUILayout.Button("Fix names of points"))
                {
                    RenamePoints(this.curve);
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

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = this.keyPoints.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // Draw "Add Before" button
            if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
            {
                AddKeyPointAt(this.curve, index);
            }

            // Draw point name
            EditorGUI.PropertyField(
                new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

            // Draw "Add After" button
            if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
            {
                AddKeyPointAt(this.curve, index + 1);
            }

            // Draw remove button
            if (this.curve.KeyPointsCount > 2)
            {
                if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
                {
                    //this.curve.RemoveKeyPointAt(index);
                    RemoveKeyPointAt(this.curve, index);
                }
            }
        }

        public static void DrawPointsSceneGUI(BezierCurve curve, BezierPoint exclude = null)
        {
            for (int i = 0; i < curve.KeyPointsCount; i++)
            {
                if (curve.KeyPoints[i] == exclude)
                {
                    continue;
                }

                BezierPointEditor.handleCapSize = BezierPointEditor.CircleCapSize;
                BezierPointEditor.DrawPointSceneGUI(curve.KeyPoints[i]);
            }
        }

        private static void RenamePoints(BezierCurve curve)
        {
            for (int i = 0; i < curve.KeyPointsCount; i++)
            {
                curve.KeyPoints[i].name = "Point " + i;
            }
        }

        private static BezierPoint AddKeyPointAt(BezierCurve curve, int index)
        {
            BezierPoint newPoint = new GameObject("Point " + curve.KeyPointsCount, typeof(BezierPoint)).GetComponent<BezierPoint>();
            newPoint.transform.parent = curve.transform;
            newPoint.transform.localRotation = Quaternion.identity;
            newPoint.Curve = curve;

            if (curve.KeyPointsCount == 0 || curve.KeyPointsCount == 1)
            {
                newPoint.LocalPosition = Vector3.zero;
            }
            else
            {
                if (index == 0)
                {
                    newPoint.Position = (curve.KeyPoints[0].Position - curve.KeyPoints[1].Position).normalized + curve.KeyPoints[0].Position;
                }
                else if (index == curve.KeyPointsCount)
                {
                    newPoint.Position = (curve.KeyPoints[index - 1].Position - curve.KeyPoints[index - 2].Position).normalized + curve.KeyPoints[index - 1].Position;
                }
                else
                {
                    newPoint.Position = BezierCurve.EvaluateCubicCurve(0.5f, curve.KeyPoints[index - 1], curve.KeyPoints[index]);
                }
            }

            Undo.RegisterCreatedObjectUndo(newPoint.gameObject, "Create point");
            Undo.RegisterCompleteObjectUndo(curve, "Save curve");
            curve.KeyPoints.Insert(index, newPoint);
            Undo.RegisterCompleteObjectUndo(curve, "Save curve");

            return newPoint;
        }

        private static bool RemoveKeyPointAt(BezierCurve curve, int index)
        {
            if (curve.KeyPointsCount < 2)
            {
                return false;
            }

            var point = curve.KeyPoints[index];

            Undo.RegisterCompleteObjectUndo(curve, "Save curve");
            curve.KeyPoints.RemoveAt(index);
            Undo.RegisterCompleteObjectUndo(curve, "Save curve");
            Undo.DestroyObjectImmediate(point.gameObject);

            return true;
        }
    }
}
