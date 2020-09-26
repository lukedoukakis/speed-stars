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
	public float swingFrames;
	
	float power;
	float knee_dominance;
	float knee_dominance_powerModifier;
	float zSpeed;
	float ySpeed;
	float transitionPivotSpeed;
	float zSpeedOverTransitionPivotSpeed;
	float turnoverFactor;
	
	float torsoAngle_max;
	float torsoAngle;
	
	Vector3 forceDirHoriz;
	Vector3 forceDirVert;
	float forceHoriz;
	
    // Start is called before the first frame update
    void Start()
    {
		attributes = GetComponent<PlayerAttributes>();
		knee_dominance = attributes.KNEE_DOMINANCE;
		transitionPivotSpeed = attributes.TRANSITION_PIVOT_SPEED;
		if(knee_dominance <= 1f){
			knee_dominance_powerModifier = Mathf.Pow(2f - knee_dominance, 1.5f);
		}
		else{
			knee_dominance_powerModifier = Mathf.Pow(2f - knee_dominance, 1.5f*(Mathf.Pow(1f/knee_dominance,6f)));
		}
		swingFrames = 0f;
		zTiltMinAbs = Mathf.Abs(-45f);
		zTiltMaxAbs = Mathf.Abs(45f);
		turnoverFactor = 150f * (2f-animation.turnover);
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
			zSpeed = animation.speedHoriz;
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
				//swingTimeBonus *= swingTimeBonus*swingTimeBonus*swingTimeBonus * 10000f;
				swingTimeBonus *= swingTimeBonus*swingTimeBonus*swingTimeBonus * 1000000f * Time.deltaTime;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		GameObject g = collision.gameObject;
		// -----------------
		if(animation.mode == 2){
			if(g.tag.StartsWith("Ground")){
				touchDown = true;
				// -----------------
				float f = (torsoAngle)/(torsoAngle_max);
				f *= f*f;
				float driveBonus = f * (1f - (zSpeed/(40f))) * 12f;
				if(driveBonus > 1f){
					if(driveBonus > 12f){ driveBonus = 12f; }
				}else{ driveBonus = 1f; }
				
				// -----------------
				forceHoriz = 1f;
				forceHoriz *= power * (1f-zSpeedOverTransitionPivotSpeed) * knee_dominance_powerModifier;
				forceHoriz *= driveBonus;
				forceHoriz *= swingTimeBonus;
				// -----------------
				StartCoroutine(applyForce(forceHoriz));
				animation.updateEnergy(zSpeed, swingTimeBonus);
				// -----------------
				swingTimeBonus = 0f;
				swingFrames = 0;
			}
		}
	}
	
	void OnCollisionStay(Collision collision){
		GameObject g = collision.gameObject;
		// -----------------
		if(g.tag.StartsWith("Ground")){
			groundContact = true;
		}
		if(animation.mode == 2){
			if(g.tag.StartsWith("Ground")){
				touchDown = false;
			}
		}
	}
	
	void OnCollisionExit(Collision collision){
		GameObject g = collision.gameObject;
		// -----------------
		if(g.tag.StartsWith("Ground")){
			groundContact = false;
		}
	}
	
	public IEnumerator applyForce(float forceMagnitude){
		for(int i = 0; i < 5; i++){
			forceDirHoriz = animation.gyro.transform.forward;
			rb.AddForce(forceDirHoriz * forceMagnitude, ForceMode.Force);
			yield return null;
		}
	}
	
}