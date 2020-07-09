using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionButtonScript : MonoBehaviour
{
	public SelectionListScript list;

	public bool selected;
	public string selectedColorCode = "#B96481";
	public string unselectedColorCode = "#CDB6CF";
	public string id;
	public string name;
	public string time;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void setFromRacer(string id){
		// -----------------
		this.id = id;
		name = PlayerPrefs.GetString(id).Split(':')[1];
		gameObject.transform.Find("NameText").GetComponent<Text>().text = name;
		// -----------------
		time = PlayerPrefs.GetString(id).Split(':')[2];
		if(float.Parse(time) == -1f){
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = "--";
		}else{
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = time;
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
		selected = !selected;
		if(selected){
			if(list.numSelected < list.maxSelectable){
				setColor(selectedColorCode);
				list.numSelected++;
				list.selectedButtonIDs.Add(id);
			}
			else{
				// max selectable message
				if(list.replaceLastSelection){
					list.getButton(list.selectedButtonIDs[0]).GetComponent<SelectionButtonScript>().toggle();
					setColor(selectedColorCode);
					list.numSelected++;
					list.selectedButtonIDs.Add(id);
				}
				else{
					selected = false;
				}
			}
		}
		else{
			setColor(unselectedColorCode);
			list.numSelected--;
			list.selectedButtonIDs.Remove(id);
		}
	}
	
	public void toggle(bool setting){
		selected = !setting;
		toggle();
	}
	
	
	
	public void setColor(string colorCode){
		Color color;
		if(ColorUtility.TryParseHtmlString(colorCode, out color)){
			GetComponent<Image>().color = color;
			transform.Find("Delete Button").gameObject.GetComponent<Image>().color = color;
		}
	}
	
	public void init(){
		selected = false;
	}
}
