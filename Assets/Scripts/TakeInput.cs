using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TakeInput : MonoBehaviour
{
    int blockNum;

    string expr;

    public GameObject trigBlock;
    public GameObject digitBlock;
    public GameObject[] ops;
    public GameObject blockParent;
    public GameObject pedestal;

    public Transform xMarker;

    Stack<GameObject> currentBlocks;

    string[] opstrings;

    Dictionary<string, GameObject> opsMap;

    // Start is called before the first frame update
    void Start()
    {
        
        currentBlocks = new Stack<GameObject>();
        opsMap = new Dictionary<string, GameObject>();

        foreach(GameObject obj in ops)
        {
            opsMap.Add(obj.tag, obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetExpr();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PopBlock();
            UpdateBlocksArray();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trig"))
        {
            AddTrig();
           
        }
        else if (other.gameObject.CompareTag("Digit"))
        {
            AddDigit();
        }
        else
        {
            AddOp(other.gameObject.tag);
        }
        
        Destroy(other.gameObject);      
    }

    public void AddDigit()
    {
        GameObject obj = Instantiate(digitBlock, blockParent.GetComponent<Transform>());
        obj.GetComponent<BoxCollider>().enabled = false;
        UpdateBlocksArray();
    }

    public void AddTrig()
    {
        GameObject obj = Instantiate(trigBlock, blockParent.GetComponent<Transform>());
        obj.GetComponent<BoxCollider>().enabled = false;
        UpdateBlocksArray();
    }

    public void AddOp(string blockTag)
    {
        GameObject obj = Instantiate(opsMap[blockTag], blockParent.GetComponent<Transform>());
        obj.GetComponent<BoxCollider>().enabled = false;
        UpdateBlocksArray();
    }

    public void UpdateBlocksArray()
    {   
        Transform[] allBlockObjs = blockParent.GetComponentsInChildren<Transform>();
        List<GameObject> updatedBlocks = new List<GameObject>();

        foreach(Transform trans in allBlockObjs)
        {
            Debug.Log("Object name: " + trans.gameObject.name + ", layer: " + trans.gameObject.layer);
            if (trans.gameObject.layer == 9)
            {
                updatedBlocks.Add(trans.gameObject);
            }
        }

        currentBlocks = new Stack<GameObject>(updatedBlocks);


        float xpos = xMarker.position.x;
        float rot = 0f;
        float elevate;
        for (int i = 0; i < updatedBlocks.Count; i++)
        {
            elevate = 0.65f;
            switch (updatedBlocks[i].tag)
            {
                case "-":
                    elevate = 0.6f;
                    break;
                case "+":
                    elevate = 0.63f;
                    break;
                case "*":
                    elevate = 0.63f;
                    break;
                case "y":
                    elevate = 0.61f;
                    break;
                case "x":
                    elevate = 0.62f;
                    break;
                case "z":
                    elevate = 0.63f;
                    break;
                case ".":
                    elevate = 0.56f;
                    break;
                case "div":
                    elevate = 0.53f;
                    break;
                case "π":
                    elevate = 0.62f;
                    break;

            }
            if (updatedBlocks[i].CompareTag("(") ||
                updatedBlocks[i].CompareTag(")") ||
                updatedBlocks[i].CompareTag("y") ||
                updatedBlocks[i].CompareTag("e") ||
                updatedBlocks[i].CompareTag("+") ||
                updatedBlocks[i].CompareTag("π") ||
                updatedBlocks[i].CompareTag("Log (") ||
                updatedBlocks[i].CompareTag("Expr (") ||
                updatedBlocks[i].CompareTag("div") ||
                updatedBlocks[i].CompareTag("x") ||
                updatedBlocks[i].CompareTag("*") ||
                updatedBlocks[i].CompareTag("^") ||
                updatedBlocks[i].CompareTag("z"))
            {
                rot = 180;
            }
            else if(updatedBlocks[i].tag == "Sqrt (")
            {
                rot = -90;
            }
            else
            {
                rot = 0;
            }
            if (updatedBlocks[i].CompareTag("div"))
            {
                updatedBlocks[i].GetComponent<Transform>().rotation = Quaternion.Euler(0, rot, 0);
            }
            updatedBlocks[i].GetComponent<Transform>().rotation = Quaternion.Euler(0, rot, 0);
            updatedBlocks[i].GetComponent<Transform>().position = new Vector3(xpos, pedestal.GetComponent<Transform>().position.y + elevate, pedestal.GetComponent<Transform>().position.z);
            
            if (updatedBlocks[i].CompareTag("Trig") || updatedBlocks[i].CompareTag("Log (") || updatedBlocks[i].CompareTag("Expr ("))
            {
                xpos += .3f;
            }
            else if (updatedBlocks[i].CompareTag("Sqrt ("))
            {
                xpos += .15f;
            }
            else if (updatedBlocks[i].CompareTag("Digit"))
            {
                xpos += 0.12f;
            }
            else if (updatedBlocks[i].CompareTag("!") || updatedBlocks[i].CompareTag("."))
            {
                xpos += .04f;
            }
            else
            {
                xpos += 0.11f;
            }
        }

        //Update Incementation (I know I forgot the "r") permissions when relevant
        // Brute force solution cause everything else failed me:
        Debug.Log("setting all isLasts to false");
        blockParent.GetComponent<Broadcaster>().KillAllIncrements();

        if (currentBlocks.Count > 0)
        {
            if (currentBlocks.Peek().GetComponent<Incement>() != null)
            {
                Debug.Log("Setting isLast to true");
                currentBlocks.Peek().GetComponent<Incement>().IsLast(true);
            }
        }
        Debug.Log("Current currentBlocks: " + currentBlocks);
    }

    public void PopBlock()
    {
        if (currentBlocks.Count > 0)
        {
            Destroy(currentBlocks.Pop());
        }
        UpdateBlocksArray();
    }

    private void AddToExpr(string s, bool space)
    {
        if (space)
        {
            expr += " " + s;
        }
        else
        {
            expr += s;
        }
    }

    public string GetExpr()
    {
        GameObject[] newCurrentBlocks = currentBlocks.ToArray();
        expr = "";
        bool digit = false;
        for(int i = currentBlocks.Count - 1; i >= 0; i--)
        {
            //Handles digits and concatenation:
            if (newCurrentBlocks[i].CompareTag("Digit"))
            {
                if (newCurrentBlocks[i].GetComponent<TextMesh>().text != "")
                {
                    newCurrentBlocks[i].GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(true);
                    newCurrentBlocks[i].GetComponent<TextMesh>().text = "";
                }
                Transform[] thing = newCurrentBlocks[i].GetComponentsInChildren<Transform>(false);
                if (!digit)
                {
                    Debug.Log(thing.Length);
                    AddToExpr(thing[1].gameObject.tag, true);
                }
                else
                {
                    Debug.Log(thing.Length);
                    AddToExpr(thing[1].gameObject.tag, false);
                }
                digit = true;
            }
            else if (newCurrentBlocks[i].CompareTag("."))
            {
                if (!digit)
                {
                    AddToExpr(newCurrentBlocks[i].gameObject.tag, true);
                }
                else
                {
                    AddToExpr(newCurrentBlocks[i].gameObject.tag, false);
                }
                digit = true;
            }
            //Handles trig functions (which have their values in the child components
            else if (newCurrentBlocks[i].CompareTag("Trig"))
            {
                if (newCurrentBlocks[i].GetComponent<TextMesh>().text != "")
                {
                    newCurrentBlocks[i].GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(true);
                    newCurrentBlocks[i].GetComponent<TextMesh>().text = "";
                }
                Transform[] thing = newCurrentBlocks[i].GetComponentsInChildren<Transform>(false);
                AddToExpr(thing[1].gameObject.tag, true);
                digit = false;
            }
            //All others
            else { 
                AddToExpr(newCurrentBlocks[i].tag, true);
                digit = false;
            }
        }
        string finalString = expr.Replace("div", "/");
        Debug.Log(finalString);
        return finalString;
    }
}
