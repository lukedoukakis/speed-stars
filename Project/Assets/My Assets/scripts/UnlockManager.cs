using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockManager : MonoBehaviour
{
	
	public GlobalController gc;
	
	
	public float pb_100m;
	public float pb_200m;
	public float pb_400m;
	public float pb_60m;
	
	public int rank_100m;
	public int rank_200m;
	public int rank_400m;
	public int rank_60m;
	public int bestRank;
	public static int NOOB = 0;
	public static int AVERAGE = 1;
	public static int STAR = 2;
	public static int ELITE = 3;
	public static int GOD = 4;
	
	public bool unlocked_200m;
	public bool unlocked_400m;
	public bool unlocked_60m;
	
	// -----------------
	public static float GOD_TIME_100M = 9.7f;
	public static float GOD_TIME_200M = 19.3f;
	public static float GOD_TIME_400M = 43.5f;
	public static float GOD_TIME_60M = 6.35f;
	
	public static float ELITE_TIME_100M = 10f;
	public static float ELITE_TIME_200M = 20f;
	public static float ELITE_TIME_400M = 45f;
	public static float ELITE_TIME_60M = 6.5f;
	
	public static float STAR_TIME_100M = 11f;
	public static float STAR_TIME_200M = 22f;
	public static float STAR_TIME_400M = 48f;
	public static float STAR_TIME_60M = 7f;
	
	public static float AVERAGE_TIME_100M = 12f;
	public static float AVERAGE_TIME_200M = 24f;
	public static float AVERAGE_TIME_400M = 52f;
	public static float AVERAGE_TIME_60M = 7.5f;
	
	public static float NOOB_TIME_100M = 15f;
	public static float NOOB_TIME_200M = 30f;
	public static float NOOB_TIME_400M = 60f;
	public static float NOOB_TIME_60M = 10f;
	// -----------------
	public List<string> unlockReqList_200m;
	public List<string> unlockReqList_400m;
	public List<string> unlockReqList_60m;
	public List<string> unlockReqList_characterSlot;
	// -----------------
	public int characterSlots;
	// -----------------
	
	[SerializeField] Button eventSelect_100m;
	[SerializeField] Button eventSelect_200m;
	[SerializeField] Button eventSelect_400m;
	[SerializeField] Button eventSelect_60m;
	[SerializeField] Button newRacer;
	
	// called when user PB's - updates unlocks during runtime
	public void updateUnlocks(int raceEvent, float time){
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			if(time < NOOB_TIME_100M){
				if(!unlocked_200m){
					unlockEvent(RaceManager.RACE_EVENT_200M);
				}
				if(time < AVERAGE_TIME_100M){
					
					if(time < STAR_TIME_100M){
						if(rank_100m < STAR){
							unlockCharacterSlot();
						}
						if(time < ELITE_TIME_100M){
							if(rank_100m < ELITE){
								unlockCharacterSlot();
							}
							if(time < GOD_TIME_100M){
								// todo
							}
						}
					}
				}
			}
			pb_100m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			if(time < NOOB_TIME_200M){
				if(!unlocked_400m){
					unlockEvent(RaceManager.RACE_EVENT_400M);
				}
				if(time < AVERAGE_TIME_200M){
					
					if(time < STAR_TIME_200M){
						if(rank_200m < STAR){
							unlockCharacterSlot();
						}
						if(time < ELITE_TIME_200M){
							if(rank_200m < ELITE){
								unlockCharacterSlot();
							}
							if(time < GOD_TIME_200M){
								// todo
							}
						}
					}
				}
			}
			pb_200m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			if(time < NOOB_TIME_400M){
				if(!unlocked_60m){
					unlockEvent(RaceManager.RACE_EVENT_60M);
				}
				if(time < AVERAGE_TIME_400M){
					
					if(time < STAR_TIME_400M){
						if(rank_400m < STAR){
							unlockCharacterSlot();
						}
						if(time < ELITE_TIME_400M){
							if(rank_400m < ELITE){
								unlockCharacterSlot();
							}
							if(time < GOD_TIME_400M){
								// todo
							}
						}
					}
				}
			}
			pb_400m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			if(time < NOOB_TIME_60M){
				if(time < AVERAGE_TIME_60M){
					if(time < STAR_TIME_60M){
						if(rank_60m < STAR){
							unlockCharacterSlot();
						}
						if(time < ELITE_TIME_60M){
							if(rank_60m < ELITE){
								unlockCharacterSlot();
							}
							if(time < GOD_TIME_60M){
								// todo
							}
						}
					}
				}
			}
			pb_60m = time;
		}
		updateRanks();
		initUnlockReqList(RaceManager.RACE_EVENT_100M);
		initUnlockReqList(RaceManager.RACE_EVENT_200M);
		initUnlockReqList(RaceManager.RACE_EVENT_400M);
		initUnlockReqList(RaceManager.RACE_EVENT_60M);
		initUnlockReqList("Character Slot");
	}
	
	
	void unlockEvent(int raceEvent){
		if(raceEvent == RaceManager.RACE_EVENT_200M){
			eventSelect_200m.interactable = true;
			unlocked_200m = true;
			PlayerPrefs.SetInt("unlocked_event_200m", 1);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			eventSelect_400m.interactable = true;
			unlocked_400m = true;
			PlayerPrefs.SetInt("unlocked_event_400m", 1);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			eventSelect_60m.interactable = true;
			unlocked_60m = true;
			PlayerPrefs.SetInt("unlocked_event_60m", 1);
		}
	
	}
	
	
	
	public void init(int charSlots){
		
		// set event unlock status
		unlocked_200m = true;
		unlocked_400m = true;
		unlocked_60m = true;
		int unlocked;
		unlocked = PlayerPrefs.GetInt("unlocked_event_200m");
		if(unlocked == 0){
			eventSelect_200m.interactable = false;
			unlocked_200m = false;
		}
		unlocked = PlayerPrefs.GetInt("unlocked_event_400m");
		if(unlocked == 0){
			eventSelect_400m.interactable = false;
			unlocked_400m = false;
		}
		unlocked = PlayerPrefs.GetInt("unlocked_event_60m");
		if(unlocked == 0){
			eventSelect_60m.interactable = false;
			unlocked_60m = false;
		}
		
		// set event ranks
		pb_100m = gc.getUserPB(RaceManager.RACE_EVENT_100M);
		pb_200m = gc.getUserPB(RaceManager.RACE_EVENT_200M);
		pb_400m = gc.getUserPB(RaceManager.RACE_EVENT_400M);
		pb_60m = gc.getUserPB(RaceManager.RACE_EVENT_60M);
		updateRanks();
		
		// set unlock strings (for tooltip)
		initUnlockReqList(RaceManager.RACE_EVENT_100M);
		initUnlockReqList(RaceManager.RACE_EVENT_200M);
		initUnlockReqList(RaceManager.RACE_EVENT_400M);
		initUnlockReqList(RaceManager.RACE_EVENT_60M);
		initUnlockReqList("Character Slot");
		
		// initialize character slots
		characterSlots = charSlots;
		if(characterSlots >= 1){
			newRacer.interactable = true;
		}
		else{
			newRacer.interactable = false;
		}
		
		
	}
	
	// updates unlock requirement list for race events
	public void initUnlockReqList(int raceEvent){
		if(raceEvent == RaceManager.RACE_EVENT_200M){
			unlockReqList_200m = new List<string>();
			if(rank_100m < AVERAGE){
				unlockReqList_200m.Add("<color=green>" + NOOB_TIME_100M.ToString("F2") + "s</color> for 100m");
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			unlockReqList_400m = new List<string>();
			if(rank_200m < AVERAGE){
				unlockReqList_400m.Add("<color=green>" + NOOB_TIME_200M.ToString("F2") + "s</color> for 200m");
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			unlockReqList_60m = new List<string>();
			if(rank_400m < AVERAGE){
				unlockReqList_60m.Add("<color=green>" + NOOB_TIME_400M.ToString("F2") + "s</color> for 400m");
			}
		}
	}
	
	// updates unlock requirement list for other things
	public void initUnlockReqList(string thingToUnlock){
		if(thingToUnlock == "Character Slot"){
			unlockReqList_characterSlot = new List<string>();
			if(rank_100m < STAR){
				unlockReqList_characterSlot.Add("<color=blue>" + STAR_TIME_100M.ToString("F2") + "s</color> for 100m");
			}
			else if(rank_100m < ELITE){
				unlockReqList_characterSlot.Add("<color=red>" + ELITE_TIME_100M.ToString("F2") + "s</color> for 100m");
			}
			if(rank_200m < STAR){
				unlockReqList_characterSlot.Add("<color=blue>" + STAR_TIME_200M.ToString("F2") + "s</color> for 200m");
			}
			else if(rank_200m < ELITE){
				unlockReqList_characterSlot.Add("<color=red>" + ELITE_TIME_200M.ToString("F2") + "s</color> for 200m");
			}
			if(rank_400m < STAR){
				unlockReqList_characterSlot.Add("<color=blue>" + STAR_TIME_400M.ToString("F2") + "s</color> for 400m");
			}
			else if(rank_400m < ELITE){
				unlockReqList_characterSlot.Add("<color=red>" + ELITE_TIME_400M.ToString("F2") + "s</color> for 400m");
			}
			if(rank_60m < STAR){
				unlockReqList_characterSlot.Add("<color=blue>" + STAR_TIME_60M.ToString("F2") + "s</color> for 60m");
			}
			else if(rank_60m < ELITE){
				unlockReqList_characterSlot.Add("<color=red>" + ELITE_TIME_60M.ToString("F2") + "s</color> for 60m");
			}
		}
	}
	
	
	public void updateRanks(){
		if(pb_100m < AVERAGE_TIME_100M){
			if(pb_100m < STAR_TIME_100M){
				if(pb_100m < ELITE_TIME_100M){
					if(pb_100m < GOD_TIME_100M){
						rank_100m = GOD;
					}else{ rank_100m = ELITE;}
				}else{ rank_100m = STAR;}
			}else{ rank_100m = AVERAGE;}
		}else{ rank_100m = NOOB;}
		if(pb_200m < AVERAGE_TIME_200M){
			if(pb_200m < STAR_TIME_200M){
				if(pb_200m < ELITE_TIME_200M){
					if(pb_200m < GOD_TIME_200M){
						rank_200m = GOD;
					}else{ rank_200m = ELITE;}
				}else{ rank_200m = STAR;}
			}else{ rank_200m = AVERAGE;}
		}else{ rank_200m = NOOB;}
		if(pb_400m < AVERAGE_TIME_400M){
			if(pb_400m < STAR_TIME_400M){
				if(pb_400m < ELITE_TIME_400M){
					if(pb_400m < GOD_TIME_400M){
						rank_400m = GOD;
					}else{ rank_400m = ELITE;}
				}else{ rank_400m = STAR;}
			}else{ rank_400m = AVERAGE;}
		}else{ rank_400m = NOOB;}
		if(pb_60m < AVERAGE_TIME_60M){
			if(pb_60m < STAR_TIME_60M){
				if(pb_60m < ELITE_TIME_60M){
					if(pb_60m < GOD_TIME_60M){
						rank_60m = GOD;
					}else{ rank_60m = ELITE;}
				}else{ rank_60m = STAR;}
			}else{ rank_60m = AVERAGE;}
		}else{ rank_60m = NOOB;}
		bestRank = Mathf.Max(Mathf.Max(rank_100m,rank_200m),Mathf.Max(rank_400m,rank_60m));
	}
	
	public void unlockCharacterSlot(){
		characterSlots++;
		if(characterSlots >= 1){
			PlayerPrefs.SetInt("Character Slots", characterSlots);
		}
		newRacer.interactable = true;
	}
	
	public void fillCharacterSlot(){
		characterSlots--;
		PlayerPrefs.SetInt("Character Slots", characterSlots);
		if(characterSlots < 1){
			newRacer.interactable = false;
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
