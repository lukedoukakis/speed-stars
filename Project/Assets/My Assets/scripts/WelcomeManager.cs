using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeManager : MonoBehaviour
{
	
	public GameObject WelcomeWindow;
	
   
   public void show(){
	   WelcomeWindow.SetActive(true);
   }
   public void hide(){
	   WelcomeWindow.SetActive(false);
   }
}
