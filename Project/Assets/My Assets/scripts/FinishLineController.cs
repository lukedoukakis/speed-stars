using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineController : MonoBehaviour
{
	
	public RaceManager raceManager;
	public ParticleSystem yayParticles;
	public bool isActive;
	
	int finishers;
	
	public void init(){
		isActive = false;
		finishers = 0;
	}
	
	void OnTriggerEnter(Collider col){
		GameObject g = col.gameObject;
		if(g.tag == "Chest"){
			GameObject racer = g.transform.parent.parent.parent.parent.parent.parent.parent.gameObject;
			if(raceManager.raceTime > 5f){
				if(racer == raceManager.player){
					if(isActive){
						raceManager.addFinisher(racer);
						finishers++;
					}
				}
				else{
					raceManager.addFinisher(racer);
					finishers++;
				}
			}
		}
	}
		
		
		
		
	
	
}
