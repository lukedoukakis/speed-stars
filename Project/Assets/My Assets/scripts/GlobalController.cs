using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GlobalController : MonoBehaviour
{
	
	
	bool write;
	
	
	public static string PLAYABLE_RACER_MEMORY = "PLAYABLE RACER IDS";
	public static string SAVED_RACER_MEMORY = "SAVED RACER IDS";
	
	
	public CameraController gameCamera;
	public AudioSource musicSource;
	
	// UI
	public GameObject canvas;
	public GameObject RaceUI;
		bool RaceUIActive;
	public GameObject StartScreen;
		bool StartScreenActive;
	public GameObject SetupScreen;
		bool SetupScreenActive;
	public GameObject CharacterCreatorScreen;
		public GameObject previewRacer;
		public GameObject newRacerButton;
		bool CharacterCreatorScreenActive;
		public GameObject nameInputField;
	public GameObject CountdownScreen;
		bool CountdownScreenActive;
	public GameObject RaceScreen;
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
	
	// countdown
	public TaskManager taskManager;
	public Countdowner countdowner;
		public Text countdownText;
	public RaceManager raceManager;
	public SetupManager setupManager;
	
	// racers
	public GameObject racerPrefab;
	public GameObject player_backEnd;
	public int playerIndex;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers_backEnd_replay;
	public List<GameObject> racers;
	
	// scene
	public int selectedRaceEvent;
	
	// finish screen ghost
	public List<int> checkedRacerIndexes;
	
	
    // Start is called before the first frame update
    void Start()
    {
		
		write = false;
		
		musicSource.Play(0);
		
		Application.targetFrameRate = 20;
		
		racers_backEnd = new List<GameObject>();
		racers_backEnd_replay = new List<GameObject>();
		racers = new List<GameObject>();
		// -----------------
		playerSelectButtonList.init(RaceManager.RACE_EVENT_100M, PLAYABLE_RACER_MEMORY, 1, 1, true, true);
		playableRacerIDs = PlayerPrefs.GetString("PLAYABLE RACER IDS").Split(':').ToList();
		
		ghostSelectButtonList.init(RaceManager.RACE_EVENT_100M, SAVED_RACER_MEMORY, 7, 0, false, false);
		savedRacerIDs = PlayerPrefs.GetString("SAVED RACER IDS").Split(':').ToList();
		// -----------------
		previewRacer.GetComponent<PlayerAttributes>().renderInForeground();
		previewRacer.GetComponent<PlayerAnimationV2>().setIdle();
		// -----------------
		checkedRacerIndexes = new List<int>();
		// -----------------
		goStartScreen();
    }

    // Update is called once per frame
    void Update()
    {
		
		// debug
		if(Input.GetKeyUp(KeyCode.O)){
			Debug.Log("Deleting PlayerPrefs");
			PlayerPrefs.DeleteAll();
		}
		
		if(Input.GetKeyUp(KeyCode.T)){
			Time.timeScale -= .1f;
		}
		if(Input.GetKeyUp(KeyCode.Y)){
			Time.timeScale += .1f;
		}
		if(Input.GetKeyUp(KeyCode.W)){
			write = true;
		}
		
		
		if(CountdownScreenActive){
			countdownText.text = countdowner.currentString;
			if(countdowner.currentString == "Set"){
				raceManager.raceStatus = RaceManager.STATUS_SET;
			}
			if(countdowner.finished){
				goRaceScreen();
			}
		}
		
		if(RaceUIActive){
			if(Input.GetKeyDown(KeyCode.R)){
				startRaceAsLive();
			}
			if(Input.GetKeyDown(KeyCode.Escape)){
				raceManager.quitRace();
			}
		}
		
    }
	
	public void goStartScreen(){
		//Debug.Log("Going to start screen");
		StartCoroutine(screenTransition("Start Screen", false));
		setCameraFocus(raceManager.startingLines_100m[4], CameraController.CAMERA_MODE_CINEMATIC);
	}
	
	public void goSetupScreen(){
		//Debug.Log("Going to setup screen");
		StartCoroutine(screenTransition("Setup Screen", false));
	}
	
	public void goCharacterCreatorScreen(){
		//Debug.Log("Going to creator screen");
		PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_DEFAULT);
		att.setClothing(PlayerAttributes.ATTRIBUTES_DEFAULT);
		StartCoroutine(screenTransition("Character Creator Screen", false));
	}
	
	public void goCountDownScreen(){
		//Debug.Log("Going to countdown screen");
		StartCoroutine(screenTransition("Countdown Screen", false));
		// -----------------
		countdowner.startCountdown();
	}
	
	public void goRaceScreen(){
		//Debug.Log("Going to race screen");
		StartCoroutine(screenTransition("Race Screen", true));
		// -----------------
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
		RaceScreen.SetActive(false);
			RaceScreenActive = false;
		FinishScreen.SetActive(false);
			FinishScreenActive = false;
		switch (nextScreen) {
			case "Start Screen" :
				StartScreen.SetActive(true);
				StartScreenActive = true;
				break;
			case "Setup Screen" :
				SetupScreen.SetActive(true);
				SetupScreenActive = true;
				SetupScreen.GetComponent<SetupManager>().moveUIElement("center");
				break;	
			case "Character Creator Screen" :
				CharacterCreatorScreen.SetActive(true);
				CharacterCreatorScreenActive = true;
				break;	
			case "Countdown Screen" :
				CountdownScreen.SetActive(true);
				RaceUI.SetActive(true);
				CountdownScreenActive = true;
				RaceUIActive = true;
				break;
			case "Race Screen" :
				RaceScreen.SetActive(true);
				RaceUI.SetActive(true);
				RaceScreenActive = true;
				RaceUIActive = true;
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
	
	//-----------------------------------------------------------------------------------------------------------
	
	public void startRaceAsLive(){
		Time.timeScale = 1f;
		startRace(selectedRaceEvent, RaceManager.VIEW_MODE_LIVE);
	}
	public void startRaceAsReplay(){
		if(raceManager.viewMode != RaceManager.VIEW_MODE_REPLAY){
			taskManager.addTask(TaskManager.SETUP_REPLAY);
		}
		startRace(selectedRaceEvent, RaceManager.VIEW_MODE_REPLAY);
	}
	public void startRace(int raceEvent, int viewMode){
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			//loadBots(setupManager.botCount - raceManager.botCount);
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			
		}
		goCountDownScreen();
		StartCoroutine(setupRaceWhenReady(raceEvent, viewMode));
	}
	IEnumerator setupRaceWhenReady(int raceEvent, int viewMode){
		yield return new WaitUntil(() => taskManager.tasks.Count == 0);
		clearListAndObjects(racers);
		// -----------------
		List<GameObject> backEndRacers = new List<GameObject>();
		int cameraMode = 0;
		// --
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			backEndRacers = racers_backEnd;
			cameraMode = CameraController.CAMERA_MODE_SIDESCROLL;
		}
		else if(viewMode == RaceManager.VIEW_MODE_REPLAY){
			backEndRacers = racers_backEnd_replay;
			cameraMode = CameraController.CAMERA_MODE_TV;
		}
		// --
		racers = raceManager.setupRace(backEndRacers, raceEvent, viewMode);
		setCameraFocus(raceManager.focusRacer, cameraMode);
	}
	//-----------------------------------------------------------------------------------------------------------
	public void showResultsScreen(){
		goFinishScreen();
	}
	public void endRace(){
		if(raceManager.playerPB){
			taskManager.addTask(TaskManager.SAVE_PLAYER);
		}
		gameCamera.setFieldOfView(CameraController.FOV_STANDARD);
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
		string vM, vX, vY, vZ, pX, pY, pZ, r, l, s1P, s2P;
		vM = vX = vY = vZ = pX = pY = pZ = r = l = s1P = s2P = "";
		int pLength = att.pathLength;
		int leanLockTick = att.leanLockTick;
		//Debug.Log("path length: " + pLength);
		for(int i = 0; i < pLength; i++){
			vM += att.velMagPath[i].ToString() + ",";
			vX += att.velPathX[i].ToString() + ",";
			vY += att.velPathY[i].ToString() + ",";
			vZ += att.velPathZ[i].ToString() + ",";
			pX += att.posPathX[i].ToString() + ",";
			pY += att.posPathY[i].ToString() + ",";
			pZ += att.posPathZ[i].ToString() + ",";
			r += att.rightInputPath[i].ToString() + ",";
			l += att.leftInputPath[i].ToString() + ",";
			s1P += att.sphere1Prog[i].ToString() + ",";
			s2P += att.sphere2Prog[i].ToString() + ",";
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
			+ ":" + vX
			+ ":" + vY
			+ ":" + vZ
			+ ":" + pX
			+ ":" + pY
			+ ":" + pZ
			+ ":" + r
			+ ":" + l
			+ ":" + s1P
			+ ":" + s2P
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
		);
	}
	
	public void printAtt(){
		
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
		string[] vX = pathInfo[i].Split(','); i++;
		string[] vY = pathInfo[i].Split(','); i++;
		string[] vZ = pathInfo[i].Split(','); i++;
		string[] pX = pathInfo[i].Split(','); i++;
		string[] pY = pathInfo[i].Split(','); i++;
		string[] pZ = pathInfo[i].Split(','); i++;
		string[] r = pathInfo[i].Split(','); i++;
		string[] l = pathInfo[i].Split(','); i++;
		string[] s1P = pathInfo[i].Split(','); i++;
		string[] s2P = pathInfo[i].Split(','); i++;
		
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
		// -----------------
		
		// paths
		att.setPaths(att.pathLength + 500);
		for(int j = 0; j < att.pathLength; j++){
			att.velMagPath[j] = float.Parse(vM[j]);
			att.velPathX[j] = float.Parse(vX[j]);
			att.velPathY[j] = float.Parse(vY[j]);
			att.velPathZ[j] = float.Parse(vZ[j]);
			att.posPathX[j] = float.Parse(pX[j]);
			att.posPathY[j] = float.Parse(pY[j]);
			att.posPathZ[j] = float.Parse(pZ[j]);
			att.sphere1Prog[j] = float.Parse(s1P[j]);
			att.sphere2Prog[j] = float.Parse(s2P[j]);
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
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setStats(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setAnimatorController(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		// -----------------
		return racer;
	}
	
	
	public GameObject createBot(int preset){
		int usainbolt = PlayerAttributes.ATTRIBUTES_LEGEND_USAINBOLT;
		int michaeljohnson = PlayerAttributes.ATTRIBUTES_LEGEND_MICHAELJOHNSON;
		int yohanblake = PlayerAttributes.ATTRIBUTES_LEGEND_YOHANBLAKE;
		int jesseowens = PlayerAttributes.ATTRIBUTES_LEGEND_JESSEOWENS;
		int waydevanniekerk = PlayerAttributes.ATTRIBUTES_LEGEND_WAYDEVANNIEKERK;
		int reg = preset;
		
		int i = reg;
		
		
		GameObject bot = Instantiate(racerPrefab);
		bot.tag = "Bot (Back End)";
		bot.transform.SetParent(raceManager.RacersBackEndParent.transform);
		bot.SetActive(false);
		PlayerAttributes att = bot.GetComponent<PlayerAttributes>();
		att.racerName = TextReader.getRacerName(i);
		att.id = PlayerAttributes.generateID(att.racerName);
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		att.pathLength = PlayerAttributes.DEFAULT_PATH_LENGTH;
		att.setClothing(i);
		att.setBodyProportions(i);
		att.setStats(i);
		att.setAnimatorController(preset);
		bot.AddComponent<Bot_AI>();
		
		return bot;
	}
	
	public void loadBots(){
		int numOfBots = setupManager.botCount - raceManager.botCount;
		GameObject bot;
		for(int i = 0; i < numOfBots; i++){
			bot = createBot(PlayerAttributes.ATTRIBUTES_RANDOM);
			bot.GetComponent<PlayerAttributes>().finishTime = -1f;
			bot.GetComponent<PlayerAttributes>().personalBests = new float[]{-1f,-1f,-1f,-1f};
			racers_backEnd.Add(bot);
		}
		
		
		/*
		// legends
		GameObject bot;
		int count = _numOfBots;
		for(int i = 4; i < count+4; i++){
			bot = createBot(i);
			bot.GetComponent<PlayerAttributes>().finishTime = -1f;
			bot.GetComponent<PlayerAttributes>().personalBests = new float[]{-1f,-1f,-1f};
			racers_backEnd.Add(bot);
		}
		*/
		
		/*
		// all same bot
		GameObject bot;
		bot = createBot("Bot Clone");
		bot.GetComponent<PlayerAttributes>().finishTime = -1f;
		bot.GetComponent<PlayerAttributes>().personalBests = new float[]{-1f,-1f,-1f};
		int count = 8 - racers_backEnd.Count;
		for(int i = 0; i < count; i++){
			racers_backEnd.Add(bot);
		}
		*/
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
			}
			else if(currentTask == TaskManager.CLEAR_RACERS_FROM_SCENE){
				clearRacersFromScene();
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
		att.copyAttributesFromOther(previewRacer, "body proportions");
		att.copyAttributesFromOther(previewRacer, "clothing");
		string name = nameInputField.gameObject.transform.Find("Text").GetComponent<Text>().text;
		att.id = PlayerAttributes.generateID(name);
		att.racerName = name;
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setAnimatorController(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setStats(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.finishTime = -1f;
		att.personalBests = new float[]{-1f, -1f, -1f, -1f};
		// -----------------
		saveRacer(newRacer, -1, new string[]{PLAYABLE_RACER_MEMORY}, false);
		Destroy(newRacer);
		// -----------------
		GameObject b = playerSelectButtonList.GetComponent<SelectionListScript>().addButton(att.id, selectedRaceEvent);
		b.GetComponent<SelectionButtonScript>().toggle();
	}
	
	void savePlayer(){
		Debug.Log("SAVE PLAYER");
		string id = raceManager.player.GetComponent<PlayerAttributes>().id;
		GameObject b;
		// -----------------
		playerSelectButtonList.removeButton(id);
		saveRacer(raceManager.player, selectedRaceEvent, new string[]{PLAYABLE_RACER_MEMORY, SAVED_RACER_MEMORY}, false);
		b = playerSelectButtonList.addButton(id, selectedRaceEvent);
		b.GetComponent<SelectionButtonScript>().toggle();
		// -----------------
		bool selected = false;
		if(ghostSelectButtonList.getButton(id) != null){
			selected = ghostSelectButtonList.getButton(id).GetComponent<SelectionButtonScript>().selected;
		}
		ghostSelectButtonList.removeButton(id);
		b = ghostSelectButtonList.addButton(id, selectedRaceEvent);
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
			ghostSelectButtonList.addButton(att.id, selectedRaceEvent);
		}	
	}
	
	
	void loadSelectedPlayer(){
		Debug.Log("LOAD SELECTED PLAYER");
		GameObject player;
		GameObject b;
		SelectionButtonScript s;
		GameObject grid = playerSelectButtonList.grid;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.selected){
				player = loadRacer(s.id, selectedRaceEvent, "Player (Back End)", false);
				player.GetComponent<PlayerAttributes>().setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
				player.SetActive(false);
				player.transform.SetParent(raceManager.RacersBackEndParent.transform);
				racers_backEnd.Add(player);
				break;
			}	
		}
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
		gameCamera.GetComponent<CameraController>().setForRaceEvent(selectedRaceEvent);
	}
	
	public void clearRacersFromScene(){
		Debug.Log("CLEAR RACERS FROM SCENE");
		clearListAndObjects(racers);
		clearListAndObjects(racers_backEnd);
		clearListAndObjects(racers_backEnd_replay);
	}
	

	public void setCameraFocus(GameObject referenceObject, int cameraMode){
		gameCamera.referenceObject = referenceObject;
		gameCamera.mode = cameraMode;
	}
	
	IEnumerator wait(float t){
		yield return new WaitForSeconds(t);
	}

}
