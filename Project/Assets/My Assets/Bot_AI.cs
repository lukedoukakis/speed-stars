using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_AI : MonoBehaviour
{
	public RaceManager raceManager;
	public PlayerAnimationV2 anim;
	public PlayerAttributes att;
	// -----------------
	int[] leftInputPath;
	int[] rightInputPath;
	// -----------------
	bool reaction;
	float reactionTime;
	float randomRange;
	// -----------------
	int freeTicks;
	int downTicks;
	int ticksPassedLeft;
	int ticksPassedRight;
	int frequencyLeft;
	int frequencyRight;
	int inputLeft;
	int inputRight;
	float time;
	int tickRate;
	// -----------------
	float minLean;
	float maxLean;
	bool fullUpright;
	bool forward;
	bool back;
	float zSpeed;
	float transitionPivotSpeed;
	float zSpeedOverTransitionPivotSpeed;
	
    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {	
    }
	
	public void runAI(int tick){
		if(!reaction){
			reaction = raceManager.raceTime > reactionTime;
		}
		else{
			adjustStride();
			// -----------------
			int randomDeviation = (int)(Random.Range(0f-randomRange,randomRange));
			// -----------------
			if(ticksPassedLeft >= frequencyLeft){
				if(inputLeft == 0){
					inputLeft = 1;
					frequencyLeft = downTicks;
				}
				else if(inputLeft == 1){
					inputLeft = 0;
					frequencyLeft = freeTicks;
				}
				ticksPassedLeft = 0;
			}
			att.leftInputPath[tick] = inputLeft;
			ticksPassedLeft += tickRate + randomDeviation;
			
			//Debug.Log(inputLeft);
			// --
			if(ticksPassedRight >= frequencyRight){
				if(inputRight == 0){
					inputRight = 1;
					frequencyRight = downTicks;
				}
				else if(inputRight == 1){
					inputRight = 0;
					frequencyRight = freeTicks;
				}
				ticksPassedRight = 0;
			}
			att.rightInputPath[tick] = inputRight;
			ticksPassedRight += tickRate  + randomDeviation;
		}
	}
	
	
	public void adjustStride(){
		float zTilt = anim.zTilt;
		zSpeed = anim.velocity.z;
		zSpeedOverTransitionPivotSpeed = (zSpeed / (transitionPivotSpeed-50f)) + .55f;
		if(zSpeedOverTransitionPivotSpeed > 1f){
			if(zSpeedOverTransitionPivotSpeed > 1.1f){
				zSpeedOverTransitionPivotSpeed = 1.1f;
			}
		}
		else{
			zSpeedOverTransitionPivotSpeed = 1f;
		}
		tickRate = (int)(100f * (zSpeedOverTransitionPivotSpeed));
		// -----------------
		if(!fullUpright){
			if(zTilt < minLean){
				downTicks += 100;
				fullUpright = true;
				back = true;
			}
		}
		if(fullUpright){
			if(back){
				downTicks += 5;
				back = false;
			}
			if(forward){
				downTicks -= 5;
				forward = false;
			}
			if(raceManager.raceTick % 10 == 0){
				if(zTilt > minLean){
					forward = true;
				}
				else{
					back = true;
				}
			}
			
		}

		
	}
	
	
	
	public void init(float difficulty){
		
		float cadenceModifier = difficulty;
		cadenceModifier *= cadenceModifier;
		if(cadenceModifier < .7f){
			cadenceModifier = .7f;
		}
		cadenceModifier += Random.Range(-.015f, .015f);
		// -----------------
		float quicknessModifier = difficulty;
		quicknessModifier *= quicknessModifier*quicknessModifier;
		if(quicknessModifier < .6f){
			quicknessModifier = .6f;
		}
		quicknessModifier += Random.Range(-.015f, .015f);
		// -----------------
		float driveModifier = Random.Range(.998f, 1.002f);
		// -----------------
		
		
		
		anim = gameObject.GetComponent<PlayerAnimationV2>();
		att = anim.attributes;
		//att = gameObject.GetComponent<PlayerAttributes>();
		// -----------------
		att.QUICKNESS_BASE = quicknessModifier;
		leftInputPath = att.leftInputPath;
		rightInputPath = att.rightInputPath;
		// -----------------
		reaction = false;
		reactionTime = Random.Range(.19f, .21f);
		randomRange = 0f + ((1f-difficulty) * 10f);
		// -----------------
		freeTicks = (int)((4400f * driveModifier) * (2f-cadenceModifier));
		downTicks = (int)((1886f * (2f-driveModifier)) * (2f-cadenceModifier));
		ticksPassedLeft = 0;
		ticksPassedRight = downTicks / 2 + (int)(315f*(2f-cadenceModifier));
		frequencyLeft = downTicks;
		frequencyRight = freeTicks;
		inputLeft = 1;
		inputRight = 0;
		tickRate = 100;
		// -----------------
		minLean = -20f - (1f - att.STRENGTH_BASE);
		maxLean = minLean + .25f;
		fullUpright = false;
		forward = false;
		back = false;
		transitionPivotSpeed = att.TRANSITION_PIVOT_SPEED;
	}
	
}
