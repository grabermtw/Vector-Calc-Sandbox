using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowStick : MonoBehaviour
{
    public float threshold;

    void Update()
    {
        Vector2 p = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Debug.Log(p.magnitude);
        if (p.magnitude > threshold)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, (180/Mathf.PI) * Mathf.Atan2(p.y, p.x), transform.localRotation.z);
        }
    }
}
