using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
	
	[SerializeField] GlobalController gc;
	[SerializeField] PlayFabManager pfm;
	[SerializeField] GameObject grid;
	[SerializeField] GameObject entryPrefab;
	[SerializeField] int selectedRaceEvent;
	public int maxDownloadableGhosts;
	
	public int pageNum;
	public int playerRank;
	
	public TooltipController tooltipController;
	public Text pageNumText;
	public Text yourRankNumText;
	
	public void init(int raceEvent, bool fromPageZero){
		
		pfm.selectedRaceEvent = raceEvent;
		
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
	
	IEnumerator initLeaderboard(int raceEvent, int _pageNum){
		
		pfm.leaderboardLoaded = false;
		pfm.getLeaderboard(raceEvent, 0 + (_pageNum*10), 10 + (_pageNum*10));
		yield return new WaitUntil(() => pfm.leaderboardLoaded);
		
		string ls = pfm.leaderboardString;
		string[] entries = ls.Split(':');
		GameObject entryButton;
		for(int i = 0; i < entries.Length-1; i++){
			string[] entryComps = entries[i].Split('*');
			string playfabId = entryComps[0];
			string placing = entryComps[1];
			string displayName = entryComps[2];
			string score = entryComps[3];
			entryButton = Instantiate(entryPrefab, grid.transform);
			entryButton.GetComponent<LeaderboardEntryButtonController>().init(this, raceEvent, playfabId, int.Parse(placing), displayName, float.Parse(score)/-100f);
		}
		pfm.leaderboardLoaded = false;
	}
	
	IEnumerator getThisUserInfo(){
		pfm.thisUserInfoRetrieved = false;
		pfm.getThisUserLeaderboardInfo();
		yield return new WaitUntil(() => pfm.thisUserInfoRetrieved);
		
		yourRankNumText.text = (pfm.thisUserPosition+1).ToString();
	}
	
	public IEnumerator downloadRacer(int raceEvent, string pfid){
		
		pfm.userLeaderboardInfoRetrieved = false;
		pfm.getLeaderboardEntryInfo(raceEvent, pfid);
		yield return new WaitUntil(() => pfm.userLeaderboardInfoRetrieved);
		
		string id = PlayerAttributes.generateID(pfm.userRacerName);
		GameObject racer = gc.loadRacer(id, raceEvent, "Ghost (Back End)", true, false);
		gc.saveRacer(racer, raceEvent, new string[]{GlobalController.SAVED_RACER_MEMORY}, false, false);
		
		int downloadedGhosts = gc.downloadedGhosts + 1;
		PlayerPrefs.SetInt("Downloaded Ghosts", downloadedGhosts);
		gc.downloadedGhosts = downloadedGhosts;
		updateTooltipText();
		
		GameObject.Destroy(racer);
		Debug.Log("Ghost downloaded");
	
	}
	
	
	public void nextPage(){
		this.pageNum++;
		init(this.selectedRaceEvent, false);
	}
	public void prevPage(){
		if(this.pageNum > 0){
			this.pageNum--;
			init(this.selectedRaceEvent, false);
		}
	}
	
	
	public void updateTooltipText(){
		int d = gc.downloadedGhosts;
		string tooltipText;
		if(d < maxDownloadableGhosts){
			tooltipText = "Click to download ghost";
		}
		else{
			tooltipText = "You have <color=green>" + d + ("/" + maxDownloadableGhosts + "</color> ghosts from online downloaded. Delete some to download more.");
		}
		tooltipController.show();
		tooltipController.setText(tooltipText);
	}
	
	void clearGrid(){
		foreach(Transform entryT in grid.transform){
			GameObject.Destroy(entryT.gameObject);
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
