using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdowner : MonoBehaviour
{
	
	float countdownTime;
	string[] countdownStrings = new string[3] { "On Your Marks", "Set", ""};
	int countdownIndex;
	bool isCounting;
	public string currentString;
	public bool finished;
	
	
	public void reset(){
		finished = false;
	}
	
	
    public void startCountdown(){
		countdownTime = 3f;
		countdownIndex = 0;
		isCounting = true;
		finished = false;
	}

    void FixedUpdate()
    {
		if(isCounting){
			currentString = countdownStrings[countdownIndex];
			if(countdownIndex > 1){
				finished = true;
				isCounting = false;
			}
			countdownTime -= 1f * Time.deltaTime;
			if(countdownTime <= 0f){
				countdownTime = Random.Range(2f, 3f);
				countdownIndex++;
			}
		}
		else{
			currentString = "";
		}
    }
}
