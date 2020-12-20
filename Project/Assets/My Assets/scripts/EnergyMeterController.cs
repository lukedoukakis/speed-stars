using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMeterController : MonoBehaviour
{
	
	float energy;
	public Image UIImage;
	public bool hasUIImage;
	float UIAlpha;
	
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
		if(energy < 100f){
			if(hasUIImage){
				UIImage.transform.localScale = new Vector3(energy*.01f, 1f, 1f);
				Color c = Color.Lerp(Color.red, Color.green, energy/120f);
				if(energy < 37f){
					UIAlpha = Mathf.PingPong(Time.time*3f, 1f);
					c.a = UIAlpha;
				}
			
				UIImage.color = c;
			}
		}
    }
	
	public void adjustForEnergyLevel(float _energy){
		energy = _energy;
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
	}
	
	public void setUIImage(Image img){
		UIImage = img;
		hasUIImage = true;
	}
	

	public void init(){
		sweatFlag = false;
		sweatParticles.Stop();
		energy = 100f;
		UIAlpha = 1f;
		if(hasUIImage){
			UIImage.transform.localScale = Vector3.one;
			UIImage.color = Color.green;
		}
		//hasUIImage = false;
	}
	
	
}
