using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacersManager : MonoBehaviour
{
	
	public static int PLAYER = 0;
	public static int BOT = 1;
	public static int GHOST = 2;
	
	
	public static int MAX_RACERS = 8;
	
	
	
	
	public GameObject[] pool_players;
	public GameObject[] pool_bots;
	public GameObject[] pool_ghosts;
	
	
	public GameObject player;
	public List<GameObject> bots;
	public List<GameObject> ghosts;
	public List<GameObject> racers;
	public GameObject sampleRacer;
	
	int count_players;
	int count_racers;
	int count_bots;
	int count_ghosts;
	
	
	public GameObject addRacer(int type){
		GameObject racer = null;
		if(type == PLAYER){
			racer = pool_players[0];
			player = racer;
			racers.Add(player);
			count_racers++;
			count_players++;
		}
		else if(type == BOT){
			racer = pool_bots[count_bots];
			// -----------------
			//racer.transform.SetParent(raceManager.RacersBackEndParent.transform);
			racer.SetActive(false);
			PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
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
			bots.Add(racer);
			racers.Add(racer);
			count_racers++;
			count_bots++;
		}
		else if(type == GHOST){
			racer = pool_ghosts[count_ghosts];
			ghosts.Add(racer);
			racers.Add(racer);
			count_racers++;
			count_ghosts++;
		}
		
		if(racer == null){
			Debug.Log("RacersManager: new racer is null");
		}
		return racer;
		
	}
	
	public GameObject addSpecialRacer(int preset, int raceEvent){
		GameObject racer = null;
		if(count_racers < MAX_RACERS){
			racer = pool_ghosts[count_ghosts];
			// -----------------
			//racer.transform.SetParent(raceManager.RacersBackEndParent.transform);
			racer.SetActive(false);
			PlayerAttributes att = racer.GetComponent<PlayerAttributes>();
			att.setInfo(preset);
			att.setClothing(preset);
			att.setBodyProportions(preset);
			att.setStats(preset);
			att.setAnimations(preset);
			att.setPathsFromSpecial(preset, raceEvent);
			// -----------------
			ghosts.Add(racer);
			racers.Add(racer);
			count_racers++;
			count_ghosts++;
		}
		return racer;
	}
	
	
	
	public void removePlayer(){
		if(player != null){
			player = null;
			count_players--;
			count_racers--;
		}
	}
	
	
	public void reset(){
		count_players = 0;
		count_bots = 0;
		count_ghosts = 0;
		count_racers = 0;
		player = null;
		bots.Clear();
		ghosts.Clear();
	}
	
	public void init(){
		bots = new List<GameObject>();
		ghosts = new List<GameObject>();
		racers = new List<GameObject>();
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
