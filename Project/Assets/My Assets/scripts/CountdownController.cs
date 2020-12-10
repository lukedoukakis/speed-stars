using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownController : MonoBehaviour
{
	
	public RaceManager raceManager;
	// -----------------
	public Transform countdownTransform;
	float targetY;
	float y;
	// -----------------
	string[] strings;
	public TextMeshProUGUI countdownText0;
	public TextMeshProUGUI countdownText1;
	// -----------------
	public bool isRunning;
	public bool finished;
	// -----------------
	public TextMeshProUGUI falseStartText0;
	public TextMeshProUGUI falseStartText1;
	// -----------------
	public int state;
		public static int MARKS = 0;
		public static int SET = 1;
		public static int GO = 2;
	// -----------------
	

    public void startCountdown(){
		isRunning = true;
		strings = new string[2] { "On your marks...", "Set"};

		StartCoroutine("countdown");
		StartCoroutine("moveText");
	}
	
	public void cancelCountdown(){
		StopCoroutine("countdown");
		StopCoroutine("moveText");
	}
	
	IEnumerator countdown(){
		
		finished = false;
		
		y = 340f;
		targetY = 0f;
		state = MARKS;
		countdownText0.fontSize = 42f;
		countdownText1.fontSize = 42f;
		countdownText0.text = strings[0];
		countdownText1.text = strings[0];
		yield return new WaitForSeconds(1.5f);
		targetY = -600f;
		yield return new WaitForSeconds(1.5f);
		
		while(raceManager.gc.tipsManager.isShowing){
			yield return null;
		}
		
		y = 340f;
		targetY = 0f;
		state = SET;
		countdownText0.fontSize = 60f;
		countdownText1.fontSize = 60f;
		countdownText0.text = strings[1];
		countdownText1.text = strings[1];
		yield return new WaitForSeconds(2f);
		
		state = GO;
		
		finished = true;
		isRunning = false;
	}
	
	IEnumerator moveText(){
		
		while(!finished){
			y = Mathf.Lerp(y, targetY, 8f*Time.deltaTime);
			countdownTransform.position = new Vector3(countdownTransform.position.x, y, countdownTransform.position.z);
			yield return null;
		}
	}
	
	public void showCountdownText(){
		Color c0 = countdownText0.color;
		Color c1 = countdownText1.color;
		c0.a = c1.a = 1f;
		countdownText0.color = c0;
		countdownText1.color = c1;
	}
	public void hideCountdownText(){
		Color c0 = countdownText0.color;
		Color c1 = countdownText1.color;
		c0.a = c1.a = 0f;
		countdownText0.color = c0;
		countdownText1.color = c1;
	}
	
	public void showFalseStartText(){
		StartCoroutine("falseStartText");
	}
	
	IEnumerator falseStartText(){
		
		
		falseStartText0.gameObject.SetActive(true);
		falseStartText1.gameObject.SetActive(true);
		Color c1 = new Color32(255, 65, 0, 0);
		Color c0 = new Color32(0, 0, 0, 0);
		falseStartText0.color = c0;
		falseStartText1.color = c1;
		
		while(c1.a < 1f){
			c1.a += 15f*Time.deltaTime/Time.timeScale;
			c0.a += 15f*Time.deltaTime/Time.timeScale;
			falseStartText1.color = c1;
			falseStartText0.color = c0;
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		while(c1.a > 0f){
			c1.a -= 1.75f*Time.deltaTime/Time.timeScale;
			c0.a -= 1.75f*Time.deltaTime/Time.timeScale;
			falseStartText1.color = c1;
			falseStartText0.color = c0;
			yield return null;
		}
		c1.a = 0f;
		c0.a = 0f;
		falseStartText1.color = c1;
		falseStartText0.color = c0;
		
		
		falseStartText0.gameObject.SetActive(false);
		falseStartText1.gameObject.SetActive(false);
		
		
		yield return null;
	}
	
	
	public void hideFalseStartText(){
		falseStartText0.gameObject.SetActive(false);
		falseStartText1.gameObject.SetActive(false);
	}
		
    void FixedUpdate()
    {
		
    }
}
