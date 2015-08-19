using UnityEngine;

public class Handle : MonoBehaviour
{
    [SerializeField]
    private Color gizmosColor = Color.red;

    [SerializeField]
    private ControlPoint start;

    [SerializeField]
    private ControlPoint end;

    public ControlPoint Start
    {
        get
        {
            return this.start;
        }
        set
        {
            this.start = value;
            this.start.transform.parent = this.transform;
            this.start.IsPointOnCurve = true;
        }
    }

    public ControlPoint End
    {
        get
        {
            return this.end;
        }
        set
        {
            this.end = value;
            this.end.transform.parent = this.Start.transform;
            this.end.IsPointOnCurve = false;
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
        Gizmos.DrawLine(this.Start.Position, this.End.Position);
    }
}
