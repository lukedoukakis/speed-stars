using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRacerAnimation : MonoBehaviour
{
	
	[SerializeField] Animator animator;
	[SerializeField] Transform bodyT;
	[SerializeField] Transform footT;
	[SerializeField] Transform platformT;
	
	[SerializeField] Transform visibleLoc;
	[SerializeField] Transform invisibleLoc;
	
	public void resetPos(){
		bodyT.position += transform.TransformDirection(Vector3.up);
		RaycastHit hit;
		Physics.Raycast(footT.position, footT.TransformDirection(Vector3.down), out hit);
		float distance = hit.distance;
		bodyT.transform.position += transform.TransformDirection(Vector3.down*distance);
		
	}
	
	public void land(){
		StartCoroutine(previewRacerLand());
	}
	IEnumerator previewRacerLand(){
		animator.SetBool("land", true);
		yield return new WaitForSeconds(.2f);
		animator.SetBool("land", false);
	}
	
	public void setVisibility(bool visible){
		if(visible){
			bodyT.transform.position = visibleLoc.position;
		}
		else{
			bodyT.transform.position = invisibleLoc.position;
		}
		platformT.transform.position = bodyT.transform.position;
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
