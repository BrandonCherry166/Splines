using UnityEngine;
using System.Collections.Generic;
public class BezierSpline : MonoBehaviour
{
    public List<BezierKnot> knots = new List<BezierKnot>();

    public int SegmentCount()
    {
        return knots.Count;
    }

    public Vector3 Evaluate(float T)
    {
        if (knots.Count < 2)
        {
            return Vector3.zero;
        }


        foreach (var knot in knots)
        {
            if (knot == null)
            {
                return Vector3.zero;
            }
        }

        GetSegmentAndLocalT(T, out int i, out float t);

        Vector3 p0 = knots[i].transform.position;
        Vector3 p1 = knots[i].ControlPointOut();
        Vector3 p2 = knots[i + 1].ControlPointIn();
        Vector3 p3 = knots[i + 1].transform.position;

        return CubicBezier(p0, p1, p2, p3, t);
    }

    public Vector3 EvaluateTangent(float T)
    {
        if (knots.Count < 2)
        {
            return Vector3.zero;
        }

        GetSegmentAndLocalT(T, out int i, out float t);
        
        Vector3 p0 = knots[i].transform.position;
        Vector3 p1 = knots[i].ControlPointOut();
        Vector3 p2 = knots[i + 1].ControlPointIn();
        Vector3 p3 = knots[i + 1].transform.position;

        return CubicBezierDerivative(p0, p1, p2, p3, t);
    }

    //Bezier Math
    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t2 = t * t;
        float t3 = t2 * t;

        return (u3 * p0) + (3f * u2 * t * p1) + (3f * u * t2 * p2) + (t3 * p3); 
    }

    private Vector3 CubicBezierDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return (3f * u * u * (p1 - p0)) + (6f * u * t * (p2 - p1)) + (3f * t * t * (p3 - p2));
    }

    //Helper Methods
    private void GetSegmentAndLocalT(float t, out int segmentIndex, out float localT)
    {
        t = Mathf.Clamp01(t);

        float scaled = t * SegmentCount();
        segmentIndex = Mathf.Min(Mathf.FloorToInt(scaled), SegmentCount() - 1);
        localT = scaled - segmentIndex;
    }

    //Gizmos
    private void OnDrawGizmos()
    {
        if (knots.Count < 2)
        {
            return;
        }

        int steps = 50 * SegmentCount();
        Vector3 prev = Evaluate(0f);

        for (int s = 1; s < steps; s++)
        {
            float T = s / (float)steps;
            Vector3 current = Evaluate(T);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(prev, current);

            prev = current;
        }

        foreach (BezierKnot knot in knots)
        {
            if (knot == null)
            {
                continue;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(knot.ControlPointIn(), knot.transform.position);
            Gizmos.DrawLine(knot.transform.position, knot.ControlPointOut());

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(knot.ControlPointIn(), 0.08f);
            Gizmos.DrawSphere(knot.ControlPointOut(), 0.08f);
        }
    }
}
