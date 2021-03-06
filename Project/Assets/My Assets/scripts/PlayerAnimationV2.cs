﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationV2 : MonoBehaviour
{
	
	public GameObject gyro;
	public Transform hudIndicatorT;
	public MeshRenderer hudIndicatorRenderer;
	public GameObject marker;
	public Transform root;
	public Rigidbody rb;
	public BoxCollider chestCollider;
	public Animator animator;
	
	public Transform rootTransform;
	public Transform headT;
	public Transform rightFoot;
	public Transform leftFoot;
	public Transform rightUpperArm;
	public Transform leftUpperArm;
	public Transform pushLeg;
	public Quaternion pushRotation;
	
	public ParticleSystem sweatParticles;
	public ParticleSystem popParticles;
	public ParticleSystem dustParticles;
	bool showDust;
	
	Vector3 rightFootPos_lastFrame;
	Vector3 leftFootPos_lastFrame;
	float rightFootVel_lastFrame;
	float leftFootVel_lastFrame;
	
	public static int Set = 1;
	public static int Run = 2;
	public int mode;
	
	int tick;
	
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
	public bool leftInput;
	
	public bool isPlayer;
	public bool finished;
	public bool onCurve;
	public bool launchFlag;
	Vector3 friction;
	bool canFalseStart;
	public float frictionMagnitude;
	public float torsoAngle;
	public float torsoAngle_upright;
	public float torsoAngle_max;
	public float power;
	public float powerMod;
	float knee_dominance;
	public float driveModifier;
	public float launch_power;
	float curve_power;
	float cruise;
	public float maxPower;
	public float maxUpSpeed;
	float topSpeed;
	Vector3 velocityLastFrame;
	Vector3 velocityLastFrame_relative;
	public float speedHoriz;
	public float speedHoriz_lastFrame;
	public float energy;
	public float fitness;
	float energyCost_base;
	float energyBurnoutMod;
	float energyBurnoutThreshold;
	
	public int pathLength;
	public float[] velMagPath;
	public float[] velPathY;
	public float[] posPathY;
	public float[] posPathZ;
	
	public bool leans;
	public float zTilt;
	public bool leanLock;
	public int leanLockTick;
	float leanThreshold;
	public float quickness;
	public float quickness_base;
	public float quicknessMod;
	public float turnover;
	public float armFlexL;
	public float armExtendL;
	public float armFlexR;
	public float armExtendR;
	
	public int leadLeg;
	public bool upInSet;
	public float setPositionWeight;
	public float leanWeight;
	public float runWeight;
	public float driveWeight;
	public float cruiseWeight;
	
	float dTime;

	
    // Start is called before the first frame update
    void Start()
    {

		feet = new RacerFootV2[] { rightFootScript, leftFootScript };

    }
	
	public void init(int raceEvent){
		
		this.raceManager = globalController.raceManager;
		
		// ghost attributes
		pathLength = attributes.pathLength;
		velMagPath = attributes.velMagPath;
		velPathY =  attributes.velPathY;
		posPathY =  attributes.posPathY;
		posPathZ =  attributes.posPathZ;
		
		// base stats
		maxPower = attributes.POWER;
		turnover = attributes.TURNOVER;
		quickness_base = attributes.QUICKNESS;
		fitness = attributes.FITNESS;
		knee_dominance = attributes.KNEE_DOMINANCE;
		launch_power = attributes.LAUNCH_POWER;
		curve_power = attributes.CURVE_POWER;
		cruise = attributes.CRUISE;
		
		// animation stats
		leadLeg = attributes.leadLeg;
		animator.SetInteger("leadLeg", leadLeg);
		armFlexL = attributes.armSpeedFlexL;
		armExtendL = attributes.armSpeedExtendL;
		armFlexR = attributes.armSpeedFlexL;
		armExtendR = attributes.armSpeedExtendL;
		
		// special stats
		driveModifier = 1f * Mathf.Pow(knee_dominance, 1.2f) * Mathf.Pow((2f-(cruise+.5f)), .25f);
		leanThreshold = .825f * knee_dominance;
		torsoAngle_max = 355f;
		torsoAngle_upright = 320f * Mathf.Pow(knee_dominance,.01f);
		
		// energy
		energyCost_base = 48f;
		energyBurnoutMod = 1f;
		energyBurnoutThreshold = 20f;
		
		// particles
		showDust = false;
	
		if(gameObject.tag.StartsWith("Player")){
			isPlayer = true;
			hudIndicatorRenderer.material = globalController.hudIndicatorMat_player;
			if(raceManager.viewMode == RaceManager.VIEW_MODE_LIVE){
				canFalseStart = true;
			}
		}
		else{
			isPlayer = false;
			hudIndicatorRenderer.material = globalController.hudIndicatorMat_nonPlayer;
			canFalseStart = false;
		}
		
		leftFootScript.raceManager = raceManager;
		rightFootScript.raceManager = raceManager;
		
		// -----------------
		
		quicknessMod = 1f;
		powerMod = 1f;
		leanLock = false;
		
		/*
		if(tag.StartsWith("Ghost")){
			rightFootScript.enabled = false;
			leftFootScript.enabled = false;
		}
		*/
	}
	
	
	
    public void FixedUpdate()
    {
		animator.SetBool("groundContact", rightFootScript.groundContact || leftFootScript.groundContact);
		
		dTime = (1f / 90f);

		speedHoriz = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;

		if(mode == Set){
			setPositionMode();
		}
		else if(mode == Run){
			runMode();
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
		velocityLastFrame = rb.velocity;
		velocityLastFrame_relative = gyro.transform.InverseTransformDirection(rb.velocity);
		speedHoriz_lastFrame = speedHoriz;
		rightFootPos_lastFrame = rightFoot.position;
		leftFootPos_lastFrame = leftFoot.position;
	}
		
	void runMode(){
		
		
		if(raceManager.raceEvent >= 2){
			//Debug.Log("raceEvent: " + raceManager.raceEvent);
			if(!finished){ oc.updateOrientation(true); }
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
		quickness = quickness_base * speedHoriz *.25f;
		if(quickness > quickness_base){
			if(quickness > quickness_base * 1.35f){
				quickness = quickness_base * 1.35f;
			}
		}
		else{
			quickness = quickness_base;
		}
		if(energy < 70f){
			quickness *= Mathf.Pow(energy/70f, .075f);
		}
		
		if(quickness < .95f){
			quickness = .95f;
		}
		float animSpeedMod = quickness*quicknessMod;
		animator.SetFloat("limbSpeed", animSpeedMod);
		animator.SetFloat("armFlexL", armFlexL*animSpeedMod);
		animator.SetFloat("armExtendL", armExtendL*animSpeedMod);
		animator.SetFloat("armFlexR", armFlexR*animSpeedMod);
		animator.SetFloat("armExtendR", armExtendR*animSpeedMod);
		
		//-----------------------------------------------------------------------------------------------------------
		// particles
		
		if(speedHoriz >= 5f){
			if(!showDust){
				showDust = true;
				dustParticles.Play();
			}
		}
		else{
			if(showDust){
				showDust = false;
				dustParticles.Stop();
			}
		}
		//-----------------------------------------------------------------------------------------------------------
		if(setPositionWeight >= 0f){
			setPositionWeight -= transitionSpeed * quickness_base * dTime;
		}else{ setPositionWeight = 0f;}
		runWeight += transitionSpeed * quickness_base * dTime;
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
			rightInput = Input.GetKey(SettingsManager.controlsRight);
			leftInput = Input.GetKey(SettingsManager.controlsLeft);
		}
		else if(tag == "Ghost" || tag == "Bot"){
			if(tick < pathLength){
				rightInput = attributes.rightInputPath[tick] == 1;
				leftInput = attributes.leftInputPath[tick] == 1;
			}
		}
	}
	
	public void applyInput(int tick){
		if(mode == Set){
			if(rightInput || leftInput){
				mode = Run;
				if(canFalseStart){
					if(tick <= 1){
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
				rightFootScript.input = true;
				animator.SetBool("right", true);
				
				if(!launchFlag){
					StartCoroutine(launch(leadLeg == 1));
					launchFlag = true;
				}
				
			}else{
				rightFootScript.input = false;
				animator.SetBool("right", false);
			}
			if(leftInput){
				leftFootScript.input = true;
				animator.SetBool("left", true);
				
				if(!launchFlag){
					StartCoroutine(launch(leadLeg == 0));
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
				leanWeight += .5f * dTime;
			}
			else if(mode == Run){
				if(!leanLock){
				// torso lean
					bool pushing = false;
					bool[] inputs = new bool[2] {rightInput, leftInput};
					foreach(bool input in inputs){
						if(input){
							pushing = true;
							leanWeight += .58f * (1f / 60f);
							root.Rotate(Vector3.right * 23f * (1f / 60f), Space.Self);
							zTilt += 23f * (1f / 60f);
						}
					}
					if(!pushing){
						leanWeight -= 1.2f * (1f / 60f);
						root.Rotate(Vector3.left * 47f * (1f / 60f), Space.Self);
						zTilt -= 47f * (1f / 60f);
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
		if(tick > attributes.leanLockTick && attributes.leanLockTick > 0){
			leanLock = true;
		} else{
			leanLock = false;
		}
		
		if(tick < pathLength){
			
			float vM = velMagPath[tick];
			float vY = velPathY[tick];
			float pY = posPathY[tick];
			float pZ = posPathZ[tick];
			// -----------------
			// set horizontal velocity magnitude to magnitude from paths
			Vector3 vel = rb.velocity;
			vel.y = 0f;
			vel *= 100f;
			vel = Vector3.ClampMagnitude(vel, vM);
			vel.y = vY;
			rb.velocity = vel;
			// -----------------

			// 2/7/21 -- found to be the cause ghost stuttering
			/*
			// set y-position from path
			Vector3 pos = transform.position;
			pos.y = pY;
			transform.position = pos;
			*/
			
			
			if(oc.trackSegment == 4){
				Vector3 targetPos = new Vector3(transform.position.x, pY, pZ);
				if(Vector3.Distance(transform.position, globalController.raceManager.finishLine.transform.position) > 2f){
					transform.position = Vector3.Lerp(transform.position, targetPos, 1f * dTime);
				}
				else{
					transform.position = targetPos;
				}
			}
			
		}

	}
	
	public void updateEnergy(float speed, float swingTimeBonus){
		float energyCost;
		energyCost = energyCost_base * dTime;
		energyCost *= Mathf.Pow(speed / 27f, 2.5f);
		energyCost *= 1f + (1f - (swingTimeBonus / 2.0736f));
		energyCost *= (2f - fitness);
		energyCost *= (1f - cruiseWeight/3f);
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
		friction = gyro.transform.forward*-1f * speedHoriz * frictionMagnitude;
		if(rightFootScript.groundContact || leftFootScript.groundContact){
			rb.AddForce(friction, ForceMode.Force);
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
			modifier *= modifier * modifier * modifier * modifier * modifier * modifier * modifier;
			modifiedPower *= modifier;
		}
		// -----------------
		// reduce power if leaned back (based on torsoAngle)
		if(torsoAngle < torsoAngle_upright){
			modifier = torsoAngle/torsoAngle_upright;
			modifier *= modifier;
			modifiedPower *= modifier;
		}
		// -----------------
		// reduce power if on curve
		if(onCurve){
			modifier = .925f * curve_power;
			modifiedPower *= modifier;
		}
		// -----------------
		// modify power from energy
		if(energy < 80f){
			if(energy >= energyBurnoutThreshold) {
				modifier *= (energy / 80f);
			}
			else{
				energyBurnoutMod = Mathf.Lerp(energyBurnoutMod, .5f, .25f*(speedHoriz/27f)*dTime);
				modifier *= ((50f*energyBurnoutMod) / 80f);
			}
			modifier = Mathf.Pow(modifier, .95f);
			modifiedPower *= modifier;
		}
		
		// -----------------
		power = modifiedPower;
		power *= powerMod;
	}
	
	void applySpeedModifiers(){
		
		// get horizontal velocity
		Vector3 velHoriz = rb.velocity;
		float velY = velHoriz.y;
		velHoriz.y = 0f;
		if(gyro.transform.forward.z > 0){
			if(velHoriz.z < 0f){
				velHoriz.z = 0f;
			}
		}
		else{
			if(velHoriz.z > 0f){
				velHoriz.z = 0f;
			}
		}
		
		// limit maximum increase in velocity based on driveModifier
		float maxD = .5f * driveModifier + speedHoriz_lastFrame;
		if(speedHoriz > maxD){
			velHoriz = Vector3.ClampMagnitude(velHoriz, maxD);
		}
		
		// limit positive vertical velocity
		if(velY > maxUpSpeed){
			velY = maxUpSpeed;
		}
		
		// set new velocity from changes
		Vector3 newVelocity = velHoriz;
		newVelocity.y = velY;
		rb.velocity = newVelocity;
	}
	
	// set layer weight values to corresponding variables
	void updateLayerWeights(){
			/* // -----------------
				layers
				1	cruise
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
			cruiseWeight -= .7f * dTime;
			if(cruiseWeight < 0f){
				cruiseWeight = 0f;
			}
		}else{
			if(speedHoriz > 15f){
				cruiseWeight += cruise * dTime;
			}
		}
		
		if(speedHoriz < 10f){
			cruiseWeight *= speedHoriz/10f;
		}
		cruiseWeight *= 313f/torsoAngle;
		if(cruiseWeight > 1f){
			cruiseWeight = 1f;
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
	
	public IEnumerator launch(bool leadLegLaunch){
		raceManager.startingBlocks_current[attributes.lane-1].GetComponent<StartingBlockController>().addLaunchForce();
		
		if(isPlayer){ 
			if(raceManager.viewMode == RaceManager.VIEW_MODE_LIVE){
				globalController.audioController.playSound(AudioController.BLOCK_EXIT, 0f);
			}
		}
		
		animator.SetBool("launch", true);
		cruiseWeight = 0f;
		float launchPower = 27.489f * launch_power;
		
		// reduce launch power if launched with non-dominant leg
		if(!leadLegLaunch){
			launchPower *= .2f;
		}
		
		Vector3 launchVecVert = Vector3.up * launch_power * .35f;
		
		for(int i = 0; i < 12; i++){
			rb.AddForce((gyro.transform.forward + launchVecVert) * launchPower, ForceMode.Force);
			yield return new WaitForSeconds(1f / 90f);
		}
		for(int i = 0; i < 10; i++){
			yield return new WaitForSeconds(1f / 90f);
		}
		animator.SetBool("launch", false);
	}
	
	public IEnumerator pop(float delay){
		yield return new WaitForSeconds(delay);
		popParticles.Play();
		sweatParticles.gameObject.SetActive(false);
		dustParticles.gameObject.SetActive(false);
		hide();
	}
	
	public void hide(){
		attributes.smr_dummy.enabled = false;
		attributes.smr_top.enabled = false;
		attributes.smr_bottoms.enabled = false;
		attributes.smr_shoes.enabled = false;
		attributes.smr_socks.enabled = false;
		attributes.smr_headband.enabled = false;
		attributes.smr_sleeve.enabled = false;
	}
	
	
	
	
}