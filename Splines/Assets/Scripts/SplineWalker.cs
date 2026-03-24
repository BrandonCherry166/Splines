using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    [SerializeField] BezierSpline spline;
    [SerializeField] float speed = 0.2f;
    private float t;

    private void Update()
    {
        if (spline == null)
        {
            return;
        }

        t += speed * Time.deltaTime;

        //Handle Looping
        if (t > 1f)
        {
            t -= 1f;
        }

        Vector3 position = spline.Evaluate(t);
        Vector3 tangent = spline.EvaluateTangent(t);

        transform.position = position;

        if (tangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(tangent);
        }
    }
}
