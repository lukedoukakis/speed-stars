using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerFootV2 : MonoBehaviour
{
	
	public Animator animator;
	public BoxCollider bc;
    public Rigidbody rb;
	
	PlayerAttributes attributes;
	public PlayerAnimationV2 animation;
	
	float swingTimeBonus;
	float swingTimeBonusModifier;
	float zTiltMinAbs;
	float zTiltMaxAbs;
	public float leanMagnitude;
	float energyModifier;
	
	public string side;
	public bool input;
	bool touchDown;
	public bool groundContact;
	float groundFrames;
	float groundBit;
	public float swingFrames;
	
	float power;
	float zSpeed;
	float ySpeed;
	float transitionPivotSpeed;
	float zSpeedOverTransitionPivotSpeed;
	float turnoverFactor;
	
	float strength;
	float bounce;
	float strengthBonus;
	float bounceBonus;
	
	Vector3 forceDirHoriz;
	Vector3 forceDirVert;
	float forceHoriz;
	float forceVert;
	

    // Start is called before the first frame update
    void Start()
    {
		attributes = GetComponent<PlayerAttributes>();
		swingFrames = 0f;
		groundFrames = 0f;
		
		zTiltMinAbs = Mathf.Abs(attributes.ZTILT_MIN);
		zTiltMaxAbs = Mathf.Abs(attributes.ZTILT_MAX);
		
		strength = attributes.STRENGTH_BASE;
		bounce = attributes.BOUNCE_BASE;
		turnoverFactor = 150f * (2f-attributes.TURNOVER);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if(animation.mode == 1){
			swingTimeBonus = 1f;
		}
		else if(animation.mode == 2){
			power = animation.power;
			leanMagnitude = (animation.zTilt + zTiltMinAbs) / (zTiltMaxAbs + zTiltMinAbs);
			transitionPivotSpeed = attributes.TRANSITION_PIVOT_SPEED;
			zSpeed = rb.velocity.z;
			ySpeed = rb.velocity.y;
			zSpeedOverTransitionPivotSpeed = zSpeed/transitionPivotSpeed;
			if(zSpeedOverTransitionPivotSpeed > 1f){
				zSpeedOverTransitionPivotSpeed = 1f;
			}
			if(leanMagnitude < .375f){
				leanMagnitude = .375f;
			}
			if(!touchDown){
				swingFrames++;
				swingTimeBonus = swingFrames  / turnoverFactor;
				if(swingTimeBonus > .065f){
					if(swingTimeBonus > .12f){
						swingTimeBonus = .12f;
					}
				}
				else{
					swingTimeBonus = .065f;
				}
				swingTimeBonus *= swingTimeBonus*swingTimeBonus*swingTimeBonus * 10000f;
			}
			//Debug.Log(animation.zTilt);
			//Debug.Log(leanMagnitude);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				touchDown = true;
				groundContact = true;
				// -----------------
				strengthBonus = leanMagnitude * (1f - (zSpeedOverTransitionPivotSpeed)) * 4.0f;
				bounceBonus = (1f - leanMagnitude) * (zSpeedOverTransitionPivotSpeed) * 3.0f;
				if(strengthBonus < 1f){ strengthBonus = 1f; }
				if(strengthBonus > 3f * strength){ strengthBonus = 3f * strength; }
				if(bounceBonus < .75f){ bounceBonus = .75f; }
				if(bounceBonus > 1.5f * (bounce*bounce)){ bounceBonus = 1.5f * (bounce*bounce); }
				// -----------------
				forceDirHoriz = Vector3.forward;
				forceDirVert = Vector3.up;
				forceHoriz = 1f;
				forceVert = 1f;
				forceHoriz *= power * (1f-zSpeedOverTransitionPivotSpeed);
				forceHoriz *= strengthBonus;
				forceHoriz *= bounceBonus;
				forceHoriz *= swingTimeBonus;
				// -----------------
				StartCoroutine(applyForce(forceHoriz));
				// -----------------
				swingTimeBonus = 0f;
				swingFrames = 0;
			}
		}
	}
	
	void OnCollisionStay(Collision collision){
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				touchDown = false;
				groundContact = true;
				if(groundFrames < 15f){
					groundFrames++;
				}
			}
		}
	}
	
	void OnCollisionExit(Collision collision){
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				strengthBonus = leanMagnitude * (1f - (zSpeedOverTransitionPivotSpeed)) * groundFrames;
				// -----------------
				forceHoriz = 350f * (1f-(zSpeed/(transitionPivotSpeed + 120f)));
				if(forceHoriz > 200f){ forceHoriz = 200f; }
				forceHoriz *= strengthBonus;
				StartCoroutine(applyForce(forceHoriz));
				// -----------------
				groundFrames = 0f;
				groundContact = false;
				
				
			}
		}
	}
	
	public IEnumerator applyForce(float forceMagnitude){
		for(int i = 0; i < 2; i++){
			rb.AddForce(forceDirHoriz * forceMagnitude * Time.deltaTime, ForceMode.Force);
			yield return null;
		}
	}
	
}