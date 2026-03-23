using UnityEngine;

public class BezierKnot : MonoBehaviour
{
    public Vector3 ControlPointIn()
    {
        return (transform.position - transform.right * transform.localScale.x);
    }

    public Vector3 ControlPointOut()
    {
        return (transform.position + transform.right * transform.localScale.x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ControlPointIn(), 0.08f);
        Gizmos.DrawSphere(ControlPointOut(), 0.08f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ControlPointIn(), ControlPointOut());
    }
}
