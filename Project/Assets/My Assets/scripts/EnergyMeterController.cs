using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeterController : MonoBehaviour
{
	public GameObject energyMeter;
		public GameObject energyBar;
	public Material energyBarMaterial;
	public ParticleSystem sweatParticles;
	public GameObject root;
	public Camera camera;
	
	Renderer r;
	
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        //energyMeter.transform.rotation = gyro.transform.rotation;
		updateOrientation();
    }
	
	public void setEnergyLevel(float energy){
		Vector3 scale = energyBar.transform.localScale;
		energyBar.transform.localScale = new Vector3(scale.x, scale.y, (energy*.01f));
		updateColor(energy);
		
		if(energy < 60f){
			sweatParticles.Play();
			float sweatScale = 1f - energy/100f;
			sweatParticles.transform.localScale = Vector3.one * sweatScale;
		}
	}
	
	public void updateOrientation(){
		if(camera != null){
			energyMeter.transform.LookAt(camera.transform.position);
			energyMeter.transform.Rotate(transform.up, -90f);
			Vector3 pos = root.transform.position;
			pos.y = 2f;
			energyMeter.transform.position = pos;
		}
	}
	
	public void updateColor(float energy){
		float h = .136f * (energy/100f);
		r.sharedMaterial.color = Color.HSVToRGB(h, 1f, 1f);
	}
	
	public void show(){
		StartCoroutine(snazzyPopup());
		// -----------------
		IEnumerator snazzyPopup(){
			Transform meterTrans = energyMeter.transform;
			energyMeter.SetActive(true);
			// -----------------
			meterTrans.localScale = Vector3.right;
			Vector3 temp = meterTrans.localScale;
			float lerpSpeed = 30f;
			while(temp.z < .9f){
				temp.z = Mathf.Lerp(temp.z, 1f, lerpSpeed * Time.deltaTime);
				temp.y = Mathf.Lerp(temp.y, .7f, lerpSpeed * Time.deltaTime);
				meterTrans.localScale = temp;
				yield return null;
			}
			meterTrans.localScale = new Vector3(1f, .7f, 1f);
			while(temp.z > .2f){
				temp.z = Mathf.Lerp(temp.z, .3f, lerpSpeed * Time.deltaTime);
				temp.y = Mathf.Lerp(temp.y, .3f, lerpSpeed * Time.deltaTime);
				meterTrans.localScale = temp;
				yield return null;
			}
			meterTrans.localScale = new Vector3(1f, .3f, .3f);
		}
	}
	
	public void hide(){
		StartCoroutine(snazzyHide());
		// -----------------
		IEnumerator snazzyHide(){
			Transform meterTrans = energyMeter.transform;
			// -----------------
			Vector3 temp = meterTrans.localScale;
			while(temp.z != 0f){
				temp.z = Mathf.Lerp(temp.z, 0f, 30f * Time.deltaTime);
				temp.y = Mathf.Lerp(temp.y, 0f, 30f * Time.deltaTime);
				meterTrans.localScale = temp;
				yield return null;
			}
			energyMeter.SetActive(true);
		}
		
	}
	
	public void init(){
		camera = null;
		r = energyBar.transform.Find("Cube").gameObject.GetComponent<Renderer>();
		r.sharedMaterial = Instantiate(energyBarMaterial);
		
		sweatParticles.Stop();
	}
	
	
}
