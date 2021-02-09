using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlockController : MonoBehaviour
{
	
	public Rigidbody rb;
	public GameObject rightPedal;
	public GameObject leftPedal;
	public ParticleSystem sparks;
	
	
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void adjustPedals(PlayerAnimationV2 anim){
		
		Transform leftFoot = anim.leftFoot;
		Transform rightFoot = anim.rightFoot;
		
		Vector3 leftPedalPos = new Vector3(leftFoot.transform.position.x, leftPedal.transform.position.y, leftFoot.transform.position.z);
		Vector3 rightPedalPos = new Vector3(rightFoot.transform.position.x, rightPedal.transform.position.y, rightFoot.transform.position.z);
		leftPedal.transform.position = leftPedalPos;
		rightPedal.transform.position = rightPedalPos;
	}
	
	public void addLaunchForce(){
		Vector3 randomDirection = Random.insideUnitCircle.normalized;
		rb.AddForce( ((transform.forward*-120f) + (Vector3.up*250f) + (randomDirection*100f)) * (1f / 90f), ForceMode.Impulse );
		transform.Rotate(randomDirection * 300f * (1f / 90f));
		sparks.Play();
	}
}
