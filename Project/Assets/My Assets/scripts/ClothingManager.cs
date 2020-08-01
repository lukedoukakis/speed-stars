using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingManager : MonoBehaviour
{
	
	public int numberOfArticles;
	
	// MESHES
	// -----------------
	public Mesh[] dummyMeshes;
	public Mesh[] topMeshes;
	public Mesh[] bottomsMeshes;
	public Mesh[] shoesMeshes;
	public Mesh[] socksMeshes;
	public Mesh[] headbandMeshes;
	public Mesh[] sleeveMeshes;
	// -----------------
	
	// MATERIALS
	// -----------------
	public Material[] dummyMaterials;
	public Material[] topMaterials;
	public Material[] bottomsMaterials;
	public Material[] shoesMaterials;
	public Material[] socksMaterials;
	public Material[] headbandMaterials;
	public Material[] sleeveMaterials;
	// -----------------
	public Shader shader_renderOnTop;
	// -----------------

	public string[] skinTones = new string[]{"#ffad60","#ffe39f",   "#c69076","#af6e51","#843722","#3d0c02","#260701"};
	
	// Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public Mesh getMesh(string article, int index){	
		if(article == "dummy"){
			return dummyMeshes[index];
		}
		else if(article == "top"){
			return topMeshes[index];
		}
		else if(article == "bottoms"){
			return bottomsMeshes[index];
		}
		else if(article == "shoes"){
			return shoesMeshes[index];
		}
		else if(article == "socks"){
			return socksMeshes[index];
		}
		else if(article == "headband"){
			return headbandMeshes[index];
		}
		else if(article == "sleeve"){
			return sleeveMeshes[index];
		}
		else{
			return null;
		}
	}
	
	public Material getMaterial(string article, int index){
		if(article == "dummy"){
			return dummyMaterials[index];
		}
		else if(article == "top"){
			return topMaterials[index];
		}
		else if(article == "bottoms"){
			return bottomsMaterials[index];
		}
		else if(article == "shoes"){
			return shoesMaterials[index];
		}
		else if(article == "socks"){
			return socksMaterials[index];
		}
		else if(article == "headband"){
			return headbandMaterials[index];
		}
		else if(article == "sleeve"){
			return sleeveMaterials[index];
		}
		else{
			return null;
		}
	}
	
	
	
	
	
	public int[] getRandomMeshNumbers(){
		string style;
		int[] randomMeshNumbers = new int[numberOfArticles];
		int n;
		// -----------------
		
		// determine style
		style = "";
		n = Random.Range(0, 4);
		if(n < 3){
			style = "short";
		} else{
			style = "long";
		}
		
		// dummy
		randomMeshNumbers[0] = Random.Range(0, dummyMeshes.Length);
		
		// top and bottoms
		if(style == "short"){
			randomMeshNumbers[1] = 0;
			randomMeshNumbers[2] = 0;
		}else{
			randomMeshNumbers[1] = 1;
			randomMeshNumbers[2] = 1;
		}
		
		// shoes
		randomMeshNumbers[3] = Random.Range(0, shoesMeshes.Length);
		
		// socks
		n = Random.Range(0, 2);
		if(n == 0){ 
			n = socksMeshes.Length-1;
		}
		else{
			n = Random.Range(0, socksMeshes.Length-1);
		}
		randomMeshNumbers[4] = n;
		
		// headband
		n = Random.Range(0, 4);
		if(n < 3){ 
			n = headbandMeshes.Length-1;
		}
		else{
			n = Random.Range(0, headbandMeshes.Length-1);
		}
		randomMeshNumbers[5] = n;
		
		// sleeve
		if(style == "short"){
			n = Random.Range(0, 7);
			if(n < 6){ 
				n = sleeveMeshes.Length-1;
			}
			else{
				n = Random.Range(0, sleeveMeshes.Length-1);
			}
		}
		else{
			n = sleeveMeshes.Length-1;
		}
		randomMeshNumbers[6] = n;
		// -----------------
		return randomMeshNumbers;
	}
	
	public int[] getRandomMaterialNumbers(){
		int[] randomMaterialNumbers = new int[numberOfArticles];
		// -----------------
		randomMaterialNumbers[0] = Random.Range(0, dummyMaterials.Length);
		randomMaterialNumbers[1] = Random.Range(0, topMaterials.Length);
		randomMaterialNumbers[2] = Random.Range(0, bottomsMaterials.Length);
		randomMaterialNumbers[3] = Random.Range(0, shoesMaterials.Length);
		randomMaterialNumbers[4] = Random.Range(0, socksMaterials.Length);
		randomMaterialNumbers[5] = Random.Range(0, headbandMaterials.Length);
		randomMaterialNumbers[6] = Random.Range(0, sleeveMaterials.Length);
		// -----------------
		return randomMaterialNumbers;
	}
	
	public Color[] getRandomColors(){
		Color[] randomClothingColors;
		int random;
		// -----------------
		
		// dummy
		Color dummy = Color.white;
		Color c;
		string s = skinTones[Random.Range(0, skinTones.Length)];
		if(ColorUtility.TryParseHtmlString(s, out c)){
			 dummy = c;
		}
		
		// top
		Color top = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
		
		// bottoms
		Color bottoms = Color.white;
		random = Random.Range(0,3);
		switch (random){
			case 0 :
				bottoms = top;
				break;
			case 1 :
				bottoms = Color.black;
				break;
			case 2 :
				break;
		}
		
		// shoes
		Color shoes = Color.white;
		random = Random.Range(0,3);
		switch (random){
			case 0 :
				shoes = top;
				break;
			case 1 :
				shoes = bottoms;
				break;
			case 2 :
				break;
		}
		
		// socks
		Color socks = Color.white;
		random = Random.Range(0,2);
		switch (random){
			case 0 :
				socks = shoes;
				break;
			case 1 :
				break;
		}
		
		Color headband = Color.white;
		random = Random.Range(0,3);
		switch (random){
			case 0 :
				headband = top;
				break;
			case 1 :
				headband = bottoms;
				break;
			case 2 :
				break;
		}
		
		Color sleeve = Color.white;
		random = Random.Range(0,2);
		switch (random){
			case 0 :
				sleeve = top;
				break;
			case 1 :
				break;
		}
		// -----------------
		
		randomClothingColors = new Color[]{dummy, top, bottoms, shoes, socks, headband, sleeve};
		return randomClothingColors;
	}
}
