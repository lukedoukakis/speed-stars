using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationV2 : MonoBehaviour
{
	public Transform root;
	public Rigidbody rb;
	public BoxCollider chestCollider;
	public Animator animator;
	
	public Transform rightFoot;
	public Transform leftFoot;
	public Transform rightUpperArm;
	public Transform leftUpperArm;
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
	public RaceManager raceManager;
	public PlayerAttributes attributes;
	public TimerController timer;
	
	public RacerFootV2[] feet;
	public RacerFootV2 rightFootScript;
	public RacerFootV2 leftFootScript;
	public bool rightInput;
	public bool leftInput;
	
	int tick;
	public bool firstMove;
	bool firstMoveFlag;
	bool launchFlag;
	Vector3 friction;
	public float frictionMagnitude;
	public float torsoAngle_ideal;
	public float torsoAngle;
	public float torsoAngle_neutral;
	public float torsoAngle_max;
	public float power;
	float knee_dominance;
	float knee_dominance_driveModifier;
	float maxPower;
	public float maxUpSpeed;
	float topSpeed;
	Vector3 velocityLastFrame;
	
	public bool leans;
	public float zTilt;
	float leanThreshold;
	public float quickness;
	
	public bool upInSet;
	public float setPositionWeight;
	public float leanWeight;
	public float runWeight;
	public float driveWeight;

	
	
    // Start is called before the first frame update
    void Start()
    {
	
		
		feet = new RacerFootV2[] { rightFootScript, leftFootScript };
		quickness = attributes.QUICKNESS;
		maxPower = attributes.POWER;
		knee_dominance = attributes.KNEE_DOMINANCE;
		knee_dominance_driveModifier = Mathf.Pow(knee_dominance, 2.6f);
		if(knee_dominance_driveModifier < 1f){
			knee_dominance_driveModifier = 1f;
		}
		leanThreshold = .825f * attributes.KNEE_DOMINANCE;
		torsoAngle_ideal = torsoAngle_neutral - ((1f-attributes.KNEE_DOMINANCE) * 10f);
		
		if(tag.StartsWith("Ghost")){
			rightFootScript.enabled = false;
			leftFootScript.enabled = false;
		}
    }
	
	
    // Update is called once per frame
	void Update()
	{
		//Debug.Log(root.rotation.eulerAngles.x);
		
	}
	
	
	
    void FixedUpdate()
    {
	
		if (mode == 1){
			setPositionMode();
		}
		else if(mode == 2){
			//Debug.Log("RUNMODE");
			runMode();
		}
		//-----------------------------------------------------------------------------------------------------------
		adjustRotation();
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
		//-----------------------------------------------------------------------------------------------------------
		applyFriction();
		//-----------------------------------------------------------------------------------------------------------
		applyPowerModifiers();
		//-----------------------------------------------------------------------------------------------------------
		applySpeedModifiers();
		
		
		
		velocityLastFrame = rb.velocity;
	}	
		
	void runMode(){
		// set quickness to increase with speed
		torsoAngle = root.rotation.eulerAngles.x;
		quickness = attributes.QUICKNESS * (rb.velocity.z)*.25f;
		if(quickness > attributes.QUICKNESS){
			if(quickness > attributes.QUICKNESS * 1.35f){
				quickness = attributes.QUICKNESS * 1.35f;
			}
		}
		else{
			quickness = attributes.QUICKNESS;
		}
		animator.SetFloat("limbSpeed", quickness);
		//-----------------------------------------------------------------------------------------------------------
		if(setPositionWeight >= 0f){
			setPositionWeight -= transitionSpeed * attributes.QUICKNESS * Time.deltaTime;
		}else{ setPositionWeight = 0f;}
		runWeight += transitionSpeed * attributes.QUICKNESS * Time.deltaTime;
		if(runWeight > 1f){
			runWeight = 1f;
		}
   }
	
	public void setPositionMode(){
		animator.SetBool("upInSet", upInSet);
		setPositionWeight = 1f;
		runWeight = 0f;
	}
	
	
	
	
	public void readInput(int tick){
		if(tag == "Player"){
			//Debug.Log(torsoAngle_ideal);
			rightInput = Input.GetKey(KeyCode.D);
			leftInput = Input.GetKey(KeyCode.A);
		}
		else if(tag == "Ghost" || tag == "Bot"){
			if(tick < attributes.pathLength){
				rightInput = attributes.rightInputPath[tick] == 1;
				leftInput = attributes.leftInputPath[tick] == 1;
			}
		}
	}
	
	public void applyInput(int tick){
		if(mode == 1){
			if(rightInput || leftInput){
				if(attributes.isRacing){
					mode = 2;
				}
				else{
					if(upInSet){
						mode = 2;
						Debug.Log("false start");
						StartCoroutine(raceManager.falseStart());
					}
				}
			}
		}
		else if(mode == 2){
			if(rightInput || leftInput){
				animator.SetBool("input", true);
			}
			else{
				animator.SetBool("input", false);
			}
			
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
	
	void adjustRotation(){
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
						leanWeight += .58f * Time.deltaTime;
						root.Rotate(transform.TransformDirection(Vector3.right * 23f * Time.deltaTime));
						zTilt += 23f * Time.deltaTime;
					}
				}
				if(!pushing){
					leanWeight -= 1.2f * Time.deltaTime;
					root.Rotate(transform.TransformDirection(Vector3.left * 47f * Time.deltaTime));
					zTilt -= 47f * Time.deltaTime;
				}
			}
			if(leanWeight > 0f){
				if(leanWeight > 1f){
					leanWeight = 1f;
				}
			}else{
				leanWeight = 0f;	
			}
			
			if(zTilt > 45f){
				root.Rotate(transform.TransformDirection(Vector3.left * (zTilt - 45f)));
				zTilt = 45f;
			}
			else if(zTilt < -45f){
				root.Rotate(transform.TransformDirection(Vector3.left * (zTilt + 45f)));
				zTilt = -45f;
			}
		}
	}
	
	// for mapping ghost position and velocity from paths
	public void setPositionAndVelocity(int tick){
		if(tick < attributes.pathLength){
			rb.velocity = new Vector3(rb.velocity.x, attributes.velPathY[tick], attributes.velPathZ[tick]);
			Vector3 targetPos = new Vector3(transform.position.x, attributes.posPathY[tick], attributes.posPathZ[tick]);
			transform.position = targetPos;
		}
	}
	
	
	void applyFriction(){
		friction = Vector3.back * rb.velocity.z * frictionMagnitude * 100f;
		if(rightFootScript.groundContact || leftFootScript.groundContact){
			rb.AddForce(friction * Time.deltaTime);
		}
	}
	
	void applyPowerModifiers(){
		
		// reduce power if both feet on ground
		float modifiedPower = maxPower;
		if(rightFootScript.groundContact && leftFootScript.groundContact){
			modifiedPower *= .2f;
		}
		
		/*
		// reduce power based on deviance from torsoAngle_ideal
		torsoAngle = root.rotation.eulerAngles.x;
		if(torsoAngle < torsoAngle_ideal){
			float modifier = torsoAngle / torsoAngle_ideal;
			modifiedPower *= modifier;
		}
		*/
		
		float leanMag = rightFootScript.leanMagnitude;
		if(leanMag > leanThreshold){
			float modifier = leanThreshold/leanMag;
			modifier *= modifier * modifier * modifier * modifier * modifier;
			modifiedPower *= modifier;
		}
		
		// reduce power if torso angle too low
		/*
		float f = 0;
		if(root.rotation.eulerAngles.x > 350f){
			f += root.rotation.eulerAngles.x - 350f;
		}
		else if(root.rotation.eulerAngles.x < 290f){
			f += root.rotation.eulerAngles.x;
		}
		modifiedPower -= (modifiedPower*.025f) * f;
		*/
		
		power = modifiedPower;
	}
	
	void applySpeedModifiers(){
		Vector3 vel = rb.velocity;
		float vz = vel.z;
		float vy = vel.y;
		if(vz < 0f){
			vz = 0f;
		}
		if(vy > maxUpSpeed){
			vy = maxUpSpeed;
		}
		
		float maxD = .5f * knee_dominance;
		if(vz > velocityLastFrame.z + maxD){
			vz = velocityLastFrame.z + maxD;
		}
		
		vel.z = vz;
		vel.y = vy;
		rb.velocity = vel;
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
			
		driveWeight = leanWeight * (1-((rb.velocity.z)/(attributes.TRANSITION_PIVOT_SPEED*.1425f))) * knee_dominance_driveModifier;
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
		for(int i = 0; i < 8; i++){
			float launchPower = 1500f;
			rb.AddForce((Vector3.forward + (Vector3.up * .2f)) * (launchPower) * knee_dominance_driveModifier * Time.deltaTime, ForceMode.Force);
			yield return null;
		}
		
	}
	
	public IEnumerator launchAnimation(){
		pushRotation = pushLeg.rotation;
		for(int i = 0; i < 12; i++){
			yield return null;
		}
		launchFlag = false;
	}
	
	void LateUpdate(){
		if(launchFlag){
			pushLeg.rotation = pushRotation;
			pushLeg.Rotate(Vector3.up * 60f * knee_dominance_driveModifier * Time.deltaTime);
			pushRotation = pushLeg.rotation;
		}
	}
	
	
	
	
}