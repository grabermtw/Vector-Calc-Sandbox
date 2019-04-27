using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDot : MonoBehaviour
{
    private RaycastHit hit;
    public float maxDist = 10.0f;
    public MeshCollider plot;

    public GameObject pointer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (plot.Raycast(new Ray (transform.position, transform.forward), out hit, maxDist))
        {
            pointer.GetComponent<LineRenderer>().enabled = true;
            pointer.transform.position = hit.point;
        }
        else
        {
            pointer.GetComponent<LineRenderer>().enabled = false;
        }
    }
}
