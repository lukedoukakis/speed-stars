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

	public static string getRandomName(){
		string name = "";
		
		int index;
		
		// webGL
		//reader = new StreamReader("Assets/Resources/txt_first_names.txt");
		
		// standalone
		reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "txt_first_names.txt"));
		index = Random.Range(0, 2943);
		for(int i = 0; i < index; i++){
			reader.ReadLine();
		}
		name += reader.ReadLine();
		
		// webGL;
		//reader = new StreamReader("Assets/Resources/txt_last_names.txt");
		
		// standalone;
		reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "txt_last_names.txt"));
		index = Random.Range(0, 2000);
		for(int i = 0; i < index; i++){
			reader.ReadLine();
		}
		name += " " + reader.ReadLine();
	
		return name;
	}
}
