using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
	
	public ClothingManager clothingManager;
	// -----------------
	public static int DEFAULT_PATH_LENGTH = 9000;
	// -----------------

	public static int RANDOM = 1;
	public static int FROM_THIS = 2;
	public static int DEFAULT = 3;
	// -----------------
	
	// id info
	public string id;
	public string racerName;
	public float[] personalBests;
	public int totalScore;
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
	public float LAUNCH_POWER;						// base 1
	public float CURVE_POWER;						// base 1
	public float CRUISE;							// base 1
	
	// -----------------
	
	// animation info
	public Animator animator;
	public RuntimeAnimatorController[] animatorControllers;
	public int animatorNum;
	public int leadLeg;
	public float armSpeedFlexL;
	public float armSpeedExtendL;
	public float armSpeedFlexR;
	public float armSpeedExtendR;
	
	
	
	
	// Start is called before the first frame update
    void Start()
    {
		
		//Debug.Log("------------START-----------------");
		// -----------------
		//racerName = "DefaultName";
		resultTag = "";
		if(tag.StartsWith("Player")){
			resultColor = "blue";
		}
		else if(tag.StartsWith("Ghost")){
			resultColor = "#5962ff";
		}
		else if(tag.StartsWith("Bot")){
			resultColor = "#666666";
		}
		// -----------------

    }
	
	public void setInfo(int setting){
		if(setting == PlayerAttributes.FROM_THIS){
			// do nothing
		}
		else if(setting == PlayerAttributes.RANDOM){
			racerName = TextReader.getRandomName();
		}	
		id = PlayerAttributes.generateID(racerName, PlayFabManager.thisUserDisplayName);
	}
	
	public void setClothing(int setting){
		
		Mesh mesh;
		Material material;
		// -----------------
		if(setting == DEFAULT){
			dummyMeshNumber = 0;
			topMeshNumber = 0;
			bottomsMeshNumber = 0;
			shoesMeshNumber = 0;
			socksMeshNumber = 0;
			headbandMeshNumber = 0;
			sleeveMeshNumber = 2;
			dummyMaterialNumber = 0;
			topMaterialNumber = 0;
			bottomsMaterialNumber = 0;
			shoesMaterialNumber = 0;
			socksMaterialNumber = 0;
			headbandMaterialNumber = 0;
			sleeveMaterialNumber = 0;
			dummyRGB = new float[]{.62f,.5f,.25f};
			topRGB = new float[]{1f,1f,1f};
			bottomsRGB = new float[]{1f,1f,1f};
			shoesRGB = new float[]{1f,1f,1f};
			socksRGB = new float[]{1f,1f,1f};
			headbandRGB = new float[]{1f,1f,1f};
			sleeveRGB = new float[]{1f,1f,1f};
		}	
		else if(setting == FROM_THIS){
			// do nothing
		}
		else if(setting == RANDOM){
			
			// get random values for meshes, materials, and colors
			int i;
			int[] randomMeshes = clothingManager.getRandomMeshNumbers();
			i = 0;
			dummyMeshNumber = randomMeshes[i]; i++;
			topMeshNumber = randomMeshes[i]; i++;
			bottomsMeshNumber = randomMeshes[i]; i++;
			shoesMeshNumber = randomMeshes[i]; i++;
			socksMeshNumber = randomMeshes[i]; i++;
			headbandMeshNumber = randomMeshes[i]; i++;
			sleeveMeshNumber = randomMeshes[i]; i++;	
			int[] randomMaterials = clothingManager.getRandomMaterialNumbers();
			i = 0;
			dummyMaterialNumber = randomMaterials[i]; i++;
			topMaterialNumber = randomMaterials[i]; i++;
			bottomsMaterialNumber = randomMaterials[i]; i++;
			shoesMaterialNumber = randomMaterials[i]; i++;
			socksMaterialNumber = randomMaterials[i]; i++;
			headbandMaterialNumber = randomMaterials[i]; i++;
			sleeveMaterialNumber = randomMaterials[i]; i++;
			Color[] randomColors = clothingManager.getRandomColors();
			i = 0;
			dummyRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			topRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			bottomsRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			shoesRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			socksRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			headbandRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
			sleeveRGB = new float[]{randomColors[i].r,randomColors[i].g,randomColors[i].b}; i++;
		}

		// apply attributes to model
		int[] meshNumbers = new int[]{ shoesMeshNumber, socksMeshNumber, topMeshNumber, bottomsMeshNumber, sleeveMeshNumber, headbandMeshNumber, dummyMeshNumber};
		int[] materialNumbers = new int[]{ shoesMaterialNumber, socksMaterialNumber, topMaterialNumber, bottomsMaterialNumber, sleeveMaterialNumber, headbandMaterialNumber, dummyMaterialNumber};
		float[][] clothingRGBs = new float[][]{shoesRGB, socksRGB, topRGB, bottomsRGB, sleeveRGB, headbandRGB, dummyRGB};
		string[] articles = new string[]{ "shoes", "socks", "top", "bottoms", "sleeve", "headband", "dummy"};
		SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[]{smr_shoes, smr_socks, smr_top, smr_bottoms, smr_sleeve, smr_headband, smr_dummy};
		
		SkinnedMeshRenderer renderer;
		for(int j = 0; j < renderers.Length; j++){
			// set meshes, materials and colors
			renderer = renderers[j];
			// -----------------
			mesh = clothingManager.getMesh(articles[j], meshNumbers[j]);
			// -----------------
			//Material oldMat = renderer.sharedMaterial;
			material = Instantiate(clothingManager.getMaterial(articles[j], materialNumbers[j]));
			material.color = new Color(clothingRGBs[j][0],clothingRGBs[j][1],clothingRGBs[j][2]);
			//Destroy(oldMat);
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
			
			renderer.sharedMaterial = material;
			//renderer.materials = new Material[]{material};
			//renderer.materials[0] = material;
		}
	}
	

	public void setBodyProportions(int setting){
		if(setting == DEFAULT){
			headX = 1f;neckX = 1f;torsoX = 1f;armX = 1f;legX = 1f;feetX = 1f;
			headY = 1f;neckY = 1f;torsoY = 1f;armY = 1f;legY = 1f;feetY = 1f;
			headZ = 1f;neckZ = 1f;torsoZ = 1f;armZ = 1f;legZ = 1f;feetZ = 1f;
		}
		else if(setting == PlayerAttributes.FROM_THIS){
			// do nothing
		}
		else if(setting == PlayerAttributes.RANDOM){
			
			
			headX = 1f;neckX = 1f;torsoX = 1f;armX = 1f;legX = 1f;feetX = 1f;
			headY = 1f;neckY = 1f;torsoY = 1f;armY = 1f;legY = 1f;feetY = 1f;
			headZ = 1f;neckZ = 1f;torsoZ = 1f;armZ = 1f;legZ = 1f;feetZ = 1f;
			
			/*
			// testing
			int r = Random.Range(1, 3);
			if(r == 1){
				torsoX = 1.05f;
			} else { torsoX = .95f; }
			torsoY = torsoX;
			torsoZ = torsoY;
			*/
			
			
			// randomize torso proportions
			torsoX = (Random.Range(.95f, 1.05f)+Random.Range(.95f, 1.05f))/2f;
			torsoY = Random.Range(.9f, 1.1f);
			torsoZ = torsoY * Random.Range(.8f, 1.2f);
			
			
		
			// adjust head, neck, arm, leg proportions for torso
			neckX *= (1f/torsoX);
			neckY *= (1f/torsoY);
			neckZ *= (1f/torsoZ);
			legX *= torsoX;
			legY *= 1f;
			legZ = 1f;
			armX *= Mathf.Pow((1f/torsoY), .75f);
			
			/*
			// testing
			neckX *= 1.3f;
			neckY *= 1;
			neckZ = 1;
			armX *= 1f;
			legX *= 1f;
			*/
			
			
			// randomize neck, arm and leg proportions
			neckX *= Random.Range(.5f, 1.5f);
			float lankiness = Random.Range(.95f,1.05f);
			armX *= lankiness;
			legX *= lankiness;
			
	
			/*
			//testing
			headX *= (1f / neckX) * (1f / torsoX);
			headY *= .98f;
			headZ *= .98f;
			*/
			
			// adjust head proportion for neck and torso
			headX *= (1f / neckX) * (1f / torsoX);
			//headY *= 1f / neckY;
			//headZ *= 1f / neckZ;
			
			
			// adjust feet proportion for legs
			feetX *= 2f-thighRight.localScale.x;
			feetZ *= 2f-thighRight.localScale.x;
			// -----------------
		}
	
		setTorsoProportions(torsoX, torsoY, torsoZ);
		setHeadProportions(headX, headY, headZ);
		setNeckProportions(neckX, neckY, neckZ);
		setArmProportions(armX, armY, armZ);
		setLegProportions(legX, legY, legZ);
		setFeetProportions(feetX, feetY, feetZ);
		adjustHeightAndWeight();
		
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
			height *= Mathf.Pow(torsoX, .5f) * Mathf.Pow(legX, .5f) * Mathf.Pow(neckX, .05f);
			weight *= Mathf.Pow(height/70f, 2.48f) * Mathf.Pow(torsoY*torsoZ, .8f);
		}
	}
	
	public void setStats(int setting){
		
		float power = 2.8875f;
		float transPivSpeed = 200f;
		float quickness = 1f;
		float knee_dominance = 1f;
		float turnover = 1f;
		float fitness = 1f;
		float launch_power = 1f;
		float curve_power = 1f;
		float cruise = 1f;
		
		if(setting == PlayerAttributes.DEFAULT){
			POWER = power;
			TRANSITION_PIVOT_SPEED = transPivSpeed;
			QUICKNESS = quickness;
			KNEE_DOMINANCE = knee_dominance;
			TURNOVER = turnover;
			FITNESS = fitness;
			LAUNCH_POWER = launch_power;
			CURVE_POWER = curve_power;
			CRUISE = cruise;
		}
		else if(setting == PlayerAttributes.FROM_THIS){
			power = this.POWER;
			transPivSpeed = this.TRANSITION_PIVOT_SPEED;
			quickness = this.QUICKNESS;
			knee_dominance = this.KNEE_DOMINANCE;
			turnover = this.TURNOVER;
			fitness = this.FITNESS;
			launch_power = this.LAUNCH_POWER;
			curve_power = this.CURVE_POWER;
			cruise = this.CRUISE;
			
			POWER = power;
			TRANSITION_PIVOT_SPEED = transPivSpeed;
			QUICKNESS = quickness;
			KNEE_DOMINANCE = knee_dominance;
			TURNOVER = turnover;
			FITNESS = fitness;
			LAUNCH_POWER = launch_power;
			CURVE_POWER = curve_power;
			CRUISE = cruise;
		}
		else if(setting == PlayerAttributes.RANDOM){
			// modify stats from leg length
			quickness = .95f * Mathf.Pow((2f - legX), .1f);
			turnover = Mathf.Pow((2f - legX), .5f);
			cruise = (Random.Range(.5f, 1f));
			launch_power = Mathf.Pow(knee_dominance, 1f) * Mathf.Pow((2f-(cruise+.25f)), .5f);
			knee_dominance = Mathf.Pow((2f - legX), 1f);
			power *= Mathf.Pow(legX, .5f) * Mathf.Pow(2f-(cruise+.5f), .05f);
			fitness *= Mathf.Pow(cruise, .35f);
			
			POWER = power;
			TRANSITION_PIVOT_SPEED = transPivSpeed;
			QUICKNESS = quickness;
			KNEE_DOMINANCE = knee_dominance;
			TURNOVER = turnover;
			FITNESS = fitness;
			LAUNCH_POWER = launch_power;
			CURVE_POWER = curve_power;
			CRUISE = cruise;
		}	
	}
	
	public void setAnimations(int setting){
		if(setting == PlayerAttributes.FROM_THIS){
			// do nothing
		}
		else if(setting == PlayerAttributes.RANDOM){
			int random = Random.Range(0, animatorControllers.Length);
			animator.runtimeAnimatorController = animatorControllers[random];
			animatorNum = random;
			leadLeg = Random.Range(0,2);
			armSpeedFlexL = Random.Range(.9f, 1.1f);
			armSpeedExtendL = (2f-armSpeedFlexL);
			
			if(tag.StartsWith("Bot")){
				armSpeedFlexR = armSpeedFlexL;
				armSpeedExtendR = armSpeedExtendL;
			}
			else{
				armSpeedFlexR = Random.Range(.9f, 1.1f);
				armSpeedExtendR = (2f-armSpeedFlexR);
			}
		}
		
		animator.runtimeAnimatorController = animatorControllers[animatorNum];
	}
	
	public void setPaths(int setting){
		int length = setting;
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
		
	
	public void copyAttributesFromOther(GameObject other, string whichAttributes){
		PlayerAttributes otherAttributes = other.GetComponent<PlayerAttributes>();
		// -----------------
		if(whichAttributes == "all"){
			id = otherAttributes.id;
			racerName = otherAttributes.racerName;
			personalBests = otherAttributes.personalBests;
			totalScore = otherAttributes.totalScore;
			resultString = otherAttributes.resultString;
			// -----------------
			copyAttributesFromOther(other, "stats");
			copyAttributesFromOther(other, "ghost data");
			copyAttributesFromOther(other, "clothing");
			copyAttributesFromOther(other, "body proportions");
			copyAttributesFromOther(other, "animation properties");
		}
		else if(whichAttributes == "info"){
			racerName = otherAttributes.racerName;
			personalBests = otherAttributes.personalBests;
			totalScore = otherAttributes.totalScore;
		}
		else if(whichAttributes == "stats"){
			POWER = otherAttributes.POWER;
			QUICKNESS = otherAttributes.QUICKNESS;
			TRANSITION_PIVOT_SPEED = otherAttributes.TRANSITION_PIVOT_SPEED;
			KNEE_DOMINANCE = otherAttributes.KNEE_DOMINANCE;
			TURNOVER = otherAttributes.TURNOVER;
			FITNESS = otherAttributes.FITNESS;
			LAUNCH_POWER = otherAttributes.LAUNCH_POWER;
			CURVE_POWER = otherAttributes.CURVE_POWER;
			CRUISE = otherAttributes.CRUISE;
			// -----------------
		}
		else if(whichAttributes == "ghost data"){
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
		else if(whichAttributes == "clothing"){
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
			RGB = otherAttributes.dummyRGB; dummyRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.topRGB; topRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.bottomsRGB; bottomsRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.shoesRGB; shoesRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.socksRGB; socksRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.headbandRGB; headbandRGB = new float[]{RGB[0], RGB[1], RGB[2]};
			RGB = otherAttributes.sleeveRGB; sleeveRGB = new float[]{RGB[0], RGB[1], RGB[2]};
		}
		else if(whichAttributes == "body proportions"){
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
		}
		else if(whichAttributes == "animation properties"){
			animatorNum = otherAttributes.animatorNum;
			leadLeg = otherAttributes.leadLeg;
			armSpeedFlexL = otherAttributes.armSpeedFlexL;
			armSpeedExtendL = otherAttributes.armSpeedExtendL;
			armSpeedFlexR = otherAttributes.armSpeedFlexR;
			armSpeedExtendR = otherAttributes.armSpeedExtendR;
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
	
	
	public static string generateID(string racerName, string displayName){
		string id = racerName + "_" + displayName + "_";
		
		int digit;
		for(int i = 0; i < 9; i++){
			digit = Random.Range(0,10);
			id += digit.ToString();
		}
		return id;
		
	}

}
