using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionListScript : MonoBehaviour
{
	public GlobalController gc;
	public GameObject selectionButtonPrefab;
	public GameObject grid;
	public List<string> buttonIDs;
	
	public bool canSelectMultiple;
	
	
    // Start is called before the first frame update
    void Start()
    {
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
		buttonIDs.Add(id.Split('_')[0]);
		
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
				buttonIDs.Remove(id.Split('_')[0]);
				break;
			}	
		}
	}

	public void toggleAllOff(){
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			s = child.gameObject.GetComponent<SelectionButtonScript>();
			if(s.selected){
				s.selected = false;
				s.setColor(s.unselectedColorCode);
			}
		}
	}

	
	public void clear(){
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}
		buttonIDs.Clear();
	}
	
	public void init(bool canSelectMultiple){
		buttonIDs = new List<string>();
		// -----------------
		this.canSelectMultiple = canSelectMultiple;
		// -----------------
		/*
		string[] playerNames = PlayerPrefs.GetString("PLAYER NAMES").Split(':');
		foreach(string p in playerNames){
			
		}
		*/
	}
	
	
	
	
	
}
