using System.Collections;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	public RaceManager raceManager;
	PlayerAttributes attributes;
	public PlayerAnimationV2 animation;
	
	public bool finished;
	public float time;
	public int ticks;
	public bool isCounting;
	public bool isRecording;
	
	
	// Start is called before the first frame update
	void Start(){
		reset();
	}
	
    // Update is called once per frame
    void Update()
    {
		if(isCounting){
			time += 1f * Time.deltaTime;
		}
		
		
    }
	
	void FixedUpdate()
	{
		if(isRecording){
			if(tag == "Player" /*|| tag == "Bot"*/){
				recordInput(ticks);
			}
			ticks++;
		}	
	}
	
	public void start(){
		finished = false;
		time = 0f;
		ticks = 0;
		isCounting = true;
		isRecording = true;
	}
	
	public void stop(){
		if(!finished){
			//Time.timeScale = 0.0f;
			if(tag == "Player" || tag == "Bot"){
				attributes.finishTime = time;
				attributes.pathLength = ticks;
			}
			finished = true;
			isCounting = false;
			isRecording = false;
		
			raceManager.addFinisher(this.gameObject);
		}
		
		
	}
	
	public void reset(){
		finished = false;
		time = 0f;
		ticks = 0;
		isCounting = false;
		isRecording = false;
	}
	
	public void recordInput(int tick){
		if(animation.rightInput){
			attributes.rightInputPath[tick] = 1;
		}
		else{
			attributes.rightInputPath[tick] = 0;
		}
		if(animation.leftInput){
			attributes.leftInputPath[tick] = 1;
		}
		else{
			attributes.leftInputPath[tick] = 0;
		}
		attributes.velPathY[tick] = animation.velocity.y;
		attributes.velPathZ[tick] = animation.velocity.z;
		attributes.posPathZ[tick] = transform.position.z;
		attributes.posPathY[tick] = transform.position.y;
	}
	
	
	public void init(){
		attributes = GetComponent<PlayerAttributes>();
		reset();
	}
}
