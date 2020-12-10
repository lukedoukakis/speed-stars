using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryButtonController : MonoBehaviour
{
	
	public LeaderboardManager lbm;
	public Button button;
	public Image image;
	
	public int raceEvent;
	public string pfid;
	public int position;
	public string name;
	public float score;
	
	
	
	public void init(LeaderboardManager _lbm, int _raceEvent, string _pfid, int _position, string _name, float _score){
		this.lbm = _lbm;
		this.button = GetComponent<Button>();
		this.image = GetComponent<Image>();
		
		this.raceEvent = _raceEvent;
		this.pfid = _pfid;
		this.position = _position;
		this.name = _name;
		this.score = _score;
		transform.Find("Text_Placing").gameObject.GetComponent<Text>().text = (position+1).ToString();
		transform.Find("Text_Username").gameObject.GetComponent<Text>().text = name;
		transform.Find("Text_Score").gameObject.GetComponent<Text>().text = score.ToString("F3");
	}
	
	public void downloadRacer(){
		StartCoroutine(lbm.downloadRacer(this.raceEvent, this.pfid));
		image.color = new Color(.37f, .37f, 1f); 
	}
	
	public void showTooltip(){
		TooltipController ttc = lbm.tooltipController;
		string tooltipText = "<color=yellow>Click to download ghost</color>";
		ttc.show(-1f);
		ttc.setText(tooltipText);
	}
	
	public void hideTooltip(){
		lbm.tooltipController.hide();
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
