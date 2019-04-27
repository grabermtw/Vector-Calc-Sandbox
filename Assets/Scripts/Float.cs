using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float speed;
    public float range;
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(0, range * Mathf.Sin(speed * Time.time), 0);
    }
}
