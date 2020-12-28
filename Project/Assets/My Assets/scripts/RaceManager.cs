using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
	public CameraController cameraController;
	public FinishLineController finishLineController;
	public SplitsController splitsController;
	public CheckpointController checkpointController;
	public Text resultsText;
	public GameObject gameCamera;
	public GameObject cameraButton;
	// -----------------
	public int raceEvent;
	public int viewMode;
	public int raceStatus;
		public static int STATUS_MARKS = 1;
		public static int STATUS_SET = 2;
		public static int STATUS_GO = 3;
		public static int STATUS_FINISHED = 4;
		public static int STATUS_INACTIVE = 5;
	public bool fStart;
	public bool raceStarted;
	public float raceTime;
	public int raceTick;
	public bool cancelSaveGhost;
	public bool playerFinished;
	public bool allRacersFinished;
	public bool racerPB;
	public float racerPB_time;
	public bool userPB;
	public float userPB_time;
	public bool playerWR;
	public float userRecord_time;
	public string userRecord_name;
	public float wr_time;
	public string wr_name;
	public string raceEvent_string;
	public int focusIndex;
	// -----------------
	public GameObject player_backEnd;
	public GameObject player;
	int playerLane;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers;
	public List<GameObject> racers_laneOrder;
	public List<GameObject> bots;
	public List<GameObject> ghosts;
	public GameObject RacersBackEndParent;
	public GameObject RacersFieldParent;
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
	public GameObject finishTape;
	public GameObject hurdlesSetPrefab;
	public GameObject hurdlesSet;
	public GameObject curve1_surface;
	public GameObject curve2_surface;
	public GameObject curve1_sphere;
	public GameObject curve2_sphere;
	// -----------------
	public TextMeshProUGUI speedText;
	public TextMeshProUGUI clockText;
		TimeSpan timeSpan;
		int minutes;
		int seconds;
		int milliseconds;
	public Image energyUIImage;
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
	public GameObject finishPlaceHolder;
	
	
    // Start is called before the first frame update
    void Start()
    {
    }
	
	
	void Update(){
	
		if(raceStatus < STATUS_FINISHED){
			
			if(raceStatus >= STATUS_GO){
				raceTime += Time.deltaTime;
			}
			updateSpeedometer();
			
			if(GlobalController.allowInput){
				if(Input.GetKeyDown(KeyCode.C)){
					cameraController.cycleCameraMode(viewMode);
				}
				if(viewMode == VIEW_MODE_REPLAY){
					if(Input.GetKeyDown(SettingsManager.controlsRight)){
						focusIndex++;
						if(focusIndex >= racers_laneOrder.Count){
							focusIndex = 0;
						}
						cameraController.referenceObject = racers_laneOrder[focusIndex];
					}
					if(Input.GetKeyDown(SettingsManager.controlsLeft)){
						focusIndex--;
						if(focusIndex < 0){
							focusIndex = racers_laneOrder.Count-1;
						}
						cameraController.referenceObject = racers_laneOrder[focusIndex];
					}
				}	
			}
			
			if(raceTime >= 90f){
				if(!cancelSaveGhost){
					cancelSaveGhost = true;
					StartCoroutine(gc.showNotificationBlip("Your ghost will not be saved (too much data).", 5f));
				}
			}
			
			GameObject racer;
			int cameraMode = cameraController.mode;
			bool setTrans = false;
			if(viewMode == RaceManager.VIEW_MODE_LIVE){
				for(int i = 0; i < racers_laneOrder.Count; i++){
					racer = racers_laneOrder[i];
					if(racer != player){
						float a = 1f;
						if(racer.tag == "Bot"){
							a = transparency_bot_base;
							
							bool sandwiched = (cameraMode == CameraController.SIDE_RIGHT && i >= playerLane) || (cameraMode == CameraController.SIDE_LEFT && i < playerLane);
							if(sandwiched){
								float distance = Mathf.Abs(gameCamera.transform.InverseTransformDirection(racer.transform.position - player.transform.position).x);
								if(distance < 3f){
									setTrans = true;
									a *= (distance/3f);
									if(a < transparency_bot_min){
										a = transparency_bot_min;
									}
								}
							}
						}
						else if(racer.tag == "Ghost"){
							a = transparency_ghost_base;
							bool sandwiched = (cameraMode == CameraController.SIDE_RIGHT && i >= playerLane) || (cameraMode == CameraController.SIDE_LEFT && i < playerLane);
							if(sandwiched){
								float distance = Mathf.Min(Vector3.Distance(racer.transform.position, player.transform.position), Vector3.Distance(racer.transform.position, cameraController.camera.transform.position));
								if(distance < 3f){
									setTrans = true;
									a *= (distance/3f);
									if(a < transparency_ghost_min){
										a = transparency_ghost_min;
									}
								}
							}
						}
						if(setTrans){
							setTransparency(racer, a);
						}
					}
				}
			}
		}
	}

    void FixedUpdate()
    {
		if(raceStatus < STATUS_INACTIVE){
			if(!allRacersFinished){
				setRacerModes(raceStatus);
				if(raceStatus > STATUS_MARKS){
					manageRace();
				}
			}
			if(raceStatus >= STATUS_GO){
				raceTick++;
			}
		}
    }
	
	public void init(){
		raceStatus = STATUS_INACTIVE;
	}
	
	public void setRaceEvent(int e){
		raceEvent = e;
	}
	
	
	public List<GameObject> setupRace(List<GameObject> backEndRacers, int raceEvent, int viewMode, bool newLanes){
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
		cancelSaveGhost = false;
		playerFinished = false;
		allRacersFinished = false;
		racerPB = false;
		userPB = false;
		userPB_time = GlobalController.getUserPB(raceEvent);
		playerWR = false;
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
			StartCoroutine(setResultSheetHeader());
			
			// calculate bot difficulty and set player
			for(int i = 0; i < racers_backEnd.Count; i++){
				
				racer = racers_backEnd[i];
				
				if(racer.tag == "Player (Back End)"){
					player_backEnd = racer;
					botDifficulty = calculateDifficulty(raceEvent, GlobalController.getUserPB(raceEvent));
					break;
				}
			}
			cameraButton.SetActive(false);
		} else{
			cameraButton.SetActive(true);
		}
		// -- set up racers field
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
			att.copyAttributesFromOther(racers_backEnd[i], "all");
			att.isRacing = false;
			// --
			anim = racer.GetComponent<PlayerAnimationV2>();
			anim.globalController = gc;
			anim.init(raceEvent);
			anim.mode = PlayerAnimationV2.Set;
			anim.finished = false;
			anim.energy = 100f;
			anim.launchFlag = false;
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
				playerAnim.marker.SetActive(true);
				playerAnim.hudIndicatorT.position = playerAnim.hudIndicatorT.position + Vector3.up;
				emc.setUIImage(energyUIImage);
				emc.adjustForEnergyLevel(100f);
			}
			else if(racer.tag == "Bot"){
				Bot_AI ai = racer.GetComponent<Bot_AI>();
				ai.raceManager = this;
				ai.init(botDifficulty);
				anim.init(raceEvent);
				bots.Add(racer);
				botAtts[botCount] = att;
				botAnims[botCount] = anim;
				botRbs[botCount] = rb;
				botTcs[botCount] = tc;
				botOcs[botCount] = oc;
				botEmcs[botCount] = emc;
				botAIs[botCount] = ai;
				botCount++;
				if(viewMode == VIEW_MODE_LIVE){
					setTransparency(racer, transparency_bot_base);
				}
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
				if(viewMode == VIEW_MODE_LIVE){
					setTransparency(racer, transparency_ghost_base);
				}
			}
				
			racers.Add(racer);
		}
		// -----------------
		if(raceEvent == RACE_EVENT_100M){
			startingLines_current = startingLines_100m;
			startingBlocks_current = startingBlocks_100m;
			raceEvent_string = "100m";
		}
		else if(raceEvent == RACE_EVENT_200M){
			startingLines_current = startingLines_200m;
			startingBlocks_current = startingBlocks_200m;
			raceEvent_string = "200m";
		}
		else if(raceEvent == RACE_EVENT_400M){
			startingLines_current = startingLines_400m;
			startingBlocks_current = startingBlocks_400m;
			raceEvent_string = "400m";
		}
		else if(raceEvent == RACE_EVENT_60M){
			startingLines_current = startingLines_60m;
			startingBlocks_current = startingBlocks_60m;
			raceEvent_string = "60m";
		}
		if(viewMode == VIEW_MODE_REPLAY){
			player = racers[gc.playerIndex];
			EnergyMeterController emc = player.GetComponent<EnergyMeterController>();
			PlayerAnimationV2 pAnim = player.GetComponent<PlayerAnimationV2>();
			pAnim.marker.SetActive(true);
			pAnim.hudIndicatorRenderer.material = gc.hudIndicatorMat_player;
			pAnim.hudIndicatorT.position = pAnim.hudIndicatorT.position + Vector3.up;
			emc.setUIImage(energyUIImage);
			emc.adjustForEnergyLevel(100f);
			Time.timeScale = 4f;
		}
		// -----------------
		hideStartingBlocks();
		
		assignLanes(racers, newLanes);
		playerLane = playerAtt.lane;
		racers_laneOrder = new List<GameObject>(racers);
		racers_laneOrder.Sort((x,y) =>
			x.GetComponent<PlayerAttributes>().lane.CompareTo(y.GetComponent<PlayerAttributes>().lane));
	
		finishLineController.init();
		splitsController.init(raceEvent, player);
		checkpointController.player = this.player;
		
		resetClock();
		raceStatus = STATUS_MARKS;
		
		checkTipsConditions();
		
		focusIndex = racers_laneOrder.IndexOf(player);
		
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
		float cap = 0f;
		float minDifficulty = 0f;
		if(raceEvent == RACE_EVENT_100M){
			cap = 9.8f;
			minDifficulty = .65f;
		}
		else if(raceEvent == RACE_EVENT_200M){
			cap = 19.8f;
			minDifficulty = .7f;
		}
		else if(raceEvent == RACE_EVENT_400M){
			cap = 44f;
			minDifficulty = .825f;
		}
		else if(raceEvent == RACE_EVENT_60M){
			cap = 6.45f;
			minDifficulty = .85f;
		}
		
		float difficulty = cap/time;
		
		if(difficulty > 0f){
			if(difficulty > minDifficulty){
				if(difficulty > 1f){
					difficulty = 1f;
				}
			}
			else{
				difficulty = minDifficulty;
			}
		}
		else{
			difficulty = minDifficulty;
		}
		return difficulty;
	}
	
	void assignLanes(List<GameObject> racers, bool newLanes){
		
		List<int> lanes = new List<int>(){ 5, 4, 6, 3, 7, 2, 8, 1 };
		int index = racers.Count;
		lanes.RemoveRange(index, lanes.Count-index);
		if(viewMode == VIEW_MODE_LIVE){
			PlayerAttributes _att;
			int randIndex;
			if(viewMode == VIEW_MODE_LIVE){
				if(newLanes){
					for(int i = 0; i < racers.Count; i++){
						_att = racers[i].GetComponent<PlayerAttributes>();
						randIndex = UnityEngine.Random.Range(0, lanes.Count);
						_att.lane = lanes[randIndex];
						racers_backEnd[i].GetComponent<PlayerAttributes>().lane = lanes[randIndex];
						lanes.RemoveAt(randIndex);
					}
				}
				//setRenderQueues(racers);
			}
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
			
			lane = att.lane;
			line = startingLines_current[lane - 1];
			block = startingBlocks_current[lane - 1];
			
			racer.transform.position = line.transform.position + (line.transform.forward*-.13f) + Vector3.up*(Mathf.Pow(lane,.3f));
			
			block.SetActive(true);
			Physics.IgnoreCollision(anim.rightFootScript.bc, block.GetComponent<BoxCollider>());
			Physics.IgnoreCollision(anim.leftFootScript.bc, block.GetComponent<BoxCollider>());
			block.transform.position = line.transform.position + (line.transform.forward*-1f) + (Vector3.up*.22f);
			block.transform.rotation = line.transform.rotation;
			block.GetComponent<Rigidbody>().velocity = Vector3.zero;
			StartCoroutine(setBlockPedals(block, racers[i]));
			
				
			oc.sphere1 = curve1_sphere;
			oc.sphere2 = curve2_sphere;
			oc.sphere = oc.sphere1;
			if(raceEvent != RACE_EVENT_100M && raceEvent != RACE_EVENT_60M){
				oc.updateOrientation(false);
			}
			oc.initRotations();
		}
	}
	
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
		int side = cameraController.mode;
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
			raceStarted = true;
			if(viewMode == VIEW_MODE_LIVE){
				gc.audioController.playSound(AudioController.GUNSHOT, 0f);
				gc.audioController.playSound(AudioController.CHEERING, 0f);
			}
		}
	}
	
	
	public void manageRace(){
		bool allRacersFinished = racersFinished >= racerCount;
		// -----------------
		if(playerFinished){
			playerFinished = false;
			gc.showResultsScreen();
		}
		if(allRacersFinished){
			raceStatus = STATUS_FINISHED;
			//gc.endRace();
		}
		
		if(viewMode == VIEW_MODE_LIVE){
			playerAnim.readInput(raceTick);
			playerAnim.applyInput(raceTick);
			playerTc.recordInput(raceTick);
		}
		// -----------------
		for(int i = 0; i < bots.Count; i++){
			botAIs[i].runAI(raceTick);
			botAnims[i].readInput(raceTick);
			botAnims[i].applyInput(raceTick);
			botTcs[i].recordInput(raceTick);
		}
		// -----------------
		for(int i = 0; i < ghosts.Count; i++){
			ghostAnims[i].readInput(raceTick);
			ghostAnims[i].applyInput(raceTick);
			if(raceTick > 0){
				ghostAnims[i].setPositionAndVelocity(raceTick);
			}
		}
	}
	
	public void addFinisher(GameObject racer){
		racersFinished++;
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		PlayerAnimationV2 anim = racer.GetComponent<PlayerAnimationV2>();
		// -----------------
		float t;
		// -----------------
		if(racer == player){
			t = float.Parse(raceTime.ToString("F3"));
			playerFinished = true;
			if(viewMode == VIEW_MODE_LIVE){
				finishPlaceHolder.transform.position = racer.transform.position;
				cameraController.setCameraFocus(finishPlaceHolder, cameraController.mode);
				
				// handle if player PB
				if((t < att.personalBests[raceEvent]) || (att.personalBests[raceEvent] == -1f)){
					playerAtt.personalBests[raceEvent] = t;
					racerPB = true;
					racerPB_time = t;

					// handle if user PB
					if (t < userPB_time){
						userPB = true;
						userPB_time = t;
						
						// handle if player sets WR
						if(t <= (wr_time)){
							playerWR = true;
							att.resultTag = "<color=magenta>WR</color>";
						}
						else{
							att.resultTag = "<color=blue>PB</color>";
						}
						gc.unlockManager.updateUnlocks(raceEvent, t);
						finishLineController.yayParticles.Play();
					}
				}
				else{
					att.resultTag = "";
				}
				
			}
			anim.finished = true;
			anim.marker.SetActive(false);
		}
		else if(racer.tag == "Ghost"){
			t = att.personalBests[raceEvent];
		}
		else{
			t = ((float)(raceTick)) / 100f;
		}
		// -----------------
		if(viewMode == VIEW_MODE_LIVE){
			att.finishTime = t;
			att.pathLength = raceTick;
			string tString = t.ToString("F2").PadLeft(24-att.racerName.Length);
			att.resultString = "<color="+ att.resultColor+">" + att.racerName + "</color>" + tString + "  " + att.resultTag;
			if(racer == player){
				resultsText.text += "\n <b><color=blue>" + (racersFinished) + "</color>  " + att.resultString + "</b>";
			}
			else{
				resultsText.text += "\n " + (racersFinished) + "  " + att.resultString;
			}
		}
		// -----------------
		StartCoroutine(anim.pop(.5f));

	}
	
	public IEnumerator falseStart(){
		fStart = true;
		gc.goRaceScreen();
		gc.countdownController.showFalseStartText();
		yield return new WaitForSeconds(.2f);
		Time.timeScale = .15f;
		yield return new WaitForSeconds(2f*Time.timeScale);
		Time.timeScale = 1f;
		gc.restartRace();
	}
	
	public void falseStartBotsReaction(){
		for(int i = 0; i < bots.Count; i++){
			botAtts[i].isRacing = true;
			botAIs[i].runAI(raceTick+1);
		}
		raceTick++;
	}
	
	public void setRacerModes(int mode){
		if(mode == STATUS_MARKS){
			playerAnim.upInSet = false;
		}
		else if(mode == STATUS_SET){
			if(viewMode == VIEW_MODE_REPLAY){
				Time.timeScale = 2f;
			}
			if(!playerAnim.upInSet){
				if(viewMode == VIEW_MODE_LIVE){
					gc.audioController.playSound(AudioController.BLOCK_RATTLE, 0f);
					gc.audioController.playSound(AudioController.VOICE_SET, 0f);
				}
			}
			playerAnim.upInSet = true;
		}
		else if(mode == STATUS_GO){
			if(viewMode == VIEW_MODE_REPLAY){
				Time.timeScale = 1f;
			}
			playerAtt.isRacing = true;
		}	
		for(int i = 0; i < ghostCount; i++){
			if(mode == STATUS_MARKS){
				ghostAnims[i].upInSet = false;
			}
			else if(mode == STATUS_SET){
				ghostAnims[i].upInSet = true;
			}
			else if(mode == STATUS_GO){
				ghostAtts[i].isRacing = true;
			}	
		}
		for(int i = 0; i < botCount; i++){
			if(mode == STATUS_MARKS){
				botAnims[i].upInSet = false;
			}
			else if(mode == STATUS_SET){
				botAnims[i].upInSet = true;
			}
			else if(mode == STATUS_GO){
				botAtts[i].isRacing = true;
			}	
		}
	}
	
	public void quitRace(){
		for(int i = 0; i < racers.Count; i++){
			racers[i].GetComponent<PlayerAnimationV2>().pop(0f);
		}
		gc.taskManager.addTask(TaskManager.SAVE_DEFAULT_CAMERA_MODES);
		gc.clearListAndObjects(racers);
		cameraController.setCameraFocusOnStart();
		gc.goSetupScreen();
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
			clockText.text = minutes + ":" + seconds + "." + milliseconds;
		}
		else{
			clockText.text = seconds + "." + milliseconds + " s";
		}
		
		// speed
		float speed = player.GetComponent<PlayerAnimationV2>().speedHoriz / 2f;
		speed *= 2.236936f;
		speed = (Mathf.Round(speed * 10f) / 10f);
		speedText.text = speed + " mph";
		speedText.color = Color.Lerp(Color.white, Color.red, speed/35f);

	}
	
	void resetClock(){
		minutes = 0;
		seconds = 0;
		milliseconds = 0;
	}
	
	void checkTipsConditions(){
		if(!GlobalController.hasFirstRace){
			gc.tipsManager.showTips_firstRace();
		}
		else if(raceEvent == RACE_EVENT_400M){
			if(!GlobalController.hasFirstRace_400m){
				gc.tipsManager.showTips_400m();
			}
		}
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
		}
	}
	
	IEnumerator setResultSheetHeader(){
		userRecord_time = GlobalController.getUserRecordTime(raceEvent);
		userRecord_name = GlobalController.getUserRecordName(raceEvent);
		string s;
		
		// attempt to log in if not logged in
		if(!PlayFabManager.loggedIn){
			PlayFabManager.loginError = false;
			StartCoroutine(gc.handleLogin(true, false));
			yield return new WaitUntil(() => (PlayFabManager.loggedIn || PlayFabManager.loginError));
		}
		// if logged in, attempt to get world record info
		if(PlayFabManager.loggedIn){
			GlobalController.getWorldRecordInfo(raceEvent);
			yield return new WaitUntil(() => (PlayFabManager.userLeaderboardInfoRetrieved || PlayFabManager.leaderboardGetError));
		}
		
		// if not logged in or world record retrieval failed, default wr info
		if(!PlayFabManager.loggedIn || PlayFabManager.leaderboardGetError){
			wr_time = -1f;
			wr_name = "NA";
			s = raceEvent_string + " Dash Finals\n==========================\n<color=magenta>WR:</color> <No connection!>";
		}
		
		// if logged in and got world record info, good to go
		else{
			wr_time = PlayFabManager.userScore/-10000f;
			wr_name = PlayFabManager.userRacerName + " (" + PlayFabManager.userDisplayName + ")";
			s = raceEvent_string + " Dash Finals\n==========================\n<color=magenta>WR: " + (wr_time.ToString("F3")) + "</color> " + wr_name;
		}
		resultsText.text = s;
		if(userRecord_name != "None"){
			s += "\n<color=blue>PB: " + (userRecord_time.ToString("F3")) + "</color> " + userRecord_name;
		}
		s += "\n\n==========================\n\nFinals";
		resultsText.text = s;
	}
	
	IEnumerator setBlockPedals(GameObject block, GameObject racer){
		while(raceStatus != STATUS_SET){
			block.GetComponent<StartingBlockController>().adjustPedals(racer.GetComponent<PlayerAnimationV2>());
			yield return null;
		}
	}
	
	IEnumerator wait(float t){
		yield return new WaitForSeconds(t);
	}

}