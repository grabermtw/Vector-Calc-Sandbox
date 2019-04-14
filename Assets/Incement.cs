using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incement : MonoBehaviour
{
   

    Transform[] children;

    int index;
    bool enable = false;

    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Transform>(true);
        index = 1;
    }

    public void IsLast(bool last)
    {
        enable = last;
    }

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp))
            {
                Increment();
            }
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown))
            {
                Decrement();
            }
        }
    }

    public void Increment()
    {
        GetComponent<TextMesh>().text = "";
        Debug.Log("Up Arrow just pressed, iterating now");
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
        }
        children[index].gameObject.SetActive(true);
        if (index < children.Length - 1)
        {
            index++;
        }
        else
        {
            index = 1;
        }
    }

    public void Decrement()
    {
        GetComponent<TextMesh>().text = "";
        Debug.Log("Down Arrow just pressed, iterating now");
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
        }
        children[index].gameObject.SetActive(true);
        if (index > 1)
        {
            index--;
        }
        else
        {
            index = children.Length - 1;
        }
    }
}
