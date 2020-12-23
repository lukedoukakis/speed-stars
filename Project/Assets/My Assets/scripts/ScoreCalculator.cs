﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreCalculator : MonoBehaviour
{
	
	
	public static int calculateScore(float[] times){
		
		float[] referenceTimes = new float[]{10.7f,17f,35.5f,79f};
		float[] conversionFactors = new float[]{68.6f,24.63f,5.08f,1.021f};
		
		int pts = 0;
		for(int i = 0; i < times.Length; i++){
			if(times[i] != -1f){
				pts += (int)(conversionFactors[i] * Mathf.Pow((referenceTimes[i] - times[i]), 2f));
			}
		}
		return pts;
	}
	
	public static int calculateScore_user(){
		float[] times = new float[]{
			GlobalController.getUserPB(RaceManager.RACE_EVENT_60M),
			GlobalController.getUserPB(RaceManager.RACE_EVENT_100M),
			GlobalController.getUserPB(RaceManager.RACE_EVENT_200M),
			GlobalController.getUserPB(RaceManager.RACE_EVENT_400M)
		};
		
		return calculateScore(times);
	}
	
}