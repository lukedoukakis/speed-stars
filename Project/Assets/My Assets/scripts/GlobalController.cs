using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GlobalController : MonoBehaviour
{
	
	
	public bool ranBefore;
	
	public static string PLAYABLE_RACER_MEMORY = "PLAYABLE RACER IDS";
	public static string SAVED_RACER_MEMORY = "SAVED RACER IDS";
	
	// UI
	public GameObject canvas;
	public GameObject canvas_camera;
	public GameObject RaceUI;
		bool RaceUIActive;
	public GameObject StartScreen;
		bool StartScreenActive;
	public GameObject SetupScreen;
		bool SetupScreenActive;
	public GameObject CharacterCreatorScreen;
		public GameObject previewRacer_creation;
		public GameObject previewRacer_setup;
		public GameObject newRacerButton;
		bool CharacterCreatorScreenActive;
		public GameObject nameInputField;
	public GameObject CountdownScreen;
		bool CountdownScreenActive;
	public GameObject RaceScreen;
		public Image screenFlash;
		bool RaceScreenActive;
	public GameObject FinishScreen;
		bool FinishScreenActive;
	public GameObject TransitionScreen;
		bool TransitionScreenActive;
	public ButtonHandler buttonHandler;
	public SelectionListScript ghostSelectButtonList;
	public SelectionListScript playerSelectButtonList;
	public List<string> playableRacerIDs;
	public List<string> savedRacerIDs;
	public GameObject tooltip;
	
	// countdown
	public TaskManager taskManager;
	public CountdownController countdownController;
	public CameraController cameraController;
	public AudioController audioController;
	public MusicManager musicManager;
	public RaceManager raceManager;
	public SetupManager setupManager;
	public UnlockManager unlockManager;
	
	// racers
	public GameObject racerPrefab;
	public GameObject player_backEnd;
	public int playerIndex;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers_backEnd_replay;
	public List<GameObject> racers;
	
	// scene
	public int selectedRaceEvent;
	
	// hud
	public GameObject overheadCamera;
	public Material hudIndicatorMat_player;
	public Material hudIndicatorMat_nonPlayer;
	
	// preview cameras
	public Camera previewCamera_setup;
	public Camera previewCamera_creation;
	
	// finish screen ghost
	public List<int> checkedRacerIndexes;
	
	public GameObject legends;
	
    // Start is called before the first frame update
    void Start()
    {
	
	
		if(PlayerPrefs.GetInt("ranBefore") == 1){
			ranBefore = true;
			musicManager.init(false);
		}
		else{
			ranBefore = false;
			musicManager.init(true);
			runFirstTimeOperations();
		}
		
		setupManager.init();
		
		int charSlots = PlayerPrefs.GetInt("Character Slots");
		unlockManager.init(charSlots);
		
		Application.targetFrameRate = 60;
		
		racers_backEnd = new List<GameObject>();
		racers_backEnd_replay = new List<GameObject>();
		racers = new List<GameObject>();
		// -----------------
		SetupScreen.SetActive(true);
		playerSelectButtonList.init(RaceManager.RACE_EVENT_100M, PLAYABLE_RACER_MEMORY, 1, 0, true, true);
		playableRacerIDs = PlayerPrefs.GetString("PLAYABLE RACER IDS").Split(':').ToList();
		
		ghostSelectButtonList.init(RaceManager.RACE_EVENT_100M, SAVED_RACER_MEMORY, 7, 0, false, false);
		savedRacerIDs = PlayerPrefs.GetString("SAVED RACER IDS").Split(':').ToList();
		// -----------------
		previewRacer_creation.GetComponent<PlayerAnimationV2>().setIdle();
		SetupScreen.SetActive(false);
		// -----------------
		checkedRacerIndexes = new List<int>();
		// -----------------
		if(ranBefore){
			goStartScreen();
		}
		else{
			goCharacterCreatorScreen();
			CharacterCreatorScreen.transform.Find("Back Button").gameObject.SetActive(false);
		}
		// -----------------
		cameraController.setCameraFocus("100m Start", CameraController.STATIONARY);
    }

    // Update is called once per frame
    void Update()
    {
		
		// debug
		if(Input.GetKeyUp(KeyCode.F1)){
			Debug.Log("Deleting PlayerPrefs");
			PlayerPrefs.DeleteAll();
		}
		
		/*
		if(Input.GetKeyUp(KeyCode.T)){
			Time.timeScale -= .1f;
		}
		if(Input.GetKeyUp(KeyCode.Y)){
			Time.timeScale += .1f;
		}
		*/
		
		
		if(CountdownScreenActive){
			if(countdownController.state == CountdownController.SET){
				raceManager.raceStatus = RaceManager.STATUS_SET;
			}
			if(countdownController.finished){
				goRaceScreen();
			}
		}
		
		if(RaceUIActive){
			if(Input.GetKeyDown(KeyCode.R)){
				restartRace();
			}
			if(Input.GetKeyDown(KeyCode.Escape)){
				raceManager.quitRace();
			}
		}
		
    }
	
	public void goStartScreen(){
		//Debug.Log("Going to start screen");
		StartCoroutine(screenTransition("Start Screen", false));
		if(countdownController.isRunning){ countdownController.cancelCountdown(); }
	}
	
	public void goSetupScreen(){
		//Debug.Log("Going to setup screen");
		StartCoroutine(screenTransition("Setup Screen", false));
	}
	
	public void goCharacterCreatorScreen(){
		//Debug.Log("Going to creator screen");
		PlayerAttributes previewAtt = previewRacer_creation.GetComponent<PlayerAttributes>();
		previewAtt.setBodyProportions(PlayerAttributes.DEFAULT);
		previewAtt.setClothing(PlayerAttributes.DEFAULT);
		StartCoroutine(screenTransition("Character Creator Screen", false));
	}
	
	public void goCountDownScreen(){
		//Debug.Log("Going to countdown screen");
		StartCoroutine(screenTransition("Countdown Screen", false));
		// -----------------
		if(countdownController.isRunning){ countdownController.cancelCountdown(); }
		countdownController.startCountdown();
	}
	
	public void goRaceScreen(){
		//Debug.Log("Going to race screen");
		StartCoroutine(screenTransition("Race Screen", true));
		// -----------------
		StartCoroutine("flash");
		raceManager.raceStatus = RaceManager.STATUS_GO;
		raceManager.setOffRacers();

	
	}
	
	public void goFinishScreen(){
		//Debug.Log("Going to finish screen");
		StartCoroutine(screenTransition("Finish Screen", true));
		// -----------------
		buttonHandler.setResultButtons(racers.Count);
	}
	
	
	public IEnumerator screenTransition(string nextScreen, bool immediate){
		TransitionScreen.SetActive(true);
		CanvasGroup cg = TransitionScreen.GetComponent<CanvasGroup>();
		if(!immediate){
			// fade in ----------------
			while(cg.alpha < 1f){
				cg.alpha += .1f;
				yield return null;
			}
			// -----------------
			doTasks();
			// -----------------
		}
		canvas_camera.SetActive(false);
		RaceUI.SetActive(false);
			RaceUIActive = false;
		StartScreen.SetActive(false);
			StartScreenActive = false;
		SetupScreen.SetActive(false);
			SetupScreenActive = false;
		CharacterCreatorScreen.SetActive(false);
			CharacterCreatorScreenActive = false;
		CountdownScreen.SetActive(false);
			CountdownScreenActive = false;
			countdownController.hideFalseStartText();
		RaceScreen.SetActive(false);
			RaceScreenActive = false;
		FinishScreen.SetActive(false);
			FinishScreenActive = false;
		overheadCamera.SetActive(false);
		previewCamera_creation.gameObject.SetActive(false);
		previewCamera_setup.gameObject.SetActive(false);
		switch (nextScreen) {
			case "Start Screen" :
				StartScreen.SetActive(true);
				StartScreenActive = true;
				break;
			case "Setup Screen" :
				SetupScreen.SetActive(true);
				SetupScreenActive = true;
				previewCamera_setup.gameObject.SetActive(true);
				break;	
			case "Character Creator Screen" :
				CharacterCreatorScreen.SetActive(true);
				CharacterCreatorScreenActive = true;
				canvas_camera.SetActive(true);
				previewCamera_creation.gameObject.SetActive(true);
				break;	
			case "Countdown Screen" :
				CountdownScreen.SetActive(true);
				RaceUI.SetActive(true);
				CountdownScreenActive = true;
				RaceUIActive = true;
				overheadCamera.SetActive(true);
				break;
			case "Race Screen" :
				RaceScreen.SetActive(true);
				RaceUI.SetActive(true);
				RaceScreenActive = true;
				RaceUIActive = true;
				overheadCamera.SetActive(true);
				break;
			case "Finish Screen" :
				FinishScreen.SetActive(true);
				FinishScreenActive = true;
				break;
			default:
				break;
		}
		if(!immediate){
			// fade out ----------------
			while(cg.alpha > 0f){
				cg.alpha -= .1f;
				yield return null;
			}
			TransitionScreen.SetActive(false);
		}
	}
	
	public void goScreenAfterCharacterCreation(){
		if(ranBefore){
			goSetupScreen();
		}
		else{
			goStartScreen();
		}
	}
	
	//-----------------------------------------------------------------------------------------------------------
	public void startNewRace(){
		startRaceAsLive(true);
	}
	public void restartRace(){
		startRaceAsLive(false);
	}
	public void startRaceAsLive(bool newLanes){
		Time.timeScale = 1f;
		startRace(selectedRaceEvent, RaceManager.VIEW_MODE_LIVE, newLanes);
	}
	public void startRaceAsReplay(){
		if(raceManager.viewMode != RaceManager.VIEW_MODE_REPLAY){
			taskManager.addTask(TaskManager.SETUP_REPLAY);
		}
		startRace(selectedRaceEvent, RaceManager.VIEW_MODE_REPLAY, false);
	}
	public void startRace(int raceEvent, int viewMode, bool newLanes){
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			StopCoroutine(raceManager.falseStart());
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			
		}
		goCountDownScreen();
		StartCoroutine(setupRaceWhenReady(raceEvent, viewMode, newLanes));
	}
	IEnumerator setupRaceWhenReady(int raceEvent, int viewMode, bool newLanes){
		yield return new WaitUntil(() => taskManager.tasks.Count == 0);
		clearListAndObjects(racers);
		// -----------------
		List<GameObject> backEndRacers = new List<GameObject>();
		int cameraMode = 0;
		// --
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			backEndRacers = racers_backEnd;
			cameraMode = CameraController.SIDE;
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			backEndRacers = racers_backEnd_replay;
			cameraMode = CameraController.TV;
		}
		// --
		racers = raceManager.setupRace(backEndRacers, raceEvent, viewMode, newLanes);
		cameraController.setCameraFocus(raceManager.player, cameraMode);
		Debug.Log("camera mode: " + cameraMode);
	}
	//-----------------------------------------------------------------------------------------------------------
	public void showResultsScreen(){
		goFinishScreen();
	}
	public void endRace(){
		if(raceManager.playerPB){
			taskManager.addTask(TaskManager.SAVE_PLAYER);
			if(raceManager.userPB){
				Debug.Log("USER PB");
				taskManager.addTask(TaskManager.SAVE_USER_PB);
			}
			if(raceManager.playerWR){
				taskManager.addTask(TaskManager.SET_WR);
			}
		}
	}
	
	
	
	//-----------------------------------------------------------------------------------------------------------
	public void clearListAndObjects(List<GameObject> list){
		list.Clear();
		Transform parent = null;
		if(list == racers_backEnd){
			parent = raceManager.RacersBackEndParent.transform;
		}
		else if(list == racers){
			parent = raceManager.RacersFieldParent.transform;
		}
		if(parent != null){
			foreach(Transform child in parent){
				Destroy(child.gameObject);
			}
		}
	}
	
	public void forgetRacer(string id, string[] sourceMemoryLocations, bool forReplay){
		
		// remove from memory
		if(forReplay){
			id += "_REPLAY";
			PlayerPrefs.DeleteKey(id);
		}
		
		// remove from remembered IDs list
		string MEM_LOC = "";
		for(int i = 0; i < sourceMemoryLocations.Length; i++){
			MEM_LOC = sourceMemoryLocations[i];
			if(MEM_LOC == PLAYABLE_RACER_MEMORY){
				playableRacerIDs.Remove(id);
				string s = PlayerPrefs.GetString(PLAYABLE_RACER_MEMORY).Replace(id + ":", "");
				PlayerPrefs.SetString(PLAYABLE_RACER_MEMORY, s);
				if(!PlayerPrefs.GetString(SAVED_RACER_MEMORY).Contains(id)){
					PlayerPrefs.DeleteKey(id);
				}
			}
			else if(MEM_LOC == SAVED_RACER_MEMORY){
				savedRacerIDs.Remove(id);
				string s = PlayerPrefs.GetString(SAVED_RACER_MEMORY).Replace(id + ":", "");
				PlayerPrefs.SetString(SAVED_RACER_MEMORY, s);
				if(!PlayerPrefs.GetString(PLAYABLE_RACER_MEMORY).Contains(id)){
					PlayerPrefs.DeleteKey(id);
				}
			}
		}
	}
	
	public void saveRacer(GameObject racer, int raceEvent, string[] targetMemoryLocations, bool forReplay){
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		string id = att.id;
		if(forReplay){
			id += "_REPLAY";
		}
		else{
			string MEM_LOC = "";
			for(int i = 0; i < targetMemoryLocations.Length; i++){
				MEM_LOC = targetMemoryLocations[i];
				// -----------------
				if(MEM_LOC == PLAYABLE_RACER_MEMORY){
					if(Array.IndexOf(playableRacerIDs.ToArray(), id) == -1f){
						PlayerPrefs.SetString(PLAYABLE_RACER_MEMORY, PlayerPrefs.GetString(PLAYABLE_RACER_MEMORY) + id + ":");
						playableRacerIDs.Add(id);
					}
				}
				else if(MEM_LOC == SAVED_RACER_MEMORY){
					if(Array.IndexOf(savedRacerIDs.ToArray(), id) == -1f){
						PlayerPrefs.SetString(SAVED_RACER_MEMORY, PlayerPrefs.GetString(SAVED_RACER_MEMORY) + id + ":");
						savedRacerIDs.Add(id);
					}
				}
			}
		}
		// -----------------
		// paths
		string vM, vX, vY, vZ, pX, pY, pZ, r, l;
		vM = vX = vY = vZ = pX = pY = pZ = r = l = "";
		int pLength = att.pathLength;
		int leanLockTick = att.leanLockTick;
		//Debug.Log("path length: " + pLength);
		for(int i = 0; i < pLength; i++){
			vM += att.velMagPath[i].ToString() + ",";
			vY += att.velPathY[i].ToString() + ",";
			pY += att.posPathY[i].ToString() + ",";
			pZ += att.posPathZ[i].ToString() + ",";
			r += att.rightInputPath[i].ToString() + ",";
			l += att.leftInputPath[i].ToString() + ",";
		}
		// -----------------
		// clothing
		string dummyColor, topColor, bottomsColor, shoesColor, socksColor, headbandColor, sleeveColor;
		dummyColor = topColor = bottomsColor = shoesColor = socksColor = headbandColor = sleeveColor = "";
		for(int i = 0; i < 3; i++){
			dummyColor += att.dummyRGB[i].ToString() + ",";
			topColor += att.topRGB[i].ToString() + ",";
			bottomsColor += att.bottomsRGB[i].ToString() + ",";
			shoesColor += att.shoesRGB[i].ToString() + ",";
			socksColor += att.socksRGB[i].ToString() + ",";
			headbandColor += att.headbandRGB[i].ToString() + ",";
			sleeveColor += att.sleeveRGB[i].ToString() + ",";
		}
		
		
		// -----------------
		int[] raceEvents = new int[0];
		if(raceEvent == -1){
			raceEvents = new int[4]{0,1,2,3};
		}
		else{
			raceEvents = new int[1]{raceEvent};
		}
		for(int i = 0; i < raceEvents.Length; i++){
			int _raceEvent = raceEvents[i];
			// -----------------
			PlayerPrefs.SetString(id+"_pathsForEvent "+_raceEvent,
				pLength
			+ ":" +	leanLockTick
			+ ":" +	vM
			+ ":" + vY
			+ ":" + pY
			+ ":" + pZ
			+ ":" + r
			+ ":" + l
			);
		}
		PlayerPrefs.SetString(id,
				id
		+ ":" + att.racerName
		+ ":" + att.finishTime
		+ ":" + att.personalBests[0]
		+ ":" + att.personalBests[1]
		+ ":" + att.personalBests[2]
		+ ":" + att.personalBests[3]
		+ ":" + att.resultString
		+ ":" + att.POWER
		+ ":" + att.TRANSITION_PIVOT_SPEED
		+ ":" + att.QUICKNESS
		+ ":" + att.KNEE_DOMINANCE
		+ ":" + att.TURNOVER
		+ ":" + att.FITNESS
		+ ":" + att.LAUNCH_POWER
		+ ":" + att.CURVE_POWER
		+ ":" + att.CRUISE
		+ ":" + att.dummyMeshNumber
		+ ":" + att.topMeshNumber
		+ ":" + att.bottomsMeshNumber
		+ ":" + att.shoesMeshNumber
		+ ":" + att.socksMeshNumber
		+ ":" + att.headbandMeshNumber
		+ ":" + att.sleeveMeshNumber
		+ ":" + att.dummyMaterialNumber
		+ ":" + att.topMaterialNumber
		+ ":" + att.bottomsMaterialNumber
		+ ":" + att.shoesMaterialNumber
		+ ":" + att.socksMaterialNumber
		+ ":" + att.headbandMaterialNumber
		+ ":" + att.sleeveMaterialNumber
		+ ":" + dummyColor
		+ ":" + topColor
		+ ":" + bottomsColor
		+ ":" + shoesColor
		+ ":" + socksColor
		+ ":" + headbandColor
		+ ":" + sleeveColor
		+ ":" + att.headX
		+ ":" + att.headY
		+ ":" + att.headZ
		+ ":" + att.neckX
		+ ":" + att.neckY
		+ ":" + att.neckZ
		+ ":" + att.torsoX
		+ ":" + att.torsoY
		+ ":" + att.torsoZ
		+ ":" + att.armX
		+ ":" + att.armY
		+ ":" + att.armZ
		+ ":" + att.legX
		+ ":" + att.legY
		+ ":" + att.legZ
		+ ":" + att.feetX
		+ ":" + att.feetY
		+ ":" + att.feetZ
		+ ":" + att.height
		+ ":" + att.weight
		+ ":" + att.animatorNum
		+ ":" + att.leadLeg
		+ ":" + att.armSpeedFlex
		+ ":" + att.armSpeedExtend
		);
	}	
	
	public GameObject loadRacer(string id, int raceEvent, string asTag, bool forReplay){
		// -----------------
		
		GameObject racer = Instantiate(racerPrefab);
		racer.tag = asTag;
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		// -----------------
		if(forReplay){
			id += "_REPLAY";
		}
		
		att.personalBests = new float[4];
		string[] racerInfo = PlayerPrefs.GetString(id).Split(':');
		string[] pathInfo = PlayerPrefs.GetString(id+"_pathsForEvent "+raceEvent).Split(':');
		int i;
		// -----------------
		
		i = 0;
		Debug.Log("PATHLENGTH: " + pathInfo[i]);
		att.pathLength = int.Parse(pathInfo[i]); i++;
		att.leanLockTick = int.Parse(pathInfo[i]); i++;
		string[] vM = pathInfo[i].Split(','); i++;
		string[] vY = pathInfo[i].Split(','); i++;
		string[] pY = pathInfo[i].Split(','); i++;
		string[] pZ = pathInfo[i].Split(','); i++;
		string[] r = pathInfo[i].Split(','); i++;
		string[] l = pathInfo[i].Split(','); i++;
		
		i = 0;
		att.id = racerInfo[i]; i++;
		att.racerName = racerInfo[i]; i++;
		att.finishTime = float.Parse(racerInfo[i]); i++;
		att.personalBests[0] = float.Parse(racerInfo[i]); i++;
		att.personalBests[1] = float.Parse(racerInfo[i]); i++;
		att.personalBests[2] = float.Parse(racerInfo[i]); i++;
		att.personalBests[3] = float.Parse(racerInfo[i]); i++;
		att.resultString = racerInfo[i]; i++;
		att.POWER = float.Parse(racerInfo[i]); i++;
		att.TRANSITION_PIVOT_SPEED = float.Parse(racerInfo[i]); i++;
		att.QUICKNESS = float.Parse(racerInfo[i]); i++;
		att.KNEE_DOMINANCE = float.Parse(racerInfo[i]); i++;
		att.TURNOVER = float.Parse(racerInfo[i]); i++;
		att.FITNESS = float.Parse(racerInfo[i]); i++;
		att.LAUNCH_POWER = float.Parse(racerInfo[i]); i++;
		att.CURVE_POWER = float.Parse(racerInfo[i]); i++;
		att.CRUISE = float.Parse(racerInfo[i]); i++;
		att.dummyMeshNumber = int.Parse(racerInfo[i]); i++;
		att.topMeshNumber = int.Parse(racerInfo[i]); i++;
		att.bottomsMeshNumber = int.Parse(racerInfo[i]); i++;
		att.shoesMeshNumber = int.Parse(racerInfo[i]); i++;
		att.socksMeshNumber = int.Parse(racerInfo[i]); i++;
		att.headbandMeshNumber = int.Parse(racerInfo[i]); i++;
		att.sleeveMeshNumber = int.Parse(racerInfo[i]); i++;
		att.dummyMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.topMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.bottomsMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.shoesMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.socksMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.headbandMaterialNumber = int.Parse(racerInfo[i]); i++;
		att.sleeveMaterialNumber = int.Parse(racerInfo[i]); i++;
		string[] dummyColorArr = racerInfo[i].Split(','); i++;
		string[] topColorArr = racerInfo[i].Split(','); i++;
		string[] bottomsColorArr = racerInfo[i].Split(','); i++;
		string[] shoesColorArr = racerInfo[i].Split(','); i++;
		string[] socksColorArr = racerInfo[i].Split(','); i++;
		string[] headbandColorArr = racerInfo[i].Split(','); i++;
		string[] sleeveColorArr = racerInfo[i].Split(','); i++;
		att.headX = float.Parse(racerInfo[i]); i++;
		att.headY = float.Parse(racerInfo[i]); i++;
		att.headZ = float.Parse(racerInfo[i]); i++;
		att.neckX = float.Parse(racerInfo[i]); i++;
		att.neckY = float.Parse(racerInfo[i]); i++;
		att.neckZ = float.Parse(racerInfo[i]); i++;
		att.torsoX = float.Parse(racerInfo[i]); i++;
		att.torsoY = float.Parse(racerInfo[i]); i++;
		att.torsoZ = float.Parse(racerInfo[i]); i++;
		att.armX = float.Parse(racerInfo[i]); i++;
		att.armY = float.Parse(racerInfo[i]); i++;
		att.armZ = float.Parse(racerInfo[i]); i++;
		att.legX = float.Parse(racerInfo[i]); i++;
		att.legY = float.Parse(racerInfo[i]); i++;
		att.legZ = float.Parse(racerInfo[i]); i++;
		att.feetX = float.Parse(racerInfo[i]); i++;
		att.feetY = float.Parse(racerInfo[i]); i++;
		att.feetZ = float.Parse(racerInfo[i]); i++;
		att.height = float.Parse(racerInfo[i]); i++;
		att.weight = float.Parse(racerInfo[i]); i++;
		att.animatorNum = int.Parse(racerInfo[i]); i++;
		att.leadLeg = int.Parse(racerInfo[i]); i++;
		att.armSpeedFlex = float.Parse(racerInfo[i]); i++;
		att.armSpeedExtend = float.Parse(racerInfo[i]); i++;
		// -----------------
		
		// paths
		att.setPaths(att.pathLength + 500);
		for(int j = 0; j < att.pathLength; j++){
			att.velMagPath[j] = float.Parse(vM[j]);
			att.velPathY[j] = float.Parse(vY[j]);
			att.posPathY[j] = float.Parse(pY[j]);
			att.posPathZ[j] = float.Parse(pZ[j]);
			att.rightInputPath[j] = int.Parse(r[j]);
			att.leftInputPath[j] = int.Parse(l[j]);
		}
		
		// clothing colors
		float[] dummyRGB = new float[3];
		float[] topRGB = new float[3];
		float[] bottomsRGB = new float[3];
		float[] shoesRGB = new float[3];
		float[] socksRGB = new float[3];
		float[] headbandRGB = new float[3];
		float[] sleeveRGB = new float[3];
		for(int j = 0; j < 3; j++){
			dummyRGB[j] = float.Parse(dummyColorArr[j]);
			topRGB[j] = float.Parse(topColorArr[j]);
			bottomsRGB[j] = float.Parse(bottomsColorArr[j]);
			shoesRGB[j] = float.Parse(shoesColorArr[j]);
			socksRGB[j] = float.Parse(socksColorArr[j]);
			headbandRGB[j] = float.Parse(headbandColorArr[j]);
			sleeveRGB[j] = float.Parse(sleeveColorArr[j]);
		}
		att.dummyRGB = dummyRGB;
		att.topRGB = topRGB;
		att.bottomsRGB = bottomsRGB;
		att.shoesRGB = shoesRGB;
		att.socksRGB = socksRGB;
		att.headbandRGB = headbandRGB;
		att.sleeveRGB = sleeveRGB;
		// -----------------
		att.setClothing(PlayerAttributes.FROM_THIS);
		att.setBodyProportions(PlayerAttributes.FROM_THIS);
		att.setStats(PlayerAttributes.FROM_THIS);
		att.setAnimations(PlayerAttributes.FROM_THIS);
		// -----------------
		return racer;
	}
	
	public void loadBots(){
		int numOfBots = setupManager.botCount - raceManager.botCount;
		
		int b = 2;
		for(int i = 0; i < numOfBots; i++){
			
			if(UnityEngine.Random.Range(b,2) == 1){
				racers_backEnd.Add(getSpecialGhost(PlayerAttributes.USAINBOLT, raceManager.raceEvent));
				
				b = 2;
			}
			else{
				racers_backEnd.Add(getRandomBot());
			}	
		}
	}
	
	
	public GameObject getRandomBot(){
		GameObject bot = Instantiate(racerPrefab);
		
		bot.tag = "Bot (Back End)";
		bot.transform.SetParent(raceManager.RacersBackEndParent.transform);
		bot.SetActive(false);
		PlayerAttributes att = bot.GetComponent<PlayerAttributes>();
		att.setInfo(PlayerAttributes.RANDOM);
		att.finishTime = -1f;
		att.personalBests = new float[]{-1f,-1f,-1f,-1f};
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		att.pathLength = PlayerAttributes.DEFAULT_PATH_LENGTH;
		att.setClothing(PlayerAttributes.RANDOM);
		att.setBodyProportions(PlayerAttributes.RANDOM);
		att.setStats(PlayerAttributes.RANDOM);
		att.setAnimations(PlayerAttributes.RANDOM);
		// -----------------
		return bot;
	}
	
	
	
	public GameObject getSpecialGhost(int preset, int raceEvent){
		GameObject ghost = Instantiate(racerPrefab);
		
		ghost.tag = "Ghost (Back End)";
		ghost.transform.SetParent(raceManager.RacersBackEndParent.transform);
		ghost.SetActive(false);
		PlayerAttributes att = ghost.GetComponent<PlayerAttributes>();
		att.setInfo(preset);
		att.setClothing(preset);
		att.setBodyProportions(preset);
		att.setStats(preset);
		att.setAnimations(preset);
		att.setPathsFromSpecial(preset, raceEvent);
		// -----------------
		return ghost;
	}
	
	
	void doTasks(){
		int currentTask;
		for(int i = 0; i < taskManager.tasks.Count; i++){
			currentTask = taskManager.tasks[i];
			// -----------------
			if(currentTask == TaskManager.SAVE_PLAYER){
				savePlayer();
			}
			else if(currentTask == TaskManager.SAVE_SELECTED_RACERS){
				saveSelectedRacers();
			}
			else if(currentTask == TaskManager.LOAD_SELECTED_PLAYER){
				loadSelectedPlayer();
			}
			else if(currentTask == TaskManager.LOAD_SELECTED_RACERS){
				loadSelectedRacers();
			}
			else if(currentTask == TaskManager.SETUP_REPLAY){
				setupReplay();
			}
			else if(currentTask == TaskManager.CREATE_RACER){
				createRacer();
				if(!ranBefore){
					CharacterCreatorScreen.transform.Find("Back Button").gameObject.SetActive(true);
				}
			}
			else if(currentTask == TaskManager.CLEAR_RACERS_FROM_SCENE){
				clearRacersFromScene();
			}
			else if(currentTask == TaskManager.SAVE_USER_PB){
				saveUserPB(raceManager.raceEvent, raceManager.userPB_time);
			}
			else if(currentTask == TaskManager.SET_WR){
				setWorldRecord_local(raceManager.raceEvent, raceManager.userPB_time, raceManager.player.GetComponent<PlayerAttributes>().id);
			}
		}
		taskManager.tasks.Clear();
		checkedRacerIndexes.Clear();
	}
	
	
	// -----------------// -----------------// -----------------// -----------------// -----------------// -----------------// -----------------
	
	void createRacer(){
		Debug.Log("CREATE RACER");
		// -----------------
		GameObject newRacer = Instantiate(racerPrefab);
		newRacer.tag = "Player";
	
		newRacer.SetActive(false);
		PlayerAttributes att = newRacer.GetComponent<PlayerAttributes>();
		
		///*
		// random
		att.racerName = nameInputField.gameObject.transform.Find("Text").GetComponent<Text>().text;
		att.id = PlayerAttributes.generateID(att.racerName);
		att.copyAttributesFromOther(previewRacer_creation, "clothing");
		att.copyAttributesFromOther(previewRacer_creation, "body proportions");
		att.setStats(PlayerAttributes.RANDOM);
		att.setAnimations(PlayerAttributes.RANDOM);
		att.pathLength = 0;
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		//*/

		/*
		// preset
		int preset = PlayerAttributes.MICHAELJOHNSON;
		att.racerName = nameInputField.gameObject.transform.Find("Text").GetComponent<Text>().text;
		att.id = PlayerAttributes.generateID(att.racerName);
		att.setClothing(preset);
		att.setBodyProportions(preset);
		att.setStats(preset);
		att.setAnimations(preset);
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		*/
		
		att.finishTime = -1f;
		att.personalBests = new float[]{-1f, -1f, -1f, -1f};
		// -----------------
		saveRacer(newRacer, -1, new string[]{PLAYABLE_RACER_MEMORY}, false);
		Destroy(newRacer);
		// -----------------

		GameObject b = playerSelectButtonList.GetComponent<SelectionListScript>().addButton(att.id, selectedRaceEvent, SelectionButtonScript.playerButtonColor);
		playerSelectButtonList.numSelected = 1;
		playerSelectButtonList.toggleAllOff();
		b.GetComponent<SelectionButtonScript>().toggle();
		
		unlockManager.fillCharacterSlot();
		
		if(!ranBefore){
			PlayerPrefs.SetInt("ranBefore", 1);
		}
	}
	
	void savePlayer(){
		Debug.Log("SAVE PLAYER");
		string id = raceManager.player.GetComponent<PlayerAttributes>().id;
		GameObject b;
		// -----------------
		playerSelectButtonList.removeButton(id);
		saveRacer(raceManager.player, selectedRaceEvent, new string[]{PLAYABLE_RACER_MEMORY, SAVED_RACER_MEMORY}, false);
		b = playerSelectButtonList.addButton(id, selectedRaceEvent, SelectionButtonScript.playerButtonColor);
		b.GetComponent<SelectionButtonScript>().toggle();
		// -----------------
		bool selected = false;
		if(ghostSelectButtonList.getButton(id) != null){
			selected = ghostSelectButtonList.getButton(id).GetComponent<SelectionButtonScript>().selected;
		}
		ghostSelectButtonList.removeButton(id);
		b = ghostSelectButtonList.addButton(id, selectedRaceEvent, SelectionButtonScript.ghostButtonColor);
		if(selected){
			b.GetComponent<SelectionButtonScript>().toggle(true);
		}
	}
	
	void saveSelectedRacers(){
		Debug.Log("SAVE SELECTED RACERS");
		int racerIndex;
		GameObject racer;
		PlayerAttributes att;
		for(int j = 0; j < checkedRacerIndexes.Count; j++){
			racerIndex = checkedRacerIndexes[j];
			racer = racers[racerIndex];
			att = racer.GetComponent<PlayerAttributes>();
			saveRacer(racer, selectedRaceEvent, new string[]{SAVED_RACER_MEMORY}, false);
			ghostSelectButtonList.addButton(att.id, selectedRaceEvent, SelectionButtonScript.ghostButtonColor);
		}	
	}
	
	
	void loadSelectedPlayer(){
		Debug.Log("LOAD SELECTED PLAYER");
		GameObject player;
		GameObject b;
		SelectionButtonScript s;
		GameObject grid = playerSelectButtonList.grid;
		string playerID = "";
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.selected){
				playerID = s.id;
				break;
			}
		}
		// if we didn't find a selected player, select the first one
		if(playerID == ""){
			s = playerSelectButtonList.getFirst().GetComponent<SelectionButtonScript>();
			playerID = s.id;
		}
			
		// load player
		player = loadRacer(playerID, selectedRaceEvent, "Player (Back End)", false);
		player.GetComponent<PlayerAttributes>().setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		player.SetActive(false);
		player.transform.SetParent(raceManager.RacersBackEndParent.transform);
		racers_backEnd.Add(player);
	}
	
	
	void loadSelectedRacers(){
		Debug.Log("LOAD SELECTED RACERS");
		GameObject b;
		SelectionButtonScript s;
		GameObject grid = ghostSelectButtonList.grid;
		GameObject racer;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.selected){
				racer = loadRacer(s.id, selectedRaceEvent, "Ghost (Back End)", false);
				racer.transform.SetParent(raceManager.RacersBackEndParent.transform);
				racers_backEnd.Add(racer);
			}
		}
	}
	
	void setupReplay(){
		Debug.Log("SETUP REPLAY");
		int numOfRacers = racers.Count;
		playerIndex = 0;
		// -----------------
		clearListAndObjects(racers_backEnd_replay);
		GameObject racer;
		GameObject racer_replay;
		string id;
		for(int j = 0; j < numOfRacers; j++){
			
			racer = racers[j];
			id = racer.GetComponent<PlayerAttributes>().id;
			saveRacer(racer, selectedRaceEvent, new string[]{PLAYABLE_RACER_MEMORY}, true);
			racer_replay = loadRacer(id, selectedRaceEvent, "Ghost (Back End)", true);
			racer_replay.SetActive(false);
			racer_replay.transform.SetParent(raceManager.RacersBackEndParent.transform);
			racer_replay.GetComponent<PlayerAttributes>().lane = racers[j].GetComponent<PlayerAttributes>().lane;
			racers_backEnd_replay.Add(racer_replay);
			forgetRacer(id, new string[]{PLAYABLE_RACER_MEMORY}, true);
			if(racer.tag == "Player"){
				playerIndex = j;
			}
			
		}
		//cameraController.GetComponent<CameraController>().setForRaceEvent(selectedRaceEvent);
	}
	
	public void clearRacersFromScene(){
		Debug.Log("CLEAR RACERS FROM SCENE");
		clearListAndObjects(racers);
		clearListAndObjects(racers_backEnd);
		clearListAndObjects(racers_backEnd_replay);
	}
	
	void saveUserPB(int raceEvent, float time){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			PlayerPrefs.SetFloat("user_PB_100m", time);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			PlayerPrefs.SetFloat("user_PB_200m", time);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			PlayerPrefs.SetFloat("user_PB_400m", time);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			PlayerPrefs.SetFloat("user_PB_60m", time);
		}
		
		Debug.Log("updating unlocks: " + raceEvent + " " + time);
		unlockManager.updateUnlocks(raceEvent, time);
	}
	
	public float getUserPB(int raceEvent){

		if(raceEvent == RaceManager.RACE_EVENT_100M){
			return PlayerPrefs.GetFloat("user_PB_100m");
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			return PlayerPrefs.GetFloat("user_PB_200m");
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			return PlayerPrefs.GetFloat("user_PB_400m");
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			return PlayerPrefs.GetFloat("user_PB_60m");
		}
		else{
			return 0f;
		}
	}
	
	public void setWorldRecord_local(int raceEvent, float time, string racerID){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			PlayerPrefs.SetString("wr_local_100m", time + ":" + racerID);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			PlayerPrefs.SetString("wr_local_200m", time + ":" + racerID);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			PlayerPrefs.SetString("wr_local_400m", time + ":" + racerID);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			PlayerPrefs.SetString("wr_local_60m", time + ":" + racerID);
		}
	}
	
	public float getWorldRecordTime_local(int raceEvent){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			return float.Parse(PlayerPrefs.GetString("wr_local_100m").Split(':')[0]);
			//return 9.58f;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			return float.Parse(PlayerPrefs.GetString("wr_local_200m").Split(':')[0]);
			//return 19.19f;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			return float.Parse(PlayerPrefs.GetString("wr_local_400m").Split(':')[0]);
			//return 43.03f;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			return float.Parse(PlayerPrefs.GetString("wr_local_60m").Split(':')[0]);
			//return 6.34f;
		}
		else{
			return 0f;
		}
	}
	
	public string getWorldRecordID_local(int raceEvent){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			return PlayerPrefs.GetString("wr_local_100m").Split(':')[1];
			//return "Usain Bolt";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			return PlayerPrefs.GetString("wr_local_200m").Split(':')[1];
			//return "Usain Bolt";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			return PlayerPrefs.GetString("wr_local_400m").Split(':')[1];
			//return "Wayde van Niekerk";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			return PlayerPrefs.GetString("wr_local_60m").Split(':')[1];
			//return "Christian Coleman";
		}
		else{
			return "";
		}
	}
	
	void runFirstTimeOperations(){
		saveUserPB(RaceManager.RACE_EVENT_100M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_200M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_400M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_60M, float.MaxValue);
		setWorldRecord_local(RaceManager.RACE_EVENT_100M, 9.58f, "Usain Bolt_");
		setWorldRecord_local(RaceManager.RACE_EVENT_200M, 19.19f, "Usain Bolt_");
		setWorldRecord_local(RaceManager.RACE_EVENT_400M, 43.03f, "Wayde van Niekerk_");
		setWorldRecord_local(RaceManager.RACE_EVENT_60M, 6.34f, "Christian Coleman_");
		PlayerPrefs.SetInt("Character Slots", 1);
	}
	
	IEnumerator flash(){
		Color c = new Color32(255, 217, 0, 0);
		screenFlash.color = c;
		
		while(c.a < .2f){
			c.a += 3f*Time.deltaTime/Time.timeScale;
			screenFlash.color = c;
			yield return null;
		}
		while(c.a > 0f){
			c.a -= .35f*Time.deltaTime/Time.timeScale;
			screenFlash.color = c;
			yield return null;
		}
		c.a = 0f;
		screenFlash.color = c;
	}
	
	IEnumerator wait(float t){
		yield return new WaitForSeconds(t);
	}

}
