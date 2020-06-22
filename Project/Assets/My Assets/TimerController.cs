using System.Collections;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	
	public PlayerAttributes attributes;
	public PlayerAnimationV2 animation;

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
		Vector3 vel = animation.rb.velocity;
		Vector3 pos = transform.position;
		attributes.velPathY[tick] = vel.y;
		attributes.velPathZ[tick] = vel.z;
		attributes.posPathY[tick] = pos.y;
		attributes.posPathZ[tick] = pos.z;
	}

}
