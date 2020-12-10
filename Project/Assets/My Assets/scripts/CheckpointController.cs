using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
	public FinishLineController flc;
	public GameObject player;
	
    void OnTriggerEnter(Collider col){
		GameObject g = col.gameObject;
		if(g.tag == "Chest"){
			GameObject racer = g.transform.parent.parent.parent.parent.parent.parent.parent.gameObject;
			if(racer == player){
				flc.isActive = true;	
			}
		}
	}
}
