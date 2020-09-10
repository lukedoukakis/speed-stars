using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextReader : MonoBehaviour
{
	
	static StreamReader reader;
	
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public static string getRacerName(int preset){
		string name = "";
		
		if(preset == PlayerAttributes.ATTRIBUTES_RANDOM){
			int index;
			reader = new StreamReader("Assets/Resources/txt_first_names.txt");
			index = Random.Range(0, 2943);
			for(int i = 0; i < index; i++){
				reader.ReadLine();
			}
			name += reader.ReadLine();
		
			reader = new StreamReader("Assets/Resources/txt_last_names.txt");
			index = Random.Range(0, 2000);
			for(int i = 0; i < index; i++){
				reader.ReadLine();
			}
			name += " " + reader.ReadLine();
		}
		
		else{
			setReader(preset);
			string s = "";
			while(s.Split('=')[0] != "racerName"){
				s = reader.ReadLine();
			}
			name = s.Split('=')[1];
		}
		
		
		
		
		
		return name;
	}
	
	
	static void setReader(int preset){
		if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_USAINBOLT){
			reader = new StreamReader("Assets/Resources/txt_attributes_usainbolt.txt");
		}
		else if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_MICHAELJOHNSON){
			reader = new StreamReader("Assets/Resources/txt_attributes_michaeljohnson.txt");
		}
		else if(preset == PlayerAttributes.ATTRIBUTES_LEGEND_YOHANBLAKE){
			reader = new StreamReader("Assets/Resources/txt_attributes_yohanblake.txt");
		}
	}
	
	public static string getAttribute(int preset, string attributeName){
		setReader(preset);
		string s = "";
		while(s.Split('=')[0] != attributeName){
			s = reader.ReadLine();
		}
		return s.Split('=')[1];
	}
}
