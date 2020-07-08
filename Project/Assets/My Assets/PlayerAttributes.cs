using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
	
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
	public SkinnedMeshRenderer smr_top;
	public SkinnedMeshRenderer smr_bottoms;
	public SkinnedMeshRenderer smr_shoes;
	public SkinnedMeshRenderer smr_dummy;
	public Material[] topMaterials;
	public Material[] bottomsMaterials;
	public Material[] shoesMaterials;
	public Material[] dummyMaterials;
	public int topNumber;
	public int bottomsNumber;
	public int shoesNumber;
	public float dummyV;
	public float topR;
	public float topG;
	public float topB;
	public float bottomsR;
	public float bottomsG;
	public float bottomsB;
	public float shoesR;
	public float shoesG;
	public float shoesB;
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
	
	
	
	public void setMaterials(int setting){
		float h,s,v;
		Color.RGBToHSV(dummyMaterials[0].color, out h, out s, out v);
		// --
		Material material;
		int top = -1;
		int bottoms = -1;
		int shoes = -1;
		Color topColor = Color.white;
		Color bottomsColor = Color.white;
		Color shoesColor = Color.white;
		// -----------------
		if(setting == ATTRIBUTES_FROM_THIS){
			// get values for dummy and clothing materials/colors from self
			v = this.dummyV;
			top = this.topNumber;
			bottoms = this.bottomsNumber;
			shoes = this.shoesNumber;
			topColor = new Color(this.topR, this.topG, this.topB);
			bottomsColor = new Color(this.bottomsR, this.bottomsG, this.bottomsB);
			shoesColor = new Color(this.shoesR, this.shoesG, this.shoesB);
		}
		else if(setting == ATTRIBUTES_RANDOM){
			
			// random skin color for dummy
			v = Random.Range(0f, 1.2f);
			if(v > .3f){
				if(v > .9f){
					v = Random.Range(.8f, 1f);
				}
			}
			else{
				v = Random.Range(.2f, .4f);
			}
			
			// random materials for top, bottoms, shoes
			top = Random.Range(0, topMaterials.Length);
			bottoms = Random.Range(0, bottomsMaterials.Length);
			shoes = Random.Range(0, shoesMaterials.Length);
			
			// random colors for top, bottoms, shoes
			topColor = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
			int random;
			random = Random.Range(0,3);
			switch (random){
				case 0 :
					bottomsColor = topColor;
					break;
				case 1 :
					bottomsColor = Color.black;
					break;
				case 2 :
					bottomsColor = Color.white;
					break;
			}
			random = Random.Range(0,3);
			switch (random){
				case 0 :
					shoesColor = topColor;
					break;
				case 1 :
					shoesColor = bottomsColor;
					break;
				case 2 :
					shoesColor = Color.white;
					break;
			}
		}
		
		// set skin material
		material = dummyMaterials[0];
		material.color = Color.HSVToRGB(h, s, v);
		dummyMaterials[0] = Instantiate(material);
		smr_dummy.materials = dummyMaterials;
		
		// set clothing materials and colors
		material = topMaterials[top];
		material.color = topColor;
		topMaterials[0] = Instantiate(material);
		smr_top.materials = topMaterials;
		// --
		material = bottomsMaterials[bottoms];
		material.color = bottomsColor;
		bottomsMaterials[0] = Instantiate(material);
		smr_bottoms.materials = bottomsMaterials;
		// --
		material = shoesMaterials[shoes];
		material.color = shoesColor;
		shoesMaterials[0] = Instantiate(material);
		smr_shoes.materials = shoesMaterials;
		
		// update attributes
		this.dummyV = v;
		this.topNumber = top;
		this.bottomsNumber = bottoms;
		this.shoesNumber = shoes;
		this.topR = smr_top.material.color.r;
		this.topG = smr_top.material.color.g;
		this.topB = smr_top.material.color.b;
		this.bottomsR = smr_bottoms.material.color.r;
		this.bottomsG = smr_bottoms.material.color.g;
		this.bottomsB = smr_bottoms.material.color.b;
		this.shoesR = smr_shoes.material.color.r;
		this.shoesG = smr_shoes.material.color.g;
		this.shoesB = smr_shoes.material.color.b;
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
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			float headScaleX = 1f;
			float neckScaleX = 1f;
			float torsoScaleX = 1f;
			float armScaleX = 1f;
			float legScaleX = 1f;
			float headScaleY = 1f;
			float neckScaleY = 1f;
			float torsoScaleY = 1f;
			float armScaleY = 1f;
			float legScaleY = 1f;
			float headScaleZ = 1f;
			float neckScaleZ = 1f;
			float torsoScaleZ = 1f;
			float armScaleZ = 1f;
			float legScaleZ = 1f;
		
			// randomize torso proportions
			float torsoLength = Random.Range(.95f, 1.05f);
			float torsoWidth = Random.Range(.8f, 1.2f) * torsoLength;
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
			setArmProportions(Random.Range(.985f, 1.015f) * torsoLength, 1f, 1f);
			setLegProportions(Random.Range(1f, 1.02f) * torsoLength, 1f, 1f);
			
			// adjust head proportion for neck
			headScaleX = 1f / neck.localScale.x;
			headScaleY = 1f / neck.localScale.y;
			headScaleZ = 1f / neck.localScale.z;
			setHeadProportions(headScaleX, headScaleY, headScaleZ);
			
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
		}
	}
	
	public void setStats(int setting){
		if(setting == PlayerAttributes.ATTRIBUTES_FROM_THIS){
			// TODO
		}
		else if(setting == PlayerAttributes.ATTRIBUTES_RANDOM){
			
			
			// modify stats from leg length
			TURNOVER += (1f - thighRight.localScale.x) * .1f;
			KNEE_DOMINANCE *= (2f - thighRight.localScale.x);
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
		topNumber = otherAttributes.topNumber;
		bottomsNumber = otherAttributes.bottomsNumber;
		shoesNumber = otherAttributes.shoesNumber;
		dummyV = otherAttributes.dummyV;
		topR = otherAttributes.topR;
		topG = otherAttributes.topG;
		topB = otherAttributes.topB;
		bottomsR = otherAttributes.bottomsR;
		bottomsG = otherAttributes.bottomsG;
		bottomsB = otherAttributes.bottomsB;
		shoesR = otherAttributes.shoesR;
		shoesG = otherAttributes.shoesG;
		shoesB = otherAttributes.shoesB;
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
