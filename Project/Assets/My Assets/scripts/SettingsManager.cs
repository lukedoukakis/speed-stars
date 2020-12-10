using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	
	public static KeyCode controlsLeft;
	public static KeyCode controlsRight;
	
	
	public GlobalController gc;
	public GameObject SettingsWindow;
	[SerializeField] GameObject controlsButtonLeft;
	[SerializeField] GameObject controlsButtonRight;
	[SerializeField] GameObject controlsButtonLeft2;
	[SerializeField] GameObject controlsButtonRight2;
	[SerializeField] Slider cameraSlider;
	[SerializeField] float cameraDistance;
	
	
	public void init(KeyCode l, KeyCode r, float camD, int camGameplayMode){
		controlsLeft = l;
		controlsRight = r;
		controlsButtonLeft.transform.Find("Text").GetComponent<Text>().text = l.ToString();
		controlsButtonRight.transform.Find("Text").GetComponent<Text>().text = r.ToString();
		controlsButtonLeft2.transform.Find("Text").GetComponent<Text>().text = l.ToString();
		controlsButtonRight2.transform.Find("Text").GetComponent<Text>().text = r.ToString();
		
		cameraSlider.value = cameraDistance = camD;
	}
	
	public void show(){
		SettingsWindow.SetActive(true);
	}
	public void hide(){
		SettingsWindow.SetActive(false);
	}
	
	public void setControls(int side, KeyCode k){
		if(side == 0){
			controlsLeft = k;
		}
		else{
			controlsRight = k;
		}
	}
	
	public void getControlsInput(int side){
		StartCoroutine(readInput(side));
	}
	IEnumerator readInput(int side){
		
		Text text;
		Text text2;
		Button button;
		Button button2;
		
		KeyCode origKey;
		
		// set buttons
		if(side == 0){
			text = controlsButtonLeft.transform.Find("Text").GetComponent<Text>();
			text2 = controlsButtonLeft2.transform.Find("Text").GetComponent<Text>();
			button = controlsButtonLeft.GetComponent<Button>();
			button2 = controlsButtonLeft2.GetComponent<Button>();
			origKey = controlsLeft;
		}
		else{
			text = controlsButtonRight.transform.Find("Text").GetComponent<Text>();
			text2 = controlsButtonRight.transform.Find("Text").GetComponent<Text>();
			button = controlsButtonRight.GetComponent<Button>();
			button2 = controlsButtonRight.GetComponent<Button>();
			origKey = controlsRight;
		}
		text.text = "[PRESS KEY]";
		text2.text = "[PRESS KEY]";
		button.interactable = false;
		button2.interactable = false;
		
		// read input for 5 seconds
		KeyCode k = KeyCode.None;
		float t = 0f;
		while(t < 5f){
			foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode))){
				if(Input.GetKey(kcode)){
					//Debug.Log("KeyCode down: " + kcode);
					k = kcode;
					t = 5f;
					break;
				}
			}
			t += Time.deltaTime;
			yield return null;
		}
		
		// set controls
		if(k != KeyCode.None){
			text.text = k.ToString();
			text2.text = k.ToString();
			setControls(side, k);
		}
		else{
			text.text = origKey.ToString();
			text2.text = origKey.ToString();
		}
		
		button.interactable = true;
	}
	
	public void adjustCameraDistance(){
		cameraDistance = Mathf.Pow(cameraSlider.value, 1f);
		gc.cameraController.setCameraDistance(cameraDistance);
	}
	
	public void saveSettings(){
		PlayerPrefs.SetInt("Controls Left", (int)controlsLeft);
		PlayerPrefs.SetInt("Controls Right", (int)controlsRight);
		PlayerPrefs.SetFloat("Camera Distance", cameraDistance);
	}
}
