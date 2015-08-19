using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    [SerializeField]
    private int drawSegmentsCount = 25;

    [SerializeField]
    private Handle firstHandle;

    [SerializeField]
    private Handle secondHandle;

    public Handle FirstHandle
    {
        get
        {
            return this.firstHandle;
        }
        set
        {
            this.firstHandle = value;
            this.firstHandle.transform.parent = this.transform;
        }
    }

    public Handle SecondHandle
    {
        get
        {
            return this.secondHandle;
        }
        set
        {
            this.secondHandle = value;
            this.secondHandle.transform.parent = this.transform;
        }
    }

    public Vector3 StartPosition
    {
        get
        {
            return this.FirstHandle.Start.Position;
        }
    }

    public Vector3 EndPosition
    {
        get
        {
            return this.SecondHandle.Start.Position;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        // Draw the curve
        Gizmos.color = Color.green;
        Vector3 firstPoint = this.Evaluate(0f);
        Vector3 secondPoint = Vector3.zero;

        for (int i = 1; i <= this.drawSegmentsCount; i++)
        {
            float t = i / (float)this.drawSegmentsCount;
            secondPoint = this.Evaluate(t);

            Gizmos.DrawLine(firstPoint, secondPoint);

            firstPoint = secondPoint;
        }
    }

    public Vector3 Evaluate(float t)
    {
        Vector3 result = BezierCurve.Evaluate(
            t,
            this.FirstHandle.Start.Position,
            this.FirstHandle.End.Position,
            this.SecondHandle.End.Position,
            this.SecondHandle.Start.Position);

        return result;
    }

    public static Vector3 Evaluate(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 result = uuu * p0; // first term
        result += 3 * uu * t * p1; // second term
        result += 3 * u * tt * p2; // third term
        result += ttt * p3; // fourth term

        return result;
    }
}
