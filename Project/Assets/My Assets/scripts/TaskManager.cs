using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	
	public GlobalController gc;
	public List<int> tasks;
	
	// taskID ----------------
	public static int LOAD_SELECTED_RACERS = 2;
	public static int LOAD_SELECTED_PLAYER = 3;
	public static int CREATE_RACER = 4;
	public static int CLEAR_RACERS_FROM_SCENE = 5;
	public static int SAVE_PLAYER = 6;
	public static int SETUP_REPLAY = 7;
	public static int SAVE_USER_PB = 8;
	public static int SET_USER_RECORD = 10;
	public static int DISABLE_ANIMATION_STARTSCREEN = 11;
	public static int SAVE_DEFAULT_CAMERA_MODES = 12;
	
	// -----------------
	
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		tasks = new List<int>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public void addTask(int taskID){
		tasks.Add(taskID);
	}
	
}
