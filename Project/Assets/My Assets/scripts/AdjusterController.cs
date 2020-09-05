using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjusterController : MonoBehaviour
{
	
	public Collider thisCollider;
	float y;
	public GameObject g;
	
    // Start is called before the first frame update
    void Start()
    {
        y = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter(Collider otherCollider){
		if(otherCollider.gameObject.tag != "Ground"){
			Physics.IgnoreCollision(thisCollider, otherCollider);
			Transform t = otherCollider.gameObject.transform;
			while(t.gameObject.tag != "Runner"){
				t = t.parent;
			}
			g = t.parent.gameObject;
			g.GetComponent<PlayerAnimationV2>().turnTowardsY(y);
		}
		
	}
}
