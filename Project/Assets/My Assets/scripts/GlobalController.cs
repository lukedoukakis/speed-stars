using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GlobalController : MonoBehaviour
{
	
	// first time vars
	public static bool hasInitialSetup;
	public static bool hasFirstRace;
	public static bool hasFirstRace_400m;
	public static bool hasFirstLeaderboard;
	
	
	public static string PLAYABLE_RACER_MEMORY = "PLAYABLE RACER IDS";
	public static string SAVED_RACER_MEMORY = "SAVED RACER IDS";
	
	// UI
	public GameObject canvas;
	public GameObject canvas_camera;
	public GameObject RaceUI;
		bool RaceUIActive;
		public static float keyPressAgo;
		[SerializeField] float cooldown_keyPress;
	public GameObject StartScreen;
		[SerializeField] Animator startScreenAnimator;
		bool StartScreenActive;
	public GameObject SetupScreen;
		bool SetupScreenActive;
	public GameObject LeaderboardScreen;
		bool LeaderboardScreenActive;
	public GameObject CharacterCreatorScreen;
		public GameObject previewRacer_creation;
		public GameObject previewRacer_setup;
		public GameObject newRacerButton;
		bool CharacterCreatorScreenActive;
		public GameObject nameInputField;
	public GameObject CountdownScreen;
		public bool CountdownScreenActive;
	public GameObject RaceScreen;
		public Image screenFlash;
		public bool RaceScreenActive;
	public GameObject FinishScreen;
		bool FinishScreenActive;
	public GameObject EmptyScreen;
		bool EmptyScreenActive;
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
	public WelcomeManager welcomeManager;
	public SettingsManager settingsManager;
	public TipsManager tipsManager;
	public SetupManager setupManager;
	public UnlockManager unlockManager;
	public NotificationBlipController notificationBlipController;
	public EnvironmentController environmentController;
	public LeaderboardManager leaderboardManager;
	public SteamController steamController;
	
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
	
	[SerializeField] Transform cameraStartTransform;
	
	// online leaderboard
	[SerializeField] Text usernameInputText;
	public string username;
	bool lbUpdate;
	public int downloadedGhosts;
	
	// input
	public static bool allowInput;
	
    // Start is called before the first frame update
    void Start()
    {

		// initialize immediate view and camera setting
		TransitionScreen.SetActive(true);
		CanvasGroup cg = TransitionScreen.GetComponent<CanvasGroup>();
		cg.alpha = 1f;


		cameraController.setCameraFocus("100m Start", CameraController.STATIONARY_SLOW);
		startScreenAnimator.SetBool("startScreenAccessed", false);
		
		// if first time running, run first rime operations
		if(PlayerPrefs.GetInt("hasInitialSetup") == 1){
			hasInitialSetup = true;
			if(PlayerPrefs.GetInt("hasFirstRace") == 1){
				hasFirstRace = true;
			} else { hasFirstRace = false; }
			if(PlayerPrefs.GetInt("hasFirstRace_400m") == 1){
				hasFirstRace_400m = true;
			} else { hasFirstRace_400m = false; }
			if (PlayerPrefs.GetInt("hasFirstLeaderboard") == 1)
			{
				hasFirstLeaderboard = true;
			}
			else { hasFirstLeaderboard = false; }
		}
		else{
			hasInitialSetup = false;
			runFirstTimeOperations();
		}
		
		// Steam and PlayFab login
		StartCoroutine(handleLogin(false, true));
		
		float audioVolume = PlayerPrefs.GetFloat("Audio Volume");
		downloadedGhosts = PlayerPrefs.GetInt("Downloaded Ghosts");
		int charSlots = PlayerPrefs.GetInt("Character Slots");
		string displayMode = PlayerPrefs.GetString("Display Mode");
		float cameraDistance = PlayerPrefs.GetFloat("Camera Distance");
		int cameraGameplayMode = PlayerPrefs.GetInt("Camera Gameplay Mode");
		int cameraReplayMode = PlayerPrefs.GetInt("Camera Replay Mode");
		KeyCode l = (KeyCode)PlayerPrefs.GetInt("Controls Left");
		KeyCode r = (KeyCode)PlayerPrefs.GetInt("Controls Right");
		
		cameraController.init(cameraDistance, cameraGameplayMode, cameraReplayMode);
		raceManager.init();
		musicManager.init(audioVolume, !hasInitialSetup);
		environmentController.init(PlayerPrefs.GetInt("Theme"));
		setupManager.init(false);
		settingsManager.init(l, r, cameraDistance, displayMode);
		unlockManager.init(charSlots);
		
		racers_backEnd = new List<GameObject>();
		racers_backEnd_replay = new List<GameObject>();
		racers = new List<GameObject>();
		playerSelectButtonList.init(RaceManager.RACE_EVENT_100M, PLAYABLE_RACER_MEMORY, 1, 0, true, true);
		playableRacerIDs = PlayerPrefs.GetString("PLAYABLE RACER IDS").Split(':').ToList();
		ghostSelectButtonList.init(RaceManager.RACE_EVENT_100M, SAVED_RACER_MEMORY, 7, 0, false, false);
		savedRacerIDs = PlayerPrefs.GetString("SAVED RACER IDS").Split(':').ToList();
		
		if(hasInitialSetup){
			goStartScreen();
		}
		else{
			//goCharacterCreatorScreen();
			goEmptyScreen();
			welcomeManager.show();
			CharacterCreatorScreen.transform.Find("Back Button").gameObject.SetActive(false);
		}
		
    }

    // Update is called once per frame
    void Update()
    {

		//Debug.Log(Time.deltaTime);
		
		// debug
		/*
		if(Input.GetKeyUp(KeyCode.F1)){
			Debug.Log("Deleting PlayerPrefs");
			PlayerPrefs.DeleteAll();
		}
		
		if(Input.GetKeyUp(KeyCode.F3)){
			Time.timeScale = Time.deltaTime;
		}
		if(Input.GetKeyUp(KeyCode.F4)){
			if(Time.timeScale == 0f){
				Time.timeScale = 1f;
			}
			else{
				Time.timeScale = 0f;
			}
		}
		
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
			if(allowInput){
				if(keyPressAgo >= cooldown_keyPress){
					if(Input.GetKeyDown(KeyCode.R)){
						restartRace();
						keyPressAgo = 0f;
					}
					if(Input.GetKeyDown(KeyCode.Escape)){
						raceManager.quitRace();
						keyPressAgo = 0f;
					}
				}
			}
		}
		keyPressAgo += Time.deltaTime;
    }
	
	public void goStartScreen(){
		//Debug.Log("Going to start screen");
		StartCoroutine(screenTransition("Start Screen", false));
		// -----------------
		if(countdownController.isRunning){ countdownController.cancelCountdown(); }
		raceManager.raceStatus = RaceManager.STATUS_INACTIVE;
	}
	
	public void goSetupScreen(){
		//Debug.Log("Going to setup screen");
		StartCoroutine(screenTransition("Setup Screen", false));
		// -----------------
		raceManager.raceStatus = RaceManager.STATUS_INACTIVE;
	}
	
	public void goLeaderboardScreen(){
		//Debug.Log("Going to leaderboard screen");
		StartCoroutine(screenTransition("Leaderboard Screen", false));
		if (!hasFirstLeaderboard)
		{
			tipsManager.showTips_leaderboard();
		}
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
		// -----------------;
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
		raceManager.raceStatus = RaceManager.STATUS_FINISHED;
	}
	public void goEmptyScreen(){
		//Debug.Log("Going to empty screen");
		StartCoroutine(screenTransition("Empty Screen", false));
		// -----------------
		raceManager.raceStatus = RaceManager.STATUS_INACTIVE;
	}
	
	
	public IEnumerator screenTransition(string nextScreen, bool immediate){
		allowInput = false;
		TransitionScreen.SetActive(true);
		GameObject[] deactivatedButtons0 = deactivateAllButtons();
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
		LeaderboardScreen.SetActive(false);
			LeaderboardScreenActive = false;
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
				setupManager.setSelectedRaceEvent(RaceManager.RACE_EVENT_100M, false);
				StartScreenActive = true;
				break;
			case "Setup Screen" :
				SetupScreen.SetActive(true);
				SetupScreenActive = true;
				previewCamera_setup.gameObject.SetActive(true);
				if (playerSelectButtonList.numSelected < 1){ playerSelectButtonList.getFirst().GetComponent<SelectionButtonScript>().toggle(); }
				break;	
			case "Leaderboard Screen" :
				LeaderboardScreen.SetActive(true);
				LeaderboardScreenActive = true;
				leaderboardManager.setLeaderboardMode(LeaderboardManager.GLOBAL);
				leaderboardManager.setEmpty();
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
			case "Empty Screen" :
				EmptyScreen.SetActive(true);
				EmptyScreenActive = true;
				break;
			default:
				break;
		}
		GameObject[] deactivatedButtons1 = deactivateAllButtons();
		if(!immediate){
			// fade out ----------------
			while(cg.alpha > 0f){
				cg.alpha -= .1f;
				yield return null;
			}
			TransitionScreen.SetActive(false);
		}
		allowInput = true;
		activateButtons(deactivatedButtons0);
		activateButtons(deactivatedButtons1);
	}
	
	public void goScreenAfterCharacterCreation(){
		if(hasInitialSetup){
			goSetupScreen();
		}
		else{
			clearRacersFromScene();
			raceManager.resetCounts();
			buttonHandler.loadSelectedPlayer();
			loadBots();
			startNewRace();
		}
	}
	
	//-----------------------------------------------------------------------------------------------------------
	public void startNewRace(){
		startRaceAsLive(true);
	}
	public void restartRace(){
		PlayerPrefs.SetInt("Camera Gameplay Mode", cameraController.cameraGameplayMode);
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
			countdownController.showCountdownText();
			audioController.playSound(AudioController.VOICE_READY, 0f);
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			countdownController.hideCountdownText();
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
			cameraMode = cameraController.cameraGameplayMode;
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			backEndRacers = racers_backEnd_replay;
			cameraMode = cameraController.cameraReplayMode;
		}
		// --
		racers = raceManager.setupRace(backEndRacers, raceEvent, viewMode, newLanes);
		cameraController.setCameraFocus(raceManager.player, cameraMode);
	}
	//-----------------------------------------------------------------------------------------------------------
	public void showResultsScreen(){
		StartCoroutine(endRace());
		goFinishScreen();
	}
	public IEnumerator endRace(){
		//Debug.Log("ending race");
		lbUpdate = false;
		if(raceManager.racerPB && !raceManager.cancelSaveGhost){
			if(raceManager.userPB){
				
				// DEBUG: COMMENT THIS BLOCK OUT TO PREVENT SAVING SCORES
				//Debug.Log("USER PB");
				lbUpdate = true;
				taskManager.addTask(TaskManager.SAVE_USER_PB);
				taskManager.addTask(TaskManager.SET_USER_RECORD);
				
			}
			taskManager.addTask(TaskManager.SAVE_PLAYER);
		}
		taskManager.addTask(TaskManager.SAVE_DEFAULT_CAMERA_MODES);
		yield return null;
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
	
	public void saveRacer(GameObject racer, int raceEvent, string[] targetMemoryLocations, bool sendToLeaderboard, bool forReplay){
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
		
		string pathsForEventString = "";
		string attString = "";
		
		for(int i = 0; i < raceEvents.Length; i++){
			int _raceEvent = raceEvents[i];
			// -----------------
			string s = 
					pLength
			+ ":" +	leanLockTick
			+ ":" +	vM
			+ ":" + vY
			+ ":" + pY
			+ ":" + pZ
			+ ":" + r
			+ ":" + l;
			pathsForEventString += s;
			PlayerPrefs.SetString(id+"_pathsForEvent "+_raceEvent, pathsForEventString);
		}
		
		attString =
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
		+ ":" + att.armSpeedFlexL
		+ ":" + att.armSpeedExtendL
		+ ":" + att.armSpeedFlexR
		+ ":" + att.armSpeedExtendR;
		PlayerPrefs.SetString(id, attString);
		
		if(sendToLeaderboard){
		
			int totalScore = ScoreCalculator.calculateScore_user();
			if(totalScore <= PlayerPrefs.GetInt("Total Score1", 0)){ totalScore = -1; }
			else{ PlayerPrefs.SetInt("Total Score1", totalScore); }
			int totalScore_racer = ScoreCalculator.calculateScore(att.personalBests);
			if(totalScore_racer <= PlayerPrefs.GetInt("Total Score (Best Racer)1", 0)){ totalScore_racer = -1; }
			else{ PlayerPrefs.SetInt("Total Score (Best Racer)1", totalScore_racer); }
			
			// package score and attempt to send it to leaderboard
			string data = attString + "&" + pathsForEventString;
			string package = String.Join("#", new String[]{raceEvent.ToString(), att.personalBests[raceEvent].ToString(), att.racerName, data, totalScore.ToString(), totalScore_racer.ToString()});
			StartCoroutine(handleLeaderboardSend(package, raceEvent));
		}
	}
	// attempts to send package containing player's score to leaderboard, storing package in PlayerPrefs if it can't do so
	IEnumerator handleLeaderboardSend(string package, int raceEvent){
		
		//Debug.Log("handleLeaderboardSend()");
		
		// if not logged in, attempt to log in
		if(!PlayFabManager.loggedIn){
			PlayFabManager.loginError = false;
			StartCoroutine(handleLogin(true, false));
			yield return new WaitUntil(() => (PlayFabManager.loggedIn || PlayFabManager.loginError));
		}
		// if still not logged in, add package to PlayerPrefs to be sent later
		if(!PlayFabManager.loggedIn){
			PlayerPrefs.SetString("UnsentScore " + raceEvent.ToString(), package);
			//Debug.Log(" - Not logged in -> saving score locally");
		}
		// else, attempt to send to leaderboard
		else{
			PlayFabManager.leaderboardSent = false;
			PlayFabManager.leaderboardSendFailure = false;
			PlayFabManager.sendLeaderboard(package);
			yield return new WaitUntil(() => (PlayFabManager.leaderboardSent || PlayFabManager.leaderboardSendFailure));
			// on success, delete PlayerPrefs entry for event (if it exists);
			if(PlayFabManager.leaderboardSent){
				PlayerPrefs.DeleteKey("UnsentScore " + raceEvent.ToString());
				//Debug.Log(" - Logged in, leaderboard sent successfully -> deleting playerpref entry");
			}
			// on failure, save package to PlayerPrefs to be sent later
			else if(PlayFabManager.leaderboardSendFailure){
				PlayerPrefs.SetString("UnsentScore " + raceEvent.ToString(), package);
				//Debug.Log(" - Logged in, leaderboard send failure -> saving score locally");
			}
		}
	}
	
	public GameObject loadRacer(string id, int raceEvent, string asTag, bool fromLeaderboard, bool forReplay){
		// -----------------
		GameObject racer = Instantiate(racerPrefab);
		racer.tag = asTag;
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		// -----------------
		if(forReplay){
			id += "_REPLAY";
		}
		
		att.personalBests = new float[4];
		string[] racerInfo;
		string[] pathInfo;
		
		if(!fromLeaderboard){
			racerInfo = PlayerPrefs.GetString(id).Split(':');
			pathInfo = PlayerPrefs.GetString(id+"_pathsForEvent "+raceEvent).Split(':');
		}
		else{
			string[] racerData = PlayFabManager.userRacerData.Split('&');
			racerInfo = racerData[0].Split(':');
			racerInfo[0] = id;
			pathInfo = racerData[1].Split(':');
			/*
			Debug.Log("racerInfo: " + racerData[0]);
			Debug.Log("pathInfo: " + racerData[1]);
			*/
		}
	
		int i;
		// -----------------
		i = 0;
		try{
			att.pathLength = int.Parse(pathInfo[i]); i++;
		}
		catch(Exception e){
			Debug.Log("error loading racer: " + id);
			Debug.Log(e.ToString());
		}
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
		att.armSpeedFlexL = float.Parse(racerInfo[i]); i++;
		att.armSpeedExtendL = float.Parse(racerInfo[i]); i++;
		att.armSpeedFlexR = float.Parse(racerInfo[i]); i++;
		att.armSpeedExtendR = float.Parse(racerInfo[i]); i++;
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
		for (int i = 0; i < numOfBots; i++)
		{
			racers_backEnd.Add(getRandomBot());
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

	
	void doTasks(){
		int currentTask;
		for(int i = 0; i < taskManager.tasks.Count; i++){
			currentTask = taskManager.tasks[i];
			// -----------------
			if(currentTask == TaskManager.SAVE_PLAYER){
				savePlayer(lbUpdate);
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
				if(!hasInitialSetup){
					CharacterCreatorScreen.transform.Find("Back Button").gameObject.SetActive(true);
				}
			}
			else if(currentTask == TaskManager.CLEAR_RACERS_FROM_SCENE){
				clearRacersFromScene();
			}
			else if(currentTask == TaskManager.SAVE_USER_PB){
				saveUserPB(raceManager.raceEvent, raceManager.userPB_time);
			}
			else if(currentTask == TaskManager.SET_USER_RECORD){
				setUserRecord(raceManager.raceEvent, raceManager.userPB_time, raceManager.player.GetComponent<PlayerAttributes>().racerName);
			}
			else if(currentTask == TaskManager.SAVE_DEFAULT_CAMERA_MODES){
				PlayerPrefs.SetInt("Camera Gameplay Mode", cameraController.cameraGameplayMode);
				PlayerPrefs.SetInt("Camera Replay Mode", cameraController.cameraReplayMode);
			}
		}
		taskManager.tasks.Clear();
	}
	
	
	// -----------------// -----------------// -----------------// -----------------// -----------------// -----------------// -----------------
	
	void createRacer(){
		//Debug.Log("CREATE RACER");
		// -----------------
		GameObject newRacer = Instantiate(racerPrefab);
		newRacer.tag = "Player";
	
		newRacer.SetActive(false);
		PlayerAttributes att = newRacer.GetComponent<PlayerAttributes>();
		
		string name = legalizeEnteredName(nameInputField.gameObject.transform.Find("Text").GetComponent<Text>().text);
		
		// random
		att.racerName = name;
		att.id = PlayerAttributes.generateID(att.racerName, PlayFabManager.thisUserDisplayName);
		att.copyAttributesFromOther(previewRacer_creation, "clothing");
		att.copyAttributesFromOther(previewRacer_creation, "body proportions");
		att.setStats(PlayerAttributes.RANDOM);
		att.setAnimations(PlayerAttributes.RANDOM);
		att.pathLength = 0;
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		att.finishTime = -1f;
		att.personalBests = new float[]{-1f, -1f, -1f, -1f};
		// -----------------
		saveRacer(newRacer, -1, new string[]{PLAYABLE_RACER_MEMORY}, false, false);
		Destroy(newRacer);
		// -----------------

		GameObject b = playerSelectButtonList.GetComponent<SelectionListScript>().addButton(att.id, selectedRaceEvent, SelectionButtonScript.playerButtonColor);
		playerSelectButtonList.numSelected = 1;
		playerSelectButtonList.toggleAllOff();
		b.GetComponent<SelectionButtonScript>().toggle();
		
		unlockManager.fillCharacterSlot();
		
		if(!hasInitialSetup){
			PlayerPrefs.SetInt("hasInitialSetup", 1);
		}
	}
	
	void savePlayer(bool sendToLeaderboard){
		//Debug.Log("SAVE PLAYER");
		string id = raceManager.player.GetComponent<PlayerAttributes>().id;
		GameObject b;
		// -----------------
		playerSelectButtonList.removeButton(id);
		saveRacer(raceManager.player, selectedRaceEvent, new string[]{PLAYABLE_RACER_MEMORY, SAVED_RACER_MEMORY}, sendToLeaderboard, false);
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
		this.lbUpdate = false;
	}
	
	
	void loadSelectedPlayer(){
		//Debug.Log("LOAD SELECTED PLAYER");
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
		player = loadRacer(playerID, selectedRaceEvent, "Player (Back End)", false, false);
		player.GetComponent<PlayerAttributes>().setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		player.SetActive(false);
		player.transform.SetParent(raceManager.RacersBackEndParent.transform);
		racers_backEnd.Add(player);
	}
	
	
	void loadSelectedRacers(){
		//Debug.Log("LOAD SELECTED RACERS");
		GameObject b;
		SelectionButtonScript s;
		GameObject grid = ghostSelectButtonList.grid;
		GameObject racer;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.selected){
				racer = loadRacer(s.id, selectedRaceEvent, "Ghost (Back End)", false, false);
				racer.transform.SetParent(raceManager.RacersBackEndParent.transform);
				racers_backEnd.Add(racer);
			}
		}
	}
	
	void setupReplay(){
		int numOfRacers = racers.Count;
		//this.playerIndex = 0;
		// -----------------
		clearListAndObjects(racers_backEnd_replay);
		GameObject racer;
		GameObject racer_replay;
		PlayerAttributes att;
		string id;
		for(int j = 0; j < numOfRacers; j++){
			racer = racers[j];
			att = racer.GetComponent<PlayerAttributes>();
			// add to replay if has a finish time (either a ghost or a bot that's finished)
			if(att.finishTime != -1f){
				id = racer.GetComponent<PlayerAttributes>().id;
				saveRacer(racer, selectedRaceEvent, new string[]{PLAYABLE_RACER_MEMORY}, false, true);
				racer_replay = loadRacer(id, selectedRaceEvent, "Ghost (Back End)", false, true);
				racer_replay.SetActive(false);
				racer_replay.transform.SetParent(raceManager.RacersBackEndParent.transform);
				racer_replay.GetComponent<PlayerAttributes>().lane = racers[j].GetComponent<PlayerAttributes>().lane;
				racers_backEnd_replay.Add(racer_replay);
				forgetRacer(id, new string[]{PLAYABLE_RACER_MEMORY}, true);
				if(racer.tag == "Player"){
					this.playerIndex = racers_backEnd_replay.Count-1;
				}
			}
		}
		//cameraController.GetComponent<CameraController>().setForRaceEvent(selectedRaceEvent);
	}
	
	public void clearRacersFromScene(){
		//Debug.Log("CLEAR RACERS FROM SCENE");
		clearListAndObjects(racers);
		clearListAndObjects(racers_backEnd);
		clearListAndObjects(racers_backEnd_replay);
	}
	
	static void saveUserPB(int raceEvent, float time){
		
		// save pb to playerPrefs
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
		
		//unlockManager.updateUnlocks(raceEvent, time);
	}
	
	public static float getUserPB(int raceEvent){

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
	
	public static void setUserRecord(int raceEvent, float time, string racerName){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			PlayerPrefs.SetString("wr_local_100m", time + ":" + racerName);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			PlayerPrefs.SetString("wr_local_200m", time + ":" + racerName);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			PlayerPrefs.SetString("wr_local_400m", time + ":" + racerName);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			PlayerPrefs.SetString("wr_local_60m", time + ":" + racerName);
		}
	}
	
	public static float getUserRecordTime(int raceEvent){
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
	
	public static string getUserRecordName(int raceEvent){
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
	
	public static void getWorldRecordInfo(int raceEvent){
		PlayFabManager.userLeaderboardInfoRetrieved = false;
		PlayFabManager.getWorldRecordInfo(raceEvent);
	}
	
	public GameObject[] deactivateAllButtons(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Button");
		foreach (GameObject obj in objs) {
			obj.GetComponent<Button>().interactable = false;
		}
		return objs;
     }
	 
	 public void activateButtons(GameObject[] objs){
		foreach (GameObject obj in objs) {
			if(obj != null){
				obj.GetComponent<Button>().interactable = true;
			 }
		}
	 }
	 
	 public static string legalizeEnteredName(string str){
		 str = Regex.Replace(str, @"[^a-zA-Z0-9 ]", "");
		 return str;
	}
	
	public void exitGame(){
		Application.Quit();
	}
	
	// Steam and PlayFab login
	public IEnumerator handleLogin(bool successMsg, bool failMsg){
		//Debug.Log("handleLogin()");
		if(SteamManager.Initialized){
			PlayFabManager.loggedIn = false;
			PlayFabManager.login();
			yield return new WaitUntil(() => (PlayFabManager.loggedIn || PlayFabManager.loginError));
			if(PlayFabManager.loginError){
				if(failMsg){
					string msg = "Could not log in with Steam. Scores will be saved locally.";
					StartCoroutine(showNotificationBlip(msg, 4f));
				}
			}else{
				if(successMsg){
					string msg = "Logged in to Steam!";
					StartCoroutine(showNotificationBlip(msg, 4f));
				}
				// attempt to send unsent scores
				PlayFabManager.setUserDisplayName(steamController.getThisUserSteamName());
				sendUnsentScores();
			}
		}
	}
	void sendUnsentScores(){
		//Debug.Log("sendUnsentScores()");
		string unsentPackage;
		for(int raceEvent = 0; raceEvent < 4; raceEvent++){
			unsentPackage = PlayerPrefs.GetString("UnsentScore " + raceEvent.ToString(), "NA");
			if(unsentPackage != "NA"){
				StartCoroutine(handleLeaderboardSend(unsentPackage, raceEvent));
			}
		}
	}
	
	public IEnumerator showNotificationBlip(string message, float time){
		notificationBlipController.setText(message);
		notificationBlipController.show();
		yield return new WaitForSeconds(time);
		notificationBlipController.hide();
	}
	
	void runFirstTimeOperations(){
		saveUserPB(RaceManager.RACE_EVENT_100M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_200M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_400M, float.MaxValue);
		saveUserPB(RaceManager.RACE_EVENT_60M, float.MaxValue);
		setUserRecord(RaceManager.RACE_EVENT_100M, float.MaxValue, "None");
		setUserRecord(RaceManager.RACE_EVENT_200M, float.MaxValue, "None");
		setUserRecord(RaceManager.RACE_EVENT_400M, float.MaxValue, "None");
		setUserRecord(RaceManager.RACE_EVENT_60M, float.MaxValue, "None");
		PlayerPrefs.SetInt("Total Score1", 0);
		PlayerPrefs.SetInt("Total Score (Best Racer)1", 0);
		PlayerPrefs.SetFloat("Audio Volume", .04f);
		PlayerPrefs.SetInt("Camera Gameplay Mode", CameraController.SIDE_RIGHT);
		PlayerPrefs.SetInt("Camera Replay Mode", CameraController.TV);
		PlayerPrefs.SetFloat("Camera Distance", .5f);
		PlayerPrefs.SetInt("Controls Left", (int)KeyCode.LeftArrow);
		PlayerPrefs.SetInt("Controls Right", (int)KeyCode.RightArrow);
		PlayerPrefs.SetString("Display Mode", "Full screen");
		PlayerPrefs.SetInt("Character Slots", 1);
		PlayerPrefs.SetInt("Downloaded Ghosts", 0);
		PlayerPrefs.SetInt("Theme", EnvironmentController.SUNNY);
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
