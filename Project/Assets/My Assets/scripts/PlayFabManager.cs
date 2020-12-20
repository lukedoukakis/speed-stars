using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;

public class PlayFabManager : MonoBehaviour
{
	
	public SteamController steamController;
	
	// login info
	public static bool loggedIn;
	
	// user retrieval info
	public static bool userLeaderboardInfoRetrieved;
	public static string userPlayFabId;
	public static string userDisplayName;
	public static int userPosition;
	public static int userScore;
	public static string userRacerData;
	public static string userRacerName;
	public static string userDate;
	public static string userTotalScore;
	
	// for sending and loading leaderboard
	public static bool leaderboardSent;
	public static bool leaderboardLoaded;
	public static string leaderboardString;
	
	// for loading this user's info
	public static bool thisUserInfoRetrieved;
	public static string thisUserPlayFabId;
	public static string thisUserDisplayName;
	public static int thisUserPosition;
	public static int thisUserScore;
	
	// error flags
	public static bool loginError;
	public static bool leaderboardGetError;
	public static bool leaderboardSendFailure;
	
	
	static bool gettingUser;
	public static int selectedRaceEvent;
	
	static PlayFabManager instance;
	
	// login user
	public static void login(){
		/*
		// custom id login
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			//CustomId = loginID,
			CreateAccount = true
		};
		PlayFabClientAPI.LoginWithCustomID(request, onLoginSuccess, onLoginError);
		*/
		
		// Steam login
		if(SteamManager.Initialized) {
            // Execute PlayFab API call to log in with steam ticket
			loginError = false;
            PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest {
                CreateAccount = true,
                SteamTicket = getSteamAuthTicket()
            }, onLoginSuccess, onLoginError);
        }
		
		
	}
	static void onLoginSuccess(LoginResult result){
		thisUserPlayFabId = result.PlayFabId;
		loggedIn = true;
		Debug.Log("login/account create success");
	}
	
	public static string getSteamAuthTicket() {
        byte[] ticketBlob = new byte[1024];
        uint ticketSize;

        // Retrieve ticket; hTicket should be a field in the class so you can use it to cancel the ticket later
        // When you pass an object, the object can be modified by the callee. This function modifies the byte array you've passed to it.
        HAuthTicket hTicket = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, out ticketSize);

        // Resize the buffer to actual length
        Array.Resize(ref ticketBlob, (int)ticketSize);

        // Convert bytes to string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in ticketBlob) {
            sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }
	
	// delete user
	public static void unlinkAccount(){
		
		UnlinkCustomIDRequest request = new UnlinkCustomIDRequest
		{
			CustomId = thisUserPlayFabId
		};
		PlayFabClientAPI.UnlinkCustomID(request, onUnlinkAccountSuccess, onUnlinkAccountError);
	}
	static void onUnlinkAccountSuccess(UnlinkCustomIDResult result){
		Debug.Log("account unlinked");
	}
	static void onUnlinkAccountError(PlayFabError error){
		Debug.Log("error unlinking account");
		error.GenerateErrorReport();
	}
	// -----------------
	
	public static void setUserDisplayName(string username){
		if(loggedIn){
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = username
			}, result =>
			{
				string name = result.DisplayName;
				if(name.Length < 3){
					name.PadRight(3-name.Length);
				}
				else if(name.Length > 25){
					name = name.Substring(0, 25);
				}
				thisUserDisplayName = name;
				Debug.Log("The player's display name is now: " + name);
			}, error => Debug.LogError("Error setting display name"));
		}
    }
	
	// -----------------
	public static void sendLeaderboard(string packagedData){
		leaderboardSendFailure = false;
		leaderboardSent = false;
		string[] data = packagedData.Split('#');
		int raceEvent = int.Parse(data[0]);
		float time = float.Parse(data[1]);
		string racerName = data[2];
		string racerData = data[3];
		int totalScore = int.Parse(data[4]);
		int totalScore_racer = int.Parse(data[5]);
	
		if(loggedIn){
			selectedRaceEvent = raceEvent;
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
			
			
			List<StatisticUpdate> statistics = new List<StatisticUpdate>{ new StatisticUpdate{StatisticName = leaderboard_event, Value = (int)(time*-10000f)} };
			if(totalScore != -1){
				statistics.Add(new StatisticUpdate{ StatisticName = "Total Score", Value = totalScore });
			}
			if(totalScore_racer != -1){
				statistics.Add(new StatisticUpdate{ StatisticName = "Total Score (Best Racer)", Value = totalScore_racer });
			}
			UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest{
				Statistics = statistics
			};
			PlayFabClientAPI.UpdatePlayerStatistics(request, onLeaderboardSend, onLeaderboardSendError);
			
			Dictionary<string, string> dataDic = new Dictionary<string, string>{ {"RacerName_" + leaderboard_event, racerName},{"RacerData_" + leaderboard_event, racerData}, {"Date_" + leaderboard_event, DateGetter.getDate()}};
			if(totalScore != -1){
				dataDic.Add("UserTotalScore", totalScore.ToString());
				dataDic.Add("Date_UserTotalScore", DateGetter.getDate());
			}
			if(totalScore_racer != -1){
				dataDic.Add("RacerName_Total Score (Best Racer)", racerName);
				dataDic.Add("Date_Total Score (Best Racer)", DateGetter.getDate());
			}
			UpdateUserDataRequest request2 = new UpdateUserDataRequest{
				Data = dataDic,
				Permission = UserDataPermission.Public
			};
			PlayFabClientAPI.UpdateUserData(request2, onRacerDataSend, onUpdateUserDataError);
		}
	}
	
	// -----------------
	// get leaderboard for specified event, with specified start position and max entries
	public static void getLeaderboard(int raceEvent, bool friendsOnly, int startPos, int maxEntries){
		if(loggedIn){
			selectedRaceEvent = raceEvent;
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
			else if(raceEvent == 4){
				leaderboard_event = "Total Score";
			}
			else if(raceEvent == 5){
				leaderboard_event = "Total Score (Best Racer)";
			}
			
			if(friendsOnly){
				GetFriendLeaderboardRequest request = new GetFriendLeaderboardRequest{
					StatisticName = leaderboard_event,
					StartPosition = startPos,
					MaxResultsCount = maxEntries
				};
				PlayFabClientAPI.GetFriendLeaderboard(request, instance.onLeaderboardGet, onLeaderboardGetError);
			}
			else{
				GetLeaderboardRequest request = new GetLeaderboardRequest{
					StatisticName = leaderboard_event,
					StartPosition = startPos,
					MaxResultsCount = maxEntries
				};
				PlayFabClientAPI.GetLeaderboard(request, instance.onLeaderboardGet, onLeaderboardGetError);
			}
		}
	}
	// update leaderboardString
	void onLeaderboardGet(GetLeaderboardResult result){
		StartCoroutine(generateLeaderboardResultString(result));
	}
	IEnumerator generateLeaderboardResultString(GetLeaderboardResult result){
		string resultString = "";
		string data_racerName = "";
		string data_date = "";
		bool dataRetrieved;
		foreach(var entry in result.Leaderboard){
			
			// get relevant player data
			dataRetrieved = false;
			string leaderboard_event = "";
			if(selectedRaceEvent == RaceManager.RACE_EVENT_100M){
				leaderboard_event = "100m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_200M){
				leaderboard_event = "200m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_400M){
				leaderboard_event = "400m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_60M){
				leaderboard_event = "60m";
			}
			else if(selectedRaceEvent == 4){
				leaderboard_event = "Total Score";
			}
			else if(selectedRaceEvent == 5){
				leaderboard_event = "Total Score (Best Racer)";
			}
			PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
				PlayFabId = entry.PlayFabId,
				//Keys = new List<string>{"RacerName_" + leaderboard_event}
				Keys = null
			}, titleDataResult => {
				if(titleDataResult.Data != null){
					try{
						if(selectedRaceEvent <= 3){
							data_racerName = titleDataResult.Data["RacerName_" + leaderboard_event].Value;
							data_date = titleDataResult.Data["Date_" + leaderboard_event].Value;
						}
						else{
							if(selectedRaceEvent == 5){
								data_racerName = titleDataResult.Data["RacerName_" + leaderboard_event].Value;
								data_date = titleDataResult.Data["Date_" + leaderboard_event].Value;
							}
							else{
								data_racerName = string.Join(", ", titleDataResult.Data["RacerName_100m"].Value, titleDataResult.Data["RacerName_200m"].Value, titleDataResult.Data["RacerName_400m"].Value, titleDataResult.Data["RacerName_60m"].Value);
								data_date = titleDataResult.Data["Date_UserTotalScore"].Value;
							}
						}
					}
					catch(System.Exception e){
						Debug.Log(e.ToString());
					}
					dataRetrieved = true;
				}
			}, (error) => {
				Debug.Log("Error retrieving RacerData:");
				Debug.Log(error.GenerateErrorReport());
			});
			
			yield return new WaitUntil(() => dataRetrieved);
			// set result string to leaderboard info, appending relevant player data if it was obtained
			resultString += entry.PlayFabId + "*" + entry.Position + "*" + entry.DisplayName + "*" + entry.StatValue;
			resultString += "*" + data_racerName + "*" + data_date;
			resultString += ":";
		}

		leaderboardString = resultString;
		leaderboardLoaded = true;
		leaderboardGetError = false;
	}
	
	// -----------------

	// get info on leaderboard for specified raceEvent for specified PlayFabId
	public static void getLeaderboardEntryInfo(int raceEvent, string pfid){
		
		if(loggedIn){
			selectedRaceEvent = raceEvent;
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
			else if(raceEvent == 4){
				leaderboard_event = "Total Score";
			}
			else if(raceEvent == 5){
				leaderboard_event = "Total Score (Best Racer)";
			}
		
			GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest{
				PlayFabId = pfid,
				StatisticName = leaderboard_event,
				MaxResultsCount = 1
			};	
		
			PlayFabClientAPI.GetLeaderboardAroundPlayer(request, onLeaderboardEntryGet, onLeaderboardGetError);
		}
	}
	// set info
	static void onLeaderboardEntryGet(GetLeaderboardAroundPlayerResult result){
		
		string leaderboard_event = "";
		if(selectedRaceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		else if(selectedRaceEvent == 4){
			leaderboard_event = "Total Score";
		}
		else if(selectedRaceEvent == 5){
			leaderboard_event = "Total Score (Best Racer)";
		}
		
		PlayerLeaderboardEntry entry = result.Leaderboard[0];
		userPlayFabId = entry.PlayFabId;
		userDisplayName = entry.DisplayName;
		userPosition = entry.Position;
		userScore = entry.StatValue;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
			PlayFabId = entry.PlayFabId,
			//Keys = new List<string>{"RacerData_" + leaderboard_event, "RacerName_" + leaderboard_event}
			Keys = null
		}, titleDataResult => {
			if(titleDataResult.Data == null){
				Debug.Log("No RacerData found");
			}
			try{
				if(selectedRaceEvent <= 3){
					userRacerData = titleDataResult.Data["RacerData_" + leaderboard_event].Value;
					userRacerName = titleDataResult.Data["RacerName_" + leaderboard_event].Value;
					userDate = titleDataResult.Data["Date_" + leaderboard_event].Value;
				}
				else{
					if(selectedRaceEvent == 4){
						userRacerData = "NA";
						userRacerName = "NA";
						userDate = titleDataResult.Data["Date_UserTotalScore"].Value;
					}
					else{
						userRacerData = "NA";
						userRacerName = titleDataResult.Data["RacerName_" + leaderboard_event].Value;
						userDate = titleDataResult.Data["Date_" + leaderboard_event].Value;
					}
				}
				//userTotalScore = titleDataResult.Data["UserTotalScore"].Value;   --not found in dictionary
				
				//Debug.Log("userRacerData: " + userRacerData);
			}
			catch(System.Exception e){
				userRacerData = "Not found";
				userRacerName = "Not found";
				userDate = "Not found";
				userTotalScore = "Not found";
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
	public static void getThisUserLeaderboardInfo(){
		
		if(loggedIn){
		
			//Debug.Log("PlayFabManager: Getting this user info...");
		
			string leaderboard_event = "";
			if(selectedRaceEvent == RaceManager.RACE_EVENT_100M){
				leaderboard_event = "100m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_200M){
				leaderboard_event = "200m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_400M){
				leaderboard_event = "400m";
			}
			else if(selectedRaceEvent == RaceManager.RACE_EVENT_60M){
				leaderboard_event = "60m";
			}
			else if(selectedRaceEvent == 4){
				leaderboard_event = "Total Score";
			}
			else if(selectedRaceEvent == 5){
				leaderboard_event = "Total Score (Best Racer)";
			}
			GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest{
				PlayFabId = thisUserPlayFabId,
				StatisticName = leaderboard_event,
				MaxResultsCount = 1
			};
			PlayFabClientAPI.GetLeaderboardAroundPlayer(request, onThisUserLeaderboardInfoGet, onLeaderboardGetError);
		}
	}
	// set info
	static void onThisUserLeaderboardInfoGet(GetLeaderboardAroundPlayerResult result){
		
		string leaderboard_event = "";
		if(selectedRaceEvent == RaceManager.RACE_EVENT_100M){
			leaderboard_event = "100m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_200M){
			leaderboard_event = "200m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_400M){
			leaderboard_event = "400m";
		}
		else if(selectedRaceEvent == RaceManager.RACE_EVENT_60M){
			leaderboard_event = "60m";
		}
		else if(selectedRaceEvent == 4){
			leaderboard_event = "Total Score";
		}
		else if(selectedRaceEvent == 4){
			leaderboard_event = "Total Score (Best Racer)";
		}
		
		PlayerLeaderboardEntry entry = result.Leaderboard[0];
		thisUserDisplayName = entry.DisplayName;
		thisUserPosition = entry.Position;
		thisUserScore = entry.StatValue;
		thisUserInfoRetrieved = true;
	}
	// -----------------
	
	// get info on world record holder by loading leaderboard at pos 0 and getting info from that PlayFabId
	public static void getWorldRecordInfo(int raceEvent){
		if(loggedIn){
			selectedRaceEvent = raceEvent;
			instance.StartCoroutine(retrieveWR(raceEvent));
		}
	}
	static IEnumerator retrieveWR(int raceEvent){
		leaderboardLoaded = false;
		getLeaderboard(raceEvent, false, 0, 1);
		yield return new WaitUntil(() => leaderboardLoaded);
		string pfid = leaderboardString.Split('*')[0];
		userLeaderboardInfoRetrieved = false;
		getLeaderboardEntryInfo(raceEvent, pfid);
		yield return new WaitUntil(() => userLeaderboardInfoRetrieved);
	}
	
	
	
	// -----------------
	static void onLeaderboardSend(UpdatePlayerStatisticsResult result){
		leaderboardSent = true;
		//Debug.Log("successful leaderboard send");
	}
	static void onRacerDataSend(UpdateUserDataResult result){
		//Debug.Log("successful racer data send");
	}
	
	static void onLoginError(PlayFabError error){
		loggedIn = false;
		loginError = true;
		Debug.Log("Error logging in/creating account");
		Debug.Log(error.GenerateErrorReport());
	}
	static void onLeaderboardGetError(PlayFabError error){
		loggedIn = false;
		leaderboardGetError = true;
		Debug.Log("Error getting leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	static void onLeaderboardSendError(PlayFabError error){
		loggedIn = false;
		leaderboardSendFailure = true;
		Debug.Log("Error sending time to leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	static void onUpdateUserDataError(PlayFabError error){
		loggedIn = false;
		Debug.Log("Error sending user data to leaderboard");
		Debug.Log(error.GenerateErrorReport());
	}
	static void onLeaderboardEntryGetError(PlayFabError error){
		loggedIn = false;
		Debug.Log("Error getting leaderboard entry");
		Debug.Log(error.GenerateErrorReport());
	}
	
	// Start is called before the first frame update
    void Start()
    {
		instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
