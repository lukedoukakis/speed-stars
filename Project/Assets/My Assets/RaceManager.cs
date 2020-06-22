using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
	
	public static int LIVE_MODE = 1;
	public static int REPLAY_MODE = 2;
	// -----------------
	public GlobalController gc;
	public Countdowner countdowner;
	public Text resultsText;
	// -----------------
	public int raceMode;
	public int raceStatus;
		public static int STATUS_MARKS = 1;
		public static int STATUS_SET = 2;
		public static int STATUS_GO = 3;
		public static int STATUS_FINISHED = 4;
	public bool fStart;
	public bool raceStarted;
	public float raceTime;
	public int raceTick;
	public bool focusRacerFinished;
	public bool allRacersFinished;
	public bool playerPB;
	// -----------------
	public GameObject player_backEnd;
	public GameObject player;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers;
	List<GameObject> bots;
	List<GameObject> ghosts;
	public GameObject RacersBackEndParent;
	public GameObject RacersFieldParent;
	public GameObject focusRacer;
	// -----------------
	public GameObject startingLine;
	// -----------------
	public int racerCount;
	public int playerCount;
	public int botCount;
	public int ghostCount;
	public int racersFinished;
	// -----------------
	float botDifficulty;
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
		
		if(raceStatus == STATUS_GO){
			raceTime += 1f * Time.deltaTime;
		}
		
	}

    void FixedUpdate()
    {
		if(!allRacersFinished){
			setRacerModes(racers, raceStatus);
			if(raceStatus == STATUS_SET || raceStatus == STATUS_GO){
				manageRace();
			}
			if(raceStatus == STATUS_GO){
				raceTick++;
			}
		}
    }
	
	
	public List<GameObject> setupRace(List<GameObject> backEndRacers, int mode){
		resultsText.text = "100m Dash Finals\n==========================\nWorld Record: 9.58 U. Bolt\nLocal Record: x.xx N. Name\n==========================\n\nFinals";
		// -----------------
		racers_backEnd = backEndRacers;
		racers = new List<GameObject>();
		bots = new List<GameObject>();
		ghosts = new List<GameObject>();
		// -----------------
		raceMode = mode;
		fStart = false;
		raceStarted = false;
		raceTime = 0f;
		raceTick = 0;
		focusRacerFinished = false;
		allRacersFinished = false;
		playerPB = false;
		// -----------------
		racerCount = 0;
		playerCount = 0;
		botCount = 0;
		ghostCount = 0;
		racersFinished = 0;
		// -----------------
		GameObject racer;
		if(raceMode == LIVE_MODE){
			for(int i = 0; i < racers_backEnd.Count; i++){
				racer = racers_backEnd[i];
				if(racer.tag == "Player (Back End)"){
					player_backEnd = racer;
					botDifficulty = calculateDifficulty(racer.GetComponent<PlayerAttributes>().personalBest);
					break;
				}
			}
		}
		else if(raceMode == REPLAY_MODE){
			
		}
		// -----------------
		for(int i = 0; i < racers_backEnd.Count; i++){
			racers_backEnd[i].SetActive(false);
			racer = Instantiate(racers_backEnd[i]);
			racer.tag = (racers_backEnd[i].tag).Split(' ')[0];
			Debug.Log("raceManager: racer tag is: " + racer.tag);
			racer.transform.SetParent(RacersFieldParent.transform);
			racer.SetActive(true);
			// --
			rb = racer.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			// --
			att = racer.GetComponent<PlayerAttributes>();
			att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
			att.setAttributesFromOther(racers_backEnd[i]);
			att.isRacing = false;
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
				racer.GetComponent<PlayerAnimationV2>().setPositionAndVelocity(raceTick);
				ghosts.Add(racer);
				ghostCount++;
			}
			racers.Add(racer);
		}
		// -----------------
		if(raceMode == LIVE_MODE){
			focusRacer = player;
		}
		else if(raceMode == REPLAY_MODE){
			focusRacer = racers[gc.playerIndex];
		}
		// -----------------
		assignLanes(racers);
		raceStatus = STATUS_MARKS;
		
		//Debug.Log("player pb: " + player.GetComponent<PlayerAttributes>().personalBest);
		
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
		if(raceMode == LIVE_MODE){
			PlayerAttributes att;
			for(int i = 0; i < racers.Count; i++){
				att = racers[i].GetComponent<PlayerAttributes>();
				if(att.personalBest == -1f){
					att.personalBest = float.MaxValue;
				}
			}
			List<int> lanes = new List<int>(){ 5, 4, 6, 3, 7, 2, 8, 1 };
			racers.Sort((x,y) => x.GetComponent<PlayerAttributes>().personalBest.CompareTo(y.GetComponent<PlayerAttributes>().personalBest));
			GameObject racer;
			for(int i = 0; i < racers.Count; i++){
				racer = racers[i];
				att = racers[i].GetComponent<PlayerAttributes>();
				att.lane = lanes[i];
				racer.transform.position = new Vector3((startingLine.transform.position.x - 4.4f) + (racer.GetComponent<PlayerAttributes>().lane-1)*1.252f, startingLine.transform.position.y + 1f, startingLine.transform.position.z - .13f);
				if(att.personalBest == float.MaxValue){
					att.personalBest = -1f;
				}
			}
		}
		else if(raceMode == REPLAY_MODE){
			GameObject racer;
			for(int i = 0; i < racers.Count; i++){
				racer = racers[i];
				racer.GetComponent<PlayerAttributes>().lane = racers_backEnd[i].GetComponent<PlayerAttributes>().lane;
				racer.transform.position = new Vector3((startingLine.transform.position.x - 4.4f) + (racer.GetComponent<PlayerAttributes>().lane-1)*1.252f, startingLine.transform.position.y + 1f, startingLine.transform.position.z - .13f);
			}
		}
	}
	
	public void setOffRacers(){
		if(!fStart){
			raceStatus = STATUS_GO;
			raceStarted = true;
		}
	}
	
	
	public void manageRace(){
		bool allRacersFinished = racersFinished >= racerCount;
		// -----------------
		if(focusRacerFinished){
			gc.showResultsScreen();
			focusRacerFinished = false;
		}
		if(allRacersFinished){
			raceStatus = STATUS_FINISHED;
			gc.endRace();
		}
		else{
			if(raceMode == LIVE_MODE){
				anim = player.GetComponent<PlayerAnimationV2>();
				anim.readInput(raceTick);
				anim.applyInput(raceTick);
				player.GetComponent<TimerController>().recordInput(raceTick);
			}
			// -----------------
			GameObject bot;
			for(int i = 0; i < bots.Count; i++){
				bot = bots[i];
				anim = bot.GetComponent<PlayerAnimationV2>();
				bot.GetComponent<Bot_AI>().runAI(raceTick);
				anim.readInput(raceTick);
				anim.applyInput(raceTick);
				bot.GetComponent<TimerController>().recordInput(raceTick);
			}
			// -----------------
			GameObject ghost;
			for(int i = 0; i < ghosts.Count; i++){
				ghost = ghosts[i];
				anim = ghost.GetComponent<PlayerAnimationV2>();
				anim.readInput(raceTick);
				anim.applyInput(raceTick);
				ghost.GetComponent<PlayerAnimationV2>().setPositionAndVelocity(raceTick);
			}
		}
	}
	
	public void addFinisher(GameObject racer){
		racersFinished++;
		att = racer.GetComponent<PlayerAttributes>();
		// -----------------
		if(racer == focusRacer){
			focusRacerFinished = true;
			if(raceMode == LIVE_MODE){
				if((raceTime < att.personalBest) || (att.personalBest == -1f)){
					att.resultTag = "<color=red>PB</color>";
					att.personalBest = raceTime;
					player.GetComponent<PlayerAttributes>().personalBest = raceTime;
					player_backEnd.GetComponent<PlayerAttributes>().personalBest = raceTime;
					playerPB = true;
				}
			}	
		}
		// -----------------
		att.finishTime = raceTime;
		att.pathLength = raceTick;
		att.resultString = "<color="+ att.resultColor+">" + att.racerName + "</color>" + "  " + (Mathf.Round(att.finishTime * 100f) / 100f) + "  " + att.resultTag;
		resultsText.text += "\n  " + (racersFinished) + "  " + att.resultString;
		
	}
	
	public IEnumerator falseStart(){
		fStart = true;
		float time = 0f;
		while(time < 1f){
			time += Time.deltaTime;
			if(time >= .25f){
				gc.setCameraFocus(startingLine, CameraController.CAMERA_MODE_SIDESCROLL);
				Time.timeScale = .15f;
			}
			yield return null;
		}
		Time.timeScale = 1f;
		gc.startRace(LIVE_MODE);
		
	}
	
	public void falseStartBotsReaction(){
		for(int i = 0; i < bots.Count; i++){
			GameObject bot = bots[i];
			int rand = Random.Range(0,10);
			//if(rand == 1){
				bot.GetComponent<PlayerAttributes>().isRacing = true;
			//}
			//if(bot.GetComponent<PlayerAttributes>().isRacing){
				bot.GetComponent<Bot_AI>().runAI(raceTick+1);
			//}
		}
		raceTick++;
	}
	
	public void setRacerModes(List<GameObject> racers, int mode){
		GameObject racer;
		for(int i = 0; i < racers.Count; i++){
			racer = racers[i];
			if(mode == STATUS_MARKS){
				racer.GetComponent<PlayerAnimationV2>().upInSet = false;
			}
			else if(mode == STATUS_SET){
				racer.GetComponent<PlayerAnimationV2>().upInSet = true;
			}
			else if(mode == STATUS_GO){
				racer.GetComponent<PlayerAttributes>().isRacing = true;
			}	
		}
	}
	


	
	
	
}