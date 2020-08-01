using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour
{
	
	public static string PLAYABLE_RACER_MEMORY = "PLAYABLE RACER IDS";
	public static string SAVED_RACER_MEMORY = "SAVED RACER IDS";
	
	
	public CameraController gameCamera;
	
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
	
	// racers
	public GameObject racerPrefab;
	public GameObject player_backEnd;
	public int playerIndex;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers_backEnd_replay;
	public List<GameObject> racers;
	
	// scene
	public GameObject startingLine;
	public GameObject finishLine;
	
	// finish screen ghost
	public List<int> checkedRacerIndexes;
	
	
    // Start is called before the first frame update
    void Start()
    {
		racers_backEnd = new List<GameObject>();
		racers_backEnd_replay = new List<GameObject>();
		racers = new List<GameObject>();
		// -----------------
		playerSelectButtonList.init(PLAYABLE_RACER_MEMORY, 1, true);
		playableRacerIDs = PlayerPrefs.GetString("PLAYABLE RACER IDS").Split(':').ToList();
		
		ghostSelectButtonList.init(SAVED_RACER_MEMORY, 7, false);
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
		setCameraFocus(startingLine, CameraController.CAMERA_MODE_CINEMATIC);
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
		StartCoroutine(wait(.01f));
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
		startRace(RaceManager.LIVE_MODE);
	}
	public void startRaceAsReplay(){
		if(raceManager.raceMode != RaceManager.REPLAY_MODE){
			taskManager.addTask(TaskManager.SETUP_REPLAY);
		}
		startRace(RaceManager.REPLAY_MODE);
	}
	public void startRace(int mode){
		if(mode == RaceManager.LIVE_MODE){
			//fillRemainingSpotsWithBots();
		}
		else if(mode == RaceManager.REPLAY_MODE){
			
		}
		goCountDownScreen();
		StartCoroutine(setupRaceWhenReady(mode));
	}
	IEnumerator setupRaceWhenReady(int mode){
		yield return new WaitUntil(() => taskManager.tasks.Count == 0);
		clearListAndObjects(racers);
		clearListAndObjects(raceManager.startingBlocks);
		// --
		int cameraMode = 0;
		if(mode == RaceManager.LIVE_MODE){
			racers = raceManager.setupRace(racers_backEnd, mode);
			cameraMode = CameraController.CAMERA_MODE_SIDESCROLL;
		}
		else if(mode == RaceManager.REPLAY_MODE){
			racers = raceManager.setupRace(racers_backEnd_replay, mode);
			cameraMode = CameraController.CAMERA_MODE_TV;
		}
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
	}
	
	
	
	//-----------------------------------------------------------------------------------------------------------
	public void clearListAndObjects(List<GameObject> list){
		list.Clear();
		foreach(Transform child in raceManager.RacersFieldParent.transform){
			Destroy(child.gameObject);
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
	
	public void saveRacer(GameObject racer, string[] targetMemoryLocations, bool forReplay){
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
		string vY, vZ, pY, pZ, r, l;
		vY = vZ = pY = pZ = r = l = "";
		int length = att.pathLength;
		for(int i = 0; i < length; i++){
			vY += att.velPathY[i].ToString() + ",";
			vZ += att.velPathZ[i].ToString() + ",";
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
		PlayerPrefs.SetString(id,
				id
		+ ":" + att.racerName
		+ ":" + att.finishTime.ToString()
		+ ":" + att.personalBest.ToString()
		+ ":" + att.resultString
		+ ":" + att.POWER.ToString()
		+ ":" + att.TRANSITION_PIVOT_SPEED.ToString()
		+ ":" + att.QUICKNESS.ToString()
		+ ":" + att.KNEE_DOMINANCE.ToString()
		+ ":" + att.TURNOVER.ToString()
		+ ":" + att.pathLength.ToString()
		+ ":" + vY
		+ ":" + vZ
		+ ":" + pY
		+ ":" + pZ
		+ ":" + r
		+ ":" + l
		+ ":" + att.dummyMeshNumber.ToString()
		+ ":" + att.topMeshNumber.ToString()
		+ ":" + att.bottomsMeshNumber.ToString()
		+ ":" + att.shoesMeshNumber.ToString()
		+ ":" + att.socksMeshNumber.ToString()
		+ ":" + att.headbandMeshNumber.ToString()
		+ ":" + att.sleeveMeshNumber.ToString()
		+ ":" + att.dummyMaterialNumber.ToString()
		+ ":" + att.topMaterialNumber.ToString()
		+ ":" + att.bottomsMaterialNumber.ToString()
		+ ":" + att.shoesMaterialNumber.ToString()
		+ ":" + att.socksMaterialNumber.ToString()
		+ ":" + att.headbandMaterialNumber.ToString()
		+ ":" + att.sleeveMaterialNumber.ToString()
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
	
	
	public GameObject loadRacer(string id, string asTag, bool forReplay){
		// -----------------
		GameObject racer = Instantiate(racerPrefab);
		racer.tag = asTag;
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		// -----------------
		if(forReplay){
			id += "_REPLAY";
		}
		string[] racerInfo = PlayerPrefs.GetString(id).Split(':');
		/*
		for(int i = 0; i < racerInfo.Length; i++){
			Debug.Log("racerInfo[" + i + "]: " + racerInfo[i]);
		}
		*/
		int i = 0;
		// -----------------
		att.id = racerInfo[i]; i++;
		att.racerName = racerInfo[i]; i++;
		att.finishTime = float.Parse(racerInfo[i]); i++;
		att.personalBest = float.Parse(racerInfo[i]); i++;
		att.resultString = racerInfo[i]; i++;
		att.POWER = float.Parse(racerInfo[i]); i++;
		att.TRANSITION_PIVOT_SPEED = float.Parse(racerInfo[i]); i++;
		att.QUICKNESS = float.Parse(racerInfo[i]); i++;
		att.KNEE_DOMINANCE = float.Parse(racerInfo[i]); i++;
		att.TURNOVER = float.Parse(racerInfo[i]); i++;
		att.pathLength = int.Parse(racerInfo[i]); i++;
		string[] vY = racerInfo[i].Split(','); i++;
		string[] vZ = racerInfo[i].Split(','); i++;
		string[] pY = racerInfo[i].Split(','); i++;
		string[] pZ = racerInfo[i].Split(','); i++;
		string[] r = racerInfo[i].Split(','); i++;
		string[] l = racerInfo[i].Split(','); i++;
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
			att.velPathY[j] = float.Parse(vY[j]);
			att.velPathZ[j] = float.Parse(vZ[j]);
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
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setStats(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setAnimatorController(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		// -----------------
		return racer;
	}
	
	
	public GameObject loadNewBot(string racerName){
		GameObject bot = Instantiate(racerPrefab);
		bot.tag = "Bot (Back End)";
		bot.transform.SetParent(raceManager.RacersBackEndParent.transform);
		bot.SetActive(false);
		PlayerAttributes att = bot.GetComponent<PlayerAttributes>();
		att.id = PlayerAttributes.generateID(racerName);
		att.racerName = racerName;
		att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
		att.pathLength = PlayerAttributes.DEFAULT_PATH_LENGTH;
		att.setClothing(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setStats(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setAnimatorController(PlayerAttributes.ATTRIBUTES_RANDOM);
		bot.AddComponent<Bot_AI>();
		
		return bot;
	}
	
	public void fillRemainingSpotsWithBots(){
		GameObject bot;
		int count = 8 - racers_backEnd.Count;
		for(int i = 0; i < count; i++){
			bot = loadNewBot("Bot " + (i).ToString());
			bot.GetComponent<PlayerAttributes>().finishTime = -1f;
			bot.GetComponent<PlayerAttributes>().personalBest = -1f;
			racers_backEnd.Add(bot);
		}
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
		att.personalBest = -1f;
		// -----------------
		saveRacer(newRacer, new string[]{PLAYABLE_RACER_MEMORY}, false);
		Destroy(newRacer);
		// -----------------
		GameObject b = playerSelectButtonList.GetComponent<SelectionListScript>().addButton(att.id);
		b.GetComponent<SelectionButtonScript>().toggle();
	}
	
	void savePlayer(){
		Debug.Log("SAVE PLAYER");
		string id = raceManager.player.GetComponent<PlayerAttributes>().id;
		GameObject b;
		// -----------------
		playerSelectButtonList.removeButton(id);
		saveRacer(raceManager.player, new string[]{PLAYABLE_RACER_MEMORY, SAVED_RACER_MEMORY}, false);
		b = playerSelectButtonList.addButton(id);
		b.GetComponent<SelectionButtonScript>().toggle();
		// -----------------
		bool selected = false;
		if(ghostSelectButtonList.getButton(id) != null){
			selected = ghostSelectButtonList.getButton(id).GetComponent<SelectionButtonScript>().selected;
		}
		ghostSelectButtonList.removeButton(id);
		b = ghostSelectButtonList.addButton(id);
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
			saveRacer(racer, new string[]{SAVED_RACER_MEMORY}, false);
			ghostSelectButtonList.addButton(att.id);
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
				player = loadRacer(s.id, "Player (Back End)", false);
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
				racer = loadRacer(s.id, "Ghost (Back End)", false);
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
			saveRacer(racer, new string[]{PLAYABLE_RACER_MEMORY}, true);
			racer_replay = loadRacer(id, "Ghost (Back End)", true);
			racer_replay.SetActive(false);
			racer_replay.transform.SetParent(raceManager.RacersBackEndParent.transform);
			racer_replay.GetComponent<PlayerAttributes>().lane = racers[j].GetComponent<PlayerAttributes>().lane;
			racers_backEnd_replay.Add(racer_replay);
			forgetRacer(id, new string[]{PLAYABLE_RACER_MEMORY}, true);
			if(racer.tag == "Player"){
				playerIndex = j;
			}
		}
	}
	
	void clearRacersFromScene(){
		Debug.Log("CLEAR RACERS FROM SCENE");
		clearListAndObjects(racers);
		clearListAndObjects(racers_backEnd);
		clearListAndObjects(racers_backEnd_replay);
		clearListAndObjects(raceManager.startingBlocks);
	}
	

	public void setCameraFocus(GameObject referenceObject, int cameraMode){
		gameCamera.referenceObject = referenceObject;
		gameCamera.mode = cameraMode;
	}
	
	IEnumerator wait(float t){
		yield return new WaitForSeconds(t);
	}

}
