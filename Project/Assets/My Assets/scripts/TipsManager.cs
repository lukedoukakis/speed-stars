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

	public void showTips_400m()
	{
		headerText.text = "Tip:";
		bodyText0.text = "The green bar at the bottom of the screen shows your energy. If you get tired too fast, try pacing yourself!";
		bodyText1.text = "";
		controlsButtonL.gameObject.SetActive(false);
		controlsButtonR.gameObject.SetActive(false);
		GlobalController.hasFirstRace_400m = true;
		PlayerPrefs.SetInt("hasFirstRace_400m", 1);
		timeScale = Time.timeScale;
		show();
	}

	public void showTips_leaderboard()
    {
		headerText.text = "<color=#000561>Tip:</color>";
		bodyText0.text = "Click on any score to get that player's ghost. Then you can race against it by selecting it when you start a race!";
		bodyText1.text = "";
		controlsButtonL.gameObject.SetActive(false);
		controlsButtonR.gameObject.SetActive(false);
		GlobalController.hasFirstLeaderboard = true;
		PlayerPrefs.SetInt("hasFirstLeaderboard", 1);
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
	

	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
