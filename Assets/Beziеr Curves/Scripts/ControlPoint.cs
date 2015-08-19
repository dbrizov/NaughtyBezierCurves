using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    [SerializeField]
    private Color gizmosColor = Color.red;

    public bool IsPointOnCurve { get; set; }

    public Vector3 Position
    {
        get
        {
            return this.transform.position;
        }
    }

    public Color GizmosColor
    {
        get
        {
            return this.gizmosColor;
        }
        set
        {
            this.gizmosColor = value;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = this.GizmosColor;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }
}
