﻿using System.Collections;
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
	float torsoAngle_min;
	float torsoAngle_max;
	bool fullUpright;
	bool forward;
	bool back;
	bool dip;
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
			att.leftInputPath[tick] = inputLeft + randomDeviation;
			ticksPassedLeft += (tickRate + randomDeviation);
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
			ticksPassedRight += (tickRate + randomDeviation);
		}
	}
	
	
	public void adjustStride(){
		float torsoAngle = anim.torsoAngle;
		zSpeed = anim.rb.velocity.z;
		zSpeedOverTransitionPivotSpeed = (zSpeed / (transitionPivotSpeed-110f)) + .55f;
		if(zSpeedOverTransitionPivotSpeed > 1f){
			if(zSpeedOverTransitionPivotSpeed > 1.2f){
				zSpeedOverTransitionPivotSpeed = 1.2f;
			}
		}
		else{
			zSpeedOverTransitionPivotSpeed = 1f;
		}
		tickRate = (int)(100f * (zSpeedOverTransitionPivotSpeed));
		// -----------------
		if(!fullUpright){
			if(torsoAngle < torsoAngle_min){
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
				downTicks -= 6;
				forward = false;
			}
			if(raceManager.raceTick % 10 == 0){
				if(torsoAngle > torsoAngle_min){
					forward = true;
				}
				else{
					back = true;
				}
			}
			if(raceManager.finishLine.transform.position.z - transform.position.z < 20f){
				if(!dip){
					downTicks += 500;
					freeTicks -= 400;
					dip = true;
				}
			}
		}

		
	}
	
	
	
	public void init(float difficulty){
		
		anim = gameObject.GetComponent<PlayerAnimationV2>();
		att = anim.attributes;
		
		float cadenceModifier = difficulty;
		cadenceModifier *= cadenceModifier;
		if(cadenceModifier < .7f){
			cadenceModifier = .7f;
		}
		cadenceModifier *= att.TURNOVER;
		//cadenceModifier += Random.Range(-.015f, .015f);
		// -----------------
		float quicknessModifier = difficulty;
		quicknessModifier *= quicknessModifier*quicknessModifier;
		if(quicknessModifier < .6f){
			quicknessModifier = .6f;
		}
		//quicknessModifier += Random.Range(-.015f, .015f);
		// -----------------
		float driveModifier = 1f - ((1f - (2f - att.KNEE_DOMINANCE))*.1f);
		// -----------------
		
		
		

		// -----------------
		att.QUICKNESS = quicknessModifier;
		leftInputPath = att.leftInputPath;
		rightInputPath = att.rightInputPath;
		// -----------------
		reaction = false;
		reactionTime = Random.Range(.17f, .21f);
		randomRange = 0f + ((1f-difficulty) * 10f);
		// -----------------
		freeTicks = (int)(2625f * (2f-cadenceModifier));
		downTicks = (int)((950f * driveModifier) * (2f-cadenceModifier));
		ticksPassedLeft = 0;
		ticksPassedRight = downTicks / 2 + (int)(310f*(2f-cadenceModifier));
		frequencyLeft = downTicks;
		frequencyRight = freeTicks;
		inputLeft = 1;
		inputRight = 0;
		tickRate = 100;
		// -----------------
		torsoAngle_min = 330f /** Mathf.Pow((2f-att.legX), .2f)*/;
		torsoAngle_max = torsoAngle_min + .25f;
		fullUpright = false;
		forward = false;
		back = false;
		dip = false;
		transitionPivotSpeed = att.TRANSITION_PIVOT_SPEED;
	}
	
}
