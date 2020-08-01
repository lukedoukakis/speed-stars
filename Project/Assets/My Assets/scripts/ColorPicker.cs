using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
	

	public Camera camera;
	public GameObject previewRacer;
	public GameObject colorPicker_skin;
		public GameObject skinTextureObject;
	public GameObject colorPicker_clothing;
		public GameObject clothingTextureObject;
	public Color selectedColor;
	
	bool colorPickerOpen;
	
	public Button buttonDummy;
	public Button buttonHeadband;
	public Button buttonTop;
	public Button buttonSleeve;
	public Button buttonBottoms;
	public Button buttonShoes;
	public Button buttonSocks;
	
	public Button[] buttons;
	public Button selectedButton;
	public string selectedArticle;
	
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if(colorPickerOpen){
			if(Input.GetMouseButtonUp(0)){
				RaycastHit hit;
				if(Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)){
					Renderer rend = hit.transform.GetComponent<Renderer>();
					MeshCollider meshCollider = hit.collider as MeshCollider;
					if(rend.gameObject == skinTextureObject || rend.gameObject == clothingTextureObject){
						/*
						if(rend == null){
							Debug.Log("REND NULL");
						}
						if(rend.sharedMaterial == null){
							Debug.Log("REND SHAREDMATERIAL NULL");
						}
						if(rend.sharedMaterial.mainTexture == null){
							Debug.Log("REND SHAREDMATERIAL MAINTEXTURE NULL");
						}
						if(meshCollider == null){
							Debug.Log("MESHCOLLIDER NULL");
						}
						*/
						if(!(rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)){
							Texture2D tex = rend.material.mainTexture as Texture2D;
							Vector2 pixelUV = hit.textureCoord;
							pixelUV.x *= tex.width;
							pixelUV.y *= tex.height;
						
						
							selectedColor = tex.GetPixel(Mathf.FloorToInt(pixelUV.x) , Mathf.FloorToInt(pixelUV.y));
							Debug.Log("selectedColor: " + selectedColor.r + ", "+ selectedColor.g + ", "+ selectedColor.b);
							
							PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
							if(selectedArticle == "dummy"){
								att.dummyRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "headband"){
								att.headbandRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "top"){
								att.topRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "sleeve"){
								att.sleeveRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "bottoms"){
								att.bottomsRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "shoes"){
								att.shoesRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							else if(selectedArticle == "socks"){
								att.socksRGB = new float[]{selectedColor.r,  selectedColor.g,  selectedColor.b};
							}
							
							att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
						
						}
					}
				}	 
			}
		}  
    }
	
	
	public void setSelectedArticle(string article){
		selectedArticle = article;
		// -----------------
		float yOffset = 27.27f;
		if(selectedArticle == "dummy"){
			yOffset *= 1f;
		}
		else if(selectedArticle == "headband"){
			yOffset *= 2f;
		}
		else if(selectedArticle == "top"){
			yOffset *= 3f;
		}
		else if(selectedArticle == "sleeve"){
			yOffset *= 4f;
		}
		else if(selectedArticle == "bottoms"){
			yOffset *= 5f;
		}
		else if(selectedArticle == "shoes"){
			yOffset *= 6f;
		}
		else if(selectedArticle == "socks"){
			yOffset *= 7f;
		}
		Vector3 colorPickerPos = new Vector3(34f, 177f - yOffset, 0f);
		//colorPicker_skin.transform.position = colorPickerPos;
		//colorPicker_clothing.transform.position = colorPickerPos;
	}
	

	public void openColorPicker(string type){
		if(colorPickerOpen){
			closeColorPicker();
		}
		if(type == "skin"){
			colorPicker_skin.SetActive(true);
		}
		else if(type == "clothing"){
			colorPicker_clothing.SetActive(true);
		}
		colorPickerOpen = true;
	}
	
	public void closeColorPicker(){
		colorPicker_skin.SetActive(false);
		colorPicker_clothing.SetActive(false);
		colorPickerOpen = false;
	}
	
	public void setRandomColors(){
		PlayerAttributes att = previewRacer.GetComponent<PlayerAttributes>();
		Color[] colors = att.clothingManager.getRandomColors();
		Color c;
		// -----------------
		int j = 0;
		c = colors[j];
		att.dummyRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.topRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.bottomsRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.shoesRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.socksRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.headbandRGB = new float[]{ c.r, c.g, c.b}; j++;
		c = colors[j];
		att.sleeveRGB = new float[]{ c.r, c.g, c.b}; j++;
		// -----------------
		att.setClothing(PlayerAttributes.ATTRIBUTES_FROM_THIS);
	}
}
