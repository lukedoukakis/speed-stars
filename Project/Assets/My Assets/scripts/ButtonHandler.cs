using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
	public GlobalController gc;
	public TaskManager taskManager;
	public Button[] resultButtons;
		public Button resultButton1;
		public Button resultButton2;
		public Button resultButton3;
		public Button resultButton4;
		public Button resultButton5;
		public Button resultButton6;
		public Button resultButton7;
		public Button resultButton8;
	Button b;
	int activeButtons;
	
	
	public GameObject selectionButtonPrefab;
	
    // Start is called before the first frame update
    void Start()
    {
		initResultButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// -----------------
	public void initResultButtons(){
		resultButtons = new Button[8]{
			resultButton1,
			resultButton2,
			resultButton3,
			resultButton4,
			resultButton5,
			resultButton6,
			resultButton7,
			resultButton8
		};
	}
	public void setResultButtons(int amount){
		activeButtons = amount;
		for(int i = 0; i < amount; i++){
			resultButtons[i].gameObject.SetActive(true);
			resultButtons[i].gameObject.GetComponentInChildren<Text>().enabled = false;
		}
		for(int i = amount; i < resultButtons.Length; i++){
			resultButtons[i].gameObject.GetComponentInChildren<Text>().enabled = false;
			resultButtons[i].gameObject.SetActive(false);
		}
	}
	public void setSelected(int buttonIndex){
		b = resultButtons[buttonIndex];
		bool status = b.gameObject.transform.Find("Text").GetComponent<Text>().enabled;
		if(status == false){
			gc.checkedRacerIndexes.Add(buttonIndex);
		}
		else{
			gc.checkedRacerIndexes.Remove(buttonIndex);
		}
		b.gameObject.transform.Find("Text").GetComponent<Text>().enabled = !(status);
	}
	// -----------------
	public void saveSelectedAsGhosts(){
		taskManager.addTask(TaskManager.SAVE_SELECTED_RACERS);
	}
	public void loadSelectedGhosts(){
		taskManager.addTask(TaskManager.LOAD_SELECTED_RACERS);
	}
	public void loadSelectedPlayer(){
		taskManager.addTask(TaskManager.LOAD_SELECTED_PLAYER);
	}
	// -----------------
	public void createCharacter(){
		taskManager.addTask(TaskManager.CREATE_RACER);
	}
	// -----------------
	public void clearRacersFromScene(){
		taskManager.addTask(TaskManager.CLEAR_RACERS_FROM_SCENE);
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
