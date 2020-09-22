using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
	
	public static int RACE_EVENT_60M = 0;
	public static int RACE_EVENT_100M = 1;
	public static int RACE_EVENT_200M = 2;
	public static int RACE_EVENT_400M = 3;
	// -----------------
	public static int VIEW_MODE_LIVE = 0;
	public static int VIEW_MODE_REPLAY = 1;
	// -----------------
	
	public GlobalController gc;
	public Countdowner countdowner;
	public Text resultsText;
	// -----------------
	public int raceEvent;
	public int viewMode;
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
	public GameObject track;
	public GameObject[] startingBlocks_100m;
	public GameObject[] startingBlocks_200m;
	public GameObject[] startingBlocks_400m;
	public GameObject[] startingBlocks_60m;
	public GameObject[] startingBlocks_current;
	public GameObject[] startingLines_100m;
	public GameObject[] startingLines_200m;
	public GameObject[] startingLines_400m;
	public GameObject[] startingLines_60m;
	public GameObject[] startingLines_current;
	
	public GameObject finishLine;
	public GameObject hurdlesSetPrefab;
	public GameObject hurdlesSet;
	public GameObject curve1_surface;
	public GameObject curve2_surface;
	public GameObject curve1_sphere;
	public GameObject curve2_sphere;
	// -----------------
	public Text speedometer;
	public Text clock;
		TimeSpan timeSpan;
		int minutes;
		int seconds;
		int milliseconds;
	public Image speedImage;
	// -----------------
	public int racerCount;
	public int playerCount;
	public int botCount;
	public int ghostCount;
	public int racersFinished;
	// -----------------
	float botDifficulty;
	// -----------------
	public float transparency_ghost_base;
	public float transparency_ghost_min;
	public float transparency_bot_base;
	public float transparency_bot_min;
	// -----------------
	PlayerAttributes att;
	PlayerAnimationV2 anim;
	Rigidbody rb;
	TimerController tc;
	OrientationController oc;
	EnergyMeterController emc;
	// -----------------
	PlayerAttributes playerAtt;
	PlayerAnimationV2 playerAnim;
	Rigidbody playerRb;
	TimerController playerTc;
	OrientationController playerOc;
	EnergyMeterController playerEmc;
	PlayerAttributes[] botAtts;
	PlayerAnimationV2[] botAnims;
	Rigidbody[] botRbs;
	TimerController[] botTcs;
	OrientationController[] botOcs;
	EnergyMeterController[] botEmcs;
	Bot_AI[] botAIs;
	PlayerAttributes[] ghostAtts;
	PlayerAnimationV2[] ghostAnims;
	Rigidbody[] ghostRbs;
	TimerController[] ghostTcs;
	OrientationController[] ghostOcs;
	EnergyMeterController[] ghostEmcs;
	// -----------------
	
    // Start is called before the first frame update
    void Start()
    {
        focusRacer = startingLines_100m[4];
    }
	
	
	void Update(){
		
		if(raceStatus == STATUS_GO){

		}
		updateSpeedometer();
		
		GameObject racer;
		Vector3 racerPos;
		
		/*
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			for(int i = 0; i < racers.Count; i++){
				racer = racers[i];
				if(racer.tag != "Player"){
					racerPos = racer.transform.position;
					float xDistance = racerPos.x - focusRacer.transform.position.x;
					if(xDistance > 0f){
						float trans_base = 0f;
						float trans_min = 0f;
						if(racer.tag == "Ghost"){
							trans_base = transparency_ghost_base;
							trans_min = transparency_ghost_min;
						}
						else if(racer.tag == "Bot"){
							trans_base = transparency_bot_base;
							trans_min = transparency_bot_min;
						}
						float a = trans_base;
						Vector3 cameraPos = gc.gameCamera.transform.position;
						Vector3 focusRacerPos = focusRacer.transform.position;
						float angle = Vector3.Angle(racerPos-cameraPos, focusRacerPos - cameraPos);
						if(angle < 20f){
							a = trans_base *  (angle/20f);
							if(a < trans_min){ a = trans_min; }
		
							setTransparency(racer, a);
						
						}
					}
					else{
						if(racer.tag == "Ghost"){
							setTransparency(racer, transparency_ghost_base);
						}
					}
				}
			}
		}
		*/	
	}

    void FixedUpdate()
    {
		if(!allRacersFinished){
			setRacerModes(racers, raceStatus);
			if(raceStatus == STATUS_SET || raceStatus == STATUS_GO){
				manageRace();
			}
			if(raceStatus == STATUS_GO){
				raceTime += 1f * Time.deltaTime;
				raceTick++;
			}
		}
    }
	
	
	public List<GameObject> setupRace(List<GameObject> backEndRacers, int raceEvent, int viewMode){
		resultsText.text = "100m Dash Finals\n==========================\nWorld Record: 9.58 U. Bolt\nLocal Record: x.xx N. Name\n==========================\n\nFinals";
		// -----------------
		racers_backEnd = backEndRacers;
		racers = new List<GameObject>();
		bots = new List<GameObject>();
		ghosts = new List<GameObject>();
		// -----------------
		this.raceEvent = raceEvent;
		this.viewMode = viewMode;
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
		// -----------------
		botAtts = new PlayerAttributes[8];
		botAnims = new PlayerAnimationV2[8];
		botRbs = new Rigidbody[8];
		botTcs = new TimerController[8];
		botOcs = new OrientationController[8];
		botEmcs = new EnergyMeterController[8];
		botAIs = new Bot_AI[8];
		ghostAtts = new PlayerAttributes[8];
		ghostAnims = new PlayerAnimationV2[8];
		ghostRbs = new Rigidbody[8];
		ghostTcs = new TimerController[8];
		ghostOcs = new OrientationController[8];
		ghostEmcs = new EnergyMeterController[8];
		// -----------------

		if(viewMode == VIEW_MODE_LIVE){
			// calculate bot difficulty and set player
			for(int i = 0; i < racers_backEnd.Count; i++){
				racer = racers_backEnd[i];
				if(racer.tag == "Player (Back End)"){
					player_backEnd = racer;
					botDifficulty = calculateDifficulty(raceEvent, racer.GetComponent<PlayerAttributes>().personalBests[raceEvent]);
					break;
				}
			}
		}
		else if(viewMode == VIEW_MODE_REPLAY){
			
		}
		// -- set up racers field
		for(int i = 0; i < racers_backEnd.Count; i++){
			racers_backEnd[i].SetActive(false);
			racer = Instantiate(racers_backEnd[i]);
			racer.tag = (racers_backEnd[i].tag).Split(' ')[0];
			//Debug.Log("raceManager: racer tag is: " + racer.tag);
			racer.transform.SetParent(RacersFieldParent.transform);
			racer.SetActive(true);
			// --
			rb = racer.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			// --
			att = racer.GetComponent<PlayerAttributes>();
			att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
			att.copyAttributesFromOther(racers_backEnd[i], "all");
			att.isRacing = false;
			// --
			anim = racer.GetComponent<PlayerAnimationV2>();
			anim.init();
			anim.globalController = gc;
			anim.raceManager = this;
			anim.mode = PlayerAnimationV2.Set;
			anim.energy = 100f;
			anim.launchFlag = false;
			anim.setViewMode(viewMode);
			// --
			tc = racer.GetComponent<TimerController>();
			tc.attributes = att;
			tc.animation = anim;
			tc.oc = anim.GetComponent<OrientationController>();
			// -----------------
			emc = racer.GetComponent<EnergyMeterController>();
			// -----------------
			racerCount++;
			if(racer.tag == "Player"){
				setPlayer(racer);
				playerAtt = att;
				playerAnim = anim;
				playerRb = rb;
				playerTc = tc;
				playerOc = oc;
				playerEmc = emc;
				playerCount++;
			}
			else if(racer.tag == "Bot"){
				Bot_AI ai = racer.GetComponent<Bot_AI>();
				ai.raceManager = this;
				ai.init(botDifficulty);
				anim.init();
				bots.Add(racer);
				botAtts[botCount] = att;
				botAnims[botCount] = anim;
				botRbs[botCount] = rb;
				botTcs[botCount] = tc;
				botOcs[botCount] = oc;
				botEmcs[botCount] = emc;
				botAIs[botCount] = ai;
				botCount++;
			}
			else if(racer.tag == "Ghost"){
				ghosts.Add(racer);
				ghostAtts[ghostCount] = att;
				ghostAnims[ghostCount] = anim;
				ghostRbs[ghostCount] = rb;
				ghostTcs[ghostCount] = tc;
				ghostOcs[ghostCount] = oc;
				ghostEmcs[ghostCount] = emc;
				ghostCount++;
			}
				
			racers.Add(racer);
		}
		// -----------------
		if(raceEvent == RACE_EVENT_100M){
			startingLines_current = startingLines_100m;
			startingBlocks_current = startingBlocks_100m;
		}
		else if(raceEvent == RACE_EVENT_200M){
			startingLines_current = startingLines_200m;
			startingBlocks_current = startingBlocks_200m;
		}
		else if(raceEvent == RACE_EVENT_400M){
			startingLines_current = startingLines_400m;
			startingBlocks_current = startingBlocks_400m;
		}
		else if(raceEvent == RACE_EVENT_60M){
			startingLines_current = startingLines_60m;
			startingBlocks_current = startingBlocks_60m;
		}
		if(viewMode == VIEW_MODE_LIVE){
			focusRacer = player;
		}
		else if(viewMode == VIEW_MODE_REPLAY){
			focusRacer = racers[gc.playerIndex];
		}
		// -----------------
		hideStartingBlocks();
		assignLanes(racers);
		setRenderQueues(racers);
		
		resetClock();
		raceStatus = STATUS_MARKS;

		
		return racers;
		
	}
	
	public void resetCounts(){
		racerCount = 0;
		playerCount = 0;
		botCount = 0;
		ghostCount = 0;
		racersFinished = 0;
	}
	
	float calculateDifficulty(int raceEvent, float time){
		//Debug.Log("playerPB: " + time);
		float eliteStandard = 0f;
		if(raceEvent == RACE_EVENT_100M){
			eliteStandard = 10f;
		}
		else if(raceEvent == RACE_EVENT_200M){
			eliteStandard = 20f;
		}
		else if(raceEvent == RACE_EVENT_400M){
			eliteStandard = 45f;
		}
		else if(raceEvent == RACE_EVENT_60M){
			eliteStandard = 6.5f;
		}
		
		float difficulty = eliteStandard/time;
		
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
			difficulty = .7f;
		}
		return difficulty;
	}
	
	void assignLanes(List<GameObject> racers){
		
		List<int> lanes = new List<int>(){ 5, 4, 6, 3, 7, 2, 8, 1 };
		if(viewMode == VIEW_MODE_LIVE){
			PlayerAttributes _att;
			for(int i = 0; i < racers.Count; i++){
				_att = racers[i].GetComponent<PlayerAttributes>();
				if(_att.personalBests[raceEvent] == -1f){
					_att.personalBests[raceEvent] = float.MaxValue;
				}
			}
			racers.Sort((x,y) => x.GetComponent<PlayerAttributes>().personalBests[raceEvent].CompareTo(y.GetComponent<PlayerAttributes>().personalBests[raceEvent]));
		}
		
		
		GameObject racer;
		PlayerAttributes att;
		PlayerAnimationV2 anim;
		OrientationController oc;
		int lane = 0;
		GameObject line;
		GameObject block;
		for(int i = 0; i < racers.Count; i++){
			
			racer = racers[i];
			att = racer.GetComponent<PlayerAttributes>();
			anim = racer.GetComponent<PlayerAnimationV2>();
			oc = racer.GetComponent<OrientationController>();
			
			if(viewMode == VIEW_MODE_LIVE){
				lane = lanes[i];
				if(att.personalBests[raceEvent] == float.MaxValue){
					att.personalBests[raceEvent] = -1f;
				}
			}
			else if(viewMode == VIEW_MODE_REPLAY){
				lane = racers_backEnd[i].GetComponent<PlayerAttributes>().lane;
			}
			
			
			att.lane = lane;
			line = startingLines_current[lane - 1];
			block = startingBlocks_current[lane - 1];
			
			racer.transform.position = line.transform.position + (line.transform.forward*-.13f) + Vector3.up;
			
			block.SetActive(true);
			Physics.IgnoreCollision(anim.rightFootScript.bc, block.GetComponent<BoxCollider>());
			Physics.IgnoreCollision(anim.leftFootScript.bc, block.GetComponent<BoxCollider>());
			block.transform.position = line.transform.position + (line.transform.forward*-1f) + (Vector3.up*.22f);
			block.transform.rotation = line.transform.rotation;
			block.GetComponent<Rigidbody>().velocity = Vector3.zero;
			block.GetComponent<StartingBlockController>().adjustPedals(att.legX);
			
				
			oc.sphere1 = curve1_sphere;
			oc.sphere2 = curve2_sphere;
			oc.sphere = oc.sphere1;
			if(raceEvent >= 2){
				oc.updateOrientation(false);
			}
			oc.initRotations();
			
			/*
			if(raceEvent == RACE_EVENT_100M){
				oc.sphere1 = null;
				oc.sphere2 = null;
				oc.sphere = null;
				oc.trackSegment = 4;
			}
			else if(raceEvent == RACE_EVENT_200M){
				oc.sphere1 = curve1_sphere;
				oc.sphere2 = curve2_sphere;
				oc.sphere = curve2_sphere;
				oc.trackSegment = 3;
				Vector3 spherePos = new Vector3(oc.sphere.transform.position.x, 0f, oc.sphere.transform.position.z);
				racerPos.y = 0f;
				oc.distance = Vector3.Distance(spherePos, racerPos);
			}
			else if(raceEvent == RACE_EVENT_400M){
				oc.sphere1 = curve1_sphere;
				oc.sphere2 = curve2_sphere;
				oc.sphere = curve1_sphere;
				oc.trackSegment = 1;
				Vector3 spherePos = new Vector3(oc.sphere.transform.position.x, 0f, oc.sphere.transform.position.z);
				racerPos.y = 0f;
				oc.distance = Vector3.Distance(spherePos, racerPos);
			}
			*/
		}
	}
	
	/*
	void resetStartingBlocks(){
		GameObject block;
		GameObject blockPedal;
		for(int i = 0; i < startingBlocks_current.Length; i++){
			block = startingBlocks[i];
			blockPedal = block.transform.Find("Block Pedal Right").gameObject;
			Vector3 targetPos = 
			
			.transform.position +=
		}
	}
	*/
	
	void hideStartingBlocks(){
		for(int i = 0; i < startingBlocks_current.Length; i++){
			startingBlocks_100m[i].SetActive(false);
			startingBlocks_200m[i].SetActive(false);
			startingBlocks_400m[i].SetActive(false);
			startingBlocks_60m[i].SetActive(false);
		}
	}
	
	void setRenderQueues(List<GameObject> racers){
		int lane;
		PlayerAttributes att;
		SkinnedMeshRenderer[] renderers;
		SkinnedMeshRenderer renderer;
		Material material;
		for(int i = 0; i < racers.Count; i++){
			att = racers[i].GetComponent<PlayerAttributes>();
			lane = att.lane;
			renderers = new SkinnedMeshRenderer[]{att.smr_dummy, att.smr_top, att.smr_bottoms, att.smr_shoes, att.smr_socks, att.smr_headband, att.smr_sleeve};
			for(int j = 0; j < renderers.Length; j++){
				renderer = renderers[j];
				material = renderer.sharedMaterial;
				material.renderQueue += (lane*100 + j);
				renderer.sharedMaterial = material;
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
			gc.endRace();
		}
		if(allRacersFinished){
			raceStatus = STATUS_FINISHED;
			//gc.endRace();
		}
		else{
			if(viewMode == VIEW_MODE_LIVE){
				playerAnim.readInput(raceTick);
				playerAnim.applyInput(raceTick);
				playerTc.recordInput(raceTick);
			}
			// -----------------
			GameObject bot;
			for(int i = 0; i < bots.Count; i++){
				botAIs[i].runAI(raceTick);
				botAnims[i].readInput(raceTick);
				botAnims[i].applyInput(raceTick);
				botTcs[i].recordInput(raceTick);
			}
			// -----------------
			GameObject ghost;
			for(int i = 0; i < ghosts.Count; i++){
				ghostAnims[i].readInput(raceTick);
				ghostAnims[i].applyInput(raceTick);
				if(raceTick > 0){
					ghostAnims[i].setPositionAndVelocity(raceTick);
				}
			}
		}
	}
	
	public void addFinisher(GameObject racer){
		racersFinished++;
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		PlayerAnimationV2 anim = racer.GetComponent<PlayerAnimationV2>();
		// -----------------
		if(racer == focusRacer){
			focusRacerFinished = true;
			if(viewMode == VIEW_MODE_LIVE){
				gc.setCameraFocus(finishLine, CameraController.CAMERA_MODE_SIDESCROLL);
				if((raceTime < att.personalBests[raceEvent]) || (att.personalBests[raceEvent] == -1f)){
					att.resultTag = "<color=red>PB</color>";
					att.personalBests[raceEvent] = raceTime;
					//player.GetComponent<PlayerAttributes>().personalBests[raceEvent] = raceTime;
					player_backEnd.GetComponent<PlayerAttributes>().personalBests[raceEvent] = raceTime;
					playerPB = true;
				}
			}
			else{
				gc.setCameraFocus(finishLine, CameraController.CAMERA_MODE_TV);
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
				gc.setCameraFocus(null, CameraController.CAMERA_MODE_SIDESCROLL);
				Time.timeScale = .15f;
			}
			yield return null;
		}
		Time.timeScale = 1f;
		gc.startRace(raceEvent, VIEW_MODE_LIVE);
		
	}
	
	public void falseStartBotsReaction(){
		for(int i = 0; i < bots.Count; i++){
			GameObject bot = bots[i];
			int rand = UnityEngine.Random.Range(0,10);
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
	
	public void quitRace(){
		focusRacer = startingLines_current[4];
		gc.clearListAndObjects(racers);
		gc.goStartScreen();
	}
	
	void setPlayer(GameObject racer){
		player = racer;
		playerAtt = player.GetComponent<PlayerAttributes>();
		playerAnim = player.GetComponent<PlayerAnimationV2>();
		playerTc = player.GetComponent<TimerController>();
		playerOc = player.GetComponent<OrientationController>();
		playerEmc = player.GetComponent<EnergyMeterController>();
		playerRb = player.GetComponent<Rigidbody>();
		}
	
	void updateSpeedometer(){
		
		
		// time
		timeSpan = TimeSpan.FromSeconds(raceTime);
		minutes = timeSpan.Minutes;
		seconds = timeSpan.Seconds;
		milliseconds = timeSpan.Milliseconds / 10;
		if(minutes > 0f){
			clock.text = minutes + ":" + seconds + "." + milliseconds;
		}
		else{
			clock.text = seconds + "." + milliseconds;
		}
		
		// speed
		float speed = 0f;
		if(focusRacer != null){
			PlayerAnimationV2 anim = focusRacer.GetComponent<PlayerAnimationV2>();
			if(anim != null){
				speed = (focusRacer.GetComponent<PlayerAnimationV2>().speedHoriz) / 2f;
			}
		}
		speed *= 2.236936f;
		speed = (Mathf.Round(speed * 10f) / 10f);
		speedometer.text = speed + " mph";
		
		
		/*
		// image
		float h,s,v;
		h = .26f + ((speed/30f));
		s = 1f;
		v = speed/30f;
		if(v < .5f){ v = .5f; }
	
		speedImage.GetComponent<Image>().color = Color.HSVToRGB(h, s, v);
		//speedImage.enabled = false;
		*/
	}
	
	void resetClock(){
		minutes = 0;
		seconds = 0;
		milliseconds = 0;
	}
	
	void setTransparency(GameObject racer, float alpha){
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[]{att.smr_shoes, att.smr_socks, att.smr_top, att.smr_bottoms, att.smr_sleeve, att.smr_headband, att.smr_dummy};
		// -----------------
		Material material;
		Color color;
		SkinnedMeshRenderer renderer;
		for(int i = 0; i < renderers.Length; i++){
			renderer = renderers[i];
			material = renderer.materials[0];
			color = material.color;
			color.a = alpha;
			material.color = color;
			// -----------------
			//renderer.materials = new Material[]{material};
			//renderer.materials[0] = material;
		}
	}
	
	IEnumerator wait(float t){
		yield return new WaitForSeconds(t);
	}

}