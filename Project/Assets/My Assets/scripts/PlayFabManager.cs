using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
	
	// user retrieval info
	public bool userLeaderboardInfoRetrieved;
	public string userPlayFabId;
	public string userDisplayName;
	public int userPosition;
	public int userScore;
	public string userRacerData;
	public string userRacerName;
	
	// for loading leaderboard
	public bool leaderboardLoaded;
	public string leaderboardString;
	
	// for loading this user's info
	public bool thisUserInfoRetrieved;
	public string thisUserPlayFabId;
	public string thisUserDisplayName;
	public int thisUserPosition;
	public int thisUserScore;
	
	
	bool gettingUser;
	public int selectedRaceEvent;
	
	
	
	// login user
	public void login(){
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true
		};
		PlayFabClientAPI.LoginWithCustomID(request, onLoginSuccess, onLoginError);
	}
	void onLoginSuccess(LoginResult result){
		this.thisUserPlayFabId = result.PlayFabId;
		Debug.Log("login/account create success");
	}
	
	// delete user
	public void unlinkAccount(){
		
		UnlinkCustomIDRequest request = new UnlinkCustomIDRequest
		{
			CustomId = this.thisUserPlayFabId
		};
		PlayFabClientAPI.UnlinkCustomID(request, onUnlinkAccountSuccess, onUnlinkAccountError);
	}
	void onUnlinkAccountSuccess(UnlinkCustomIDResult result){
		Debug.Log("account unlinked");
	}
	void onUnlinkAccountError(PlayFabError error){
		Debug.Log("error unlinking account");
		error.GenerateErrorReport();
	}
	// -----------------
	
	public void setUserDisplayName(string username){
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError("Error setting display name"));
    }
	
	// -----------------
	
	public void sendLeaderboard(int raceEvent, float time, string racerName, string racerData){
		
		this.selectedRaceEvent = raceEvent;
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
					Value = (int)(time*-100f)
				}
			}
		};
		PlayFabClientAPI.UpdatePlayerStatistics(request, onLeaderboardSend, onUpdateTimeError);
		
		UpdateUserDataRequest request2 = new UpdateUserDataRequest{
			Data = new Dictionary<string, string>{
				{"RacerName_" + leaderboard_event, racerName},
				{"RacerData_" + leaderboard_event, racerData}
			},
			Permission = UserDataPermission.Public
		};
		PlayFabClientAPI.UpdateUserData(request2, onRacerDataSend, onUpdateUserDataError);
	}
	
	// -----------------
	// get leaderboard for specified event, with specified start position and max entries
	public void getLeaderboard(int raceEvent, int startPos, int maxEntries){
		this.selectedRaceEvent = raceEvent;
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
			StartPosition = startPos,
			MaxResultsCount = maxEntries
		};
		PlayFabClientAPI.GetLeaderboard(request, onLeaderboardGet, onLeaderboardGetError);
	}
	// update leaderboardString
	void onLeaderboardGet(GetLeaderboardResult result){
		string resultString = "";
		foreach(var entry in result.Leaderboard){
			resultString += entry.PlayFabId + "*" + entry.Position + "*" + entry.DisplayName + "*" + entry.StatValue + ":";
		}
		this.leaderboardString = resultString;
		//Debug.Log("PlayFabManager: Leaderboard string: " + leaderboardString);
		leaderboardLoaded = true;
	}
	
	// -----------------

	// get info on leaderboard for specified raceEvent for specified PlayFabId
	public void getLeaderboardEntryInfo(int raceEvent, string pfid){
		
		this.selectedRaceEvent = raceEvent;
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
		
		GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest{
			PlayFabId = pfid,
			StatisticName = leaderboard_event,
			MaxResultsCount = 1
		};
		
		PlayFabClientAPI.GetLeaderboardAroundPlayer(request, onLeaderboardEntryGet, onLeaderboardGetError);
	}
	// set info
	void onLeaderboardEntryGet(GetLeaderboardAroundPlayerResult result){
		
		string leaderboard_event = "";
		if(this.selectedRaceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		
		PlayerLeaderboardEntry entry = result.Leaderboard[0];
		this.userPlayFabId = entry.PlayFabId;
		this.userDisplayName = entry.DisplayName;
		this.userPosition = entry.Position;
		this.userScore = entry.StatValue;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
			PlayFabId = entry.PlayFabId,
			//Keys = new List<string>{"RacerData_" + leaderboard_event, "RacerName_" + leaderboard_event}
			Keys = null
		}, titleDataResult => {
			if(titleDataResult.Data == null){
				Debug.Log("No RacerData found");
			}
			try{
				userRacerData = titleDataResult.Data["RacerData_" + leaderboard_event].Value;
				userRacerName = titleDataResult.Data["RacerName_" + leaderboard_event].Value;
			}
			catch(System.Exception e){
				userRacerData = "Not set";
				userRacerName = "Not set";
				Debug.Log(e.ToString());
			}
			userLeaderboardInfoRetrieved = true;
		}, (error) => {
			Debug.Log("Error retrieving RacerData:");
			Debug.Log(error.GenerateErrorReport());
		});
	}
	
	// -----------------
	
	// gets info on this user for current selectedRaceEvent
	public void getThisUserLeaderboardInfo(){
		
		//Debug.Log("PlayFabManager: Getting this user info...");
		
		string leaderboard_event = "";
		if(this.selectedRaceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest{
			PlayFabId = thisUserPlayFabId,
			StatisticName = leaderboard_event,
			MaxResultsCount = 1
		};
		PlayFabClientAPI.GetLeaderboardAroundPlayer(request, onThisUserLeaderboardInfoGet, onLeaderboardGetError);
	}
	// set info
	void onThisUserLeaderboardInfoGet(GetLeaderboardAroundPlayerResult result){
		
		string leaderboard_event = "";
		if(this.selectedRaceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(this.selectedRaceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		
		PlayerLeaderboardEntry entry = result.Leaderboard[0];
		this.thisUserDisplayName = entry.DisplayName;
		this.thisUserPosition = entry.Position;
		this.thisUserScore = entry.StatValue;
		thisUserInfoRetrieved = true;
	}
	// -----------------
	
	// get info on world record holder by loading leaderboard at pos 0 and getting info from that PlayFabId
	public void getWorldRecordInfo(int raceEvent){
		this.selectedRaceEvent = raceEvent;
		StartCoroutine(retrieveWR(raceEvent));
	}
	IEnumerator retrieveWR(int raceEvent){
		leaderboardLoaded = false;
		getLeaderboard(raceEvent, 0, 1);
		yield return new WaitUntil(() => leaderboardLoaded);
		string pfid = leaderboardString.Split('*')[0];
		userLeaderboardInfoRetrieved = false;
		getLeaderboardEntryInfo(raceEvent, pfid);
		yield return new WaitUntil(() => userLeaderboardInfoRetrieved);
	}
	
	
	
	// -----------------
	void onLeaderboardSend(UpdatePlayerStatisticsResult result){
		//Debug.Log("successful leaderboard send");
	}
	void onRacerDataSend(UpdateUserDataResult result){
		//Debug.Log("successful racer data send");
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
