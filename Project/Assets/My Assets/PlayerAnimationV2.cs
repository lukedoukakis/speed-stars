using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationV2 : MonoBehaviour
{
	public Transform root;
	public Rigidbody rb;
	public BoxCollider chestCollider;
	public Animator animator;
	
	public Transform pushLeg;
	public Quaternion pushRotation;
	
	public int mode;
		/* // -----------------
			mode
			1	set
			2	run
		*/ // -----------------		
		
	public float transitionSpeed;
	
	public GlobalController globalController;
	public PlayerAttributes attributes;
	public TimerController timer;
	
	public RacerFootV2[] feet;
	public RacerFootV2 rightFootScript;
	public RacerFootV2 leftFootScript;
	public bool rightInput;
	public bool leftInput;
	
	public bool firstMove;
	bool firstMoveFlag;
	bool launchFlag;
	Vector3 friction;
	public float frictionMagnitude;
	float maxPower;
	public float maxUpSpeed;
	float topSpeed;
	
	public bool leans;
	public float zTilt;
	public float quickness;
	public Vector3 velocity;
	
	public bool upInSet;
	public float setPositionWeight;
	public float leanWeight;
	public float runWeight;
	public float driveWeight;

	
	
    // Start is called before the first frame update
    void Start()
    {
		chestCollider = animator.GetBoneTransform(HumanBodyBones.UpperChest).gameObject.GetComponent<BoxCollider>();
		pushLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
		
		feet = new RacerFootV2[] { rightFootScript, leftFootScript };
		quickness = attributes.QUICKNESS_BASE;
		maxPower = attributes.POWER_BASE;
		topSpeed = 25 * attributes.BOUNCE_BASE;
		
		if(tag.StartsWith("Ghost")){
			rightFootScript.enabled = false;
			leftFootScript.enabled = false;
		}
    }
	
	
    // Update is called once per frame
    void FixedUpdate()
    {
		
		if(tag == "Player" || tag == "Bot"){
			velocity = rb.velocity;
			//Debug.Log(velocity.z);
		}
		else if(tag == "Ghost"){
			if(timer.ticks < attributes.pathLength){
				velocity = new Vector3(rb.velocity.x, attributes.velPathY[timer.ticks], attributes.velPathZ[timer.ticks]);
				transform.position = new Vector3(transform.position.x, attributes.posPathY[timer.ticks], attributes.posPathZ[timer.ticks]);
			}
		}
		//-----------------------------------------------------------------------------------------------------------
		if (mode == 1){ setPositionMode(); }
		else if	(mode == 2){ runMode(); }
		//-----------------------------------------------------------------------------------------------------------
		readInput();
		//-----------------------------------------------------------------------------------------------------------
		applyInput();
		//-----------------------------------------------------------------------------------------------------------
		updateRotation();
		//-----------------------------------------------------------------------------------------------------------
		updateLayerWeights();
		
		// first move -----------------------------------------------------------------------------------------------------------
		if(firstMove){
			if(!firstMoveFlag){
				launchFlag = true;
				StartCoroutine(launch());
				firstMoveFlag = true;
			}
		}
		// friction -----------------------------------------------------------------------------------------------------------
		friction = Vector3.back * velocity.z * frictionMagnitude * 100f;
		if(rightFootScript.groundContact || leftFootScript.groundContact){
			rb.AddForce(friction * Time.deltaTime);
		}
		
		if(rightFootScript.input && leftFootScript.input){
			attributes.POWER_BASE *= .2f;
		}
		else{
			attributes.POWER_BASE = maxPower;
		}
		// speed limit -----------------------------------------------------------------------------------------------------------
		float v = velocity.z;
		if(v > 0f){
			if(v > 25f){
				//v = 25f;
			}
		}
		else{
			v = 0f;
		}
		velocity.z = v;
		
		if(velocity.y > maxUpSpeed){
			velocity.y = maxUpSpeed;
		}
		rb.velocity = velocity;
		
		

	}	
		
	void runMode(){
		// set quickness to increase with speed
		quickness = attributes.QUICKNESS_BASE * (velocity.z)*.25f;
		if(quickness > attributes.QUICKNESS_BASE){
			if(quickness > attributes.QUICKNESS_BASE * 1.35f){
				quickness = attributes.QUICKNESS_BASE * 1.35f;
			}
		}
		else{
			quickness = attributes.QUICKNESS_BASE;
		}
		animator.SetFloat("limbSpeed", quickness);
		//-----------------------------------------------------------------------------------------------------------
		if(setPositionWeight >= 0f){
			setPositionWeight -= transitionSpeed * attributes.QUICKNESS_BASE * Time.deltaTime;
		}else{ setPositionWeight = 0f;}
		runWeight += transitionSpeed * attributes.QUICKNESS_BASE * Time.deltaTime;
		if(runWeight > 1f){
			runWeight = 1f;
		}
   }
	
	void setPositionMode(){
		animator.SetBool("upInSet", upInSet);
		setPositionWeight = 1f;
		runWeight = 0f;
	}
	
	
	
	
	void readInput(){
		if(tag == "Player"){
			rightInput = Input.GetKey(KeyCode.D);
			leftInput = Input.GetKey(KeyCode.A);
		}
		if(tag == "Ghost" || tag == "Bot"){
			//Debug.Log(zTilt);
			if(timer.ticks < attributes.pathLength){
				rightInput = attributes.rightInputPath[timer.ticks] == 1;
				leftInput = attributes.leftInputPath[timer.ticks] == 1;
			}
		}
	}
	
	void applyInput(){
		if(mode == 1){
			if(rightInput || leftInput){
				if(attributes.isRacing){
					mode = 2;
				}
				else{
					// false start
				}
			}
		}
		else if(mode == 2){
			if(rightInput){
				if(tag == "Player" || tag == "Bot"){
					rightFootScript.input = true;
					firstMove = true;
				}
				animator.SetBool("right", true);
			}else{
				rightFootScript.input = false;
				animator.SetBool("right", false);
			}
			if(leftInput){
				if(tag == "Player" || tag == "Bot"){
					leftFootScript.input = true;
					firstMove = true;
				}
				animator.SetBool("left", true);	
			}else{
				leftFootScript.input = false;
				animator.SetBool("left", false);
			}
		}
	}
	
	void updateRotation(){
		if(leans){
			if(mode == 1){
				leanWeight += .5f * Time.deltaTime;
			}
			else if(mode == 2){
				// torso lean
				bool pushing = false;
				bool[] inputs = new bool[2] {rightInput, leftInput};
				foreach(bool input in inputs){
					if(input){
						pushing = true;
						leanWeight += .58f * attributes.TILT_SPEED * Time.deltaTime;
						root.Rotate(transform.TransformDirection(Vector3.right * 23f * attributes.TILT_SPEED * Time.deltaTime));
						zTilt += 23f * attributes.TILT_SPEED * Time.deltaTime;
					}
				}
				if(!pushing){
					leanWeight -= 1.2f * attributes.TILT_SPEED * Time.deltaTime;
					root.Rotate(transform.TransformDirection(Vector3.left * 47f * attributes.TILT_SPEED * Time.deltaTime));
					zTilt -= 47f * attributes.TILT_SPEED * Time.deltaTime;
				}
			}
			if(leanWeight > 0f){
				if(leanWeight > 1f){
					leanWeight = 1f;
				}
			}else{
				leanWeight = 0f;	
			}
			
			if(zTilt > attributes.ZTILT_MAX){
				root.Rotate(transform.TransformDirection(Vector3.left * (zTilt - attributes.ZTILT_MAX)));
				zTilt = attributes.ZTILT_MAX;
			}
			else if(zTilt < attributes.ZTILT_MIN){
				root.Rotate(transform.TransformDirection(Vector3.left * (zTilt - attributes.ZTILT_MIN)));
				zTilt = attributes.ZTILT_MIN;
			}

			
			
		}
		//Debug.Log("rotation x: " + transform.rotation.eulerAngles.x);
	}
	
	
	// set layer weight values to corresponding variables
	void updateLayerWeights(){
			/* // -----------------
				layers
				1	idle
				2	set
				3	torso (upright)
				4	torso (drive)
				5	right leg (upright)
				6	left leg (upright)
				7	right leg (drive)
				8	left leg (drive)
				9	right arm (upright)
				10	left arm (upright)
				11	right arm (drive)
				12	left arm (drive)
				13	triple extension
			*/ // -----------------
			
		driveWeight = leanWeight * (1-(velocity.z/(attributes.TRANSITION_PIVOT_SPEED*.75f))) * (attributes.STRENGTH_BASE * 1.4f);
		if(driveWeight > 1f){
			driveWeight = 1f;
		}
		
	
		animator.SetLayerWeight(1,0f);
		animator.SetLayerWeight(2,setPositionWeight);
		animator.SetLayerWeight(3,(1f-leanWeight)*runWeight);
		animator.SetLayerWeight(4,leanWeight*runWeight);
		animator.SetLayerWeight(5,(1-driveWeight)*runWeight);
		animator.SetLayerWeight(6,(1-driveWeight)*runWeight);
		animator.SetLayerWeight(7,driveWeight*runWeight);
		animator.SetLayerWeight(8,driveWeight*runWeight);
		animator.SetLayerWeight(9,(1-driveWeight)*runWeight);
		animator.SetLayerWeight(10,(1-driveWeight)*runWeight);
		animator.SetLayerWeight(11,driveWeight*runWeight);
		animator.SetLayerWeight(12,driveWeight*runWeight);
	}

	
	public IEnumerator launch(){
		StartCoroutine(launchAnimation());
		for(int i = 0; i < 2; i++){
			rb.AddForce((Vector3.forward + (Vector3.up * .2f)) * (8000f) * attributes.STRENGTH_BASE * Time.deltaTime, ForceMode.Force);
			yield return null;
		}
		
	}
	
	public IEnumerator launchAnimation(){
		pushRotation = pushLeg.rotation;
		for(int i = 0; i < 15; i++){
			yield return null;
		}
		launchFlag = false;
	}
	
	void LateUpdate(){
		if(launchFlag){
			pushLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			pushLeg.rotation = pushRotation;
			pushLeg.Rotate(Vector3.up * 3.25f * attributes.STRENGTH_BASE);
			pushRotation = pushLeg.rotation;
		}
	}
	
	
	
	
}