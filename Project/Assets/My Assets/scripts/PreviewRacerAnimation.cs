using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRacerAnimation : MonoBehaviour
{
	
	[SerializeField] Animator animator;
	
	public void land(){
		StartCoroutine(previewRacerLand());
	}
	IEnumerator previewRacerLand(){
		animator.SetBool("land", true);
		yield return new WaitForSeconds(.2f);
		animator.SetBool("land", false);
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
