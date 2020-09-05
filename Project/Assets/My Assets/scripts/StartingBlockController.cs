using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlockController : MonoBehaviour
{
	
	public Rigidbody rb;
	public GameObject rightPedal;
	public GameObject leftPedal;
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void adjustPedals(float legX){
		Vector3 leftPedalPos = leftPedal.transform.position;
		Vector3 rightPedalPos = (leftPedalPos) + (transform.forward*-.21f*Mathf.Pow(legX, 6f)) + (transform.right*.3f);
		rightPedal.transform.position = rightPedalPos;
	}
	
	public void addLaunchForce(){
		Vector3 randomDirection = Random.insideUnitCircle.normalized;
		rb.AddForce( ((transform.forward*-120f) + (Vector3.up*250f) + (randomDirection*100f)) * Time.deltaTime, ForceMode.Impulse );
		transform.Rotate(randomDirection * 300f * Time.deltaTime);
	
	}
}
