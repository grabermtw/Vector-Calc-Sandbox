using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{

    public float charge;

    private MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();

        if (charge < 0)
        {
            mr.material.color = Color.blue;
        }
        else
        {
            mr.material.color = Color.red;
        }
        
    }

}
