using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_AI : MonoBehaviour
{
	public RaceManager raceManager;
	public PlayerAnimationV2 anim;
	public PlayerAttributes att;
	public int preset;
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
	float knee_dominance_modifier;
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
	float topSpeed;
	
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
			if(ticksPassedLeft >= frequencyLeft){
				if(inputLeft == 0){
					inputLeft = 1;
					frequencyLeft = downTicks;
					if(maintenanceFlag){
						ticksPassedRight = (freeTicks * .25f);
					}
				}
				else if(inputLeft == 1){
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
				else if(inputRight == 1){
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
							Debug.Log("leanLock at tick: " + att.leanLockTick);
						}
					}
				}
			}
			
			float energy = anim.energy;
			
			if(energy >= energyThreshold){
			
			}
			else{
				//Debug.Log("energy adjust");
				//anim.leanLock = false;
				//torsoAngle_back = torsoAngle_upright - (.05f * ((energy+35f)*.001f));
				float newPace;
				if(energy > 30f){
					newPace = Mathf.Lerp(pace, 1f * (1f - energy/1000f), 30f * Time.deltaTime);
				}else{ newPace = 1f; }
				if(newPace > pace){
					pace = newPace;
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
		
		anim = gameObject.GetComponent<PlayerAnimationV2>();
		att = anim.attributes;
		preset = anim.preset;
		// -----------------
		cadenceModifier = 1f;
		//cadenceModifier *= Mathf.Pow(difficulty, .5f);
		cadenceModifier *= Mathf.Pow(att.TURNOVER, .01f);
		// -----------------
		quicknessModifier = Mathf.Pow(difficulty, .3f);
		
		//quicknessModifier = 1f;
		anim.quicknessMod = quicknessModifier;
		// -----------------
		powerModifier = Mathf.Pow(difficulty, 2f);
		//powerModifier = .9f;
		anim.powerMod = powerModifier;
		// -----------------
		leftInputPath = att.leftInputPath;
		rightInputPath = att.rightInputPath;
		// -----------------
		reaction = false;
		reactionTime = Random.Range(.12f, .17f) * (2f - difficulty);
		reactionTime = .15f;
		// -----------------
		freeTicks = 2650f * (2f - cadenceModifier);
		downTicks = 950f * (2f - cadenceModifier);
		frequencyLeft = downTicks;
		frequencyRight = freeTicks;
		inputLeft = 1;
		inputRight = 0;
		tickRate = 100f;
		// -----------------
		topEndFlag = false;
		maintenanceFlag = false;
		forward = false;
		transitionPivotSpeed = att.TRANSITION_PIVOT_SPEED;
		//knee_dominance_modifier = Mathf.Pow(att.KNEE_DOMINANCE,.5f);
		knee_dominance_modifier = 1f;
		modifierCapSpeed = 23f * difficulty;
		// -----------------
		ticksPassedLeft = 0;
		ticksPassedRight = (downTicks / 2f) + 310f;
		// -----------------
		setPresetStats();
	}
	
	
	void setPresetStats(){
		if(raceManager.raceEvent == RaceManager.RACE_EVENT_100M){
			if(preset < 4){
				pace = 1f;
				energyThreshold = 0f;
				topSpeed = 27.5f;
			}
			else{
				pace = float.Parse(TextReader.getAttribute(preset, "pace100"));
				energyThreshold = float.Parse(TextReader.getAttribute(preset, "energyThreshold100"));
			}
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_200M){
			if(preset < 4){
				pace = (Random.Range(.92f, 1f)+Random.Range(.92f, 1f)+Random.Range(.92f, 1f)+Random.Range(.92f, 1f))/4f * cadenceModifier;
				energyThreshold = 95f;
				topSpeed = 26f;
			}
			else{
				pace = float.Parse(TextReader.getAttribute(preset, "pace200"));
				energyThreshold = float.Parse(TextReader.getAttribute(preset, "energyThreshold200"));
				if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_YOHANBLAKE){
					att.POWER *= .805f;
				}
				if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_WAYDEVANNIEKERK){
					att.POWER *= .79f;
				}
			}
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_400M){
			if(preset < 4){
				pace = (Random.Range(.7f, 1f)+Random.Range(.7f, 1f)+Random.Range(.7f, 1f)+Random.Range(.7f, 1f))/4f * (2f - att.KNEE_DOMINANCE);
				energyThreshold = 65f;
				topSpeed = 26f;
			}
			else{
				pace = float.Parse(TextReader.getAttribute(preset, "pace400"));
				energyThreshold = float.Parse(TextReader.getAttribute(preset, "energyThreshold400"));
				
				if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_MICHAELJOHNSON){
					att.POWER *= .88f;
					pace = .97f;
				}
				if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_YOHANBLAKE){
					att.POWER *= .75f;
				}
				if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_WAYDEVANNIEKERK){
					att.POWER *= .62f;
					pace = .8f;
				}
			}
		}
		else if(raceManager.raceEvent == RaceManager.RACE_EVENT_60M){
			pace = 1f;
			energyThreshold = 0f;
			topSpeed = 27.5f;
		}
		else{
			pace = 1f;
			energyThreshold = 0f;
		}
		// -----------------
		if(preset < 4){
			torsoAngle_upright = 312f * Mathf.Pow(att.KNEE_DOMINANCE,.01f);
		}
		else{
			torsoAngle_upright = float.Parse(TextReader.getAttribute(preset, "torsoAngle"));
		}
		torsoAngle_back = torsoAngle_upright;
	}
	
}
