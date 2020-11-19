using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{
	
	public int selectedRaceEvent;
	
	public GlobalController gc;
	public CameraController cameraController;
	public Canvas canvas;
	
	public GameObject selectionLists;
	public GameObject playerSelectButtonList;
	public GameObject ghostSelectButtonList;
	public GameObject pointerPos_100m;
	public GameObject pointerPos_200m;
	public GameObject pointerPos_400m;
	public GameObject pointerPos_60m;
	public GameObject pointer;
	
	// selection list locations
	public Vector2 onScreenLocation;
	public Vector2 offScreenLocation;
	bool movingElement;
	
	// selection list swap speed
	public float swapSpeed;
	
	// bot counter
	public Text botCountText;
	public int botCount;
	public int botCount_max;
	
    // Start is called before the first frame update
    void Start()
    {
    }
	
	public void setSelectedRaceEvent(int raceEvent){
		
			selectedRaceEvent = raceEvent;
			gc.selectedRaceEvent = raceEvent;
			toggleSelectionLists(selectedRaceEvent);
			movePointer(raceEvent);
			gc.cameraController.setCameraFocusOnStart();
	}
	
	void toggleSelectionLists(int raceEvent){
		
		SelectionListScript ps = playerSelectButtonList.GetComponent<SelectionListScript>();
		SelectionListScript gs = ghostSelectButtonList.GetComponent<SelectionListScript>();
		
		ps.setForEvent(raceEvent);
		gs.init(raceEvent, GlobalController.SAVED_RACER_MEMORY, 7, 0, false, false);

	}
	
	
	public void incrementBotCount(int n){
		if((botCount + n >= 0) && (botCount + n <= botCount_max)){
			botCount += n;
			botCountText.text = botCount.ToString();
		}
		else{
			// <alert max racers>
		}
		//Debug.Log("botCount_max: " + botCount_max);
		//Debug.Log("botCount: " + botCount);
		//Debug.Log("---");
	}
	
	void movePointer(int raceEvent){
		Transform t_button;
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			t_button = pointerPos_100m.transform;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			t_button = pointerPos_200m.transform;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			t_button = pointerPos_400m.transform;
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			t_button = pointerPos_60m.transform;
		}
		else{
			return;
		}
		pointer.transform.position = t_button.position;
	}
	
	public void init(){
		incrementBotCount(7);
        setSelectedRaceEvent(RaceManager.RACE_EVENT_100M);
	}
	

}
