using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	
	public AudioSource[] audioSources;
	public static int GUNSHOT = 0;
	public static int BLOCK_RATTLE = 1;
	public static int BLOCK_EXIT= 2;
	public static int CHEERING = 3;
	
	public float soundVolume;
	
	public void playSound(int soundIndex){
		audioSources[soundIndex].Play(0);
	}
	
	public void setVolume(float vol){
		soundVolume = vol*3f;
		foreach(AudioSource audio in audioSources){
			audio.volume = soundVolume;
		}
	}
	
	public void easeSoundVolume(int soundIndex, float vol){
		StartCoroutine(easeVolume(soundIndex, vol));
	}
	IEnumerator easeVolume(int soundIndex, float vol){
		AudioSource audio = audioSources[soundIndex];
		float f;
		while(Mathf.Abs(audioSources[soundIndex].volume  - vol) > .1f){
			f = audioSources[soundIndex].volume;
			audio.volume = Mathf.Lerp(f, vol, 60f*Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		audioSources[soundIndex].volume = vol;
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
