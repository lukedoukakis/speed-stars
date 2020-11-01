using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
	
	public RaceManager raceManager;
	// -----------------
	public RectTransform countdownTransform;
	float targetY;
	float y;
	// -----------------
	string[] strings;
	public Text countdownText0;
	public Text countdownText1;
	// -----------------
	public bool isRunning;
	public bool finished;
	// -----------------
	public Text falseStartText0;
	public Text falseStartText1;
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
		
		y = 600f;
		targetY = 0f;
		state = MARKS;
		countdownText0.text = strings[0];
		countdownText1.text = strings[0];
		yield return new WaitForSeconds(1.5f);
		targetY = -600f;
		yield return new WaitForSeconds(1.5f);
		
		y = 600f;
		targetY = 0f;
		state = SET;
		countdownText0.text = strings[1];
		countdownText1.text = strings[1];
		yield return new WaitForSeconds(2f);
		
		state = GO;
		
		finished = true;
		isRunning = false;
	}
	
	IEnumerator moveText(){
		
		while(!finished){
			y = Mathf.Lerp(y, targetY, 7f*Time.deltaTime);
			countdownTransform.anchoredPosition = new Vector3(countdownTransform.position.x, y);
			
			yield return null;
		}
	}
	
	public void showFalseStartText(){
		StartCoroutine("falseStartText");
	}
	
	IEnumerator falseStartText(){
		
		
		falseStartText0.gameObject.SetActive(true);
		falseStartText1.gameObject.SetActive(true);
		Color c = new Color32(255, 65, 0, 0);
		falseStartText0.color = c;
		
		while(c.a < 1f){
			c.a += 15f*Time.deltaTime/Time.timeScale;
			falseStartText1.color = c;
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		while(c.a > 0f){
			c.a -= 1.75f*Time.deltaTime/Time.timeScale;
			falseStartText1.color = c;
			yield return null;
		}
		c.a = 0f;
		falseStartText1.color = c;
		
		
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
