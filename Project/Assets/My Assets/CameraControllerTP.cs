using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerTP : MonoBehaviour
{
	
	public Transform player;
	
	Vector3 playerPos;
	Vector3 targetPos;
	float pi = Mathf.PI;
	float heightModifier;
	
	public float cameraDistance;
	float sensitivityUpDown = .005f;


    // Start is called before the first frame update
    void Start(){
		
		heightModifier = 0f;
		
		
		playerPos = player.position;
		targetPos = player.position - player.forward;
		
    }

    // Update is called once per frame
    void Update()
    {
		
		
		
		playerPos = player.position;
		heightModifier += Input.GetAxis("Mouse Y") * -1 * sensitivityUpDown;
		if(heightModifier*pi >= pi/2){
			heightModifier = .5f;
		}
		if(heightModifier*pi <= (pi*-1)/2){
			heightModifier = -.5f;
		}
		
		targetPos = playerPos + (cameraDistance*Mathf.Cos(heightModifier*pi) * player.forward*-1) + (cameraDistance*Mathf.Sin(heightModifier*pi) * Vector3.up) + (Vector3.up*.5f);
		float x = Mathf.Lerp(transform.position.x, targetPos.x, .3f);
		float y = Mathf.Lerp(transform.position.y, targetPos.y + 1f, 1f);
		float z = Mathf.Lerp(transform.position.z, targetPos.z, .2f);
		transform.position = new Vector3(x, y, z);
		
		transform.LookAt(playerPos + Vector3.up*.5f + transform.TransformDirection(Vector3.forward*.5f));
		

		/*
		// Smoothly tilts a transform towards a target rotation.
        float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;

        // Rotate the cube by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);
		*/
		

    }
}