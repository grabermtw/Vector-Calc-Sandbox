using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnChargeArrays : MonoBehaviour
{

    public float boundary = 0.0f;
    public Sprite[] selectedSprites;
    public Sprite[] normalSprites;
    public SpriteRenderer[] buttons;
    public Animator[] anims;
    private int selectedIndex;
    private int oldSelectedIndex;
    private Vector2 oldStickPos;

    public SpawnObjects so;

    public GameObject testChargePrefab;
    public int[] sizes;
    public float spacing;

    public int shells;
    public float theta;
    public float phi;
    public float r;

    public int cylinders;
    public int layers;
    public float cylTheta;
    public float zStep;
    public float rStep;

    // Start is called before the first frame update
    void Start()
    {
        oldStickPos = new Vector2();
        selectedIndex = 0;
        oldSelectedIndex = 0;

        phi = theta / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 stickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (stickPos.x < -boundary)
        {
            oldSelectedIndex = selectedIndex;
            buttons[selectedIndex].sprite = normalSprites[selectedIndex];

            buttons[2].sprite = selectedSprites[2];

            selectedIndex = 2;

            /*
            if (oldSelectedIndex != selectedIndex)
            {
                anims[selectedIndex].Play("Shrink");
                anims[0].Play("Grow");
            }
            */
        }
        else if (stickPos.x > boundary)
        {
            oldSelectedIndex = selectedIndex;
            buttons[selectedIndex].sprite = normalSprites[selectedIndex];


            buttons[0].sprite = selectedSprites[0];

            selectedIndex = 0;

        }
        else
        {
            oldSelectedIndex = selectedIndex;
            buttons[selectedIndex].sprite = normalSprites[selectedIndex];

            

            buttons[1].sprite = selectedSprites[1];
            selectedIndex = 1;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
        {
            switch (selectedIndex)
            {
                case 0:
                    GenerateCylindricalChargeArray();
                    break;

                case 1:
                    GenerateRectangularChargeArray();
                    break;

                case 2:
                    GenerateSphericalChargeArray();
                    break;
            }
            
        }

    }

    private void GenerateCylindricalChargeArray()
    {
        for (int i = 0; i < cylinders; i++)
        {
            for (int j = 0; j < cylTheta; j++)
            {
                for (int k = 0; k < layers; k++)
                {
                    var charge1 = Instantiate(testChargePrefab, new Vector3((r * i) * Mathf.Cos((2 * Mathf.PI)/cylTheta * j), k * zStep, (r * i) * Mathf.Sin((2 * Mathf.PI) / cylTheta * j)), Quaternion.identity);
                    charge1.GetComponent<DynamicCharge>().spawnObjects = so;
                }
            }
        }
    }

    private void GenerateRectangularChargeArray()
    {
        for (int i = 0; i < sizes[0]; i++)
        {
            for (int j = 0; j < sizes[1]; j++)
            {
                for (int k = 0; k < sizes[2]; k++)
                {
                    var charge1 = Instantiate(testChargePrefab, new Vector3(spacing * i, spacing * j, spacing * k), Quaternion.identity);
                    var charge2 = Instantiate(testChargePrefab, new Vector3(-spacing * i, spacing * j, spacing * k), Quaternion.identity);
                    var charge3 = Instantiate(testChargePrefab, new Vector3(spacing * i, spacing * j, -spacing * k), Quaternion.identity);
                    var charge4 = Instantiate(testChargePrefab, new Vector3(-spacing * i, spacing * j, -spacing * k), Quaternion.identity);
                    charge1.GetComponent<DynamicCharge>().spawnObjects = so;
                    charge2.GetComponent<DynamicCharge>().spawnObjects = so;
                    charge3.GetComponent<DynamicCharge>().spawnObjects = so;
                    charge4.GetComponent<DynamicCharge>().spawnObjects = so;
                }
            }
        }
    }

    private void GenerateSphericalChargeArray()
    {
        for (int i = 0; i < shells; i++)
        {
            for (int j = 0; j < theta; j++)
            {
                for (int k = 0; k < phi; k++)
                {
                    var charge1 = Instantiate(testChargePrefab, new Vector3((r * i) * Mathf.Cos((Mathf.PI/phi)*k - (Mathf.PI/2))*Mathf.Cos(((2*Mathf.PI)/theta)*j), (r * i) * Mathf.Cos((Mathf.PI / phi) * k - (Mathf.PI / 2)) * Mathf.Sin(((2 * Mathf.PI) / theta) * j), (r * i) * Mathf.Sin((Mathf.PI / phi) * k - (Mathf.PI / 2))) + transform.position, Quaternion.identity);
                    charge1.GetComponent<DynamicCharge>().spawnObjects = so;
                }
            }
        }
    }
}
