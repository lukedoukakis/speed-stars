using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{
	
	public GameObject thighRight;
	public GameObject thighLeft;
	
	
	public float maxHipFlex;
	public float minHipFlex;
	public float currentHipFlexRight;
	public float currentHipFlexLeft;
	public float hipSpeed;
	
	
	public float rightInput;
	public float leftInput;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if(Input.GetKey(KeyCode.D)){
			Debug.Log("right pressed");
			rightInput = 1f;
		} else { rightInput = -1f; }
		if(Input.GetKey(KeyCode.A)){
			leftInput = 1f;
		} else { rightInput = -1f; }
		
		if(currentHipFlexRight <= maxHipFlex && currentHipFlexRight >= minHipFlex){
			currentHipFlexRight += hipSpeed * rightInput * Time.deltaTime;
			thighRight.transform.Rotate(Vector3.up * rightInput * 60f * Time.deltaTime);
		}
		
		
		
        
    }
}
