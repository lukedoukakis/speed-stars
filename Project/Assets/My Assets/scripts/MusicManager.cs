using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
	
	public Button audioButton;
	public Sprite audio_on;
	public Sprite audio_off;
	
	
	public AudioClip[] playlist;
	public List<AudioClip> tracks;
	public AudioClip firstPlayTrack;
	
	int trackIndex;
	public bool sound;
	public float volume;
	
	public AudioSource audio;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void play(){
		StartCoroutine("playMusic");
	}
	
	IEnumerator playMusic(){
		while(true){
			if(!audio.isPlaying){
				nextTrack();
			}
			yield return new WaitForSeconds(1f);
		}
	}
	
	public void nextTrack(){
		if(trackIndex == playlist.Length-1){ trackIndex = -1; }
		AudioClip clip = playlist[++trackIndex];
		audio.clip = clip;
		audio.Play();
	}
	
	public void toggleSound(){
		if(sound){
			mute();
		}
		else{
			unmute();
		}
	}
	
	public void mute(){
		audio.volume = 0f;
		sound = false;
		audioButton.image.sprite = audio_off;
	}
	
	public void unmute(){
		audio.volume = volume;
		audioButton.image.sprite = audio_on;
		sound = true;
	}
	
	
	public void init(bool firstPlay){
		int tracksLength = tracks.Count;
		playlist = new AudioClip[tracksLength];
		int randIndex;
		for(int i = 0; i < tracksLength; i++){
			randIndex = Random.Range(0, tracks.Count);
			playlist[i] = tracks[randIndex];
			tracks.RemoveAt(randIndex);
		}
		
		if(firstPlay){
			playlist[0] = firstPlayTrack;	
		}
		
		trackIndex = 0;
		volume = .2f;
		sound = true;
		
		play();
		
	}
}
