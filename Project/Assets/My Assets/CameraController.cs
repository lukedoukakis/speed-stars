using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	
	public GlobalController gc;
	
	public GameObject referenceObject;
	Vector3 velocity;
	public float cameraDistance;
	
	public int mode;
		/* // -----------------
		modes
		0	side lock-on
		1	tv
		2	3rd person
		*/ // -----------------
	
	
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		velocity = referenceObject.GetComponent<Rigidbody>().velocity;
		transform.LookAt(referenceObject.transform.position);
		
		switch (mode) {
			case 0 :
				move_lockOn();
				break;
			case 1 :
				move_tv();
				break;
			case 2 :
				move_thirdPerson();
				break;
			default:
				break;
		}  
    }
	
	void move_lockOn(){
		transform.position = Vector3.Lerp(transform.position, referenceObject.transform.position + new Vector3(cameraDistance + referenceObject.GetComponent<Rigidbody>().velocity.z*.15f, 1.25f, cameraDistance*.2f), .2f);
		transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
	}
	
	void move_tv(){
		
	}
	
	void move_thirdPerson(){
		
	}
}
