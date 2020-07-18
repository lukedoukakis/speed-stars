using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
	
	public ClothingManager clothingManager;
	
	public static int DEFAULT_PATH_LENGTH = 10000;
	// -----------------
	public static int ATTRIBUTES_RANDOM = 1;
	public static int ATTRIBUTES_FROM_THIS = 2;
	// -----------------
	
	// id info
	public string id;
	public string racerName;
	public float personalBest;
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
	public float[] velPathY;
	public float[] velPathZ;
	public float[] posPathY;
	public float[] posPathZ;
	public int[] rightInputPath;
	public int[] leftInputPath;
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
	
	// -----------------
	
	// stats info
	public float POWER;											
	public float TRANSITION_PIVOT_SPEED;			
	public float QUICKNESS;							// base 1
	public float KNEE_DOMINANCE;					// base 1
	public float TURNOVER;							// base 1
	
	// -----------------
	
	// animation info
	public Animator animator;
	public RuntimeAnimatorController[] animatorControllers;
	public int animatorNum;
	
	
	
	
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
		int dummyMeshNum = -1;
		int topMeshNum = -1;
		int bottomsMeshNum = -1;
		int shoesMeshNum = -1;
		int socksMeshNum = -1;
		int headbandMeshNum = -1;
		int sleeveMeshNum = -1;
		int dummyMaterialNum = -1;
		int topMaterialNum = -1;
		int bottomsMaterialNum = -1;
		int shoesMaterialNum = -1;
		int socksMaterialNum = -1;
		int headbandMaterialNum = -1;
		int sleeveMaterialNum = -1;
		Color dummyColor = Color.white;
		Color topColor = Color.white;
		Color bottomsColor = Color.white;
		Color shoesColor = Color.white;
		Color socksColor = Color.white;
		Color headbandColor = Color.white;
		Color sleeveColor = Color.white;
		// -----------------
		if(setting == ATTRIBUTES_FROM_THIS){
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
		
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			setTorsoProportions(torsoX, torsoY, torsoZ);
			setHeadProportions(headX, headY, headZ);
			setNeckProportions(neckX, neckY, neckZ);
			setArmProportions(armX, armY, armZ);
			setLegProportions(legX, legY, legZ);
			setFeetProportions(feetX, feetY, feetZ);
			
			
			
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
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
		
			// randomize torso proportions
			float torsoLength = Random.Range(.9f, 1.1f);
			float torsoWidth = Random.Range(.8f, 1.2f) * Mathf.Pow(torsoLength, .5f);
			float torsoDepth = Random.Range(.95f, 1.05f) * torsoWidth;
			setTorsoProportions(torsoLength, torsoWidth, torsoDepth);
		
			// adjust neck, head, arm proportions for torso
			armScaleY = (1f/torsoLength) + (1f-(1f/torsoLength))*.5f;
			neckScaleX = (1f/torsoLength);
			headScaleX = (1f/torsoLength);
			// --
			armScaleX = (1f/torsoWidth) + (1f-(1f/torsoWidth))*.5f;
			neckScaleY = (1f/torsoWidth);
			headScaleY = (1f/torsoWidth);
			// --
			armScaleZ = (1f/torsoDepth) + (1f-(1f/torsoDepth))*.5f;
			neckScaleZ = (1f/torsoDepth);
			headScaleZ = (1f/torsoDepth);
			// --
			if(torsoLength < 1f){
				headScaleX *= torsoLength;
				headScaleY *= torsoLength;
				headScaleZ *= torsoLength;
			}
			setHeadProportions(headScaleX, headScaleY, headScaleZ);
			setNeckProportions(neckScaleX, neckScaleY, neckScaleZ);
			setArmProportions(armScaleX, armScaleY, armScaleZ);
		
			// randomize neck, arm and leg proportions
			setNeckProportions(Random.Range(.5f, 1.5f), Random.Range(.7f, 1.8f)*torsoWidth, torsoDepth);
			setArmProportions(Mathf.Pow(torsoLength, .28f)*Random.Range(1f,1.02f), 1f, 1f);
			setLegProportions(Random.Range(1f, 1.02f) * torsoLength, 1f, 1f);
			
			// adjust head proportion for neck
			headScaleX = 1f / neck.localScale.x;
			headScaleY = 1f / neck.localScale.y;
			headScaleZ = 1f / neck.localScale.z;
			setHeadProportions(headScaleX, headScaleY, headScaleZ);
			
			// adjust feet proportion for legs
			setFeetProportions(2f-thighRight.localScale.x, 1f, 2f-thighRight.localScale.x);
			
			// -----------------
			
			updateAttributes();
		}

		// -----------------

		void setTorsoProportions(float scaleX, float scaleY, float scaleZ){
			torso.localScale = Vector3.Scale(torso.localScale, new Vector3(scaleX, scaleY, scaleZ));
		}
		
		void setHeadProportions(float scaleX, float scaleY, float scaleZ){
			head.localScale = Vector3.Scale(head.localScale, new Vector3(scaleX, scaleY, scaleZ));
		}
		
		void setNeckProportions(float scaleX, float scaleY, float scaleZ){
			neck.localScale = Vector3.Scale(neck.localScale, new Vector3(scaleX, scaleY, scaleZ));
		}
		
		void setArmProportions(float scaleX, float scaleY, float scaleZ){
			Vector3 scaleVec = new Vector3(scaleX, scaleY, scaleZ);
			upperArmRight.localScale = Vector3.Scale(upperArmRight.localScale, scaleVec);
			upperArmLeft.localScale = Vector3.Scale(upperArmLeft.localScale, scaleVec);
			lowerArmRight.localScale = Vector3.Scale(lowerArmRight.localScale, scaleVec);
			lowerArmLeft.localScale = Vector3.Scale(lowerArmLeft.localScale, scaleVec);
		}
		void setLegProportions(float scaleX, float scaleY, float scaleZ){
			Vector3 scaleVec_thigh = new Vector3(scaleX, scaleY, scaleZ);
			Vector3 scaleVec_shin = new Vector3(scaleX, 1f, 1f);
			thighRight.localScale = Vector3.Scale(thighRight.localScale, scaleVec_thigh);
			thighLeft.localScale = Vector3.Scale(thighLeft.localScale, scaleVec_thigh);
			shinRight.localScale = Vector3.Scale(shinRight.localScale, scaleVec_shin);
			shinLeft.localScale = Vector3.Scale(shinLeft.localScale, scaleVec_shin);
		}
		
		void setFeetProportions(float scaleX, float scaleY, float scaleZ){
			footRight.localScale = Vector3.Scale(footRight.localScale, new Vector3(scaleX, scaleY, scaleZ));
			footLeft.localScale = footRight.localScale;
		}
		
		// sets stats based on body proportions
		void updateAttributes(){
			Vector3 scale;
			// --
			scale = head.localScale;
			headX = scale.x;
			headY = scale.y;
			headZ = scale.z;
			scale = neck.localScale;
			neckX = scale.x;
			neckY = scale.y;
			neckZ = scale.z;
			scale = torso.localScale;
			torsoX = scale.x;
			torsoY = scale.y;
			torsoZ = scale.z;
			scale = upperArmRight.localScale;
			armX = scale.x;
			armY = scale.y;
			armZ = scale.z;
			scale = thighRight.localScale;
			legX = scale.x;
			legY = scale.y;
			legZ = scale.z;
			scale = footRight.localScale;
			feetX = scale.x;
			feetY = scale.y;
			feetZ = scale.z;
		}
	}
	
	public void setStats(int setting){
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			// TODO
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			
			
			// modify stats from leg length
			TURNOVER = 1f + ((1f - thighRight.localScale.x) * .2f);
			KNEE_DOMINANCE = 1f * (2f - thighRight.localScale.x);
		}	
		
		
	}
	
	public void setAnimatorController(int setting){
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			animator.runtimeAnimatorController = animatorControllers[animatorNum];
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			int random = Random.Range(0, animatorControllers.Length);
			animator.runtimeAnimatorController = animatorControllers[random];
			this.animatorNum = random;
		}	
	}
	
	public void copyAttributesFromOther(GameObject other){
		PlayerAttributes otherAttributes = other.GetComponent<PlayerAttributes>();
		// -----------------
		id = otherAttributes.id;
		racerName = otherAttributes.racerName;
		personalBest = otherAttributes.personalBest;
		resultString = otherAttributes.resultString;
		// -----------------
		POWER = otherAttributes.POWER;
		TRANSITION_PIVOT_SPEED = otherAttributes.TRANSITION_PIVOT_SPEED;
		KNEE_DOMINANCE = otherAttributes.KNEE_DOMINANCE;
		TURNOVER = otherAttributes.TURNOVER;
		// -----------------
		pathLength = otherAttributes.pathLength;
		velPathY = otherAttributes.velPathY;
		velPathZ = otherAttributes.velPathZ;
		posPathZ = otherAttributes.posPathZ;
		posPathY = otherAttributes.posPathY;
		rightInputPath = otherAttributes.rightInputPath;
		leftInputPath = otherAttributes.leftInputPath;
		// -----------------
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
		// -----------------
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
		// -----------------
		animatorNum = otherAttributes.animatorNum;
	}
	


	public void setPaths(int length){
		velPathY = new float[length];
		velPathZ = new float[length];
		posPathZ = new float[length];
		posPathY = new float[length];
		rightInputPath = new int[length];
		leftInputPath = new int[length];
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
