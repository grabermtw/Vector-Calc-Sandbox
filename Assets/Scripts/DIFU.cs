using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIFU : MonoBehaviour
{

    public Vector3 coord1;
    public Vector3 coord2;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position.x > coord1.x || transform.position.z > coord1.z || transform.position.x < coord2.x || transform.position.z < coord2.z)
        {
            Destroy(gameObject);
        }
    }

}
