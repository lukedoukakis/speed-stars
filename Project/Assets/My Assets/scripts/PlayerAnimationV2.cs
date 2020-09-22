using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationV2 : MonoBehaviour
{
	
	public int preset;
	
	public GameObject gyro;
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
	
	Vector3 rightFootPos_lastFrame;
	Vector3 leftFootPos_lastFrame;
	float rightFootVel_lastFrame;
	float leftFootVel_lastFrame;
	
	
	public static int Set = 1;
	public static int Run = 2;
	public static int Idle = 3;
	public int mode;
	
	public float transitionSpeed;
	
	public GlobalController globalController;
	public RaceManager raceManager;
	public PlayerAttributes attributes;
	public OrientationController oc;
	public TimerController timer;
	public EnergyMeterController emc;
	
	public RacerFootV2[] feet;
	public RacerFootV2 rightFootScript;
	public RacerFootV2 leftFootScript;
	public bool rightInput;
	int framesSinceRight;
	public bool leftInput;
	int framesSinceLeft;
	
	int tick;
	public bool onCurve;
	public bool launchFlag;
	Vector3 friction;
	public float frictionMagnitude;
	public float torsoAngle_ideal;
	public float torsoAngle;
	public float torsoAngle_neutral;
	public float torsoAngle_max;
	public float power;
	public float powerMod;
	float knee_dominance;
	float driveModifier;
	public float launchModifier;
	float curveModifier;
	public float maxPower;
	public float maxUpSpeed;
	float topSpeed;
	Vector3 velocityLastFrame;
	Vector3 velocityLastFrame_relative;
	public float speedHoriz;
	public float speedHoriz_lastFrame;
	public float energy;
	public float fitness;
	
	public bool leans;
	public float zTilt;
	public bool leanLock;
	public int leanLockTick;
	float leanThreshold;
	public float quickness;
	public float quicknessMod;
	public float turnover;
	public float armFlex;
	public float armExtend;
	
	public bool upInSet;
	public float setPositionWeight;
	public float leanWeight;
	public float runWeight;
	public float driveWeight;
	public float cruiseWeight;
	
    // Start is called before the first frame update
    void Start()
    {
	
		
		feet = new RacerFootV2[] { rightFootScript, leftFootScript };

    }
	
	
    // Update is called once per frame
	void Update()
	{
		//Debug.Log(root.rotation.eulerAngles.x);
		
	}
	
	public void init(){
		
		setPreset(attributes.racerName);
		// -----------------
		if(preset < 4){
			turnover = attributes.TURNOVER;
			quickness = attributes.QUICKNESS;
			fitness = attributes.FITNESS;
			maxPower = attributes.POWER;
			knee_dominance = attributes.KNEE_DOMINANCE;
			driveModifier = Mathf.Pow(knee_dominance, 3f);
			launchModifier = Mathf.Pow(knee_dominance, 1.5f);
			curveModifier = Mathf.Pow(knee_dominance, .6f);
			leanThreshold = .825f * attributes.KNEE_DOMINANCE;
			torsoAngle_ideal = torsoAngle_neutral - ((1f-attributes.KNEE_DOMINANCE) * 10f);
			armFlex = attributes.armSpeedFlex;
			armExtend = attributes.armSpeedExtend;
			//animator.SetFloat("armFlex", 1f);
			//animator.SetFloat("armExtend", 1f);
		}
		else if(preset >= 4){
			turnover = attributes.TURNOVER;
			quickness = attributes.QUICKNESS;
			fitness = attributes.FITNESS;
			maxPower = attributes.POWER;
			knee_dominance = attributes.KNEE_DOMINANCE;
			driveModifier = float.Parse(TextReader.getAttribute(preset, "driveModifier"));
			launchModifier = float.Parse(TextReader.getAttribute(preset, "launchModifier"));
			curveModifier = float.Parse(TextReader.getAttribute(preset, "curveModifier"));
			leanThreshold = .825f * attributes.KNEE_DOMINANCE;
			torsoAngle_ideal = torsoAngle_neutral - ((1f-attributes.KNEE_DOMINANCE) * 10f);
		}
		// -----------------
		
		quicknessMod = 1f;
		powerMod = 1f;
		leanLock = false;
		
		if(tag.StartsWith("Ghost")){
			//rightFootScript.enabled = false;
			//leftFootScript.enabled = false;
		}
	}
	
	
	
    void FixedUpdate()
    {
		speedHoriz = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;

		if(mode == Set){
			setPositionMode();
		}
		else if(mode == Run){
			//Debug.Log("RUNMODE");
			runMode();
		}
		else if(mode == Idle){
			idleMode();
		}
		
		//-----------------------------------------------------------------------------------------------------------
		adjustRotation();
		//-----------------------------------------------------------------------------------------------------------
		updateLayerWeights();
		//-----------------------------------------------------------------------------------------------------------
		applyFriction();
		//-----------------------------------------------------------------------------------------------------------
		applyPowerModifiers();
		//-----------------------------------------------------------------------------------------------------------
		applySpeedModifiers();
		//-----------------------------------------------------------------------------------------------------------
		
		animator.SetBool("groundContact", rightFootScript.groundContact || leftFootScript.groundContact);
		
		velocityLastFrame = rb.velocity;
		velocityLastFrame_relative = gyro.transform.InverseTransformDirection(rb.velocity);
		speedHoriz_lastFrame = speedHoriz;
	}
	
	void setPreset(string name){
		if(name == "Usain Bolt"){
			preset = PlayerAttributes.ATTRIBUTES_LEGEND_USAINBOLT;
		}
		else if(name == "Michael Johnson"){
			preset = PlayerAttributes.ATTRIBUTES_LEGEND_MICHAELJOHNSON;
		}
		else if(name == "Yohan Blake"){
			preset = PlayerAttributes.ATTRIBUTES_LEGEND_YOHANBLAKE;
		}
		else if(name == "Jesse Owens"){
			preset = PlayerAttributes.ATTRIBUTES_LEGEND_JESSEOWENS;
		}
		else if(name == "Wayde van Niekerk"){
			preset = PlayerAttributes.ATTRIBUTES_LEGEND_WAYDEVANNIEKERK;
		}
		else{
			preset = PlayerAttributes.ATTRIBUTES_RANDOM;
		}
	}
		
	void runMode(){
		
		if(raceManager.raceEvent >= 2){
			Debug.Log("raceEvent: " + raceManager.raceEvent);
			oc.updateOrientation(true);
			if(oc.trackSegment == 1 || oc.trackSegment == 3){
				if(!onCurve){
					animator.SetBool("onCurve", true);
				}
				onCurve = true;
			}
			else{
				if(onCurve){
					animator.SetBool("onCurve", false);
				}
				onCurve = false;
			}
		}
		
		// set quickness to increase with speed
		torsoAngle = root.rotation.eulerAngles.x;
		quickness = attributes.QUICKNESS * speedHoriz *.25f;
		if(quickness > attributes.QUICKNESS){
			if(quickness > attributes.QUICKNESS * 1.35f){
				quickness = attributes.QUICKNESS * 1.35f;
			}
		}
		else{
			quickness = attributes.QUICKNESS;
		}
		if(energy < 70f){
			quickness *= Mathf.Pow(energy/70f, .075f);
		}
		if(quickness < .95f){
			quickness = .95f;
		}
		float q = quickness*quicknessMod;
		animator.SetFloat("limbSpeed", q);
		animator.SetFloat("armFlex", q*armFlex);
		animator.SetFloat("armExtend", q*armExtend);
		//-----------------------------------------------------------------------------------------------------------
		if(setPositionWeight >= 0f){
			setPositionWeight -= transitionSpeed * attributes.QUICKNESS * Time.deltaTime;
		}else{ setPositionWeight = 0f;}
		runWeight += transitionSpeed * attributes.QUICKNESS * Time.deltaTime;
		if(runWeight > 1f){
			runWeight = 1f;
		}
		
		if(!rightInput){
			framesSinceRight++;
		}
		if(!leftInput){
			framesSinceLeft++;
		}
   }
	
	public void setPositionMode(){
		animator.SetBool("upInSet", upInSet);
		setPositionWeight = 1f;
		runWeight = 0f;
	}
	
	public void idleMode(){
		animator.SetBool("idle", true);
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
		if(mode == Set){
			if(rightInput || leftInput){
				if(attributes.isRacing){
					mode = 2;
				}
				else{
					if(raceManager.raceStatus == RaceManager.STATUS_SET){
						mode = 2;
						//Debug.Log("false start");
						StartCoroutine(raceManager.falseStart());
					}
				}
			}
		}
		else if(mode == Run){
			if(rightInput || leftInput){
				animator.SetBool("input", true);
			}
			else{
				animator.SetBool("input", false);
			}
			
			if(rightInput){
				framesSinceRight = 0;
				rightFootScript.input = true;
				animator.SetBool("right", true);
				
				if(!launchFlag){
					StartCoroutine(launch());
					launchFlag = true;
				}
				
			}else{
				rightFootScript.input = false;
				animator.SetBool("right", false);
			}
			if(leftInput){
				framesSinceLeft = 0;
				leftFootScript.input = true;
				animator.SetBool("left", true);
				
				if(!launchFlag){
					StartCoroutine(launch());
					launchFlag = true;
				}
			}else{
				leftFootScript.input = false;
				animator.SetBool("left", false);
			}
		}
	}
	
	void adjustRotation(){
		if(leans){
			if(mode == Set){
				leanWeight += .5f * Time.deltaTime;
			}
			else if(mode == Run){
				if(!leanLock){
				// torso lean
					bool pushing = false;
					bool[] inputs = new bool[2] {rightInput, leftInput};
					foreach(bool input in inputs){
						if(input){
							pushing = true;
							leanWeight += .58f * Time.deltaTime;
							root.Rotate(Vector3.right * 23f * Time.deltaTime, Space.Self);
							zTilt += 23f * Time.deltaTime;
						}
					}
					if(!pushing){
						leanWeight -= 1.2f * Time.deltaTime;
						root.Rotate(Vector3.left * 47f * Time.deltaTime, Space.Self);
						zTilt -= 47f * Time.deltaTime;
					}
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
				root.Rotate(Vector3.left * (zTilt - 45f), Space.Self);
				zTilt = 45f;
			}
			else if(zTilt < -45f){
				root.Rotate(Vector3.left * (zTilt + 45f), Space.Self);
				zTilt = -45f;
			}
			
			
			
			
			
			
		}
	}
	
	
	public void turnTowardsY(float y){
		
		//Debug.Log("turning towards y: " + y);
		Vector3 eulers = transform.rotation.eulerAngles;
		eulers.y = y;
		transform.rotation = Quaternion.Euler(eulers);
		
		eulers = gyro.transform.rotation.eulerAngles;
		eulers.y = y;
		gyro.transform.rotation = Quaternion.Euler(eulers);
	}
	
	
	// for mapping ghost position and velocity from paths
	public void setPositionAndVelocity(int tick){
		Debug.Log("leanLockTick: " + attributes.leanLockTick);
		if(tick > attributes.leanLockTick && attributes.leanLockTick > 0){
			Debug.Log("leanLock TRUE");
			leanLock = true;
		} else{
			leanLock = false;
			Debug.Log("leanLock FALSE");
		}
		
		if(tick < attributes.pathLength){
			float vM = attributes.velMagPath[tick];
			//float vX = attributes.velPathX[tick];
			float vY = attributes.velPathY[tick];
			//float vZ = attributes.velPathZ[tick];
			//float pX = attributes.posPathX[tick];
			float pY = attributes.posPathY[tick];
			float pZ = attributes.posPathZ[tick];
			//float s1P = attributes.sphere1Prog[tick];
			//float s2P = attributes.sphere2Prog[tick];
			// -----------------
			// set horizontal velocity magnitude to magnitude from paths
			Vector3 vel = rb.velocity;
			vel.y = 0f;
			vel *= 100f;
			vel = Vector3.ClampMagnitude(vel, vM);
			vel.y = vY;
			rb.velocity = vel;
			// -----------------
			// set y-position from path
			Vector3 pos = transform.position;
			pos.y = pY;
			transform.position = pos;
			
			
			if(oc.trackSegment == 4){
				Vector3 targetPos = new Vector3(transform.position.x, pY, pZ);
				if(Vector3.Distance(transform.position, globalController.raceManager.finishLine.transform.position) > 20f){
					transform.position = Vector3.Lerp(transform.position, targetPos, 1f * Time.deltaTime);
				}
				else{
					transform.position = targetPos;
				}
			}
			
		}

	}
	
	public void updateEnergy(float speed, float swingTimeBonus){
		float energyCost;
		energyCost = 50f * Time.deltaTime;
		energyCost *= Mathf.Pow(speed / 27f, 2.5f);
		energyCost *= 1f + (1f - (swingTimeBonus / 2.0736f));
		energyCost *= (2f - fitness);
		energyCost *= (1f - cruiseWeight/5f);
		if(energyCost < .1f){
			energyCost = .1f;
		}
		
		energy -= energyCost;
		if(energy < 0f){
			energy = 0f;
		}
		
		emc.adjustForEnergyLevel(energy);

	}
	
	
	void applyFriction(){
		friction = gyro.transform.forward*-1f * speedHoriz * frictionMagnitude * 100f;
		if(rightFootScript.groundContact || leftFootScript.groundContact){
			rb.AddForce(friction * .011f, ForceMode.Force);
		}
	}
	
	void applyPowerModifiers(){
		float modifier = 1f;
		float modifiedPower = maxPower;
		// -----------------
		// reduce power if both feet on ground
		if(rightFootScript.groundContact && leftFootScript.groundContact){
			modifiedPower *= .2f;
		}
		// -----------------
		// modify power from lean magnitude
		float leanMag = rightFootScript.leanMagnitude;
		if(leanMag > leanThreshold){
			modifier = leanThreshold/leanMag;
			modifier *= modifier * modifier * modifier * modifier * modifier;
			modifiedPower *= modifier;
		}
		// -----------------
		// modify power from torso angle
		if(torsoAngle < 313f){
			modifier = torsoAngle/310f;
			modifiedPower *= modifier;
		}
		
		// -----------------
		// reduce power if on curve
		if(onCurve){
			modifier = .9f * curveModifier;
			modifiedPower *= modifier;
		}
		// -----------------
		// modify power from energy
		if(energy < 80f){
			if(energy >= 30f){
				modifier *= (energy / 80f);
			}
			else{
				modifier *= (30f / 80f);
			}
			modifier = Mathf.Pow(modifier, .75f);
			modifiedPower *= modifier;
		}
		// -----------------
		power = modifiedPower;
		power *= powerMod;
	}
	
	void applySpeedModifiers(){
		
		
		
		Vector3 velHoriz = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		float velY = rb.velocity.y;
		
		if(gyro.transform.forward.z > 0){
			if(velHoriz.z < 0f){
				velHoriz.z = 0f;
			}
		}
		else if(gyro.transform.forward.z < 0){
			if(velHoriz.z > 0f){
				velHoriz.z = 0f;
			}
		}
		
		float maxD = .475f * driveModifier;
		if(speedHoriz > speedHoriz_lastFrame + maxD){
			velHoriz = Vector3.ClampMagnitude(velHoriz, speedHoriz_lastFrame + maxD);
		}
		
		if(velY > maxUpSpeed){
			velY = maxUpSpeed;
		}
		
		
		
		
		
		Vector3 newVelocity = new Vector3(velHoriz.x, velY, velHoriz.z);
		rb.velocity = newVelocity;
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
			*/ // -----------------
		
		driveWeight = leanWeight * (1f-(speedHoriz/(attributes.TRANSITION_PIVOT_SPEED*.1425f)));
		if(driveWeight > .8f){
			driveWeight = .8f;
		}
		
		
		if(speedHoriz > speedHoriz_lastFrame){
			cruiseWeight -= .7f * Time.deltaTime;
			if(cruiseWeight < 0f){
				cruiseWeight = 0f;
			}
		}else{
			if(speedHoriz > 20f){
				//cruiseWeight += .005f*(2f-driveModifier);
				cruiseWeight += .7f * Time.deltaTime;
				if(cruiseWeight > 1f){
					cruiseWeight = 1f;
				}
			}
		}
	
		animator.SetLayerWeight(1,cruiseWeight*runWeight);
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
		
		animator.SetFloat("horizSpeed", speedHoriz/28f);
	}
	
	public void setViewMode(int viewMode){
		animator.SetInteger("viewMode", viewMode);
	}

	
	public IEnumerator launch(){
		raceManager.startingBlocks_current[attributes.lane-1].GetComponent<StartingBlockController>().addLaunchForce();
		
		animator.SetBool("launch", true);
		cruiseWeight = 0f;
		for(int i = 0; i < 12; i++){
			float launchPower = 1666f;
			//if(raceManager.raceEvent == RaceManager.RACE_EVENT_60M){ launchPower *= 1.1f; }
			rb.AddForce((gyro.transform.forward + (Vector3.up * launchModifier * .2f)) * (launchPower) * launchModifier * .0165f, ForceMode.Force);
			yield return null;
		}
		for(int i = 0; i < 10; i++){
			yield return null;
		}
		animator.SetBool("launch", false);
	}
	
	
	public void setIdle(){
		mode = 3;
	}
	
	
	
	
}