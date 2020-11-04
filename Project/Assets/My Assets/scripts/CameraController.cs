using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public int mode;
	public static int STATIONARY = 0;
	public static int SIDE = 1;
	public static int CINEMATIC = 2;
	public static int TV = 3;
	public static int THIRDPERSON = 4;
	
	public static float FOV_STANDARD = 75f;
	
	public static float posX_base = 24f;
	// -----------------
	public GlobalController gc;
	public Camera camera;
	public GameObject referenceObject;
	public PlayerAnimationV2 referenceAnim;
	public bool referenceHasAnim;
	public float referenceSpeed;
	public float cameraDistance;
	// -----------------
	public GameObject trackReferenceObject_start_60m;
	public GameObject trackReferenceObject_start_100m;
	public GameObject trackReferenceObject_start_200m;
	public GameObject trackReferenceObject_start_400m;
	// -----------------
	Vector3 currentPos, referencePos, targetPos;
	Quaternion currentRot, referenceRot, targetRot;
	Vector3 posOffset, rotTargetOffset;
	float posSpeed, rotSpeed;
	
	// -----------------
	float dTime;

    // Update is called once per frame
    void Update()
    {
		dTime = Time.deltaTime;
		currentPos = transform.position;
		referencePos = referenceObject.transform.position;
		referenceRot = referenceObject.transform.rotation;
		currentRot = transform.rotation;
		if(referenceHasAnim){
			referenceSpeed = Mathf.Pow(referenceAnim.speedHoriz, .4f);
			if(referenceSpeed < 1f){
				referenceSpeed = 1f;
			}
		}
		else{
			referenceSpeed = 1f;
		}
		
		
		if(mode == STATIONARY){
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset);
			targetRot = referenceRot;
		}
		else if(mode == SIDE){
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset*cameraDistance) + referenceObject.transform.right*referenceSpeed;
			targetRot = Quaternion.LookRotation(referenceObject.transform.right*-1f, Vector3.up);
		}
		else if(mode == CINEMATIC){
			referencePos.y = 0f;
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset);
			targetRot = Quaternion.LookRotation(referencePos - transform.position);
		}
		else if(mode == TV){
			float xFromBase = posX_base-referencePos.x;
			//referencePos.y = 1.35f;
			referencePos.y = 0f;
			targetPos = referencePos + posOffset*cameraDistance + Vector3.right*xFromBase*.03f;
			targetRot = Quaternion.LookRotation(referencePos - transform.position);
			setFov(50f - (xFromBase*.1f));
		}
		else if(mode == THIRDPERSON){
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset);
			targetRot = referenceRot;
		}
			
		
		transform.position = Vector3.Lerp(currentPos, targetPos, posSpeed * dTime);
		transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotSpeed * dTime);
			
    }
	
	public void setCameraFocusOnStart(){
		int raceEvent = gc.setupManager.selectedRaceEvent;
		
		if(raceEvent == RaceManager.RACE_EVENT_60M){
			setCameraFocus("60m Start", CameraController.STATIONARY);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_100M){
			setCameraFocus("100m Start", CameraController.STATIONARY);
		}
		if(raceEvent == RaceManager.RACE_EVENT_200M){
			setCameraFocus("200m Start", CameraController.STATIONARY);
		}
		if(raceEvent == RaceManager.RACE_EVENT_400M){
			setCameraFocus("400m Start", CameraController.STATIONARY);
		}
	}
	
	public void setCameraFocus(GameObject g, int cameraMode){
		referenceObject = g;
		mode = cameraMode;
		// -----------------
		if(mode == STATIONARY){
			setFov(100f);
			posOffset = new Vector3(0f, .5f, -5f);
			posSpeed = 7f;
			rotSpeed = .2f;
		}
		else if(mode == SIDE){
			setFov(100f);
			posOffset = new Vector3(1.55f, 1.25f, 1.75f);
			posSpeed = 8f;
			rotSpeed = 10f;
		}
		else if(mode == CINEMATIC){
			setFov(100f);
			posOffset = new Vector3(5f, 1f, 2f);
			posSpeed = 10f;
			rotSpeed = .5f;
		}
		if(mode == TV){
			setFov(50f);
			posOffset = new Vector3(10f, 4f, 4f);
			//posOffset = new Vector3(2.5f, 0f, 1f);
			posSpeed = 100f;
			rotSpeed = 100f;
		}
		if(mode == THIRDPERSON){
			setFov(100f);
			posOffset = new Vector3(0f, 1.5f, -2f);
			posSpeed = 10f;
			rotSpeed = 2f;
		}
		
		PlayerAnimationV2 anim = g.GetComponent<PlayerAnimationV2>();
		if(anim != null){
			referenceHasAnim = true;
			referenceAnim = anim;
		}
		else{
			referenceHasAnim = false;
			referenceAnim = null;
		}
	}
	
	public void setCameraFocus(string s, int cameraMode){
		if(s == "60m Start"){
			setCameraFocus(trackReferenceObject_start_60m, cameraMode);
		}
		else if(s == "100m Start"){
			setCameraFocus(trackReferenceObject_start_100m, cameraMode);
		}
		else if(s == "200m Start"){
			setCameraFocus(trackReferenceObject_start_200m, cameraMode);
		}
		else if(s == "400m Start"){
			setCameraFocus(trackReferenceObject_start_400m, cameraMode);
		}
	}
	
	public void setFov(float fov){
		camera.fieldOfView = fov;
	}

	// Start is called before the first frame update
    void Start()
    {
		
	
	
    }
	
	
	
	
}
