using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
	public GlobalController gc;
	public TaskManager taskManager;
	
	public GameObject selectionButtonPrefab;
	
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
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
