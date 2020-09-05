using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{
	
	public int selectedRaceEvent;
	
	public GlobalController gc;
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
		canvasWidth = canvas.GetComponent<RectTransform>().rect.size.x;
		canvasHeight = canvas.GetComponent<RectTransform>().rect.size.y;
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		
		float x0, y0;
		float x1, y1;
		
        setSelectedRaceEvent(0);
    }

    // Update is called once per frame
    void Update()
    {
		
        
    }
	
	public void setSelectedRaceEvent(int raceEvent){
		
		selectedRaceEvent = raceEvent;
		gc.selectedRaceEvent = raceEvent;
		toggleSelectionLists(selectedRaceEvent);
		
	}
	
	void toggleSelectionLists(int raceEvent){
		
		playerSelectButtonList.GetComponent<SelectionListScript>().setForEvent(raceEvent);
		ghostSelectButtonList.GetComponent<SelectionListScript>().init(raceEvent, GlobalController.SAVED_RACER_MEMORY, 7, 0, false, false);
		
		//playerSelectButtonList.GetComponent<SelectionListScript>().toggleAllOff();
		//ghostSelectButtonList.GetComponent<SelectionListScript>().toggleAllOff();

	}
	
	
	public void moveUIElement(string way){
		/*
		StartCoroutine(move());
		// -----------------
		IEnumerator move(){
			movingElement = true;
			// -----------------
			GameObject element = this.gameObject;
			RectTransform rectTransform = element.GetComponent<RectTransform>();
			Vector2 position = rectTransform.anchoredPosition;
			float targetX = 0f;
			float targetY = 0f;
			if(way == "center"){
				targetX = 0f;
				targetY = 0f;
			}
			else if(way == "left"){
				targetX = canvasWidth * -1f;
				targetY = 0f;
			}
			else if(way == "right"){
				targetX = canvasWidth;
				targetY = 0f;
			}
			else if(way == "up"){
				targetX = 0f;
				targetY = canvasHeight;
			}
			else if(way == "down"){
				targetX = 0f;
				targetY = canvasHeight * -1f;
			}
			float x, y;
			// -----------------
			while(Mathf.Abs(position.x - targetX) > .2f){
				position = rectTransform.anchoredPosition;
				x = Mathf.Lerp(position.x, targetX, swapSpeed * Time.deltaTime);
				y = Mathf.Lerp(position.y, targetY, swapSpeed * Time.deltaTime);
				position.x = x;
				position.y = y;
				rectTransform.anchoredPosition = position;
				yield return null;
			}
			Debug.Log("done");
			position.x = targetX;
			position.y = targetY;
			rectTransform.anchoredPosition = new Vector2(targetX, targetY);
			movingElement = false;
		}
		*/
	}
	
	public void incrementBotCount(int n){
		if((botCount + n >= 0) && (botCount + n <= botCount_max)){
			botCount += n;
			botCountText.text = botCount.ToString();
		}
		else{
			// <alert max racers>
		}
		Debug.Log("botCount_max: " + botCount_max);
		Debug.Log("botCount: " + botCount);
		Debug.Log("---");
	}
	

}
