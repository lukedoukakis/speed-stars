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
	
	
	
	public void init(int raceEvent){
		
		selectedRaceEvent = raceEvent;
		clearGrid();

		StartCoroutine(initLeaderboard(raceEvent));
		
	}
	
	IEnumerator initLeaderboard(int raceEvent){
		
		pfm.leaderboardLoaded = false;
		pfm.getLeaderboard(raceEvent);
		yield return new WaitUntil(() => pfm.leaderboardLoaded);
		
		string ls = pfm.leaderboardString;
		string[] entries = ls.Split(':');
		GameObject entryButton;
		for(int i = 0; i < entries.Length-1; i++){
			string[] entryComps = entries[i].Split('-');
			string placing = entryComps[0];
			string playerName = entryComps[1];
			string score = entryComps[2];
			entryButton = Instantiate(entryPrefab, grid.transform);
			entryButton.GetComponent<LeaderboardEntryButtonController>().init(this, raceEvent, int.Parse(placing), playerName, float.Parse(score)/100f);
		}
		pfm.leaderboardLoaded = false;
	}
	
	public IEnumerator downloadRacer(int raceEvent, int leaderboardPos){
		
		pfm.racerDataRetrieved = false;
		pfm.getLeaderboardEntry(raceEvent, leaderboardPos);
		while(pfm.racerDataRetrieved == false){
			yield return null;
		}
		//yield return new WaitUntil(() => pfm.racerDataRetrieved);
		
		//Debug.Log("About to download racer with racerData: " + pfm.racerDataString);
		
		string id = PlayerAttributes.generateID(this.name);
		GameObject racer = gc.loadRacer(id, raceEvent, "Ghost (Back End)", true, false);
		gc.saveRacer(racer, raceEvent, new string[]{GlobalController.SAVED_RACER_MEMORY}, false, false);
		GameObject.Destroy(racer);
		Debug.Log("Ghost downloaded");
		
	
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
