using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
	
	// for loading leaderboard
	public bool leaderboardLoaded;
	public string leaderboardString;
	
	// for retrieving a player data
	public bool racerDataRetrieved;
	public string racerDataString;
	
	public void login(){
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true
		};
		PlayFabClientAPI.LoginWithCustomID(request, onLoginSuccess, onLoginError);
	}
	
	// -----------------
	
	public void sendLeaderboard(int raceEvent, float time, string racerName, string racerData){
		
		string leaderboard_event = "";
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}

		UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest{
			Statistics = new List<StatisticUpdate>{
				new StatisticUpdate{
					StatisticName = leaderboard_event,
					Value = (int)(time*100f)
				}
			}
		};
		PlayFabClientAPI.UpdatePlayerStatistics(request, onLeaderboardSend, onUpdateTimeError);
		
		UpdateUserDataRequest request2 = new UpdateUserDataRequest{
			Data = new Dictionary<string, string>{
				{"RacerName", racerName},
				{"RacerData", racerData}
			}
		};
		PlayFabClientAPI.UpdateUserData(request2, onRacerDataSend, onUpdateUserDataError);
	}
	
	// -----------------
	
	public void getLeaderboard(int raceEvent){
		string leaderboard_event = "";
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		
		GetLeaderboardRequest request = new GetLeaderboardRequest{
			StatisticName = leaderboard_event,
			StartPosition = 0,
			MaxResultsCount = 10
		};
		PlayFabClientAPI.GetLeaderboard(request, onLeaderboardGet, onLeaderboardGetError);
	}
	void onLeaderboardGet(GetLeaderboardResult result){
		string resultString = "";
		foreach(var entry in result.Leaderboard){
			resultString += entry.Position + "-" + entry.PlayFabId + "-" + entry.StatValue + ":";
		}
		this.leaderboardString = resultString;
		Debug.Log("PlayFabManager: Leaderboard string: " + leaderboardString);
		leaderboardLoaded = true;
	}
	
	// -----------------
	// gets truncated leaderboard containing only the entry at the specified position, and gets the racerData of that entry
	public void getLeaderboardEntry(int raceEvent, int position){
		string leaderboard_event = "";
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		
		GetLeaderboardRequest request = new GetLeaderboardRequest{
			StatisticName = leaderboard_event,
			StartPosition = position,
			MaxResultsCount = 2
		};
		PlayFabClientAPI.GetLeaderboard(request, onLeaderboardEntryGet, onLeaderboardGetError);
	}
	// set racerDataString to racerData at first leaderboard entry
	void onLeaderboardEntryGet(GetLeaderboardResult result){
		PlayerLeaderboardEntry entry = result.Leaderboard[0];
		string id = entry.PlayFabId;
		//Debug.Log("PlayFabId: " + id);
		PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
			PlayFabId = id,
			Keys = null
		}, dataResult => {
			if(dataResult.Data == null){
				Debug.Log("No RacerData found");
			}
			racerDataString = dataResult.Data["RacerData"].Value;
			//Debug.Log("racerData: " + racerDataString);
			racerDataRetrieved = true;
		}, (error) => {
			Debug.Log("Error retrieving RacerData:");
			Debug.Log(error.GenerateErrorReport());
		});
	}
	
	// -----------------
	
	void onLoginSuccess(LoginResult result){
		Debug.Log("login/account create success");
	}
	void onLeaderboardSend(UpdatePlayerStatisticsResult result){
		Debug.Log("successful leaderboard send");
	}
	void onRacerDataSend(UpdateUserDataResult result){
		Debug.Log("successful racer data send");
	}
	
	void onLoginError(PlayFabError error){
		Debug.Log("Error logging in/creating account");
		Debug.Log(error.GenerateErrorReport());
	}
	void onLeaderboardGetError(PlayFabError error){
		Debug.Log("Error getting leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	void onUpdateTimeError(PlayFabError error){
		Debug.Log("Error sending time to leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	void onUpdateUserDataError(PlayFabError error){
		Debug.Log("Error sending user data to leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	void onLeaderboardEntryGetError(PlayFabError error){
		Debug.Log("Error getting leaderboard entry");
		Debug.Log(error.GenerateErrorReport());
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
