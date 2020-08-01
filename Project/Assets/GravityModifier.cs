using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
	
	public Rigidbody rb;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * 10f);
    }
}
