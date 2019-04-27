using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("exiting trigger");
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            Debug.Log("should be instantiating now");
            GameObject newItem = Instantiate(item);
            newItem.GetComponent<Positioner>().enabled = true;
        }
    }
}
