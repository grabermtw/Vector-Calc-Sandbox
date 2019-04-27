using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform point;
    public Transform player;
    public float waitTime;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine("teleport");
    }

    public IEnumerator teleport()
    {
        yield return new WaitForSeconds(waitTime);

        player.SetPositionAndRotation(point.position, point.rotation);
    }
}
