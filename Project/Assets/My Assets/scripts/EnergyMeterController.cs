using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeterController : MonoBehaviour
{
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
	}
	

	public void init(){
		sweatFlag = false;
		sweatParticles.Stop();
	}
	
	
}
