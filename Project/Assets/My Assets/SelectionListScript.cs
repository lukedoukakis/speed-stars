using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionListScript : MonoBehaviour
{
	public GlobalController gc;
	public GameObject selectionButtonPrefab;
	public GameObject grid;
	public List<string> buttonNames;
	
	public bool canSelectMultiple;
	
	
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
	
	public GameObject getButton(string racerName){
		GameObject b;
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.name == racerName){
				return b;
			}
		}
		return null;
	}
	
	
	
	public GameObject addButton(string racerName){
		GameObject button = Instantiate(selectionButtonPrefab);
		SelectionButtonScript buttonScript = button.GetComponent<SelectionButtonScript>();
		buttonScript.init();
		button.transform.SetParent(grid.transform, false);
		buttonScript.list = this;
		buttonScript.setFromRacer(racerName);
		// -----------------
		buttonNames.Add(racerName);
		
		return button;
	}
	
	public void removeButton(string racerName){
		GameObject b;
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			b = child.gameObject;
			s = b.GetComponent<SelectionButtonScript>();
			if(s.name == racerName){
				Destroy(b);
				buttonNames.Remove(racerName);
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
		buttonNames.Clear();
	}
	
	public void init(bool canSelectMultiple){
		buttonNames = new List<string>();
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
