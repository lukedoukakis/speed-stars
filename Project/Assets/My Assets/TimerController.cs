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
		attributes.velPathY[tick] = animation.velocity.y;
		attributes.velPathZ[tick] = animation.velocity.z;
		attributes.posPathY[tick] = transform.position.y;
		attributes.posPathZ[tick] = transform.position.z;
	}

}
