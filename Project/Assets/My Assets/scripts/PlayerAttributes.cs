using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
	
	public ClothingManager clothingManager;
	// -----------------
	public static int DEFAULT_PATH_LENGTH = 10000;
	// -----------------

	public static int ATTRIBUTES_RANDOM = 1;
	public static int ATTRIBUTES_FROM_THIS = 2;
	public static int ATTRIBUTES_DEFAULT = 3;
	
	public static int ATTRIBUTES_LEGEND_USAINBOLT = 4;
	public static int ATTRIBUTES_LEGEND_MICHAELJOHNSON = 5;
	public static int ATTRIBUTES_LEGEND_YOHANBLAKE = 6;
	public static int ATTRIBUTES_LEGEND_JESSEOWENS = 7;
	public static int ATTRIBUTES_LEGEND_WAYDEVANNIEKERK = 8;
	
	// -----------------
	
	// id info
	public string id;
	public string racerName;
	public float[] personalBests;
	// -----------------
	
	// race info
	public int lane;
	public bool isRacing;
	public float finishTime;
	public string resultString;
	public string resultTag;
	public string resultColor;
	// -----------------
	
	// ghost info
	public int pathLength;
	public int leanLockTick;
	public float[] velMagPath;
	public float[] velPathX;
	public float[] velPathY;
	public float[] velPathZ;
	public float[] posPathX;
	public float[] posPathY;
	public float[] posPathZ;
	public int[] rightInputPath;
	public int[] leftInputPath;
	public float[] sphere1Prog;
	public float[] sphere2Prog;
	// -----------------
	
	// material info
	public SkinnedMeshRenderer smr_dummy;
	public SkinnedMeshRenderer smr_top;
	public SkinnedMeshRenderer smr_bottoms;
	public SkinnedMeshRenderer smr_shoes;
	public SkinnedMeshRenderer smr_socks;
	public SkinnedMeshRenderer smr_headband;
	public SkinnedMeshRenderer smr_sleeve;
	public int dummyMeshNumber;
	public int topMeshNumber;
	public int bottomsMeshNumber;
	public int shoesMeshNumber;
	public int socksMeshNumber;
	public int headbandMeshNumber;
	public int sleeveMeshNumber;
	public int dummyMaterialNumber;
	public int topMaterialNumber;
	public int bottomsMaterialNumber;
	public int shoesMaterialNumber;
	public int socksMaterialNumber;
	public int headbandMaterialNumber;
	public int sleeveMaterialNumber;
	public float[] dummyRGB;
	public float[] topRGB;
	public float[] bottomsRGB;
	public float[] shoesRGB;
	public float[] socksRGB;
	public float[] headbandRGB;
	public float[] sleeveRGB;
	// -----------------
	
	
	
	// body info
	public Transform head;
	public Transform neck;
	public Transform torso;
	public Transform thighRight;
	public Transform thighLeft;
	public Transform shinRight;
	public Transform shinLeft;
	public Transform upperArmRight;
	public Transform upperArmLeft;
	public Transform lowerArmRight;
	public Transform lowerArmLeft;
	public Transform footRight;
	public Transform footLeft;
	// --
	public float headX;
	public float headY;
	public float headZ;
	public float neckX;
	public float neckY;
	public float neckZ;
	public float torsoX;
	public float torsoY;
	public float torsoZ;
	public float armX;
	public float armY;
	public float armZ;
	public float legX;
	public float legY;
	public float legZ;
	public float feetX;
	public float feetY;
	public float feetZ;
	// --
	public float height;
	public float weight;
	
	// -----------------
	
	// stats info
	public float POWER;
	public float TRANSITION_PIVOT_SPEED;			
	public float QUICKNESS;							// base 1
	public float KNEE_DOMINANCE;					// base 1
	public float TURNOVER;							// base 1
	public float FITNESS;							// base 1
	
	// -----------------
	
	// animation info
	public Animator animator;
	public RuntimeAnimatorController[] animatorControllers;
	public int animatorNum;
	public float armSpeedFlex;
	public float armSpeedExtend;
	
	
	
	
	// Start is called before the first frame update
    void Start()
    {
		
		//Debug.Log("------------START-----------------");
		// -----------------
		//racerName = "DefaultName";
		resultTag = "";
		if(tag.StartsWith("Player")){
			resultColor = "green";
		}
		else if(tag.StartsWith("Ghost")){
			resultColor = "grey";
		}
		else if(tag.StartsWith("Bot")){
			resultColor = "blue";
		}
		// -----------------

    }

    // Update is called once per frame
    void Update()
    {
	
    }
	
	
	public void setClothing(int setting){
		
		Mesh mesh;
		Material material;
		int dummyMeshNum = 0;
		int topMeshNum = 0;
		int bottomsMeshNum = 0;
		int shoesMeshNum = 0;
		int socksMeshNum = 0;
		int headbandMeshNum = 0;
		int sleeveMeshNum = 0;
		int dummyMaterialNum = 0;
		int topMaterialNum = 0;
		int bottomsMaterialNum = 0;
		int shoesMaterialNum = 0;
		int socksMaterialNum = 0;
		int headbandMaterialNum = 0;
		int sleeveMaterialNum = 0;
		Color dummyColor; ColorUtility.TryParseHtmlString(clothingManager.skinTones[Random.Range(0, clothingManager.skinTones.Length)], out dummyColor);
		Color topColor = Color.red;
		Color bottomsColor = Color.white;
		Color shoesColor = Color.white;
		Color socksColor = Color.white;
		Color headbandColor = Color.white;
		Color sleeveColor = Color.red;
		// -----------------
		if(setting == ATTRIBUTES_DEFAULT){
			// do nothing
		}	
		else if(setting == ATTRIBUTES_FROM_THIS){
			// get values for meshes, materials, and colors from self
			dummyMeshNum = this.dummyMeshNumber;
			topMeshNum = this.topMeshNumber;
			bottomsMeshNum = this.bottomsMeshNumber;
			shoesMeshNum = this.shoesMeshNumber;
			socksMeshNum = this.socksMeshNumber;
			headbandMeshNum = this.headbandMeshNumber;
			sleeveMeshNum = this.sleeveMeshNumber;
			dummyMaterialNum = this.dummyMaterialNumber;
			topMaterialNum = this.topMaterialNumber;
			bottomsMaterialNum = this.bottomsMaterialNumber;
			shoesMaterialNum = this.shoesMaterialNumber;
			socksMaterialNum = this.socksMaterialNumber;
			headbandMaterialNum = this.headbandMaterialNumber;
			sleeveMaterialNum = this.sleeveMaterialNumber;
			dummyColor = new Color(this.dummyRGB[0], this.dummyRGB[1], this.dummyRGB[2]);
			topColor = new Color(this.topRGB[0], this.topRGB[1], this.topRGB[2]);
			bottomsColor = new Color(this.bottomsRGB[0], this.bottomsRGB[1], this.bottomsRGB[2]);
			shoesColor = new Color(this.shoesRGB[0], this.shoesRGB[1], this.shoesRGB[2]);
			socksColor = new Color(this.socksRGB[0], this.socksRGB[1], this.socksRGB[2]);
			headbandColor = new Color(this.headbandRGB[0], this.headbandRGB[1], this.headbandRGB[2]);
			sleeveColor = new Color(this.sleeveRGB[0], this.sleeveRGB[1], this.sleeveRGB[2]);
		}
		else if(setting == ATTRIBUTES_RANDOM){
			
			// get random values for meshes, materials, and colors
			int i;
			
			int[] randomMeshes = clothingManager.getRandomMeshNumbers();
			i = 0;
			dummyMeshNum = randomMeshes[i]; i++;
			topMeshNum = randomMeshes[i]; i++;
			bottomsMeshNum = randomMeshes[i]; i++;
			shoesMeshNum = randomMeshes[i]; i++;
			socksMeshNum = randomMeshes[i]; i++;
			headbandMeshNum = randomMeshes[i]; i++;
			sleeveMeshNum = randomMeshes[i]; i++;
				
			int[] randomMaterials = clothingManager.getRandomMaterialNumbers();
			i = 0;
			dummyMaterialNum = randomMaterials[i]; i++;
			topMaterialNum = randomMaterials[i]; i++;
			bottomsMaterialNum = randomMaterials[i]; i++;
			shoesMaterialNum = randomMaterials[i]; i++;
			socksMaterialNum = randomMaterials[i]; i++;
			headbandMaterialNum = randomMaterials[i]; i++;
			sleeveMaterialNum = randomMaterials[i]; i++;
			
			// get random colors
			Color[] randomColors = clothingManager.getRandomColors();
			int j = 0;
			dummyColor = randomColors[j]; j++;
			topColor = randomColors[j]; j++;
			bottomsColor = randomColors[j]; j++;
			shoesColor = randomColors[j]; j++;
			socksColor = randomColors[j]; j++;
			headbandColor = randomColors[j]; j++;
			sleeveColor = randomColors[j]; j++;
		}
		else if(setting >= 4){
			dummyMeshNum = int.Parse(TextReader.getAttribute(setting, "dummyMesh"));
			topMeshNum = int.Parse(TextReader.getAttribute(setting, "topMesh"));
			bottomsMeshNum = int.Parse(TextReader.getAttribute(setting, "bottomsMesh"));
			shoesMeshNum = int.Parse(TextReader.getAttribute(setting, "shoesMesh"));
			socksMeshNum = int.Parse(TextReader.getAttribute(setting, "socksMesh"));
			headbandMeshNum = int.Parse(TextReader.getAttribute(setting, "headbandMesh"));
			sleeveMeshNum = int.Parse(TextReader.getAttribute(setting, "sleeveMesh"));
			dummyMaterialNum = int.Parse(TextReader.getAttribute(setting, "dummyMaterial"));
			topMaterialNum = int.Parse(TextReader.getAttribute(setting, "topMaterial"));
			bottomsMaterialNum = int.Parse(TextReader.getAttribute(setting, "bottomsMaterial"));
			shoesMaterialNum = int.Parse(TextReader.getAttribute(setting, "shoesMaterial"));
			socksMaterialNum = int.Parse(TextReader.getAttribute(setting, "socksMaterial"));
			headbandMaterialNum = int.Parse(TextReader.getAttribute(setting, "headbandMaterial"));
			sleeveMaterialNum = int.Parse(TextReader.getAttribute(setting, "sleeveMaterial"));
			dummyColor = new Color(float.Parse(TextReader.getAttribute(setting, "dummyColorR")), float.Parse(TextReader.getAttribute(setting, "dummyColorG")), float.Parse(TextReader.getAttribute(setting, "dummyColorB")));
			topColor = new Color(float.Parse(TextReader.getAttribute(setting, "topColorR")), float.Parse(TextReader.getAttribute(setting, "topColorG")), float.Parse(TextReader.getAttribute(setting, "topColorB")));
			bottomsColor = new Color(float.Parse(TextReader.getAttribute(setting, "bottomsColorR")), float.Parse(TextReader.getAttribute(setting, "bottomsColorG")), float.Parse(TextReader.getAttribute(setting, "bottomsColorB")));
			shoesColor = new Color(float.Parse(TextReader.getAttribute(setting, "shoesColorR")), float.Parse(TextReader.getAttribute(setting, "shoesColorG")), float.Parse(TextReader.getAttribute(setting, "shoesColorB")));
			socksColor = new Color(float.Parse(TextReader.getAttribute(setting, "socksColorR")), float.Parse(TextReader.getAttribute(setting, "socksColorG")), float.Parse(TextReader.getAttribute(setting, "socksColorB")));
			headbandColor = new Color(float.Parse(TextReader.getAttribute(setting, "headbandColorR")), float.Parse(TextReader.getAttribute(setting, "headbandColorG")), float.Parse(TextReader.getAttribute(setting, "headbandColorB")));
			sleeveColor = new Color(float.Parse(TextReader.getAttribute(setting, "sleeveColorR")), float.Parse(TextReader.getAttribute(setting, "sleeveColorG")), float.Parse(TextReader.getAttribute(setting, "sleeveColorB")));
		}
			
		/*
		int[] meshNumbers = new int[]{ dummyMeshNum, topMeshNum, bottomsMeshNum, shoesMeshNum, socksMeshNum, headbandMeshNum, sleeveMeshNum};
		int[] materialNumbers = new int[]{ dummyMaterialNum, topMaterialNum, bottomsMaterialNum, shoesMaterialNum, socksMaterialNum, headbandMaterialNum, sleeveMaterialNum};
		Color[] clothingColors = new Color[]{dummyColor, topColor, bottomsColor, shoesColor, socksColor, headbandColor, sleeveColor};
		string[] articles = new string[]{ "dummy", "top", "bottoms", "shoes", "socks", "headband", "sleeve"};
		SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[]{ smr_dummy, smr_top, smr_bottoms, smr_shoes, smr_socks, smr_headband, smr_sleeve};
		*/
		
		int[] meshNumbers = new int[]{ shoesMeshNum, socksMeshNum, topMeshNum, bottomsMeshNum, sleeveMeshNum, headbandMeshNum, dummyMeshNum};
		int[] materialNumbers = new int[]{ shoesMaterialNum, socksMaterialNum, topMaterialNum, bottomsMaterialNum, sleeveMaterialNum, headbandMaterialNum, dummyMaterialNum};
		Color[] clothingColors = new Color[]{shoesColor, socksColor, topColor, bottomsColor, sleeveColor, headbandColor, dummyColor};
		string[] articles = new string[]{ "shoes", "socks", "top", "bottoms", "sleeve", "headband", "dummy"};
		SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[]{smr_shoes, smr_socks, smr_top, smr_bottoms, smr_sleeve, smr_headband, smr_dummy};
		
		SkinnedMeshRenderer renderer;
		for(int j = 0; j < renderers.Length; j++){
			// set meshes, materials and colors
			renderer = renderers[j];
			// -----------------
			mesh = clothingManager.getMesh(articles[j], meshNumbers[j]);
			// -----------------
			material = Instantiate(clothingManager.getMaterial(articles[j], materialNumbers[j]));
			material.color = clothingColors[j];
			// ~~~~
				material.SetFloat("_Mode", 2);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000 - j;
			// ~~~~
			// -----------------
			renderer.sharedMesh = mesh;
			renderer.materials = new Material[]{material};
			renderer.materials[0] = material;
		}
		// -----------------
		
		// update attributes
		
		Color c;

		this.dummyMeshNumber = dummyMeshNum;
		this.dummyMaterialNumber = dummyMaterialNum;
		c = dummyColor;
		this.dummyRGB = new float[]{ c.r, c.g, c.b};
		
		this.topMeshNumber = topMeshNum;
		this.topMaterialNumber = topMaterialNum;
		c = topColor;
		this.topRGB = new float[]{ c.r, c.g, c.b};
		
		this.bottomsMeshNumber = bottomsMeshNum;
		this.bottomsMaterialNumber = bottomsMaterialNum;
		c = bottomsColor;
		this.bottomsRGB = new float[]{ c.r, c.g, c.b};
		
		this.shoesMeshNumber = shoesMeshNum;
		this.shoesMaterialNumber = shoesMaterialNum;
		c = shoesColor;
		this.shoesRGB = new float[]{ c.r, c.g, c.b};
		
		this.socksMeshNumber = socksMeshNum;
		this.socksMaterialNumber = socksMaterialNum;
		c = socksColor;
		this.socksRGB = new float[]{ c.r, c.g, c.b};
		
		this.headbandMeshNumber = headbandMeshNum;
		this.headbandMaterialNumber = headbandMaterialNum;
		c = headbandColor;
		this.headbandRGB = new float[]{ c.r, c.g, c.b};
		
		this.sleeveMeshNumber = sleeveMeshNum;
		this.sleeveMaterialNumber = sleeveMaterialNum;
		c = sleeveColor;
		this.sleeveRGB = new float[]{ c.r, c.g, c.b};
		// -----------------
		
	}
	
	// x: length
	// y: width
	// z: depth
	public void setBodyProportions(int setting){
		
		float headScaleX = 1f;
		float neckScaleX = 1f;
		float torsoScaleX = 1f;
		float armScaleX = 1f;
		float legScaleX = 1f;
		float feetScaleX = 1f;
		float headScaleY = 1f;
		float neckScaleY = 1f;
		float torsoScaleY = 1f;
		float armScaleY = 1f;
		float legScaleY = 1f;
		float feetScaleY = 1f;
		float headScaleZ = 1f;
		float neckScaleZ = 1f;
		float torsoScaleZ = 1f;
		float armScaleZ = 1f;
		float legScaleZ = 1f;
		float feetScaleZ = 1f;
		float h = 70f;
		float w = 155f;
		
		if(setting == ATTRIBUTES_DEFAULT){
			// do nothing
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			headScaleX = headX;
			neckScaleX = neckX;
			torsoScaleX = torsoX;
			armScaleX = armX;
			legScaleX = legX;
			feetScaleX = feetX;
			headScaleY = headY;
			neckScaleY = neckY;
			torsoScaleY =torsoY;
			armScaleY = armY;
			legScaleY = legY;
			feetScaleY = feetY;
			headScaleZ = headZ;
			neckScaleZ = neckZ;
			torsoScaleZ = torsoZ;
			armScaleZ = armZ;
			legScaleZ = legZ;
			feetScaleZ = feetZ;
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			
			/*
			// testing
			torsoScaleX = 1.01f;
			torsoScaleY = torsoScaleX * .92f;
			torsoScaleZ = torsoScaleY * .9f;
			*/
			
			// randomize torso proportions
			torsoScaleX = (Random.Range(.9f, 1.1f)+Random.Range(.9f, 1.1f)+Random.Range(.9f, 1.1f))/3f;
			torsoScaleY = Random.Range(.9f, 1.2f);
			torsoScaleZ = torsoScaleY * Random.Range(.9f, 1.1f);
			
		
			// adjust head, neck, arm, leg proportions for torso
			neckScaleX *= (1f/torsoScaleX);
			neckScaleY *= (1f/torsoScaleY);
			neckScaleZ *= (1f/torsoScaleZ);
			armScaleX *= Mathf.Pow((1f/torsoScaleY), .5f);
			armScaleY *= Mathf.Pow(torsoScaleY, .5f);
			armScaleZ *= Mathf.Pow(torsoScaleY, .5f);
			legScaleX *= torsoScaleX;
			legScaleY *= 1f;
			legScaleZ = legScaleY;
			
			/*
			// testing
			neckScaleX *= 1.3f;
			neckScaleY *= 1;
			neckScaleZ = 1;
			armScaleX *= 1f;
			legScaleX *= 1f;
			/*
			
			// randomize neck, arm and leg proportions
			neckScaleX *= Random.Range(.5f, 1.5f);
			neckScaleY *= Random.Range(.8f, 1.2f);
			neckScaleZ = neckScaleY;
			armScaleX *= Random.Range(1f,1.02f);
			legScaleX *= Random.Range(1f, 1f);
			
	
			/*
			//testing
			headScaleX *= (1f / neckScaleX) * (1f / torsoScaleX);
			headScaleY *= .98f;
			headScaleZ *= .98f;
			*/
			
			// adjust head proportion for neck and torso
			headScaleX *= (1f / neckScaleX) * (1f / torsoScaleX);
			//headScaleY *= 1f / neckScaleY;
			//headScaleZ *= 1f / neckScaleZ;
			
			
			// adjust feet proportion for legs
			feetScaleX *= 2f-thighRight.localScale.x;
			feetScaleZ *= 2f-thighRight.localScale.x;
			// -----------------
			
		}
		else if(setting >= 4){
			headScaleX = float.Parse(TextReader.getAttribute(setting, "headX"));
			headScaleY = float.Parse(TextReader.getAttribute(setting, "headY"));
			headScaleZ = float.Parse(TextReader.getAttribute(setting, "headZ"));
			neckScaleX = float.Parse(TextReader.getAttribute(setting, "neckX"));
			neckScaleY = float.Parse(TextReader.getAttribute(setting, "neckY"));
			neckScaleZ = float.Parse(TextReader.getAttribute(setting, "neckZ"));
			torsoScaleX = float.Parse(TextReader.getAttribute(setting, "torsoX"));
			torsoScaleY = float.Parse(TextReader.getAttribute(setting, "torsoY"));
			torsoScaleZ = float.Parse(TextReader.getAttribute(setting, "torsoZ"));
			armScaleX = float.Parse(TextReader.getAttribute(setting, "armX"));
			armScaleY = float.Parse(TextReader.getAttribute(setting, "armY"));
			armScaleZ = float.Parse(TextReader.getAttribute(setting, "armZ"));
			legScaleX = float.Parse(TextReader.getAttribute(setting, "legX"));
			legScaleY = float.Parse(TextReader.getAttribute(setting, "legY"));
			legScaleZ = float.Parse(TextReader.getAttribute(setting, "legZ"));
			feetScaleX = float.Parse(TextReader.getAttribute(setting, "feetX"));
			feetScaleY = float.Parse(TextReader.getAttribute(setting, "feetY"));
			feetScaleZ = float.Parse(TextReader.getAttribute(setting, "feetZ"));
		}
		setTorsoProportions(torsoScaleX, torsoScaleY, torsoScaleZ);
		setHeadProportions(headScaleX, headScaleY, headScaleZ);
		setNeckProportions(neckScaleX, neckScaleY, neckScaleZ);
		setArmProportions(armScaleX, armScaleY, armScaleZ);
		setLegProportions(legScaleX, legScaleY, legScaleZ);
		setFeetProportions(feetScaleX, feetScaleY, feetScaleZ);
		adjustHeightAndWeight();
		
		updateAttributes();
		//Debug.Log(height);
		// -----------------

		void setTorsoProportions(float scaleX, float scaleY, float scaleZ){
			torso.localScale = new Vector3(scaleX, scaleY, scaleZ);
		}
		
		void setHeadProportions(float scaleX, float scaleY, float scaleZ){
			head.localScale = new Vector3(scaleX, scaleY, scaleZ);
		}
		
		void setNeckProportions(float scaleX, float scaleY, float scaleZ){
			neck.localScale = new Vector3(scaleX, scaleY, scaleZ);
		}
		
		void setArmProportions(float scaleX, float scaleY, float scaleZ){
			Vector3 scaleVec = new Vector3(scaleX, scaleY, scaleZ);
			upperArmRight.localScale = scaleVec;
			upperArmLeft.localScale = scaleVec;
			lowerArmRight.localScale = scaleVec;
			lowerArmLeft.localScale = scaleVec;
		}
		void setLegProportions(float scaleX, float scaleY, float scaleZ){
			Vector3 scaleVec_thigh = new Vector3(scaleX, scaleY, scaleZ);
			Vector3 scaleVec_shin = new Vector3(scaleX, 1f/scaleX, 1f/scaleX);
			thighRight.localScale = scaleVec_thigh;
			thighLeft.localScale = scaleVec_thigh;
			shinRight.localScale = scaleVec_shin;
			shinLeft.localScale = scaleVec_shin;
		}
		
		void setFeetProportions(float scaleX, float scaleY, float scaleZ){
			footRight.localScale = new Vector3(scaleX, scaleY, scaleZ);
			footLeft.localScale = footRight.localScale;
		}
		
		void adjustHeightAndWeight(){
			h *= Mathf.Pow(torsoScaleX, .5f) * Mathf.Pow(legScaleX, .5f) * Mathf.Pow(neckScaleX, .05f);
			w *= Mathf.Pow(h/70f, 2.48f) * Mathf.Pow(torsoScaleY*torsoScaleZ, .8f);
		}
		
		// sets stats based on body proportions
		void updateAttributes(){
			headX = headScaleX;
			headY = headScaleY;
			headZ = headScaleZ;
			neckX = neckScaleX;
			neckY = neckScaleY;
			neckZ = neckScaleZ;
			torsoX = torsoScaleX;
			torsoY = torsoScaleY;
			torsoZ = torsoScaleZ;
			armX = armScaleX;
			armY = armScaleY;
			armZ = armScaleZ;
			legX = legScaleX;
			legY = legScaleY;
			legZ = legScaleZ;
			feetX = feetScaleX;
			feetY = feetScaleY;
			feetZ = feetScaleZ;
			height = h;
			weight = w;
			
			int feet = (int)height / 12;
			int inchesLeft = (int)height % 12;
			
			/*
			Debug.Log("Height: " + feet + "\'" + inchesLeft + "\"");
			Debug.Log("Weight: " + weight);
			Debug.Log("");
			*/
		}
	}
	
	public void setStats(int setting){
		
		float power = 175f;
		float transPivSpeed = 200f;
		float quickness = 1f;
		float knee_dominance = 1f;
		float turnover = 1f;
		float fitness = 1f;
		
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			power = this.POWER;
			transPivSpeed = this.TRANSITION_PIVOT_SPEED;
			quickness = this.QUICKNESS;
			knee_dominance = this.KNEE_DOMINANCE;
			turnover = this.TURNOVER;
			fitness = this.FITNESS;
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			// modify stats from leg length
			quickness = Mathf.Pow((2f - thighRight.localScale.x), .5f);
			turnover = Mathf.Pow((2f - thighRight.localScale.x), .5f);
			knee_dominance = Mathf.Pow((2f - thighRight.localScale.x), 1f);
			fitness = Mathf.Pow((2f - thighRight.localScale.x), .3f);
		}	
		else if(setting >= 4){
			
			//TURNOVER = Mathf.Pow((2f - thighRight.localScale.x), .5f);
			//KNEE_DOMINANCE = Mathf.Pow((2f - thighRight.localScale.x), 1f);
			
			if(setting == ATTRIBUTES_LEGEND_USAINBOLT){
				power = 224f;
				knee_dominance = .88f;
				turnover = .938083152f;
				fitness = .97f;
			}
			else if(setting == ATTRIBUTES_LEGEND_MICHAELJOHNSON){
				power = 163.625f;
				knee_dominance = .91f;
				turnover = 1.2f;
				fitness = 1.5f;
			}
			else if(setting == ATTRIBUTES_LEGEND_YOHANBLAKE){
				power = 180f;
				knee_dominance = .9f;
				turnover = 1f;
				fitness = 1.2f;
				quickness = 1.02f;
			}
			else if(setting == ATTRIBUTES_LEGEND_JESSEOWENS){
				power = 130f;
				knee_dominance = .7f;
				turnover = 1.25f;
				fitness = 1.2f;
				quickness = .9f;
			}
			else if(setting == ATTRIBUTES_LEGEND_WAYDEVANNIEKERK){
				power = 280f;
				knee_dominance = .7f;
				turnover = .8f;
				fitness = 1.4f;
				quickness = .975f;
			}
	
		}
		
		POWER = power;
		TRANSITION_PIVOT_SPEED = transPivSpeed;
		QUICKNESS = quickness;
		KNEE_DOMINANCE = knee_dominance;
		TURNOVER = turnover;
		FITNESS = fitness;
	}
	
	public void setAnimatorController(int setting){
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			animator.runtimeAnimatorController = animatorControllers[animatorNum];
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			int random = Random.Range(0, animatorControllers.Length);
			animator.runtimeAnimatorController = animatorControllers[random];
			this.animatorNum = random;
			this.armSpeedFlex = Random.Range(.9f,1.1f);
			this.armSpeedExtend = (2f-armSpeedFlex);
		}	
	}
	
	public void copyAttributesFromOther(GameObject other, string whichAttributes){
		PlayerAttributes otherAttributes = other.GetComponent<PlayerAttributes>();
		// -----------------
		if(whichAttributes == "all"){
			id = otherAttributes.id;
			racerName = otherAttributes.racerName;
			personalBests = otherAttributes.personalBests;
			resultString = otherAttributes.resultString;
			// -----------------
		}
		if(whichAttributes == "all" || whichAttributes == "stats"){
			POWER = otherAttributes.POWER;
			TRANSITION_PIVOT_SPEED = otherAttributes.TRANSITION_PIVOT_SPEED;
			KNEE_DOMINANCE = otherAttributes.KNEE_DOMINANCE;
			TURNOVER = otherAttributes.TURNOVER;
			FITNESS = otherAttributes.FITNESS;
			// -----------------
		}
		if(whichAttributes == "all" || whichAttributes == "ghost data"){
			pathLength = otherAttributes.pathLength;
			leanLockTick = otherAttributes.leanLockTick;
			velMagPath = otherAttributes.velMagPath;
			velPathX = otherAttributes.velPathX;
			velPathY = otherAttributes.velPathY;
			velPathZ = otherAttributes.velPathZ;
			posPathX = otherAttributes.posPathX;
			posPathZ = otherAttributes.posPathZ;
			posPathY = otherAttributes.posPathY;
			rightInputPath = otherAttributes.rightInputPath;
			leftInputPath = otherAttributes.leftInputPath;
			sphere1Prog = otherAttributes.sphere1Prog;
			sphere2Prog = otherAttributes.sphere2Prog;
			// -----------------
		}
		if(whichAttributes == "all" || whichAttributes == "clothing"){
			topMeshNumber = otherAttributes.topMeshNumber;
			bottomsMeshNumber = otherAttributes.bottomsMeshNumber;
			shoesMeshNumber = otherAttributes.shoesMeshNumber;
			socksMeshNumber = otherAttributes.socksMeshNumber;
			headbandMeshNumber = otherAttributes.headbandMeshNumber;
			sleeveMeshNumber = otherAttributes.sleeveMeshNumber;
			topMaterialNumber = otherAttributes.topMaterialNumber;
			bottomsMaterialNumber = otherAttributes.bottomsMaterialNumber;
			shoesMaterialNumber = otherAttributes.shoesMaterialNumber;
			socksMaterialNumber = otherAttributes.socksMaterialNumber;
			headbandMaterialNumber = otherAttributes.headbandMaterialNumber;
			sleeveMaterialNumber = otherAttributes.sleeveMaterialNumber;
		
			float[] RGB;
		
			RGB = otherAttributes.dummyRGB;
			dummyRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			
			RGB = otherAttributes.topRGB;
			topRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		
			RGB = otherAttributes.bottomsRGB;
			bottomsRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		
			RGB = otherAttributes.shoesRGB;
			shoesRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		
			RGB = otherAttributes.socksRGB;
			socksRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		
			RGB = otherAttributes.headbandRGB;
			headbandRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		
			RGB = otherAttributes.sleeveRGB;
			sleeveRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		}
		if(whichAttributes == "all" || whichAttributes == "body proportions"){
			headX = otherAttributes.headX;
			headY = otherAttributes.headY;
			headZ = otherAttributes.headZ;
			neckX = otherAttributes.neckX;
			neckY = otherAttributes.neckY;
			neckZ = otherAttributes.neckZ;
			torsoX = otherAttributes.torsoX;
			torsoY = otherAttributes.torsoY;
			torsoZ = otherAttributes.torsoZ;
			armX = otherAttributes.armX;
			armY = otherAttributes.armY;
			armZ = otherAttributes.armZ;
			legX = otherAttributes.legX;
			legY = otherAttributes.legY;
			legZ = otherAttributes.legZ;
			feetX = otherAttributes.feetX;
			feetY = otherAttributes.feetY;
			feetZ = otherAttributes.feetZ;
			height = otherAttributes.height;
			weight = otherAttributes.weight;
			// -----------------
			animatorNum = otherAttributes.animatorNum;
			armSpeedFlex = otherAttributes.armSpeedFlex;
			armSpeedExtend = otherAttributes.armSpeedExtend;
		}
	}
	
	public void renderInForeground(){
		Material m;
		Color c;
		Shader s = clothingManager.shader_renderOnTop;
		// -----------------
		m = smr_dummy.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_dummy.materials[0] = m;
		
		m = smr_top.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_top.materials[0] = m;
		
		m = smr_bottoms.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_bottoms.materials[0] = m;
		
		m = smr_shoes.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_shoes.materials[0] = m;
		
		m = smr_socks.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_socks.materials[0] = m;
		
		m = smr_headband.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_headband.materials[0] = m;
		
		m = smr_sleeve.materials[0];
		c = m.color;
		m.shader = s;
		m.color = c;
		smr_sleeve.materials[0] = m;
	}
	


	public void setPaths(int length){
		velMagPath = new float[length];
		velPathX = new float[length];
		velPathY = new float[length];
		velPathZ = new float[length];
		posPathX = new float[length];
		posPathZ = new float[length];
		posPathY = new float[length];
		rightInputPath = new int[length];
		leftInputPath = new int[length];
		sphere1Prog = new float[length];
		sphere2Prog = new float[length];
	}
	
	
	public static string generateID(string racerName){
		string id = racerName + "_";
		
		int digit;
		for(int i = 0; i < 9; i++){
			digit = Random.Range(0,10);
			id += digit.ToString();
		}
		return id;
		
	}
	
	
	
}
