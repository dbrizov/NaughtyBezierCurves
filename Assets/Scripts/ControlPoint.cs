using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public Vector3 Position
    {
        get
        {
            return this.transform.position;
        }
    }

    public bool IsPointOnCurve { get; set; }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }
}
