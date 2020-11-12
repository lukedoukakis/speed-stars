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
	float canvasWidth;
	float canvasHeight;
	float screenWidth;
	float screenHeight;
	
	public GameObject selectionLists;
	public GameObject playerSelectButtonList;
	public GameObject ghostSelectButtonList;
	
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
	
	public void init(){
		canvasWidth = canvas.GetComponent<RectTransform>().rect.size.x;
		canvasHeight = canvas.GetComponent<RectTransform>().rect.size.y;
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		
		incrementBotCount(7);
		
        setSelectedRaceEvent(RaceManager.RACE_EVENT_100M);
	}
	

}
