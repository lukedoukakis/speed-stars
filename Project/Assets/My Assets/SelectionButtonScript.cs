﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButtonScript : MonoBehaviour
{
	
	public SelectionListScript list;

	public bool selected;
	public string selectedColorCode = "#B96481";
	public string unselectedColorCode = "#CDB6CF";
	public string name;
	public string time;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void setFromRacer(string racerName){
		// -----------------
		name = racerName;
		gameObject.transform.Find("NameText").GetComponent<Text>().text = name;
		// -----------------
		time = PlayerPrefs.GetString(racerName).Split(':')[1];
		if(float.Parse(time) == -1f){
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = "No Mark";
		}else{
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = time;
		}	
	}
	
	public void toggle(){
		if(!(list.canSelectMultiple)){
			list.toggleAllOff();
		}
		selected = !selected;
		if(selected){
			setColor(selectedColorCode);
		}
		else{
			setColor(unselectedColorCode);
		}
	}
	
	public void setColor(string colorCode){
		Color color;
		if(ColorUtility.TryParseHtmlString(colorCode, out color)){
			GetComponent<Image>().color = color;
		}
	}
	
	public void init(){
		selected = false;
	}
}
