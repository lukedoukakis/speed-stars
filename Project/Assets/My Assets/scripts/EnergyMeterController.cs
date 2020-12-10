using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMeterController : MonoBehaviour
{
	
	public Image UIImage;
	public bool hasUIImage;
	
	public ParticleSystem sweatParticles;
	public GameObject root;
	public Camera camera;
	
	public bool sweatFlag;
	
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
    }
	
	public void adjustForEnergyLevel(float energy){
		if(energy < 60f){
			if(!sweatFlag){
				sweatFlag = true;
				sweatParticles.Play();
			}
			else{
				float sweatScale = 1f - energy/100f;
				sweatParticles.transform.localScale = Vector3.one * sweatScale;
			}
		}
		if(hasUIImage){
			UIImage.transform.localScale = new Vector3(energy*.01f, 1f, 1f);
			UIImage.color = Color.Lerp(Color.red, Color.green, energy/120f);
		}
	}
	
	public void setUIImage(Image img){
		UIImage = img;
		hasUIImage = true;
	}
	

	public void init(){
		sweatFlag = false;
		sweatParticles.Stop();
		//hasUIImage = false;
	}
	
	
}
