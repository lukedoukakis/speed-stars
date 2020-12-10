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
		if(raceEvent == RaceManager.RACE_EVENT_100M){
			if(gc.unlockManager.rank_100m > UnlockManager.NONE){
				show(-1f);
				setText(gc.unlockManager.getCurrentRankString(raceEvent));
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_200M){
			if(!gc.unlockManager.unlocked_200m){
				show(-1f);
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_200m, false));
			}
			else{
				if(gc.unlockManager.rank_200m > UnlockManager.NONE){
					show(-1f);
					setText(gc.unlockManager.getCurrentRankString(raceEvent));
				}
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_400M){
			if(!gc.unlockManager.unlocked_400m){
				show(-1f);
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_400m, false));
			}
			else{
				if(gc.unlockManager.rank_400m > UnlockManager.NONE){
					show(-1f);
					setText(gc.unlockManager.getCurrentRankString(raceEvent));
				}
			}
		}
		else if(raceEvent == RaceManager.RACE_EVENT_60M){
			if(!gc.unlockManager.unlocked_60m){
				show(-1f);
				setText(formatListToUnlockText(gc.unlockManager.unlockReqList_60m, false));
			}
			else{
				if(gc.unlockManager.rank_100m > UnlockManager.NONE){
					show(-1f);
					setText(gc.unlockManager.getCurrentRankString(raceEvent));
				}
			}
		}
	}
	
	public void showUnlockReq(string thingToUnlock){
		if(thingToUnlock == "Character Slot"){
			show(-1f);
			string s = "Slots: " + gc.unlockManager.characterSlots;
			s += "\n\n" + formatListToUnlockText(gc.unlockManager.unlockReqList_characterSlot, true);
			setText(s);
		}
	}
	
	string formatListToUnlockText(List<string> list, bool canSatisfyAny){
		string str;
		if(canSatisfyAny){ str = "Unlock by getting:"; }
		else{ str = "Unlock by getting:"; }
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
	
	public void show(float time){
		text.enabled = true;
		backgroundImage.enabled = true;
		if(time > -1f){
			StartCoroutine(hideAfterDelay(time));
		}
	}
	
	public void hide(){
		text.enabled = false;
		backgroundImage.enabled = false;
	}
	IEnumerator hideAfterDelay(float time){
		yield return new WaitForSeconds(time);
		hide();
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
