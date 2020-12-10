using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBlipController : MonoBehaviour
{
	
	public GameObject blip;
	public Text text;
	
    
	public void show(){
		blip.SetActive(true);
	}
	public void hide(){
		blip.SetActive(false);
	}
	
	public void setText(string s){
		text.text = s;
	}
	
}
