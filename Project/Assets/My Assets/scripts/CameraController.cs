using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public int mode;
	public static int STATIONARY = 0;
	public static int STATIONARY_SLOW = 1;
	public static int SIDE_RIGHT = 2;
	public static int SIDE_LEFT = 3;
	public static int ISOMETRIC = 4;
	public static int TV = 5;
	public static int FIRSTPERSON = 6;
	public static int THIRDPERSON = 7;
	public static int TOPDOWN = 8;
	[SerializeField] List<int> cameraModes_live;
	[SerializeField] List<int> cameraModes_replay;
	
	public static float posX_base = 24f;
	
	public bool cameraShake;
	public int fov;
	// -----------------
	public GlobalController gc;
	public Camera camera;
	public Animator animator;
	public GameObject referenceObject;
	public PlayerAnimationV2 referenceAnim;
	public bool referenceHasAnim;
	public float referenceSpeed;
	public float cameraDistance;
	public int cameraGameplayMode;
	public int cameraReplayMode;
	// -----------------
	public GameObject trackReferenceObject_start_60m;
	public GameObject trackReferenceObject_start_100m;
	public GameObject trackReferenceObject_start_200m;
	public GameObject trackReferenceObject_start_400m;
	// -----------------
	public Transform t;
	Vector3 currentPos, referencePos, targetPos;
	Quaternion currentRot, referenceRot, targetRot;
	Vector3 posOffset, rotTargetOffset;
	float posSpeed, rotSpeed;
	
	// -----------------
	float dTime;
	
	
	public void init(float _cameraDistance, int _cameraGameplayMode, int _cameraReplayMode){
		setCameraDistance(_cameraDistance);
		cameraGameplayMode = _cameraGameplayMode;
		cameraReplayMode = _cameraReplayMode;

		camera.ResetProjectionMatrix();
		Matrix4x4 m = camera.projectionMatrix;
		float w = 1.5f;
		m.m00 *= w;
		camera.projectionMatrix = m;
		initViewSettings(PlayerPrefs.GetString("Display Mode"));
	}

	public void initViewSettings(string dm)
	{

		Debug.Log(dm);

		Application.targetFrameRate = 60;
		FullScreenMode fsm;
		if (dm == "Windowed")
		{
			fsm = FullScreenMode.Windowed;
			Screen.SetResolution(1280, 720, fsm, 60);
		}
		else
		{
			fsm = FullScreenMode.ExclusiveFullScreen;
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fsm, 60);
		}
	}
	
	public void setCameraDistance(float d){
		cameraDistance = d;
		cameraDistance = Mathf.Clamp(cameraDistance, .1f, 1f);
		setCameraFocus(this.referenceObject, this.mode);
	}

    // Update is called once per frame
    void Update()
    {

		//Debug.Log(Time.deltaTime);

		dTime = Time.deltaTime;
		currentPos = t.position;
		referencePos = referenceObject.transform.position;
		referenceRot = referenceObject.transform.rotation;
		currentRot = t.rotation;
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
		else if(mode == STATIONARY_SLOW){
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset);
			targetRot = referenceRot;
		}
		else if(mode == SIDE_RIGHT){
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset) + referenceObject.transform.right*referenceSpeed;
			targetRot = Quaternion.LookRotation(referenceObject.transform.right*-1f, Vector3.up);
		}
		else if(mode == SIDE_LEFT){
			referencePos.y = 1f;
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset) + referenceObject.transform.right*-1f;
			targetRot = Quaternion.LookRotation(referencePos - t.position);
		}
		else if(mode == ISOMETRIC){
			referencePos.y = 0f;
			targetPos = referencePos + posOffset;
			targetRot = Quaternion.LookRotation(referencePos - t.position);
		}
		else if(mode == TV){
			referencePos.y = 1f;
			targetPos = referencePos + posOffset;
			targetRot = Quaternion.LookRotation(referencePos - t.position);
		}
		else if(mode == FIRSTPERSON){
			cameraShake = false;
			targetPos = referenceAnim.headT.position;
			targetRot = referenceAnim.headT.rotation;
		}
		else if(mode == THIRDPERSON){
			cameraShake = false;
			referencePos.y = 0f;
			targetPos = referencePos + referenceObject.transform.TransformDirection(posOffset);
			targetRot = Quaternion.LookRotation(referencePos - t.position);
		}
		else if(mode == TOPDOWN){
			cameraShake = false;
			referencePos.y = 0f;
			targetPos = referencePos + posOffset + referenceObject.transform.forward*-.5f;
			targetRot = Quaternion.LookRotation(referencePos - t.position);
		}
		
		t.position = Vector3.Lerp(currentPos, targetPos, posSpeed * dTime);
		t.rotation = Quaternion.Slerp(currentRot, targetRot, rotSpeed * dTime);
			
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
			cameraShake = false;
			fov = 100;
			posOffset = new Vector3(0f, .5f, -5f);
			posSpeed = 7f;
			rotSpeed = .2f;
		}
		else if(mode == STATIONARY_SLOW){
			cameraShake = false;
			fov = 100;
			posOffset = new Vector3(0f, .5f, -5f);
			posSpeed = .05f;
			rotSpeed = .05f;
		}
		else if(mode == SIDE_RIGHT){
			cameraShake = false;
			fov = 100;
			posOffset = new Vector3(3.1f*Mathf.Pow(cameraDistance, 1f), 1.34f*Mathf.Pow(cameraDistance, .1f), 2.5f*Mathf.Pow(cameraDistance, 1f));
			posSpeed = 8f * Mathf.Pow(2f-cameraDistance, 1f);
			rotSpeed = 10f;
		}
		else if(mode == SIDE_LEFT){
			cameraShake = true;
			fov = 100;
			posOffset = new Vector3(-3f, 0f, 7f);
			posSpeed = 4f;
			rotSpeed = 7f;
		}
		else if(mode == ISOMETRIC){
			cameraShake = false;
			fov = 100;
			posOffset = new Vector3(3f, 2f, 1f);
			posSpeed = 1000f;
			rotSpeed = 10f;
		}
		if(mode == TV){
			cameraShake = true;
			fov = 100;
			posOffset = new Vector3(5f, 2f, 3f);
			posSpeed = 10f;
			rotSpeed = 10f;
		}
		if(mode == FIRSTPERSON){
			cameraShake = false;
			fov = 100;
			posOffset = new Vector3(0f, 0f, .1f);
			posSpeed = 100f;
			rotSpeed = 1.5f;
		}
		if(mode == THIRDPERSON){
			cameraShake = true;
			fov = 100;
			posOffset = new Vector3(1f, 3f, -1.5f);
			posSpeed = 5f;
			rotSpeed = 2.5f;
		}
		if(mode == TOPDOWN){
			cameraShake = true;
			fov = 100;
			posOffset = new Vector3(0f, 10f, 0f);
			posSpeed = 100f;
			rotSpeed = 5f;
		}
		
		animator.SetBool("shake", cameraShake);
		animator.SetInteger("fov", fov);
		
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
	
	public void cycleCameraMode(int viewMode){
		List<int> cameraModes;
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			cameraModes = cameraModes_live;
		}
		else{
			cameraModes = cameraModes_replay;
		}
		
		int newIndex = cameraModes.IndexOf(mode) + 1;
		if(newIndex >= cameraModes.Count){
			newIndex = 0;
		}
		else if(newIndex < 0){
			newIndex = cameraModes.Count - 1;
		}
		int newMode = cameraModes[newIndex];
		setCameraFocus(referenceObject, newMode);
		if(viewMode == RaceManager.VIEW_MODE_LIVE){
			cameraGameplayMode = newMode;
		}
		else{
			cameraReplayMode = newMode;
		}
	}

	
	
	
	
}
