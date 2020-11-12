using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
	

	public Camera camera;
	public GameObject previewRacer;
	public GameObject colorButtonGrid;
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
	
	[SerializeField] TooltipController tooltipController;
	
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if(colorPickerOpen){
			if(Input.GetMouseButton(0)){
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
							
							att.setClothing(PlayerAttributes.FROM_THIS);
						
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
	}
	

	public void openColorPicker(string s){
		
		string[] arr = s.Split('_');
		string type = arr[0];
		int buttonIndex = int.Parse(arr[1]);
		GameObject referenceButton = colorButtonGrid.transform.Find("ColorPickerButton (" + buttonIndex + ")").gameObject;
		
		
		if(colorPickerOpen){
			closeColorPicker();
		}
		
		GameObject colorPicker = null;
		if(type == "skin"){
			colorPicker = colorPicker_skin;
		}
		else if(type == "clothing"){
			colorPicker = colorPicker_clothing;
		}
		
		colorPicker.GetComponent<RectTransform>().anchoredPosition = new Vector2(colorPicker.GetComponent<RectTransform>().anchoredPosition.x, referenceButton.GetComponent<RectTransform>().anchoredPosition.y + 100f);
		//referenceButton.GetComponent<RectTransform>().anchoredPosition + new Vector2(160f, 100f);
		colorPicker.SetActive(true);
		
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
		att.setClothing(PlayerAttributes.FROM_THIS);
	}
}
