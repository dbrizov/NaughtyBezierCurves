using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    private BezierCurve bezierCurve;

    [MenuItem("GameObject/Create Other/Bezier Curve")]
    private static void CreateBezeirCurve()
    {
        Vector3 position = Vector3.zero;
        if (Camera.current != null)
        {
            position = Camera.current.transform.position + Camera.current.transform.forward * 10f;
        }

        GameObject bezierCurve = new GameObject("Bezier Curve", typeof(BezierCurve));
        bezierCurve.transform.position = position;
    }

    protected virtual void OnEnable()
    {
        this.bezierCurve = (BezierCurve)this.target;
    }

    protected virtual void OnSceneGUI()
    {
        this.bezierCurve.StartPointWorldPosition = Handles.DoPositionHandle(this.bezierCurve.StartPointWorldPosition, Quaternion.identity);
        this.bezierCurve.EndPointWorldPosition = Handles.DoPositionHandle(this.bezierCurve.EndPointWorldPosition, Quaternion.identity);        
        this.bezierCurve.StartTangentWorldPosition = Handles.DoPositionHandle(this.bezierCurve.StartTangentWorldPosition, Quaternion.identity);
        this.bezierCurve.EndTangentWorldPosition = Handles.DoPositionHandle(this.bezierCurve.EndTangentWorldPosition, Quaternion.identity);
    }
}