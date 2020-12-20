using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;

public class DateGetter : MonoBehaviour
{
	
	public static string getDate(){
		int seconds = (int)SteamUtils.GetServerRealTime();
		
		DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
		DateTime dt = dateTimeOffset.DateTime;
		
		return string.Join("/", dt.Month, dt.Day, dt.Year);
	}
	
}