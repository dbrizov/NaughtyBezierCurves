using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    private BezierCurve bezierCurve;

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
        this.bezierCurve = (BezierCurve)this.target;
    }

    protected virtual void OnSceneGUI()
    {
        this.bezierCurve.StartPointWorldPosition = Handles.PositionHandle(this.bezierCurve.StartPointWorldPosition, Quaternion.identity);
        this.bezierCurve.EndPointWorldPosition = Handles.PositionHandle(this.bezierCurve.EndPointWorldPosition, Quaternion.identity);
        this.bezierCurve.StartTangentWorldPosition = Handles.PositionHandle(this.bezierCurve.StartTangentWorldPosition, Quaternion.identity);
        this.bezierCurve.EndTangentWorldPosition = Handles.PositionHandle(this.bezierCurve.EndTangentWorldPosition, Quaternion.identity);
    }
}