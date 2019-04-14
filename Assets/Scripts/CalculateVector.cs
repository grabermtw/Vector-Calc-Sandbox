using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateVector : MonoBehaviour
{
    private Vector3 r;
    public float k;
    public float VectorScale;    
    public SpawnObjects spawnObjects;

    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = new Vector3();
        foreach (var charge in spawnObjects.charges)
        {
            r = charge.transform.position - transform.position; // calculates the vector pointing from this object to given charge
            float kq = k * charge.GetComponent<Charge>().charge;

            force += (kq / (Mathf.Pow(r.magnitude, 2))) * r.normalized;
        }

        Vector3[] positions = new Vector3[2]
        {
            transform.position,
            transform.position + (force.normalized * VectorScale)
        };

        lr.SetPositions(positions);
    }
}
