using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDynamicCharges : MonoBehaviour
{
    public GameObject dynamicChargePrefab;
    public GameObject testPointPrefab;
    public GameObject ChargeConfigMenu;
    public SpawnObjects spawnObjects;

    private bool held = false;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four) || held)
        {
            var charge = Instantiate(dynamicChargePrefab, transform.position, Quaternion.identity);
            charge.GetComponent<DynamicCharge>().spawnObjects = spawnObjects;

            if (OVRInput.GetUp(OVRInput.Button.Four))
            {
                held = false;
            }
            else
            {
                held = true;
            }
            
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) //Spawn test points
        {
            var point = Instantiate(testPointPrefab, transform.position, Quaternion.identity);
            point.GetComponent<CalculateVector>().spawnObjects = spawnObjects;
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            ChargeConfigMenu.SetActive(!ChargeConfigMenu.activeSelf);
        }
    }
}
