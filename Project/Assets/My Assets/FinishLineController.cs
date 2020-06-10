using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	void OnTriggerEnter(Collider col){
		GameObject g = col.gameObject;
		if(g.tag == "Chest"){
			GameObject racer = g.transform.parent.parent.parent.parent.parent.parent.parent.gameObject;
			racer.GetComponent<TimerController>().stop();
		}
	}
		
		
		
		
	
	
}
