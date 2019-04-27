using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivativeDeagle : MonoBehaviour
{
    public Plot3D plot;
    public calculus calc;
    public int iterations;
    public GameObject lineVert;
    public float resolutionFactor;

    private RaycastHit hit;
    private string func;
    private Vector2 p;
    private float theta;
    private List<GameObject> vertices;

    // Start is called before the first frame update
    void Start()
    {
        vertices = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            foreach (var vert in vertices)
            {
                Destroy(vert);
            }
            vertices.Clear();

            func = plot.func;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 10.0f))
            {
                var v = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                theta = Mathf.Atan2(v.normalized.y, v.normalized.x);
                DrawGraph();
            }
        }
    }

    private void DrawGraph()
    {
        var vert = hit.point;
        var resolution = plot.resolution * resolutionFactor;

        Debug.Log("Derivative Deagle function:" + func);

        for (int t = -iterations; t <= iterations; t++)
        {
            float xMathCoord = plot.xDomain.x + ((vert.x * plot.xSize) / plot.plotDimensions.x)*plot.resolution;
            float yMathCoord = plot.yDomain.x + ((vert.z * plot.zSize) / plot.plotDimensions.y)*plot.resolution;
            vert = new Vector3(hit.point.x + (t * plot.resolution * Mathf.Cos(theta)), calc.stringyEvaluate(func, xMathCoord, yMathCoord) / plot.verticalScale, hit.point.z + (t * plot.resolution * Mathf.Sin(theta)));
            vertices.Add(Instantiate(lineVert, vert, Quaternion.identity));
        }   
    }
}
