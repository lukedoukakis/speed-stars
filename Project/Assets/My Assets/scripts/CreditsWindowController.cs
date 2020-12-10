using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindowController : MonoBehaviour
{
	
	public GameObject CreditsWindow;
    
	public void show(){
		CreditsWindow.SetActive(true);
	}
	public void hide(){
		CreditsWindow.SetActive(false);
	}
}
