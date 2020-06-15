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
	public float raceTime;
	public int raceTick;
	public bool raceFinished;
	public bool playerPB;
	// -----------------
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers;
	List<GameObject> bots;
	List<GameObject> ghosts;
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
	float botDifficulty;
	// -----------------
	public GameObject player_backEnd;
	public GameObject player;
	GameObject racer;
	// -----------------
	PlayerAttributes att;
	PlayerAnimationV2 anim;
	Rigidbody rb;
	TimerController tc;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
	void Update(){
		if(raceActive){
			raceTime += 1f * Time.deltaTime;
		}
		
	}

    void FixedUpdate()
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
		ghosts = new List<GameObject>();
		// -----------------
		raceActive = false;
		raceTime = 0f;
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
			racer = racers_backEnd[i];
			if(racer.tag == "Player (Back End)"){
				player_backEnd = racer;
				botDifficulty = calculateDifficulty(racer.GetComponent<PlayerAttributes>().personalBest);
				break;
			}
		}
		for(int i = 0; i < racers_backEnd.Count; i++){
			racers_backEnd[i].SetActive(false);
			racer = Instantiate(racers_backEnd[i]);
			racer.tag = (racers_backEnd[i].tag).Split(' ')[0];
			racer.transform.SetParent(RacersFieldParent.transform);
			racer.SetActive(true);
			// --
			rb = racer.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			// --
			att = racer.GetComponent<PlayerAttributes>();
			att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
			att.setAttributesFromOther(racers_backEnd[i]);
			// --
			anim = racer.GetComponent<PlayerAnimationV2>();
			anim.globalController = gc;
			anim.raceManager = this;
			anim.mode = 1;
			// --
			tc = racer.GetComponent<TimerController>();
			tc.attributes = att;
			tc.animation = anim;
			// -----------------
			racerCount++;
			if(racer.tag == "Player"){
				player = racer;
				playerCount++;
			}
			else if(racer.tag == "Bot"){
				Bot_AI ai = racer.GetComponent<Bot_AI>();
				ai.init(botDifficulty);
				ai.raceManager = this;
				bots.Add(racer);
				botCount++;
			}
			else if(racer.tag == "Ghost"){
				ghosts.Add(racer);
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
		
		Debug.Log("player pb: " + player.GetComponent<PlayerAttributes>().personalBest);
		
		return racers;
		
	}
	
	float calculateDifficulty(float time){
		//Debug.Log("playerPB: " + time);
		float difficulty = 10f/time;
		if(difficulty > 0f){
			if(difficulty > .7f){
				if(difficulty > 1f){
					difficulty = 1f;
				}
			}
			else{
				difficulty = .7f;
			}
		}
		else{
			difficulty = Random.Range(.7f, 1f);
		}
		return difficulty;
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
	}
	
	
	public void manageRace(){
		bool b = racersFinished >= racerCount;
		// -----------------
		if(b){
			raceActive = false;
			raceFinished = true;
			gc.endRace();
		}
		else{
			player.GetComponent<TimerController>().recordInput(raceTick);
			// -----------------
			for(int i = 0; i < bots.Count; i++){
				GameObject bot = bots[i];
				bot.GetComponent<Bot_AI>().runAI(raceTick+1);
			}
			// -----------------
			raceTick++;
		}
	}
	
	public void addFinisher(GameObject racer){
		racersFinished++;
		att = racer.GetComponent<PlayerAttributes>();
		// -----------------
		if(racer.tag == "Player"){
			playersFinished++;
			if((raceTime < att.personalBest) || (att.personalBest == -1f)){
				att.resultTag = "<color=red>PB</color>";
				att.personalBest = raceTime;
				player.GetComponent<PlayerAttributes>().personalBest = raceTime;
				player_backEnd.GetComponent<PlayerAttributes>().personalBest = raceTime;
				playerPB = true;
			}
		}
		// -----------------
		att.finishTime = raceTime;
		att.pathLength = raceTick;
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
				racer.GetComponent<PlayerAttributes>().isRacing = true;
			}
				
		}
	}
	

	
	
	
}