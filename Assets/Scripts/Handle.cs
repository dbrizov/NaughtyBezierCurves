using UnityEngine;

public class Handle : MonoBehaviour
{
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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.Start.Position, this.End.Position);
    }
}
