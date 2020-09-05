using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothingPicker : MonoBehaviour
{
	
	public GameObject previewRacer;
	public GameObject previewPlatform;
	
	public PlayerAttributes att;
	public ClothingManager clothingManager;
	
	public Button buttonDummy_prev;
	public Button buttonDummy_next;
	public Button buttonHeadband_prev;
	public Button buttonHeadband_next;
	public Button buttonTop_prev;
	public Button buttonTop_next;
	public Button buttonSleeve_prev;
	public Button buttonSleeve_next;
	public Button buttonBottoms_prev;
	public Button buttonBottoms_next;
	public Button buttonShoes_prev;
	public Button buttonShoes_next;
	public Button buttonSocks_prev;
	public Button buttonSocks_next;
	
	
	
	
    // Start is called before the first frame update
    void Start()
    {
        att = previewRacer.GetComponent<PlayerAttributes>();
		clothingManager = att.clothingManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void cycleMesh(string article){
		int meshNum;
		// -----------------
		if(article == "dummy"){
			att.setBodyProportions(PlayerAttributes.ATTRIBUTES_RANDOM);
		}
		else if(article == "headband"){
			meshNum = att.headbandMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.headbandMeshes.Length){
				meshNum = 0;
			}
			att.headbandMeshNumber = meshNum;
		}
		else if(article == "top"){
			meshNum = att.topMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.topMeshes.Length){
				meshNum = 0;
			}
			att.topMeshNumber = meshNum;
		}
		else if(article == "sleeve"){
			meshNum = att.sleeveMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.sleeveMeshes.Length){
				meshNum = 0;
			}
			att.sleeveMeshNumber = meshNum;
		}
		else if(article == "bottoms"){
			meshNum = att.bottomsMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.bottomsMeshes.Length){
				meshNum = 0;
			}
			att.bottomsMeshNumber = meshNum;
		}
		else if(article == "shoes"){
			meshNum = att.shoesMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.shoesMeshes.Length){
				meshNum = 0;
			}
			att.shoesMeshNumber = meshNum;
		}
		else if(article == "socks"){
			meshNum = att.socksMeshNumber;
			meshNum++;
			if(meshNum >= clothingManager.socksMeshes.Length){
				meshNum = 0;
			}
			att.socksMeshNumber = meshNum;
		}
		// -----------------
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
	}
	
	public void setRandomClothing(){
		PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
		int[] meshNumbers = att.clothingManager.getRandomMeshNumbers();
		// -----------------
		int j = 0;
		att.dummyMeshNumber = meshNumbers[j]; j++;
		att.topMeshNumber = meshNumbers[j]; j++;
		att.bottomsMeshNumber = meshNumbers[j]; j++;
		att.shoesMeshNumber = meshNumbers[j]; j++;
		att.socksMeshNumber = meshNumbers[j]; j++;
		att.headbandMeshNumber = meshNumbers[j]; j++;
		att.sleeveMeshNumber = meshNumbers[j]; j++;
		// -----------------
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
		att.setBodyProportions(PlayerAttributes.ATTRIBUTES_RANDOM);
	}
	
	public void resetPreviewPosition(){
		previewRacer.transform.position = previewPlatform.transform.position + Vector3.up*.5f;
		//previewRacer.GetComponent<PlayerAnimationV2>().energy = 100f;
	}
}
