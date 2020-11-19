﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryButtonController : MonoBehaviour
{
	
	public LeaderboardManager lbm;
	
	public int raceEvent;
	public string pfid;
	public int position;
	public string name;
	public float score;
	
	
	
	public void init(LeaderboardManager _lbm, int _raceEvent, string _pfid, int _position, string _name, float _score){
		this.lbm = _lbm;
		this.raceEvent = _raceEvent;
		this.pfid = _pfid;
		this.position = _position;
		this.name = _name;
		this.score = _score;
		transform.Find("Text_Placing").gameObject.GetComponent<Text>().text = (position+1).ToString();
		transform.Find("Text_Username").gameObject.GetComponent<Text>().text = name;
		transform.Find("Text_Score").gameObject.GetComponent<Text>().text = score.ToString("F2");
	}
	
	public void downloadRacer(){
		StartCoroutine(lbm.downloadRacer(this.raceEvent, this.pfid));
	}
	
	public void updateTooltipText(){
		lbm.updateTooltipText();
	}
	
	public void hideTooltip(){
		lbm.tooltipController.hide();
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
