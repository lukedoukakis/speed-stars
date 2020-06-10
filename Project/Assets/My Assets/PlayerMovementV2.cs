using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
	/*
	public Rigidbody rb;
	
	public PlayerAttributes attributes;
	public PlayerAnimationV2 animation;
	
	float maxPower;
	public float maxUpSpeed;
	
	public RacerFootV2 rightFoot;
	public RacerFootV2 leftFoot;
	bool firstMoveFlag;
	public Vector3 velocity;
	Vector3 friction;
	public float frictionMagnitude;
	
	
    // Start is called before the first frame update
    void Start()
    {
		maxPower = attributes.POWER_BASE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// first move -----------------------------------------------------------------------------------------------------------
		if(animation.firstMove){
			if(!firstMoveFlag){
				rb.AddForce(transform.TransformDirection(Vector3.forward) * attributes.POWER_BASE * attributes.STRENGTH_BASE * Time.deltaTime, ForceMode.Force);
				firstMoveFlag = true;
			}
		}
		// friction -----------------------------------------------------------------------------------------------------------
		friction = transform.TransformDirection(Vector3.back) * velocity.z * (frictionMagnitude);
		if(rightFoot.groundContact){
			applyFriction();
		}
		if(leftFoot.groundContact){
			applyFriction();
		}
		
		if(rightFoot.input && leftFoot.input){
			attributes.POWER_BASE *= .2f;
		}
		else{
			attributes.POWER_BASE = maxPower;
		}
		// speed limit -----------------------------------------------------------------------------------------------------------
		velocity = rb.velocity;
		if(velocity.z < 0){
			velocity.z = 0;
		}
		if(velocity.y > maxUpSpeed){
			velocity.y = maxUpSpeed;
		}
		rb.velocity = velocity;
		
		//Debug.Log(velocity.z);
		
		
		
		
	}
	
	void applyFriction(){
		rb.AddForce(friction * Time.deltaTime, ForceMode.Impulse);
	}
	*/
}
