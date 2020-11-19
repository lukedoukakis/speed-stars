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
	
	public GameObject deleteDialog;
	public DeleteDialogController ddc;
	public SelectionButtonScript buttonToDelete;
	
	
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
	
	public GameObject getFirst(){
		return grid.transform.GetChild(0).gameObject;
	}
	
	
	public GameObject addButton(string id, int raceEvent, Color32 color32){
		GameObject button = Instantiate(selectionButtonPrefab);
		button.GetComponent<Image>().color = color32;
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
		}
		numSelected = 0;
	}

	
	public void clear(){
		foreach(Transform child in grid.transform){
			if(child.gameObject != gc.newRacerButton){
				Destroy(child.gameObject);
			}
		}
		buttonIDs.Clear();
		selectedButtonIDs.Clear();
	}
	
	public void setPreviewRacer(string id){
		GameObject temp = gc.loadRacer(id, 0, "Untagged", false, false);
		PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
		
		att.copyAttributesFromOther(temp, "body proportions");
		att.copyAttributesFromOther(temp, "clothing");
		att.copyAttributesFromOther(temp, "stats");
		
		att.setClothing(PlayerAttributes.FROM_THIS);
		att.setBodyProportions(PlayerAttributes.FROM_THIS);
		att.setStats(PlayerAttributes.FROM_THIS);
		
		Destroy(temp);
	
	}
	
	public void init(int _raceEvent, string _sourceMemory, int _maxSelectable, int _minSelectable, bool _replaceLastSelection, bool _showIfNoTime){
		clear();
		buttonIDs = new List<string>();
		selectedButtonIDs = new List<string>();
		// -----------------
		this.maxSelectable = _maxSelectable;
		this.minSelectable = _minSelectable;
		this.numSelected = 0;
		this.replaceLastSelection = _replaceLastSelection;
		this.sourceMemory = _sourceMemory;
		// -----------------
		string[] playerIDs = PlayerPrefs.GetString(_sourceMemory).Split(':');
		if(playerIDs.Length > 0){
			Color32 buttonColor = new Color32(0,0,0,0);
			if(_replaceLastSelection){
				buttonColor = SelectionButtonScript.playerButtonColor;
			}
			else{
				buttonColor = SelectionButtonScript.ghostButtonColor;
			}
			string playerID = "";
			for(int i = 0; i < playerIDs.Length; i++){
				playerID = playerIDs[i];
				if(playerID != ""){
					GameObject button;
					if(_showIfNoTime || PlayerPrefs.GetString(playerID).Split(':')[3 + _raceEvent] != "-1"){
						button = addButton(playerID, _raceEvent, buttonColor);
					}
				}
			}
		}
		if(replaceLastSelection){
			setPreviewRacerVisibility();
		}
	}
	
	public void setPreviewRacerVisibility(){
		if(numSelected == 0){
			previewRacer.SetActive(false);
		}else{
			previewRacer.SetActive(true);
		}
	}
	
	public void deleteButtonToDelete(){
		if(buttonToDelete != null){
			buttonToDelete.removeThisButtonAndForgetAssociatedRacer();
		}
		if(ddc != null){
			ddc.hide();
		}
	}
	
}
