using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerAudio : MonoBehaviour
{
	
	public AudioSource as_startingGun, as_blockRattle, as_blockExit, as_footfall, as_wind;
	
    // Start is called before the first frame update
    void Start()
    {
		
		
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void playSound(string sound){
		switch (sound){
			case "Starting Gun" :
				as_startingGun.Play(0);
				break;
			case "Block Rattle" :
				as_blockRattle.Play(0);
				break;
			case "Block Exit" :
				as_blockExit.Play(0);
				break;
			case "Footfall" :
				as_footfall.Play(0);
				break;
			case "Wind" :
				as_wind.Play(0);
				break;
			default:
				break;
			
		}
	}
}
