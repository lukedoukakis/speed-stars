using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : MonoBehaviour
{
	
	[SerializeField] Animator animator;
	
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(initAnimation()); 
    }
	
	
	IEnumerator initAnimation(){
		int r = 0;
		while(r != 1){
			r = Random.Range(0, 11);
			yield return new WaitForSeconds(.1f);
		}
		animator.SetBool("cheer", true);
		
		r = Random.Range(0, 2);
		if(r == 1){
			Vector3 flippedScale = gameObject.transform.localScale;
			flippedScale.x *= -1f;
			gameObject.transform.localScale = flippedScale;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
