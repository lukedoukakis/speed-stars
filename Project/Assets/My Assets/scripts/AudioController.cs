using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	
	public AudioSource[] audioSources;
	public static int GUNSHOT = 0;
	public static int BLOCK_RATTLE = 1;
	public static int BLOCK_EXIT= 2;
	public static int CHEERING = 3;
	public static int VOICE_READY = 4;
	public static int VOICE_SET = 5;
	
	public float soundVolume;
	
	public void playSound(int soundIndex, float delay){
		audioSources[soundIndex].PlayDelayed(Convert.ToUInt64(delay));
	}
	
	public void setVolume(float vol){
		soundVolume = vol*5f;
		foreach(AudioSource audio in audioSources){
			audio.volume = soundVolume;
		}
		PlayerPrefs.SetFloat("Audio Volume", vol);
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
