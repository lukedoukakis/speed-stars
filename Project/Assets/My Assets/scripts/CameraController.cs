using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public int mode;
	int raceEvent;
	public static int CAMERA_MODE_SIDESCROLL = 1;
	public static int CAMERA_MODE_CINEMATIC = 2;
	public static int CAMERA_MODE_TV = 3;
	public static int CAMERA_MODE_THIRDPERSON = 4;
	
	public static float FOV_STANDARD = 75f;
	// -----------------
	public GlobalController gc;
	public Camera camera;
	public GameObject referenceObject;
	public GameObject startLine_100m;
	public GameObject finishLine;
	public float cameraDistance;
	// -----------------
	Vector3 referenceObjectPos;
	Vector3 referenceObjectVelocity;
	public bool referenceObjectFinished;
	// -----------------
	Vector3 cameraStartPos_cinematic;
	Vector3 cameraFinishPos_cinematic;
	Vector3 cameraStartPos_tv_100m;
	Vector3 cameraStartPos_tv_200m;
	Vector3 cameraStartPos_tv_400m;
	Vector3 cameraStartPos_tv;
	Vector3 cameraFinishPos_tv;
	Vector3 startLinePos_100m;
	Vector3 finishLinePos;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		cameraStartPos_cinematic = new Vector3(10.5f, 2.1f, -70f);
		cameraFinishPos_cinematic = new Vector3(10.5f, 1.6f, 128f);
		cameraStartPos_tv_100m = new Vector3(12f, 4f, -70f);
		cameraStartPos_tv_200m = new Vector3(12f, 4f, -70f);
		cameraStartPos_tv_400m = new Vector3(12f, 4f, -70f);
		cameraFinishPos_tv = new Vector3(12f, 4f, 128f);
		
		startLinePos_100m = startLine_100m.transform.position;
		finishLinePos = finishLine.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		if(referenceObject == null){
			referenceObject = finishLine;
		}
		referenceObjectPos = referenceObject.transform.position;
		referenceObjectVelocity = referenceObject.GetComponent<Rigidbody>().velocity;
		
		if(mode == CAMERA_MODE_SIDESCROLL){
			lockOn();
		}
		else if(mode == CAMERA_MODE_CINEMATIC){
			cinematic();
		}
		else if(mode == CAMERA_MODE_TV){
			tv();
		}
		else if(mode == CAMERA_MODE_THIRDPERSON){
			thirdPerson();
		}
			
    }
	
	public void setFieldOfView(float fov){
		camera.fieldOfView = fov;
	}
	
	public void setForRaceEvent(int _raceEvent){
		raceEvent = _raceEvent;
		if(_raceEvent == RaceManager.RACE_EVENT_100M){
			cameraStartPos_tv = cameraStartPos_tv_100m;
		}
		else if(_raceEvent == RaceManager.RACE_EVENT_200M){
			cameraStartPos_tv = cameraStartPos_tv_200m;
		}
		else if(_raceEvent == RaceManager.RACE_EVENT_400M){
			cameraStartPos_tv = cameraStartPos_tv_400m;
		}
	}
	
	void lockOn(){
		transform.position = Vector3.Lerp(transform.position, referenceObjectPos + (referenceObject.transform.right*cameraDistance) + (referenceObject.transform.right * cameraDistance * new Vector3(referenceObjectVelocity.x, 0f, referenceObjectVelocity.z).magnitude * .05f) + (Vector3.up*1.25f) + (referenceObject.transform.forward * 1.25f), 9f * Time.deltaTime);
		Quaternion targetRot = Quaternion.LookRotation(referenceObject.transform.right*-1f, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
		
		
		//transform.position = Vector3.Lerp(transform.position, referenceObjectPos + new Vector3(cameraDistance + (new Vector3(referenceObjectVelocity.x, 0f, referenceObjectVelocity.z).magnitude)*.15f, 1.25f, cameraDistance*.2f), .15f);
		//transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
	}

	void cinematic(){
		float pathProgress = (referenceObjectPos.z - startLinePos_100m.z) / ((finishLinePos.z - startLinePos_100m.z) + 0f);
		float x,y,z;
		// -----------------
		x = cameraStartPos_cinematic.x + (referenceObjectPos.x*.2f) + ((cameraFinishPos_cinematic.x - cameraStartPos_cinematic.x) * pathProgress);
		y = cameraStartPos_cinematic.y + ((cameraFinishPos_cinematic.y - cameraStartPos_cinematic.y) * pathProgress);
		z = cameraStartPos_cinematic.z + ((cameraFinishPos_cinematic.z - cameraStartPos_cinematic.z) * pathProgress);
		transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), 7f * Time.deltaTime);
		// -----------------
		
		Quaternion targetRot = Quaternion.LookRotation((referenceObject.transform.position - transform.position) + (Vector3.forward*1f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .7f * Time.deltaTime);
		
		referenceObjectFinished = pathProgress >= 1f;
		if(referenceObjectFinished){
			//referenceObject = finishLine;
		}
	}
	
	void tv(){
		float pathProgress = (referenceObjectPos.z - startLinePos_100m.z) / ((finishLinePos.z - startLinePos_100m.z) + 0f);
		float x,y,z;
		
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			x = cameraStartPos_tv.x + (referenceObjectPos.x*.2f) + ((cameraFinishPos_cinematic.x - cameraStartPos_cinematic.x) * pathProgress);
			y = cameraStartPos_tv.y + ((cameraFinishPos_tv.y - cameraStartPos_tv.y) * pathProgress);
			z = cameraStartPos_tv.z + ((cameraFinishPos_tv.z - cameraStartPos_tv.z) * pathProgress);
			transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .3f);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			float distanceFromFinishX = Mathf.Abs(referenceObjectPos.x - (finishLinePos.x + 60f));
			if(distanceFromFinishX < 40f){
				//distanceFromFinishX = 40f;
			}
			
			x = referenceObjectPos.x + (distanceFromFinishX*.015f + (cameraDistance*2f));
			y = cameraStartPos_tv.y + ((cameraFinishPos_tv.y - cameraStartPos_tv.y) * pathProgress);
			z = cameraStartPos_tv.z + ((cameraFinishPos_tv.z - cameraStartPos_tv.z) * pathProgress);
			transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .3f);
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			float distanceFromFinishX = Mathf.Abs(referenceObjectPos.x - (finishLinePos.x + 60f));
			if(distanceFromFinishX < 40f){
				//distanceFromFinishX = 40f;
			}
			
			x = referenceObjectPos.x + (distanceFromFinishX*.015f + (cameraDistance*2f));
			y = cameraStartPos_tv.y + ((cameraFinishPos_tv.y - cameraStartPos_tv.y) * pathProgress);
			z = cameraStartPos_tv.z + ((cameraFinishPos_tv.z - cameraStartPos_tv.z) * pathProgress);
			transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .3f);
		}
		
		/*
		float distanceAway = Vector3.Distance(camera.transform.position, referenceObjectPos);
		Vector3 dVec = (camera.transform.position - referenceObjectPos).normalized;
		
		float fov;
		fov = FOV_STANDARD * ((cameraDistance+5f)/distanceAway);
		if(fov > FOV_STANDARD){
			fov = FOV_STANDARD;
		}
		setFieldOfView(fov);
		transform.position = new Vector3(transform.position.x, transform.position.y + (distanceAway * .05f), transform.position.z);
		
		
		
		//Vector3 targetPos = referenceObjectPos + (dVec*10f) + (Vector3.up * (Mathf.Abs(referenceObjectPos.x - finishLinePos.x) * .03f));
		//transform.position = targetPos;

		*/
		
		referenceObjectPos.y = 1f;
		Quaternion targetRot = Quaternion.LookRotation((referenceObject.transform.position - transform.position) + (referenceObject.transform.forward*1f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 2f * Time.deltaTime);
		
		referenceObjectFinished = (pathProgress >= 1f);
		if(referenceObjectFinished){
			//referenceObject = finishLine;
		}
	}
	
	void thirdPerson(){
		
	}
}
