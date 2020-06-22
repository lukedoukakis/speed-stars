using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public int mode;
	public static int CAMERA_MODE_SIDESCROLL = 1;
	public static int CAMERA_MODE_TV100M = 2;
	public static int CAMERA_MODE_THIRDPERSON = 3;
	// -----------------
	public GlobalController gc;
	public GameObject referenceObject;
	public GameObject startLine_100m;
	public GameObject finishLine;
	public float cameraDistance;
	// -----------------
	Vector3 referenceObjectPos;
	Vector3 referenceObjectVelocity;
	// -----------------
	Vector3 cameraStartPos_100m;
	Vector3 cameraFinishPos_100m;
	Vector3 startLinePos_100m;
	Vector3 finishLinePos;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		cameraStartPos_100m = new Vector3(14.9f, 3.2f, -65f);
		cameraFinishPos_100m = new Vector3(14.9f, 1.6f, 130.5f);
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
		else if(mode == CAMERA_MODE_TV100M){
			tv_100m();
		}
		else if(mode == CAMERA_MODE_THIRDPERSON){
			thirdPerson();
		}
			
    }
	
	void lockOn(){
		transform.position = Vector3.Lerp(transform.position, referenceObjectPos + new Vector3(cameraDistance + referenceObjectVelocity.z*.15f, 1.25f, cameraDistance*.2f), .2f);
		transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
	}
	
	void tv_100m(){
		float pathProgress = (referenceObjectPos.z - startLinePos_100m.z) / (finishLinePos.z - startLinePos_100m.z);
		// -----------------
		float x = cameraStartPos_100m.x + ((cameraFinishPos_100m.x - cameraStartPos_100m.x) * pathProgress);
		float y = cameraStartPos_100m.y + ((cameraFinishPos_100m.y - cameraStartPos_100m.y) * pathProgress);
		float z = cameraStartPos_100m.z + ((cameraFinishPos_100m.z - cameraStartPos_100m.z) * pathProgress);
		transform.position = Vector3.Lerp(transform.position, new Vector3(x,y,z), .08f);
		// -----------------
		Vector3 lookAtPos = new Vector3(referenceObjectPos.x, finishLinePos.y + 1.5f, referenceObjectPos.z);
		transform.LookAt(lookAtPos);
	}
	
	void thirdPerson(){
		
	}
}
