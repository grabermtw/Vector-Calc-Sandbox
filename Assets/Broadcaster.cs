using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillAllIncrements()
    {
        
        BroadcastMessage("IsLast", false, SendMessageOptions.DontRequireReceiver);
    }
}
