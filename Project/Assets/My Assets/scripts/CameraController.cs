using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public int mode;
	public static int CAMERA_MODE_SIDESCROLL = 1;
	public static int CAMERA_MODE_CINEMATIC = 2;
	public static int CAMERA_MODE_TV = 3;
	public static int CAMERA_MODE_THIRDPERSON = 4;
	// -----------------
	public GlobalController gc;
	public GameObject referenceObject;
	public GameObject startLine_100m;
	public GameObject finishLine;
	public float cameraDistance;
	// -----------------
	Vector3 referenceObjectPos;
	Vector3 referenceObjectVelocity;
	bool referenceObjectFinished;
	// -----------------
	Vector3 cameraStartPos_cinematic;
	Vector3 cameraFinishPos_cinematic;
	Vector3 cameraStartPos_tv;
	Vector3 cameraFinishPos_tv;
	Vector3 startLinePos_100m;
	Vector3 finishLinePos;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		cameraStartPos_cinematic = new Vector3(10.5f, 2.1f, -70f);
		cameraFinishPos_cinematic = new Vector3(10.5f, 1.6f, 128f);
		cameraStartPos_tv = new Vector3(12f, 4f, -70f);
		cameraFinishPos_tv = new Vector3(12f, 4f, 128f);
		
		startLinePos_100m = startLine_100m.transform.position;
		finishLinePos = finishLine.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
	
	void lockOn(){
		transform.position = Vector3.Lerp(transform.position, referenceObjectPos + new Vector3(cameraDistance + referenceObjectVelocity.z*.15f, 1.25f, cameraDistance*.2f), .15f);
		//transform.position = Vector3.Lerp(transform.position, referenceObjectPos + new Vector3(cameraDistance, 1.25f, cameraDistance*.2f), .2f);
		transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
	}

	void cinematic(){
		float pathProgress = (referenceObjectPos.z - startLinePos_100m.z) / ((finishLinePos.z - startLinePos_100m.z) + 0f);
		float x,y,z;
		// -----------------
		x = cameraStartPos_cinematic.x + (referenceObjectPos.x*.2f) + ((cameraFinishPos_cinematic.x - cameraStartPos_cinematic.x) * pathProgress);
		y = cameraStartPos_cinematic.y + ((cameraFinishPos_cinematic.y - cameraStartPos_cinematic.y) * pathProgress);
		z = cameraStartPos_cinematic.z + ((cameraFinishPos_cinematic.z - cameraStartPos_cinematic.z) * pathProgress);
		transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .12f);
		// -----------------
		
		Quaternion targetRot = Quaternion.LookRotation((referenceObject.transform.position - transform.position) + (Vector3.forward*1f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .012f);
		
		referenceObjectFinished = pathProgress >= 1f;
		if(referenceObjectFinished){
			referenceObject = finishLine;
		}
	}
	
	void tv(){
		float pathProgress = (referenceObjectPos.z - startLinePos_100m.z) / ((finishLinePos.z - startLinePos_100m.z) + 0f);
		float x,y,z;
		// -----------------
		x = cameraStartPos_tv.x + (referenceObjectPos.x*.2f) + ((cameraFinishPos_cinematic.x - cameraStartPos_cinematic.x) * pathProgress);
		y = cameraStartPos_tv.y + ((cameraFinishPos_tv.y - cameraStartPos_tv.y) * pathProgress);
		z = cameraStartPos_tv.z + ((cameraFinishPos_tv.z - cameraStartPos_tv.z) * pathProgress);
		transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .3f);
		// -----------------
		
		Quaternion targetRot = Quaternion.LookRotation((referenceObject.transform.position - transform.position) + (Vector3.forward*1f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .012f);
		
		referenceObjectFinished = pathProgress >= 1f;
		if(referenceObjectFinished){
			referenceObject = finishLine;
		}
	}
	
	void thirdPerson(){
		
	}
}
