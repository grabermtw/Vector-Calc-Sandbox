using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positioner : MonoBehaviour
{
    public string spawnerTag;

    Transform spawner;
    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindWithTag(gameObject.tag + "Spawn").GetComponent<Transform>();
        transform.position = spawner.position;
        transform.rotation = spawner.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
