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
	// -----------------
	float freeTicks;
	float downTicks;
	float ticksPassedLeft;
	float ticksPassedRight;
	float frequencyLeft;
	float frequencyRight;
	int inputLeft;
	int inputRight;
	float time;
	float tickRate;
	float pace;
	bool maintenanceFlag;
	// -----------------
	float difficulty;
	float cadenceModifier;
	float quicknessModifier;
	float powerModifier;
	// -----------------
	float torsoAngle_upright;
	float torsoAngle_forward;
	float torsoAngle_back;
	bool topEndFlag;
	bool forward;
	float zSpeed;
	float transitionPivotSpeed;
	float zSpeedOverTransitionPivotSpeed;
	float modifierCapSpeed;
	float curveModifier;
	float energyThreshold;
	
	float dTime;
	
	
	public void runAI(int tick){
		
		dTime = Time.deltaTime;
		
		if(!reaction){
			reaction = raceManager.raceTime > reactionTime;
		}
		else{
			adjustStride();
			// -----------------
			if(ticksPassedLeft >= frequencyLeft){
				if(inputLeft == 0){
					inputLeft = 1;
					frequencyLeft = downTicks;
					if(maintenanceFlag){
						ticksPassedRight = (freeTicks * .25f);
					}
				}
				else{
					inputLeft = 0;
					frequencyLeft = freeTicks;
				}
				ticksPassedLeft = 0;
			}
			att.leftInputPath[tick] = inputLeft;
			ticksPassedLeft += tickRate;
			// --
			if(ticksPassedRight >= frequencyRight){
				if(inputRight == 0){
					inputRight = 1;
					frequencyRight = downTicks;
				}
				else{
					inputRight = 0;
					frequencyRight = freeTicks;
				}
				ticksPassedRight = 0;
			}
			att.rightInputPath[tick] = inputRight;
			ticksPassedRight += tickRate;
		}

	}
	
	
	
	public void adjustStride(){
		float torsoAngle = anim.torsoAngle;
		zSpeed = anim.speedHoriz;
		if(anim.onCurve){ curveModifier = 1.5f; } else{ curveModifier = 1f; };
		zSpeedOverTransitionPivotSpeed = (zSpeed / (modifierCapSpeed*curveModifier*1.1f));
		if(zSpeedOverTransitionPivotSpeed > 1f){
			if(zSpeedOverTransitionPivotSpeed > 1.2f){
				zSpeedOverTransitionPivotSpeed = 1.2f;
			}
		}
		else{
			zSpeedOverTransitionPivotSpeed = 1f;
		}
		if(zSpeed < 3f){
			zSpeedOverTransitionPivotSpeed = .5f;
		}
		tickRate = (int)(100f * zSpeedOverTransitionPivotSpeed);
		// -----------------
		if(!topEndFlag){
			if(torsoAngle < torsoAngle_upright){
				downTicks += 105f;
				topEndFlag = true;
			}
		}
		else if(topEndFlag){
			if(forward){
				downTicks -= 6f;
				forward = false;
			}
			if(raceManager.raceTick % 10 == 0){
				if(torsoAngle > torsoAngle_upright){
					forward = true;
				}
				else{
					if(!maintenanceFlag){
						setTicks(2400f);
						maintenanceFlag = true;
					}
					if(!anim.leanLock){
						if(torsoAngle < torsoAngle_back){
							anim.leanLock = true;
							att.leanLockTick = raceManager.raceTick;
						}
					}
				}
			}
			
			float energy = anim.energy;
			
			if(pace < 1f){
				if(energy < energyThreshold){
					if(energy > 30f){
						pace = Mathf.Lerp(pace, 1f * (1f - energy/1000f), 1f * dTime);
					}else{ pace = 1f; }
					setTicks(2400f);
				}
			}
		}
		
	}
	
	void setTicks(float _freeTicks){
		freeTicks = _freeTicks * (2f - cadenceModifier) * (2f-pace);
		downTicks = (3575f - _freeTicks) * (2f - cadenceModifier) * (2f-pace);
	}
	
	
	
	public void init(float _difficulty){
		
		difficulty = _difficulty;
		// -----------------
		cadenceModifier = 1f;
		cadenceModifier *= Mathf.Pow(att.TURNOVER, .01f);
		// -----------------
		quicknessModifier = Mathf.Pow(difficulty, .3f);
		anim.quicknessMod = quicknessModifier;
		// -----------------
		powerModifier = Mathf.Pow(difficulty, 2f);
		att.POWER *= powerModifier;
		anim.powerMod = powerModifier;
		// -----------------
		leftInputPath = att.leftInputPath;
		rightInputPath = att.rightInputPath;
		// -----------------
		reaction = false;
		reactionTime = Random.Range(.01f, .03f);
		reactionTime = Random.Range(.01f, .01f + (2f-Mathf.Pow(difficulty,10f))/10f) * Mathf.Pow((2f - difficulty), 4f);
		// -----------------
		topEndFlag = false;
		maintenanceFlag = false;
		forward = false;
		transitionPivotSpeed = att.TRANSITION_PIVOT_SPEED;
		modifierCapSpeed = 23f * difficulty;
		// -----------------
		//freeTicks = 2650f * (2f - cadenceModifier);
		freeTicks = 2700f * (2f - cadenceModifier);
		//downTicks = 950f * (2f - cadenceModifier);
		downTicks = 1000f * (2f - cadenceModifier);
		tickRate = 100f;
		if(att.leadLeg == 0){
			frequencyLeft = downTicks;
			frequencyRight = freeTicks;
			inputLeft = 1;
			inputRight = 0;
			ticksPassedLeft = 0;
			ticksPassedRight = (downTicks / 2f) + 310f;
		}
		else{
			frequencyLeft = freeTicks;
			frequencyRight = downTicks;
			inputLeft = 0;
			inputRight = 1;
			ticksPassedLeft = (downTicks / 2f) + 310f;
			ticksPassedRight = 0;	
		}
		// -----------------
		setSpecialAttributes();
	}
	
	
	void setSpecialAttributes(){
		if(raceManager.raceEvent == RaceManager.RACE_EVENT_100M){
			torsoAngle_upright = anim.torsoAngle_upright - 3f;
			pace = .95f;
			energyThreshold = 0f;
			att.CRUISE = .5f;
			att.KNEE_DOMINANCE *= .9f;
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_200M){
			torsoAngle_upright = anim.torsoAngle_upright - 5.25f;
			pace = (Random.Range(.97f, 1f)+Random.Range(.97f, 1f)+Random.Range(.97f, 1f)+Random.Range(.97f, 1f))/4f * cadenceModifier;
			pace = .95f;
			energyThreshold = 75f;
			att.CRUISE = .5f;
			att.KNEE_DOMINANCE *= .9f;
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_400M){
			torsoAngle_upright = anim.torsoAngle_upright - 7f;
			energyThreshold = 75f;
			pace = .8f;
			att.CRUISE = .8f;
			att.KNEE_DOMINANCE *= .95f;
			att.FITNESS *= 1.05f;
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_60M){
			torsoAngle_upright = anim.torsoAngle_upright - 3f;
			pace = .95f;
			energyThreshold = 0f;
			att.CRUISE = .5f;
			att.KNEE_DOMINANCE *= .9f;
		}
		// -----------------
		torsoAngle_back = torsoAngle_upright;
	}
}
