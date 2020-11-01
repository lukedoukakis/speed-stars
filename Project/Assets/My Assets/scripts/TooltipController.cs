using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipController : MonoBehaviour
{
	
	public GlobalController gc;
	
	[SerializeField] Image backgroundImage;
	[SerializeField] RectTransform backgroundImageT;
	[SerializeField] TextMeshProUGUI text;
	[SerializeField] RectTransform rectTransform;
	[SerializeField] RectTransform canvasRectTransform;
	[SerializeField] Vector2 padding;
	
	
	public void showUnlockReq(int raceEvent){
		if(raceEvent == RaceManager.RACE_EVENT_200M){
			if(!gc.unlockManager.unlocked_200m){
				show();
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_200m, false));
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			if(!gc.unlockManager.unlocked_400m){
				show();
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_400m, false));
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			if(!gc.unlockManager.unlocked_60m){
				show();
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_60m, false));
			}
		}
	}
	
	public void showUnlockReq(string thingToUnlock){
		if(thingToUnlock == "Character Slot"){
			show();
			if(gc.unlockManager.characterSlots < 1){
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_characterSlot, true));
			}
			else{
				setText("Slots: " + gc.unlockManager.characterSlots);
			}
		}
	}
	
	string formatListToUnlockText(List<string> list, bool canSatisfyAny){
		string str;
		if(canSatisfyAny){ str = "To unlock, achieve any:"; }
		else{ str = "To unlock, achieve:"; }
		for(int i = 0; i < list.Count; i++){
			str += "\n - "+list[i];
		}
		return str;
	}
	
	
	public void setText(string str){
		text.SetText(str);
		text.ForceMeshUpdate();
		Vector2 textSize = text.GetRenderedValues(false);
		backgroundImageT.sizeDelta = textSize + padding;
	}
	
	public void show(){
		text.enabled = true;
		backgroundImage.enabled = true;
	}
	
	public void hide(){
		text.enabled = false;
		backgroundImage.enabled = false;
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
        setText("Example Text");
		hide();
		
    }

    // Update is called once per frame
    void Update()
    {
		rectTransform.anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
    }
}
