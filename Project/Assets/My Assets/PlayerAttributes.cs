using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
	
	public static int DEFAULT_PATH_LENGTH = 10000;
	
	//info
	public string id;
	public string racerName;
	public float personalBest;
	
	
	// race info
	public int lane;
	public bool isRacing;
	public float finishTime;
	public string resultString;
	public string resultTag;
	public string resultColor;

	
	// ghost
	public int pathLength;
	public float[] velPathY;
	public float[] velPathZ;
	public float[] posPathY;
	public float[] posPathZ;
	public int[] rightInputPath;
	public int[] leftInputPath;
	
	
	// physics stats
	public float POWER_BASE;											
	public float TRANSITION_PIVOT_SPEED;			// base 20
	public float QUICKNESS_BASE;					// base 1
	public float STRENGTH_BASE;						// base 1
	public float BOUNCE_BASE;						// base 1
	public float ENDURANCE_BASE;
	public float ZTILT_MIN;							// [-45,45]
	public float ZTILT_MAX;							// [-45,45]
	public float HORIZ_BONUS;						// [.45, .65]
	public float TURNOVER;							// base 1
	public float TILT_SPEED;						// base 1
	
	
	public void randomizeStats(){
		// -----------------
		int randIndex;
		List<float> statModifiers = new List<float>();
		// -----------------
		float element = -.075f;
		for(int i = 0; i < 2; i++){
			statModifiers.Add(element);
			element += .15f;
		}
		// -----------------
		/*
		randIndex = Random.Range(0, statModifiers.Count);
		TRANSITION_PIVOT_SPEED += statModifiers[randIndex] * 15f;
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = Random.Range(0, statModifiers.Count);
		QUICKNESS_BASE += statModifiers[randIndex] * .5f;
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = Random.Range(0, statModifiers.Count);
		STRENGTH_BASE += statModifiers[randIndex];
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = Random.Range(0, statModifiers.Count);
		BOUNCE_BASE += statModifiers[randIndex];
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = r.Next(statModifiers.Count);
		ZTILT_MIN += statModifiers[randIndex] * 30f;
		statModifiers.RemoveAt(randIndex);
		
		
		randIndex = r.Next(statModifiers.Count);
		ZTILT_MAX += statModifiers[randIndex] * 30f;
		statModifiers.RemoveAt(randIndex);
		
		randIndex = r.Next(statModifiers.Count);
		HORIZ_BONUS += statModifiers[randIndex] * .2f;
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = Random.Range(0, statModifiers.Count);
		TURNOVER += statModifiers[randIndex];
		statModifiers.RemoveAt(randIndex);
		*/
		
		/*
		randIndex = r.Next(statModifiers.Count);
		TILT_SPEED += statModifiers[randIndex];
		statModifiers.RemoveAt(randIndex);
		*/
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
		
		//Debug.Log("------------START-----------------");
		// -----------------
		//racerName = "DefaultName";
		resultTag = "";
		if(tag.StartsWith("Player")){
			resultColor = "green";
		}
		else if(tag.StartsWith("Ghost")){
			resultColor = "grey";
		}
		else if(tag.StartsWith("Bot")){
			resultColor = "blue";
		}
		// -----------------

    }

    // Update is called once per frame
    void Update()
    {
	
    }
	
	
	public void setAttributesFromOther(GameObject other){
		PlayerAttributes otherAttributes = other.GetComponent<PlayerAttributes>();
		// -----------------
		id = otherAttributes.id;
		racerName = otherAttributes.racerName;
		personalBest = otherAttributes.personalBest;
		resultString = otherAttributes.resultString;
		// -----------------
		POWER_BASE = otherAttributes.POWER_BASE;
		TRANSITION_PIVOT_SPEED = otherAttributes.TRANSITION_PIVOT_SPEED;
		QUICKNESS_BASE = otherAttributes.QUICKNESS_BASE;
		STRENGTH_BASE = otherAttributes.STRENGTH_BASE;
		BOUNCE_BASE = otherAttributes.BOUNCE_BASE;
		ENDURANCE_BASE = otherAttributes.ENDURANCE_BASE;
		ZTILT_MIN = otherAttributes.ZTILT_MIN;
		ZTILT_MAX = otherAttributes.ZTILT_MAX;
		HORIZ_BONUS = otherAttributes.HORIZ_BONUS;
		TURNOVER = otherAttributes.TURNOVER;
		TILT_SPEED = otherAttributes.TILT_SPEED;
		// -----------------
		pathLength = otherAttributes.pathLength;
		velPathY = otherAttributes.velPathY;
		velPathZ = otherAttributes.velPathZ;
		posPathZ = otherAttributes.posPathZ;
		posPathY = otherAttributes.posPathY;
		rightInputPath = otherAttributes.rightInputPath;
		leftInputPath = otherAttributes.leftInputPath;
	}
	
	/*
	public void init(){
		// -----------------
		resultTag = "";
		if(tag.StartsWith("Player")){
			resultColor = "green";
		}
		else if(tag.StartsWith("Ghost")){
			resultColor = "grey";
		}
		else if(tag.StartsWith("Bot")){
			resultColor = "blue";
		}
		// -----------------
		POWER_BASE = 2300f;
		TRANSITION_PIVOT_SPEED = 35f;
		QUICKNESS_BASE = 1f;
		STRENGTH_BASE = 1f;
		BOUNCE_BASE = 1f;
		ZTILT_MIN = -45f;
		ZTILT_MAX = 45f;
		HORIZ_BONUS = .55f;
		TURNOVER = 1f;
		TILT_SPEED = 1f;
		// -----------------
		setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
	}
	*/
	
	// format: "blank", "bot"
	public void setPaths(int length){
		velPathY = new float[length];
		velPathZ = new float[length];
		posPathZ = new float[length];
		posPathY = new float[length];
		rightInputPath = new int[length];
		leftInputPath = new int[length];
	}
	
	
	public static string generateID(string racerName){
		string id = racerName + "_";
		
		int digit;
		for(int i = 0; i < 9; i++){
			digit = Random.Range(0,10);
			id += digit.ToString();
		}
		return id;
		
	}
	
	
	
}
