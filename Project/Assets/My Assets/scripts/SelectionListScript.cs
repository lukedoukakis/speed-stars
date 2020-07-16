﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionListScript : MonoBehaviour
{
	public GlobalController gc;
	public GameObject selectionButtonPrefab;
	public GameObject grid;
	public List<string> buttonIDs;
	public List<string> selectedButtonIDs;
	
	public int maxSelectable;
	public int numSelected;
	
	public bool replaceLastSelection;	// if true, last selection will be replaced with new if maxSelectable has been reached
	
	public string sourceMemory;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		numSelected = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
	
	public GameObject getButton(string id){
		GameObject b;
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.id == id){
				return b;
			}
		}
		return null;
	}
	
	
	
	public GameObject addButton(string id){
		GameObject button = Instantiate(selectionButtonPrefab);
		SelectionButtonScript buttonScript = button.GetComponent<SelectionButtonScript>();
		buttonScript.init();
		button.transform.SetParent(grid.transform, false);
		buttonScript.list = this;
		buttonScript.setFromRacer(id);
		// -----------------
		buttonIDs.Add(id);
		
		return button;
	}
	
	public void removeButton(string id){
		GameObject b;
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.id == id){
				Destroy(b);
				buttonIDs.Remove(id);
				break;
			}	
		}
	}

	public void toggleAllOff(){
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			s = child.gameObject.GetComponent<SelectionButtonScript>();
			if(s.selected){
				s.toggle(false);
			}
		}
	}

	
	public void clear(){
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}
		buttonIDs.Clear();
	}
	
	public void init(string sourceMemory, int maxSelectable, bool replaceLastSelection){
		buttonIDs = new List<string>();
		// -----------------
		this.maxSelectable = maxSelectable;
		this.replaceLastSelection = replaceLastSelection;
		this.sourceMemory = sourceMemory;
		// -----------------
		
		string[] playerIDs = PlayerPrefs.GetString(sourceMemory).Split(':');
		if(playerIDs.Length > 0){
			foreach(string playerID in playerIDs){
				if(playerID != ""){
					//Debug.Log(sourceMemory + ": " + playerID);
					addButton(playerID);
				}
			}
		}
	}
	
	
	
	
	
}