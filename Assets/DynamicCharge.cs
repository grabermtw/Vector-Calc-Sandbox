using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCharge : MonoBehaviour
{
    public SpawnObjects spawnObjects;
    public float k;
    public float lifeTime;
    private Rigidbody rb;
    private float timer;

    public string chargeTag;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 force = new Vector3();
        foreach (var charge in spawnObjects.charges)
        {
            Vector3 r = charge.transform.position - transform.position;
            force += (k * charge.GetComponent<Charge>().charge / (Mathf.Pow(r.magnitude, 2))) * r.normalized;
        }
        rb.AddForce(force);

        timer += Time.fixedDeltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == chargeTag)
        {
            Destroy(this.gameObject);
        }

        Debug.Log("Destroyed Charge");
    }
}
