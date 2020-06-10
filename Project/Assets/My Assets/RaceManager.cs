using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
	
	public GlobalController gc;
	public Countdowner countdowner;
	public Text resultsText;
	// -----------------
	public bool raceActive;
	public float time;
	public int raceTick;
	public bool raceFinished;
	public bool playerPB;
	// -----------------
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers;
	List<GameObject> bots;
	public GameObject RacersBackEndParent;
	public GameObject RacersFieldParent;
	// -----------------
	public GameObject startingLine;
	// -----------------
	public int racerCount;
	public int playerCount;
	public int botCount;
	public int ghostCount;
	public int racersFinished;
	public int playersFinished;
	// -----------------
	public GameObject player;
	GameObject racer;
	PlayerAttributes att;
	PlayerAnimationV2 anim;
	Rigidbody rb;
	TimerController tc;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(raceActive){
			manageRace();
		}
    }
	
	
	public List<GameObject> initRace(List<GameObject> backEndRacers){
		resultsText.text = "100m Dash Finals\n==========================\nWorld Record: 9.58 U. Bolt\nLocal Record: x.xx N. Name\n==========================\n\nFinals";
		// -----------------
		racers_backEnd = backEndRacers;
		racers = new List<GameObject>();
		bots = new List<GameObject>();
		// -----------------
		raceActive = false;
		time = 0f;
		raceTick = 0;
		raceFinished = false;
		playerPB = false;
		// -----------------
		racerCount = 0;
		playerCount = 0;
		botCount = 0;
		ghostCount = 0;
		racersFinished = 0;
		playersFinished = 0;
		// -----------------
		for(int i = 0; i < racers_backEnd.Count; i++){
			racers_backEnd[i].SetActive(false);
			racer = Instantiate(racers_backEnd[i]);
			racer.transform.SetParent(RacersFieldParent.transform);
			att = racer.GetComponent<PlayerAttributes>();
			anim = racer.GetComponent<PlayerAnimationV2>();
			rb = racer.GetComponent<Rigidbody>();
			tc = racer.GetComponent<TimerController>();
			// --
			racer.tag = (racers_backEnd[i].tag).Split(' ')[0];
			if(racer.tag == "Player" || racer.tag == "Ghost"){
				att.setPaths(PlayerAttributes.BLANK_FORMAT, racers_backEnd[i].GetComponent<PlayerAttributes>().pathLength);
			}
			else if(racer.tag == "Bot"){
				att.setPaths(PlayerAttributes.BOT_FORMAT, PlayerAttributes.DEFAULT_PATH_LENGTH);
				racer.GetComponent<Bot_AI>().init();
			}
			// --
			att.setAttributesFromOther(racers_backEnd[i]);
			tc.init();
			tc.raceManager = this;
			anim.globalController = gc;
			anim.mode = 1;
			rb.velocity = Vector3.zero;
			racer.SetActive(true);
			// -----------------
			racerCount++;
			if(racer.tag == "Player"){
				player = racer;
				playerCount++;
			}
			else if(racer.tag == "Bot"){
				bots.Add(racer);
				botCount++;
			}
			else if(racer.tag == "Ghost"){
				ghostCount++;
			}
			racers.Add(racer);
			
			
			if(playerCount < 1){
				player = racers[0];
			}
			// -----------------
		}
		assignLanes(racers);
		setRacerModes(racers, "Marks");
		// -----------------
		
		Debug.Log("racer count this race: " + racers.Count);
		
		return racers;
		
	}
	
	void assignLanes(List<GameObject> racers){
		List<int> lanes = new List<int>(){ 5, 4, 6, 3, 7, 2, 8, 1 };
		racers.Sort((x,y) =>
			x.GetComponent<PlayerAttributes>().personalBest.CompareTo(y.GetComponent<PlayerAttributes>().personalBest));
		for(int i = 0; i < racers.Count; i++){
			GameObject racer = racers[i];
			racer.GetComponent<PlayerAttributes>().lane = lanes[i];
			racer.transform.position = new Vector3((startingLine.transform.position.x - 4.4f) + (racer.GetComponent<PlayerAttributes>().lane-1)*1.252f, startingLine.transform.position.y + 1f, startingLine.transform.position.z - .13f);
		}
		
		
	}
	
	public void setOffRacers(){
		raceActive = true;
		setRacerModes(racers, "Race");
		/*
		GameObject racer;
		for(int i = 0; i < racers.Count; i++){
			racer = racers[i];
			racer.GetComponent<TimerController>().start();
			racer.GetComponent<PlayerAttributes>().isRacing = true;
		}
		*/
	}
	
	
	public void manageRace(){
		for(int i = 0; i < bots.Count; i++){
			GameObject bot = bots[i];
			bot.GetComponent<Bot_AI>().runAI(raceTick);
			// -----------------
		}
		if(racersFinished >= racerCount){
			raceActive = false;
			raceFinished = true;
			gc.endRace();
		}
		raceTick++;
	}
	
	public void addFinisher(GameObject racer){
		racersFinished++;
		// -----------------
		att = racer.GetComponent<PlayerAttributes>();
		if(racer.tag == "Player"){
			playersFinished++;
			if((att.finishTime < att.personalBest) || (att.personalBest == -1f)){
				att.personalBest = att.finishTime;
				att.resultTag = "<color=red>PB</color>";
				playerPB = true;
			}
		}
		// -----------------
		att.resultString = "<color="+ att.resultColor+">" + att.racerName + "</color>" + "  " + (Mathf.Round(att.finishTime * 100f) / 100f) + "  " + att.resultTag;
		resultsText.text += "\n  " + (racersFinished) + "  " + att.resultString;
		
	}
	
	public void setRacerModes(List<GameObject> racers, string mode){
		GameObject racer;
		for(int i = 0; i < racers.Count; i++){
			racer = racers[i];
			if(mode == "Marks"){
				racer.GetComponent<PlayerAnimationV2>().upInSet = false;
			}
			else if(mode == "Set"){
				racer.GetComponent<PlayerAnimationV2>().upInSet = true;
			}
			else if(mode == "Race"){
				racer.GetComponent<TimerController>().start();
				racer.GetComponent<PlayerAttributes>().isRacing = true;
			}
				
		}
	}
	

	
	
	
}