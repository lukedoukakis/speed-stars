using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
	
	[SerializeField] Button[] eventButtons;
	[SerializeField] GameObject loadingIcon;
	
	public int leaderboardMode;
	public static int GLOBAL = 0;
	public static int FRIENDS = 1;
	[SerializeField] Text headerText;
	[SerializeField] GameObject noConnectionIndicator;
	
	[SerializeField] GlobalController gc;
	//[SerializeField] PlayFabManager PlayFabManager;
	[SerializeField] GameObject grid;
	[SerializeField] GameObject entryPrefab;
	[SerializeField] int selectedRaceEvent;
	public int maxDownloadableGhosts;
	
	public int pageNum;
	public int playerRank;
	int entriesOnPage;
	
	public TooltipController tooltipController;
	public Text pageNumText;
	public Text yourRankNumText;
	public Button yourRankButton;
	
	public void setLeaderboardMode(int mode){
		this.leaderboardMode = mode;
		if(mode == GLOBAL){
			headerText.text = "Global Leaderboard";
		}else{
			headerText.text = "Friends Leaderboard";
		}
	}
	public void toggleLeaderboardMode(){
		if(this.leaderboardMode == GLOBAL){
			setLeaderboardMode(FRIENDS);
		}else{
			setLeaderboardMode(GLOBAL);
		}
	}
	
	public void init(int raceEvent, bool fromPageZero){
		
		entriesOnPage = 10;
		PlayFabManager.selectedRaceEvent = raceEvent;
		yourRankButton.interactable = false;
			
		// show leaderboard
		selectedRaceEvent = raceEvent;
		clearGrid();
		pageNumText.text = (this.pageNum+1).ToString();
		if(fromPageZero){
			this.pageNum = 0;
		}
		StartCoroutine(initLeaderboard(raceEvent, pageNum));
		
		// get user position
		StartCoroutine(getThisUserInfo());
		
	}
	public void init(int raceEvent){
		init(raceEvent, true);
	}
	public void refresh(){
		init(this.selectedRaceEvent, true);
	}
	IEnumerator initLeaderboard(int raceEvent, int _pageNum){
		
		PlayFabManager.leaderboardLoaded = false;
		noConnectionIndicator.SetActive(false);
		disableEventButtons();
		
		// if not logged in, attempt to log in
		if(!PlayFabManager.loggedIn){
			PlayFabManager.loginError = false;
			StartCoroutine(gc.handleLogin(true, false));
			yield return new WaitUntil(() => (PlayFabManager.loggedIn || PlayFabManager.loginError));
		}
		
		// if still not logged in, show no connection msg
		if(!PlayFabManager.loggedIn){
			noConnectionIndicator.SetActive(true);
		}
		
		// else, attempt to pull leaderboard
		else{
			if(leaderboardMode == GLOBAL){
				PlayFabManager.getLeaderboard(raceEvent, false, 0 + (_pageNum*10), 10 + (_pageNum*10));
			}else{
				PlayFabManager.getLeaderboard(raceEvent, true, 0 + (_pageNum*10), 10 + (_pageNum*10));
			}
			yield return new WaitUntil(() => (PlayFabManager.leaderboardLoaded || PlayFabManager.leaderboardGetError));
		
			// if error getting leaderboard, show error msg
			if(PlayFabManager.leaderboardGetError){
				noConnectionIndicator.SetActive(true);
			}
			
			// else, we out here
			else{
				string ls = PlayFabManager.leaderboardString;
				string[] entries = ls.Split(':');
				GameObject entryButton;
				entriesOnPage = entries.Length;
				for(int i = 0; i < entries.Length-1; i++){
					string[] entryComps = entries[i].Split('*');
					string playfabId = entryComps[0];
					int placing = int.Parse(entryComps[1]);
					string displayName = entryComps[2];
					float score;
					if(raceEvent <= 3){ score = float.Parse(entryComps[3])/-10000f; }
					else{ score = float.Parse(entryComps[3]); }
					string racerName = entryComps[4];
					string date = entryComps[5];
					if(score != 0f){
						entryButton = Instantiate(entryPrefab, grid.transform);
						entryButton.GetComponent<LeaderboardEntryButtonController>().init(this, raceEvent, playfabId, placing + 10*(_pageNum), displayName, score, racerName, date);
					}
				}
			}
			PlayFabManager.leaderboardLoaded = false;
		}
		enableEventButtons();
	}
	IEnumerator getThisUserInfo(){
		PlayFabManager.thisUserInfoRetrieved = false;
		PlayFabManager.getThisUserLeaderboardInfo();
		yield return new WaitUntil(() => PlayFabManager.thisUserInfoRetrieved);
		
		yourRankNumText.text = "#" + (PlayFabManager.thisUserPosition+1).ToString();
		yourRankButton.interactable = true;
	}
	
	public void getLeaderboardAroundThisUser(){
		int pos = PlayFabManager.thisUserPosition;
		int page = (pos+10)/10 - 1;
		this.pageNum = page;
		init(PlayFabManager.selectedRaceEvent);
	}
	
	public IEnumerator downloadRacer(int raceEvent, string pfid){
		
		setEntryButtonsEnabled(false);
		PlayFabManager.userLeaderboardInfoRetrieved = false;
		PlayFabManager.getLeaderboardEntryInfo(raceEvent, pfid);
		yield return new WaitUntil(() => PlayFabManager.userLeaderboardInfoRetrieved);
		
		string id = PlayerAttributes.generateID(PlayFabManager.userRacerName, PlayFabManager.userDisplayName);
		GameObject racer = gc.loadRacer(id, raceEvent, "Ghost (Back End)", true, false);
		PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
		for(int i = 0; i < att.personalBests.Length; i++){
			if(i != raceEvent){
				att.personalBests[i] = -1f;
			}
		}
		gc.saveRacer(racer, raceEvent, new string[]{GlobalController.SAVED_RACER_MEMORY}, false, false);
		GameObject.Destroy(racer);
		
		int downloadedGhosts = gc.downloadedGhosts + 1;
		PlayerPrefs.SetInt("Downloaded Ghosts", downloadedGhosts);
		gc.downloadedGhosts = downloadedGhosts;
		tooltipController.setText("Downloaded!");
		tooltipController.show(3f);
		setEntryButtonsEnabled(true);
		//Debug.Log("Ghost downloaded");
	}
	
	
	public void nextPage(){
		if(entriesOnPage >= 10){
			this.pageNum++;
			init(this.selectedRaceEvent, false);
		}
	}
	public void prevPage(){
		if(this.pageNum > 0){
			this.pageNum--;
			init(this.selectedRaceEvent, false);
		}
	}
	
	void clearGrid(){
		foreach(Transform entryT in grid.transform){
			GameObject.Destroy(entryT.gameObject);
		}
	}
	
	void setEntryButtonsEnabled(bool enabled){
		foreach(Transform child in grid.transform){
			child.gameObject.GetComponent<Button>().interactable = enabled;
		}
	}
	
	void disableEventButtons(){
		loadingIcon.SetActive(true);
		foreach(Button button in eventButtons){
			button.interactable = false;
		}
	}
	void enableEventButtons(){
		loadingIcon.SetActive(false);
		foreach(Button button in eventButtons){
			button.interactable = true;
		}
	}
	
	
	
	
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
