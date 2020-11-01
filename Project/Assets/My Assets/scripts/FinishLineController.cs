using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineController : MonoBehaviour
{
	
	public RaceManager raceManager;
	public ParticleSystem yayParticles;
	
	int finishers;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void init(){
		finishers = 0;
	}
	
	void OnTriggerEnter(Collider col){
		GameObject g = col.gameObject;
		if(g.tag == "Chest"){
			GameObject racer = g.transform.parent.parent.parent.parent.parent.parent.parent.gameObject;
			if(raceManager.raceTick > 500f){
				raceManager.addFinisher(racer);
				if(finishers == 0){
					if(racer == raceManager.player){
						yayParticles.transform.position = racer.transform.position;
						yayParticles.Play();
					}
				}
				finishers++;
			}
		}
	}
		
		
		
		
	
	
}
