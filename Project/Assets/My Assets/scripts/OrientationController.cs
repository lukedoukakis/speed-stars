using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationController : MonoBehaviour
{
	
	public PlayerAnimationV2 animation;
	public Rigidbody rb;
	
	public float distance;
	public Vector3 sphereEulersForward;
	public Vector3 sphereEulersLeft;
	public GameObject sphere;
	
	public GameObject sphere1;
	public GameObject sphere2;
	public Vector3 sphere1_pos;
	public Vector3 sphere2_pos;
	public float sphere1_z;
	public float sphere2_z;
	public float sphere1_x;
	public float sphere2_x;
	
	public float sphere1_prog;
	public float sphere2_prog;
	
	public int trackSegment;
	
	
	public float rotY_initial_sphere1;
	public float rotY_complete_sphere1;
	public float rotY_initial_sphere2;
	public float rotY_complete_sphere2;
	public float rotY_current_sphere1;
	public float rotY_current_sphere2;
	

    // Start is called before the first frame update
    void Start()
    {
		if(sphere1 != null){
			sphere1_pos = sphere1.transform.position;
		}
		if(sphere2 != null){
			sphere2_pos = sphere2.transform.position;
		}
		sphere1_z = sphere1_pos.z;
		sphere2_z = sphere2_pos.z;
		sphere1_x = sphere1_pos.x;
		sphere2_x = sphere2_pos.x;
    }

	public void updateOrientation(bool enforcePosition){
	
		checkOrientationPosition();
		// -----------------
		if(sphere != null){
			Vector3 racerPos = new Vector3(transform.position.x, 0f, transform.position.z);
			Vector3 spherePos = new Vector3(sphere.transform.position.x, 0f, sphere.transform.position.z);
			// -----------------
			if(enforcePosition){
				float curD = Vector3.Distance(racerPos, spherePos);
				float diff;
				diff = curD - distance;
				if(curD > distance){
					rb.AddForce(transform.right*-1f * 1500f * Time.deltaTime);
				}
			
				else if(curD < distance - 1.25f){
					rb.AddForce(transform.right * 1500f * Time.deltaTime);
				}
			}
			sphere.transform.LookAt(racerPos);
			// -----------------
			updateRotationsAndProgress();
			// -----------------
			updatePlayerRotation();
		}
	}
	
	void checkOrientationPosition(){
		Vector3 pos = transform.position;
		float zPos = pos.z;
		// -----------------
		if(zPos > sphere2_z){
			if(zPos > sphere1_z){
				sphere = sphere1;
				trackSegment = 1;
			}
			else{
				if(sphere != null){
					sphere = null;
					pos = adjustForStraight(pos);
					transform.position = pos;
					if(pos.x > sphere1_x){
						trackSegment = 4;
					}
					else{
						trackSegment = 2;
					}
				}
			}
		}
		else{
			sphere = sphere2;
			trackSegment = 3;
		}
	}
	
	Vector3 adjustForStraight(Vector3 _pos){
		Vector3 adjustedPos = _pos;
		// -----------------
		if(adjustedPos.x > sphere1_x){
			adjustedPos.x = (sphere1_x + distance);
		}
		else{
			adjustedPos.x = (sphere1_x - distance);
		}
				
		Vector3 velocity = rb.velocity;
		velocity.x = 0f;
		rb.velocity = velocity;
		
		return adjustedPos;
	}
	
	public void initRotations(){
		distance = Vector3.Distance(sphere.transform.position, transform.position);
		// -----------------
		Vector3 sphere1Eulers = sphere1.transform.rotation.eulerAngles;
		Vector3 sphere2Eulers = sphere2.transform.rotation.eulerAngles;
		if(trackSegment == 1){
			rotY_initial_sphere1 = sphere1Eulers.y;
			rotY_initial_sphere2 = 180f;
		}
		else if(trackSegment == 3){
			rotY_initial_sphere1 = 0f;
			rotY_initial_sphere2 = sphere2Eulers.y;
		}
		else if(trackSegment == 3){
			rotY_initial_sphere1 = 0f;
			rotY_initial_sphere2 = 180f;
		}
		rotY_complete_sphere1 = 180f;
		rotY_complete_sphere2 = 0f;	
	}
	
	public void updateRotationsAndProgress(){
		rotY_current_sphere1 = sphere1.transform.rotation.eulerAngles.y;
		sphere1_prog = (rotY_current_sphere1 - rotY_initial_sphere1) / (rotY_complete_sphere1 - rotY_initial_sphere1);
		rotY_current_sphere2 = sphere2.transform.rotation.eulerAngles.y;
		sphere2_prog = (rotY_initial_sphere2 - rotY_current_sphere2) / (rotY_initial_sphere2 - rotY_complete_sphere2);
	}
	
	public void updatePlayerRotation(){
		sphere.transform.Rotate(Vector3.down * 90f, Space.Self);
		sphereEulersLeft = sphere.transform.rotation.eulerAngles;
		animation.turnTowardsY(sphereEulersLeft.y);
	}
}
