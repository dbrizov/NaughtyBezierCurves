using UnityEngine;
using UnityEditor;

public class BezierCurveMenuItems
{
    [MenuItem("GameObject/Create Other/Bezier Curve")]
    private static void CreateBezeirCurve()
    {
        Vector3 position = Vector3.zero;
        if (Camera.current != null)
        {
            position = Camera.current.transform.position + Camera.current.transform.forward * 10f;
        }

        BezierCurve bezierCurve = new GameObject("Bezier Curve").AddComponent<BezierCurve>();
        bezierCurve.transform.position = position;

        Handle firstHandle = new GameObject("First Handle").AddComponent<Handle>();
        firstHandle.Start = new GameObject("Start Control Point").AddComponent<ControlPoint>();
        firstHandle.Start.transform.localPosition = Vector3.zero;
        firstHandle.End = new GameObject("End Control Point").AddComponent<ControlPoint>();
        firstHandle.End.transform.localPosition = new Vector3(1f, 1f, 0f);
        firstHandle.GizmosColor = Color.red;
        firstHandle.Start.GizmosColor = Color.red;
        firstHandle.End.GizmosColor = Color.red;

        bezierCurve.FirstHandle = firstHandle;
        bezierCurve.FirstHandle.transform.localPosition = new Vector3(-1.5f, 0f, 0f);

        Handle secondHandle = new GameObject("Second Handle").AddComponent<Handle>();
        secondHandle.Start = new GameObject("Start Control Point").AddComponent<ControlPoint>();
        secondHandle.Start.transform.localPosition = Vector3.zero;
        secondHandle.End = new GameObject("End Control Point").AddComponent<ControlPoint>();
        secondHandle.End.transform.localPosition = new Vector3(-1f, 1f, 0f);
        secondHandle.GizmosColor = Color.blue;
        secondHandle.Start.GizmosColor = Color.blue;
        secondHandle.End.GizmosColor = Color.blue;

        bezierCurve.SecondHandle = secondHandle;
        bezierCurve.SecondHandle.transform.localPosition = new Vector3(1.5f, 0f, 0f);
    }
}
