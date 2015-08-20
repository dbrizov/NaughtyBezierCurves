using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    private const int DrawSegmentsCount = 25;

    [SerializeField]
    private Vector3 startPointLocalPosition = new Vector3(-2f, 0f, 0f);

    [SerializeField]
    private Vector3 endPointLocalPosition = new Vector3(2f, 0f, 0f);

    [SerializeField]
    private Vector3 startTangentLocalPosition = new Vector3(-1f, 1f, 0f);

    [SerializeField]
    private Vector3 endTangentLocalPosition = new Vector3(1f, 1f, 0f);

    // Local Positions
    public Vector3 StartPointLocalPosition
    {
        get
        {
            return this.startPointLocalPosition;
        }
        set
        {
            Vector3 relativePosition = this.startTangentLocalPosition - this.startPointLocalPosition;
            this.startPointLocalPosition = value;
            this.startTangentLocalPosition = this.startPointLocalPosition + relativePosition;
        }
    }

    public Vector3 EndPointLocalPosition
    {
        get
        {
            return this.endPointLocalPosition;
        }
        set
        {

            Vector3 relativePosition = this.endTangentLocalPosition - this.endPointLocalPosition;
            this.endPointLocalPosition = value;
            this.endTangentLocalPosition = this.endPointLocalPosition + relativePosition;
        }
    }

    public Vector3 StartTangentLocalPosition
    {
        get
        {
            return this.startTangentLocalPosition;
        }
        set
        {
            this.startTangentLocalPosition = value;
        }
    }

    public Vector3 EndTangentLocalPosition
    {
        get
        {
            return this.endTangentLocalPosition;
        }
        set
        {
            this.endTangentLocalPosition = value;
        }
    }

    // World Positions
    public Vector3 StartPointWorldPosition
    {
        get
        {
            return this.transform.TransformPoint(this.StartPointLocalPosition);
        }
        set
        {
            this.StartPointLocalPosition = this.transform.InverseTransformPoint(value);
        }
    }

    public Vector3 EndPointWorldPosition
    {
        get
        {
            return this.transform.TransformPoint(this.EndPointLocalPosition);
        }
        set
        {
            this.EndPointLocalPosition = this.transform.InverseTransformPoint(value);
        }
    }

    public Vector3 StartTangentWorldPosition
    {
        get
        {
            return this.transform.TransformPoint(this.StartTangentLocalPosition);
        }
        set
        {
            this.StartTangentLocalPosition = this.transform.InverseTransformPoint(value);
        }
    }

    public Vector3 EndTangentWorldPosition
    {
        get
        {
            return this.transform.TransformPoint(this.EndTangentLocalPosition);
        }
        set
        {
            this.EndTangentLocalPosition = this.transform.InverseTransformPoint(value);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        // Draw the curve
        Gizmos.color = Color.green;
        Vector3 from = this.Evaluate(0f);
        Vector3 to = Vector3.zero;

        for (int i = 1; i <= DrawSegmentsCount; i++)
        {
            float time = i / (float)DrawSegmentsCount;
            to = this.Evaluate(time);

            Gizmos.DrawLine(from, to);

            from = to;
        }

        // Draw the Start Handle
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.StartPointWorldPosition, 0.075f);
        Gizmos.DrawSphere(this.StartTangentWorldPosition, 0.075f);
        Gizmos.DrawLine(this.StartPointWorldPosition, this.StartTangentWorldPosition);

        // Draw the End Handle
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.EndPointWorldPosition, 0.075f);
        Gizmos.DrawSphere(this.EndTangentWorldPosition, 0.075f);
        Gizmos.DrawLine(this.EndPointWorldPosition, this.EndTangentWorldPosition);
    }

    public Vector3 Evaluate(float time)
    {
        Vector3 result = BezierCurve.Evaluate(
            time, this.StartPointWorldPosition, this.EndPointWorldPosition, this.StartTangentWorldPosition, this.EndTangentWorldPosition);

        return result;
    }

    public static Vector3 Evaluate(float time, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
    {
        float u = 1f - time;
        float tt = time * time;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * time;

        Vector3 result = uuu * startPosition; // first term
        result += 3 * uu * time * startTangent; // second term
        result += 3 * u * tt * endTangent; // third term
        result += ttt * endPosition; // fourth term

        return result;
    }
}
