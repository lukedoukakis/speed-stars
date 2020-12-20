using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
	
	public GameObject TipsWindow;
	public Text headerText;
	public Text bodyText0;
	public Text bodyText1;
	public Button controlsButtonL;
	public Button controlsButtonR;
	
	
	public bool isShowing;
	float timeScale;
	
	public void showTips_firstRace(){
		GlobalController.hasFirstRace = true;
		PlayerPrefs.SetInt("hasFirstRace", 1);
		timeScale = Time.timeScale;
		show();
	}
	
	public void show(){
		TipsWindow.SetActive(true);
		isShowing = true;
	}
	public void hide(){
		TipsWindow.SetActive(false);
		isShowing = false;
	}
	
	public void showTips_400m(){
		headerText.text = "Tip:";
		bodyText0.text = "Keep an eye on the green energy meter at the bottom of the screen. If you get tired too fast, try pacing yourself!";
		bodyText1.text = "";
		controlsButtonL.gameObject.SetActive(false);
		controlsButtonR.gameObject.SetActive(false);
		GlobalController.hasFirstRace_400m = true;
		PlayerPrefs.SetInt("hasFirstRace_400m", 1);
		timeScale = Time.timeScale;
		show();
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
