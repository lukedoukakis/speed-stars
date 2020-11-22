using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
	
	[SerializeField] Material[] skyboxMaterials;
	[SerializeField] Color[] ambientColors;
	[SerializeField] Color[] trackColors;
	[SerializeField] Transform[] lightPositions;
	[SerializeField] float[] lightIntensities;
	
	[SerializeField] Material trackMaterial;
	[SerializeField] Light light;
	
	
	public static int SUNNY = 0;
	public static int PINK = 1;
	public static int OVERCAST = 2;
	public static int NIGHT = 3;
	public static int DUSK = 4;
	
	int currentTheme;
	
	
	public void init(int theme){
		currentTheme = theme;
		setTheme(currentTheme);
	}
	
	public void nextTheme(){
		currentTheme++;
		if(currentTheme >= skyboxMaterials.Length){ currentTheme = 0; }
		setTheme(currentTheme);
	}
	
	public void setTheme(int theme){
		setSkybox(theme);
		setTrackColor(theme);
		setLightPosition(theme);
		setLightIntensity(theme);
		PlayerPrefs.SetInt("Theme", theme);
	}
	void setSkybox(int theme){
		RenderSettings.skybox = skyboxMaterials[theme];
	}
	void setAmbientLight(int theme){
		RenderSettings.ambientLight = ambientColors[theme];
	}
	void setTrackColor(int theme){
		trackMaterial.color = trackColors[theme];
	}
	void setLightPosition(int theme){
		light.transform.position = lightPositions[theme].position;
	}
	void setLightIntensity(int theme){
		light.intensity = lightIntensities[theme];
	}
	
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
