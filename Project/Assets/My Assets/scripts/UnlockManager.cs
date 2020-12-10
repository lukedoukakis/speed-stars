using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockManager : MonoBehaviour
{
	
	public GlobalController gc;
	[SerializeField] GameObject UnlockWindow;
	
	public static string unlockString;
	public static string rankAchieveString;
	public static string nextRankString;
	public Text rankText;
	public Text nextRankText;
	public Text unlockText;
	
	public float pb_100m;
	public float pb_200m;
	public float pb_400m;
	public float pb_60m;
	
	public int rank_100m;
	public int rank_200m;
	public int rank_400m;
	public int rank_60m;
	public int bestRank;
	public static int NONE = -1;
	public static int NOOB = 0;
	public static int AVERAGE = 1;
	public static int RIVAL = 2;
	public static int STAR = 3;
	public static int SUPERSTAR = 4;
	public static int SUPERNOVA = 5;
	
	public bool unlocked_200m;
	public bool unlocked_400m;
	public bool unlocked_60m;
	
	// -----------------
	public static float SUPERNOVA_TIME_100M = 9.55f;
	public static float SUPERNOVA_TIME_200M = 19f;
	public static float SUPERNOVA_TIME_400M = 43f;
	public static float SUPERNOVA_TIME_60M = 6.25f;
	
	public static float SUPERSTAR_TIME_100M = 9.75f;
	public static float SUPERSTAR_TIME_200M = 19.5f;
	public static float SUPERSTAR_TIME_400M = 43.5f;
	public static float SUPERSTAR_TIME_60M = 6.35f;
	
	public static float STAR_TIME_100M = 10f;
	public static float STAR_TIME_200M = 20f;
	public static float STAR_TIME_400M = 45f;
	public static float STAR_TIME_60M = 6.5f;
	
	public static float RIVAL_TIME_100M = 11f;
	public static float RIVAL_TIME_200M = 22f;
	public static float RIVAL_TIME_400M = 48f;
	public static float RIVAL_TIME_60M = 7f;
	
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
		string eventU = "";
		string charU = "";
		unlockString = "";
		rankAchieveString = "";
		nextRankString = "";
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			if(time <= NOOB_TIME_100M){
				if(rank_100m < NOOB){
					rankAchieveString = "Rank: <color=olive>NOOB</color> (100m)";
					nextRankString = "Next rank: <color=green>AVERAGE - " + AVERAGE_TIME_100M.ToString("F2") + "s</color>";
					if(!unlocked_200m){
						eventU = "<color=purple>Unlocked event: 200m</color>\n";
						unlockEvent(RaceManager.RACE_EVENT_200M);
					}
				}
				if(time <= AVERAGE_TIME_100M){
					if(rank_100m < AVERAGE){
						rankAchieveString = "Rank: <color=green>AVERAGE</color> (100m)";
						nextRankString = "Next rank: <color=blue>RIVAL - " + RIVAL_TIME_100M.ToString("F2") + "s</color>";
						charU += "+1 character slot\n";
						unlockCharacterSlot();
					}
					if(time <= RIVAL_TIME_100M){
						if(rank_100m < RIVAL){
							rankAchieveString = "Rank: <color=blue>RIVAL</color> (100m)";
							nextRankString = "Next rank: <color=red>STAR - " + STAR_TIME_100M.ToString("F2") + "s</color>";
							charU += "+1 character slot\n";
							unlockCharacterSlot();
						}
						if(time <= STAR_TIME_100M){
							if(rank_100m < STAR){
								rankAchieveString = "Rank: <color=red>STAR</color> (100m)";
								nextRankString = "Next rank: <color=orange>SUPERSTAR - " + SUPERSTAR_TIME_100M.ToString("F2") + "s</color>";
								charU += "+1 character slot\n";
								unlockCharacterSlot();
							}
							if(time <= SUPERSTAR_TIME_100M){
								if(rank_100m < SUPERSTAR){
									rankAchieveString = "Rank: <color=orange>SUPERSTAR</color> (100m)";
									nextRankString = "Final rank: <color=magenta>SUPERNOVA - " + SUPERNOVA_TIME_100M.ToString("F2") + "s</color>";
									charU += "+1 character slot\n";
									unlockCharacterSlot();
								}
								if(time <= SUPERNOVA_TIME_100M){
									if(rank_100m < SUPERNOVA){
										rankAchieveString = "Rank: <color=magenta>SUPERNOVA</color> (100m)";
										nextRankString = "You've achieved the highest possible rank. Unbelievable!";
										charU += "+1 character slot\n";
										unlockCharacterSlot();
									}
								}
							}
						}
					}
				}
			}
			pb_100m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			if(time <= NOOB_TIME_200M){
				if(rank_200m < NOOB){
					rankAchieveString = "Rank: <color=olive>NOOB</color> (200m)";
					nextRankString = "Next rank: <color=green>AVERAGE - " + AVERAGE_TIME_200M.ToString("F2") + "s</color>";
				}
				if(time <= AVERAGE_TIME_200M){
					if(rank_200m < AVERAGE){
						rankAchieveString = "Rank: <color=green>AVERAGE</color> (200m)";
						nextRankString = "Next rank: <color=blue>RIVAL - " + RIVAL_TIME_200M.ToString("F2") + "s</color>";
						if(!unlocked_400m){
							eventU = "<color=purple>Unlocked event: 400m</color>\n";
							unlockEvent(RaceManager.RACE_EVENT_400M);
						}
						charU += "+1 character slot\n";
						unlockCharacterSlot();
					}
					if(time <= RIVAL_TIME_200M){
						if(rank_200m < RIVAL){
							rankAchieveString = "Rank: <color=blue>RIVAL</color> (200m)";
							nextRankString = "Next rank: <color=red>STAR - " + STAR_TIME_200M.ToString("F2") + "s</color>";
							charU += "+1 character slot\n";
							unlockCharacterSlot();
						}
						if(time <= STAR_TIME_200M){
							if(rank_200m < STAR){
								rankAchieveString = "Rank: <color=red>STAR</color> (200m)";
								nextRankString = "Next rank: <color=orange>SUPERSTAR - " + SUPERSTAR_TIME_200M.ToString("F2") + "s</color>";
								charU += "+1 character slot\n";
								unlockCharacterSlot();
							}
							if(time <= SUPERSTAR_TIME_200M){
								if(rank_200m < SUPERSTAR){
									rankAchieveString = "Rank: <color=orange>SUPERSTAR</color> (200m)";
									nextRankString = "Final rank: <color=magenta>SUPERNOVA - " + SUPERNOVA_TIME_200M.ToString("F2") + "s</color>";
									charU += "+1 character slot\n";
									unlockCharacterSlot();
								}
								if(time <= SUPERNOVA_TIME_200M){
									if(rank_200m < SUPERNOVA){
										rankAchieveString = "Rank: <color=magenta>SUPERNOVA</color> (200m)";
										nextRankString = "You've achieved the highest possible rank. Unbelievable!";
										charU += "+1 character slot\n";
										unlockCharacterSlot();
									}
								}
							}
						}
					}
				}
			}
			pb_200m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			if(time <= NOOB_TIME_400M){
				if(rank_400m < NOOB){
					rankAchieveString = "Rank: <color=olive>NOOB</color> (400m)";
					nextRankString = "Next rank: <color=green>AVERAGE - " + AVERAGE_TIME_400M.ToString("F2") + "s</color>";
				}
				if(time <= AVERAGE_TIME_400M){
					if(rank_400m < AVERAGE){
						rankAchieveString = "Rank: <color=green>AVERAGE</color> (400m)";
						nextRankString = "Next rank: <color=blue>RIVAL - " + RIVAL_TIME_400M.ToString("F2") + "s</color>";
						charU += "+1 character slot\n";
						unlockCharacterSlot();
					}
					if(time <= RIVAL_TIME_400M){
						if(rank_400m < RIVAL){
							rankAchieveString = "Rank: <color=blue>RIVAL</color> (400m)";
							nextRankString = "Next rank: <color=red>STAR - " + STAR_TIME_400M.ToString("F2") + "s</color>";
							if(!unlocked_60m){
								eventU = "<color=purple>Unlocked event: 60m</color>\n";
								unlockEvent(RaceManager.RACE_EVENT_60M);
							}
							charU += "+1 character slot\n";
							unlockCharacterSlot();
						}
						if(time <= STAR_TIME_400M){
							if(rank_400m < STAR){
								rankAchieveString = "Rank: <color=red>STAR</color> (400m)";
								nextRankString = "Next rank: <color=orange>SUPERSTAR - " + SUPERSTAR_TIME_400M.ToString("F2") + "s</color>";
								charU += "+1 character slot\n";
								unlockCharacterSlot();
							}
							if(time <= SUPERSTAR_TIME_400M){
								if(rank_400m < SUPERSTAR){
									rankAchieveString = "Rank: <color=orange>SUPERSTAR</color> (400m)";
									nextRankString = "Final rank: <color=magenta>SUPERNOVA - " + SUPERNOVA_TIME_400M.ToString("F2") + "s</color>";
									charU += "+1 character slot\n";
									unlockCharacterSlot();
								}
								if(time <= SUPERNOVA_TIME_400M){
									if(rank_400m < SUPERNOVA){
										rankAchieveString = "Rank: <color=magenta>SUPERNOVA</color> (400m)";
										nextRankString = "You've achieved the highest possible rank. Unbelievable!";
										charU += "+1 character slot\n";
										unlockCharacterSlot();
									}
								}
							}
						}
					}
				}
			}
			pb_400m = time;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			if(time <= NOOB_TIME_60M){
				if(rank_60m < NOOB){
					rankAchieveString = "Rank: <color=yellow>NOOB</color> (60m)";
					nextRankString = "Next rank: <color=green>AVERAGE - " + AVERAGE_TIME_60M.ToString("F2") + "s</color>";
				}
				if(time <= AVERAGE_TIME_60M){
					if(rank_60m < AVERAGE){
						rankAchieveString = "Rank: <color=green>AVERAGE</color> (60m)";
						nextRankString = "Next rank: <color=blue>RIVAL - " + RIVAL_TIME_60M.ToString("F2") + "s</color>";
						charU += "+1 character slot\n";
						unlockCharacterSlot();
					}
					if(time <= RIVAL_TIME_60M){
						if(rank_60m < RIVAL){
							rankAchieveString = "Rank: <color=blue>RIVAL</color> (60m)";
							nextRankString = "Next rank: <color=red>STAR - " + STAR_TIME_60M.ToString("F2") + "s</color>";
							charU += "+1 character slot\n";
							unlockCharacterSlot();
						}
						if(time <= STAR_TIME_60M){
							if(rank_60m < STAR){
								rankAchieveString = "Rank: <color=red>STAR</color> (60m)";
								nextRankString = "Next rank: <color=orange>SUPERSTAR - " + SUPERSTAR_TIME_60M.ToString("F2") + "s</color>";
								charU += "+1 character slot\n";
								unlockCharacterSlot();
							}
							if(time <= SUPERSTAR_TIME_60M){
								if(rank_60m < SUPERSTAR){
									rankAchieveString = "Rank: <color=orange>SUPERSTAR</color> (60m)";
									nextRankString = "Final rank: <color=magenta>SUPERNOVA - " + SUPERNOVA_TIME_60M.ToString("F2") + "s</color>";
									charU += "+1 character slot\n";
									unlockCharacterSlot();
								}
								if(time <= SUPERNOVA_TIME_60M){
									if(rank_60m < SUPERNOVA){
										rankAchieveString = "Rank: <color=magenta>SUPERNOVA</color> (60m)";
										nextRankString = "You've achieved the highest possible rank. Unbelievable!";
										charU += "+1 character slot\n";
										unlockCharacterSlot();
									}
								}
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
		
		unlockString = eventU + charU;
		rankText.text = rankAchieveString;
		nextRankText.text = nextRankString;
		unlockText.text = unlockString;
		
		if(unlockString != "" && rankAchieveString != ""){
			show();
		}
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
	
	public string getCurrentRankString(int raceEvent){
		string s = "";
		string t = "";
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			if(rank_100m == NOOB){
				t = AVERAGE_TIME_100M.ToString("F2");
				s = "Rank: <color=yellow>NOOB</color>\nNext rank: <color=green>AVERAGE - " + t + "s</color>";
			}
			else if(rank_100m == AVERAGE){
				t = RIVAL_TIME_100M.ToString("F2");
				s = "Rank: <color=green>AVERAGE</color>\nNext rank: <color=blue>RIVAL - " + t + "s</color>";
			}
			else if(rank_100m == RIVAL){
				t = STAR_TIME_100M.ToString("F2");
				s = "Rank: <color=blue>RIVAL</color>\nNext rank: <color=red>STAR - " + t + "s</color>";
			}
			else if(rank_100m == STAR){
				t = SUPERSTAR_TIME_100M.ToString("F2");
				s = "Rank: <color=red>STAR</color>\nNext rank: <color=orange>SUPERSTAR - " + t + "s</color>";
			}
			else if(rank_100m == SUPERSTAR){
				t = SUPERNOVA_TIME_100M.ToString("F2");
				s = "Rank: <color=orange>SUPERSTAR</color>\nNext rank: <color=purple>SUPERNOVA - " + t + "s</color>";
			}
			else if(rank_100m == SUPERNOVA){
				s = "Rank: <color=purple>SUPERNOVA</color>";
			}
		}	
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			if(rank_200m == NOOB){
				t = AVERAGE_TIME_200M.ToString("F2");
				s = "Rank: <color=yellow>NOOB</color>\nNext rank: <color=green>AVERAGE - " + t + "s</color>";
			}
			else if(rank_200m == AVERAGE){
				t = RIVAL_TIME_200M.ToString("F2");
				s = "Rank: <color=green>AVERAGE</color>\nNext rank: <color=blue>RIVAL - " + t + "s</color>";
			}
			else if(rank_200m == RIVAL){
				t = STAR_TIME_200M.ToString("F2");
				s = "Rank: <color=blue>RIVAL</color>\nNext rank: <color=red>STAR - " + t + "s</color>";
			}
			else if(rank_200m == STAR){
				t = SUPERSTAR_TIME_200M.ToString("F2");
				s = "Rank: <color=red>STAR</color>\nNext rank: <color=orange>SUPERSTAR - " + t + "s</color>";
			}
			else if(rank_200m == SUPERSTAR){
				t = SUPERNOVA_TIME_200M.ToString("F2");
				s = "Rank: <color=orange>SUPERSTAR</color>\nNext rank: <color=purple>SUPERNOVA - " + t + "s</color>";
			}
			else if(rank_200m == SUPERNOVA){
				s = "Rank: <color=purple>SUPERNOVA</color>";
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			if(rank_400m == NOOB){
				t = AVERAGE_TIME_400M.ToString("F2");
				s = "Rank: <color=yellow>NOOB</color>\nNext rank: <color=green>AVERAGE - " + t + "s</color>";
			}
			else if(rank_400m == AVERAGE){
				t = RIVAL_TIME_400M.ToString("F2");
				s = "Rank: <color=green>AVERAGE</color>\nNext rank: <color=blue>RIVAL - " + t + "s</color>";
			}
			else if(rank_400m == RIVAL){
				t = STAR_TIME_400M.ToString("F2");
				s = "Rank: <color=blue>RIVAL</color>\nNext rank: <color=red>STAR - " + t + "s</color>";
			}
			else if(rank_400m == STAR){
				t = SUPERSTAR_TIME_400M.ToString("F2");
				s = "Rank: <color=red>STAR</color>\nNext rank: <color=orange>SUPERSTAR - " + t + "s</color>";
			}
			else if(rank_400m == SUPERSTAR){
				t = SUPERNOVA_TIME_400M.ToString("F2");
				s = "Rank: <color=orange>SUPERSTAR</color>\nNext rank: <color=purple>SUPERNOVA - " + t + "s</color>";
			}
			else if(rank_400m == SUPERNOVA){
				s = "Rank: <color=purple>SUPERNOVA</color>";
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			if(rank_60m == NOOB){
				t = AVERAGE_TIME_60M.ToString("F2");
				s = "Rank: <color=yellow>NOOB</color>\nNext rank: <color=green>AVERAGE - " + t + "s</color>";
			}
			else if(rank_60m == AVERAGE){
				t = RIVAL_TIME_60M.ToString("F2");
				s = "Rank: <color=green>AVERAGE</color>\nNext rank: <color=blue>RIVAL - " + t + "s</color>";
			}
			else if(rank_60m == RIVAL){
				t = STAR_TIME_60M.ToString("F2");
				s = "Rank: <color=blue>RIVAL</color>\nNext rank: <color=red>STAR - " + t + "s</color>";
			}
			else if(rank_60m == STAR){
				t = SUPERSTAR_TIME_60M.ToString("F2");
				s = "Rank: <color=red>STAR</color>\nNext rank: <color=orange>SUPERSTAR - " + t + "s</color>";
			}
			else if(rank_60m == SUPERSTAR){
				t = SUPERNOVA_TIME_60M.ToString("F2");
				s = "Rank: <color=orange>SUPERSTAR</color>\nNext rank: <color=purple>SUPERNOVA - " + t + "s</color>";
			}
			else if(rank_60m == SUPERNOVA){
				s = "Rank: <color=purple>SUPERNOVA</color>";
			}
		}
		return s;
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
				unlockReqList_200m.Add("rank: <color=yellow>NOOB (" + NOOB_TIME_100M.ToString("F2") + "s)</color> for 100m");
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			unlockReqList_400m = new List<string>();
			if(rank_200m < AVERAGE){
				unlockReqList_400m.Add("rank: <color=green>AVERAGE (" + AVERAGE_TIME_200M.ToString("F2") + "s)</color> for 200m");
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			unlockReqList_60m = new List<string>();
			if(rank_400m < AVERAGE){
				unlockReqList_60m.Add("rank: <color=blue>RIVAL (" + RIVAL_TIME_400M.ToString("F2") + "s)</color> for 400m");
			}
		}
	}
	
	// updates unlock requirement list for other things
	public void initUnlockReqList(string thingToUnlock){
		if(thingToUnlock == "Character Slot"){
			unlockReqList_characterSlot = new List<string>();
			if(rank_100m < AVERAGE){
				unlockReqList_characterSlot.Add("rank: <color=green>AVERAGE (" + AVERAGE_TIME_100M.ToString("F2") + "s)</color> for 100m");
			}
			else if(rank_100m < RIVAL){
				unlockReqList_characterSlot.Add("rank: <color=blue>RIVAL (" + RIVAL_TIME_100M.ToString("F2") + "s)</color> for 100m");
			}
			else if(rank_100m < STAR){
				unlockReqList_characterSlot.Add("rank: <color=red>STAR (" + STAR_TIME_100M.ToString("F2") + "s)</color> for 100m");
			}
			if(rank_200m < AVERAGE){
				unlockReqList_characterSlot.Add("rank: <color=green>AVERAGE (" + AVERAGE_TIME_200M.ToString("F2") + "s)</color> for 200m");
			}
			else if(rank_200m < RIVAL){
				unlockReqList_characterSlot.Add("rank: <color=blue>RIVAL (" + RIVAL_TIME_200M.ToString("F2") + "s)</color> for 200m");
			}
			else if(rank_200m < STAR){
				unlockReqList_characterSlot.Add("rank: <color=red>STAR (" + STAR_TIME_200M.ToString("F2") + "s)</color> for 200m");
			}
			if(rank_400m < AVERAGE){
				unlockReqList_characterSlot.Add("rank: <color=green>AVERAGE (" + AVERAGE_TIME_400M.ToString("F2") + "s)</color> for 400m");
			}
			else if(rank_400m < RIVAL){
				unlockReqList_characterSlot.Add("rank: <color=blue>RIVAL (" + RIVAL_TIME_400M.ToString("F2") + "s)</color> for 400m");
			}
			else if(rank_400m < STAR){
				unlockReqList_characterSlot.Add("rank: <color=red>STAR (" + STAR_TIME_400M.ToString("F2") + "s)</color> for 400m");
			}
			if(rank_60m < AVERAGE){
				unlockReqList_characterSlot.Add("rank: <color=green>AVERAGE (" + AVERAGE_TIME_60M.ToString("F2") + "s)</color> for 60m");
			}
			else if(rank_60m < RIVAL){
				unlockReqList_characterSlot.Add("rank: <color=blue>RIVAL (" + RIVAL_TIME_60M.ToString("F2") + "s)</color> for 60m");
			}
			else if(rank_60m < STAR){
				unlockReqList_characterSlot.Add("rank: <color=red>STAR (" + STAR_TIME_60M.ToString("F2") + "s)</color> for 60m");
			}
		}
	}
	
	
	public void updateRanks(){
		if(pb_100m < NOOB_TIME_100M){
			if(pb_100m < AVERAGE_TIME_100M){
				if(pb_100m < RIVAL_TIME_100M){
					if(pb_100m < STAR_TIME_100M){
						if(pb_100m < SUPERSTAR_TIME_100M){
							if(pb_100m < SUPERNOVA_TIME_100M){
								rank_100m = SUPERNOVA;
							}else{ rank_100m = SUPERSTAR;}
						}else{ rank_100m = STAR;}
					}else{ rank_100m = RIVAL;}
				}else{ rank_100m = AVERAGE;}
			}else{ rank_100m = NOOB;}
		}else{ rank_100m = NONE;}
		if(pb_200m < NOOB_TIME_200M){
			if(pb_200m < AVERAGE_TIME_200M){
				if(pb_200m < RIVAL_TIME_200M){
					if(pb_200m < STAR_TIME_200M){
						if(pb_200m < SUPERSTAR_TIME_200M){
							if(pb_200m < SUPERNOVA_TIME_200M){
								rank_200m = SUPERNOVA;
							}else{ rank_200m = SUPERSTAR;}
						}else{ rank_200m = STAR;}
					}else{ rank_200m = RIVAL;}
				}else{ rank_200m = AVERAGE;}
			}else{ rank_200m = NOOB;}
		}else{ rank_200m = NONE;}
		if(pb_400m < NOOB_TIME_400M){
			if(pb_400m < AVERAGE_TIME_400M){
				if(pb_400m < RIVAL_TIME_400M){
					if(pb_400m < STAR_TIME_400M){
						if(pb_400m < SUPERSTAR_TIME_400M){
							if(pb_400m < SUPERNOVA_TIME_400M){
								rank_400m = SUPERNOVA;
							}else{ rank_400m = SUPERSTAR;}
						}else{ rank_400m = STAR;}
					}else{ rank_400m = RIVAL;}
				}else{ rank_400m = AVERAGE;}
			}else{ rank_400m = NOOB;}
		}else{ rank_400m = NONE;}
		if(pb_60m < NOOB_TIME_60M){
			if(pb_60m < AVERAGE_TIME_60M){
				if(pb_60m < RIVAL_TIME_60M){
					if(pb_60m < STAR_TIME_60M){
						if(pb_60m < SUPERSTAR_TIME_60M){
							if(pb_60m < SUPERNOVA_TIME_60M){
								rank_60m = SUPERNOVA;
							}else{ rank_60m = SUPERSTAR;}
						}else{ rank_60m = STAR;}
					}else{ rank_60m = RIVAL;}
				}else{ rank_60m = AVERAGE;}
			}else{ rank_60m = NOOB;}
		}else{ rank_60m = NONE;}
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
	
	public void show(){
		UnlockWindow.SetActive(true);
	}
	public void hide(){
		UnlockWindow.SetActive(false);
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
