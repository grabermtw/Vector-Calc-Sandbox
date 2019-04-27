using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot3D : MonoBehaviour
{

    public Vector2 xDomain;
    public Vector2 yDomain;
    public float resolution;
    public float horizontalScale;
    public float verticalScale;
    public string func;
    public TakeInput takeInput;
    public GameObject pointer1;
    public GameObject pointer2;
    public Vector2 plotDimensions;
    public GameObject ErrorText;

    [HideInInspector]
    public int zSize;
    [HideInInspector]
    public int xSize;
    private calculus calc;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        var mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.cookingOptions = MeshColliderCookingOptions.None;

        calc = GetComponent<calculus>();

        //PlotFunction();
        //UpdateMesh();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            func = takeInput.GetExpr();
            PlotFunction();
            UpdateMesh();
            pointer1.SetActive(true);
            pointer2.SetActive(true);
        }
    }

    public void PlotFunction ()
    {
        xSize = (int)(xDomain.magnitude / resolution);
        zSize = (int)(yDomain.magnitude / resolution);

        vertices = new Vector3[xSize * zSize];

        Debug.Log(xSize.ToString() + " " + zSize.ToString());

        int i = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                try
                {
                    vertices[i] = new Vector3(x * (plotDimensions.x / xSize), calc.stringyEvaluate(func, xDomain.x + x * resolution, yDomain.x + z * resolution) / verticalScale, z * (plotDimensions.y / zSize));
                }
                catch(System.FormatException e)
                {
                    StartCoroutine("DisplayErrorText");
                }
                    i++;
            }
        }

        //xSize += 1;
        //zSize += 1;

        /*
        triangles = new int[6];
        triangles[0] = 1;
        triangles[1] = 0;
        triangles[2] = xSize;
        triangles[3] = xSize;
        triangles[4] = xSize + 1;
        triangles[5] = 1;
        */

        
        triangles = new int[xSize * zSize  * 6];
        int tris = 0;
        int vert = 0;
        for (int k = 0; k < zSize - 1; k++)
        {
            for (int j = 0; j < xSize - 1; j++)
            {
                triangles[tris + 0] = vert + xSize;
                triangles[tris + 1] = vert + 0;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize;

                vert++;
                tris += 6;
            }
            vert++;
        }

        
        

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;
    }

    float Func1 (float x, float y)
    {
        float num = Mathf.Sin(x) + Mathf.Cos(y);
        return num;
    }

    IEnumerator DisplayErrorText ()
    {
        ErrorText.SetActive(true);
        yield return new WaitForSeconds(3);
        ErrorText.SetActive(false);
    }
}
