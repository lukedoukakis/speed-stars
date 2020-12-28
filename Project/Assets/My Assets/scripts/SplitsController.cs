using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SplitsController : MonoBehaviour
{

	public RaceManager rm;

    [SerializeField] GameObject splitLines_100m;
    [SerializeField] GameObject splitLines_200m;
    [SerializeField] GameObject splitLines_400m;

	[SerializeField] GameObject splitUI;
    [SerializeField] TextMeshProUGUI splitText;

	string splitName;
    bool first;

    float leaderSplit;
    float playerSplit;
    float differential;



    public void init(int raceEvent, GameObject player)
    {
        first = false;
		splitUI.SetActive(false);

		if (raceEvent == RaceManager.RACE_EVENT_100M)
		{
			splitLines_100m.SetActive(true);
			splitLines_200m.SetActive(false);
			splitLines_400m.SetActive(false);
			splitName = "60m";
		}
		else if (raceEvent == RaceManager.RACE_EVENT_200M)
		{
			splitLines_100m.SetActive(false);
			splitLines_200m.SetActive(true);
			splitLines_400m.SetActive(false);
			splitName = "100m";
		}
		else if (raceEvent == RaceManager.RACE_EVENT_400M)
		{
			splitLines_100m.SetActive(false);
			splitLines_200m.SetActive(false);
			splitLines_400m.SetActive(true);
			splitName = "200m";
		}
	}

	public void register(GameObject racer)
	{ 
		float split = ((float)rm.raceTick) / 100f;
		if (!first)
		{
			first = true;
			leaderSplit = split;
		}

		if (racer == rm.player)
		{
			playerSplit = split;
			differential = playerSplit - leaderSplit;
			showSplit();
		}
	}

	void showSplit()
    {
		splitUI.SetActive(true);
		if(differential > 0f)
        {
			splitText.text = splitName + ": " + playerSplit.ToString("F2") + "s (<color=#ffa8a8>+" + differential.ToString("F2") + "</color>)";
		}
        else
        {
			splitText.text = splitName + ": " + playerSplit.ToString("F2") + "s";
		}
    }

	
}
