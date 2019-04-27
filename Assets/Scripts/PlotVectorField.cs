using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotVectorField : MonoBehaviour
{
    public GameObject vFieldArrowPrefab;
    private calculus3D calc;
    public float vectorScale;
    // Start is called before the first frame update
    void Start()
    {
        calc = GetComponent<calculus3D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            DrawVField(calc.constantFlow);
        }
    }

    void DrawVField(string[] name)
    {
        List<List<Vector3>> vectors = calc.rectTestPoints(name);

        foreach (var point in vectors)
        {
            GameObject obj = Instantiate(vFieldArrowPrefab, point[0], Quaternion.identity);
            obj.GetComponent<LineRenderer>().SetPositions(new Vector3[] { point[0], (point[0] + point[1]).normalized * (1 + Mathf.Log((point[0] + point[1]).magnitude)) * vectorScale});
        }
    }
}
