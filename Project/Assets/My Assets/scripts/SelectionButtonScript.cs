using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionButtonScript : MonoBehaviour
{
	public SelectionListScript list;

	public bool selected;
	public string id;
	public string name;
	public string time;
	
	public static Color32 playerButtonColor = new Color32(250, 196, 192, 255);
	public static Color32 ghostButtonColor = new Color32(207, 255, 236, 255);
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void setFromRacer(string _id, int raceEvent){
		// -----------------
		this.id = _id;
		name = PlayerPrefs.GetString(_id).Split(':')[1];
		gameObject.transform.Find("NameText").GetComponent<Text>().text = name;
		// -----------------
		time = PlayerPrefs.GetString(_id).Split(':')[3 + raceEvent];
		if(float.Parse(time) == -1f){
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = "--";
		}else{
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = float.Parse(time).ToString("F2");
		}	
	}
	
	public void removeThisButtonAndForgetAssociatedRacer(){
		if(selected){
			list.numSelected--;
			list.selectedButtonIDs.Remove(id);
		}
		list.gc.forgetRacer(id, new string[]{list.sourceMemory}, false);
		list.removeButton(id);
		
	}
	
	
	public void toggle(){
		// if trying to select
		if(!selected){
			// select if numSelected less than maxSelectable
			if(list.numSelected < list.maxSelectable){
				if(!list.getButton(id).GetComponent<SelectionButtonScript>().selected){
					setColor("selected");
					list.numSelected++;
					list.selectedButtonIDs.Add(id);
					if(list == list.gc.playerSelectButtonList){
						list.setPreviewRacer(id);
					}
					selected = true;
				}
			}
			// else, if replaceLastSelection true, unselect last selected button
			else{
				if(list.replaceLastSelection){
					if(list.selectedButtonIDs != null){
						if(list.selectedButtonIDs.Count != 0){
						Debug.Log("replacing last selection");
						Debug.Log("replaced button id: " + list.getButton(list.selectedButtonIDs[0]).GetComponent<SelectionButtonScript>().id);
						list.minSelectable--;
						list.getButton(list.selectedButtonIDs[list.selectedButtonIDs.Count-1]).GetComponent<SelectionButtonScript>().toggle(false);
						list.minSelectable++;
						}
					}
				}
				setColor("selected");
				list.numSelected++;
				list.selectedButtonIDs.Add(id);
				if(list == list.gc.playerSelectButtonList){
					list.setPreviewRacer(id);
				}
				selected = true;
			}
			if(list.pa != null){
				list.pa.resetPos();
			}
		}
		// if trying to deselect, deselect if numSelected greater than minSelectable, otherwise do nothing
		else{
			if(list.numSelected > list.minSelectable){
				setColor("unselected");
				list.numSelected--;
				list.selectedButtonIDs.Remove(id);
				selected = false;
			}
			else{

			}
		}
		if(list == list.gc.ghostSelectButtonList){
			list.gc.setupManager.botCount_max = 7 - list.numSelected;
			if(list.gc.setupManager.botCount > list.gc.setupManager.botCount_max){
				list.gc.setupManager.incrementBotCount(list.gc.setupManager.botCount_max - list.gc.setupManager.botCount);
				//list.gc.setupManager.botCount = list.gc.setupManager.botCount_max;
				//list.gc.setupManager.botCountText.text = list.gc.setupManager.botCount.ToString();
			}
		}
		if(list.numSelected == 0){
			if(list.pa != null){
				list.pa.setVisibility(false);
			}
		}else{
			if(list.pa != null){
				list.pa.setVisibility(true);
				list.pa.land();
			}
		}
	}
	
	public void toggle(bool setting){
		if(selected != setting){
			toggle();
		}
	}
	
	
	
	public void setColor(string state){
		Image image = GetComponent<Image>();
		float h,s,v;
		Color.RGBToHSV(image.color, out h, out s, out v);
		
		if(state == "selected"){
			s *= 3f;
		} else{ s /= 3f; }
		
		image.color = Color.HSVToRGB(h, s, v);
	}
	
	
	public void showDeleteDialog(){
		
		// if no ddc (ghost button list)
		if(list.ddc == null){
			removeThisButtonAndForgetAssociatedRacer();
			return;
		}
		
		// don't allow if only button available
		if(list.buttonIDs.Count <= 1){
			return;
		}
		
		SelectionButtonScript button_list1 = list.ddc.list1.getButton(this.id).GetComponent<SelectionButtonScript>();
		list.ddc.list1.buttonToDelete = list.ddc.list1.getButton(this.id).GetComponent<SelectionButtonScript>();
		
		if(list.ddc.list2.getButton(this.id) != null){
			SelectionButtonScript button_list2 = list.ddc.list2.getButton(this.id).GetComponent<SelectionButtonScript>();
			list.ddc.list2.buttonToDelete = list.ddc.list2.getButton(this.id).GetComponent<SelectionButtonScript>();
		}
	
		list.ddc.show();
	}
	
	
	public void init(SelectionListScript s){
		list = s;
		transform.SetParent(list.grid.transform, false);
	}
}
