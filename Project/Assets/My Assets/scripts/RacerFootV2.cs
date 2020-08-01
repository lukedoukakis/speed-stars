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
	float knee_dominance;
	float knee_dominance_powerModifier;
	float zSpeed;
	float ySpeed;
	float transitionPivotSpeed;
	float zSpeedOverTransitionPivotSpeed;
	float turnoverFactor;
	float torsoAngle_ideal;
	
	float torsoAngle_max;
	float torsoAngle;
	float strength;
	float bounce;
	
	Vector3 forceDirHoriz;
	Vector3 forceDirVert;
	float forceHoriz;
	float forceVert;
	

    // Start is called before the first frame update
    void Start()
    {
		attributes = GetComponent<PlayerAttributes>();
		knee_dominance = attributes.KNEE_DOMINANCE;
		knee_dominance_powerModifier = Mathf.Pow(2f-knee_dominance, 1.082f);
		swingFrames = 0f;
		groundFrames = 0f;
		
		zTiltMinAbs = Mathf.Abs(-45f);
		zTiltMaxAbs = Mathf.Abs(45f);
		
		turnoverFactor = 150f * (2f-attributes.TURNOVER);
		torsoAngle_ideal = animation.torsoAngle_ideal;
		torsoAngle_max = animation.torsoAngle_max;
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
			torsoAngle = animation.torsoAngle;
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
		if(collision.gameObject.tag == "Ground"){
			groundContact = true;
		}
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				touchDown = true;
				// -----------------
				
				float f = (torsoAngle)/(torsoAngle_max);
				f *= f*f;
				float driveBonus = f * (1f - (zSpeed/(40f))) * 8f;
				if(driveBonus > 1f){
					if(driveBonus > 8f){ driveBonus = 8f; }
				}else{ driveBonus = 1f; }
				// -----------------
				forceDirHoriz = Vector3.forward;
				forceDirVert = Vector3.up;
				forceHoriz = 1f;
				forceVert = 1f;
				forceHoriz *= power * (1f-zSpeedOverTransitionPivotSpeed) * knee_dominance_powerModifier;
				forceHoriz *= driveBonus;
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
		if(collision.gameObject.tag == "Ground"){
			groundContact = true;
		}
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				touchDown = false;
				if(groundFrames < 15f){
					groundFrames++;
				}
			}
		}
	}
	
	void OnCollisionExit(Collision collision){
		if(collision.gameObject.tag == "Ground"){
			groundContact = false;
		}
		if(animation.mode == 2){
			if(collision.gameObject.tag == "Ground"){
				groundFrames = 0f;
			}
		}
	}
	
	public IEnumerator applyForce(float forceMagnitude){
		for(int i = 0; i < 5; i++){
			rb.AddForce(forceDirHoriz * forceMagnitude * Time.deltaTime, ForceMode.Force);
			yield return null;
		}
	}
	
}