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
	public List<string> selectedButtonIDs;
	
	public int maxSelectable;
	public int minSelectable;
	public int numSelected;
	
	public bool replaceLastSelection;	// if true, last selection will be replaced with new if maxSelectable has been reached
	
	public string sourceMemory;
	
	public GameObject previewRacer;
	public GameObject previewPlatform;
	
	
    // Start is called before the first frame update
    void Start()
    {
		numSelected = 0;
		previewRacer.GetComponent<PlayerAnimationV2>().setIdle();
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
	
	
	public GameObject addButton(string id, int raceEvent){
		GameObject button = Instantiate(selectionButtonPrefab);
		SelectionButtonScript buttonScript = button.GetComponent<SelectionButtonScript>();
		buttonScript.init(this);
		buttonScript.setFromRacer(id, raceEvent);
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
	
	public void setForEvent(int raceEvent){
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			s = child.gameObject.GetComponent<SelectionButtonScript>();
			s.setFromRacer(s.id, raceEvent);
		
		}
	}

	public void toggleAllOff(){
		SelectionButtonScript s;
		foreach(Transform child in grid.transform){
			s = child.gameObject.GetComponent<SelectionButtonScript>();
			s.toggle(false);
			numSelected = 0;
		}
	}

	
	public void clear(){
		foreach(Transform child in grid.transform){
			if(child.gameObject != gc.newRacerButton){
				Destroy(child.gameObject);
			}
		}
		buttonIDs.Clear();
	}
	
	public void setPreviewRacer(string id){
		GameObject temp = gc.loadRacer(id, 0, "Untagged", false);
		PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
		
		att.copyAttributesFromOther(temp, "body proportions");
		att.copyAttributesFromOther(temp, "clothing");
		att.copyAttributesFromOther(temp, "stats");
		
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setStats(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		
		//att.renderInForeground();
		previewRacer.transform.position = previewPlatform.transform.position + Vector3.up*.5f;
		previewRacer.GetComponent<PlayerAnimationV2>().energy = 100f;
		Destroy(temp);
	
	}
	
	public void init(int _raceEvent, string _sourceMemory, int _maxSelectable, int _minSelectable, bool _replaceLastSelection, bool _showIfNoTime){
		clear();
		buttonIDs = new List<string>();
		// -----------------
		this.maxSelectable = _maxSelectable;
		this.minSelectable = _minSelectable;
		this.numSelected = 0;
		this.replaceLastSelection = _replaceLastSelection;
		this.sourceMemory = _sourceMemory;
		// -----------------
		
		string[] playerIDs = PlayerPrefs.GetString(_sourceMemory).Split(':');
		if(playerIDs.Length > 0){
			foreach(string playerID in playerIDs){
				if(playerID != ""){
					GameObject button;
					if(_showIfNoTime || PlayerPrefs.GetString(playerID).Split(':')[3 + _raceEvent] != "-1"){
						button = addButton(playerID, _raceEvent);
						if(numSelected < _minSelectable){
							button.GetComponent<SelectionButtonScript>().toggle();
						}
					}
				}
			}
		}
		
		previewRacer.GetComponent<EnergyMeterController>().enabled = false;
	}
	
	
	
	
	
}
