using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGradientVector : MonoBehaviour
{

    public LineRenderer lr;
    public Vector3 normal;
    public Plot3D plot;

    private calculus Calc;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        Calc = GetComponent<calculus>();

        //Debug.Log(Calc.stringyNumericGradient("Sin(x)+Cos(y)", 1.0f, 1.0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 GradientInput = new Vector2(plot.xDomain.x + (transform.position.x * plot.resolution * plot.horizontalScale), plot.yDomain.x + (transform.position.z * plot.resolution * plot.horizontalScale));
        Vector2 Gradient = Calc.stringyNumericGradient(plot.func, GradientInput.x, GradientInput.y);
        Debug.Log("GRAD: " + Gradient.ToString());
        Vector3[] linePoints = new Vector3[]
        {
            transform.position - new Vector3(Gradient.x, 0, Gradient.y),
            transform.position
        };

        lr.SetPositions(linePoints);
    }
}
