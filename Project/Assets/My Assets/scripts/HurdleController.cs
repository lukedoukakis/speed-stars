using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleController : MonoBehaviour
{
	
	public Collider barCollider;
	public Collider stickCollider_right;
	public Collider stickCollider_left;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag != "Ground"){
			StartCoroutine(wait(.25f));
			Collider c = collision.collider;
			Physics.IgnoreCollision(barCollider, c);
			Physics.IgnoreCollision(stickCollider_right,c);
			Physics.IgnoreCollision(stickCollider_left, c);
		}
	}
	
	IEnumerator wait(float seconds){
		yield return new WaitForSeconds(seconds);
	}
}
