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
	public string id;
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
	
	public void setFromRacer(string _id, int raceEvent){
		// -----------------
		this.id = _id;
		name = PlayerPrefs.GetString(_id).Split(':')[1];
		gameObject.transform.Find("NameText").GetComponent<Text>().text = name;
		// -----------------
		time = PlayerPrefs.GetString(_id).Split(':')[3 + raceEvent];
		if(float.Parse(time) == -1f){
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = "--";
		}else{
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = time;
		}	
	}
	
	public void removeThisButtonAndForgetAssociatedRacer(){
		if(selected){
			list.numSelected--;
			list.selectedButtonIDs.Remove(id);
		}
		list.gc.forgetRacer(id, new string[]{list.sourceMemory}, false);
		list.removeButton(id);
		
	}
	
	
	public void toggle(){
		// if trying to select
		if(!selected){
			// select if numSelected less than maxSelectable
			if(list.numSelected < list.maxSelectable){
				if(!list.getButton(id).GetComponent<SelectionButtonScript>().selected){
					setColor(selectedColorCode);
					list.numSelected++;
					list.selectedButtonIDs.Add(id);
					if(list == list.gc.playerSelectButtonList){
						list.setPreviewRacer(id);
					}
					selected = true;
				}
			}
			// else, if replaceLastSelection true, unselect first selected button and select this one
			else{
				if(list.replaceLastSelection){
					Debug.Log("replacing last selection");
					Debug.Log("replaced button id: " + list.getButton(list.selectedButtonIDs[0]).GetComponent<SelectionButtonScript>().id);
					list.minSelectable--;
					list.getButton(list.selectedButtonIDs[0]).GetComponent<SelectionButtonScript>().toggle(false);
					list.minSelectable++;
					setColor(selectedColorCode);
					list.numSelected++;
					list.selectedButtonIDs.Add(id);
					if(list == list.gc.playerSelectButtonList){
						list.setPreviewRacer(id);
					}
					selected = true;
					
					
					string s = "selectedButtonIDs: ";
					for(int i = 0; i < list.selectedButtonIDs.Count; i++){
						s += list.selectedButtonIDs[i] + ", ";
					}
					Debug.Log(s);
					
				}
				else{
					
				}
			}
		}
		// else, deselect if numSelected greater than minSelectable, otherwise do nothing
		else{
			if(list.numSelected > list.minSelectable){
				setColor(unselectedColorCode);
				list.numSelected--;
				list.selectedButtonIDs.Remove(id);
				selected = false;
			}
			else{

			}
		}
		if(list == list.gc.ghostSelectButtonList){
			list.gc.setupManager.botCount_max = 7 - list.numSelected;
			if(list.gc.setupManager.botCount > list.gc.setupManager.botCount_max){
				list.gc.setupManager.incrementBotCount(list.gc.setupManager.botCount_max - list.gc.setupManager.botCount);
				//list.gc.setupManager.botCount = list.gc.setupManager.botCount_max;
				//list.gc.setupManager.botCountText.text = list.gc.setupManager.botCount.ToString();
			}
		}
	}
	
	public void toggle(bool setting){
		if(selected != setting){
			toggle();
		}
	}
	
	
	
	public void setColor(string colorCode){
		Color color;
		if(ColorUtility.TryParseHtmlString(colorCode, out color)){
			GetComponent<Image>().color = color;
			transform.Find("Delete Button").gameObject.GetComponent<Image>().color = color;
		}
	}
	
	public void init(SelectionListScript s){
		list = s;
		transform.SetParent(list.grid.transform, false);
	}
}
