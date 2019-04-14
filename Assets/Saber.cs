using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{
    public string Tag;
    public SpawnObjects spawnObjects;

    private void OnTriggerEnter(Collider other)
    {
        spawnObjects.charges.Remove(other.gameObject);
        Destroy(other.gameObject);
    }
}
