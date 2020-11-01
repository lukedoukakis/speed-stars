using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteDialogController : MonoBehaviour
{
	
	[SerializeField] GlobalController gc;
	
	[SerializeField] GameObject root;
	[SerializeField] public SelectionListScript list1;
	[SerializeField] public SelectionListScript list2;
	[SerializeField] RectTransform rectTransform;
	[SerializeField] RectTransform canvasRectTransform;
	[SerializeField] Toggle deleteGhostToggle;
	
	
	public void delete(){
		list1.deleteButtonToDelete();
		if(deleteGhostToggle.isOn){
			list2.deleteButtonToDelete();
		}
		gc.unlockManager.unlockCharacterSlot();
		
	}
	
	public void show(){
		rectTransform.anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x + new Vector3(0f, -270f, 0f);
		root.SetActive(true);
		
	}
	
	public void hide(){
		root.SetActive(false);
	}
	
    // Start is called before the first frame update
    void Start()
    {
        hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
