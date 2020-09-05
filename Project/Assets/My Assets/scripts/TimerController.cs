using System.Collections;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	
	public PlayerAttributes attributes;
	public PlayerAnimationV2 animation;
	public OrientationController oc;

	// Start is called before the first frame update
	void Start()
	{
	}
	
    // Update is called once per frame
    void Update()
    {
    }
	
	void FixedUpdate()
	{
	}
	

	
	public void recordInput(int tick){
		if(animation.rightInput){
			attributes.rightInputPath[tick] = 1;
		}
		else{
			attributes.rightInputPath[tick] = 0;
		}
		if(animation.leftInput){
			attributes.leftInputPath[tick] = 1;
		}
		else{
			attributes.leftInputPath[tick] = 0;
		}
		//Vector3 vel = animation.rb.velocity;
		Vector3 vel = animation.rb.velocity;
		float y = vel.y;
		vel.y = 0f;
		float velMagnitude = vel.magnitude;
		
		Vector3 pos = transform.position;
		
		attributes.velMagPath[tick] = velMagnitude;
		attributes.velPathX[tick] = vel.x;
		attributes.velPathY[tick] = y;
		attributes.velPathZ[tick] = vel.z;
		attributes.posPathX[tick] = pos.x;
		attributes.posPathY[tick] = pos.y;
		attributes.posPathZ[tick] = pos.z;
		attributes.sphere1Prog[tick] = oc.sphere1_prog;
		attributes.sphere2Prog[tick] = oc.sphere2_prog;
			
		
		
	}

}
