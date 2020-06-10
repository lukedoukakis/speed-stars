using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_AI : MonoBehaviour
{
	public PlayerAnimationV2 anim;
	public TimerController timer;
	// -----------------
	public PlayerAttributes att;
	int[] leftInputPath;
	int[] rightInputPath;
	// -----------------
	bool reaction;
	float reactionTime;
	int randomRange;
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
		if(!reaction){
			if(timer.time > reactionTime){
				reaction = true;
			}
		}
    }
	
	public void runAI(int tick){
		if(reaction){
			adjustStride();
			// -----------------
			int randomDeviation = Random.Range(0-randomRange,randomRange);
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
		zSpeedOverTransitionPivotSpeed = (zSpeed / transitionPivotSpeed) + .55f;
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
			if(timer.ticks % 10 == 0){
				if(zTilt > minLean){
					forward = true;
				}
				else{
					back = true;
				}
			}
			
		}

		
	}
	
	
	
	public void init(){
		anim = gameObject.GetComponent<PlayerAnimationV2>();
		timer = gameObject.GetComponent<TimerController>();
		// -----------------
		att = gameObject.GetComponent<PlayerAttributes>();
		leftInputPath = att.leftInputPath;
		rightInputPath = att.rightInputPath;
		// -----------------
		reaction = false;
		reactionTime = Random.Range(.19f, .21f);
		randomRange = 0;
		// -----------------
		freeTicks = 3000;
		downTicks = 1250;
		ticksPassedLeft = 0;
		ticksPassedRight = downTicks / 2 + 325;
		frequencyLeft = downTicks;
		frequencyRight = freeTicks;
		inputLeft = 1;
		inputRight = 0;
		tickRate = 100;
		// -----------------
		minLean = -13f - ((1f - att.STRENGTH_BASE) * 10f);
		maxLean = minLean + .25f;
		fullUpright = false;
		forward = false;
		back = false;
		transitionPivotSpeed = att.TRANSITION_PIVOT_SPEED;
	}
	
}
