using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject chargePrefab;
    public List<GameObject> charges;

    // Update is called once per frame
    void Update()
    {
        //Spawn Positive Charges
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            GameObject charge = Instantiate(chargePrefab, transform.position, Quaternion.identity);
            charges.Add(charge);
        }

        //Spawn Negative Charges
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            GameObject charge = Instantiate(chargePrefab, transform.position, Quaternion.identity);
            charge.GetComponent<Charge>().charge = -1.0f;
            charges.Add(charge);
        }
    }
}
