using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricPlot : MonoBehaviour
{
    public Vector2 Domain;
    public Plot3D plot;
    public float resolution;
    public string func;
    public Vector2 v;
    private calculus calc;

    public GameObject lineSegPF;

    private Vector2 p;
    private float theta = 0.0f;
    private float omega;
    private float alpha;
    private float phi;
    private RaycastHit hit;
    private Vector3[] vertices;
    private float swing;

    // Start is called before the first frame update
    void Start()
    {
        calc = GetComponent<calculus>();

        int n = (int)(Domain.magnitude / resolution);
        Debug.Log(n);

        DrawPlot();

        swing = Mathf.PI / 2.1f;
        func = plot.func;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            func = plot.func;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 10.0f))
            {
                p = new Vector2((hit.point.x * plot.resolution * plot.horizontalScale), ((hit.point.z * plot.resolution * plot.horizontalScale)));
                p = new Vector2(p.y, p.x);
                Debug.Log(p);
                Debug.Log(hit.point);
                alpha = Mathf.Atan2(p.normalized.x, p.normalized.y);
            }

            v = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            //Debug.Log("V " + v);
            omega = Mathf.Atan2(v.normalized.x, v.normalized.y);
            phi = omega - (Mathf.PI / 2);
            Domain = new Vector2(phi - swing, phi + swing);

            DrawPlot();

            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                DrawPhi();
            }
        }
    }

    public void DrawPlot()
    {
        float beta = Mathf.Atan2(resolution, p.magnitude);
        int n = (int)(Domain.magnitude / resolution);
        for (int i = 0; i < n; i++)
        {
            float t = (i * beta);
            Vector3 vert = new Vector3(R(t) * Mathf.Cos(t) * (plot.plotDimensions.x/ (2*Mathf.PI)), calc.stringyEvaluate(func, plot.xDomain.x + R(t) * Mathf.Cos(t), plot.yDomain.x + R(t) * Mathf.Sin(t)) / plot.verticalScale, R(t) * Mathf.Sin(t) * (plot.plotDimensions.y/ ((2 * Mathf.PI))));

            var seg = Instantiate(lineSegPF, vert, Quaternion.identity);
        }
    }

    float R (float theta)
    {
        float num = (p.magnitude * Mathf.Cos(alpha - phi)) / (Mathf.Cos(theta - phi));
        return num;
    }


    public void DrawPhi()
    {
        Vector3 vert = new Vector3(R(phi) * Mathf.Cos(phi), calc.stringyEvaluate(func, plot.xDomain.x + R(phi) * Mathf.Cos(phi), plot.yDomain.x + R(phi) * Mathf.Sin(phi)) / plot.verticalScale, R(phi) * Mathf.Sin(phi));

        var seg = Instantiate(lineSegPF, vert, Quaternion.identity);
    }
}
