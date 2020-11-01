using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	
	public AudioSource[] audioSources;
	public static int GUNSHOT = 0;
	public static int BLOCK_RATTLE = 1;
	public static int BLOCK_EXIT= 2;
	
	public void playSound(int soundIndex){
		audioSources[soundIndex].Play(0);
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
