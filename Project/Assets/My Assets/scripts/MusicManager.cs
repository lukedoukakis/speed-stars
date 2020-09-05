using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	
	public static AudioClip bgm1;
	
    // Start is called before the first frame update
    void Start()
    {
        bgm1 = Resources.Load<AudioClip>("bgm_bensound-house");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public static void setBackgroundMusic(AudioSource ac, string music){
		switch (music){
			case "bgm1" :
				ac.clip = bgm1;
				break;
			default:
				break;
		}
	}
}
