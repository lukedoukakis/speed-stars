using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamController : MonoBehaviour
{
	
	public CSteamID getThisUserSteamID(){
		if(!SteamManager.Initialized){
			return CSteamID.Nil;
		}
		return SteamUser.GetSteamID();
	}
	
	public string getThisUserSteamName(){
		if(!SteamManager.Initialized){
			return "";
		}
		return SteamFriends.GetPersonaName();
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
