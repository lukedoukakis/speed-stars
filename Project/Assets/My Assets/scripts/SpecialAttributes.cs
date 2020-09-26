using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttributes : MonoBehaviour
{
	
	public PlayerAttributes[] eventAttributes;

	public float torsoAngle;
	public float pace_60m;
	public float pace_100m;
	public float pace_200m;
	public float pace_400m;
	public float eThresh_60m;
	public float eThresh_100m;
	public float eThresh_200m;
	public float eThresh_400m;
	
	public void copyFromOther(SpecialAttributes other){
		this.torsoAngle = other.torsoAngle;
		this.pace_60m = other.pace_60m;
		this.pace_100m = other.pace_100m;
		this.pace_200m = other.pace_200m;
		this.pace_400m = other.pace_400m;
		this.eThresh_60m = other.eThresh_60m;
		this.eThresh_100m = other.eThresh_100m;
		this.eThresh_200m = other.eThresh_200m;
		this.eThresh_400m = other.eThresh_400m;
		
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
