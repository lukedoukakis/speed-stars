using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour
{
	
	public CameraController gameCamera;
	
	// UI
	public GameObject canvas;
	public GameObject StartScreen;
	public GameObject SetupScreen;
	public GameObject CharacterCreatorScreen;
		public GameObject previewCharacter;
		public GameObject nameInputField;
	public GameObject CountdownScreen;
	public GameObject RaceScreen;
	public GameObject FinishScreen;
	public GameObject TransitionScreen;
	public ButtonHandler buttonHandler;
	public SelectionListScript ghostSelectButtonList;
	public SelectionListScript playerSelectButtonList;
	
	// countdown
	public TaskManager taskManager;
	public Countdowner countdowner;
		public Text countdownText;
	public RaceManager raceManager;
	
	// results
	//public Text resultsText;
	//string resultsHeader;
	
	// racers
	public GameObject racerPrefab;
	public GameObject player_backEnd;
	public int playerIndex;
	public List<GameObject> racers_backEnd;
	public List<GameObject> racers;
	
	// scene
	public GameObject startingLine;
	public GameObject finishLine;
	
	// finish screen ghost
	public List<int> checkedRacerIndexes;
	
	
    // Start is called before the first frame update
    void Start()
    {
		PlayerPrefs.DeleteAll();
		// -----------------
		racers_backEnd = new List<GameObject>();
		racers = new List<GameObject>();
		// -----------------
		ghostSelectButtonList.init(true);
		playerSelectButtonList.init(false);
		// -----------------
		setIdle(previewCharacter);
		// -----------------
		checkedRacerIndexes = new List<int>();
		// -----------------
		goStartScreen();
    }

    // Update is called once per frame
    void Update()
    {
		if(CountdownScreen.activeInHierarchy){
			countdownText.text = countdowner.currentString;
			if(countdowner.currentString == "Set"){
				raceManager.raceStatus = RaceManager.STATUS_SET;
			}
			if(countdowner.finished){
				goRaceScreen();
			}
		}
		
    }
	
	public void goStartScreen(){
		//Debug.Log("Going to start screen");
		StartCoroutine(screenTransition("Start Screen", false));
	}
	
	public void goSetupScreen(){
		//Debug.Log("Going to setup screen");
		StartCoroutine(screenTransition("Setup Screen", false));
	}
	
	public void goCharacterCreatorScreen(){
		//Debug.Log("Going to creator screen");
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
		StartScreen.SetActive(false);
		SetupScreen.SetActive(false);
		CharacterCreatorScreen.SetActive(false);
			previewCharacter.SetActive(false);
		CountdownScreen.SetActive(false);
		RaceScreen.SetActive(false);
		FinishScreen.SetActive(false);
		switch (nextScreen) {
			case "Start Screen" :
				StartScreen.SetActive(true);
				break;
			case "Setup Screen" :
				SetupScreen.SetActive(true);
				break;	
			case "Character Creator Screen" :
				CharacterCreatorScreen.SetActive(true);
					previewCharacter.SetActive(true);
				break;	
			case "Countdown Screen" :
				CountdownScreen.SetActive(true);
				break;
			case "Race Screen" :
				RaceScreen.SetActive(true);
				break;
			case "Finish Screen" :
				FinishScreen.SetActive(true);
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
	public void startRace(int mode){
		if(mode == RaceManager.LIVE_MODE){
			fillRemainingSpotsWithBots();
		}
		goCountDownScreen();
		StartCoroutine(setupRaceWhenReady(mode));
	}
	IEnumerator setupRaceWhenReady(int mode){
		yield return new WaitUntil(() => taskManager.tasks.Count == 0);
		clearRacersField();
		racers = raceManager.setupRace(racers_backEnd, mode);
		// --
		int cameraMode = 0;
		if(mode == RaceManager.LIVE_MODE){
			cameraMode = CameraController.CAMERA_MODE_SIDESCROLL;
		}
		else if(mode == RaceManager.REPLAY_MODE){
			cameraMode = CameraController.CAMERA_MODE_TV100M;
		}
		setCameraFocus(raceManager.focusRacer, cameraMode);
	}
	public void startRaceAsLive(){
		startRace(RaceManager.LIVE_MODE);
	}
	public void startRaceAsReplay(){
		if(raceManager.raceMode != RaceManager.REPLAY_MODE){
			taskManager.addTask(TaskManager.SETUP_REPLAY);
		}
		startRace(RaceManager.REPLAY_MODE);
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
	void clearRacersField(){
		racers.Clear();
		foreach(Transform child in raceManager.RacersFieldParent.transform){
			Destroy(child.gameObject);
		}
	}
	void clearRacersBackEnd(){
		racers_backEnd.Clear();
		foreach(Transform child in raceManager.RacersBackEndParent.transform){
			Destroy(child.gameObject);
		}
	}
	
	public void forgetRacer(string id, bool forReplay){
		if(forReplay){
			id += "_REPLAY";
		}
		PlayerPrefs.DeleteKey(id);
	}
	
	public void saveRacer(GameObject racer, bool forReplay){
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		string id = att.id;
		if(forReplay){
			id += "_REPLAY";
		}
		// -----------------
		/*
		if(PlayerPrefs.HasKey(racerName)){
			PlayerPrefs.DeleteKey(racerName);
			//ghostSelectButtonList.removeButton(racerName);
		}
		*/

		// -----------------
		string vY, vZ, pY, pZ, r, l;
		vY = vZ = pY = pZ = r = l = "";
		for(int i = 0; i < att.pathLength; i++){
			vY += att.velPathY[i].ToString() + ",";
			vZ += att.velPathZ[i].ToString() + ",";
			pY += att.posPathY[i].ToString() + ",";
			pZ += att.posPathZ[i].ToString() + ",";
			r += att.rightInputPath[i].ToString() + ",";
			l += att.leftInputPath[i].ToString() + ",";
		}
		// -----------------
		PlayerPrefs.SetString(id,
				id
		+ ":" + att.racerName
		+ ":" + att.finishTime.ToString()
		+ ":" + att.personalBest.ToString()
		+ ":" + att.resultString
		+ ":" + att.POWER_BASE.ToString()
		+ ":" + att.TRANSITION_PIVOT_SPEED.ToString()
		+ ":" + att.QUICKNESS_BASE.ToString()
		+ ":" + att.STRENGTH_BASE.ToString()
		+ ":" + att.BOUNCE_BASE.ToString()
		+ ":" + att.ENDURANCE_BASE.ToString()
		+ ":" + att.ZTILT_MIN.ToString()
		+ ":" + att.ZTILT_MAX.ToString()
		+ ":" + att.HORIZ_BONUS.ToString()
		+ ":" + att.TURNOVER.ToString()
		+ ":" + att.TILT_SPEED.ToString()
		+ ":" + att.pathLength.ToString()
		+ ":" + vY
		+ ":" + vZ
		+ ":" + pY
		+ ":" + pZ
		+ ":" + r
		+ ":" + l
		+ ":" + att.topNumber
		+ ":" + att.bottomsNumber
		+ ":" + att.shoesNumber
		+ ":" + att.dummyV
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
		);
		//Debug.Log(att.headX);
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
		for(int i = 0; i < 23; i++){
			Debug.Log("racerInfo[" + i + "]: " + racerInfo[i]);
		}
		*/
		// -----------------
		att.id = racerInfo[0];
		att.racerName = racerInfo[1];
		att.finishTime = float.Parse(racerInfo[2]);
		att.personalBest = float.Parse(racerInfo[3]);
		att.resultString = racerInfo[4];
		att.POWER_BASE = float.Parse(racerInfo[5]);
		att.TRANSITION_PIVOT_SPEED = float.Parse(racerInfo[6]);
		att.QUICKNESS_BASE = float.Parse(racerInfo[7]);
		att.STRENGTH_BASE = float.Parse(racerInfo[8]);
		att.BOUNCE_BASE = float.Parse(racerInfo[9]);
		att.ENDURANCE_BASE = float.Parse(racerInfo[10]);
		att.ZTILT_MIN = float.Parse(racerInfo[11]);
		att.ZTILT_MAX = float.Parse(racerInfo[12]);
		att.HORIZ_BONUS = float.Parse(racerInfo[13]);
		att.TURNOVER = float.Parse(racerInfo[14]);
		att.TILT_SPEED = float.Parse(racerInfo[15]);
		att.pathLength = int.Parse(racerInfo[16]);
		string[] vY = racerInfo[17].Split(',');
		string[] vZ = racerInfo[18].Split(',');
		string[] pY = racerInfo[19].Split(',');
		string[] pZ = racerInfo[20].Split(',');
		string[] r = racerInfo[21].Split(',');
		string[] l = racerInfo[22].Split(',');
		att.topNumber = int.Parse(racerInfo[23]);
		att.bottomsNumber = int.Parse(racerInfo[24]);
		att.shoesNumber = int.Parse(racerInfo[25]);
		att.dummyV = float.Parse(racerInfo[26]);
		att.headX = float.Parse(racerInfo[27]);
		att.headY = float.Parse(racerInfo[28]);
		att.headZ = float.Parse(racerInfo[29]);
		att.neckX = float.Parse(racerInfo[30]);
		att.neckY = float.Parse(racerInfo[31]);
		att.neckZ = float.Parse(racerInfo[32]);
		att.torsoX = float.Parse(racerInfo[33]);
		att.torsoY = float.Parse(racerInfo[34]);
		att.torsoZ = float.Parse(racerInfo[35]);
		att.armX = float.Parse(racerInfo[36]);
		att.armY = float.Parse(racerInfo[37]);
		att.armZ = float.Parse(racerInfo[38]);
		att.legX = float.Parse(racerInfo[39]);
		att.legY = float.Parse(racerInfo[40]);
		att.legZ = float.Parse(racerInfo[41]);
		// --
		att.setPaths(att.pathLength + 500);
		for(int i = 0; i < att.pathLength; i++){
			att.velPathY[i] = float.Parse(vY[i]);
			att.velPathZ[i] = float.Parse(vZ[i]);
			att.posPathY[i] = float.Parse(pY[i]);
			att.posPathZ[i] = float.Parse(pZ[i]);
			att.rightInputPath[i] = int.Parse(r[i]);
			att.leftInputPath[i] = int.Parse(l[i]);
		}
		// -----------------
		att.setMaterials(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setStats(PlayerAttributes.ATTRIBUTES_FROM_THIS);
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
		att.setMaterials(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_RANDOM);
		att.setStats(PlayerAttributes.ATTRIBUTES_RANDOM);
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
				Debug.Log("SAVE PLAYER");
				string id = raceManager.player.GetComponent<PlayerAttributes>().id;
				GameObject b;
				// -----------------
				playerSelectButtonList.removeButton(id);
				saveRacer(raceManager.player, false);
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
			else if(currentTask == TaskManager.SAVE_SELECTED_RACERS){
				Debug.Log("SAVE SELECTED RACERS");
				int racerIndex;
				GameObject racer;
				PlayerAttributes att;
				for(int j = 0; j < checkedRacerIndexes.Count; j++){
					racerIndex = checkedRacerIndexes[j];
					racer = racers[racerIndex];
					att = racer.GetComponent<PlayerAttributes>();
					saveRacer(racer, false);
					ghostSelectButtonList.addButton(att.id);
				}	
			}
			else if(currentTask == TaskManager.LOAD_SELECTED_PLAYER){
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
			else if(currentTask == TaskManager.LOAD_SELECTED_RACERS){
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
			else if(currentTask == TaskManager.SETUP_REPLAY){
				Debug.Log("SETUP REPLAY");
				int numOfRacers = racers.Count;
				playerIndex = 0;
				// -----------------
				clearRacersBackEnd();
				GameObject racer;
				GameObject racer_replay;
				string id;
				for(int j = 0; j < numOfRacers; j++){
					racer = racers[j];
					id = racer.GetComponent<PlayerAttributes>().id;
					saveRacer(racer, true);
					racer_replay = loadRacer(id, "Ghost (Back End)", true);
					racer_replay.SetActive(false);
					racer_replay.transform.SetParent(raceManager.RacersBackEndParent.transform);
					racer_replay.GetComponent<PlayerAttributes>().lane = racers[j].GetComponent<PlayerAttributes>().lane;
					racers_backEnd.Add(racer_replay);
					forgetRacer(id, true);
					if(racer.tag == "Player"){
						playerIndex = j;
					}
				}
			}
			else if(currentTask == TaskManager.CREATE_RACER){
				Debug.Log("CREATE RACER");
				// -----------------
				GameObject newRacer = Instantiate(racerPrefab);
				newRacer.tag = "Player";
				newRacer.SetActive(false);
				PlayerAttributes att = newRacer.GetComponent<PlayerAttributes>();
				string name = nameInputField.gameObject.transform.Find("Text").GetComponent<Text>().text;
				att.id = PlayerAttributes.generateID(name);
				att.racerName = name;
				att.setPaths(PlayerAttributes.DEFAULT_PATH_LENGTH);
				att.setMaterials(PlayerAttributes.ATTRIBUTES_RANDOM);
				att.setBodyProportions(PlayerAttributes.ATTRIBUTES_RANDOM);
				att.setStats(PlayerAttributes.ATTRIBUTES_RANDOM);
				att.finishTime = -1f;
				att.personalBest = -1f;
				// -----------------
				saveRacer(newRacer, false);
				Destroy(newRacer);
				// -----------------
				GameObject b = playerSelectButtonList.GetComponent<SelectionListScript>().addButton(att.id);
				b.GetComponent<SelectionButtonScript>().toggle();
			}
			else if(currentTask == TaskManager.CLEAR_RACERS_FROM_SCENE){
				Debug.Log("CLEAR RACERS FROM SCENE");
				clearRacersField();
				clearRacersBackEnd();
			}
		}
		taskManager.tasks.Clear();
		checkedRacerIndexes.Clear();
	}
	
	
	
	
	
	
	
	
	public void setCameraFocus(GameObject referenceObject, int cameraMode){
		gameCamera.referenceObject = referenceObject;
		gameCamera.mode = cameraMode;
	}
	
	
	
	
	
	
	void setIdle(GameObject g){
		Animator animator = g.GetComponent<Animator>();
		animator.SetLayerWeight(1,0f);
		animator.SetLayerWeight(2,0f);
		animator.SetLayerWeight(3,0f);
		animator.SetLayerWeight(4,0f);
		animator.SetLayerWeight(5,0f);
		animator.SetLayerWeight(6,0f);
		animator.SetLayerWeight(7,0f);
		animator.SetLayerWeight(8,0f);
		animator.SetLayerWeight(9,0f);
		animator.SetLayerWeight(10,0f);
		animator.SetLayerWeight(11,0f);
		animator.SetLayerWeight(12,0f);
		
	}
	
	
	
	
}
